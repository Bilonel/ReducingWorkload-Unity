using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scan : MonoBehaviour
{
    public List<StaticObject> Objects = new List<StaticObject>();
    [SerializeField] GameObject rayS;
    // Update is called once per frame
    private void OnEnable()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Respawn");
        foreach(var o in objects)
        {
            Objects.Add(o.AddComponent<StaticObject>());
        }
        rayS.SetActive(true);
    }
    void Update()
    {
        
    }
}
