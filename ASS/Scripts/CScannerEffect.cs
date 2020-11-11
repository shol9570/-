using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CScannerEffect : MonoBehaviour
{
    void Awake()
    {
        InitializeShaderUniform();
    }
    
    void InitializeShaderUniform()
    {
        MeshRenderer mr = this.GetComponent<MeshRenderer>();
        MeshFilter mf = this.GetComponent<MeshFilter>();
        mf.mesh.RecalculateBounds();
        mr.material
            .SetVector("_Center", mf.mesh.bounds.center);
        mr.material
            .SetVector("_Extends", mf.mesh.bounds.extents);
    }
}