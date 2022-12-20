using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.ProBuilder;

public class raySource : MonoBehaviour
{
    List<Ray> rays = new List<Ray>();
    public int raysCount;
    public float inc = 50;
    public float lightIntensity = 1;
    // Start is called before the first frame update
    public void start()
    {
        

        for (int i = -180; i < 180; i += ((int)inc))
        {
            for (int k = -180; k < 180; k += ((int)inc))
            {
                for (int m = -180; m < 180; m += ((int)inc))
                {
                    rays.Add(new Ray(transform.position, new Vector3(i, k, m)));
                }
            }
        }
        raysCount = rays.Count;
    }
    // Update is called once per frame
    public void update()
    {
        int i = 0;
        StaticObject staticObject;
        for (i = 0; i < rays.Count; i++)
        {
            Ray ray = rays[i];
            ray.origin = transform.position;
            rays[i] = ray;

            RaycastHit[] hits = Physics.RaycastAll(ray, 1000, LayerMask.GetMask("object"));
            if (hits.Length > 0)
            {
                staticObject = hits[0].collider.gameObject.GetComponent<StaticObject>();
                if (staticObject != null)
                {
                    staticObject.Paint(hits[0].distance / lightIntensity);
                    staticObject = null;
                }
            }
        }
        
    }
    private void OnDrawGizmos()
    {
        for(var i=0;i<rays.Count; i+=10 )
            Gizmos.DrawLine(rays[i].GetPoint(0), rays[i].GetPoint(100));
    }
}
