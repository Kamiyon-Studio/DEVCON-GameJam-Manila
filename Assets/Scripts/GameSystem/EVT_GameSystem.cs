namespace GameSystem {

    public class EVT_OnGameStateChange {
        public GameState state { get; private set; }

        public EVT_OnGameStateChange(GameState state) {
            this.state = state;
        }
    }

    public class  EVT_OnGravityChange {
        public Gravity gravity { get; private set; }

        public EVT_OnGravityChange(Gravity gravity) {
            this.gravity = gravity;
        }
    }

    public class EVT_OnHealthChange {
        public float health;

        public EVT_OnHealthChange(float health) {
            this.health = health;
        }
    }

    public class EVT_OnComboChange {
        public int combo;

        public EVT_OnComboChange(int combo) {
            this.combo = combo;
        }
    }

    public class EVT_OnLetterSpeedChange {
        public float speed;

        public EVT_OnLetterSpeedChange(float speed) {
            this.speed = speed;
        }
    }

    public class EVT_OnLetterIntervalChange {
        public float interval;

        public EVT_OnLetterIntervalChange(float interval) {
            this.interval = interval;
        }
    }

    public class EVT_OnWordSpeedChange {
        public float speed;

        public EVT_OnWordSpeedChange(float speed) {
            this.speed = speed;
        }
    }

    public class EVT_OnWordIntervalChange {
        public float interval;

        public EVT_OnWordIntervalChange(float interval) {
            this.interval = interval;
        }
    }
}