using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using origin.graphic;

namespace origin.puzzle {
	public class PuzzleUIManager : IPuzzleUI {

		private PuzzleContainer puzzleContainer;
		private RuleContainer ruleContainer;
		private MonoBehaviour runner;

		private SideBox puzzleSideBox;
		private SideBox ruleSideBox;

		private Coroutine co_animating = null;
		private List<Trial> trials = new();
		public bool isAnimating => co_animating != null;

		public void Initialize(PuzzleContainer puzzleContainer, RuleContainer ruleContainer, MonoBehaviour runner) {
			this.puzzleContainer = puzzleContainer;
			this.ruleContainer = ruleContainer;
			this.runner = runner;

			puzzleSideBox = puzzleContainer.containerRoot.GetComponent<SideBox>();
			ruleSideBox = ruleContainer.containerRoot.GetComponent<SideBox>();
		}

		public void SetThemeColor(Color color) {
			puzzleContainer.puzzleColor.color = color;
			ruleContainer.puzzleColor.color = color;
		}

		public void SetupTrials(int digitCount, int trialCount) {
			for (int i = 0; i < trialCount; i++) {
				GameObject trialObj = Object.Instantiate(puzzleContainer.trialPrefab, puzzleContainer.trialPanel);
				Trial trial = trialObj.GetComponent<Trial>();
				trial.Initialize(digitCount);
				trials.Add(trial);
			}
			puzzleContainer.trialsPanel.SetTrialIcons(trialCount);
		}

		public void HighlightTrial(int trial, bool highlight) {
			if (trial < 0 || trial >= trials.Count) return;
			trials[trial].HighlightTrial(highlight);
		}

		public void UpdateTrial(int trial, int digit, Color color, Color subcolor, int order) {
			if (trial < 0 || trial >= trials.Count) return;
			trials[trial].InputDigit(digit, color, subcolor, order);
		}

		public void UpdateRuleSet(List<PuzzleRule> ruleSet) {
			ruleContainer.EmptyRule();
			int order = 1;
			foreach (PuzzleRule rule in ruleSet) {
				ruleContainer.Addrule(rule, order);
				order++;
			}
		}

		public void UpdateTrials() {
			puzzleContainer.trialsPanel.MinusLive();
		}

		public void Empty() {
			foreach (Trial trial in trials) {
				if (trial != null) Object.Destroy(trial.gameObject);
			}
			trials.Clear();
			ruleContainer.EmptyRule();
			puzzleContainer.trialsPanel.ClearTrialIcons();
		}

		public void Show() {
			if (isAnimating) return;
			co_animating = runner.StartCoroutine(Showing());
		}

		public void Hide() {
			if (co_animating != null) {
				runner.StopCoroutine(co_animating);
				co_animating = null;
			}
			co_animating = runner.StartCoroutine(Hiding());
		}

		private IEnumerator Showing() {
			puzzleContainer.containerRoot.gameObject.SetActive(true);
			ruleContainer.containerRoot.gameObject.SetActive(true);

			puzzleSideBox.DisableHover();
			ruleSideBox.DisableHover();

			puzzleSideBox.SlideTo(puzzleSideBox.peekXPos);
			ruleSideBox.SlideTo(ruleSideBox.peekXPos);

			yield return new WaitUntil(() => !puzzleSideBox.isSliding && !ruleSideBox.isSliding);

			puzzleSideBox.EnableHover();
			ruleSideBox.EnableHover();

			co_animating = null;
		}

		private IEnumerator Hiding() {
			puzzleSideBox.DisableHover();
			ruleSideBox.DisableHover();

			puzzleSideBox.SlideTo(puzzleSideBox.hideXPos);
			ruleSideBox.SlideTo(ruleSideBox.hideXPos);

			yield return new WaitUntil(() => !puzzleSideBox.isSliding && !ruleSideBox.isSliding);

			puzzleContainer.containerRoot.gameObject.SetActive(false);
			ruleContainer.containerRoot.gameObject.SetActive(false);

			co_animating = null;
		}
	}
}
