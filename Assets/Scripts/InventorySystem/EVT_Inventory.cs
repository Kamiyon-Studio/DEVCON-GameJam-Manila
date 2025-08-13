using SO;

namespace InventorySystem {
    public class EVT_OnPowerUpStored {
        public int index;
        public PowerUpSO powerUpSO;

        public EVT_OnPowerUpStored(int index, PowerUpSO powerUp) {
            this.index = index;
            powerUpSO = powerUp;
        }
    }
}