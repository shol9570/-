using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFracturePoint : MonoBehaviour
{
    public GameObject m_Target;

    private void Start()
    {
        this.transform.SetParent(m_Target.transform);
    }
}
