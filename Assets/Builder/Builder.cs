using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Builder : MonoBehaviour
{
    [SerializeField] List<GameObject> objects = new List<GameObject>();
    [SerializeField] List<Transform> areas = new List<Transform>();
    public float areaDefaultSize=10;
    public int objectCount=20;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in areas)
        {
            int i = 0;
            while(i++<objectCount)
            {
                Vector3 position = randomPoint(item);
                if (position == Vector3.one * -1) continue;
                Vector3 localScale = new Vector3(Random.Range(.5f, 1.5f), Random.Range(.5f, 1.5f), Random.Range(.5f, 1.5f));
                position.y = localScale.y / 2;
                Vector3 rotation = new Vector3(0, Random.Range(0, 360),0);

                Instantiate(objects[Random.Range(0, objects.Count)], position, Quaternion.Euler(rotation),transform).transform.localScale = localScale;
            }

        }





    }
    Vector3 randomPoint(Transform area)
    {
        float sizeX = areaDefaultSize * area.localScale.x/2;
        float sizeZ = areaDefaultSize * area.localScale.z/2;

        float minX = area.position.x - sizeX;
        float minZ = area.position.z - sizeZ;

        float maxX = area.position.x + sizeX;
        float maxZ = area.position.z + sizeZ;

        int i = 0;
        Vector3 pos =new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
        while(Physics.CheckSphere(pos,2,((int)Mathf.Pow(2,LayerMask.NameToLayer("object")))))
        {
            pos = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
            if (i++ > 10) return Vector3.one * -1;
        }
        return pos;
    }
    
}
