using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace origin.puzzle {

	[System.Serializable]
	
	public class PuzzleContainer {

		public RectTransform containerRoot;
		public Image puzzleColor;
		public Transform trialPanel;
		public GameObject trialPrefab;
		public TrialIconPanel trialsPanel;

	}

}
