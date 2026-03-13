using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using origin.IO;
using origin.dialogue;
using origin.puzzle;

public class GameFlow : MonoBehaviour
{
    [SerializeField] private TextAsset LangSelect;
    [SerializeField] private TextAsset StoryEng_1;
    [SerializeField] private TextAsset StoryKor_1;

    void Start()
    {
        Debug.Log("start GameFlow");

        StartCoroutine(mainGameFlow());
    }

    IEnumerator mainGameFlow()
    {
        List<string> conv0 = FileManager.ReadTextAsset(LangSelect);
        string lang = "";
        yield return DialogueManager.instance.Say(conv0, endTag => lang = endTag);

        if (lang == "eng")
        {
            List<string> conv1 = FileManager.ReadTextAsset(StoryEng_1);
            yield return DialogueManager.instance.Say(conv1, endTag => lang = endTag);

        }
        else if (lang == "kor")
        {
            List<string> conv1 = FileManager.ReadTextAsset(StoryKor_1);
            yield return DialogueManager.instance.Say(conv1, endTag => lang = endTag);

        }
        else {
            Debug.LogWarning("[WARNING] : lang string unexpected value");
        }
    }
}
