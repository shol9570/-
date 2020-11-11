using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDiseaseTestScript : MonoBehaviour
{
    public CAnimalStatus m_Target;
    
    
    void Start()
    {
        m_Target.InitializeStatus(1, 4);
        GameManager.Manager.GetDiseaseMgr.InitializeDisease(m_Target);
    }
}
