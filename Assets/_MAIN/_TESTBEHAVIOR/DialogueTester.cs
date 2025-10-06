using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTester : MonoBehaviour {

    [SerializeField] private TextAsset file1;
    [SerializeField] private TextAsset file2;

    void Start() {

        Debug.Log("start gameflow testing");

        StartCoroutine(Testing());
    }

    IEnumerator Testing() {

        List<string> conversation1 = FileManager.ReadTextAsset(file1);
        List<string> conversation2 = FileManager.ReadTextAsset(file2);

        string result = "";

        yield return DialogueManager.instance.Say(conversation1, endTag => result = endTag);

        while (result == "FAIL") {
            yield return DialogueManager.instance.Say(conversation2, endTag => result = endTag);
        }
    }
}


