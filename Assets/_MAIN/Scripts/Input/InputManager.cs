using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using origin.dialogue;
using origin.puzzle;
using origin.settings;
using origin.tutorial;

namespace origin.IO {
	public class InputManager : MonoBehaviour {
		private PlayerInput input;
		private Dictionary<InputAction, Action<InputAction.CallbackContext>> actionMap = new();

		private void Awake() {
			input = GetComponent<PlayerInput>();
			var currentActionMap = input.currentActionMap;
			actionMap.Add(currentActionMap["NextDialogue"], OnNext);
			actionMap.Add(currentActionMap["PuzzleInput"], OnPuzzleChoice);
			actionMap.Add(currentActionMap["Escape"], OnEscapeMenu);
			actionMap.Add(currentActionMap["Log"], OnLogToggle);
		}

		private void OnEnable() {
			foreach (var kvp in actionMap) {
				kvp.Key.performed += kvp.Value;
			}
		}

		private void OnDisable() {
			foreach (var kvp in actionMap) {
				kvp.Key.performed -= kvp.Value;
			}
		}

		public void SetActionEnabled(string actionName, bool enabled) {
			var action = input.currentActionMap.FindAction(actionName);
			if (action == null) return;

			if (enabled) action.Enable();
			else action.Disable();
		}

		public void OnNext(InputAction.CallbackContext c) {
			if (TutorialManager.instance != null && TutorialManager.instance.IsRunning) {
				if (!TutorialManager.instance.ShouldAllowInput("NextDialogue")) return;
				TutorialManager.instance.OnNextDialogueRequest();
				return;
			}
			if (DialogueManager.instance != null) {
				DialogueManager.instance.OnNextDialogueRequest();
			}
		}

		public void OnPuzzleChoice(InputAction.CallbackContext c) {
			if (TutorialManager.instance != null && TutorialManager.instance.IsRunning) {
				if (!TutorialManager.instance.ShouldAllowInput("PuzzleGuess")) return;
				TutorialManager.instance.OnCharacterGuess(c.control.name.ToUpper()[0]);
				return;
			}
			if (PuzzleManager.instance != null) {
				PuzzleManager.instance.OnCharacterGuess(c.control.name.ToUpper()[0]);
			}
		}

		public void OnEscapeMenu(InputAction.CallbackContext c) {
			if (TutorialManager.instance != null && !TutorialManager.instance.ShouldAllowInput("Escape"))
				return;
			GameSettingManager.instance.OnEscapeMenu();
		}

		public void OnLogToggle(InputAction.CallbackContext c) {
			if (TutorialManager.instance != null && !TutorialManager.instance.ShouldAllowInput("Log"))
				return;
			if (DialogueManager.instance != null) {
				DialogueManager.instance.ToggleLog();
			}
		}
	}
}
