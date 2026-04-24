using System.Collections.Generic;
using UnityEngine;
using origin.audio;
using origin.IO;
using origin.language;
using origin.settings;
using UnityEngine.UI;

namespace origin.dialogue {
    public class LogManager
    {
		private MonoBehaviour runner;
        private LogContainer logContainer;
        private bool isLogOpen = false;
        public bool IsLogOpen => isLogOpen;

        private const string plainTextID = "default";

        public void Initialize(LogContainer logContainer, MonoBehaviour runner) {
            this.logContainer = logContainer;
            this.runner = runner;
            isLogOpen = false;
            logContainer.root.SetActive(false);
        }

        public void ToggleLog() {
            if (isLogOpen) CloseLog();
            else OpenLog();
        }

        private void OpenLog() {
            if (GameSettingManager.instance != null && GameSettingManager.instance.isMenuOn) return;

            var input = Object.FindAnyObjectByType<InputManager>();
            if (input != null) {
                input.SetActionEnabled("NextDialogue", false);
                input.SetActionEnabled("PuzzleInput", false);
            }

            logContainer.root.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0f;
            isLogOpen = true;
            logContainer.root.SetActive(true);

            AudioManager.instance.PlayPreloadedSFX("openMenu");
        }

        public void CloseLog() {
            if (!isLogOpen) return;

            isLogOpen = false;
            logContainer.root.SetActive(false);

            var input = Object.FindAnyObjectByType<InputManager>();
            if (input != null) {
                input.SetActionEnabled("NextDialogue", true);
                input.SetActionEnabled("PuzzleInput", true);
            }
            AudioManager.instance.PlayPreloadedSFX("closeMenu");
        }

        public void AddLog(string speaker, List<string> content) {
            GameObject newLogLine = GameObject.Instantiate(logContainer.logLinePrefab, logContainer.logPanel);
            LocalizedLogLine localizedLogLine = newLogLine.GetComponent<LocalizedLogLine>();
            localizedLogLine.SetInfo(speaker == plainTextID ? "" : speaker, content);
            newLogLine.SetActive(true);
        }

        public void EmptyLog() {
            foreach (Transform child in logContainer.logPanel) {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
