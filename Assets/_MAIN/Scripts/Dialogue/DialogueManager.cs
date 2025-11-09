using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

using origin.audio;
using origin.graphic;
using NUnit.Framework;

namespace origin.dialogue {
	public class DialogueManager : MonoBehaviour {

		public static DialogueManager instance;

		private TextArchitect textArchitect;
		private ConversationManager conversationManager;
		public DialogueContainer dialogueContainer = new();

		public RectTransform upperLetterbox;
		public RectTransform lowerLetterbox;
		public GameObject promptIcon;

		public Transform choicePanel;
		public GameObject choicePrefab;

		public delegate void DialogueEvent();
		public event DialogueEvent onNextDialogueRequest;

		private Coroutine co_transitioning = null;
		private Coroutine co_name_transitioning = null;
		public bool isTransitioning => co_transitioning != null;
		public bool isNameTransitioning => co_name_transitioning != null;

		public ColorPalette colorPalette;

		bool initialized = false;
		bool showing = false;

		public void Awake() {
			if (instance == null) {
				instance = this;
				Initialize();
			}
			else DestroyImmediate(gameObject);
		}

		public void Initialize() {
			if (initialized) return;

			initialized = true;
			textArchitect = new(dialogueContainer.dialogueText);
			conversationManager = new(textArchitect);
		}

		public Coroutine Say(List<string> conversation, Action<string> endTagCallback) {
			return conversationManager.StartConversation(conversation, endTagCallback);
		}

		public void Empty() {
			dialogueContainer.nameRoot.gameObject.SetActive(false);
			dialogueContainer.dialogueText.text = "";
		}

		public void End(string code) => conversationManager.End(code);

		public void OnNextDialogueRequest() => onNextDialogueRequest?.Invoke();

		//========================================================================
		// <!!!> update dialogue box as new name container and theme color. <!!!>
		//========================================================================
		public void changeNameAndTheme(string text, string ID) {
			if (isNameTransitioning) return;
			co_name_transitioning = StartCoroutine(changeNameAnimation(text, colorPalette.Getcolor(ID)));
		}

		public void changeLetterBoxTheme(string ID) {
			upperLetterbox.GetComponent<LetterboxLoop>().ColorTransition(colorPalette.Getcolor(ID));
			lowerLetterbox.GetComponent<LetterboxLoop>().ColorTransition(colorPalette.Getcolor(ID));
        }

		private const float defaultChangeNameDuration = 0.1f;
		private const float defaultNameAnimOffset = 100.0f;

		private IEnumerator changeNameAnimation(string text, Color color) {

			RectTransform nameRect = dialogueContainer.nameRoot;
			Vector2 defaultPosition = nameRect.anchoredPosition;
			CanvasGroup nameCanvasGroup = dialogueContainer.nameRoot.GetComponent<CanvasGroup>();

			yield return DOTween.Sequence()
				.Join(nameCanvasGroup.DOFade(0f, defaultChangeNameDuration))
				.Join(nameRect.DOAnchorPosX(defaultPosition.x + defaultNameAnimOffset, defaultChangeNameDuration))
				.WaitForCompletion();

			nameRect.anchoredPosition = new Vector2(defaultPosition.x - defaultNameAnimOffset * 2f, defaultPosition.y);
			dialogueContainer.SetThemeColor(color);
			dialogueContainer.nameText.SetText(text);
			if (text == "" && nameRect.gameObject.activeSelf) nameRect.gameObject.SetActive(false);
			if (text != "" && !nameRect.gameObject.activeSelf) nameRect.gameObject.SetActive(true);

			yield return DOTween.Sequence()
				.Join(nameCanvasGroup.DOFade(1f, defaultChangeNameDuration))
				.Join(nameRect.DOAnchorPosX(defaultPosition.x, defaultChangeNameDuration))
				.WaitForCompletion();

			co_name_transitioning = null;
		}
		//========================================================================
		//    <!!!> show / hide dialogue box by conversation start / end <!!!>
		//========================================================================
		public void Show() {
			if (showing) return;
			if (isTransitioning) return;

			SetLetterboxSpeed(0.45f);
			co_transitioning = StartCoroutine(Showing());
		}

