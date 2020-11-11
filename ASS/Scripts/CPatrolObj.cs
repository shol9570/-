using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPatrolObj : MonoBehaviour
{
    public Vector3 m_StartPos = Vector3.zero;
    public Vector3 m_EndPos = Vector3.zero;
    public bool m_StartInit = true;
    public bool m_Local = false;

    void Start()
    {
        if(m_StartInit) m_StartPos = this.transform.position;
    }
    
    void Update()
    {
        float sin = Mathf.Sin((Time.time * 180f / 5f) * Mathf.Deg2Rad);
        Vector3 dir = (m_EndPos - m_StartPos).normalized;
        float dist = (m_EndPos - m_StartPos).magnitude;
        Vector3 targetPos = m_StartPos + dir * (dist + sin);
        if (m_Local) this.transform.localPosition = targetPos;
        else this.transform.position = targetPos;
    }
}
