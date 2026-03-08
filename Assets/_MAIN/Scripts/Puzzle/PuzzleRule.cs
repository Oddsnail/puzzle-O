using System;
using UnityEngine;

namespace origin.puzzle {
	/// <summary>
	/// Represents a single puzzle rule that determines how input digits are evaluated.
	/// Rules are checked in order, and the first matching rule determines the digit's color.
	/// </summary>
	[Serializable]
	public class PuzzleRule {

		/// <summary>
		/// Unique identifier for this rule (e.g., "strike", "semi-strike", "ball")
		/// </summary>
		public string ruleID;

		/// <summary>
		/// Color to display when this rule's condition is met
		/// </summary>
		public Color color;

		/// <summary>
		/// User-readable description of what this rule checks for
		/// Will be displayed when the player requests rule information
		/// </summary>
		public string description;

		/// <summary>
		/// The logic that determines if this rule applies to a given digit
		/// Parameters:
		/// - string answer: The correct 4-digit answer
		/// - char guess: The character the player guessed
		/// - int position: The current position (0-3) being evaluated
		/// Returns: true if this rule's condition is met
		/// </summary>
		public Func<string, char, int, bool> CheckCondition { get; set; }

		/// <summary>
		/// Optional: Audio pitch to play when this rule is triggered (default 1.0f)
		/// </summary>
		public float audioPitch = 1.0f;

		/// <summary>
		/// Optional: Score value to add when this rule is met (default 0)
		/// Used for win condition checking
		/// </summary>
		public int scoreValue = 0;

		/// <summary>
		/// Optional: Character effect trigger (e.g., "shiver" for strikes)
		/// </summary>
		public string characterEffect = "";

		// Constructor for basic rule creation
		public PuzzleRule(string id, Color color, string description, Func<string, char, int, bool> checkCondition) {
			this.ruleID = id;
			this.color = color;
			this.description = description;
			this.CheckCondition = checkCondition;
		}

		// Constructor with full parameters
		public PuzzleRule(string id, Color color, string description,
		                  Func<string, char, int, bool> checkCondition,
		                  float audioPitch, int scoreValue, string characterEffect = "") {
			this.ruleID = id;
			this.color = color;
			this.description = description;
			this.CheckCondition = checkCondition;
			this.audioPitch = audioPitch;
			this.scoreValue = scoreValue;
			this.characterEffect = characterEffect;
		}

		/// <summary>
		/// Evaluates whether this rule applies to the given guess
		/// </summary>
		public bool Evaluate(string answer, char guess, int position) {
			if (CheckCondition == null) {
				Debug.LogError($"PuzzleRule '{ruleID}' has no CheckCondition defined!");
				return false;
			}
			return CheckCondition(answer, guess, position);
		}
	}
}
