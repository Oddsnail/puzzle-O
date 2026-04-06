using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using origin.IO;
using origin.dialogue;
using origin.puzzle;

public class GameFlow : MonoBehaviour
{
    [SerializeField] private TextAsset stage1text;

    void Start()
    {
        Debug.Log("start GameFlow");

        StartCoroutine(mainGameFlow());
    }

    IEnumerator mainGameFlow()
    {
        string result;
        List<string> stage1 = FileManager.ReadTextAsset(stage1text);
        yield return DialogueManager.instance.Say(stage1, endTag => result = endTag);

    }
}
