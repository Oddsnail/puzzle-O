using UnityEngine;
using UnityEngine.UI;
using TMPro;
using origin.language;

namespace origin.settings {

	[System.Serializable]
	public class MenuContainer {

		public GameObject containerRoot;

		[Header("Language Menu")]
		public Button languageButtonR;
		public Button languageButtonL;
		public TextMeshProUGUI currentLanguageText;

		[Header("Colorblind Mode")]
		public Button colorblindButtonR;
		public Button colorblindButtonL;
		public LocalizedText colorblindButtonText;

		[Header("Version")]
		public TextMeshProUGUI versionText;

	}

}
