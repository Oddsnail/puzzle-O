using UnityEngine;

namespace origin.character {
	[CreateAssetMenu(fileName = "Character Configurations", menuName = "Scriptable Objects/Character Configurations")]
	public class CharacterConfigSO : ScriptableObject {
		public CharacterConfigData[] Characters;

		public CharacterConfigData GetConfig(string characterID) {
			characterID = characterID.ToLower();

			for (int i = 0; i < Characters.Length; i++) {
				if (string.Equals(characterID, Characters[i].ID.ToLower())) return Characters[i];
			}
			
			return CharacterConfigData.Default;
		}
	}
}