using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace origin.puzzle {
	public class TrialIconPanel : MonoBehaviour {

		public GameObject trialIconPrefab;

		private List<Image> trialIcons = new();
		private int currentLive = 0;

		public void SetTrialIcons(int lives) {
			ClearTrialIcons();
			currentLive = lives;
			for (int i = 0; i < lives; i++) {
				GameObject icon = Instantiate(trialIconPrefab, transform);
				trialIcons.Add(icon.GetComponent<Image>());
			}
		}

		public void MinusLive() {
			if (currentLive < 1) return;
			if (trialIcons[currentLive - 1] == null) return;

			trialIcons[currentLive - 1].color = new Color(1f, 1f, 1f, 0.05f);
			currentLive--;
		}
		
		public void ClearTrialIcons() {
			currentLive = 0;
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject); 
			}
			trialIcons = new();
		}

	}
}

