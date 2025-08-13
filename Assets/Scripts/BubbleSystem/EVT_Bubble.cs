namespace BubbleSystem {
    public class EVT_OnBubblePop {
        public int score;

        public EVT_OnBubblePop(int score) {
            this.score = score;
        }
    }

    public class EVT_OnBubbleInBoundary {
        public float healthDamage;

        public EVT_OnBubbleInBoundary(float healthDamage) {
            this.healthDamage = healthDamage;
        }
    }

    public class EVT_OnBubbleFreeze {
        public bool isfreezed;

        public EVT_OnBubbleFreeze(bool isfreezed) {
            this.isfreezed = isfreezed;
        }
    }
}