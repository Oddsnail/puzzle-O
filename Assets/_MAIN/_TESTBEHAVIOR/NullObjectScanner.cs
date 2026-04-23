#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class NullObjectScanner
{
    [MenuItem("Debug/Find Null GameObjects In Scene")]
    static void FindNullObjects()
    {
        // Finds C# wrappers around destroyed C++ objects
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
        {
            // A "fake null" passes == null but isn't actually C# null
            if (go == null)
                Debug.LogError("Found a destroyed GameObject still referenced in memory.");
        }
    }
}
#endif