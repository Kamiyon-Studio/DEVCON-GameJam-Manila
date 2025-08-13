using UnityEngine;

using EventSystem;
using BubbleSystem;
using GameSystem;

namespace ScoreSystem {
	public class ScoreManager : MonoBehaviour {
		public static ScoreManager Instance { get; private set; }

		private int score = 0;
		private int combo = 0;

        [Header("Combo Animation")]
        [Tooltip("Animator that contains the ComboWin and ComboLost animations.")]
        [SerializeField] private Animator comboAnimator;

        [Tooltip("Trigger parameter name for winning combo.")]
        [SerializeField] private string triggerComboWin = "TriggerComboWin";

        [Tooltip("Trigger parameter name for losing combo.")]
        [SerializeField] private string triggerComboLost = "TriggerComboLost";

        [Tooltip("If true, attempts to find an Animator in this GameObject's children when comboAnimator is null.")]
        [SerializeField] private bool tryFindAnimatorIfNull = true;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("ScoreManager: Instance Already Exists!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnBubblePop>(OnBubblePop);
            EventBus.Subscribe<EVT_OnComboChange>(OnComboChange);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnBubblePop>(OnBubblePop);
            EventBus.Unsubscribe<EVT_OnComboChange>(OnComboChange);
        }


        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================

        private void OnBubblePop(EVT_OnBubblePop evt) {
            int scoreToAdd = ApplyMultiplier(evt.score);
            score += scoreToAdd;
            EventBus.Publish(new EVT_OnScoreIncrement(score));
        }

        private void OnComboChange(EVT_OnComboChange evt) {
            combo = evt.combo;
            EnsureAnimator();

            if (evt.combo!= 0) {
                comboAnimator.SetTrigger(triggerComboWin);
                Debug.Log($"ScoreManager: Triggered '{triggerComboWin}'");
            }
            else if (evt.combo == 0) {
                comboAnimator.SetTrigger(triggerComboLost);
                Debug.Log($"ScoreManager: Triggered '{triggerComboLost}'");
            }
        }


        // =====================================================================
        //
        //                          Score Calculation
        //
        // =====================================================================

        /// <summary>
        /// Applies a score multiplier based on the current combo.
        /// No multiplier is applied if the combo is 0 or 1.
        /// </summary>
        /// <param name="baseScore">The base score to apply the multiplier to.</param>
        /// <returns>The score with the multiplier applied.</returns>
        private int ApplyMultiplier(int baseScore) {
            float multiplier = 1f + (combo - 1) * 0.25f;
            multiplier = Mathf.Min(multiplier, 5f); // cap at 5x
            return Mathf.RoundToInt(baseScore * multiplier);
        }


        // =====================================================================
        //
        //                          Helper Methods
        //
        // =====================================================================

        private void EnsureAnimator() {
            if (comboAnimator == null && tryFindAnimatorIfNull) {
                comboAnimator = GetComponentInChildren<Animator>();
                if (comboAnimator != null) {
                    Debug.Log("ScoreManager: Auto-found an Animator in children.");
                }
            }
        }

        public void SetComboAnimator(Animator animator) {
            comboAnimator = animator;
        }

        public int GetScore() => score;
    } 
}
