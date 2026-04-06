using UnityEngine;
using UnityEngine.UI;

using origin.IO;
using origin.language;
using origin.audio;
using origin.dialogue;

namespace origin.settings {
	public class GameSettingManager : MonoBehaviour {

		public static GameSettingManager instance;

		public MenuContainer menuContainer = new();

		public bool IsMenuOn { get; private set; }

		private readonly string[] languageCodes = { "kor", "eng" };
		private readonly string[] languageLabels = { "한국어", "ENGLISH" };
		private int currentLanguageIndex = 1;

		private void Awake() {
			if (instance == null) instance = this;
			else { DestroyImmediate(gameObject); return; }

			IsMenuOn = false;
			menuContainer.containerRoot.SetActive(false);

			menuContainer.languageButtonR.onClick.AddListener(() => CycleLanguage(true));
			menuContainer.languageButtonL.onClick.AddListener(() => CycleLanguage(false));
			menuContainer.screenModeButtonR.onClick.AddListener(ToggleScreenMode);
            menuContainer.screenModeButtonL.onClick.AddListener(ToggleScreenMode);

			menuContainer.versionText.text = $"v{Application.version}";
			UpdateLanguageLabel();
			UpdateScreenModeLabel();
		}

		private void OnDestroy() {
			if (instance == this) {
				menuContainer.languageButtonR.onClick.RemoveListener(() => CycleLanguage(true));
				menuContainer.languageButtonL.onClick.RemoveListener(() => CycleLanguage(false));
				menuContainer.screenModeButtonR.onClick.RemoveListener(ToggleScreenMode);
                menuContainer.screenModeButtonL.onClick.RemoveListener(ToggleScreenMode);
			}
		}

		public void OnEscapeMenu() {
			if (DialogueManager.instance != null && DialogueManager.instance.IsLogOpen) {
				DialogueManager.instance.CloseLog();
				return;
			}

			if (IsMenuOn) CloseMenu();
			else OpenMenu();
		}

		private void OpenMenu() {
			if (DialogueManager.instance != null && DialogueManager.instance.IsLogOpen) return;

			IsMenuOn = true;
			menuContainer.containerRoot.SetActive(true);

			var input = FindAnyObjectByType<InputManager>();
			if (input != null) {
				input.SetActionEnabled("NextDialogue", false);
				input.SetActionEnabled("PuzzleInput", false);
			}
            AudioManager.instance.PlayPreloadedSFX("openMenu", AudioManager.instance.sfxMixer);
		}

		private void CloseMenu() {
			IsMenuOn = false;
			menuContainer.containerRoot.SetActive(false);

			var input = FindAnyObjectByType<InputManager>();
			if (input != null) {
				input.SetActionEnabled("NextDialogue", true);
				input.SetActionEnabled("PuzzleInput", true);
			}
            AudioManager.instance.PlayPreloadedSFX("closeMenu", AudioManager.instance.sfxMixer);
		}

		private void CycleLanguage(bool add) {
			currentLanguageIndex = (((currentLanguageIndex + (add ? 1 : -1)) % languageCodes.Length) + languageCodes.Length) % languageCodes.Length;
			LocalizationManager.instance.SetLanguage(languageCodes[currentLanguageIndex]);
            AudioManager.instance.PlayPreloadedSFX("optionChange", AudioManager.instance.sfxMixer);
			UpdateLanguageLabel();
		}

		private void UpdateLanguageLabel() {
			menuContainer.currentLanguageText.text = languageLabels[currentLanguageIndex];
		}

		private void ToggleScreenMode() {
			bool isFullscreen = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
			Screen.fullScreenMode = isFullscreen ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;
            AudioManager.instance.PlayPreloadedSFX("optionChange", AudioManager.instance.sfxMixer);
			UpdateScreenModeLabel();
		}

		private void UpdateScreenModeLabel() {
			bool isFullscreen = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
			menuContainer.currentScreenModeText.SetKey(isFullscreen ? "ui.settings.fullscreen" : "ui.settings.windowed");
		}
	}
}
