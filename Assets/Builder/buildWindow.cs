using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class buildWindow : EditorWindow
{
    static Builder builder;

    [MenuItem("Tools/Builder")]
    public static void ShowWindow()
    {
        GetWindow<buildWindow>("Builder");
        builder = GameObject.FindObjectOfType<Builder>();
    }
    private void OnGUI()
    {
        GUILayout.Space(20);
        builder.objectCount = EditorGUILayout.IntField("Object Count", builder.objectCount);
        GUILayout.Space(20);

        if (GUILayout.Button(" BUILD ",GUILayout.Height(20)))
        {
            builder.Start();
        }
        GUILayout.Space(10);
    }
}
