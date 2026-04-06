using UnityEngine;

namespace origin.character {
	[System.Serializable]

	public class CharacterConfigData {
		public string ID;
		public string nameKey => $"char.{ID}.name";
		public string subnameKey => $"char.{ID}.subname";
		public GameObject prefabNormal;
		public GameObject prefabClient;

		public static CharacterConfigData Default {
			get {
				return new() {
					ID = "<default>",
					prefabNormal = null,
					prefabClient = null,
				};
			}
		}
	}
}
