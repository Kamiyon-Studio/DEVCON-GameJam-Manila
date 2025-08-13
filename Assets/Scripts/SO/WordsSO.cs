using UnityEngine;
using System.Collections.Generic;

namespace SO {
    [CreateAssetMenu(fileName = "PowerUpSOList", menuName = "ScriptableObjects/PowerUpSOList")]
    public class PowerUpSOList : ScriptableObject {
		public List<PowerUpSO> powerUpSOs;
	}
}