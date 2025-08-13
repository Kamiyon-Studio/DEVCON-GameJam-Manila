using EventSystem;
using GameSystem;
using PoolSystem;
using PowerUpSystem;
using SO;
using SoundSystem;
using System.Collections;
using UnityEngine;

namespace UI {
    public class FaceUI : MonoBehaviour {


        [Header("SFX source SO")]
        [SerializeField] private PoolItemSO sfxSourceSO;
        [SerializeField] private float volume = 1f;
        [Header("SFX")]
        [SerializeField] private AudioClip hurtSFX;

        private GameObject currentAudioSource;
        private AudioClip lastPlayedClip;

        private Animator animator;
        private bool isPlayingAnim;
        private bool firstInit = true;

        private Coroutine animationRoutine;
        private float health;
        private float lastHealth = 1f;

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnHealthChange>(OnHealthChange);

            EventBus.Subscribe<EVT_OnCOFFEEPowerUpEffect>(OnCOFFEEPowerUpEffect);
            EventBus.Subscribe<EVT_OnDEBUGPowerUpEffect>(OnDEBUGPowerUpEffect);
            EventBus.Subscribe<EVT_OnREFACTORPowerUpEffect>(OnREFACTORPowerUpEffect);
            EventBus.Subscribe<EVT_OnPUSHPowerUpEffect>(OnPUSHPowerUpEffect);
            EventBus.Subscribe<EVT_OnPULLPowerUpEffect>(OnPullPowerUpEffect);
            EventBus.Subscribe<EVT_OnCOMMITPowerUpEffect>(OnCommitPowerUp);
            EventBus.Subscribe<EVT_OnCHATGPTPowerUpEffect>(OnCHATGPTPowerUpEffect);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnHealthChange>(OnHealthChange);

            EventBus.Unsubscribe<EVT_OnCOFFEEPowerUpEffect>(OnCOFFEEPowerUpEffect);
            EventBus.Unsubscribe<EVT_OnDEBUGPowerUpEffect>(OnDEBUGPowerUpEffect);
            EventBus.Unsubscribe<EVT_OnREFACTORPowerUpEffect>(OnREFACTORPowerUpEffect);
            EventBus.Unsubscribe<EVT_OnPUSHPowerUpEffect>(OnPUSHPowerUpEffect);
            EventBus.Unsubscribe<EVT_OnPULLPowerUpEffect>(OnPullPowerUpEffect);
            EventBus.Unsubscribe<EVT_OnCOMMITPowerUpEffect>(OnCommitPowerUp);
            EventBus.Unsubscribe<EVT_OnCHATGPTPowerUpEffect>(OnCHATGPTPowerUpEffect);
        }

        private void OnHealthChange(EVT_OnHealthChange evt) {
            health = evt.health;

            if (firstInit) {
                firstInit = false;
                HandleFaceAnim(health, skipSFX: true);
            }
            else {
                HandleFaceAnim(health, skipSFX: false);
            }
        }

        private void OnCOFFEEPowerUpEffect(EVT_OnCOFFEEPowerUpEffect evt) {
            isPlayingAnim = true;

            if (animationRoutine != null) StopCoroutine(animationRoutine);
            animator.Play("happy_anim");
            animationRoutine = StartCoroutine(WaitForAnim(animator));
        }

        private void OnDEBUGPowerUpEffect(EVT_OnDEBUGPowerUpEffect evt) {
            isPlayingAnim = true;

            if (animationRoutine != null) StopCoroutine(animationRoutine);
            animator.Play("pump_anim");
            animationRoutine = StartCoroutine(WaitForAnim(animator));
        }

        private void OnREFACTORPowerUpEffect(EVT_OnREFACTORPowerUpEffect evt) {
            isPlayingAnim = true;

            if (animationRoutine != null) StopCoroutine(animationRoutine);
            animator.Play("happy_anim");
            animationRoutine = StartCoroutine(WaitForAnim(animator));
        }

        private void OnPUSHPowerUpEffect(EVT_OnPUSHPowerUpEffect evt) {
            isPlayingAnim = true;

            if (animationRoutine != null) StopCoroutine(animationRoutine);
            animator.Play("pump_anim");
            animationRoutine = StartCoroutine(WaitForAnim(animator));
        }

        private void OnPullPowerUpEffect(EVT_OnPULLPowerUpEffect evt) {
            isPlayingAnim = true;

            if (animationRoutine != null) StopCoroutine(animationRoutine);
            animator.Play("pump_anim");
            animationRoutine = StartCoroutine(WaitForAnim(animator));
        }

        private void OnCommitPowerUp(EVT_OnCOMMITPowerUpEffect evt) {
            isPlayingAnim = true;

            if (animationRoutine != null) StopCoroutine(animationRoutine);
            animator.Play("happy_anim");
            animationRoutine = StartCoroutine(WaitForAnim(animator));
        }

        private void OnCHATGPTPowerUpEffect(EVT_OnCHATGPTPowerUpEffect evt) {
            isPlayingAnim = true;

            if (animationRoutine != null) StopCoroutine(animationRoutine);
            animator.Play("pump_anim");
            animationRoutine = StartCoroutine(WaitForAnim(animator));
        }

        private IEnumerator WaitForAnim(Animator anim) {
            yield return null; // wait one frame so Animator starts

            // Get current state length
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // Wait until animation finishes (normalizedTime >= 1)
            while (stateInfo.normalizedTime < 1f && !stateInfo.loop) {
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            }

            isPlayingAnim = false;

            HandleFaceAnim(health);
            Debug.Log("Animation finished!");
        }

        private void HandleFaceAnim(float health, bool skipSFX = false) {
            if (isPlayingAnim) return;

            if (health >= 0.7f) {
                animator.Play("idle_anim");
            }
            else if (health <= 0.7f && health >= 0.3f) {
                animator.Play("lockin_anim");
            }
            else if (health < 0.3f) {
                animator.Play("frustrated_anim");
            }

            if (!skipSFX && health < lastHealth) {
                PlaySFX(hurtSFX);
            }

            lastHealth = health;
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
                sfxSource.SetVolume(volume);
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
