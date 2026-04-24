using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using origin.dialogue;
using origin.language;
using origin.puzzle;
using origin.graphic;

namespace origin.tutorial {
	public class TutorialManager : MonoBehaviour {

		public static TutorialManager instance;

		[Header("Tutorial Data")]
		public TextAsset tutorialSets;

		[Header("UI References")]
		public GameObject tutorialTextBox;
		public TextMeshProUGUI tutorialText;
		public GameObject promptIcon;
		public GameObject skipPanel;

		[Header("Fog Panels")]
		public GameObject tutorialFog;
		public RectTransform fogTop;
		public RectTransform fogBottom;
		public RectTransform fogLeft;
		public RectTransform fogRight;

		private TextArchitect textArchitect;
		private RectTransform tutorialTextBoxRect;

		private const float fogSpeedMultiplier = 8f;
		private const float refWidth = 3840f;
		private const float refHeight = 2160f;
		private CanvasGroup fogCanvasGroup;
		private Coroutine co_fog = null;

		private bool isOnStep = false;
		private bool isBetweenSteps = false;
		private bool nextRequest = false;
		private bool puzzleGuess = false;
		private char desiredPuzzleInput = '_';
		private Coroutine co_tutorial = null;
		private TutorialStep currentStep = null;
		private Rect currentHighlightScreenRect;
		private string textKeyFormat = null;

		private const float waitBetweenSteps = 0.2f;
		private const string textKeyPrefix = "tutorial.";
		private const string prefSawPrefix = "SawTutorial.";

		public bool IsRunning => co_tutorial != null;
		public bool IsOnStep => isOnStep;
		public bool IsBetweenSteps => isBetweenSteps;

		private void Awake() {
			if (instance == null) {
				instance = this;
				textArchitect = new TextArchitect(tutorialText);
				tutorialTextBoxRect = tutorialTextBox.GetComponent<RectTransform>();
				fogCanvasGroup = tutorialFog.GetComponent<CanvasGroup>();
				if (fogCanvasGroup == null) fogCanvasGroup = tutorialFog.AddComponent<CanvasGroup>();
			}
			else DestroyImmediate(gameObject);
		}

		private void OnDestroy() {
			if (instance == this)
				instance = null;
		}

		// --- Public API ---

		public void StartTutorial(string setID) {
			TutorialSet set = TutorialSetParser.Parse(tutorialSets, setID);
			if (set == null) {
				Debug.LogWarning($"TutorialManager: No tutorial set found with ID '{setID}'");
				return;
			}

			if (PlayerPrefs.GetInt(prefSawPrefix + setID) == 1) skipPanel.SetActive(true);
			else skipPanel.SetActive(false);
			textKeyFormat = textKeyPrefix + setID + ".";
			if (IsRunning) StopTutorial();

			co_tutorial = StartCoroutine(RunTutorial(set));
		}

		public void StopTutorial() {
			if (co_tutorial != null) {
				StopCoroutine(co_tutorial);
				co_tutorial = null;
			}
			CleanupTutorialUI();
		}

		public void OnNextDialogueRequest() => nextRequest = true;
		public void OnCharacterGuess(char c) => puzzleGuess = desiredPuzzleInput == c;

		public bool ShouldAllowInput(string actionName) {
			if (!IsRunning) return true;
			if (isBetweenSteps || !isOnStep) return false;

			if (actionName == "NextDialogue") {
				if (currentStep != null
					&& currentStep.stepType == StepType.ForceInput)
					return false;
				return true;
			}
			if (actionName == "PuzzleGuess") {
				if (currentStep != null
					&& currentStep.stepType != StepType.ForceInput)
					return false;
				if (currentStep.forceCondition.inputType == ForceInputType.PointerInArea) return false;
				return true;
			}

			return false;
		}

		// --- Coroutine ---

		private IEnumerator RunTutorial(TutorialSet set) {
			tutorialTextBox.SetActive(true);
			ShowFog(set.steps[0]);

			for (int i = 0; i < set.steps.Count; i++) {
				TutorialStep step = set.steps[i];
				currentStep = step;
				isOnStep = true;
				nextRequest = false;
				string text = textKeyFormat + $"{i}";

				// Text box Locate
				tutorialTextBoxRect.sizeDelta = new Vector2(step.dialogueLength, tutorialTextBoxRect.sizeDelta.y);
				tutorialTextBoxRect.anchoredPosition = new Vector2(step.dialogueX, step.dialogueY);

				// Text building
				string resolvedText = LocalizationManager.instance != null
					? LocalizationManager.instance.Get(text)
					: "non-instantiated LocalizationManager error";

				textArchitect.Build(resolvedText);

				while (textArchitect.isBuilding) {
					if (nextRequest) {
						textArchitect.speedUp = true;
						nextRequest = false;
					}
					yield return null;
				}

				// Wait for step completion
				switch (step.stepType) {
					case StepType.HighlightArea:
						// TODO: prompt icon
						nextRequest = false;
						promptIcon.SetActive(true);
						yield return new WaitUntil(() => nextRequest);
						// TODO: prompt icon
						nextRequest = false;
						promptIcon.SetActive(false);
						break;

					case StepType.ForceInput:
						yield return WaitForForceInput(step);
						break;
				}

				if (i + 1 < set.steps.Count) RepositionFog(set.steps[i + 1]);

				if (step.doEvent) {
					SideBox puzzleSideBox = PuzzleManager.instance.puzzleContainer.containerRoot.GetComponent<SideBox>();
					SideBox ruleSideBox = PuzzleManager.instance.ruleContainer.containerRoot.GetComponent<SideBox>();
					switch (step.stepEvent) {
						case StepEvent.RightSideboxOpen:
							puzzleSideBox.SlideTo(puzzleSideBox.showXPos);
							break;

						case StepEvent.RightSideboxClose:
							puzzleSideBox.SlideTo(puzzleSideBox.peekXPos);
							break;

						case StepEvent.LeftSideboxOpen:
							ruleSideBox.SlideTo(ruleSideBox.showXPos);
							break;

						case StepEvent.LeftSideboxClose:
							ruleSideBox.SlideTo(ruleSideBox.peekXPos);
							break;
					}
				}

				isOnStep = false;
				currentStep = null;

				isBetweenSteps = true;
				yield return new WaitForSeconds(waitBetweenSteps);
				isBetweenSteps = false;
			}

			HideFog();
			yield return new WaitForSeconds(waitBetweenSteps);

			CleanupTutorialUI();
			PlayerPrefs.SetInt(prefSawPrefix + set.setID, 1);
			PlayerPrefs.Save();

			co_tutorial = null;
		}

