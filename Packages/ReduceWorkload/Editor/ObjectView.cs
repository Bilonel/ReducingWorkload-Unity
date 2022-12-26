using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(StaticObject))]
public class ObjectView : Editor
{
    float VertexCount;
    int selectedObjectIndex = -1;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        seperator("_");
        StaticObject obj = (StaticObject)target;
        if (selectedObjectIndex == -1) selectedObjectIndex= obj.currentIndex;
        GUILayout.Space(10);
        int newIndex = GUILayout.Toolbar(selectedObjectIndex, new string[] { "Original", " New " },GUILayout.Height(30));
        GUILayout.Space(35);
        if(newIndex==1) VertexCount=obj.countNew;
        else VertexCount = obj.countOrg;
            
        if(newIndex!=selectedObjectIndex)
        {
            selectedObjectIndex= newIndex;

            if (selectedObjectIndex == 1)
                obj.setMesh(1);
            if (selectedObjectIndex == 0)
                obj.setMesh(0);
        }

        GUILayout.Label("Vertex Count\t" + VertexCount, EditorStyles.whiteLargeLabel);

        seperator(".");

        GUILayout.Label("GAIN \t\t%" + obj.difference.ToString(), EditorStyles.whiteLargeLabel);
    }
    void seperator(string c)
    {
        string text = "";
        text += c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c + c ;
        text += text + text + text + text + text + text + text + text + text + text + text + text + text;
        GUILayout.Label(text,EditorStyles.miniBoldLabel);
        GUILayout.Space(5);
    }
}
