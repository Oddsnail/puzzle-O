using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using origin.graphic;

namespace origin.dialogue {
	public class DialogueUIManager {

		private DialogueContainer container;
		private MonoBehaviour runner;

		private Coroutine co_transitioning = null;
		private Coroutine co_name_transitioning = null;
		public bool isTransitioning => co_transitioning != null;
		public bool isNameTransitioning => co_name_transitioning != null;

		private bool showing = false;

		private const float defaultChangeNameDuration = 0.1f;
		private const float defaultNameAnimOffset = 100.0f;
		private const float defaultShowAndHideDuration = 0.6f;
		private const float defaultDialogueHideYPos = -490f;

		public void Initialize(DialogueContainer dialogueContainer, MonoBehaviour runner) {
			container = dialogueContainer;
			this.runner = runner;
		}

		public void Empty() {
			container.nameRoot.gameObject.SetActive(false);
			container.dialogueText.text = "";
		}

		public void ChangeNameAndTheme(string text, Color color) {
			if (isNameTransitioning) return;
			co_name_transitioning = runner.StartCoroutine(ChangeNameAnimation(text, color));
		}

		public void ChangeLetterBoxTheme(Color color) {
			container.upperLetterbox.GetComponent<LetterboxLoop>().ColorTransition(color);
			container.lowerLetterbox.GetComponent<LetterboxLoop>().ColorTransition(color);
		}

		private IEnumerator ChangeNameAnimation(string text, Color color) {

			RectTransform nameRect = container.nameRoot;
			Vector2 defaultPosition = nameRect.anchoredPosition;
			CanvasGroup nameCanvasGroup = container.nameRoot.GetComponent<CanvasGroup>();

			yield return DOTween.Sequence()
				.Join(nameCanvasGroup.DOFade(0f, defaultChangeNameDuration))
				.Join(nameRect.DOAnchorPosX(defaultPosition.x + defaultNameAnimOffset, defaultChangeNameDuration))
				.WaitForCompletion();

			nameRect.anchoredPosition = new Vector2(defaultPosition.x - defaultNameAnimOffset * 2f, defaultPosition.y);
			container.SetThemeColor(color);
			container.nameText.SetText(text);
			if (text == "" && nameRect.gameObject.activeSelf) nameRect.gameObject.SetActive(false);
			if (text != "" && !nameRect.gameObject.activeSelf) nameRect.gameObject.SetActive(true);

			yield return DOTween.Sequence()
				.Join(nameCanvasGroup.DOFade(1f, defaultChangeNameDuration))
				.Join(nameRect.DOAnchorPosX(defaultPosition.x, defaultChangeNameDuration))
				.WaitForCompletion();

			co_name_transitioning = null;
		}

		public void Show() {
			if (showing) return;
			if (isTransitioning) runner.StopCoroutine(co_transitioning);

			SetLetterboxSpeed(0.45f);
			co_transitioning = runner.StartCoroutine(Showing());
		}

		public void Hide() {
			if (!showing) return;
			if (isTransitioning) runner.StopCoroutine(co_transitioning);

			SetLetterboxSpeed(0.15f);
			co_transitioning = runner.StartCoroutine(Hiding());
		}

		public void SetLetterboxSpeed(float speed) {
			container.upperLetterbox.GetComponent<LetterboxLoop>().speed = speed;
			container.lowerLetterbox.GetComponent<LetterboxLoop>().speed = -1 * speed;
		}

		private float defaultLetterboxGap => container.upperLetterbox.sizeDelta.y / 2f;
		private float defaultUpperLetterboxYPos => -container.upperLetterbox.sizeDelta.y;

		private IEnumerator Showing() {
			showing = true;
			container.dialogueRoot.gameObject.SetActive(true);

			yield return DOTween.Sequence()
				.Join(container.dialogueRoot.DOAnchorPosY(-defaultDialogueHideYPos / 14f, defaultShowAndHideDuration))
				.Join(container.upperLetterbox.DOAnchorPosY(defaultUpperLetterboxYPos, defaultShowAndHideDuration))
				.Join(container.lowerLetterbox.DOAnchorPosY(0.0f, defaultShowAndHideDuration))
				.WaitForCompletion();

			co_transitioning = null;
		}

		private IEnumerator Hiding() {
			showing = false;
			yield return DOTween.Sequence()
				.Join(container.dialogueRoot.DOAnchorPosY(defaultDialogueHideYPos, defaultShowAndHideDuration))
				.Join(container.upperLetterbox.DOAnchorPosY(defaultUpperLetterboxYPos + defaultLetterboxGap, defaultShowAndHideDuration))
				.Join(container.lowerLetterbox.DOAnchorPosY(-defaultLetterboxGap, defaultShowAndHideDuration))
				.WaitForCompletion();

			container.dialogueRoot.gameObject.SetActive(false);
			co_transitioning = null;
		}

		public void ClearChoices() {
			foreach (Transform child in container.choicePanel) {
				GameObject.Destroy(child.gameObject);
			}
		}

		public IEnumerator CreateChoices((string, string, string)[] choices, bool isColored, Action<string> onChoiceSelected) {

			bool choiceMade = false;
			string choicedCode = "";

			void ChoiceHandler(string code) {
				choiceMade = true;
				choicedCode = code;
			}

			for (int i = 0; i < choices.Length; i++) {
				GameObject choiceButton = GameObject.Instantiate(container.choicePrefab, container.choicePanel);
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
				if (isColored) optionButton.SetSelectColor(choices[i].Item3);
			}

			yield return new WaitUntil(() => choiceMade);

			onChoiceSelected?.Invoke(choicedCode);

			ClearChoices();
		}
	}
}
