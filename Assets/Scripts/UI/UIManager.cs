using UnityEngine;

using GameSystem;
using EventSystem;
using PowerUpSystem;
using UnityEngine.UI;
using System.Collections;
using SO;

namespace UI {
    public class UIManager : MonoBehaviour {
        public static UIManager Instance { get; private set; }

        [Header("UI Objects")]
        [SerializeField] private GameObject WaitingUI;
        [SerializeField] private GameObject powerUpSplashUI;
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private GameObject damageOverlay;

        private GameState gameState;

        private Coroutine WaitForAnimationRoutineVar;

        private float health;
        private float lastHealth = 1f;

        private Animator damageOverlayAnimator;
        private Coroutine animationRoutine;


        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("UIManager: Instance Already Exists!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            damageOverlayAnimator = damageOverlay.GetComponent<Animator>();
        }

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnGameStateChange>(OnGameStateChange);
            EventBus.Subscribe<EVT_OnHealthChange>(OnHealthChange);

            EventBus.Subscribe<EVT_OnPowerUpUsed_1>(OnPowerUpUsed1);
            EventBus.Subscribe<EVT_OnPowerUpUsed_2>(OnPowerUpUsed2);
            EventBus.Subscribe<EVT_OnPowerUpUsed_3>(OnPowerUpUsed3);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnGameStateChange>(OnGameStateChange);
            EventBus.Unsubscribe<EVT_OnHealthChange>(OnHealthChange);

            EventBus.Unsubscribe<EVT_OnPowerUpUsed_1>(OnPowerUpUsed1);
            EventBus.Unsubscribe<EVT_OnPowerUpUsed_2>(OnPowerUpUsed2);
            EventBus.Unsubscribe<EVT_OnPowerUpUsed_3>(OnPowerUpUsed3);

        }


        private void OnGameStateChange(EVT_OnGameStateChange evt) {
            switch (evt.state) {
                case GameState.WAITING:
                    WaitingUI.SetActive(true);

                    powerUpSplashUI.SetActive(false);
                    gameOverUI.SetActive(false);
                    break;
                case GameState.COUNTDOWN:
                    WaitingUI.SetActive(false);
                    break;
                case GameState.PLAYING:
                    WaitingUI.SetActive(false);
                    break;
                case GameState.GAMEOVER:
                    gameOverUI.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        private void OnHealthChange(EVT_OnHealthChange evt) {
            health = evt.health;

            if (health < lastHealth) {
                if (animationRoutine != null) {
                    StopCoroutine(animationRoutine);
                    damageOverlay.SetActive(false);

                }

                damageOverlay.SetActive(true);
                animationRoutine = StartCoroutine(WaitForAnim(damageOverlayAnimator, damageOverlay));
            }
            lastHealth = health;
        }

        private void OnPowerUpUsed1(EVT_OnPowerUpUsed_1 evt) {
            PowerUpSO powerUp = evt.PowerUp;
            Animator anim = powerUpSplashUI.GetComponent<Animator>();

            if (anim == null) {
                Debug.LogWarning("UIManager: powerUpSplashUI Animator is null!");
                return;
            }

            if (WaitForAnimationRoutineVar != null) {
                StopCoroutine(WaitForAnimationRoutineVar);
            }


            WaitForAnimationRoutineVar = StartCoroutine(WaitForAnimationRoutine(anim, powerUp.powerUpName));
        }

        private void OnPowerUpUsed2(EVT_OnPowerUpUsed_2 evt) {
            PowerUpSO powerUp = evt.PowerUp;
            Animator anim = powerUpSplashUI.GetComponent<Animator>();

            if (anim == null) {
                Debug.LogWarning("UIManager: powerUpSplashUI Animator is null!");
                return;
            }

            if (WaitForAnimationRoutineVar != null) {
                StopCoroutine(WaitForAnimationRoutineVar);
            }

            WaitForAnimationRoutineVar = StartCoroutine(WaitForAnimationRoutine(anim, powerUp.powerUpName));
        }

        private void OnPowerUpUsed3(EVT_OnPowerUpUsed_3 evt) {
            PowerUpSO powerUp = evt.PowerUp;
            Animator anim = powerUpSplashUI.GetComponent<Animator>();


            if (anim == null) {
                Debug.LogWarning("UIManager: powerUpSplashUI Animator is null!");
                return;
            }


            if (WaitForAnimationRoutineVar != null) {
                StopCoroutine(WaitForAnimationRoutineVar);
            }

            WaitForAnimationRoutineVar = StartCoroutine(WaitForAnimationRoutine(anim, powerUp.powerUpName));
        }

        private IEnumerator WaitForAnimationRoutine(Animator animator, string triggerName) {
            // Reset all triggers to ensure clean state
            animator.ResetTrigger("COFFEE");
            animator.ResetTrigger("DEBUG");
            animator.ResetTrigger("REFACTOR");
            animator.ResetTrigger("PUSH");
            animator.ResetTrigger("PULL");
            animator.ResetTrigger("COMMIT");
            animator.ResetTrigger("CHATGPT");

            // Activate the UI
            powerUpSplashUI.SetActive(true);

            // Wait one frame so the Animator processes activation
            yield return null;

            // Set the trigger
            animator.SetTrigger(triggerName);

            // Wait until animator enters the correct state
            AnimatorStateInfo stateInfo;
            do {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }
            while (!stateInfo.IsName(triggerName));

            // Wait for the animation to finish (non-looping)
            while (stateInfo.normalizedTime < 1f) {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }

            // Hide the UI
            powerUpSplashUI.SetActive(false);
        }

        private IEnumerator WaitForAnim(Animator anim, GameObject UIObject) {
            yield return null; // wait one frame so Animator starts

            // Get current state length
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // Wait until animation finishes (normalizedTime >= 1)
            while (stateInfo.normalizedTime < 1f && !stateInfo.loop) {
                yield return null;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            }

            UIObject.SetActive(false);
            Debug.Log("Animation finished!");
        }
    }
}