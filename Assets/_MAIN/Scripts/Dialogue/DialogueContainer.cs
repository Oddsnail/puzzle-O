using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace origin.dialogue {
	[System.Serializable]

	public class DialogueContainer {

		// must have every dialogue UI-related gameobjects here

		public RectTransform dialogueRoot;
		public RectTransform nameRoot;
		public TextMeshProUGUI dialogueText;
		public TextMeshProUGUI nameText;
		public Image dialogueColor;
		public Image nameColor;

		// UI objects migrated from DialogueManager
		public RectTransform upperLetterbox;
		public RectTransform lowerLetterbox;
		public GameObject promptIcon;
		public Transform choicePanel;
		public GameObject choicePrefab;

		public void SetThemeColor(Color color) {
			dialogueColor.color = color;
			nameColor.color = color;
		}
	}
}