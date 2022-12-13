using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LightMovement : MonoBehaviour
{
    [SerializeField] List<Transform> targets = new List<Transform>();
    int index=0;
    public float speed = 3;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (index >= targets.Count) return;
        if (Vector3.Distance(transform.position, targets[index].position) < .25f) index++;
        if (index >= targets.Count) return;

        rb.MovePosition(transform.position+(targets[index].position-transform.position).normalized*Time.deltaTime*speed);
    }
}
