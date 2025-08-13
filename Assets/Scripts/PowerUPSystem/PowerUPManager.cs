using EventSystem;
using PoolSystem;
using SO;
using SoundSystem;
using GameSystem;
using System.Collections;
using UnityEngine;

namespace PowerUpSystem {
    public class PowerUpManager : MonoBehaviour {
        public static PowerUpManager Instance { get; private set; }

        [Header("Power Ups")]
        [SerializeField] private float healAmount = 0.05f;
        [SerializeField] private float fullHealth = 1f;
        [SerializeField] private float coolDownTime = 1f;

        [SerializeField, Range(0f, 1f)] private float uniqueLetterChances = 0.5f;

        [SerializeField] private PoolItemSO sfxSourceSO;
        [SerializeField] private AudioClip usePowerUpSFX;
        private GameObject currentAudioSource;
        private AudioClip lastPlayedClip;

        private string uniqueLetters = "ABCDEFGHILMOPRSTU";
        private string letters = "JKNQVWXYZ";

        private bool canUsePowerUp = true;

        private GameState gameState;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("PowerUpManager: Instance Already Exists!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnPowerUpUsed_1>(OnPowerUpUsed1);
            EventBus.Subscribe<EVT_OnPowerUpUsed_2>(OnPowerUpUsed2);
            EventBus.Subscribe<EVT_OnPowerUpUsed_3>(OnPowerUpUsed3);

            EventBus.Subscribe<EVT_OnGameStateChange>(OnGameStateChange);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnPowerUpUsed_1>(OnPowerUpUsed1);
            EventBus.Unsubscribe<EVT_OnPowerUpUsed_2>(OnPowerUpUsed2);
            EventBus.Unsubscribe<EVT_OnPowerUpUsed_3>(OnPowerUpUsed3);

            EventBus.Unsubscribe<EVT_OnGameStateChange>(OnGameStateChange);
        }


        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================

        private void OnGameStateChange(EVT_OnGameStateChange evt) => gameState = evt.state;

        private void OnPowerUpUsed1(EVT_OnPowerUpUsed_1 evt) {
            if (gameState != GameState.PLAYING) return;

            PlaySFX(usePowerUpSFX);
            HandlePowerUP(evt.PowerUp);
        }
        private void OnPowerUpUsed2(EVT_OnPowerUpUsed_2 evt) {
            if (gameState != GameState.PLAYING) return;

            PlaySFX(usePowerUpSFX);
            HandlePowerUP(evt.PowerUp);
        }
        private void OnPowerUpUsed3(EVT_OnPowerUpUsed_3 evt) {
            if (gameState != GameState.PLAYING) return;

            PlaySFX(usePowerUpSFX);
            HandlePowerUP(evt.PowerUp);
        }

        private void HandlePowerUP(PowerUpSO powerUp) {
            if (!canUsePowerUp) return;

            switch (powerUp.powerUpName) {
                case "COFFEE":
                    EventBus.Publish(new EVT_OnCOFFEEPowerUpEffect(healAmount));
                    Debug.Log("Power-up used: COFFEE");
                    break;
                case "DEBUG":
                    EventBus.Publish(new EVT_OnDEBUGPowerUpEffect(powerUp));
                    Debug.Log("Power-up used: DEBUG");
                    break;
                case "REFACTOR":
                    EventBus.Publish(new EVT_OnREFACTORPowerUpEffect(powerUp));
                    Debug.Log("Power-up used: REFACTOR");
                    break;
                case "PUSH":
                    EventBus.Publish(new EVT_OnPUSHPowerUpEffect(powerUp));
                    Debug.Log("Power-up used: PUSH");
                    break;
                case "PULL":
                    EventBus.Publish(new EVT_OnPULLPowerUpEffect(powerUp));
                    Debug.Log("Power-up used: PULL");
                    break;
                case "COMMIT":
                    EventBus.Publish(new EVT_OnCOMMITPowerUpEffect(powerUp, fullHealth));
                    Debug.Log("Power-up used: COMMIT");
                    break;
                //case "MERGE":
                //    Debug.Log("Power-up used: MERGE");
                //    break;
                case "CHATGPT":
                    EventBus.Publish(new EVT_OnCHATGPTPowerUpEffect(powerUp, CHATGPTPowerUP()));
                    Debug.Log("Power-up used: CHATGPT");
                    break;
                default:
                    break;
            }

            StartCoroutine(CoolDownRoutine());
        }

        private IEnumerator CoolDownRoutine() {
            canUsePowerUp = false;
            yield return new WaitForSeconds(coolDownTime);
            canUsePowerUp = true;
        }

        private string CHATGPTPowerUP() {
            char randomLetter;

            if (Random.value < uniqueLetterChances) {
                randomLetter = uniqueLetters[Random.Range(0, uniqueLetters.Length)];
            }
            else {
                randomLetter = letters[Random.Range(0, letters.Length)];
            }

            return randomLetter.ToString();
        }

        private void PlaySFX(AudioClip clip) {
            if (lastPlayedClip == clip && currentAudioSource != null) {
                AudioSource existingSource = currentAudioSource.GetComponent<AudioSource>();
                if (existingSource != null && existingSource.isPlaying) {
                    return;
                }
            }

            currentAudioSource = PoolRuntimeSystem.Instance.SpawnFromPool(sfxSourceSO.name, transform.position);
            SFXSource sfxSource = currentAudioSource.GetComponent<SFXSource>();

            if (sfxSource != null) {
                sfxSource.PlayClip(clip);
                PoolRuntimeSystem.Instance.StartCoroutine(ResetAudioSource(sfxSource));
                lastPlayedClip = clip;
            }
            else {
                Debug.LogWarning("No SFX Source found");
            }
        }

        private IEnumerator ResetAudioSource(SFXSource sfxSource) {
            yield return new WaitUntil(sfxSource.IsDone);
            currentAudioSource = null;
        }
    }
}