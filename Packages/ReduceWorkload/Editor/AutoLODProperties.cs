
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class AutoLODProperties: Editor
{
    public Renderer _target;
    public bool _customSettings = false;
    public bool _foldout = false;
    public bool _lodGroupFoldout = false;

    public int _lodLevels = 1;
    public float _reductionRate = 5f;
    public bool _optimizeSourceMesh = false;
    public float _performance = 0.5f;
    public float _relativeHeightCulling = 0.002f;
    public bool _flatShading = false;

    public bool _writeMeshOnDisk=false;
    public string _filePath = "AutoLOD/Generated";

    public void Apply(AutoLODProperties other)
    {
        _customSettings = false;
        _foldout = false;
        _lodGroupFoldout = false;
        _lodLevels = other._lodLevels;
        _reductionRate = other._reductionRate;
        _optimizeSourceMesh = other._optimizeSourceMesh;
        _performance = other._performance;
        _relativeHeightCulling = other._relativeHeightCulling;
        _flatShading = other._flatShading;
        _writeMeshOnDisk = other._writeMeshOnDisk;
        _filePath = other._filePath;
    }
}

