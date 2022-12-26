using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using MeshDecimator;

public class ReductionVertices
{
    public float VertexCount0;
    public float VertexCount1;
    public UnityEngine.Mesh orgmf;
    public Material[] orgmr;
    public UnityEngine.Mesh newmf;
    public Material[] newmr;

    public void Run(GameObject a,float reductionRate)
    {
        //int count = 1;

        ProcessMeshRenderer(a,reductionRate);

    }
    private void ProcessMeshRenderer(GameObject go,float reductionRate, int count=1)
    {
         
        VertexCount0 = go.GetComponent<MeshFilter>().sharedMesh.vertexCount;     
        orgmf = component<MeshFilter>(go).sharedMesh;
        orgmr = component<MeshRenderer>(go).sharedMaterials;

        //GameObject detailedMesh = go;

        Transform[] children = go.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
            if (child.gameObject != go)
                Undo.DestroyObjectImmediate(child.gameObject);


        LOD[] lods = new LOD[count];
        UnityEngine.Mesh lodMesh;
        lodMesh = go.GetComponent<MeshFilter>().sharedMesh;
        bool hasBones = lodMesh.boneWeights.Length > 0;
        MeshDecimator.Mesh meshDecimator = Mesh2DecMesh(lodMesh);

        MeshDecimator.Algorithms.DecimationAlgorithm decimationAlgorithm = MeshDecimation.CreateAlgorithm(Algorithm.Default);
        decimationAlgorithm.StatusReport += ReportDecimationStatus;

        for (int l = 0; l < count; ++l)
        {
            int triangleTargetCount = (int)(lodMesh.triangles.Length / (3 * reductionRate));
            meshDecimator = MeshDecimator.MeshDecimation.DecimateMesh(decimationAlgorithm, meshDecimator, triangleTargetCount);
            lodMesh = DecMesh2Mesh(meshDecimator, hasBones);

            component<MeshFilter>(go).sharedMesh = lodMesh;
            component<MeshRenderer>(go).sharedMaterials = go.GetComponent<Renderer>().sharedMaterials;
            VertexCount1 = lodMesh.vertexCount;
            newmf = component<MeshFilter>(go).sharedMesh;
            newmr = component<MeshRenderer>(go).sharedMaterials;
        }
        
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
