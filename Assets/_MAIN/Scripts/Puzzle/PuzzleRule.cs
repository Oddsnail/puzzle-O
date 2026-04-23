using System;
using UnityEngine;

namespace origin.puzzle {
	/// <summary>
	/// Represents a single puzzle rule that determines how input digits are evaluated.
	/// Rules are checked in order, and the first matching rule determines the digit's color.
	/// </summary>
	[Serializable]
	public class PuzzleRule {

		private const string keyPrefix = "rule.";
		private const string titleSuffix = ".title";
		private const string descriptionSuffix = ".description";
		private const string commentSuffix = ".comment.";

		public string ruleID;
		public Color color;
		public Color subcolor;
		public string title => keyPrefix + ruleID + titleSuffix;
		public string description => keyPrefix + ruleID + descriptionSuffix;
		public string commentPrefix => keyPrefix + ruleID + commentSuffix;
		public Func<string, char, int, bool> checkCondition { get; set; }
		public float audioPitch = 1.0f;
		public int scoreValue = 0; // max 5, least 0
		public string characterEffect = "";

		public PuzzleRule(string id, Color color, Color subcolor,
		                  Func<string, char, int, bool> checkCondition,
		                  float audioPitch, int scoreValue, string characterEffect = "") {
			this.ruleID = id;
			this.color = color;
			this.subcolor = subcolor;
			this.checkCondition = checkCondition;
			this.audioPitch = audioPitch;
			this.scoreValue = scoreValue;
			this.characterEffect = characterEffect;
		}

		public bool Evaluate(string answer, char guess, int position) {
			if (checkCondition == null) {
				Debug.LogError($"PuzzleRule '{ruleID}' has no CheckCondition defined!");
				return false;
			}
			return checkCondition(answer, guess, position);
		}
	}
}
