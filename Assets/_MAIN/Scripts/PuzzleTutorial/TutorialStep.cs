using UnityEngine;

namespace origin.tutorial {
	[System.Serializable]
	public class TutorialStep {
		public StepType stepType;

		[Header("Dialogue Box Location")]
		public float dialogueX;
		public float dialogueY;
		[Range(800f, 1200f)]
		public float dialogueLength = 800f;

		[Header("Highlight Settings (HighlightArea & ForceInput)")]
		public Rect highlightRect;

		[Header("Force Input Settings (ForceInput only)")]
		public ForceInputCondition forceCondition;

		[Header("Event Settings")]
		public bool doEvent;
		public StepEvent stepEvent;
	}
}
