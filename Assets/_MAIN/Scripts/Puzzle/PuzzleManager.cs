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
		public RuleContainer ruleContainer = new();
		public ColorPalette colorPalette;

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

		private const string Starter = "<mspace=0.51em>";
		private const string Ender = "</mspace>";
		private const string TABLE = "0123456789";
		private char recentChoice = ' ';

		private void UpdateHistory(string history) => puzzleContainer.historyText.text = Starter + history + Ender;
		private void UpdateRuleSet(List<PuzzleRule> ruleSet) {
			ruleContainer.EmptyRule();
			foreach (PuzzleRule rule in ruleSet) {
				if (rule.ruleID == "miss") break;
				ruleContainer.Addrule(rule);
			}
		}

		public IEnumerator StartPuzzle(string charID, int trial, Action<bool> onResult, string ruleSetCode = "classic") {

			activeRules = PuzzleRuleFactory.CreateRuleSet(ruleSetCode);

			string ID = charID + "-client";
			puzzleContainer.trialsText.text = $"{trial}/{trial}";

			if (!CharacterManager.instance.HasCharacter(ID)) CharacterManager.instance.AddClient(charID);
			CHARACTER character = CharacterManager.instance.GetCharacter(ID);

			Color theme = colorPalette.Getcolor(character.config.ID);
			puzzleContainer.puzzleColor.color = theme;
			ruleContainer.puzzleColor.color = theme;
			yield return new WaitForSeconds(0.7f);

			string answer = GenerateUniqueFourDigitNumber();
			string history = "";
			UpdateHistory(history);
			UpdateRuleSet(activeRules);

			bool successed = false;

			Debug.Log($"answer is : {answer}");

			Show();
			character.Appear();

			for (int i = 0; i < trial; i++) {
				int score = 0;
				for (int choice = 0; choice < 4; choice++) {
					guessing = true;

					yield return new WaitUntil(() => recentChoice != ' ');

					bool ruleMatched = false;
					foreach (PuzzleRule rule in activeRules) {
						if (rule.Evaluate(answer, recentChoice, choice)) {
							history += $"<color=#{ColorUtility.ToHtmlStringRGB(rule.color)}>" + recentChoice + "</color> ";

							string soundEffect = rule.ruleID == "miss" ? "SFX/puzzle-fail" : "SFX/puzzle-strike";
							AudioManager.Instance.PlaySoundEffect(soundEffect, pitch: rule.audioPitch);

							score += rule.scoreValue;

							if (rule.characterEffect == "shiver") {
								character.Shiver();
							}

							ruleMatched = true;
							break;
						}
					}

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

		private const float defaultShowAndHideDuration = 0.6f;
		private const float defaultContainerXPos = 850.0f;
		private const float defaultContainerHideXPosOffset = -165.0f;

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
			ruleContainer.containerRoot.gameObject.SetActive(true);

			yield return DOTween.Sequence()
				.Join(puzzleContainer.containerRoot.DOAnchorPosX(
					defaultContainerXPos + defaultContainerHideXPosOffset, 
					defaultShowAndHideDuration))
				.Join(ruleContainer.containerRoot.DOAnchorPosX(
					- defaultContainerXPos - defaultContainerHideXPosOffset, 
					defaultShowAndHideDuration))
				.WaitForCompletion();

			co_animating = null;
		}

		private IEnumerator Hiding() {

			yield return DOTween.Sequence()
				.Join(puzzleContainer.containerRoot.DOAnchorPosX(
					defaultContainerXPos, 
					defaultShowAndHideDuration))
				.Join(ruleContainer.containerRoot.DOAnchorPosX(
					- defaultContainerXPos, 
					defaultShowAndHideDuration))
				.WaitForCompletion();

			puzzleContainer.containerRoot.gameObject.SetActive(false);
			ruleContainer.containerRoot.gameObject.SetActive(false);
			co_animating = null;
		}
	}
}