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

		private struct ScreenModeOption {
			public int width, height;
			public bool isFullscreen;
			public string labelKey;
		}

		// Ordered from largest to smallest; all are 16:9
		private static readonly (int width, int height, string labelKey)[] standardResolutions = {
			(3840, 2160, "ui.settings.screen.uhd"),
			(2560, 1440, "ui.settings.screen.qhd"),
			(1920, 1080, "ui.settings.screen.fhd"),
			(1280,  720, "ui.settings.screen.hd"),
		};

		private readonly List<ScreenModeOption> availableScreenModes = new();
		private int currentScreenModeIndex;

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
			menuContainer.screenModeButtonR.onClick.AddListener(() => CycleScreenMode(true));
			menuContainer.screenModeButtonL.onClick.AddListener(() => CycleScreenMode(false));
			menuContainer.colorblindButtonR.onClick.AddListener(ToggleColorblindMode);
			menuContainer.colorblindButtonL.onClick.AddListener(ToggleColorblindMode);

			menuContainer.versionText.text = $"v{Application.version}";
			InitializeScreenModes();
		}

		private void Start() {
			currentLanguageIndex = System.Array.IndexOf(languageCodes, LocalizationManager.instance.currentLanguageCode);
			UpdateLanguageLabel();
		}

		private void OnDestroy() {
			if (instance == this) {
				menuContainer.languageButtonR.onClick.RemoveListener(() => CycleLanguage(true));
				menuContainer.languageButtonL.onClick.RemoveListener(() => CycleLanguage(false));
				menuContainer.screenModeButtonR.onClick.RemoveListener(() => CycleScreenMode(true));
				menuContainer.screenModeButtonL.onClick.RemoveListener(() => CycleScreenMode(false));
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
            AudioManager.instance.PlayPreloadedSFX("openMenu");
		}

		private void CloseMenu() {
			isMenuOn = false;
			menuContainer.containerRoot.SetActive(false);

			var input = FindAnyObjectByType<InputManager>();
			if (input != null) {
				input.SetActionEnabled("NextDialogue", true);
				input.SetActionEnabled("PuzzleInput", true);
			}
            AudioManager.instance.PlayPreloadedSFX("closeMenu");
		}

		// Language Setting
		private void CycleLanguage(bool add) {
			currentLanguageIndex = (((currentLanguageIndex + (add ? 1 : -1)) % languageCodes.Length) + languageCodes.Length) % languageCodes.Length;
			LocalizationManager.instance.SetLanguage(languageCodes[currentLanguageIndex]);
			AudioManager.instance.PlayPreloadedSFX("optionChange");
			UpdateLanguageLabel();
		}

		private void UpdateLanguageLabel() {
			menuContainer.currentLanguageText.text = languageLabels[currentLanguageIndex];
		}

		// Screen Mode Setting
		private void InitializeScreenModes() {
			int displayWidth = Display.main.systemWidth;
			int displayHeight = Display.main.systemHeight;
			bool is16by9 = Mathf.Abs((float)displayWidth / displayHeight - 16f / 9f) < 0.02f;

			availableScreenModes.Clear();

			if (is16by9) {
				availableScreenModes.Add(new ScreenModeOption {
					width = displayWidth, height = displayHeight,
					isFullscreen = true, labelKey = "ui.settings.screen.fullscreen"
				});
			}

			foreach (var (w, h, key) in standardResolutions) {
				if (w <= displayWidth && h <= displayHeight) {
					availableScreenModes.Add(new ScreenModeOption {
						width = w, height = h, isFullscreen = false, labelKey = key
					});
				}
			}

			// Restore last saved screen mode
			int savedWidth = PlayerPrefs.GetInt("ScreenWidth", -1);
			int savedHeight = PlayerPrefs.GetInt("ScreenHeight", -1);
			int savedFullscreen = PlayerPrefs.GetInt("ScreenFullscreen", -1);

			currentScreenModeIndex = 0;
			for (int i = 0; i < availableScreenModes.Count; i++) {
				var opt = availableScreenModes[i];
				if (opt.width == savedWidth && opt.height == savedHeight &&
					(opt.isFullscreen ? 1 : 0) == savedFullscreen) {
					currentScreenModeIndex = i;
					break;
				}
			}

			ApplyScreenMode(currentScreenModeIndex);
			UpdateScreenModeLabel();
		}

		private void CycleScreenMode(bool add) {
			if (availableScreenModes.Count == 0) return;
			currentScreenModeIndex = (((currentScreenModeIndex + (add ? 1 : -1)) % availableScreenModes.Count) + availableScreenModes.Count) % availableScreenModes.Count;
			ApplyScreenMode(currentScreenModeIndex);
			AudioManager.instance.PlayPreloadedSFX("optionChange");
			UpdateScreenModeLabel();
		}

		private void ApplyScreenMode(int index) {
			if (availableScreenModes.Count == 0) return;
			var opt = availableScreenModes[index];
			Screen.SetResolution(opt.width, opt.height, opt.isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
			PlayerPrefs.SetInt("ScreenWidth", opt.width);
			PlayerPrefs.SetInt("ScreenHeight", opt.height);
			PlayerPrefs.SetInt("ScreenFullscreen", opt.isFullscreen ? 1 : 0);
		}

		private void UpdateScreenModeLabel() {
			if (availableScreenModes.Count == 0) return;
			menuContainer.currentScreenModeText.SetKey(availableScreenModes[currentScreenModeIndex].labelKey);
		}

		//Colorblind Mode Setting
		private void ToggleColorblindMode() {
			isColorblindModeOn = !isColorblindModeOn;
			PlayerPrefs.SetInt("ColorblindMode", isColorblindModeOn ? 1 : 0);
			OnColorblindModeToggled?.Invoke();
			AudioManager.instance.PlayPreloadedSFX("optionChange");
			UpdateColorblindModeLabel();
		}

		private void UpdateColorblindModeLabel() {
			menuContainer.colorblindButtonText.SetKey(isColorblindModeOn ? "ui.settings.colorblind.on" : "ui.settings.colorblind.off");
		}
	}
}
