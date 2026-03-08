using System;
using System.Collections.Generic;
using UnityEngine;

namespace origin.puzzle {
	/// <summary>
	/// Factory class for creating standard puzzle rules and custom rule sets
	/// </summary>
	public static class PuzzleRuleFactory {

		// Reference to the digit table used in puzzle logic
		private const string TABLE = "0123456789";

		/// <summary>
		/// Creates the "Strike" rule - exact position and digit match
		/// Color: Red
		/// </summary>
		public static PuzzleRule CreateStrikeRule(Color strikeColor) {
			return new PuzzleRule(
				id: "strike",
				color: strikeColor,
				description: "Correct digit in correct position",
				checkCondition: (answer, guess, position) => answer[position] == guess,
				audioPitch: 1.3f,
				scoreValue: 1,
				characterEffect: "shiver"
			);
		}

		/// <summary>
		/// Creates the "Semi-Strike" rule - digits are congruent modulo 5 at the same position
		/// Color: Purple
		/// </summary>
		public static PuzzleRule CreateSemiStrikeRule(Color semiStrikeColor) {
			return new PuzzleRule(
				id: "semi-strike",
				color: semiStrikeColor,
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

		/// <summary>
		/// Creates the "Ball" rule - correct digit but wrong position
		/// Color: Blue
		/// </summary>
		public static PuzzleRule CreateBallRule(Color ballColor) {
			return new PuzzleRule(
				id: "ball",
				color: ballColor,
				description: "Correct digit exists in answer but wrong position",
				checkCondition: (answer, guess, position) => answer.Contains(guess),
				audioPitch: 0.7f,
				scoreValue: 0
			);
		}

		/// <summary>
		/// Creates the "Miss" rule - no match at all (default/fallback rule)
		/// Color: Gray
		/// </summary>
		public static PuzzleRule CreateMissRule() {
			return new PuzzleRule(
				id: "miss",
				color: new Color(0.73f, 0.73f, 0.73f), // #BBBBBB in RGB
				description: "Digit is not in the answer",
				checkCondition: (answer, guess, position) => true, // Always true - this is the fallback
				audioPitch: 1.0f,
				scoreValue: 0
			);
		}

		/// <summary>
		/// Creates the default/classic rule set (Strike, Semi-Strike, Ball, Miss)
		/// Rules are returned in evaluation order
		/// </summary>
		public static List<PuzzleRule> CreateDefaultRuleSet(Color strikeColor, Color semiStrikeColor, Color ballColor) {
			return new List<PuzzleRule> {
				CreateStrikeRule(strikeColor),
				CreateSemiStrikeRule(semiStrikeColor),
				CreateBallRule(ballColor),
				CreateMissRule()
			};
		}

		// ============================================================
		// Additional example rules that can be used for harder puzzles
		// ============================================================

		/// <summary>
		/// "Even/Odd Match" rule - correct parity (even/odd) at this position
		/// </summary>
		public static PuzzleRule CreateParityRule(Color parityColor) {
			return new PuzzleRule(
				id: "parity-match",
				color: parityColor,
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

		/// <summary>
		/// "Range Match" rule - guess is within ±1 of the answer digit
		/// </summary>
		public static PuzzleRule CreateRangeRule(Color rangeColor) {
			return new PuzzleRule(
				id: "range-match",
				color: rangeColor,
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

		/// <summary>
		/// "Greater Than" rule - guess is greater than answer digit at this position
		/// </summary>
		public static PuzzleRule CreateGreaterThanRule(Color greaterColor) {
			return new PuzzleRule(
				id: "greater-than",
				color: greaterColor,
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

		/// <summary>
		/// "Less Than" rule - guess is less than answer digit at this position
		/// </summary>
		public static PuzzleRule CreateLessThanRule(Color lessColor) {
			return new PuzzleRule(
				id: "less-than",
				color: lessColor,
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
