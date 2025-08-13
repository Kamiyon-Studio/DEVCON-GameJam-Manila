using UnityEngine;
using UnityEngine.UI;

using EventSystem;
using InventorySystem;
using PowerUpSystem;


namespace UI {
    public class PowerUpSplashUI : MonoBehaviour {
        [SerializeField] private Image iconImage;

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnPowerUpUsed_1>(OnPowerUpUsed1);
            EventBus.Subscribe<EVT_OnPowerUpUsed_2>(OnPowerUpUsed2);
            EventBus.Subscribe<EVT_OnPowerUpUsed_3>(OnPowerUpUsed3);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnPowerUpUsed_1>(OnPowerUpUsed1);
            EventBus.Unsubscribe<EVT_OnPowerUpUsed_2>(OnPowerUpUsed2);
            EventBus.Unsubscribe<EVT_OnPowerUpUsed_3>(OnPowerUpUsed3);
        }

        private void OnPowerUpUsed1(EVT_OnPowerUpUsed_1 evt) { SetIcon(evt.PowerUp.powerUpSpriteIcon); }
        private void OnPowerUpUsed2(EVT_OnPowerUpUsed_2 evt) { SetIcon(evt.PowerUp.powerUpSpriteIcon); }
        private void OnPowerUpUsed3(EVT_OnPowerUpUsed_3 evt) { SetIcon(evt.PowerUp.powerUpSpriteIcon); }

        private void SetIcon(Sprite icon) {
            iconImage = GetComponent<Image>();

            if (iconImage != null) {
                iconImage.sprite = icon;
            }
        }
    }
}