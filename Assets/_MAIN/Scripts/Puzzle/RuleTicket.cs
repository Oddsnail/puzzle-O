using UnityEngine;
using UnityEngine.UI;
using origin.language;
using TMPro;
using origin.graphics;

namespace origin.puzzle {

	[System.Serializable]
	
	public class RuleTicket: MonoBehaviour {

		public GameObject root;
		public LocalizedText title;
        public LocalizedText description;
        public LocalizedText comments;
		public TextMeshProUGUI order;
		public ColorblindIcon colorblindIcon;

		public Image colorImage;
		public Image subcolorImage;

		public void SetColor(Color color, Color subcolor) {
			colorImage.color = color;
			subcolorImage.color = subcolor;
			colorblindIcon.SetColor(subcolor);
		}

		public void SetKey(string titleKey, string descriptionKey, string commentKey) {
			title.SetKey(titleKey);
			description.SetKey(descriptionKey);
			comments.SetKey(commentKey);
		}

		public void SetOrder(int order) {
			this.order.text = $"#{order}";
			colorblindIcon.SetOrder(order);
		}
	}
}