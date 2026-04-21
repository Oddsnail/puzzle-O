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

        public void Addrule(PuzzleRule puzzleRule, int order)
        {
            GameObject rule = GameObject.Instantiate(ruleTicketPrefab, ruleTicketPanel);
            RuleTicket ruleTicket = rule.GetComponent<RuleTicket>();
            ruleTicket.SetKey(puzzleRule.title, puzzleRule.description);
            ruleTicket.SetOrder(order);
			ruleTicket.SetColor(puzzleRule.color, puzzleRule.subcolor);
			ruleTicket.colorblindIcon.Refresh();
        }

	}

}
