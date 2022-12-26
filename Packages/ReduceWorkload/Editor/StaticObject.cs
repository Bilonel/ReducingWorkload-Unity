using Codice.CM.WorkspaceServer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObject : MonoBehaviour
{
    meshProperty original;
    meshProperty new_;
    float Difference = -1;

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
        mesh.material.color = new Color(value, value, value);
    }
    public void set(float count0, UnityEngine.Mesh mf0, Material[] mr0, float count1, UnityEngine.Mesh mf1, Material[] mr1)
    {
        original = new meshProperty();
        new_ = new meshProperty();
        original.VertexCount = count0;
        original.mf = mf0;
        original.mr = mr0;
        new_.VertexCount = count1;
        new_.mr = mr1;
        new_.mf = mf1;
        Difference = ((count0-count1) / count0) * 100;
    }
    public int currentIndex = 1;
    public void setMesh(int index)
    {
        currentIndex = index;
        if(index==0)
        {
            GetComponent<MeshRenderer>().sharedMaterials = original.mr;
            GetComponent<MeshFilter>().sharedMesh = original.mf;
        }
        else
        {
            GetComponent<MeshRenderer>().sharedMaterials = new_.mr;
            GetComponent<MeshFilter>().sharedMesh = new_.mf;
        }
    }
    public float countOrg { get => original.VertexCount; }
    public float countNew { get => new_.VertexCount; }
    public float difference { get => Difference; }

    public struct meshProperty
    {
        public float VertexCount;
        public UnityEngine.Mesh mf;
        public Material[] mr;
    }
}
