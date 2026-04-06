using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using origin.dialogue;
using origin.puzzle;
using origin.settings;

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
			DialogueManager.instance.OnNextDialogueRequest();
		}

		public void OnPuzzleChoice(InputAction.CallbackContext c) {
			PuzzleManager.instance.OnCharacterGuess(c.control.name.ToUpper()[0]);
		}

		public void OnEscapeMenu(InputAction.CallbackContext c) {
			GameSettingManager.instance.OnEscapeMenu();
		}

		public void OnLogToggle(InputAction.CallbackContext c) {
			DialogueManager.instance.ToggleLog();
		}
	}
}
