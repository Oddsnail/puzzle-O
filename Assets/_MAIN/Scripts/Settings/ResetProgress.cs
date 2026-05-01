using UnityEngine;
using origin.language;
using System.Collections;
using UnityEngine.UI;
using origin.audio;
using origin.graphic;

namespace origin.IO {
	public class ResetProgress : MonoBehaviour {

		public MenuGameFlow menuGameFlow;
		public MainScreenSelector mainMenuBG;

		private bool forReal = false;
		private const string keyQuestion = "ui.settings.resetProgress";
		private const string keyReseted = "ui.settings.reseted";
		private const string keyForReal = "ui.settings.forReal";

		private Coroutine co_waiting;
		private bool isWaiting => co_waiting != null;

		[SerializeField] LocalizedText text;

		void OnEnable() {
			forReal = false;
			text.SetKey(keyQuestion);
		}

		public void OnResetButtonPressed() {
			if (isWaiting) return;
			AudioManager.instance.PlayPreloadedSFX("optionChange");
			if (forReal) {
				PlayerPrefs.DeleteAll();
				PlayerPrefs.Save();
				forReal = false;
				menuGameFlow.RefreshContinueButton();
				mainMenuBG.RefreshMainMenuBackground();
				text.SetKey(keyReseted);
				co_waiting = StartCoroutine(waitAndReset());
			}
			else {
				forReal = true;
				text.SetKey(keyForReal);
			}
			RefreshButtonVisual();
		}

		private IEnumerator waitAndReset() {
			yield return new WaitForSeconds(1f);
			text.SetKey(keyQuestion);
			RefreshButtonVisual();
			co_waiting = null;
		}
		
		private void RefreshButtonVisual() {
			text.GetComponent<Button>().OnDeselect(null);
		}
	}
}

