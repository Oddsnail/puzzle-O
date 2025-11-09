using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using origin.audio;
using origin.character;
using origin.command;

namespace origin.dialogue {
	public class ConversationManager {
		private DialogueManager dialogueManager => DialogueManager.instance;
		private CharacterManager characterManager => CharacterManager.instance;
		private TextArchitect textArchitect = null;

		private Coroutine co_talking = null;
		public bool isTalking => co_talking != null;

		private bool nextRequest = false;
		private bool jumping = false;
		private bool endBool = false;
		private string jumpingCode = "";
		private string endTag = "EOF";

		private const char noteID = '#';
		public const string plainTextID = "default";
		private readonly HashSet<string> autoWaitCommands = new() { "wait", "add", "choice", "choiceC", "puzzle", "hl", "uhl" };

		public ConversationManager(TextArchitect architect) {
			textArchitect = architect;
			dialogueManager.onNextDialogueRequest += OnNextDialogueRequest;
		}

		private void OnNextDialogueRequest() {
			nextRequest = true;
		}

		public Coroutine StartConversation(List<string> conversation, Action<string> endTag) {
			StopConversation();
			co_talking = dialogueManager.StartCoroutine(RunningConversation(conversation, endTag));

			return co_talking;
		}

		public void StopConversation() {
			if (!isTalking) return;
			dialogueManager.StopCoroutine(co_talking);
			co_talking = null;
			dialogueManager.Hide();
		}

		IEnumerator RunningConversation(List<string> conversation, Action<string> endTagCallback) {
			endBool = false;
			dialogueManager.Show();

			foreach (string line in conversation) {
				if (string.IsNullOrWhiteSpace(line) || line[0] == noteID) continue;

				// ===== In choice / dot label logic =====
				if (jumping) {
					Debug.Log("...jumped a line");
					if (line != $".{jumpingCode}") continue;
					jumping = false;
					continue;
				}
				else if (line.StartsWith('.')) continue;
				// ===========================

				Debug.Log(line);
				LINE_DATA data = new(line);

				if (data.isDialogue) yield return ReadingDialogue(data.dialogue);
				else yield return ReadingCommand(data.command);

				if (endBool) break;
			}
			
			dialogueManager.changeLetterBoxTheme(plainTextID);
			dialogueManager.Hide();
			endTagCallback!.Invoke(endTag);
		}

		public void Jump(string code) {
			jumpingCode = code;
			jumping = true;
		}

		public void End(string code) {
			endTag = code;
			endBool = true;
		}

		//========================================================================
		//                  <!!!> Executing DIALOGUE data <!!!>
		//========================================================================
		private string recentCharacter = "";

		IEnumerator ReadingDialogue(LINE_DIALOGUE data) {
			Debug.Log(data.ToString());

			if (data.hasName && data.name != plainTextID) {
				recentCharacter = data.name;
				CharacterConfigData config = characterManager.GetInfo(data.name);
				dialogueManager.changeNameAndTheme(config.displayString, config.ID);
				dialogueManager.changeLetterBoxTheme(config.ID);
			}
			else if (data.hasName && data.name == plainTextID) {
				recentCharacter = data.name;
				dialogueManager.changeNameAndTheme("", plainTextID);
				dialogueManager.changeLetterBoxTheme(plainTextID);
			}

			if (data.hasSpriteCode) {
				CHARACTER character = characterManager.GetCharacter(recentCharacter);
				character.SetSprite(data.spriteCode);
			}

			if (data.hasSpeakEvent) {
				LINE_COMMAND command = new(data.speakEvent + $"({recentCharacter})");
				CommandManager.instance.Execute(command.commands[0].name, command.commands[0].arguments);
			}

			foreach (LINE_DIALOGUE.LINE_SEGMENT segment in data.segments) {
				yield return WaitLineSegments(segment);
				yield return BuildDialogue(segment.dialogue, segment.isAppend);
			}

			while (dialogueManager.isNameTransitioning) yield return null;
			yield return WaitForRequest();
		}

		IEnumerator WaitLineSegments(LINE_DIALOGUE.LINE_SEGMENT segment) {
			switch (segment.startSignal) {
				case LINE_DIALOGUE.LINE_SEGMENT.StartSignal.B:
				case LINE_DIALOGUE.LINE_SEGMENT.StartSignal.A:
					yield return WaitForRequest();
					break;
				case LINE_DIALOGUE.LINE_SEGMENT.StartSignal.WB:
				case LINE_DIALOGUE.LINE_SEGMENT.StartSignal.WA:
					yield return new WaitForSeconds(segment.waitDelay);
					break;
				default:
					break;
			}
		}

		IEnumerator BuildDialogue(string dialogue, bool append = false) {
			if (!append) textArchitect.Build(dialogue);
			else textArchitect.Append(dialogue);

			while (textArchitect.isBuilding) {
				if (nextRequest) {
					if (!textArchitect.speedUp) textArchitect.speedUp = true;
					nextRequest = false;
				}
				yield return null;
			}
		}

		//========================================================================
		//                   <!!!> Executing COMMAND data <!!!>
		//========================================================================

		IEnumerator ReadingCommand(LINE_COMMAND data) {
			Debug.Log(data.ToString());

			List<LINE_COMMAND.COMMAND> commands = data.commands;

			foreach (LINE_COMMAND.COMMAND command in commands) {

				if (command.waitForCompletion || autoWaitCommands.Contains(command.name))
					yield return CommandManager.instance.Execute(command.name, command.arguments);
				else CommandManager.instance.Execute(command.name, command.arguments);

			}
			yield return null;
		}
		//========================================================================
		//========================================================================
		IEnumerator WaitForRequest() {
			int randNum = UnityEngine.Random.Range(1, 3);
			dialogueManager.promptIcon.SetActive(true);

			while (!nextRequest) yield return null;

			AudioManager.Instance.PlaySoundEffect($"SFX/dialogue-{randNum}");
			dialogueManager.promptIcon.SetActive(false);
			nextRequest = false;
		}
		//========================================================================
		//========================================================================
	}
}