using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LightMovement : MonoBehaviour
{
    List<Vector3> targets = new List<Vector3>();
    int index=0;
    public float speed = 0.04f;
    Rigidbody rb;
    public bool finished = false;
    Scan scan;
    // Start is called before the first frame update
    //void Start(Scan _Scan)
    //{
    //    rb = GetComponent<Rigidbody>();
    //}
    public void customStart(Scan _Scan)
    {
        scan = _Scan;
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    public void update()
    {
        if (index >= targets.Count)
        {
            scan.reset();
            
            return;
        }
        if (Vector3.Distance(transform.position, targets[index]) < .25f) index++;
        if (index >= targets.Count) return;

        transform.Translate((targets[index] - transform.position).normalized * speed);
        
    }

    public void addTarget(Vector3 point)
    {
        targets.Add(point);
    }
}
