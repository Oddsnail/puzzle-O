using UnityEngine;
using UnityEngine.UI;
using origin.language;
using TMPro;

namespace origin.puzzle {

	[System.Serializable]
	
	public class RuleTicket: MonoBehaviour {

		public GameObject root;
		public LocalizedText title;
        public LocalizedText description;
		public TextMeshProUGUI order;

		public Image colorImage;
		public Image subcolorImage;

		public void SetColor(Color color, Color subcolor) {
			colorImage.color = color;
			subcolorImage.color = subcolor;
		}

		public void SetKey(string titleKey, string descriptionKey) {
			title.SetKey(titleKey);
			description.SetKey(descriptionKey);
		}
	}
}