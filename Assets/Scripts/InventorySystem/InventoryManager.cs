using GameSystem;
using EventSystem;
using InputSystem;
using PowerUpSystem;
using SO;
using UnityEngine;

namespace InventorySystem {
    public class InventoryManager : MonoBehaviour {
        public static InventoryManager Instance { get; private set; }

        private readonly PowerUpSO[] _powerUpSlots = new PowerUpSO[3];

        private GameState gameState;
        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("InventoryManager: Instance Already Exists!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable() {
            EventBus.Subscribe<EVT_OnGameStateChange>(OnGameStateChange);

            EventBus.Subscribe<EVT_OnPowerUpCollected>(OnPowerUpCollected);
            EventBus.Subscribe<EVT_OnInventory1Click>(OnInventory1Click);
            EventBus.Subscribe<EVT_OnInventory2Click>(OnInventory2Click);
            EventBus.Subscribe<EVT_OnInventory3Click>(OnInventory3Click);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<EVT_OnGameStateChange>(OnGameStateChange);

            EventBus.Unsubscribe<EVT_OnPowerUpCollected>(OnPowerUpCollected);
            EventBus.Unsubscribe<EVT_OnInventory1Click>(OnInventory1Click);
            EventBus.Unsubscribe<EVT_OnInventory2Click>(OnInventory2Click);
            EventBus.Unsubscribe<EVT_OnInventory3Click>(OnInventory3Click);
        }


        // =====================================================================
        //
        //                          Event Methods
        //
        // =====================================================================

        private void OnGameStateChange(EVT_OnGameStateChange evt) { gameState = evt.state; }

        private void OnPowerUpCollected(EVT_OnPowerUpCollected evt) {
            for (var i = 0; i < _powerUpSlots.Length; i++) {
                if (_powerUpSlots[i] != null) continue;
                _powerUpSlots[i] = evt.PowerUp;
                EventBus.Publish(new EVT_OnPowerUpStored(i, evt.PowerUp));
                Debug.Log(evt.PowerUp.powerUpName + $" collected at index {i}");
                return;
            }

            Debug.Log("Inventory is full. Could not collect power-up.");
        }

        private void OnInventory1Click(EVT_OnInventory1Click evt) {
            if (_powerUpSlots[0] == null) return;

            var powerUp = _powerUpSlots[0];
            EventBus.Publish(new EVT_OnPowerUpUsed_1(powerUp));
            _powerUpSlots[0] = null;
        }

        private void OnInventory2Click(EVT_OnInventory2Click evt) {
            if (_powerUpSlots[1] == null) return;

            var powerUp = _powerUpSlots[1];
            EventBus.Publish(new EVT_OnPowerUpUsed_2(powerUp));
            _powerUpSlots[1] = null;
        }

        private void OnInventory3Click(EVT_OnInventory3Click evt) {
            if (_powerUpSlots[2] == null) return;

            var powerUp = _powerUpSlots[2];
            EventBus.Publish(new EVT_OnPowerUpUsed_3(powerUp));
            _powerUpSlots[2] = null;
        }
    }
}