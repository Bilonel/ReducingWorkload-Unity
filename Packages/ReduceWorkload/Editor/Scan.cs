using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using Codice.Client.Common;

public class Scan : EditorWindow
{
    int dataCollectStyle = 0;
    int LayerIndex = 0;
    int InterfaceIndex = 0;
    Color VisualizeColor;
    float MaxReductionRate = 50;
    public bool scanning = false;
    bool scanned = false;
    static GUIStyle customStyle=new GUIStyle();

    [MenuItem("Tools/ReducingWorkload")]
    public static void ShowWindow()
    {
        GetWindow<Scan>("ReductionWorkload");
        customStyle = new GUIStyle(EditorStyles.whiteLargeLabel);
        customStyle.fontSize = 25;
        customStyle.fontStyle = FontStyle.BoldAndItalic;
        customStyle.alignment = TextAnchor.MiddleCenter;
    }

    void Update()
    {
        if (scanning)
        {
            scanUpdate();
        }
    }
    private void OnGUI()
    {
        if (scanning) scanningGUI();
        else
        {
            GUILayout.Space(10);
            InterfaceIndex = GUILayout.SelectionGrid(InterfaceIndex, new string[] { "SCAN","ALGORITHM","RESULT" },3);
            switch(InterfaceIndex)
            {
                case 0: scanGUI();
                    break;
                case 1: algorithmGUI();
                    break;
                case 2: performanceGUI();
                    break;
                default:break;
            }
        }
    }
    void performanceGUI()
    {

        GUILayout.Space(100);
        EditorGUILayout.LabelField("<Not Found>", customStyle, GUILayout.Height(50), GUILayout.ExpandWidth(true));
    }
    void scanningGUI()
    {
        GUILayout.Space(100);
        
        EditorGUILayout.LabelField("P l e a s e   W a i t . . .", customStyle,GUILayout.Height(50),GUILayout.ExpandWidth(true));

    }
    void algorithmGUI()
    {
        if(!scanned || Objects.Count<1)
        {
            GUILayout.Space(100);
            EditorGUILayout.LabelField("Target Objects Not Found", customStyle, GUILayout.Height(50), GUILayout.ExpandWidth(true));
            return;
        }
        GUILayout.Space(30);
        GUILayout.Label("Reduction Rate : ", EditorStyles.boldLabel);
        GUILayout.Space(10);
        MaxReductionRate=GUILayout.HorizontalSlider(MaxReductionRate, 10, 100);
        GUILayout.Space(50);
        if (GUILayout.Button(" SEND OBJECTS TO ALGORITHM *", new GUILayoutOption[] { GUILayout.MinHeight(25) }))
        {
            ReductionVertices algo;
            GameObject objIn;

            foreach (var item in Objects)
            {
                if (item.value > 1) item.value = 1;
                float reductionRate = MathF.Abs(item.value - 1) * 2 * MaxReductionRate / 100 + 1;
                objIn = item.gameObject;
                algo = new ReductionVertices();
                algo.Run(objIn, reductionRate);
                item.set(algo.VertexCount0, algo.orgmf, algo.orgmr, algo.VertexCount1, algo.newmf, algo.newmr);
            }
            scanned = false;
        }
    }
    void scanGUI()
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
            startT = DateTime.Now;
        }
       
    }
    DateTime startT;
    TimeSpan scanT;

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
        scanT = DateTime.Now - startT;
        startT = new DateTime();
        Debug.Log(scanT.TotalMilliseconds);
        scanT = new TimeSpan();
        ShowWindow();
        foreach(var item in GameObject.FindObjectsOfType<raySource>())
            UnityEngine.Object.DestroyImmediate(item.gameObject);
    }

    void scanUpdate()
    {
        rayS.update();
        rayS.gameObject.GetComponent<LightMovement>().update();
    }
}
