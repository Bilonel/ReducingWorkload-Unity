using Codice.CM.WorkspaceServer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObject : MonoBehaviour
{
    float distanceFactor = .04f;
    public float value = 0;
    MeshRenderer mesh;
    bool Available = true;
    // Start is called before the first frame update
    //void Start()
    //
    //    value = 0;
    //    mesh=GetComponent<MeshRenderer>();
    //    mesh.sharedMaterial.color = Color.black;
    //}
    public void start()
    {
        value = 0;
        try
        {
            mesh = GetComponent<MeshRenderer>();
            mesh.sharedMaterial.color = Color.black;
        }
        catch(Exception)
        {
            Available = false;
        }
    }
    public void Paint(float distance)
    {
        if (!Available) return;
        value+= distanceFactor/distance;
        mesh.sharedMaterial.color = new Color(value, value, value);
    }
}
