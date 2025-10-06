using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CMD_DatabaseExtension_Generics : CMD_DatabaseExtension {

	new public static void Extend(CommandDatabase database) {

		database.AddCommand("wait", new Func<string, IEnumerator>(Sleep));
		database.AddCommand("add", new Action<string[]>(Add));
		database.AddCommand("remove", new Action<string[]>(Remove));

		// Character Sprite Animation Relates
        database.AddCommand("appear", new Func<string[], IEnumerator>(Appear));
        database.AddCommand("disappear", new Func<string[], IEnumerator>(Disappear));
		database.AddCommand("setpos", new Func<string[], IEnumerator>(Move));
		database.AddCommand("h", new Action<string>(Hop));
		database.AddCommand("c", new Action<string>(Crouch));
		database.AddCommand("s", new Action<string>(Shiver));
		database.AddCommand("sprite", new Action<string[]>(Sprite));

		// Dynamic Dialogue Relates 
		database.AddCommand("choice", new Func<string[], IEnumerator>(Choice));
		database.AddCommand("j", new Action<string>(Jump));
		database.AddCommand("end", new Action<string>(EndTagReturn));

		// UI Control Relates
		database.AddCommand("closeDialogue", new Action(DialogueManager.instance.Hide));
		database.AddCommand("openDialogue", new Action(DialogueManager.instance.Show));
		database.AddCommand("empty", new Action(DialogueManager.instance.Empty));

		// Puzzle relates
		database.AddCommand("puzzle", new Func<string[], IEnumerator>(Puzzle));
	}

	private static IEnumerator Sleep(string time) {
		if (float.TryParse(time, out float t)) yield return new WaitForSeconds(t);
	}

	private static void Add(string[] characters) {
		foreach (string name in characters) {
			CharacterManager.instance.AddCharacter(name);
		}
	}

	private static void Remove(string[] characters) {
		foreach (string name in characters) {
			CharacterManager.instance.RemoveCharacter(name);
		}
	}

	private static IEnumerator Appear(string[] data) {
		List<CHARACTER> characters = new();

		foreach (string name in data) {
			characters.Add(CharacterManager.instance.GetCharacter(name));
		}

		if (characters.Count == 0) yield break;

		foreach (CHARACTER character in characters) {
			character.Appear();
		}

		while (characters.Any(c => c.isAppearing)) yield return null;
	}

	private static IEnumerator Disappear(string[] data) {
		List<CHARACTER> characters = new();

		foreach (string name in data) {
			characters.Add(CharacterManager.instance.GetCharacter(name));
		}

		if (characters.Count == 0) yield break;

		foreach (CHARACTER character in characters) {
			character.Disappear();
		}

		while (characters.Any(c => c.isDisappearing)) yield return null;
	}

	private static IEnumerator Move(string[] data) {
		CHARACTER character = CharacterManager.instance.GetCharacter(data[0]);
		if (character == null) yield break;

		var parameters = ConvertParameters(data);
		
		float.TryParse(data[1], out float x);
		parameters.TryGetValue("-i", out bool i, defaultValue: false);
		parameters.TryGetValue("-s", out float d, defaultValue: 0.5f);

		character.Move(x, duration: d, immediate: i);

		if (!i) {
			while (character.isMovingY) yield return null;
		}
	}

	private static void Sprite(string[] data) {
		CHARACTER character = CharacterManager.instance.GetCharacter(data[0]);
		if (character == null) return;

		character.SetSprite(data[1]);
	}

	private static void Hop(string character) => CharacterManager.instance.GetCharacter(character).Hop();

	private static void Crouch(string character) => CharacterManager.instance.GetCharacter(character).Crouch();

	private static void Shiver(string character) => CharacterManager.instance.GetCharacter(character).Shiver();

	private static IEnumerator Choice(string[] data) {
		(string, string)[] result = new (string, string)[data.Length / 2];
		for (int i = 0; i < data.Length; i += 2) {
			result[i/2] = (data[i], data[i+1]);
		}

		yield return DialogueManager.instance.AvailableChoices(result);
	}

	private static void Jump(string code) => DialogueManager.instance.Jump(code);

	private static void EndTagReturn(string tag) => DialogueManager.instance.End(tag);

	private static IEnumerator Puzzle(string[] data) {

		int.TryParse(data[1], out int difficulty);
		DialogueManager.instance.Hide();
		bool successed = false;
		yield return PuzzleManager.instance.StartPuzzle(data[0], difficulty, success => successed = success);
		if (successed) DialogueManager.instance.Jump(data[2]);
		else DialogueManager.instance.Jump(data[3]);
		DialogueManager.instance.Empty();
		DialogueManager.instance.Show();
	}
}
