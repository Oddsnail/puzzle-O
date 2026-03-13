using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace origin.puzzle {

	[System.Serializable]
	
	public class RuleContainer {

		public RectTransform containerRoot;
		public Image puzzleColor;
		public Transform ruleTicketPanel;
        public GameObject ruleTicketPrefab;

        public void EmptyRule() {
			foreach (Transform child in ruleTicketPanel) {
				GameObject.Destroy(child.gameObject);
			}
            return;
        }

        public void Addrule(PuzzleRule puzzleRule)
        {
            GameObject rule = GameObject.Instantiate(ruleTicketPrefab, ruleTicketPanel);
            RuleTicket ruleTicket = rule.GetComponent<RuleTicket>();
            ruleTicket.title.text = puzzleRule.title;
            ruleTicket.description.text = puzzleRule.description;
            ruleTicket.colorImage.color = puzzleRule.color;

            return;
        }

	}

}
