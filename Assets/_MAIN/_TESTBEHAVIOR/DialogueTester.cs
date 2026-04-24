using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using origin.IO;
using origin.dialogue;

public class DialogueTester : MonoBehaviour
{

    [SerializeField] private TextAsset file1;

    void Start()
    {
        Debug.Log("start testing");

        StartCoroutine(Testing());
    }

    IEnumerator Testing()
    {
        List<string> conversation = FileManager.ReadTextAsset(file1);
        string result;

        while(true)
        {
            yield return DialogueManager.instance.Say(conversation, endTag => result = endTag);
            yield return new WaitForSeconds(1f);
        }

    }
}
