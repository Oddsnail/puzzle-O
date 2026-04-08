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
				title: "game.rule.1.title",
				description: "game.rule.1.description",
				checkCondition: (answer, guess, position) => answer[position] == guess,
				audioPitch: 1.3f,
				characterEffect: "shiver"
			);
		}

		public static PuzzleRule CreateSemiStrikeRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "semi-strike",
				color: color,
				subcolor: subcolor,
				title: "game.rule.2.title",
				description: "game.rule.2.description",
				checkCondition: (answer, guess, position) => {
					int answerIndex = TABLE.IndexOf(answer[position]);
					int guessIndex = TABLE.IndexOf(guess);
					return answerIndex != -1 && guessIndex != -1 && answerIndex % 5 == guessIndex % 5;
				},
				audioPitch: 1.0f
			);
		}

		public static PuzzleRule CreateBallRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "ball",
				color: color,
				subcolor: subcolor,
				title: "game.rule.3.title",
				description: "game.rule.3.description",
				checkCondition: (answer, guess, position) => answer.Contains(guess),
				audioPitch: 0.7f
			);
		}

		public static PuzzleRule CreateMissRule() {
			return new PuzzleRule(
				id: "miss",
				color: grey,
				subcolor: grey,
				title: "game.rule.miss.title",
				description: "game.rule.miss.description",
				checkCondition: (answer, guess, position) => true,
				audioPitch: 1.0f
			);
		}

		public static PuzzleRule CreateParityRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "parity-match",
				color: color,
				subcolor: subcolor,
				title: "game.rule.4.title",
				description: "game.rule.4.description",
				checkCondition: (answer, guess, position) => {
					int answerDigit = answer[position] - '0';
					int guessDigit = guess - '0';
					return (answerDigit % 2) == (guessDigit % 2);
				},
				audioPitch: 0.9f
			);
		}

		public static PuzzleRule CreateRangeRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "range-match",
				color: color,
				subcolor: subcolor,
				title: "game.rule.5.title",
				description: "game.rule.5.description",
				checkCondition: (answer, guess, position) => {
					int answerDigit = answer[position] - '0';
					int guessDigit = guess - '0';
					return Math.Abs(answerDigit - guessDigit) <= 1;
				},
				audioPitch: 0.8f
			);
		}

		public static PuzzleRule CreateGreaterThanRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "greater-than",
				color: color,
				subcolor: subcolor,
				title: "game.rule.6.1.title",
				description: "game.rule.6.1.description",
				checkCondition: (answer, guess, position) => {
					int answerDigit = answer[position] - '0';
					int guessDigit = guess - '0';
					return guessDigit > answerDigit;
				},
				audioPitch: 0.85f
			);
		}

		public static PuzzleRule CreateLessThanRule(Color color, Color subcolor) {
			return new PuzzleRule(
				id: "less-than",
				color: color,
				subcolor: subcolor,
				title: "game.rule.6.2.title",
				description: "game.rule.6.2.description",
				checkCondition: (answer, guess, position) => {
					int answerDigit = answer[position] - '0';
					int guessDigit = guess - '0';
					return guessDigit < answerDigit;
				},
				audioPitch: 0.85f
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
