using UnityEngine;
using origin.language;

namespace origin.character {
	[System.Serializable]

	public class CharacterConfigData {
		public string ID;
		public string displayName => $"@char.{ID}.name";
		public string displaySubname => $"@char.{ID}.subname";
		public GameObject prefabNormal;
		public GameObject prefabClient;
		
		public static CharacterConfigData Default {
			get {
				return new() {
					ID = "<name>",
					prefabNormal = null,
					prefabClient = null,
				};
			}
		}

		public string localizedDisplayName() {
			string localName = LocalizationManager.Instance.Resolve(displayName);
			string localSubname = LocalizationManager.Instance.Resolve(displaySubname);
			return $"{localName} <size=70%><color=#BBBBBB>{localSubname}</color></size>";
		}
	}
}