using MeshDecimator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Algorithm_ : MonoBehaviour
{
    string currentObjectName="";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Run(GameObject a)
    {
        AutoLODProperties prop = new AutoLODProperties();
        prop._target = a.GetComponent<MeshRenderer>();
        //int count = 1;

        ProcessMeshRenderer(prop);

    }
    private void ProcessMeshRenderer(AutoLODProperties properties, int count=1)
    {
        GameObject go = properties._target.gameObject;
        float currentProgress = 0f;
        Undo.RecordObject(go, "Source GameObject modified");
        currentObjectName = go.name + "_LOD0";
        ReportProgress("", currentProgress);
        GameObject detailedMesh = go;
        Undo.RegisterCreatedObjectUndo(detailedMesh, "Created LOD0");

        //foreach (MonoBehaviour mb in detailedMesh.GetComponents<MonoBehaviour>())
        //    DestroyImmediate(mb);

        // Preventing from children duplication
        Transform[] children = detailedMesh.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
            if (child.gameObject != detailedMesh)
                Undo.DestroyObjectImmediate(child.gameObject);

        GameObject parentGo = go;
        //Undo.SetTransformParent(detailedMesh.transform, parentGo.transform, "LOD0 New Parent");
        Undo.RecordObject(detailedMesh.transform, "LOD0 scale)");
        detailedMesh.transform.localScale = Vector3.one;
        //LODGroup lodGroup = Undo.AddComponent<LODGroup>(parentGo);
        //Undo.DestroyObjectImmediate(parentGo.GetComponent<MeshFilter>());
        //Undo.DestroyObjectImmediate(parentGo.GetComponent<MeshRenderer>());
        bool hasCollider = false;
        if (parentGo.GetComponent<Collider>())
        {
            //Undo.DestroyObjectImmediate(parentGo.GetComponent<Collider>());
            hasCollider = true;
        }
        LOD[] lods = new LOD[properties._lodLevels];
        UnityEngine.Mesh lodMesh;
        lodMesh = detailedMesh.GetComponent<MeshFilter>().sharedMesh;

        bool hasBones = lodMesh.boneWeights.Length > 0;
        MeshDecimator.Mesh meshDecimator = Mesh2DecMesh(lodMesh);

        MeshDecimator.Algorithms.DecimationAlgorithm decimationAlgorithm = MeshDecimation.CreateAlgorithm(Algorithm.Default);
        decimationAlgorithm.StatusReport += ReportDecimationStatus;

        //Using the existing mesh as LOD0
        //{
        //    if (properties._optimizeSourceMesh)
        //    {
        //        meshDecimator = MeshDecimator.MeshDecimation.DecimateMeshLossless(decimationAlgorithm, meshDecimator);
        //        lodMesh = DecMesh2Mesh(meshDecimator, hasBones);
        //        lodMesh.name = go.name + "_LOD0";
        //        Undo.RecordObject(detailedMesh.GetComponent<MeshFilter>().sharedMesh, "LOD0 optimized mesh");
        //        //if (properties._flatShading)
        //        //{
        //        //    AutoLODMeshUtility.Smooth2FlatShading(lodMesh);
        //        //}
        //        detailedMesh.GetComponent<MeshFilter>().sharedMesh = lodMesh;
        //        if (properties._writeMeshOnDisk)
        //        {
        //            AssetDatabase.CreateAsset(lodMesh, AssetDatabase.GenerateUniqueAssetPath("Assets/" + properties._filePath + "/" + go.name + "_LOD0.asset"));
        //        }
        //    }
        //    LOD lod = new LOD
        //    {
        //        renderers = new Renderer[1] { detailedMesh.GetComponent<Renderer>() },
        //        screenRelativeTransitionHeight = (properties._lodLevels == 1 ? properties._relativeHeightCulling : properties._performance)
        //    };
        //    lods[0] = lod;
        //    Undo.RecordObject(detailedMesh, "LOD0 Optimized Name");
        //    detailedMesh.name = go.name + "_LOD0";
        //}


        for (int l = 0; l < properties._lodLevels; ++l)
        {

            currentObjectName = go.name + "_LOD" + l;
            GameObject clone = Instantiate(go);
            clone.name = currentObjectName;
            //Undo.RegisterCreatedObjectUndo(clone, "Created LOD0");

            int triangleTargetCount = (int)(lodMesh.triangles.Length / (3 * properties._reductionRate));
            meshDecimator = MeshDecimator.MeshDecimation.DecimateMesh(decimationAlgorithm, meshDecimator, triangleTargetCount);
            lodMesh = DecMesh2Mesh(meshDecimator, hasBones);
            lodMesh.name = go.name + "_LOD" + l;
            //if (properties._flatShading)
            //{
            //    AutoLODMeshUtility.Smooth2FlatShading(lodMesh);
            //}
            if (properties._writeMeshOnDisk)
            {
                AssetDatabase.CreateAsset(lodMesh, AssetDatabase.GenerateUniqueAssetPath("Assets/" + properties._filePath + "/" + go.name + "_LOD" + l.ToString() + ".asset"));
            }
            //clone.transform.parent = parentGo.transform;
            //clone.transform.localPosition = Vector3.zero;
            //clone.transform.localRotation = Quaternion.identity;
            //clone.transform.localScale = Vector3.one;
            
            component<MeshFilter>(clone).sharedMesh = lodMesh;
            component<MeshRenderer>(clone).sharedMaterials = detailedMesh.GetComponent<Renderer>().sharedMaterials;

            //if (hasCollider)
            //{
            //    component<MeshCollider>(clone).sharedMesh = clone.GetComponent<MeshFilter>().sharedMesh;
            //}

            LOD lod = new LOD
            {
                renderers = new Renderer[1] { clone.GetComponent<Renderer>() },
            };
            if (l < properties._lodLevels - 1)
            {
                if (Mathf.Pow(properties._performance, l + 1) > properties._relativeHeightCulling)
                    lod.screenRelativeTransitionHeight = Mathf.Pow(properties._performance, l + 1);
                else
                {
                    lod.screenRelativeTransitionHeight = (lods[l - 1].screenRelativeTransitionHeight + properties._relativeHeightCulling) / 2f;
                }
            }
            else
                lod.screenRelativeTransitionHeight = properties._relativeHeightCulling;
            lods[l] = lod;
            
        }
        if (properties._writeMeshOnDisk)
            AssetDatabase.SaveAssets();
        //lodGroup.SetLODs(lods);
        //lodGroup.animateCrossFading = true;
        //lodGroup.fadeMode = LODFadeMode.CrossFade;
        //LODGroup.crossFadeAnimationDuration = 0.1f;
        
    }
    T component<T>(GameObject go) where T:Component
    {
        if (go.GetComponent<T>())
            return go.GetComponent<T>();
        return go.AddComponent<T>();
    }
    void ReportProgress(string s,float f)
    {
        Debug.Log(s + " : " + f.ToString());
    }

    void ReportDecimationStatus(string message, int start, int current, int end)
    {
        // From Unity 2020, DisplayProgressBar runs way faster
#if UNITY_2020
        double progress = (current - Mathf.Min(start, end)) / (double)Mathf.Abs(start - end);
        message += ": " + ((int)(100 * progress)).ToString() + "%";
        ReportProgress(message, (float)progress);
#endif
    }

    public MeshDecimator.Mesh Mesh2DecMesh(UnityEngine.Mesh mesh)
    {

        MeshDecimator.BoneWeight[] bones = new MeshDecimator.BoneWeight[mesh.vertexCount];

        MeshDecimator.Math.Vector3d[] vertices = Array.ConvertAll(mesh.vertices, item => (MeshDecimator.Math.Vector3d)item);

        MeshDecimator.Math.Vector2[] uv1 = Array.ConvertAll(mesh.uv, item => (MeshDecimator.Math.Vector2)item);
        MeshDecimator.Math.Vector2[] uv2 = Array.ConvertAll(mesh.uv2, item => (MeshDecimator.Math.Vector2)item);
        MeshDecimator.Math.Vector2[] uv3 = Array.ConvertAll(mesh.uv3, item => (MeshDecimator.Math.Vector2)item);
        MeshDecimator.Math.Vector2[] uv4 = Array.ConvertAll(mesh.uv4, item => (MeshDecimator.Math.Vector2)item);
        MeshDecimator.Math.Vector4[] colors = Array.ConvertAll(mesh.colors, item => new MeshDecimator.Math.Vector4(item.r, item.g, item.b, item.a));

        if (mesh.boneWeights.Length > 0)
            bones = Array.ConvertAll(mesh.boneWeights, item => (MeshDecimator.BoneWeight)item);
        MeshDecimator.Math.Vector3[] normals = Array.ConvertAll(mesh.normals, item => (MeshDecimator.Math.Vector3)item);

        int[][] indexes = new int[mesh.subMeshCount][];
        for (int i = 0; i < mesh.subMeshCount; ++i)
        {
            indexes[i] = mesh.GetIndices(i);
        }
        MeshDecimator.Mesh res = new MeshDecimator.Mesh(vertices, indexes);
        res.SetUVs(0, uv1);
        res.SetUVs(1, uv2);
        res.SetUVs(2, uv3);
        res.SetUVs(3, uv4);
        if (mesh.colors.Length == mesh.vertices.Length)
            res.Colors = colors;
        res.Normals = normals;
        res.BoneWeights = bones;

        return res;
    }

    public UnityEngine.Mesh DecMesh2Mesh(MeshDecimator.Mesh mesh, bool hasBones)
    {
        UnityEngine.Mesh res = new UnityEngine.Mesh();
        if (mesh.VertexCount > 65535)
            res.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        res.subMeshCount = mesh.SubMeshCount;
        List<UnityEngine.Vector3> vertices = new List<UnityEngine.Vector3>();
        List<UnityEngine.Vector3> normals = new List<UnityEngine.Vector3>();

        List<UnityEngine.Vector2> uv1 = new List<UnityEngine.Vector2>();
        bool hasUV1 = mesh.UV1 != null;
        List<UnityEngine.Vector2> uv2 = new List<UnityEngine.Vector2>();
        bool hasUV2 = mesh.UV2 != null;
        List<UnityEngine.Vector2> uv3 = new List<UnityEngine.Vector2>();
        bool hasUV3 = mesh.UV3 != null;
        List<UnityEngine.Vector2> uv4 = new List<UnityEngine.Vector2>();
        bool hasUV4 = mesh.UV4 != null;
        List<UnityEngine.Color> colors = new List<UnityEngine.Color>();
        bool hasColors = mesh.Colors != null;

        UnityEngine.BoneWeight[] bones = new UnityEngine.BoneWeight[mesh.VertexCount];

        for (int i = 0; i < mesh.VertexCount; ++i)
        {
            vertices.Add(new UnityEngine.Vector3((float)mesh.Vertices[i].x, (float)mesh.Vertices[i].y, (float)mesh.Vertices[i].z));

            if (hasUV1)
                uv1.Add(new UnityEngine.Vector2(mesh.UV1[i].x, mesh.UV1[i].y));
            if (hasUV2)
                uv2.Add(new UnityEngine.Vector2(mesh.UV2[i].x, mesh.UV2[i].y));
            if (hasUV3)
                uv3.Add(new UnityEngine.Vector2(mesh.UV3[i].x, mesh.UV3[i].y));
            if (hasUV4)
                uv4.Add(new UnityEngine.Vector2(mesh.UV4[i].x, mesh.UV4[i].y));
            if (hasColors)
                colors.Add(new UnityEngine.Color(mesh.Colors[i].x, mesh.Colors[i].y, mesh.Colors[i].z, mesh.Colors[i].w));

            normals.Add(new UnityEngine.Vector3(mesh.Normals[i].x, mesh.Normals[i].y, mesh.Normals[i].z));

            bones[i].boneIndex0 = mesh.BoneWeights[i].boneIndex0;
            bones[i].boneIndex1 = mesh.BoneWeights[i].boneIndex1;
            bones[i].boneIndex2 = mesh.BoneWeights[i].boneIndex2;
            bones[i].boneIndex3 = mesh.BoneWeights[i].boneIndex3;
            bones[i].weight0 = mesh.BoneWeights[i].boneWeight0;
            bones[i].weight1 = mesh.BoneWeights[i].boneWeight1;
            bones[i].weight2 = mesh.BoneWeights[i].boneWeight2;
            bones[i].weight3 = mesh.BoneWeights[i].boneWeight3;
        }

        res.SetVertices(vertices);
        res.SetUVs(0, uv1);
        res.SetUVs(1, uv2);
        res.SetUVs(2, uv3);
        res.SetUVs(3, uv4);
        res.SetNormals(normals);
        res.SetColors(colors);
        for (int i = 0; i < mesh.SubMeshCount; ++i)
            res.SetIndices(mesh.GetIndices(i), UnityEngine.MeshTopology.Triangles, i);
        if (hasBones)
            res.boneWeights = bones;

        res.RecalculateBounds();
        return res;
    }
}
