using UnityEngine;

namespace origin.character {
	[System.Serializable]

	public class CharacterConfigData {
		public string ID;
		public string displayName;
		public string displaySubname;
		public GameObject prefabNormal;
		public GameObject prefabClient;
		
		public static CharacterConfigData Default {
			get {
				return new() {
					ID = "<name>",
					displayName = "<displayName>",
					displaySubname = null,
					prefabNormal = null,
				};
			}
		}

		public string displayString => $"{displayName} <size=70%><color=#BBBBBB>{displaySubname}</color></size>";
	}
}