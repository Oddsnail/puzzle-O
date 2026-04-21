using System;
using System.Collections;
using UnityEngine;

using origin.character;
using origin.dialogue;

namespace origin.command {

	public class CMD_DatabaseExtension_Logic : CMD_DatabaseExtension {

		new public static void Extend(CommandDatabase database) {
			database.AddCommand("wait", new Func<string, IEnumerator>(Sleep));
			database.AddCommand("add", new Action<string[]>(Add));
			database.AddCommand("remove", new Action<string[]>(Remove));
			database.AddCommand("choice", new Func<string[], IEnumerator>(Choice));
			database.AddCommand("choiceC", new Func<string[], IEnumerator>(ChoiceWithColor));
			database.AddCommand("j", new Action<string>(Jump));
			database.AddCommand("end", new Action<string>(EndTagReturn));
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
	}
}
