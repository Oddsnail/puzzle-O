using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using origin.audio;
using origin.character;
using origin.command;
using origin.language;
using System;

namespace origin.dialogue {
	public class DialogueLineHandler : ILineHandler {

		private readonly TextArchitect textArchitect;
		private readonly IDialogueUI dialogueUI;
		private readonly ICharacterService charService;
		private readonly ICommandExecutor commandExecutor;

		private bool nextRequest = false;
		private string recentCharacter = "";

		private const string plainTextID = "default";

		private List<string> currentLine = new();

		public DialogueLineHandler(
				TextArchitect textArchitect,
				IDialogueUI dialogueUI,
				ICharacterService charService,
				ICommandExecutor commandExecutor) {
			this.textArchitect = textArchitect;
			this.dialogueUI = dialogueUI;
			this.charService = charService;
			this.commandExecutor = commandExecutor;
			dialogueUI.onNextDialogueRequest += OnNextDialogueRequest;
			LocalizationManager.OnLanguageChanged += OnLanguageChanged;
		}

		public void Dispose() {
			dialogueUI.onNextDialogueRequest -= OnNextDialogueRequest;
			LocalizationManager.OnLanguageChanged -= OnLanguageChanged;
		}

		private void OnNextDialogueRequest() {
			if(!DialogueManager.instance.showing && !DialogueManager.instance.manualShowLock) {
				DialogueManager.instance.Show();
			}
			else nextRequest = true;
		} 

		public bool CanHandle(LINE_DATA data) => data.isDialogue;

		public IEnumerator Handle(LINE_DATA data) => ReadingDialogue(data.dialogue);

		private IEnumerator ReadingDialogue(LINE_DIALOGUE data) {

			if (currentLine.Count != 0) {
				DialogueManager.instance.AddLog(recentCharacter == plainTextID ? plainTextID : charService.GetInfo(recentCharacter).nameKey, currentLine);
			}
			currentLine.Clear();

			if (data.hasName && data.name != plainTextID) {
				recentCharacter = data.name;
				CharacterConfigData config = charService.GetInfo(data.name);
				dialogueUI.ChangeNameAndTheme(config.nameKey, config.subnameKey, config.ID);
				dialogueUI.ChangeLetterBoxTheme(config.ID);
			}
			else if (data.hasName && data.name == plainTextID) {
				recentCharacter = data.name;
				dialogueUI.ChangeNameAndTheme("", "", plainTextID);
				dialogueUI.ChangeLetterBoxTheme(plainTextID);
			}

			if (data.hasSpriteCode) {
				CHARACTER character = charService.GetCharacter(recentCharacter);
				character.SetSprite(data.spriteCode);
			}

			if (data.hasSpeakEvent) {
				LINE_COMMAND command = new(data.speakEvent + $"({recentCharacter})");
				commandExecutor.Execute(command.commands[0].name, command.commands[0].arguments);
			}

			foreach (LINE_DIALOGUE.LINE_SEGMENT segment in data.segments) {
				yield return WaitLineSegments(segment);
				currentLine.Add(segment.dialogue);
				yield return BuildDialogue(segment.dialogue, segment.isAppend);
			}

			while (dialogueUI.isNameTransitioning) yield return null;
			yield return WaitForRequest();
		}

		private IEnumerator WaitLineSegments(LINE_DIALOGUE.LINE_SEGMENT segment) {
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

		private IEnumerator BuildDialogue(string dialogue, bool append = false) {
			string resolved = LocalizationManager.instance != null
				? LocalizationManager.instance.Resolve(dialogue)
				: dialogue;
			if (!append) textArchitect.Build(resolved);
			else textArchitect.Append(resolved);

			while (textArchitect.isBuilding) {
				if (nextRequest) {
					if (!textArchitect.speedUp) textArchitect.speedUp = true;
					nextRequest = false;
				}
				yield return null;
			}
		}

		private void OnLanguageChanged() {
			if (textArchitect.isBuilding) {
				textArchitect.ForceComplete();
			}

			RebuildCurrentLine();
		}

		private void RebuildCurrentLine() {
			if (currentLine.Count == 0) return;

			var prev = textArchitect.buildMethod;
			textArchitect.buildMethod = TextArchitect.TextBuildMethod.instant;

			for (int i = 0; i < currentLine.Count; i++) {
				string resolved = LocalizationManager.instance != null
					? LocalizationManager.instance.Resolve(currentLine[i])
					: currentLine[i];

				if (i == 0) textArchitect.Build(resolved);
				else textArchitect.Append(resolved);
			}

			textArchitect.buildMethod = prev;
		}

		// public void OnConversationEnd() {
		// 	if (currentLine.Count != 0) {
		// 		DialogueManager.instance.AddLog(recentCharacter == plainTextID ? plainTextID : charService.GetInfo(recentCharacter).nameKey, currentLine);
		// 		currentLine.Clear();
		// 	}
		// }

		private IEnumerator WaitForRequest() {
			dialogueUI.SetPromptVisible(true);

			while (!nextRequest) yield return null;

			AudioManager.instance.PlayPreloadedSFX("nextDialogue");
			dialogueUI.SetPromptVisible(false);
			nextRequest = false;
		}
	}
}
