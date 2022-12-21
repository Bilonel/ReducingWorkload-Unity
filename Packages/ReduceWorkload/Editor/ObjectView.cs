using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(StaticObject))]
public class ObjectView : Editor
{
    float VertexCount;
    int selectedObjectIndex = 1;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        StaticObject obj = (StaticObject)target;
        if (VertexCount == 0)
            VertexCount = obj.countNew;
        int newIndex = GUILayout.Toolbar(selectedObjectIndex, new string[] { "Original", " New " });
        GUILayout.Space(15);
        if(newIndex!=selectedObjectIndex)
        {
            selectedObjectIndex= newIndex;

            if (selectedObjectIndex == 1)
            {
                obj.setMesh(1);
                VertexCount=obj.countNew;
            }
            if (selectedObjectIndex == 0)
            {
                obj.setMesh(0);
                VertexCount = obj.countOrg;
            }
        }
        GUILayout.Label("Vertex Count\t" + VertexCount, EditorStyles.boldLabel);

        GUILayout.Label("       ____________________________________");
        GUILayout.Space(20);

        GUILayout.Label("GAIN \t%" + obj.difference.ToString(), EditorStyles.boldLabel);
    }
    
}
