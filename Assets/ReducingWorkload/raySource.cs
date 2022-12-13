using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public class raySource : MonoBehaviour
{
    List<Ray> rays = new List<Ray>();
    public int raysCount;
    public int inc = 60;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=-180;i<180;i+=inc)
        {
            for (int k = -180; k < 180; k += inc)
            {
                for (int m = -180; m < 180; m += inc)
                {
                    rays.Add(new Ray(transform.position, new Vector3(i, k, m)));
                }
            }
        }
        raysCount = rays.Count;
    }
    public int UNSUCCES=0;
    // Update is called once per frame
    void Update()
    {
        UNSUCCES++;
        int i = 0;
        StaticObject staticObject;
        for (i = 0; i < rays.Count; i++)
        {
            Ray ray = rays[i];
            ray.origin = transform.position;
            rays[i] = ray;

            RaycastHit[] hits = Physics.RaycastAll(ray,1000,LayerMask.GetMask("object"));
            if(hits.Length>0)
            {
                staticObject = hits[0].collider.gameObject.GetComponent<StaticObject>();
                if(staticObject!=null)
                {
                    staticObject.Paint(hits[0].distance);
                    staticObject = null;
                }
            }
        }
        if (i >= rays.Count) UNSUCCES--;
    }
    private void OnDrawGizmos()
    {
        
        Gizmos.DrawLine(rays[10].GetPoint(0), rays[10].GetPoint(100));
    }
}
