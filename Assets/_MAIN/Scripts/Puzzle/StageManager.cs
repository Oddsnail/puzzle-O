using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using origin.audio;
using origin.character;
using origin.graphic;

namespace origin.puzzle {
	public class StageManager {

		private readonly IPuzzleUI puzzleUI;
		private readonly MonoBehaviour runner;
		private readonly ColorPalette colorPalette;
		private readonly RuleSetPalette ruleSetPalette;

		private Coroutine co_stage = null;
		public bool isRunning => co_stage != null;

		private List<PuzzleRule> activeRules;
		private bool guessing = false;
		private char recentChoice = ' ';

		private const string TABLE = "0123456789";

		public StageManager(IPuzzleUI puzzleUI, ColorPalette colorPalette, RuleSetPalette ruleSetPalette, MonoBehaviour runner) {
			this.puzzleUI = puzzleUI;
			this.colorPalette = colorPalette;
			this.ruleSetPalette = ruleSetPalette;
			this.runner = runner;
		}

		public Coroutine StartStage(string charID, int trial, Action<bool> onResult, string ruleSetCode = "classic") {
			StopStage();
			co_stage = runner.StartCoroutine(RunningStage(charID, trial, onResult, ruleSetCode));
			return co_stage;
		}

		public void StopStage() {
			if (!isRunning) return;
			runner.StopCoroutine(co_stage);
			co_stage = null;
			puzzleUI.Hide();
		}

		public void OnCharacterGuess(char guess) {
			if (guessing) recentChoice = guess;
		}

		private IEnumerator RunningStage(string charID, int trial, Action<bool> onResult, string ruleSetCode) {

			activeRules = null;
			foreach (RulePalette rp in ruleSetPalette.rulePalettes) {
				if (rp.ruleSetCode == ruleSetCode) {
					activeRules = PuzzleRuleFactory.CreateRuleSet(rp);
					break;
				}
			}
			if (activeRules == null) {
				Debug.LogWarning($"Rule set with code '{ruleSetCode}' not found in RuleSetPalette.");
				activeRules = PuzzleRuleFactory.CreateRuleSet(ruleSetPalette.rulePalettes[0]);
			}

			string ID = charID + "-client";
			puzzleUI.UpdateTrials(trial, trial);

			if (!CharacterManager.instance.HasCharacter(ID)) CharacterManager.instance.AddClient(charID);
			CHARACTER character = CharacterManager.instance.GetCharacter(ID);

			Color theme = colorPalette.Getcolor(character.config.ID);
			puzzleUI.SetThemeColor(theme);
			yield return new WaitForSeconds(0.7f);

			string answer = GenerateUniqueFourDigitNumber();
			string history = "";
			puzzleUI.UpdateHistory(history);
			puzzleUI.UpdateRuleSet(activeRules);

			bool successed = false;

			Debug.Log($"answer is : {answer}");

			puzzleUI.Show();
			character.Appear();

			for (int i = 0; i < trial; i++) {
				int score = 0;
				for (int choice = 0; choice < 4; choice++) {
					guessing = true;

					yield return new WaitUntil(() => recentChoice != ' ');

					bool ruleMatched = false;
					foreach (PuzzleRule rule in activeRules) {
						if (rule.Evaluate(answer, recentChoice, choice)) {
							history += $"<color=#{ColorUtility.ToHtmlStringRGB(rule.color)}>" + recentChoice + "</color>";

							string soundEffect = rule.ruleID == "miss" ? "sfx/puzzle-fail" : "sfx/puzzle-strike";
							AudioManager.instance.PlaySoundEffect(soundEffect, pitch: rule.audioPitch);

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
						history += $"<color=#BBBBBB>" + recentChoice + "</color>";
						AudioManager.instance.PlaySoundEffect("sfx/puzzle-fail", pitch: 1.0f);
					}

					puzzleUI.UpdateHistory(history);

					guessing = false;
					recentChoice = ' ';
				}
				history += "\n";
				puzzleUI.UpdateTrials(trial - 1 - i, trial);
				AudioManager.instance.PlaySoundEffect("sfx/dialogue-3");

				if (score == 4) {
					successed = true;
					break;
				}
			}

			yield return new WaitForSeconds(0.1f);
			puzzleUI.Hide();
			character.Disappear();
			co_stage = null;
			onResult?.Invoke(successed);
			yield return new WaitForSeconds(0.1f);
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
	}
}
