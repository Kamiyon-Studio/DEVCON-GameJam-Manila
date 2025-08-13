using UnityEngine;

using System.Collections.Generic;

namespace SO {
	[CreateAssetMenu(fileName = "SpawnPointSO", menuName = "ScriptableObjects/SpawnPointSO")]
	public class SpawnPointsSO : ScriptableObject {
		public List<GameObject> spawnPoints;
	} 
}
