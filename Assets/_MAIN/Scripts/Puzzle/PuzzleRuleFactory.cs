using System;
using System.Collections.Generic;
using UnityEngine;

namespace origin.puzzle {
	/// <summary>
	/// Factory class for creating standard puzzle rules and custom rule sets
	/// </summary>
	public static class PuzzleRuleFactory {

		private const string TABLE = "0123456789";

		// color table
		private static Color red = new(0.47f, 0.28f, 0.28f);
		private static Color orange = new(0.47f, 0.3f, 0.2f);
		private static Color yellow = new(0.56f, 0.47f, 0.1f);
		private static Color green = new(0.24f, 0.41f, 0.24f);
		private static Color blue = new(0.23f, 0.29f, 0.41f);
		private static Color purple = new(0.36f, 0.28f, 0.39f);
		private static Color grey = new(0.73f, 0.73f, 0.73f);

		public static PuzzleRule CreateStrikeRule(Color strikeColor) {
			return new PuzzleRule(
				id: "strike",
				color: strikeColor,
				title: "strike",
				description: "strike",
				checkCondition: (answer, guess, position) => answer[position] == guess,
				audioPitch: 1.3f,
				scoreValue: 1,
				characterEffect: "shiver"
			);
		}

		public static PuzzleRule CreateSemiStrikeRule(Color semiStrikeColor) {
			return new PuzzleRule(
				id: "semi-strike",
				color: semiStrikeColor,
				title: "strike",
				description: "Digit at this position is congruent modulo 5 (same remainder when divided by 5)",
				checkCondition: (answer, guess, position) => {
					int answerIndex = TABLE.IndexOf(answer[position]);
					int guessIndex = TABLE.IndexOf(guess);
					// Check if both indices are valid and congruent mod 5
					return answerIndex != -1 && guessIndex != -1 && answerIndex % 5 == guessIndex % 5;
				},
				audioPitch: 1.0f,
				scoreValue: 0
			);
		}

		public static PuzzleRule CreateBallRule(Color ballColor) {
			return new PuzzleRule(
				id: "ball",
				color: ballColor,
				title: "strike",
				description: "Correct digit exists in answer but wrong position",
				checkCondition: (answer, guess, position) => answer.Contains(guess),
				audioPitch: 0.7f,
				scoreValue: 0
			);
		}

		public static PuzzleRule CreateMissRule() {
			return new PuzzleRule(
				id: "miss",
				color: grey, // #BBBBBB in RGB
				title: "_Miss",
				description: "_Digit is not in the answer",
				checkCondition: (answer, guess, position) => true, // Always true - this is the fallback
				audioPitch: 1.0f,
				scoreValue: 0
			);
		}

		public static List<PuzzleRule> CreateRuleSet(string ruleSetCode) {
			switch(ruleSetCode) {
				case "classic":
					return new List<PuzzleRule> {
						CreateStrikeRule(red),
						CreateSemiStrikeRule(purple),
						CreateBallRule(blue),
						CreateMissRule()
					};
				case "juhyang01":
					return new List<PuzzleRule> {
						CreateStrikeRule(red),
						CreateParityRule(orange),
						CreateBallRule(yellow),
						CreateMissRule()
					};
				case "doeun01":
					return new List<PuzzleRule> {
						// not implemented yet
						CreateMissRule()
					};
				default:
					return new List<PuzzleRule> {
						// no rule for default
						CreateMissRule()
					};
			}	
		}

		public static PuzzleRule CreateParityRule(Color parityColor) {
			return new PuzzleRule(
				id: "parity-match",
				color: parityColor,
				title: "strike",
				description: "Digit has same parity (even/odd) as answer digit at this position",
				checkCondition: (answer, guess, position) => {
					int answerDigit = answer[position] - '0';
					int guessDigit = guess - '0';
					return (answerDigit % 2) == (guessDigit % 2);
				},
				audioPitch: 0.9f,
				scoreValue: 0
			);
		}

		public static PuzzleRule CreateRangeRule(Color rangeColor) {
			return new PuzzleRule(
				id: "range-match",
				color: rangeColor,
				title: "strike",
				description: "Digit is within ±1 of the answer digit at this position",
				checkCondition: (answer, guess, position) => {
					int answerDigit = answer[position] - '0';
					int guessDigit = guess - '0';
					return Math.Abs(answerDigit - guessDigit) <= 1;
				},
				audioPitch: 0.8f,
				scoreValue: 0
			);
		}

		public static PuzzleRule CreateGreaterThanRule(Color greaterColor) {
			return new PuzzleRule(
				id: "greater-than",
				color: greaterColor,
				title: "strike",
				description: "Guessed digit is greater than the answer digit at this position",
				checkCondition: (answer, guess, position) => {
					int answerDigit = answer[position] - '0';
					int guessDigit = guess - '0';
					return guessDigit > answerDigit;
				},
				audioPitch: 0.85f,
				scoreValue: 0
			);
		}

		public static PuzzleRule CreateLessThanRule(Color lessColor) {
			return new PuzzleRule(
				id: "less-than",
				color: lessColor,
				title: "strike",
				description: "Guessed digit is less than the answer digit at this position",
				checkCondition: (answer, guess, position) => {
					int answerDigit = answer[position] - '0';
					int guessDigit = guess - '0';
					return guessDigit < answerDigit;
				},
				audioPitch: 0.85f,
				scoreValue: 0
			);
		}
	}
}
