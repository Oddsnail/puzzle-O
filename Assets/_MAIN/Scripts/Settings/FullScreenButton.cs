using UnityEngine;
using origin.language;

namespace origin.graphic {
	public class FullScreenButton : MonoBehaviour {

		[SerializeField] LocalizedText text;

		void Start() {
			UpdateText();
		}

		public void OnFullscreenButtonPressed() {
			Screen.fullScreen = !Screen.fullScreen;
			UpdateText();
		}

		private void UpdateText() {
			if(Screen.fullScreen) {
				text.SetKey("ui.settings.colorblind.on");
			} else {
				text.SetKey("ui.settings.colorblind.off");
			}
		}
	}
}

