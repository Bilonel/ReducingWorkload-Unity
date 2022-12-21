using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Scan : EditorWindow
{
    int dataCollectStyle = 0;
    int LayerIndex = 0;
    Color VisualizeColor;

    public bool scanning = false;
    bool scanned = false;
    [MenuItem("Tools/ReducingWorkload")]
    public static void ShowWindow()
    {
        GetWindow<Scan>("SCANNER");
    }

    void Update()
    {
        if (scanning)
            scanUpdate();
    }
    private void OnGUI()
    {
        if (!scanning)
            guiUpdate();
    }
    void guiUpdate()
    {
        GUILayout.Space(30);
        GUILayout.Label("Data Weighting Method : ", EditorStyles.boldLabel);
        GUILayout.Space(7);

        GUIContent[] contents = new GUIContent[]
        {
            new GUIContent("Run Time","Weighted Data By Players' Playing Time"),

            new GUIContent("None","")
        };
        dataCollectStyle = GUILayout.SelectionGrid(dataCollectStyle, contents, contents.Length,
            new GUILayoutOption[] { GUILayout.MinHeight(25), GUILayout.ExpandWidth(true) });

        GUILayout.Space(30);
        LayerIndex = EditorGUILayout.LayerField("Target Layers : ", LayerIndex);

        GUILayout.Space(30);
        VisualizeColor = EditorGUILayout.ColorField("Visualize Color : ", VisualizeColor);

        GUILayout.Space(30);
        spotSpeed = EditorGUILayout.FloatField("Speed Of Spot Light : ", spotSpeed);

        GUILayout.Space(30);
        rayIncrement = EditorGUILayout.FloatField("Angle Diffrence Per Ray : ", rayIncrement);

        GUILayout.Space(30);
        lightIntensity = EditorGUILayout.FloatField("Light Intensity : ", lightIntensity);

        GUILayout.Space(30);
        if (GUILayout.Button(" SCAN ", new GUILayoutOption[] { GUILayout.MinHeight(30), GUILayout.ExpandWidth(true) }))
        {
            //reset();
            start();
            scanning = true;
        }
        else
        {
            if(scanned)
            {
                GUILayout.Space(15);
                GUILayout.Label("_________________________________________________________",EditorStyles.centeredGreyMiniLabel);
                GUILayout.Space(30);

                if (GUILayout.Button(" SEND OBJECTS TO ALGORITHM *", new GUILayoutOption[] { GUILayout.MinHeight(25) }))
                {
                    GameObject objIn = Selection.activeGameObject;
                    //string objData = MeshToString(Selection.activeGameObject.GetComponent<MeshFilter>(), Selection.activeGameObject.transform);
                    //File.WriteAllText("C:/test/in.obj", objData);
                    ////string objData = File.ReadAllText("C:/test/out.txt");
                    //Model model = new Model(objData);
                    //Debug.Log("V " +model.vertices.Count);
                    //Debug.Log("F " +model.faces.Count);
                    //model.Run();
                    //Debug.Log("JOINED COUNT : "+model.joinedFaceList.Count);
                    //string outData = model.Save();
                    //File.WriteAllText("C:/test/out.obj", outData);
                    //File.WriteAllText("C:/test/out.txt", outData);
                    Algorithm_ algo = new Algorithm_();
                    algo.Run(objIn);
                    if(!objIn.GetComponent<StaticObject>())
                        objIn.AddComponent<StaticObject>();
                    objIn.GetComponent<StaticObject>().set(algo.VertexCount0,algo.orgmf,algo.orgmr,algo.VertexCount1,algo.newmf,algo.newmr);
                }
            }
        }
    }

    float rayIncrement = 50;
    float spotSpeed = 0.04f;
    float lightIntensity = 1;

    public List<StaticObject> Objects = new List<StaticObject>();
    [SerializeField] raySource rayS;
    Vector3 point1 = new Vector3(25, 0, 0);
    Vector3 point2 = new Vector3(25, 0, 50);
    void start()
    {
        Objects = new List<StaticObject>();
        GameObject[] objects = FindObjectsOfType<GameObject>();
        foreach (var o in objects)
        {
            StaticObject[] list = o.GetComponents<StaticObject>();
            foreach (var item in list)
            {
                DestroyImmediate(item);
            }
            if (o.layer == LayerIndex)
            {
                StaticObject st;
                o.TryGetComponent<StaticObject>(out st);
                if (st == null)
                    st = o.AddComponent<StaticObject>();
                Objects.Add(st);
                st.start();
            }
        }
        var obj = new GameObject("spotLight");
        obj.AddComponent<Rigidbody>();
        obj.AddComponent<BoxCollider>();
        rayS =obj.AddComponent<raySource>();
        obj.transform.position = point1;
        obj.SetActive(true);
        rayS.inc = rayIncrement;
        rayS.lightIntensity = lightIntensity;
        rayS.start();
        obj.AddComponent<LightMovement>().addTarget(point2);
        obj.GetComponent<LightMovement>().speed = spotSpeed;
        obj.GetComponent<LightMovement>().customStart(this);
    }
    public void reset()
    {
        scanning = false;
        scanned = true;
        ShowWindow();
        foreach(var item in GameObject.FindObjectsOfType<raySource>())
            UnityEngine.Object.DestroyImmediate(item.gameObject);
    }

    void scanUpdate()
    {
        rayS.update();
        rayS.gameObject.GetComponent<LightMovement>().update();
    }
    /// 
    /// Github.com/MattRix
    /// 
    public string MeshToString(MeshFilter mf, Transform t)
    {
        int StartIndex = 0;
        Vector3 s = t.localScale;
        Vector3 p = t.localPosition;
        Quaternion r = t.localRotation;


        int numVertices = 0;
        Mesh m = mf.sharedMesh;
        if (!m)
        {
            return "####Error####";
        }
        Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;

        StringBuilder sb = new StringBuilder();

        foreach (Vector3 vv in m.vertices)
        {
            Vector3 v = t.TransformPoint(vv);
            numVertices++;
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, -v.z));
        }
        sb.Append("\n");
        foreach (Vector3 nn in m.normals)
        {
            Vector3 v = r * nn;
            sb.Append(string.Format("vn {0} {1} {2}\n", -v.x, -v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in m.uv)
        {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for (int material = 0; material < m.subMeshCount; material++)
        {
            sb.Append("\n");
            sb.Append("usemtl ").Append(mats[material].name).Append("\n");
            sb.Append("usemap ").Append(mats[material].name).Append("\n");

            int[] triangles = m.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                                        triangles[i] + 1 + StartIndex, triangles[i + 1] + 1 + StartIndex, triangles[i + 2] + 1 + StartIndex));
            }
        }

        StartIndex += numVertices;
        return sb.ToString();
    }
}
