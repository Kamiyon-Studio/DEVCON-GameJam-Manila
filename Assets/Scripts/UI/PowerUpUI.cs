using EventSystem;
using InventorySystem;
using PowerUpSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class PowerUpUI : MonoBehaviour {
        [Header("PowerUp Slots")]
        [SerializeField] private GameObject powerUpSlotOn1;
        [SerializeField] private Image powerUpSlot1Icon;
        [SerializeField] private GameObject powerUpSlotOn2;
        [SerializeField] private Image powerUpSlot2Icon;
        [SerializeField] private GameObject powerUpSlotOn3;
        [SerializeField] private Image powerUpSlot3Icon;


        private void OnEnable() {
            EventBus.Subscribe<EVT_OnPowerUpStored>(OnPowerUpStored);

            EventBus.Subscribe<EVT_OnPowerUpUsed_1>(OnPowerUpUsed1);
            EventBus.Subscribe<EVT_OnPowerUpUsed_2>(OnPowerUpUsed2);
            EventBus.Subscribe<EVT_OnPowerUpUsed_3>(OnPowerUpUsed3);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnPowerUpStored>(OnPowerUpStored);

            EventBus.Unsubscribe<EVT_OnPowerUpUsed_1>(OnPowerUpUsed1);
            EventBus.Unsubscribe<EVT_OnPowerUpUsed_2>(OnPowerUpUsed2);
            EventBus.Unsubscribe<EVT_OnPowerUpUsed_3>(OnPowerUpUsed3);
        }

        private void OnPowerUpStored(EVT_OnPowerUpStored evt) {
            switch (evt.index) {
                case 0:
                    powerUpSlotOn1.gameObject.SetActive(true);

                    if (evt.powerUpSO.powerUpSpriteIcon != null) {
                        powerUpSlot1Icon.sprite = evt.powerUpSO.powerUpSpriteIcon;
                    }
                    break;
                case 1:
                    powerUpSlotOn2.gameObject.SetActive(true);

                    if (evt.powerUpSO.powerUpSpriteIcon != null) {
                        powerUpSlot2Icon.sprite = evt.powerUpSO.powerUpSpriteIcon;
                    }
                    break;
                case 2:
                    powerUpSlotOn3.gameObject.SetActive(true);

                    if (evt.powerUpSO.powerUpSpriteIcon != null) {
                        powerUpSlot3Icon.sprite = evt.powerUpSO.powerUpSpriteIcon;
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnPowerUpUsed1(EVT_OnPowerUpUsed_1 evt) => powerUpSlotOn1.gameObject.SetActive(false);
        private void OnPowerUpUsed2(EVT_OnPowerUpUsed_2 evt) => powerUpSlotOn2.gameObject.SetActive(false);
        private void OnPowerUpUsed3(EVT_OnPowerUpUsed_3 evt) => powerUpSlotOn3.gameObject.SetActive(false);
    }
}