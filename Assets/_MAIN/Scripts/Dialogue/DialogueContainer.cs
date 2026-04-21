using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace origin.dialogue {
	[System.Serializable]

	public class DialogueContainer {

		[Header("Dialogue Container Connection")]
		public RectTransform dialogueRoot;
		public TextMeshProUGUI dialogueText;
		public Image dialogueColor;
		
		[Header("Name Container Connection")]
		public RectTransform nameRoot;
		public TextMeshProUGUI nameText;
		public Image nameColor;

		[Header("UI Connection")]
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