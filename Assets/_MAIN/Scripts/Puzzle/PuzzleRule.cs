using System;
using UnityEngine;

namespace origin.puzzle {
	/// <summary>
	/// Represents a single puzzle rule that determines how input digits are evaluated.
	/// Rules are checked in order, and the first matching rule determines the digit's color.
	/// </summary>
	[Serializable]
	public class PuzzleRule {

		public string ruleID;
		public Color color;
		public Color subcolor;
		public string title;
		public string description;
		public Func<string, char, int, bool> CheckCondition { get; set; }
		public float audioPitch = 1.0f;
		public int scoreValue = 0;
		public string characterEffect = "";

		public PuzzleRule(string id, Color color, Color subcolor, string title, string description,
		                  Func<string, char, int, bool> checkCondition,
		                  float audioPitch, string characterEffect = "") {
			this.ruleID = id;
			this.color = color;
			this.subcolor = subcolor;
			this.title = title;
			this.description = description;
			this.CheckCondition = checkCondition;
			this.audioPitch = audioPitch;
			this.characterEffect = characterEffect;
		}

		public bool Evaluate(string answer, char guess, int position) {
			if (CheckCondition == null) {
				Debug.LogError($"PuzzleRule '{ruleID}' has no CheckCondition defined!");
				return false;
			}
			return CheckCondition(answer, guess, position);
		}
	}
}
