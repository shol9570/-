using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CInflammationMgr))]
[RequireComponent(typeof(CFractureMgr))]
public class CDiseaseMgr : MonoBehaviour
{
    private List<CAnimalStatus> m_DiseasedAnimals;
    private CInflammationMgr m_InflammationMgr;
    public CInflammationMgr InFlammationManager
    {
        get
        {
            if(m_InflammationMgr == null) m_InflammationMgr = this.GetComponent<CInflammationMgr>();
            return m_InflammationMgr;
        }
    }
    private CFractureMgr m_FractureMgr;
    public CFractureMgr FractureManager
    {
        get
        {
            if (m_FractureMgr == null) m_FractureMgr = this.GetComponent<CFractureMgr>();
            return m_FractureMgr;
        }
    }

    /// <summary>
    /// Initialize disease.
    /// </summary>
    /// <param name="_target"></param>
    public void InitializeDisease(CAnimalStatus _target)
    {
        ClearDisease(_target);
        InFlammationManager.CreateInflammations(_target, 20, 40, _target.m_Inflamed, _target.m_InflammationCount);
        FractureManager.CreateFractures(_target, _target.m_FractureCount, _target.m_Fractures);
    }

    /// <summary>
    /// Return treatment rate. Return 0 ~ 1.0.
    /// </summary>
    /// <param name="_target"></param>
    public void ClearDisease(CAnimalStatus _target)
    {
        InFlammationManager.ClearInflammations(_target.m_Inflamed);
        FractureManager.ClearFractures(_target.m_Fractures);
    }

    public float DiseaseTreatmentRate(CAnimalStatus _target)
    {
        RemoveNullInList(_target);
        
        int fractureMax = _target.m_FractureCount;
        int inflammationMax = _target.m_InflammationCount;
        float maxRatePerEach = (1f - (_target.IsFracture ? 0.1f : 0f) - (_target.IsInflammation ? 0.1f : 0f) - (_target.IsCold ? 0.1f : 0f) - (_target.IsAnthelmintic ? 0.1f : 0f) - (_target.IsDHPPL ? 0.1f : 0f) - 0.1f/*Temperature*/) / (fractureMax + inflammationMax);
        
        int fractureCount = _target.m_Fractures.Count;
        int inflammationCount = _target.m_Inflamed.Count;
        float fractureAntiCoeff = _target.m_FractureAntiCoeff;
        float inflammationAntiCoeff = _target.m_InflammationAntiCoeff;
        float temperature = _target.TemperatureFigure;
        float coldAntiCoeff = _target.m_AntiColdCoeff;
        float anthelminticCoeff = _target.m_AnthelminticCoeff;
        float DHPPLCoeff = _target.m_DHPPLCoeff;

        /*
         * Antibiotics are take up 0.1 rate per each.
         * Treatment rate of fracture and inflammation is take up (1 - number of antibiotics type * 0.1) rate.
         */
        float fractureRate = (fractureMax - fractureCount) * maxRatePerEach;
        float inflammationRate = 0;
        for (int i = 0; i < _target.m_Inflamed.Count; i++)
        {
            float remainRate =
                (1f - (float) _target.m_Inflamed[i].InflammationCount / (float) _target.m_Inflamed[i].MaxInflammationCount) *
                maxRatePerEach;
            inflammationRate += remainRate;
        }
        inflammationRate += (inflammationMax - inflammationCount) * maxRatePerEach;
        float diseaseTotalRate = fractureRate + inflammationRate;
        
        float temperatureRate = (1f - (Mathf.Min(1.5f, (Mathf.Max(0f, Mathf.Abs(38.4f - _target.TemperatureFigure) - 0.5f))) / 1.5f)) * 0.1f;
        float rate = diseaseTotalRate + (_target.IsFracture && fractureAntiCoeff <= 0 ? 0.1f : 0f) + (_target.IsInflammation && inflammationAntiCoeff <= 0 ? 0.1f : 0f) + (_target.IsCold && coldAntiCoeff <= 0 ? 0.1f : 0f) + (_target.IsAnthelmintic && anthelminticCoeff <= 0 ? 0.1f : 0f) + (_target.IsDHPPL && DHPPLCoeff <= 0 ? 0.1f : 0f) + temperatureRate;
        
        if (rate > 1f) Debug.LogWarning("[CDiseaseMgr] Treatment rate is over 1. Rate figure will be clamped from 0 to 1.");
        else if (rate < 0f) Debug.LogWarning("[CDiseaseMgr] Treatment rate is below 0. Rate figure will be clamped from 0 to 1.");
        return Mathf.Clamp(rate, 0f, 1f);
    }

    public void RemoveNullInList(CAnimalStatus _target)
    {
        _target.m_Fractures.RemoveAll(item => item == null);
    }
}