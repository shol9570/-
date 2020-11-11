using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CAnimalStatus : CStageEvent
{
    public CInflammable[] m_Inflammables;
    public CFracturePoint[] m_FracturePoints;
    
    [HideInInspector] public int m_FractureCount;
    [HideInInspector] public int m_InflammationCount;
    [HideInInspector] public float m_FractureAntiCoeff; //Fracture antibiotics coeffidence
    [HideInInspector] public float m_InflammationAntiCoeff; //Inflammation antibiotics coeffidence
    [HideInInspector] public float m_AntiColdCoeff;
    [HideInInspector] public float m_AnthelminticCoeff;
    [HideInInspector] public float m_DHPPLCoeff;

    [HideInInspector] public List<CFractureDecal> m_Fractures;
    [HideInInspector] public List<CInflammable> m_Inflamed;

    [HideInInspector] private float m_ConditionFigure = 1f;
    [HideInInspector] public bool m_IsConditionChanging;
    [HideInInspector] private float m_Temperature = 38.4f;

    private bool m_IsFracture;
    private bool m_IsInflammation;
    private bool m_IsCold;
    private bool m_IsAnthelmintic;
    private bool m_IsDHPPL;

    public bool IsFracture => m_IsFracture;
    public bool IsInflammation => m_IsInflammation;
    public bool IsCold => m_IsCold;
    public bool IsAnthelmintic => m_IsAnthelmintic;
    public bool IsDHPPL => m_IsDHPPL;

    public float ConditionFigure
    {
        set
        {
            m_ConditionFigure = Mathf.Clamp(value, 0f, 1f);
        }
        get
        {
            return m_ConditionFigure;
        }
    }

    public float TemperatureFigure
    {
        set
        {
            m_Temperature = Mathf.Clamp(value, 35f, 41f);
        }
        get
        {
            return m_Temperature;
        }
    }

    /// <summary>
    /// Initialize animal status info. It should be called as required after creating animal object.
    /// </summary>
    /// <param name="_fractureCount"></param>
    /// <param name="_inflammationCount"></param>
    /// <param name="_fractureAntiCoeff"></param>
    /// <param name="_inflammationAntiCoeff"></param>
    public void InitializeStatus(int _fractureCount, int _inflammationCount, float _fractureAntiCoeff = 0f,
        float _inflammationAntiCoeff = 0f, float _temperature = 38.4f, float _antiColdCoeff = 0f,
        float _anthelminticCoeff = 0f, float _DHPPLCoeff = 0f)
    {
        m_FractureCount = _fractureCount;
        m_InflammationCount = _inflammationCount;
        m_Fractures = new List<CFractureDecal>();
        m_Inflamed = new List<CInflammable>();
        m_FractureAntiCoeff = _fractureAntiCoeff;
        m_InflammationAntiCoeff = _inflammationAntiCoeff;
        m_Temperature = _temperature;
        m_AntiColdCoeff = _antiColdCoeff;
        m_AnthelminticCoeff = _anthelminticCoeff;
        m_DHPPLCoeff = _DHPPLCoeff;

        m_IsFracture = _fractureAntiCoeff > 0;
        m_IsInflammation = _inflammationAntiCoeff > 0;
        m_IsCold = _antiColdCoeff > 0;
        m_IsAnthelmintic = _anthelminticCoeff > 0;
        m_IsDHPPL = _DHPPLCoeff > 0;
        if (m_Inflammables == null) Debug.LogError("[CAnimalStatus] No inflammable data.");
        if (m_FracturePoints == null) Debug.LogError("[CAnimalStatus] No fracture point data");
        Debug.Log("[CAnimalStatus] Animal status has initialized. " + m_FractureCount + ", " + m_InflammationCount +
                  ", " + m_FractureAntiCoeff + ", " + m_InflammationAntiCoeff + ", " + m_Temperature + ", " + m_AntiColdCoeff + ", " + m_AnthelminticCoeff + ", " + m_DHPPLCoeff);
    }

    public void InitializeStatusWithData(string _data)
    {
        Debug.Log("[CAnimalStatus] Start parsing disease info data.");
        string[] parse = _data.Split(',');
        int fractureCount = System.Convert.ToInt32(parse[0]);
        int inflammationCount = System.Convert.ToInt32(parse[1]);
        float fractureAntiCoeff = parse.Length > 2 ? System.Convert.ToSingle(parse[2]) : 0;
        float inflammationAntiCoeff = parse.Length > 3 ? System.Convert.ToSingle(parse[3]) : 0;
        float temperature = parse.Length > 4 ? System.Convert.ToSingle(parse[4]) : 38.4f;
        float antiCold = parse.Length > 5 ? System.Convert.ToSingle(parse[5]) : 0;
        float anthelminticCoeff = parse.Length > 6 ? System.Convert.ToSingle(parse[6]) : 0;
        float DHPPLCoeff = parse.Length > 7 ? System.Convert.ToSingle(parse[7]) : 0;
        InitializeStatus(fractureCount, inflammationCount, fractureAntiCoeff, inflammationAntiCoeff, temperature, antiCold, anthelminticCoeff, DHPPLCoeff);
    }

    #region DrugInjectionEffect
    /// <summary>
    /// Decrease m_FractureAntiCoeff.
    /// </summary>
    /// <param name="_change">Amount of change.</param>
    /// <returns>-1 : it is already less than or equal to zero\n0 : it becomes zero\n1 : it is still greater than zero</returns>
    public int DecreaseFractureAntiCoeff(float _change)
    {
        float result = m_FractureAntiCoeff - _change;
        if (result <= 0)
        {
            if (m_FractureAntiCoeff > 0)
            {
                m_FractureAntiCoeff = Mathf.Max(0f, result);
                m_FractureAntiCoeff = result;
                return 0;
            }
            else
            {
                m_FractureAntiCoeff = Mathf.Max(0f, result);
                m_FractureAntiCoeff = result;
                return -1;
            }
        }
        m_FractureAntiCoeff = result;
        return 1;
    }
    
    /// <summary>
    /// Decrease m_InflammationAntiCoeff.
    /// </summary>
    /// <param name="_change">Amount of change.</param>
    /// <returns>-1 : it is already less than or equal to zero\n0 : it becomes zero\n1 : it is still greater than zero</returns>
    public int DecreaseInflammationAntiCoeff(float _change)
    {
        float result = m_InflammationAntiCoeff - _change;
        if (result <= 0)
        {
            if (m_InflammationAntiCoeff > 0)
            {
                m_InflammationAntiCoeff = Mathf.Max(0f, result);
                m_InflammationAntiCoeff = result;
                return 0;
            }
            else
            {
                m_InflammationAntiCoeff = Mathf.Max(0f, result);
                m_InflammationAntiCoeff = result;
                return -1;
            }
        }
        m_InflammationAntiCoeff = result;
        return 1;
    }

    public void DecreaseTemperature(float _change)
    {
        m_Temperature = Mathf.Max(35f, m_Temperature - _change);
    }
    
    public int DecreaseAntiColdCoeff(float _change)
    {
        float result = m_AntiColdCoeff - _change;
        if (result <= 0)
        {
            if (m_AntiColdCoeff > 0)
            {
                m_AntiColdCoeff = Mathf.Max(0f, result);
                m_AntiColdCoeff = result;
                return 0;
            }
            else
            {
                m_AntiColdCoeff = Mathf.Max(0f, result);
                m_AntiColdCoeff = result;
                return -1;
            }
        }
        m_AntiColdCoeff = result;
        return 1;
    }

    public int DecreaseAnthelminticCoeff(float _change)
    {
        float result = m_AnthelminticCoeff - _change;
        if (result <= 0)
        {
            if (m_AnthelminticCoeff > 0)
            {
                m_AnthelminticCoeff = Mathf.Max(0f, result);
                m_AnthelminticCoeff = result;
                return 0;
            }
            else
            {
                m_AnthelminticCoeff = Mathf.Max(0f, result);
                m_AnthelminticCoeff = result;
                return -1;
            }
        }
        m_AnthelminticCoeff = result;
        return 1;
    }

    public int DecreaseDHPPLCoeff(float _change)
    {
        float result = m_DHPPLCoeff - _change;
        if (result <= 0)
        {
            if (m_DHPPLCoeff > 0)
            {
                m_DHPPLCoeff = Mathf.Max(0f, result);
                m_DHPPLCoeff = result;
                return 0;
            }
            else
            {
                m_DHPPLCoeff = Mathf.Max(0f, result);
                m_DHPPLCoeff = result;
                return -1;
            }
        }
        m_DHPPLCoeff = result;
        return 1;
    }
    #endregion

    public void IncreaseTemperature(float _change)
    {
        m_Temperature = Mathf.Min(41f, m_Temperature + _change);
    }
    
    public override void SceneLoadedEvent()
    {
        Debug.Log("[CAnimalStatuc] Animal status has been initialized.");

        InitializeStatusWithData(GameManager.Manager.StageDatabase.StageInfos[GameManager.Manager.lastSelectedStage]
            .levelData[1]);
        GameManager.Manager.GetDiseaseMgr.InitializeDisease(this);
        m_IsConditionChanging = true;
    }

    private void Update()
    {
        if (m_IsConditionChanging)
        {
            ConditionFigure -= (1f - GameManager.Manager.GetDiseaseMgr.DiseaseTreatmentRate(this)) * 0.005f * Time.deltaTime;
            if (m_Temperature < 38.4 && m_AntiColdCoeff <= 0) IncreaseTemperature(0.005f * Time.deltaTime);
            if (m_Temperature > 38.4 && m_AntiColdCoeff <= 0) DecreaseTemperature(0.005f * Time.deltaTime);
            else if (m_AntiColdCoeff > 0) IncreaseTemperature(0.01f * Time.deltaTime);
        }
    }
}
