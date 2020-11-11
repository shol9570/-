using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/* 1. Get meshes from CInflammable object
 * 2. Choose one of meshes and create inflammations
 * 3. Each inflammation has its own normal factor (forward direction)
 * 4. Each CInflammable store their inflammations' data (object transforms)
*/
public class CInflammationMgr : MonoBehaviour
{
    
    public GameObject m_InflammationFactory;
    [Range(0.001f, 0.05f)] public float m_MinInflammationSize = 0.01f;
    [Range(0.001f, 0.05f)] public float m_MaxInflammationSize = 0.05f;

    /// <summary>
    /// Create inflammation with CAnimalStatus info.
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_minSeverity"></param>
    /// <param name="_maxSeverity"></param>
    /// <param name="_inflammables"></param>
    /// <param name="_count">Create loop count. Default is 1.</param>
    public void CreateInflammations(CAnimalStatus _target, int _minSeverity, int _maxSeverity, List<CInflammable> _inflammables, int _count = 1)
    {
        if(_count <= 0) Debug.LogError("[CInflammationMgr] Invalid create count.");
        List<CInflammable> inflammableList = new List<CInflammable>();
        inflammableList.AddRange(_target.m_Inflammables);
        for (int i = 0; i < _count; i++)
        {
            int randomIdx = Random.Range(0, inflammableList.Count);
            CInflammable target = _target.m_Inflammables[randomIdx];
            int randomSeverity = Random.Range(_minSeverity, _maxSeverity);
            target.InflammatoryInfection(randomSeverity, m_MinInflammationSize, m_MaxInflammationSize, m_InflammationFactory);
            _inflammables.Add(target);
            inflammableList.Remove(target);
        }
    }

    public void ClearInflammations(List<CInflammable> _inflammables)
    {
        for (int i = 0; i < _inflammables.Count; i++)
        {
            _inflammables[i].ClearInflammations();;
        }
    }
}
