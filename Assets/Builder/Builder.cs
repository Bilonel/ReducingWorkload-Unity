using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Builder : MonoBehaviour
{
    [SerializeField] List<GameObject> objects = new List<GameObject>();
    [SerializeField] List<Transform> areas = new List<Transform>();
    public float maxScale = 1;
    public float minScale = 0.3f;
    public float areaDefaultSize = 10;
    public int objectCount = 20;
    // Start is called before the first frame update
    public void Start()
    {
        GameObject builds = new GameObject("BUILDS");
        Undo.SetTransformParent(builds.transform, transform, "New Parent");
        foreach (var item in areas)
        {
            int i = 0;
            while (i++ < objectCount)
            {
                bool flag = true; int k = 0;
                int index = Random.Range(0, objects.Count);
                Vector3 rotation = objects[index].transform.localRotation.eulerAngles;
                rotation.y = Random.Range(0, 360);
                Vector3 position = Vector3.one * -1;
                float r = Random.Range(minScale, maxScale);
                while(flag)
                {
                    if (k++ > 10) break;
                    position = randomPoint(item);
                    flag= Physics.CheckBox(position, Vector3.one*3*r, Quaternion.Euler(rotation), ((int)Mathf.Pow(2, LayerMask.NameToLayer("object"))));
                }
                if (position == Vector3.one * -1 || k>10) continue;
                
                Vector3 localScale = new Vector3(r, Random.Range(.8f, 1.6f) * r, r);
                position.y = localScale.y / 2;
                GameObject o = Instantiate(objects[index], position, Quaternion.Euler(rotation), builds.transform);
                o.transform.localScale = localScale;
                
            }

        }
    }
    Vector3 randomPoint(Transform area)
    {
        float sizeX = areaDefaultSize * area.localScale.x / 2;
        float sizeZ = areaDefaultSize * area.localScale.z / 2;

        float minX = area.position.x - sizeX;
        float minZ = area.position.z - sizeZ;

        float maxX = area.position.x + sizeX;
        float maxZ = area.position.z + sizeZ;

        int i = 0;
        Vector3 pos = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));

        return pos;
    }
}
   
