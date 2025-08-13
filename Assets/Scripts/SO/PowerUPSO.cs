using UnityEngine;

namespace SO {
    [CreateAssetMenu(fileName = "PowerUpSO", menuName = "ScriptableObjects/PowerUpSO")]
    public class PowerUpSO : ScriptableObject {
        public string powerUpName;
        public Sprite powerUpSpriteIcon;
        public float powerUpDuration;
	}
}