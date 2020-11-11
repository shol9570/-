using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGradientSliderColor : MonoBehaviour
{
    public Gradient m_BGGradient;
    public Gradient m_FillGradient;
    public Slider m_Slider;
    public Image m_BG;
    public Image m_Fill;

    void Update()
    {
        if (m_Slider == null || m_BG == null || m_Fill == null || float.IsNaN(m_Slider.value)) return;
        m_BG.color = m_BGGradient.Evaluate(m_Slider.value);
        m_Fill.color = m_FillGradient.Evaluate(m_Slider.value);
    }
}
