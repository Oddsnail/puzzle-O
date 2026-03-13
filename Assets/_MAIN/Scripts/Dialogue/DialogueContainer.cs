using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace origin.dialogue {
	[System.Serializable]

	public class DialogueContainer {

		public RectTransform dialogueRoot;
		public RectTransform nameRoot;
		public TextMeshProUGUI dialogueText;
		public TextMeshProUGUI nameText;
		public Image dialogueColor;
		public Image nameColor;

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