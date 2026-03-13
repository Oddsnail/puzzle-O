using System.Collections;
using UnityEngine;
using origin.audio;
using origin.character;
using origin.command;
using origin.language;

namespace origin.dialogue {
	public class DialogueLineHandler : ILineHandler {

		private readonly TextArchitect textArchitect;
		private readonly IDialogueUI dialogueUI;
		private readonly ICharacterService charService;
		private readonly ICommandExecutor commandExecutor;

		private bool nextRequest = false;
		private string recentCharacter = "";

		private const string plainTextID = "default";

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
		}

		private void OnNextDialogueRequest() => nextRequest = true;

		public bool CanHandle(LINE_DATA data) => data.isDialogue;

		public IEnumerator Handle(LINE_DATA data) => ReadingDialogue(data.dialogue);

		private IEnumerator ReadingDialogue(LINE_DIALOGUE data) {
			Debug.Log(data.ToString());

			if (data.hasName && data.name != plainTextID) {
				recentCharacter = data.name;
				CharacterConfigData config = charService.GetInfo(data.name);
				dialogueUI.ChangeNameAndTheme(config.localizedDisplayName(), config.ID);
				dialogueUI.ChangeLetterBoxTheme(config.ID);
			}
			else if (data.hasName && data.name == plainTextID) {
				recentCharacter = data.name;
				dialogueUI.ChangeNameAndTheme("", plainTextID);
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
			string resolved = LocalizationManager.Instance != null
				? LocalizationManager.Instance.Resolve(dialogue)
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

		private IEnumerator WaitForRequest() {
			int randNum = Random.Range(1, 3);
			dialogueUI.SetPromptVisible(true);

			while (!nextRequest) yield return null;

			AudioManager.Instance.PlaySoundEffect($"SFX/dialogue-{randNum}");
			dialogueUI.SetPromptVisible(false);
			nextRequest = false;
		}
	}
}
