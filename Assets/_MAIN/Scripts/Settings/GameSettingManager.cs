using UnityEngine;

using origin.IO;
using origin.language;
using origin.audio;
using origin.dialogue;
using System.Collections.Generic;

namespace origin.settings {
	public class GameSettingManager : MonoBehaviour {

		public static GameSettingManager instance;

		public MenuContainer menuContainer = new();

		public bool isMenuOn { get; private set; }
		public bool isColorblindModeOn { get; private set; }

		public static event System.Action OnColorblindModeToggled;

		private readonly string[] languageCodes = { "kor", "eng" };
		private readonly string[] languageLabels = { "한국어", "ENGLISH" };
		private int currentLanguageIndex;

		private void Awake() {
			if (instance == null) instance = this;
			else { DestroyImmediate(gameObject); return; }

			isMenuOn = false;
			if (PlayerPrefs.HasKey("ColorblindMode")) {
				isColorblindModeOn = PlayerPrefs.GetInt("ColorblindMode") == 1;
			}
			else {
				PlayerPrefs.SetInt("ColorblindMode", 0);
				isColorblindModeOn = false;
			}
			menuContainer.containerRoot.SetActive(false);

			menuContainer.languageButtonR.onClick.AddListener(() => CycleLanguage(true));
			menuContainer.languageButtonL.onClick.AddListener(() => CycleLanguage(false));
			menuContainer.screenModeButtonR.onClick.AddListener(ToggleScreenMode);
			menuContainer.screenModeButtonL.onClick.AddListener(ToggleScreenMode);
			menuContainer.colorblindButtonR.onClick.AddListener(ToggleColorblindMode);
			menuContainer.colorblindButtonL.onClick.AddListener(ToggleColorblindMode);

			menuContainer.versionText.text = $"v{Application.version}";
			UpdateScreenModeLabel();
		}
		
		private void Start() {
			currentLanguageIndex = System.Array.IndexOf(languageCodes, LocalizationManager.instance.currentLanguageCode);
			UpdateLanguageLabel();
		}

		private void OnDestroy() {
			if (instance == this) {
				menuContainer.languageButtonR.onClick.RemoveListener(() => CycleLanguage(true));
				menuContainer.languageButtonL.onClick.RemoveListener(() => CycleLanguage(false));
				menuContainer.screenModeButtonR.onClick.RemoveListener(ToggleScreenMode);
				menuContainer.screenModeButtonL.onClick.RemoveListener(ToggleScreenMode);
				menuContainer.colorblindButtonR.onClick.RemoveListener(ToggleColorblindMode);
				menuContainer.colorblindButtonL.onClick.RemoveListener(ToggleColorblindMode);
			}
		}

		public void OnEscapeMenu() {
			if (DialogueManager.instance != null && DialogueManager.instance.IsLogOpen) {
				DialogueManager.instance.CloseLog();
				return;
			}

			if (isMenuOn) CloseMenu();
			else OpenMenu();
		}

		private void OpenMenu() {
			if (DialogueManager.instance != null && DialogueManager.instance.IsLogOpen) return;

			isMenuOn = true;
			menuContainer.containerRoot.SetActive(true);

			var input = FindAnyObjectByType<InputManager>();
			if (input != null) {
				input.SetActionEnabled("NextDialogue", false);
				input.SetActionEnabled("PuzzleInput", false);
			}
            AudioManager.instance.PlayPreloadedSFX("openMenu", AudioManager.instance.sfxMixer);
		}

		private void CloseMenu() {
			isMenuOn = false;
			menuContainer.containerRoot.SetActive(false);

			var input = FindAnyObjectByType<InputManager>();
			if (input != null) {
				input.SetActionEnabled("NextDialogue", true);
				input.SetActionEnabled("PuzzleInput", true);
			}
            AudioManager.instance.PlayPreloadedSFX("closeMenu", AudioManager.instance.sfxMixer);
		}

		// Language Setting
		private void CycleLanguage(bool add) {
			currentLanguageIndex = (((currentLanguageIndex + (add ? 1 : -1)) % languageCodes.Length) + languageCodes.Length) % languageCodes.Length;
			LocalizationManager.instance.SetLanguage(languageCodes[currentLanguageIndex]);
			AudioManager.instance.PlayPreloadedSFX("optionChange", AudioManager.instance.sfxMixer);
			UpdateLanguageLabel();
		}

		private void UpdateLanguageLabel() {
			menuContainer.currentLanguageText.text = languageLabels[currentLanguageIndex];
		}

		// Screen Mode Setting
		private void ToggleScreenMode() {
			bool isFullscreen = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
			Screen.fullScreenMode = isFullscreen ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;
			AudioManager.instance.PlayPreloadedSFX("optionChange", AudioManager.instance.sfxMixer);
			UpdateScreenModeLabel();
		}

		private void UpdateScreenModeLabel() {
			bool isFullscreen = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
			menuContainer.currentScreenModeText.SetKey(isFullscreen ? "ui.settings.screen.fullscreen" : "ui.settings.screen.windowed");
		}

		//Colorblind Mode Setting
		private void ToggleColorblindMode() {
			isColorblindModeOn = !isColorblindModeOn;
			PlayerPrefs.SetInt("ColorblindMode", isColorblindModeOn ? 1 : 0);
			OnColorblindModeToggled?.Invoke();
			AudioManager.instance.PlayPreloadedSFX("optionChange", AudioManager.instance.sfxMixer);
			UpdateColorblindModeLabel();
		}

		private void UpdateColorblindModeLabel() {
			menuContainer.colorblindButtonText.SetKey(isColorblindModeOn ? "ui.settings.colorblind.on" : "ui.settings.colorblind.off");
		}
	}
}
