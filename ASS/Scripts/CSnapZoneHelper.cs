using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(CSnapZone))]
[CanEditMultipleObjects]
public class CSnapZoneHelper : Editor
{
    SerializedProperty m_SnapTarget;
    SerializedProperty m_Whitelist;
    SerializedProperty m_IsHaveToBeHeld;
    SerializedProperty m_SnapDist;
    SerializedProperty m_SnapIcon;
    SerializedProperty m_Icon;
    SerializedProperty m_IconColor;

    private CSnapZone BASE;
    private int m_PreviewIndex;
    private GameObject m_PreviewObj;
    private bool m_IsPreviewing;
    
    private void OnEnable()
    {
        BASE = target as CSnapZone;
        m_SnapTarget = serializedObject.FindProperty("m_SnapTarget");
        m_Whitelist = serializedObject.FindProperty("m_Whitelist");
        m_IsHaveToBeHeld = serializedObject.FindProperty("m_IsHaveToBeHeld");
        m_SnapDist = serializedObject.FindProperty("m_SnapDist");
        m_SnapIcon = serializedObject.FindProperty("m_SnapIcon");
        m_Icon = serializedObject.FindProperty("m_Icon");
        m_IconColor = serializedObject.FindProperty("m_IconColor");
    }

    public override void OnInspectorGUI()
    {
        List<CSnapZone.WhitelistInfo> infos = BASE.m_Whitelist;
        if (infos != null && infos.Count > 0)
        {
            string[] previewList = new string[infos.Count];
            for (int i = 0; i < previewList.Length; i++)
            {
                if (infos[i].m_Target != null) previewList[i] = infos[i].m_Target;
                else previewList[i] = "(Missing game object)";
            }
            m_PreviewIndex = EditorGUILayout.Popup("Preview list", m_PreviewIndex, previewList, EditorStyles.boldLabel);
            if (GUILayout.Button("Preivew"))
            {
                m_IsPreviewing = !m_IsPreviewing;
                if (m_IsPreviewing)
                {
                    if (infos[m_PreviewIndex].m_Preview != null)
                    {
                        m_PreviewObj = Instantiate(infos[m_PreviewIndex].m_Preview, BASE.transform.position,
                            BASE.transform.rotation, BASE.transform);
                        MeshRenderer[] renderers = m_PreviewObj.GetComponentsInChildren<MeshRenderer>();
                        for (int i = 0; i < renderers.Length; i++)
                        {
                            Material[] mats = new Material[renderers[i].sharedMaterials.Length];
                            for (int j = 0; j < mats.Length; j++)
                            {
                                Material mat = Material.Instantiate(renderers[i].sharedMaterials[j]);
                                mat.renderQueue = (int)RenderQueue.Transparent;
                                mats[j] = mat;
                                Color color = mats[j].color;
                                color.a *= 0.3f;
                                mats[j].color = color;
                            }

                            renderers[i].materials = mats;
                        }

                        m_PreviewObj.hideFlags = HideFlags.HideAndDontSave;
                    }
                }
                else
                {
                    if (m_PreviewObj != null) DestroyImmediate(m_PreviewObj);
                }
            }
        }

        serializedObject.Update();
        EditorGUILayout.PropertyField(m_SnapTarget);
        EditorGUILayout.PropertyField(m_Whitelist);
        EditorGUILayout.PropertyField(m_IsHaveToBeHeld);
        EditorGUILayout.PropertyField(m_SnapDist);
        EditorGUILayout.PropertyField(m_SnapIcon);
        EditorGUILayout.PropertyField(m_Icon);
        EditorGUILayout.PropertyField(m_IconColor);
        serializedObject.ApplyModifiedProperties();
    }

    private void OnDisable()
    {
        if (m_PreviewObj != null) DestroyImmediate(m_PreviewObj);
    }
}