		private IEnumerator WaitForForceInput(TutorialStep step) {
			switch (step.forceCondition.inputType) {
				case ForceInputType.KeyboardInput:
					puzzleGuess = false;
					desiredPuzzleInput = step.forceCondition.requiredKey;
					yield return new WaitUntil(() => puzzleGuess);
					puzzleGuess = false;
					desiredPuzzleInput = '_';

					PuzzleManager.instance.OnCharacterGuess(step.forceCondition.requiredKey);
					break;

				case ForceInputType.PointerInArea:
					yield return new WaitUntil(() => IsPointerInHighlightArea());
					break;
			}
		}

		private void CleanupTutorialUI() {
			isOnStep = false;
			isBetweenSteps = false;
			nextRequest = false;
			currentStep = null;
			tutorialTextBox.SetActive(false);
			skipPanel.SetActive(false);
			if (co_fog != null) { StopCoroutine(co_fog); co_fog = null; }
			fogCanvasGroup.alpha = 0f;
			tutorialFog.SetActive(false);
		}

		// --- Fog Masking ---

		private void RepositionFog(TutorialStep step) {
			var (minX, minY, maxX, maxY) = ResolveHighlightNormalized(step);
			PositionFogPanels(minX, minY, maxX, maxY);
		}

		private void ShowFog(TutorialStep step) {
			var (minX, minY, maxX, maxY) = ResolveHighlightNormalized(step);
			PositionFogPanels(minX, minY, maxX, maxY);
			if (co_fog != null) StopCoroutine(co_fog);
			tutorialFog.SetActive(true);
			co_fog = StartCoroutine(Co_FadeFog(0f, 1f));
		}

		private void HideFog() {
			if (co_fog != null) StopCoroutine(co_fog);
			co_fog = StartCoroutine(Co_HideFog());
		}

		private IEnumerator Co_HideFog() {
			yield return Co_FadeFog(fogCanvasGroup.alpha, 0f);
			tutorialFog.SetActive(false);
			co_fog = null;
		}

		private IEnumerator Co_FadeFog(float from, float to) {
			fogCanvasGroup.alpha = from;
			float dir = Mathf.Sign(to - from);
			if (dir == 0f) yield break;
			while (dir > 0f ? fogCanvasGroup.alpha < to : fogCanvasGroup.alpha > to) {
				fogCanvasGroup.alpha = Mathf.MoveTowards(fogCanvasGroup.alpha, to, fogSpeedMultiplier * Time.deltaTime);
				yield return null;
			}
			fogCanvasGroup.alpha = to;
		}

		private (float minX, float minY, float maxX, float maxY) ResolveHighlightNormalized(TutorialStep step) {
			Rect r = step.highlightRect;

			currentHighlightScreenRect = new Rect(
				r.x * Screen.width / refWidth,
				r.y * Screen.height / refHeight,
				r.width * Screen.width / refWidth,
				r.height * Screen.height / refHeight
			);

			return (
				Mathf.Clamp01(r.x / refWidth),
				Mathf.Clamp01(r.y / refHeight),
				Mathf.Clamp01((r.x + r.width) / refWidth),
				Mathf.Clamp01((r.y + r.height) / refHeight)
			);
		}

		private void PositionFogPanels(float minX, float minY, float maxX, float maxY) {
			fogTop.anchorMin = new Vector2(0f, maxY);
			fogTop.anchorMax = new Vector2(1f, 1f);
			fogTop.offsetMin = Vector2.zero;
			fogTop.offsetMax = Vector2.zero;

			fogBottom.anchorMin = new Vector2(0f, 0f);
			fogBottom.anchorMax = new Vector2(1f, minY);
			fogBottom.offsetMin = Vector2.zero;
			fogBottom.offsetMax = Vector2.zero;

			fogLeft.anchorMin = new Vector2(0f, minY);
			fogLeft.anchorMax = new Vector2(minX, maxY);
			fogLeft.offsetMin = Vector2.zero;
			fogLeft.offsetMax = Vector2.zero;

			fogRight.anchorMin = new Vector2(maxX, minY);
			fogRight.anchorMax = new Vector2(1f, maxY);
			fogRight.offsetMin = Vector2.zero;
			fogRight.offsetMax = Vector2.zero;
		}

		private bool IsPointerInHighlightArea() {
			Vector2 mousePos = Input.mousePosition;
			return currentHighlightScreenRect.Contains(mousePos);
		}
	}
}
