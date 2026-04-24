using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using origin.IO;
using origin.dialogue;
using origin.audio;

public class NovelGameFlow : MonoBehaviour
{
	[SerializeField] private TextAsset[] stories;
	[SerializeField] SceneTransitionButton quitEnd;
	private const string storySawPrefix = "SawStory.";

    void Start()
	{
		int startingIndex = PlayerPrefs.GetInt("_DEST", 0);
		StartCoroutine(Flow(startingIndex));
    }

    private IEnumerator Flow(int startingIndex) {
		for (int i = startingIndex; i < stories.Length; i++) {

			List<string> conversation = FileManager.ReadTextAsset(stories[i]);
			string result = "";

			yield return new WaitForSeconds(0.2f);
			yield return DialogueManager.instance.Say(conversation, endTag => result = endTag);

			if (result != "EOF") {
				Debug.Log($"Unexpected ending with story {i}.");
				break;
			}

			PlayerPrefs.SetInt(storySawPrefix + $"{i}", 1);
			PlayerPrefs.Save();
		}

		quitEnd.OnButtonPressed();
	}
}
