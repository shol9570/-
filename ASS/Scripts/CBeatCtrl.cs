using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CBeatCtrl : MonoBehaviour
{
    public CAnimalStatus m_DisplayTarget;
    public int m_LineCount = 100;
    public Vector3 m_Start;
    public Vector3 m_End;
    
    private LineRenderer m_Line;
    
    void Start()
    {
        m_Line = this.GetComponent<LineRenderer>();
        InitializeLineSetting();
    }

    void InitializeLineSetting()
    {
        m_Line.positionCount = m_LineCount;
        Vector3[] positions = new Vector3[m_LineCount];
        for (int i = 0; i < m_LineCount; i++)
        {
            positions[i] = Vector3.Lerp(m_Start, m_End, (float)i / (float)(m_LineCount - 1));
        }
        m_Line.SetPositions(positions);
    }

    void Update()
    {
        
    }
}
