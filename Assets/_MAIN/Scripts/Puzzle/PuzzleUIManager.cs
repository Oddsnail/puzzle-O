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
		public bool isAnimating => co_animating != null;

		private const string Starter = "";
		private const string Ender = "";

		private const float defaultHideXPos = 1250.0f;
		private const float defaultPeekXPos = 1050.0f;

		public void Initialize(PuzzleContainer puzzleContainer, RuleContainer ruleContainer, MonoBehaviour runner) {
			this.puzzleContainer = puzzleContainer;
			this.ruleContainer = ruleContainer;
			this.runner = runner;

			puzzleSideBox = puzzleContainer.containerRoot.GetComponent<SideBox>();
			ruleSideBox = ruleContainer.containerRoot.GetComponent<SideBox>();

			puzzleSideBox.hideXPos = defaultHideXPos;
			puzzleSideBox.peekXPos = defaultPeekXPos;
			puzzleSideBox.showXPos = 0f;

			ruleSideBox.hideXPos = -defaultHideXPos;
			ruleSideBox.peekXPos = -defaultPeekXPos;
			ruleSideBox.showXPos = 0f;
		}

		public void SetThemeColor(Color color) {
			puzzleContainer.puzzleColor.color = color;
			ruleContainer.puzzleColor.color = color;
		}

		public void UpdateHistory(string history) {
			puzzleContainer.historyText.text = Starter + history + Ender;
		}

		public void UpdateRuleSet(List<PuzzleRule> ruleSet) {
			ruleContainer.EmptyRule();
			int order = 1;
			foreach (PuzzleRule rule in ruleSet) {
				if (rule.ruleID == "miss") break;
				ruleContainer.Addrule(rule, order);
				order++;
			}
		}

		public void UpdateTrials(int remaining, int total) {
			puzzleContainer.trialsText.text = $"{remaining}/{total}";
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

			puzzleSideBox.SlideTo(defaultPeekXPos);
			ruleSideBox.SlideTo(-defaultPeekXPos);

			yield return new WaitUntil(() => !puzzleSideBox.isSliding && !ruleSideBox.isSliding);

			puzzleSideBox.EnableHover();
			ruleSideBox.EnableHover();

			co_animating = null;
		}

		private IEnumerator Hiding() {
			puzzleSideBox.DisableHover();
			ruleSideBox.DisableHover();

			puzzleSideBox.SlideTo(defaultHideXPos);
			ruleSideBox.SlideTo(-defaultHideXPos);

			yield return new WaitUntil(() => !puzzleSideBox.isSliding && !ruleSideBox.isSliding);

			puzzleContainer.containerRoot.gameObject.SetActive(false);
			ruleContainer.containerRoot.gameObject.SetActive(false);

			co_animating = null;
		}
	}
}
