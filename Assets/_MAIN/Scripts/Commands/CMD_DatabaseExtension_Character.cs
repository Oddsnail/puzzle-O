using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using origin.character;

namespace origin.command {

	public class CMD_DatabaseExtension_Character : CMD_DatabaseExtension {

		new public static void Extend(CommandDatabase database) {
			database.AddCommand("appear", new Func<string[], IEnumerator>(Appear));
			database.AddCommand("disappear", new Func<string[], IEnumerator>(Disappear));
			database.AddCommand("setposX", new Func<string[], IEnumerator>(MoveX));
			database.AddCommand("setposY", new Func<string[], IEnumerator>(MoveY));

			database.AddCommand("h", new Func<string, IEnumerator>(Hop));			// hop
			database.AddCommand("c", new Func<string, IEnumerator>(Crouch));		// crouch
			database.AddCommand("s", new Func<string, IEnumerator>(Shiver));		// shiver
			database.AddCommand("sp", new Action<string[]>(Sprite));    			// setSprite
			database.AddCommand("ix", new Action<string>(InvertX));     			// invertX

			database.AddCommand("hl", new Func<string[], IEnumerator>(Highlight));
			database.AddCommand("uhl", new Func<string[], IEnumerator>(UnHighlight));
		}

		private static IEnumerator Appear(string[] data) {
			ICharacterService chars = CharacterManager.instance;
			List<CHARACTER> characters = new();

			foreach (string name in data) {
				CHARACTER character = chars.GetCharacter(name);
				if (character == null) {
					Debug.LogError($"[ERROR] Character '{name}' not found for 'appear' command.");
					continue;
				}
				characters.Add(character);
			}

			if (characters.Count == 0) yield break;

			foreach (CHARACTER character in characters) {
				character.Appear();
			}

			while (characters.Any(c => c.isAppearing)) yield return null;
		}

		private static IEnumerator Disappear(string[] data) {
			ICharacterService chars = CharacterManager.instance;
			List<CHARACTER> characters = new();

			foreach (string name in data) {
				CHARACTER character = chars.GetCharacter(name);
				if (character == null) {
					Debug.LogError($"[ERROR] Character '{name}' not found for 'disappear' command.");
					continue;
				}
				characters.Add(character);
			}

			if (characters.Count == 0) yield break;

			foreach (CHARACTER character in characters) {
				character.Disappear();
			}

			while (characters.Any(c => c.isDisappearing)) yield return null;
		}

		private static IEnumerator MoveX(string[] data) {
			CHARACTER character = CharacterManager.instance.GetCharacter(data[0]);
			if (character == null) {
				Debug.LogError($"[ERROR] Character '{data[0]}' not found for 'setposX' command.");
				yield break;
			}

			var parameters = ConvertParameters(data);

			float.TryParse(data[1], out float x);
			parameters.TryGetValue("-i", out bool i, defaultValue: false);
			parameters.TryGetValue("-d", out float d, defaultValue: 0.5f);

			character.MoveX(x, duration: d, immediate: i);

			if (!i) {
				while (character.isMovingX) yield return null;
			}
		}

		private static IEnumerator MoveY(string[] data) {
			CHARACTER character = CharacterManager.instance.GetCharacter(data[0]);
			if (character == null) {
				Debug.LogError($"[ERROR] Character '{data[0]}' not found for 'setposY' command.");
				yield break;
			}

			var parameters = ConvertParameters(data);

			float.TryParse(data[1], out float x);
			parameters.TryGetValue("-i", out bool i, defaultValue: false);
			parameters.TryGetValue("-d", out float d, defaultValue: 0.5f);

			character.MoveY(x, duration: d, immediate: i);

			if (!i) {
				while (character.isMovingY) yield return null;
			}
		}

		private static IEnumerator Hop(string characterName) {
			CHARACTER character = CharacterManager.instance.GetCharacter(characterName);
			if (character == null) {
				Debug.LogError($"[ERROR] Character '{characterName}' not found for 'h' (hop) command.");
				yield break;
			}
			yield return character.Hop();
		}

		private static IEnumerator Crouch(string characterName) {
			CHARACTER character = CharacterManager.instance.GetCharacter(characterName);
			if (character == null) {
				Debug.LogError($"[ERROR] Character '{characterName}' not found for 'c' (crouch) command.");
				yield break;
			}
			yield return character.Crouch();
		}

		private static IEnumerator Shiver(string characterName) {
			CHARACTER character = CharacterManager.instance.GetCharacter(characterName);
			if (character == null) {
				Debug.LogError($"[ERROR] Character '{characterName}' not found for 's' (shiver) command.");
				yield break;
			}
			yield return character.Shiver();
		}

		private static void Sprite(string[] data) {
			CHARACTER character = CharacterManager.instance.GetCharacter(data[0]);
			if (character == null) {
				Debug.LogError($"[ERROR] Character '{data[0]}' not found for 'sp' (setSprite) command.");
				return;
			}

			character.SetSprite(data[1]);
		}

		private static void InvertX(string characterName) {
			CHARACTER character = CharacterManager.instance.GetCharacter(characterName);
			if (character == null) {
				Debug.LogError($"[ERROR] Character '{characterName}' not found for 'ix' (invertX) command.");
				return;
			}
			character.InvertX();
		}

		private static IEnumerator Highlight(string[] data) {
			ICharacterService chars = CharacterManager.instance;
			List<CHARACTER> characters = new();

			foreach (string name in data) {
				CHARACTER character = chars.GetCharacter(name);
				if (character == null) {
					Debug.LogError($"[ERROR] Character '{name}' not found for 'hl' (highlight) command.");
					continue;
				}
				characters.Add(character);
			}

			if (characters.Count == 0) yield break;

			foreach (CHARACTER character in characters) {
				character.Highlight();
			}

			while (characters.Any(c => c.isHighlighting)) yield return null;
		}

		private static IEnumerator UnHighlight(string[] data) {
			ICharacterService chars = CharacterManager.instance;
			List<CHARACTER> characters = new();

			foreach (string name in data) {
				CHARACTER character = chars.GetCharacter(name);
				if (character == null) {
					Debug.LogError($"[ERROR] Character '{name}' not found for 'uhl' (unhighlight) command.");
					continue;
				}
				characters.Add(character);
			}

			if (characters.Count == 0) yield break;

			foreach (CHARACTER character in characters) {
				character.UnHighlight();
			}

			while (characters.Any(c => c.isUnHighlighting)) yield return null;
		}
	}
}
