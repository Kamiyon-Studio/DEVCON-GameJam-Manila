using SO;

namespace PowerUpSystem {
    public class EVT_OnPowerUpCollected {
        public PowerUpSO PowerUp;

        public EVT_OnPowerUpCollected(PowerUpSO powerUp) {
            PowerUp = powerUp;
        }
    }

    public class EVT_OnPowerUpUsed_1 {
        public PowerUpSO PowerUp;

        public EVT_OnPowerUpUsed_1(PowerUpSO powerUp) {
            PowerUp = powerUp;
        }
    }

    public class EVT_OnPowerUpUsed_2 {
        public PowerUpSO PowerUp;

        public EVT_OnPowerUpUsed_2(PowerUpSO powerUp) {
            PowerUp = powerUp;
        }
    }

    public class EVT_OnPowerUpUsed_3 {
        public PowerUpSO PowerUp;

        public EVT_OnPowerUpUsed_3(PowerUpSO powerUp) {
            PowerUp = powerUp;
        }
    }


    // Power Up Effects Events
    public class EVT_OnDEBUGPowerUpEffect {
        public PowerUpSO PowerUp;

        public EVT_OnDEBUGPowerUpEffect(PowerUpSO powerUp) {
            PowerUp = powerUp;
        }
    }

    public class EVT_OnCOFFEEPowerUpEffect {
        public PowerUpSO PowerUp;
        public float healAmount;

        public EVT_OnCOFFEEPowerUpEffect(float healAmount) {
            this.healAmount = healAmount;
        }
    }

    public class EVT_OnREFACTORPowerUpEffect {
        public PowerUpSO PowerUp;

        public EVT_OnREFACTORPowerUpEffect(PowerUpSO powerUp) {
            PowerUp = powerUp;
        }
    }

    public class EVT_OnPUSHPowerUpEffect {
        public PowerUpSO PowerUp;

        public EVT_OnPUSHPowerUpEffect(PowerUpSO powerUp) {
            PowerUp = powerUp;
        }
    }

    public class EVT_OnPULLPowerUpEffect {
        public PowerUpSO PowerUp;

        public EVT_OnPULLPowerUpEffect(PowerUpSO powerUp) {
            PowerUp = powerUp;
        }
    }

    public class EVT_OnCOMMITPowerUpEffect {
        public PowerUpSO PowerUp;
        public float fullHealth;

        public EVT_OnCOMMITPowerUpEffect(PowerUpSO powerUp, float fullHealth) {
            PowerUp = powerUp;
            this.fullHealth = fullHealth;
        }
    }

    public class EVT_OnCHATGPTPowerUpEffect {
        public PowerUpSO PowerUp;
        public string letter;

        public EVT_OnCHATGPTPowerUpEffect(PowerUpSO powerUp, string letter) {
            PowerUp = powerUp;
            this.letter = letter;
        }
    }
}
