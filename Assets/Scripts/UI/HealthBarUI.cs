using UnityEngine;
using UnityEngine.UI;

using EventSystem;
using GameSystem;


namespace UI {
    public class HealthBarUI : MonoBehaviour {

        [Header("Health Bar")]
        [SerializeField] private Image healthBar;

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnHealthChange>(OnHealthChange);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnHealthChange>(OnHealthChange);
        }

        private void OnHealthChange(EVT_OnHealthChange evt) {
            healthBar.fillAmount = evt.health;
        }
    }
}