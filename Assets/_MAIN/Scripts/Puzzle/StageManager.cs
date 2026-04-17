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
		private bool[] usedDigits = new bool[10];

		private const string TABLE = "0123456789";

		public StageManager(IPuzzleUI puzzleUI, ColorPalette colorPalette, RuleSetPalette ruleSetPalette, MonoBehaviour runner) {
			this.puzzleUI = puzzleUI;
			this.colorPalette = colorPalette;
			this.ruleSetPalette = ruleSetPalette;
			this.runner = runner;
		}

		public Coroutine StartStage(string charID, int digitCount, int trial, Action<bool> onResult, string ruleSetCode = "classic") {
			StopStage();
			co_stage = runner.StartCoroutine(RunningStage(charID, digitCount, trial, onResult, ruleSetCode));
			return co_stage;
		}

		public void StopStage() {
			if (!isRunning) return;
			runner.StopCoroutine(co_stage);
			co_stage = null;
			puzzleUI.Hide();
		}

		public void OnCharacterGuess(char guess) {
			if (guessing) {
				if (usedDigits[guess - '0']) {
					AudioManager.instance.PlaySoundEffect("sfx/puzzle-fail");
					return;
				}
				recentChoice = guess;
				usedDigits[guess - '0'] = true;
			}
		}

		private IEnumerator RunningStage(string charID, int digitCount, int trial, Action<bool> onResult, string ruleSetCode) {

			// ** RULE SET SETUP **
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

			// ** UI COLOR THEME SETUP**
			string ID = charID + "-client";
			puzzleUI.UpdateTrials(trial, trial);
			// if (!CharacterManager.instance.HasCharacter(ID)) CharacterManager.instance.AddClient(charID);
			// CHARACTER character = CharacterManager.instance.GetCharacter(ID);
			Color theme = colorPalette.Getcolor(charID);
			puzzleUI.SetThemeColor(theme);
			yield return new WaitForSeconds(0.5f);

			// ** ANSWER GENERATION **
			string answer = GenerateUnique_nDigitNumber(digitCount);
			puzzleUI.UpdateRuleSet(activeRules);

			bool successed = false;

			Debug.Log($"answer is : {answer}");

			puzzleUI.Show();
			// character.Appear();

			for (int i = 0; i < trial; i++) {
				int score = 0;
				Array.Fill(usedDigits, false);
				puzzleUI.HighlightTrial(i, true);
				for (int choice = 0; choice < digitCount; choice++) {
					guessing = true;

					yield return new WaitUntil(() => recentChoice != ' ');

					PuzzleRule matchedRule = null;
					foreach (PuzzleRule rule in activeRules) {
						if (rule.Evaluate(answer, recentChoice, choice)) {
							
							string soundEffect = rule.ruleID == "miss" ? "sfx/puzzle-fail" : "sfx/puzzle-strike";
							AudioManager.instance.PlaySoundEffect(soundEffect, pitch: rule.audioPitch);

							score += recentChoice == answer[choice] ? 1 : 0;

							// if (rule.characterEffect == "shiver") {
							// 	character.Shiver();
							// }

							matchedRule = rule;
							break;
						}
					}

					if (matchedRule == null) {
						Debug.LogWarning($"No matching rule found for guess '{recentChoice}' at position {choice} in trial {i + 1}");
						break;
					}

					puzzleUI.UpdateTrial(i, recentChoice - '0', matchedRule.color, matchedRule.subcolor);

					guessing = false;
					recentChoice = ' ';
				}
				puzzleUI.UpdateTrials(trial - 1 - i, trial);
				puzzleUI.HighlightTrial(i, false);
				AudioManager.instance.PlaySoundEffect("sfx/dialogue-3");

				if (score == digitCount) {
					successed = true;
					break;
				}
			}

			yield return new WaitForSeconds(0.1f);
			puzzleUI.Hide();
			puzzleUI.Empty();
			// character.Disappear();
			co_stage = null;
			onResult?.Invoke(successed);
			yield return new WaitForSeconds(0.1f);
		}

		public static string GenerateUnique_nDigitNumber(int digitCount) {
			List<char> numbers = new();
			HashSet<char> uniqueness = new();

			while (numbers.Count < digitCount) {
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
