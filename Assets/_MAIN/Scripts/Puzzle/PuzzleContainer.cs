using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace origin.puzzle {

	[System.Serializable]
	
	public class PuzzleContainer {

		public RectTransform containerRoot;
		public Image puzzleColor;
		public TextMeshProUGUI historyText;
		public TextMeshProUGUI trialsText;

	}

}
