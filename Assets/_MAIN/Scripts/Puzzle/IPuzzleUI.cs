using System.Collections.Generic;
using UnityEngine;

namespace origin.puzzle {
	public interface IPuzzleUI {

		bool isAnimating { get; }

		void Show();
		void Hide();

		void SetThemeColor(Color color);
		void SetupTrials(int digitCount, int trialCount);
		void HighlightTrial(int trial, bool highlight);
		void UpdateRuleSet(List<PuzzleRule> ruleSet);
		void UpdateTrial(int trial, int digit, Color color, Color subcolor);
		void UpdateTrials(int remaining, int total);
		void Empty();
	}
}