		public void Hide() {
			if (!showing) return;
			if (isTransitioning) return;

			SetLetterboxSpeed(0.15f);
			co_transitioning = StartCoroutine(Hiding());
		}

		public void SetLetterboxSpeed(float speed) {
			upperLetterbox.GetComponent<LetterboxLoop>().speed = speed;
			lowerLetterbox.GetComponent<LetterboxLoop>().speed = -1 * speed;
        }

		private const float defaultShowAndHideDuration = 0.6f;                      // default transition duration
		private const float defaultDialogueHideYPos = -700f;                        // dialogue box y coord when showing = true
		private float defaultLetterboxGap => upperLetterbox.sizeDelta.y / 2f;       // default letterbox on&off gap
		private float defaultUpperLetterboxYPos => -upperLetterbox.sizeDelta.y;     // upper letterbox y coord when showing = true


		private IEnumerator Showing() {
			dialogueContainer.dialogueRoot.gameObject.SetActive(true);

			yield return DOTween.Sequence()
				.Join(dialogueContainer.dialogueRoot.DOAnchorPosY(-defaultDialogueHideYPos / 7f, defaultShowAndHideDuration))
				.Join(upperLetterbox.DOAnchorPosY(defaultUpperLetterboxYPos, defaultShowAndHideDuration))
				.Join(lowerLetterbox.DOAnchorPosY(0.0f, defaultShowAndHideDuration))
				.WaitForCompletion();

			showing = true;
			co_transitioning = null;
		}

		private IEnumerator Hiding() {
			yield return DOTween.Sequence()
				.Join(dialogueContainer.dialogueRoot.DOAnchorPosY(defaultDialogueHideYPos, defaultShowAndHideDuration))
				.Join(upperLetterbox.DOAnchorPosY(defaultUpperLetterboxYPos + defaultLetterboxGap, defaultShowAndHideDuration))
				.Join(lowerLetterbox.DOAnchorPosY(-defaultLetterboxGap, defaultShowAndHideDuration))
				.WaitForCompletion();

			showing = false;
			dialogueContainer.dialogueRoot.gameObject.SetActive(false);
			co_transitioning = null;
		}
		//========================================================================
		//                   <!!!> choice panel & jump system
		//========================================================================
		private void ClearChoices() {
			foreach (Transform child in choicePanel) {
				Destroy(child.gameObject);
			}
		}

		public IEnumerator AvailableChoices((string, string, string)[] choices, bool isColored) {

			bool choiceMade = false;
			string choicedCode = "";

			void ChoiceHandler(string code) {
				choiceMade = true;
				choicedCode = code;
			}

			for (int i = 0; i < choices.Length; i++) {
				GameObject choiceButton = Instantiate(choicePrefab, choicePanel);
				TMP_Text buttonText = choiceButton.GetComponentInChildren<TMP_Text>();
				OptionButton optionButton = choiceButton.GetComponent<OptionButton>();

				// ===== sprite change logic (might get deleted) =====
				optionButton.button.GetComponent<Image>().sprite = optionButton.button.GetComponent<SpriteSheetHolder>().sprites[i % 3];
				optionButton.buttonColor.GetComponent<Image>().sprite = optionButton.buttonColor.GetComponent<SpriteSheetHolder>().sprites[i % 3];
				// ===================================================

				Button button = optionButton.button.GetComponent<Button>();
				string code = choices[i].Item2;
				button.onClick.AddListener(() => ChoiceHandler(code));
				buttonText.text = choices[i].Item1;
				if (isColored) optionButton.SetSelectColor(colorPalette.Getcolor(choices[i].Item3));
			}

			yield return new WaitUntil(() => choiceMade);

			AudioManager.Instance.PlaySoundEffect("SFX/dialogue-3");
			conversationManager.Jump(choicedCode);

			ClearChoices();
		}

		public void Jump(string code) => conversationManager.Jump(code);
		//========================================================================
		//========================================================================
	}
}