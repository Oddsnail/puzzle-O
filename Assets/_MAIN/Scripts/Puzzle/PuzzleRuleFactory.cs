using System;
using System.Collections.Generic;
using UnityEngine;

using origin.graphic;

namespace origin.puzzle {
	/// <summary>
	/// Factory class for creating standard puzzle rules and custom rule sets
	/// </summary>
	public static class PuzzleRuleFactory {

		private const string TABLE = "0123456789";

		private static Color grey = new(0.40f, 0.40f, 0.40f);

		public static PuzzleRule CreateStrikeRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "strike",
				color: color,
				subcolor: subcolor,
				checkCondition: (answer, guess, position) => answer[position] == guess,
				audioPitch: 1.3f,
				scoreValue: 5,
				characterEffect: "shiver"
			);
		}

		public static PuzzleRule CreateSemiStrikeRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "semi-strike",
				color: color,
				subcolor: subcolor,
				checkCondition: (answer, guess, position) => {
					int answerIndex = TABLE.IndexOf(answer[position]);
					int guessIndex = TABLE.IndexOf(guess);
					return answerIndex != -1 && guessIndex != -1 && answerIndex % 5 == guessIndex % 5;
				},
				audioPitch: 1.0f,
				scoreValue: 4,
				characterEffect: "shiver"
			);
		}

		public static PuzzleRule CreateBallRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "ball",
				color: color,
				subcolor: subcolor,
				checkCondition: (answer, guess, position) => answer.Contains(guess),
				audioPitch: 0.7f,
				scoreValue: 3,
				characterEffect: "crouch"
			);
		}

		// always true, bottom rule.
		public static PuzzleRule CreateMissRule() {
			return new PuzzleRule(
				id: "miss",
				color: grey,
				subcolor: grey,
				checkCondition: (answer, guess, position) => true,
				audioPitch: 1.0f,
				scoreValue: 0
			);
		}

		public static PuzzleRule CreateParityRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "parity-match",
				color: color,
				subcolor: subcolor,
				checkCondition: (answer, guess, position) => {
					int answerDigit = answer[position] - '0';
					int guessDigit = guess - '0';
					return (answerDigit % 2) == (guessDigit % 2);
				},
				audioPitch: 0.9f,
				scoreValue: 3
			);
		}

		public static PuzzleRule CreateRangeRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "range-match",
				color: color,
				subcolor: subcolor,
				checkCondition: (answer, guess, position) => {
					int answerDigit = answer[position] - '0';
					int guessDigit = guess - '0';
					return Math.Abs(answerDigit - guessDigit) <= 1;
				},
				audioPitch: 0.8f,
				scoreValue: 4,
				characterEffect: "crouch"
			);
		}

		public static PuzzleRule CreateGreaterThanRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "greater-than",
				color: color,
				subcolor: subcolor,
				checkCondition: (answer, guess, position) => {
					int answerDigit = answer[position] - '0';
					int guessDigit = guess - '0';
					return guessDigit < answerDigit;
				},
				audioPitch: 0.85f,
				scoreValue: 2
			);
		}

		public static PuzzleRule CreateLessThanRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "less-than",
				color: color,
				subcolor: subcolor,
				checkCondition: (answer, guess, position) => {
					int answerDigit = answer[position] - '0';
					int guessDigit = guess - '0';
					return guessDigit > answerDigit;
				},
				audioPitch: 0.85f,
				scoreValue: 2
			);
		}

		public static PuzzleRule CreateGhostRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "ghost",
				color: color,
				subcolor: subcolor,
				checkCondition: (answer, guess, position) => !answer.Contains(guess),
				audioPitch: 0.85f,
				scoreValue: 0
			);
		}

		private static PuzzleRule CreateRuleByID(string id, Color color, Color subcolor) {
			switch (id) {
				case "strike": return CreateStrikeRule(color, subcolor);
				case "semi-strike": return CreateSemiStrikeRule(color, subcolor);
				case "ball": return CreateBallRule(color, subcolor);
				case "parity-match": return CreateParityRule(color, subcolor);
				case "range-match": return CreateRangeRule(color, subcolor);
				case "greater-than": return CreateGreaterThanRule(color, subcolor);
				case "less-than": return CreateLessThanRule(color, subcolor);
				case "ghost": return CreateGhostRule(color, subcolor);
				default:
					Debug.LogWarning($"Unknown rule ID: {id}");
					return null;
			}
		}

		public static List<PuzzleRule> CreateRuleSet(RulePalette rulePalette) {
			List<PuzzleRule> rules = new();
			foreach (Palette p in rulePalette.palettes) {

				PuzzleRule rule = CreateRuleByID(p.ID, p.primary, p.secondary);
				if (rule != null) {
					rules.Add(rule);
				}
			}
			rules.Add(CreateMissRule());
			return rules;
		}
	}
}
