using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using origin.audio;
using origin.character;
using origin.graphic;
using origin.tutorial;
using origin.dialogue;
using origin.language;

namespace origin.puzzle {
	public class StageManager {

		private readonly IPuzzleUI puzzleUI;
		private readonly PuzzleManager runner;
		private readonly ColorPalette colorPalette;
		private readonly RuleSetPalette ruleSetPalette;

		private TextArchitect textArchitect;

		private Coroutine co_stage = null;
		private Coroutine co_reactioning;
		public bool isRunning => co_stage != null;
		public bool isReactioning => co_reactioning != null;

		private List<PuzzleRule> activeRules;
		private bool guessing = false;
		private char recentChoice = ' ';
		private bool[] usedDigits = new bool[10];

		private const string TABLE = "0123456789";

		public StageManager(IPuzzleUI puzzleUI, ColorPalette colorPalette, RuleSetPalette ruleSetPalette, PuzzleManager runner) {
			this.puzzleUI = puzzleUI;
			this.colorPalette = colorPalette;
			this.ruleSetPalette = ruleSetPalette;
			this.runner = runner;
			textArchitect = new TextArchitect(runner.reactionText);
		}

		public Coroutine StartStage(string charID,
									int digitCount,
									int trial,
									Action<bool> onResult,
									string ruleSetCode = "classic",
									string predefinedAnswer = "_",
									string tutorialID = "_") 
		{
			StopStage();
			co_stage = runner.StartCoroutine(RunningStage(charID, digitCount, trial, onResult, ruleSetCode, predefinedAnswer, tutorialID));
			return co_stage;
		}

		public void StopStage() {
			if (!isRunning) return;
			runner.StopCoroutine(co_stage);
			if (isReactioning) runner.StopCoroutine(co_reactioning);
			co_stage = null;
			co_reactioning = null;
			puzzleUI.Hide();
		}

		public void OnCharacterGuess(char guess) {
			if (guessing) {
				if (usedDigits[guess - '0']) {
					AudioManager.instance.PlayPreloadedSFX("puzzleFail");
					return;
				}
				recentChoice = guess;
				usedDigits[guess - '0'] = true;
			}
		}

