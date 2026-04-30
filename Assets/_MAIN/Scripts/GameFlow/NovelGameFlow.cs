using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using origin.IO;
using origin.dialogue;

public class NovelGameFlow : MonoBehaviour
{
	// paragraph_0 (start) => paragraph_1 ~ 4 (stages) => paragraph_5 (end) 

	[SerializeField] private TextAsset[] paragraphs;
	[SerializeField] SceneTransitionButton quitEnd;
	private const string storySawPrefix = "SawStory.";

    void Start()
	{
		int startingIndex = PlayerPrefs.GetInt("_DEST", 0);
		StartCoroutine(Flow(startingIndex));
    }

    private IEnumerator Flow(int startingIndex) {
		for (int i = startingIndex; i < paragraphs.Length; i++) {

			PlayerPrefs.SetInt(storySawPrefix + $"{i}", 1);
			PlayerPrefs.Save();
			
			List<string> conversation = FileManager.ReadTextAsset(paragraphs[i]);
			string result = "";

			yield return new WaitForSeconds(0.2f);
			yield return DialogueManager.instance.Say(conversation, endTag => result = endTag);

			// end logics here

			if (result != "EOF") break;
			if (i == paragraphs.Length - 1) {	
				PlayerPrefs.SetInt("Saw.Ending", 1);
				PlayerPrefs.Save();
			}

			yield return new WaitForSeconds(3.5f);
		}
		quitEnd.OnButtonPressed();
	}
}
