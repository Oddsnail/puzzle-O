using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using origin.audio;
using origin.graphic;

namespace origin.dialogue {
	public class DialogueManager : MonoBehaviour {

		public static DialogueManager instance;

		private TextArchitect textArchitect;
		private ConversationManager conversationManager;
		private DialogueUIManager dialogueUIManager;
		public DialogueContainer dialogueContainer = new();

		public delegate void DialogueEvent();
		public event DialogueEvent onNextDialogueRequest;

		public bool isTransitioning => dialogueUIManager != null && dialogueUIManager.isTransitioning;
		public bool isNameTransitioning => dialogueUIManager != null && dialogueUIManager.isNameTransitioning;

		public ColorPalette colorPalette;

		bool initialized = false;

		public void Awake() {
			if (instance != null && instance != this) {
				Debug.LogWarning($"Duplicate {GetType().Name} detected. Destroying duplicate.");
				Destroy(gameObject);
				return;
			}
			instance = this;
			Initialize();
		}

		private void OnDestroy() {
			if (instance == this) {
				instance = null;
			}
		}

		public void Initialize() {
			if (initialized) return;

			initialized = true;
			textArchitect = new(dialogueContainer.dialogueText);
			conversationManager = new(textArchitect);
			dialogueUIManager = new();
			dialogueUIManager.Initialize(dialogueContainer, this);
		}

		public Coroutine Say(List<string> conversation, Action<string> endTagCallback) {
			return conversationManager.StartConversation(conversation, endTagCallback);
		}

		public void Empty() {
			dialogueUIManager.Empty();
		}

		public void End(string code) => conversationManager.End(code);

		public void OnNextDialogueRequest() => onNextDialogueRequest?.Invoke();

		//========================================================================
		// <!!!> update dialogue box as new name container and theme color. <!!!>
		//========================================================================
		public void changeNameAndTheme(string text, string ID) {
			dialogueUIManager.ChangeNameAndTheme(text, colorPalette.Getcolor(ID));
		}

		public void changeLetterBoxTheme(string ID) {
			dialogueUIManager.ChangeLetterBoxTheme(colorPalette.Getcolor(ID));
		}

		//========================================================================
		//    <!!!> show / hide dialogue box by conversation start / end <!!!>
		//========================================================================
		public void Show() {
			dialogueUIManager.Show();
		}

		public void Hide() {
			dialogueUIManager.Hide();
		}

		public void SetLetterboxSpeed(float speed) {
			dialogueUIManager.SetLetterboxSpeed(speed);
		}

		//========================================================================
		//                   <!!!> choice panel & jump system
		//========================================================================
		public IEnumerator AvailableChoices((string, string, string)[] choices, bool isColored) {
			yield return dialogueUIManager.CreateChoices(choices, isColored, (choicedCode) => {
				AudioManager.Instance.PlaySoundEffect("SFX/dialogue-3");
				conversationManager.Jump(choicedCode);
			});
		}

		public void Jump(string code) => conversationManager.Jump(code);
		//========================================================================
		//========================================================================
	}
}
