using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using origin.graphic;

namespace origin.puzzle {
	public class PuzzleManager : MonoBehaviour, IPuzzleUI {

		public static PuzzleManager instance;

		public PuzzleContainer puzzleContainer = new();
		public RuleContainer ruleContainer = new();
		public ColorPalette colorPalette;
		public RuleSetPalette ruleSetPalettes;

		private PuzzleUIManager puzzleUIManager;
		private StageManager stageManager;

		public delegate void PuzzleEvent(char guess);
		public event PuzzleEvent onCharacterGuess;

		public bool isAnimating => puzzleUIManager != null && puzzleUIManager.isAnimating;

		bool initialized = false;

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

			puzzleUIManager = new();
			puzzleUIManager.Initialize(puzzleContainer, ruleContainer, this);

			stageManager = new(this, colorPalette, ruleSetPalettes, this);

			onCharacterGuess += stageManager.OnCharacterGuess;
		}

		private void OnDestroy() {
			if (instance == this) {
				if (stageManager != null) onCharacterGuess -= stageManager.OnCharacterGuess;
			}
		}

		public void OnCharacterGuess(char guess) => onCharacterGuess.Invoke(guess);

		public IEnumerator StartPuzzle(string charID, int trial, Action<bool> onResult, string ruleSetCode = "classic") {
			yield return stageManager.StartStage(charID, trial, onResult, ruleSetCode);
		}

		// IPuzzleUI delegation
		public void Show() => puzzleUIManager.Show();
		public void Hide() => puzzleUIManager.Hide();
		public void SetThemeColor(Color color) => puzzleUIManager.SetThemeColor(color);
		public void UpdateHistory(string history) => puzzleUIManager.UpdateHistory(history);
		public void UpdateRuleSet(List<PuzzleRule> ruleSet) => puzzleUIManager.UpdateRuleSet(ruleSet);
		public void UpdateTrials(int remaining, int total) => puzzleUIManager.UpdateTrials(remaining, total);
	}
}
