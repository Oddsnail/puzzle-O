using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
	public static CharacterManager instance { get; private set; }

	private Dictionary<string, CHARACTER> characters = new();
	[SerializeField] private CharacterConfigSO configSO = null;
	[SerializeField] public RectTransform _characterLayer = null;
	public RectTransform characterLayer => _characterLayer;

	private void Awake() {
		instance = this;
	}

	public bool HasCharacter(string ID) => characters.ContainsKey(ID.ToLower());
	public CharacterConfigData GetInfo(string ID) {
		if (characters.ContainsKey(ID)) return characters[ID].config;
		Debug.LogWarning("[WARNING] : no such character exists in CharacterManager.");
		return null;
	}

	public CHARACTER GetCharacter(string ID) {
		if (characters.ContainsKey(ID)) return characters[ID.ToLower()];
		Debug.LogWarning("[WARNING] : no such character exists in CharacterManager.");
		return null;
	}

	public CHARACTER AddCharacter(string ID) {
		if (characters.ContainsKey(ID)) {
			Debug.LogWarning($"[WARNING] : Character with ID '{ID}' cannot added since already exists.");
			return characters[ID.ToLower()]; 
		}
		CHARACTER character = new(ID, configSO.GetConfig(ID));
		characters.Add(ID, character);
		Debug.Log($"character {ID} added to dictionary");
		return character;
	}

	public CHARACTER AddClient(string ID) {
		if (characters.ContainsKey(ID + "-client")) {
			Debug.LogWarning($"[WARNING] : Character with ID '{ID}-client' cannot added since already exists.");
			return null; 
		}
		CHARACTER character = new(ID, configSO.GetConfig(ID), true);
		characters.Add(ID + "-client", character);
		Debug.Log($"character {ID}-client added to dictionary");
		return character;
	}

	public void RemoveCharacter(string ID) {
		CHARACTER character = characters[ID];

		if (character != null) {
			Debug.LogWarning($"[WARNING] : Character with ID '{ID}' cannot deleted since does not exists.");
			return; 
		}

		Destroy(character.rectTransform.gameObject);
		characters.Remove(ID);
	}
}
