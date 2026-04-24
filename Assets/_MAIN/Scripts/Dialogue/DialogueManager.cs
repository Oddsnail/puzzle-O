using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using origin.audio;
using origin.graphic;
using origin.character;
using origin.command;

namespace origin.dialogue {
	public class DialogueManager : MonoBehaviour, IDialogueUI, IConversationControl {

		public static DialogueManager instance;

		private TextArchitect textArchitect;
		private ConversationManager conversationManager;
		private DialogueUIManager dialogueUIManager;
		private LogManager logManager;

		public DialogueContainer dialogueContainer = new();
		public LogContainer logContainer = new();

		public event Action onNextDialogueRequest;

		public bool isTransitioning => dialogueUIManager != null && dialogueUIManager.isTransitioning;
		public bool isNameTransitioning => dialogueUIManager != null && dialogueUIManager.isNameTransitioning;

		public ColorPalette colorPalette;

		public void Awake() {
			if (instance != null && instance != this) {
				Debug.LogWarning($"Duplicate {GetType().Name} detected. Destroying duplicate.");
				Destroy(gameObject);
				return;
			}
			instance = this;

			// Self-only init — other singletons are not guaranteed to exist yet
			textArchitect = new(dialogueContainer.dialogueText);
			dialogueUIManager = new();
			logManager = new();
			dialogueUIManager.Initialize(dialogueContainer, this);
			logManager.Initialize(logContainer, this);
		}

		private void Start() {
			// Cross-singleton wiring — all Awake() calls have completed by now
			conversationManager = new(new ILineHandler[] {
				new DialogueLineHandler(textArchitect, this, CharacterManager.instance, CommandManager.instance),
				new CommandLineHandler(CommandManager.instance)
			}, this, this);
		}

		private void OnDestroy() {
			if (instance == this) {
				instance = null;
				conversationManager.Dispose();
			}
		}

		public Coroutine Say(List<string> conversation, Action<string> endTagCallback) {
			return conversationManager.StartConversation(conversation, endTagCallback);
		}

		public void Empty() => dialogueUIManager.Empty();
		public void End(string code) => conversationManager.End(code);

		public void OnNextDialogueRequest() => onNextDialogueRequest?.Invoke();
		public void ChangeNameAndTheme(string nameKey, string subnameKey, string ID) => dialogueUIManager.ChangeNameAndTheme(nameKey, subnameKey, colorPalette.Getcolor(ID));
		public void ChangeLetterBoxTheme(string ID) => dialogueUIManager.ChangeLetterBoxTheme(colorPalette.Getcolor(ID));
		public void SetPromptVisible(bool visible) => dialogueContainer.promptIcon.SetActive(visible);

		public void Show() => dialogueUIManager.Show();
		public void Hide() => dialogueUIManager.Hide();

		public void SetLetterboxSpeed(float speed) => dialogueUIManager.SetLetterboxSpeed(speed);

		public bool IsLogOpen => logManager.IsLogOpen;
		public void ToggleLog() => logManager.ToggleLog();
		public void CloseLog() => logManager.CloseLog();
		public void AddLog(string speaker, List<string> content) => logManager.AddLog(speaker, content);
		public void EmptyLog() => logManager.EmptyLog();

		public IEnumerator AvailableChoices((string, string, string)[] choices, bool isColored) {
			yield return dialogueUIManager.CreateChoices(choices, isColored, (choicedCode) => {
				AudioManager.instance.PlayPreloadedSFX("choiceMade");
				conversationManager.Jump(choicedCode);
			});
		}

		public void Jump(string code) => conversationManager.Jump(code);
	}
}
