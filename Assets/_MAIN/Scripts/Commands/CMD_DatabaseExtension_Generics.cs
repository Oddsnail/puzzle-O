using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using origin.character;
using origin.puzzle;
using origin.dialogue;

namespace origin.command {

	public class CMD_DatabaseExtension_Generics : CMD_DatabaseExtension {

		new public static void Extend(CommandDatabase database) {

			database.AddCommand("wait", new Func<string, IEnumerator>(Sleep));
			database.AddCommand("add", new Action<string[]>(Add));
			database.AddCommand("remove", new Action<string[]>(Remove));

			// Character Animation Relates
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

			// Dynamic Dialogue Relates
			database.AddCommand("choice", new Func<string[], IEnumerator>(Choice));
			database.AddCommand("choiceC", new Func<string[], IEnumerator>(ChoiceWithColor));
			database.AddCommand("j", new Action<string>(Jump));
			database.AddCommand("end", new Action<string>(EndTagReturn));

			// UI Control Relates
			IDialogueUI ui = DialogueManager.instance;
			database.AddCommand("closeDialogue", new Action(ui.Hide));
			database.AddCommand("openDialogue", new Action(ui.Show));
			database.AddCommand("empty", new Action(ui.Empty));

			// Image relates
			database.AddCommand("changeBackground", new Action<string>(ChangeBackground));
			database.AddCommand("changeCutscene", new Action<string>(ChangeCutscene));
			database.AddCommand("quitCutscene", new Action(QuitCutscene));

			// Puzzle relates
			database.AddCommand("puzzle", new Func<string[], IEnumerator>(Puzzle));
		}

		private static IEnumerator Sleep(string time) {
			if (float.TryParse(time, out float t)) yield return new WaitForSeconds(t);
		}

		private static void Add(string[] characters) {
			ICharacterService chars = CharacterManager.instance;
			foreach (string name in characters) {
				chars.AddCharacter(name);
			}
		}

		private static void Remove(string[] characters) {
			ICharacterService chars = CharacterManager.instance;
			foreach (string name in characters) {
				chars.RemoveCharacter(name);
			}
		}

		// Character Animation Relates

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

		// Dynamic Dialogue Relates

		private static IEnumerator Choice(string[] data) {
			IDialogueUI ui = DialogueManager.instance;
			(string, string, string)[] result = new (string, string, string)[data.Length / 2];
			for (int i = 0; i < data.Length; i += 2) {
				result[i / 2] = (data[i], data[i + 1], null);
			}

			yield return ui.AvailableChoices(result, false);
		}

		private static IEnumerator ChoiceWithColor(string[] data) {
			IDialogueUI ui = DialogueManager.instance;
			(string, string, string)[] result = new (string, string, string)[data.Length / 3];
			for (int i = 0; i < data.Length; i += 3) {
				result[i / 3] = (data[i], data[i + 1], data[i + 2]);
			}

			yield return ui.AvailableChoices(result, true);
		}

		private static void Jump(string code) {
			IConversationControl conv = DialogueManager.instance;
			conv.Jump(code);
		}

		private static void EndTagReturn(string tag) {
			IConversationControl conv = DialogueManager.instance;
			conv.End(tag);
		}

		// Image Relates

		private static void ChangeBackground(string image) => DialogueManager.instance.ChangeBackground(image);

		private static void ChangeCutscene(string image) => DialogueManager.instance.ChangeCutscene(image);

		private static void QuitCutscene() => DialogueManager.instance.QuitCutscene();


		// puzzle(charID diff ruleSetCode successJumpCode failJumpCode)

		private static IEnumerator Puzzle(string[] data) {
			IDialogueUI ui = DialogueManager.instance;
			IConversationControl conv = DialogueManager.instance;

			int.TryParse(data[1], out int difficulty);
			ui.Hide();
			bool successed = false;
			yield return PuzzleManager.instance.StartPuzzle(data[0], difficulty, success => successed = success, data[2]);
			if (successed) conv.Jump(data[3]);
			else conv.Jump(data[4]);
			ui.Empty();
			ui.Show();
		}
	}
}
