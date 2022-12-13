using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class movement : MonoBehaviour
{
    float horizontal, vertical;
    public float speed = 3;
    public float RotationSpeed = 3;
    Vector3 lastMousePosition;
    public Transform camera;
    // Start is called before the first frame update
    void Start()
    {
        lastMousePosition = Input.mousePosition;
    }
    
    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(vertical,0, -horizontal) * Time.deltaTime * speed);
        transform.Rotate(0, (Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime), 0);
    }
}
