using System.Collections.Generic;
using UnityEngine;

namespace origin.puzzle {
	public interface IPuzzleUI {

		bool isAnimating { get; }

		void Show();
		void Hide();

		void SetThemeColor(Color color);
		void UpdateHistory(string history);
		void UpdateRuleSet(List<PuzzleRule> ruleSet);
		void UpdateTrials(int remaining, int total);
	}
}
