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

		public void SetThemeColor(Color color) {
			dialogueColor.color = color;
			nameColor.color = color;
		}
	}
}