using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CLiquidColorChange : MonoBehaviour
{
    public Color m_Color;
    private CLiquid m_Liquid;
    
    void Start()
    {
        m_Color.r = Random.Range(0f, 1f);
        m_Color.g = Random.Range(0f, 1f);
        m_Color.b = Random.Range(0f, 1f);
        m_Liquid = this.GetComponent<CLiquid>();
    }

    void Update()
    {
        float r = m_Color.r + Mathf.Sin(Time.time) * 0.5f + 0.5f;
        r = r > 1f ? 2f - r : r;
        float g = m_Color.g + Mathf.Sin(Time.time) * 0.5f + 0.5f;
        g = r > 1f ? 2f - g : g;
        float b = m_Color.b + Mathf.Sin(Time.time) * 0.5f + 0.5f;
        b = b > 1f ? 2f - b : b;
        m_Liquid.m_LiquidColor = new Color(r, g ,b);
        m_Liquid.UpdateLiquidColor();
    }
}