		private IEnumerator RunningStage(string charID,
										int digitCount,
										int trial,
										Action<bool> onResult,
										string ruleSetCode,
										string predefinedAnswer,
										string tutorialID) {

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
			if (!CharacterManager.instance.HasCharacter(ID)) CharacterManager.instance.AddClient(charID);
			CHARACTER character = CharacterManager.instance.GetCharacter(ID);
			Color theme = colorPalette.Getcolor(charID);
			puzzleUI.SetThemeColor(theme);
			yield return new WaitForSeconds(0.5f);

			// ** ANSWER GENERATION **
			string answer = "1234";
			if (predefinedAnswer != "_") answer = predefinedAnswer;
			else answer = GenerateUnique_nDigitNumber(digitCount);
			puzzleUI.UpdateRuleSet(activeRules, charID);

			Debug.Log($"answer is : {answer}");

			// ** PREPARING **
			yield return BackgroundManager.instance.ManualFogOn(true);
			yield return new WaitForSeconds(1f);

			BackgroundManager.instance.ManualTransition(BackgroundManager.instance.background, $"Graphics/Backgrounds/client_{charID}");
			puzzleUI.Show();
			character.Appear();
			runner.reactionTextBox.transform.SetParent(character.rectTransform);
			runner.reactionTextBox.SetActive(false);
			PuzzleManager.instance.reactionTextBox.GetComponent<RectTransform>().anchoredPosition = character.config.reactionLocation;
			if (tutorialID != "_") TutorialManager.instance.StartTutorial(tutorialID);
			
			yield return new WaitForSeconds(0.3f);
			yield return BackgroundManager.instance.ManualFogOn(false);

			// ** PLAY **
			bool successed = false;
			int recentEffect = 0;
			for (int i = 0; i < trial; i++) {
				int score = 0;
				Array.Fill(usedDigits, false);
				puzzleUI.HighlightTrial(i, true);
				for (int choice = 0; choice < digitCount; choice++) {
					guessing = true;
					int matchingRuleOrder = 1;

					Coroutine idleWait = runner.StartCoroutine(IdleTracker(character));
					yield return new WaitUntil(() => recentChoice != ' ');
					runner.StopCoroutine(idleWait);

					PuzzleRule matchedRule = null;
					foreach (PuzzleRule rule in activeRules) {
						if (rule.Evaluate(answer, recentChoice, choice)) {

							if (rule.ruleID != "miss") AudioManager.instance.PlayPreloadedSFX("puzzleSuccess", pitch: rule.audioPitch);
							else AudioManager.instance.PlayPreloadedSFX("puzzleFail", pitch: rule.audioPitch);

							score += recentChoice == answer[choice] ? 5 : rule.scoreValue;
							
							int dialogueEventCoeff = UnityEngine.Random.Range(0, 10);
							if (dialogueEventCoeff < 7 & recentEffect == 0) {
								recentEffect = 3;
								switch (rule.characterEffect) {
									case "shiver":
										character.Shiver();
										break;
									case "crouch":
										character.Crouch();
										break;
									default:
										break;
								}
							}

							matchedRule = rule;
							break;
						}
						matchingRuleOrder++;
					}

					if (matchedRule == null) {
						Debug.LogWarning($"No matching rule found for guess '{recentChoice}' at position {choice} in trial {i + 1}");
						break;
					}

					puzzleUI.UpdateTrial(i, recentChoice - '0', matchedRule.color, matchedRule.subcolor, matchingRuleOrder, matchedRule.ruleID != "miss");

					if (recentEffect > 0) recentEffect --;
					guessing = false;
					recentChoice = ' ';
				}
				puzzleUI.UpdateTrials();
				puzzleUI.HighlightTrial(i, false);
				AudioManager.instance.PlayPreloadedSFX("nextTrial");

				// full score is digit * 5
				// { 0       digitcount  	 			    digitcount*3  		 digitcount*4 	       digitcount*5      }
				// {    BAD   		      	  NORMAL  	                  GOOD    			   BETTER             = Best }

				if (score == digitCount * 5) {
					character.Hop();
					Reaction(character, ClientReactionType.Best);
					successed = true;
					break;
				}
				else if (score >= digitCount * 4) Reaction(character, ClientReactionType.Better);
				else if (score >= digitCount * 3) Reaction(character, ClientReactionType.Good);
				else if (score >= digitCount) Reaction(character, ClientReactionType.Normal);
				else Reaction(character, ClientReactionType.Bad);
				character.Crouch();

				yield return new WaitForSeconds(0.3f);
			}

			yield return new WaitForSeconds(3f);
			yield return BackgroundManager.instance.ManualFogOn(true);
			yield return new WaitForSeconds(1f);

			BackgroundManager.instance.ManualTransition(BackgroundManager.instance.background, "Graphics/Backgrounds/bg_default");
			puzzleUI.Hide();
			puzzleUI.Empty();
			character.Disappear();
			onResult?.Invoke(successed);
			runner.reactionTextBox.transform.SetParent(runner.GetComponent<RectTransform>());
			if (tutorialID != "_") TutorialManager.instance.StopTutorial();

			yield return new WaitForSeconds(0.3f);
			yield return BackgroundManager.instance.ManualFogOn(false);
			CharacterManager.instance.RemoveCharacter(ID);

			co_stage = null;
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

		public void Reaction(CHARACTER character, ClientReactionType type) {
			if (isReactioning) runner.StopCoroutine(co_reactioning);
			co_reactioning = runner.StartCoroutine(Reactioning(character, type));
		}

		private IEnumerator Reactioning(CHARACTER character, ClientReactionType type) {
			runner.reactionTextBox.SetActive(true);
			textArchitect.ForceComplete();
			runner.reactionText.text = "";
			CharacterConfigData config = character.config;

			int random = UnityEngine.Random.Range(1, 3);
			string text = $"{config.ID}.clientline.{type.ToString().ToLower()}.{random}";
			string resolvedText = LocalizationManager.instance != null
				? LocalizationManager.instance.Get(text)
				: "non-instantiated LocalizationManager error";
			string reactioncode = character.config.FindReaction(type);

			character.SetSprite(reactioncode);
			yield return textArchitect.Build(resolvedText);
			yield return new WaitForSeconds(2f);
			character.SetSprite(character.config.FindReaction(ClientReactionType.Normal));

			PuzzleManager.instance.reactionTextBox.SetActive(false);
			co_reactioning = null;
		}
		
		private IEnumerator IdleTracker(CHARACTER character)
		{
			while (true)
			{
				yield return new WaitForSeconds(30f);
				if (!TutorialManager.instance.IsRunning) {
					character.Crouch();
					yield return Reactioning(character, ClientReactionType.Idle);
				}
			}
		}
	}
}
