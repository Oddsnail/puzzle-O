using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using origin.graphic;
using origin.tutorial;

namespace origin.puzzle {
	public class PuzzleManager : MonoBehaviour, IPuzzleUI {

		public static PuzzleManager instance;

		public PuzzleContainer puzzleContainer = new();
		public RuleContainer ruleContainer = new();
		public ColorPalette colorPalette;
		public RuleSetPalette ruleSetPalettes;
		
		public GameObject reactionTextBox;
		public TextMeshProUGUI reactionText;

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

		public IEnumerator StartPuzzle(string charID, int digitCount, int trial, Action<bool> onResult, string ruleSetCode = "classic") {
			puzzleUIManager.SetupTrials(digitCount, trial);
			yield return stageManager.StartStage(charID, digitCount, trial, onResult, ruleSetCode);
		}

		private readonly Dictionary<string, (int, int, string, string)> tutorialIDdict = new() {
			{"basic", (4, 6, "classic", "6537") },
			{"gray", (4, 6, "mystery", "_")}
		};

		public IEnumerator StartPuzzleWithTutorial(string charID, string tutorialID, Action<bool> onResult) {
			var tutorialInfo = tutorialIDdict[tutorialID];
			puzzleUIManager.SetupTrials(tutorialInfo.Item1, tutorialInfo.Item2);
			yield return stageManager.StartStage(charID, tutorialInfo.Item1, tutorialInfo.Item2, onResult, tutorialInfo.Item3, tutorialInfo.Item4, tutorialID);
		}

		// IPuzzleUI delegation
		public void Show() => puzzleUIManager.Show();
		public void Hide() => puzzleUIManager.Hide();
		public void SetThemeColor(Color color) => puzzleUIManager.SetThemeColor(color);
		public void SetupTrials(int digitCount, int trialCount) => puzzleUIManager.SetupTrials(digitCount, trialCount);
		public void HighlightTrial(int trial, bool highlight) => puzzleUIManager.HighlightTrial(trial, highlight);
		public void UpdateRuleSet(List<PuzzleRule> ruleSet, string callerID) => puzzleUIManager.UpdateRuleSet(ruleSet, callerID);
		public void UpdateTrial(int trial, int digit, Color color, Color subcolor, int order, bool doHitEfect)
			=> puzzleUIManager.UpdateTrial(trial, digit, color, subcolor, order, doHitEfect);
		public void UpdateTrials() => puzzleUIManager.UpdateTrials();
		public void Empty() => puzzleUIManager.Empty();
	}
}
