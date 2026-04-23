using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace origin.character {
	public class CharacterManager : MonoBehaviour, ICharacterService {
		public static CharacterManager instance { get; private set; }

		private Dictionary<string, CHARACTER> characters = new();
		[SerializeField] private CharacterConfigSO configSO = null;
		[SerializeField] public RectTransform _characterLayer = null;
		public RectTransform characterLayer => _characterLayer;

		private void Awake() {
			if (instance != null && instance != this) {
				Debug.LogWarning($"Duplicate {GetType().Name} detected. Destroying duplicate.");
				Destroy(gameObject);
				return;
			}
			instance = this;
		}

		private void OnDestroy() {
			if (instance == this) {
				instance = null;
			}
		}

		public bool HasCharacter(string ID) => characters.ContainsKey(ID.ToLower());
		
		public CharacterConfigData GetInfo(string ID) {
			string key = ID.ToLower();
			if (characters.ContainsKey(key)) return characters[key].config;
			Debug.LogWarning("[WARNING] : no such character exists in CharacterManager.");
			return null;
		}

		public CHARACTER GetCharacter(string ID) {
			string key = ID.ToLower();
			if (characters.ContainsKey(key)) return characters[key];
			Debug.LogWarning("[WARNING] : no such character exists in CharacterManager.");
			return null;
		}

		public CHARACTER AddCharacter(string ID) {
			string key = ID.ToLower();
			if (characters.ContainsKey(key)) {
				Debug.LogWarning($"[WARNING] : Character with ID '{ID}' cannot added since already exists.");
				return characters[key];
			}
			CHARACTER character = new(ID, configSO.GetConfig(ID));
			characters.Add(key, character);
			Debug.Log($"character {ID} added to dictionary");
			return character;
		}

		public CHARACTER AddClient(string ID) {
			string key = (ID + "-client").ToLower();
			if (characters.ContainsKey(key)) {
				Debug.LogWarning($"[WARNING] : Character with ID '{ID}-client' cannot added since already exists.");
				return null;
			}
			CHARACTER character = new(ID, configSO.GetConfig(ID), true);
			characters.Add(key, character);
			Debug.Log($"character {ID}-client added to dictionary");
			return character;
		}

		public void RemoveCharacter(string ID) {
			string key = ID.ToLower();

			if (!characters.TryGetValue(key, out CHARACTER character)) {
				Debug.LogWarning($"[WARNING] : Character with ID '{ID}' cannot be deleted since it does not exist.");
				return;
			}

			character.StopAllCoroutines();

			if (character.rectTransform != null && character.rectTransform.gameObject != null) {
				Destroy(character.rectTransform.gameObject);
			}
			characters.Remove(key);
		}
	}

}
