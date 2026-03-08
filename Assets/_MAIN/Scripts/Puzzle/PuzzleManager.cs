using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

using origin.audio;
using origin.character;
using origin.graphic;

namespace origin.puzzle {
	public class PuzzleManager : MonoBehaviour {

		public static PuzzleManager instance;

		public PuzzleContainer puzzleContainer = new();
		public ColorPalette colorPalette;

		public Color strike;
		public Color semiStrike;
		public Color ball;

		// Active rule set for the current puzzle
		private List<PuzzleRule> activeRules;

		public delegate void PuzzleEvent(char guess);
		public event PuzzleEvent onCharacterGuess;

		private Coroutine co_animating = null;
		public bool isAnimating => co_animating != null;

		bool initialized = false;
		bool guessing = false;

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
			onCharacterGuess += CharacterTaken;
		}

		private void OnDestroy() {
			if (instance == this) {
				onCharacterGuess -= CharacterTaken;
			}
		}

		public void OnCharacterGuess(char guess) => onCharacterGuess.Invoke(guess);

		//========================================================================
		//                       <!!!> Puzzle system <!!!>
		//========================================================================

		private const string Starter = "<mspace=0.51em>";
		private const string Ender = "</mspace>";
		private const string TABLE = "0123456789";
		private char recentChoice = ' ';

		private void UpdateHistory(string history) => puzzleContainer.historyText.text = Starter + history + Ender;

		/// <summary>
		/// Starts a puzzle with custom rules or default rules if none provided
		/// </summary>
		/// <param name="charID">Character ID for the puzzle</param>
		/// <param name="trial">Number of attempts allowed</param>
		/// <param name="onResult">Callback with success/failure result</param>
		/// <param name="customRules">Optional custom rule set. If null, uses default rules</param>
		public IEnumerator StartPuzzle(string charID, int trial, Action<bool> onResult, List<PuzzleRule> customRules = null) {

			// Initialize rules: use custom rules if provided, otherwise use default rule set
			activeRules = customRules ?? PuzzleRuleFactory.CreateDefaultRuleSet(strike, semiStrike, ball);

			string ID = charID + "-client";
			puzzleContainer.trialsText.text = $"{trial}/{trial}";

			if (!CharacterManager.instance.HasCharacter(ID)) CharacterManager.instance.AddClient(charID);
			CHARACTER character = CharacterManager.instance.GetCharacter(ID);

			Color theme = colorPalette.Getcolor(character.config.ID);
			puzzleContainer.puzzleColor.color = theme;
			yield return new WaitForSeconds(0.7f);

			string answer = GenerateUniqueFourDigitNumber();
			string history = "";
			UpdateHistory(history);

			bool successed = false;


			Debug.Log($"answer is : {answer}");

			Show();
			character.Appear();

			for (int i = 0; i < trial; i++) {
				int score = 0;
				for (int choice = 0; choice < 4; choice++) {
					guessing = true;

					yield return new WaitUntil(() => recentChoice != ' ');

					// Evaluate rules in order - first matching rule wins
					bool ruleMatched = false;
					foreach (PuzzleRule rule in activeRules) {
						if (rule.Evaluate(answer, recentChoice, choice)) {
							// Apply the matched rule
							history += $"<color=#{ColorUtility.ToHtmlStringRGB(rule.color)}>" + recentChoice + "</color> ";

							// Play appropriate sound effect
							string soundEffect = rule.ruleID == "miss" ? "SFX/puzzle-fail" : "SFX/puzzle-strike";
							AudioManager.Instance.PlaySoundEffect(soundEffect, pitch: rule.audioPitch);

							// Add score if applicable
							score += rule.scoreValue;

							// Trigger character effect if specified
							if (rule.characterEffect == "shiver") {
								character.Shiver();
							}

							ruleMatched = true;
							break; // Stop checking rules once we find a match
						}
					}

					// Fallback if no rule matched (shouldn't happen if rules are set up correctly)
					if (!ruleMatched) {
						Debug.LogWarning($"No puzzle rule matched for guess '{recentChoice}' at position {choice}");
						history += $"<color=#BBBBBB>" + recentChoice + "</color> ";
						AudioManager.Instance.PlaySoundEffect("SFX/puzzle-fail", pitch: 1.0f);
					}

					UpdateHistory(history);

					guessing = false;
					recentChoice = ' ';
				}
				history += "\n";
				puzzleContainer.trialsText.text = $"{trial - 1 - i}/{trial}";
				AudioManager.Instance.PlaySoundEffect("SFX/dialogue-3");

				if (score == 4) {
					successed = true;
					break;
				}
			}

			yield return new WaitForSeconds(0.1f);
			Hide();
			character.Disappear();
			onResult?.Invoke(successed);
			yield return new WaitForSeconds(0.1f);
		}

		private void CharacterTaken(char guess) {
			if (guessing) recentChoice = guess;
		}

		public static string GenerateUniqueFourDigitNumber() {
			List<char> numbers = new();
			HashSet<char> uniqueness = new();

			while (numbers.Count < 4) {
				int digit = UnityEngine.Random.Range(0, 10);
				if (!uniqueness.Contains(TABLE[digit])) {
					numbers.Add(TABLE[digit]);
					uniqueness.Add(TABLE[digit]);
				}
			}

			return new(numbers.ToArray());
		}

		//========================================================================
		//        <!!!> show / hide puzzle UI by puzzle start / end <!!!>
		//========================================================================

		private const float defaultShowAndHideDuration = 0.6f;
		private const float defaultContainerXPos = 1700.0f;

		private void Show() {
			if (isAnimating) return;

			co_animating = StartCoroutine(Showing());
		}

		private void Hide() {
			if (isAnimating) return;
			co_animating = StartCoroutine(Hiding());
		}

		private IEnumerator Showing() {
			puzzleContainer.containerRoot.gameObject.SetActive(true);

			yield return DOTween.Sequence()
				.Join(puzzleContainer.containerRoot.DOAnchorPosX(0.0f, defaultShowAndHideDuration))
				.WaitForCompletion();

			co_animating = null;
		}

		private IEnumerator Hiding() {

			yield return DOTween.Sequence()
				.Join(puzzleContainer.containerRoot.DOAnchorPosX(defaultContainerXPos, defaultShowAndHideDuration))
				.WaitForCompletion();

			puzzleContainer.containerRoot.gameObject.SetActive(false);
			co_animating = null;
		}

		//========================================================================
		//========================================================================
	}
}