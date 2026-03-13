using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using origin.IO;
using origin.dialogue;

public class GameFlowTester : MonoBehaviour
{

    [SerializeField] private TextAsset test1;
    [SerializeField] private TextAsset test2;

    void Start()
    {
        Debug.Log("start gameflow testing");

        StartCoroutine(Testing());
    }

    IEnumerator Testing()
    {

        List<string> conversation = FileManager.ReadTextAsset(test1);

        string result;

        yield return DialogueManager.instance.Say(conversation, endTag => result = endTag);

    }
}
