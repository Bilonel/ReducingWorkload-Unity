using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObject : MonoBehaviour
{
    float distanceFactor = .04f;
    public float value = 0;
    MeshRenderer mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh=GetComponent<MeshRenderer>();
        mesh.material.color = Color.black;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Paint(float distance)
    {
        value+= distanceFactor/distance;
        mesh.material.color = new Color(value, value, value);
    }
}
