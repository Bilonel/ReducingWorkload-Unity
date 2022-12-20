using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float speed = 4; 
    public float rotationSpeed = 500;
    float valueOffset=0;
    float horizontal;
    float vertical;
    Vector3 mousePos;
    MeshRenderer mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponentInChildren<MeshRenderer>();
        mousePos = Input.mousePosition;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = mousePos.x - Input.mousePosition.x;
        mousePos = Input.mousePosition;
        vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(0, 0, vertical);
        Vector3 rotate = new Vector3(0, horizontal, 0);
        movement.Normalize();
        rotate.Normalize();
        transform.Translate(movement * Time.deltaTime * speed);
        transform.Rotate(-rotate*Time.deltaTime*rotationSpeed);
        float currentSpeed = -(movement * Time.deltaTime * speed).magnitude*Mathf.Sign(vertical);
        valueOffset += currentSpeed / 10;
        mesh.material.SetTextureOffset(16, new Vector2(0, valueOffset));
        mesh.UpdateGIMaterials();
    }
}
