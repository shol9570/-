using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(CFracturePoint))]
[CanEditMultipleObjects]
public class CFracturePointHelper : Editor
{
    SerializedProperty m_Target;
    public int m_PreviewType;
    public bool m_IsPreview;
    public bool m_IsProject;
    private CFractureMgr m_FractureMgr;
    private GameObject m_PreviewObject;
    private CFractureDecal m_Decal;
    private CFracturePoint BASE;

    private MeshFilter m_PreviewMeshFilter;
    private Mesh m_OrinMesh;
    private Vector3[] m_PreviewOrinVert;
    private Vector3[] m_PreviewOrinNor;
    private Vector4[] m_PreviewOrinTan;

    void OnEnable()
    {
        m_Target = serializedObject.FindProperty("m_Target");
        m_FractureMgr = GameObject.Find("GameManager").GetComponent<CFractureMgr>();
        BASE = (CFracturePoint) target;
    }

    public override void OnInspectorGUI()
    {
        string[] list = new string[m_FractureMgr.m_FractureFactories.Length];
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = m_FractureMgr.m_FractureFactories[i].name;
        }

        GUILayout.Label("Fracture decal preview", EditorStyles.boldLabel);
        m_PreviewType = EditorGUILayout.Popup(m_PreviewType, list); //Preview object type

        if (m_FractureMgr.m_FractureFactories.Length > m_PreviewType)
        {
            GameObject previewTarget = m_FractureMgr.m_FractureFactories[m_PreviewType];
            if (GUILayout.Button("Preview"))
            {
                m_IsPreview = !m_IsPreview;
                if (m_IsPreview)
                {
                    m_PreviewObject = Instantiate(previewTarget, BASE.transform.position, BASE.transform.rotation, BASE.transform);
                    m_Decal = m_PreviewObject.GetComponent<CFractureDecal>();
                    m_PreviewMeshFilter = m_Decal.GetComponent<MeshFilter>();
                    m_OrinMesh = m_PreviewMeshFilter.sharedMesh;
                    Mesh meshCopy = Mesh.Instantiate(m_OrinMesh) as Mesh;
                    m_PreviewMeshFilter.mesh = meshCopy;
                    m_PreviewOrinVert = m_PreviewMeshFilter.sharedMesh.vertices;
                    m_PreviewOrinNor = m_PreviewMeshFilter.sharedMesh.normals;
                    m_PreviewOrinTan = m_PreviewMeshFilter.sharedMesh.tangents;
                    m_PreviewObject.hideFlags = HideFlags.HideAndDontSave;
                }
                else
                {
                    DestroyImmediate(m_PreviewObject);
                }
            }
        }

        m_IsProject = EditorGUILayout.Toggle("Project preview", m_IsProject);
        if (BASE.m_Target != null && m_PreviewObject != null)
        {
            if (m_IsProject)
            {
                Mesh meshCopy = Mesh.Instantiate(m_OrinMesh) as Mesh;
                m_PreviewMeshFilter.mesh = meshCopy;
                m_Decal.ProjectMesh(BASE.m_Target, m_PreviewMeshFilter.sharedMesh, m_PreviewOrinVert, m_PreviewOrinNor, m_PreviewOrinTan, -BASE.transform.forward);
            }
            else
            {
                Mesh meshCopy = Instantiate(m_OrinMesh) as Mesh;
                m_PreviewMeshFilter.mesh = meshCopy;
            }
        }

        GUILayout.Space(20f);
        
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_Target);
        serializedObject.ApplyModifiedProperties();
    }

    private void OnDisable()
    {
        DestroyImmediate(m_PreviewObject);
        m_IsProject = false;
    }
}
