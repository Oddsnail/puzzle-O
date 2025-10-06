using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput input;
	private Dictionary<InputAction, Action<InputAction.CallbackContext>> actionMap = new();

	private void Awake() {
		input = GetComponent<PlayerInput>();
		InitializeActions();
	}

	private void InitializeActions() {
		actionMap.Clear();
		var currentActionMap = input.currentActionMap;
		actionMap.Add(currentActionMap["NextDialogue"], OnNext);
		actionMap.Add(currentActionMap["PuzzleInput"], OnPuzzleChoice);
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

    public void SwitchActionMap(string actionMapName)
    {
        OnDisable(); // 기존 액션 이벤트 제거
        input.SwitchCurrentActionMap(actionMapName); // 새로운 Action Map으로 전환
        InitializeActions(); // 새로운 Action Map에 맞게 초기화
        OnEnable(); // 이벤트 재등록
    }

	public void OnNext(InputAction.CallbackContext c) {
		DialogueManager.instance.OnNextDialogueRequest();
	}

	public void OnPuzzleChoice(InputAction.CallbackContext c) {
		PuzzleManager.instance.OnCharacterGuess(c.control.name.ToUpper()[0]);
	}
}