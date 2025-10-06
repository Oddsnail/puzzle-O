using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryLine_0 : MonoBehaviour {

    [SerializeField] private TextAsset tutorial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        
        Debug.Log("start main game story");

        StartCoroutine(Tutorial());
    }

    IEnumerator Tutorial() {
        
        yield return null;
    }
}
