using TMPro;
using UnityEngine;

using EventSystem;
using GameSystem;

namespace UI {
    public class ComboUI : MonoBehaviour {
        [Header("Combo Text")]
        [SerializeField] private TextMeshProUGUI comboText;

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnComboChange>(OnComboChange);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnComboChange>(OnComboChange);
        }

        private void Start() {
            

            comboText.text = "0x";
        }

        private void OnComboChange(EVT_OnComboChange evt) {
            comboText.text = $"{evt.combo}x";
        }
    }
}