using TMPro;
using UnityEngine;

using EventSystem;
using ScoreSystem;


namespace UI {
    public class ScoreUI : MonoBehaviour {
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI scoreText;

        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            scoreText.text = "0";
        }

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnScoreIncrement>(OnScoreIncrement);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnScoreIncrement>(OnScoreIncrement);
        }


        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================

        private void OnScoreIncrement(EVT_OnScoreIncrement evt) {
            scoreText.text = $"{evt.score}";
        }
    }
}
