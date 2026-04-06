using UnityEngine;
using UnityEngine.UI;
using TMPro;
using origin.language;

namespace origin.settings {

	[System.Serializable]
	public class MenuContainer {

		public GameObject containerRoot;
		public Button languageButtonR;
		public Button languageButtonL;
		public TextMeshProUGUI currentLanguageText;
		public Button screenModeButtonR;
		public Button screenModeButtonL;
		public LocalizedText currentScreenModeText;
		public TextMeshProUGUI versionText;

	}

}
