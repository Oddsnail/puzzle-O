using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace origin.dialogue {
	public class ConversationManager {

		private readonly IDialogueUI dialogueUI;
		private readonly MonoBehaviour runner;
		private readonly ILineHandler[] handlers;

		private Coroutine co_talking = null;
		public bool isTalking => co_talking != null;

		private bool jumping = false;
		private bool endBool = false;
		private string jumpingCode = "";
		private string endTag = "EOF";

		private const char noteID = '#';
		private const string plainTextID = "default";

		public ConversationManager(ILineHandler[] handlers, IDialogueUI dialogueUI, MonoBehaviour runner) {
			this.handlers = handlers;
			this.dialogueUI = dialogueUI;
			this.runner = runner;
		}

		public Coroutine StartConversation(List<string> conversation, Action<string> endTagCallback) {
			StopConversation();
			co_talking = runner.StartCoroutine(RunningConversation(conversation, endTagCallback));
			return co_talking;
		}

		public void StopConversation() {
			if (!isTalking) return;
			runner.StopCoroutine(co_talking);
			co_talking = null;
			dialogueUI.Hide();
		}

		private IEnumerator RunningConversation(List<string> conversation, Action<string> endTagCallback) {
			endBool = false;
			dialogueUI.Show();

			foreach (string line in conversation) {
				if (string.IsNullOrWhiteSpace(line) || line[0] == noteID) continue;

				// ===== In choice / dot label logic =====
				if (jumping) {
					Debug.Log("...jumped a line");
					if (line != $".{jumpingCode}") continue;
					jumping = false;
					continue;
				}
				else if (line.StartsWith('.')) continue;
				// ===========================

				Debug.Log(line);
				LINE_DATA data = new(line);

				foreach (ILineHandler handler in handlers) {
					if (handler.CanHandle(data)) {
						yield return handler.Handle(data);
						break;
					}
				}

				if (endBool) break;
			}

			dialogueUI.ChangeLetterBoxTheme(plainTextID);
			dialogueUI.Hide();
			endTagCallback?.Invoke(endTag);
		}

		public void Jump(string code) {
			jumpingCode = code;
			jumping = true;
		}

		public void End(string code) {
			endTag = code;
			endBool = true;
		}
	}
}
