using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class CStatusDisplayCtrl : MonoBehaviour
{
    #region NORMAL_PUBLIC
    public CAnimalStatus m_DisplayTarget;
    
    public TextMeshProUGUI m_TreatmentRateTxt;
    public Slider m_TreatmentRateSlider;
    public TextMeshProUGUI m_ConditionFigureTxt;
    public Slider m_ConditionFigureSlider;
    #endregion

    #region NORMAL_PRIVATE
    private bool m_EnableReturnToLobby = false;
    #endregion
    
    #region RESULT_UI_PUBLIC
    public GameObject m_ResultUI;
    public GameObject m_InformationUI;
    public Image m_Temperature;
    public TextMeshProUGUI m_TemperatureTxt;
    public Image[] m_Hearts;
    public Sprite m_HeartEmpty;
    public Sprite m_HeartFull;
    public Sprite m_HeartBroken;
    public TextMeshProUGUI m_ResultTxt;
    public TextMeshProUGUI m_SpendTimeTxt;
    public TextMeshProUGUI m_TipTxt;
    #endregion

    #region SOUND
    public AudioClip[] m_HeartSoundEffect;
    #endregion

    private float m_DisplayingTreatmentRate = 0f;
    private AudioSource m_AudioSource;

    void Start()
    {
        m_AudioSource = this.GetComponent<AudioSource>();
        HideResultUI();
    }

    void Update()
    {
        if (m_DisplayTarget == null) return;
        
        float rate = GameManager.Manager.GetDiseaseMgr.DiseaseTreatmentRate(m_DisplayTarget);
        
        m_DisplayingTreatmentRate = Mathf.Abs(rate - m_DisplayingTreatmentRate) < 0.01f || float.IsNaN(m_DisplayingTreatmentRate) ? rate : Mathf.Lerp(m_DisplayingTreatmentRate, rate, 0.1f);
        
        //Treatment rate
        m_TreatmentRateTxt.text = String.Format("{0:0.0}%", m_DisplayingTreatmentRate * 100);
        m_TreatmentRateSlider.value = m_DisplayingTreatmentRate;

        //Animal condition figure
        m_ConditionFigureTxt.text = String.Format("{0:0.0}%", m_DisplayTarget.ConditionFigure * 100);
        m_ConditionFigureSlider.value = m_DisplayTarget.ConditionFigure;
        
        //Temparature figure
        float temperature = m_DisplayTarget.TemperatureFigure;
        float temperatureRate = ((temperature - 35f) / 6f) * 0.5f;
        m_Temperature.material.SetFloat("_Heat", temperatureRate);
        m_TemperatureTxt.text = string.Format("{0:0.0}ºC", temperature);
    }

    [ContextMenu("OnEndButtonDown")]
    public void OnEndButtonDown()
    {
        print("OnEndButtonDown");
        if (!m_ResultUI.activeSelf && m_DisplayTarget.m_IsConditionChanging) StartCoroutine(ShowResultUI());
        else if(m_EnableReturnToLobby && m_ResultUI.activeSelf && !m_DisplayTarget.m_IsConditionChanging) GameManager.Manager.EndMedicalScene(GameManager.Manager.StageDatabase.StageInfos[GameManager.Manager.lastSelectedStage].levelData[0]); //SceneManager.GetSceneByBuildIndex(GameManager.Manager.lastSelectedStage).name
    }

    public void OnEndButtonUp()
    {
        print("OnEndButtonUp");
    }
    
    public IEnumerator ShowResultUI()
    {
        GameManager.Manager.OnResultWindowEvent(); //Result window event

        m_InformationUI.SetActive(false);
        
        InitializeResultUI();
        m_DisplayTarget.m_IsConditionChanging = false;
        
        m_ResultUI.SetActive(true);
        float treatmentRate = GameManager.Manager.GetDiseaseMgr.DiseaseTreatmentRate(m_DisplayTarget);
        float condition = m_DisplayTarget.ConditionFigure;
        bool[] pass = new bool[] { treatmentRate >= 0.5f && condition > 0f, treatmentRate >= 0.99f && condition > 0f, treatmentRate >= 0.99f && condition >= 0.5f};
        int heartCount = (pass[0] ? 1 : 0) + (pass[1] ? 1 : 0) + (pass[2] ? 1 : 0);
        float spendTime = Time.time - GameManager.Manager.timeAtStageStart;
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < m_Hearts.Length; i++)
        {
            print(pass[i]);
            m_Hearts[i].sprite = pass[i] ? m_HeartFull : m_HeartBroken;
            if (pass[i])
            {
                m_AudioSource.clip = m_HeartSoundEffect[i];
                m_AudioSource.Play();
            }
            yield return new WaitForSeconds(1f);
        }
        
        if (heartCount > 0)
        {
            m_ResultTxt.text = "성공";
            m_ResultTxt.color = new Color(0.87f, 0.88f, 0.15f);
        }
        else
        {
            m_ResultTxt.text = "실패";
            m_ResultTxt.color = new Color(0.8f, 0.25f, 0.2f);
        }

        yield return StartCoroutine(PopupAnimation(m_ResultTxt.gameObject));

        m_SpendTimeTxt.text = string.Format("시간 : <size=0.025>{0:0}\"{1:0.0}\'</size>", spendTime / 60, spendTime % 60);

        yield return StartCoroutine(PopupAnimation(m_SpendTimeTxt.gameObject));

        if (heartCount == 0)
        {
            m_TipTxt.text = "치료율이 너무 낮습니다\n다시 한번 타블렛을 보고 원인을 파악해보세요";
        }
        else if (heartCount == 1)
        {
            m_TipTxt.text = "치료율이 조금 부족합니다\n놓친 부분은 없는지 확인해보세요";
        }
        else if (heartCount == 2)
        {
            m_TipTxt.text = "시간이 너무 오래 걸렸네요\n조금 더 서둘러야할거 같습니다";
        }
        else if (heartCount == 3)
        {
            m_TipTxt.text = "완벽하군요!\n명의가 다 되셨네요";
        }

        yield return StartCoroutine(PopupAnimation(m_TipTxt.gameObject));

        DataSaver.SaveStageData(GameManager.Manager.lastSelectedStage, heartCount > 0, heartCount, spendTime);

        m_EnableReturnToLobby = true;
    }

    IEnumerator PopupAnimation(GameObject _target, float _time = 1f)
    {
        float time = 0f;

        while (time < _time)
        {
            time += Time.deltaTime;
            float scale = Mathf.Min(1f, time / _time);
            float size = Mathf.Sin(360f * scale * Mathf.Deg2Rad) * 0.1f +
                         Mathf.Sin(90f * Mathf.Min(1f, scale * 2f) * Mathf.Deg2Rad) * 1f;
            _target.transform.localScale = Vector3.one * size;
            yield return null;
        }
    }

    void InitializeResultUI()
    {
        for (int i = 0; i < m_Hearts.Length; i++)
        {
            m_Hearts[i].sprite = m_HeartEmpty;
        }
        m_ResultTxt.gameObject.transform.localScale = Vector3.zero;
        m_SpendTimeTxt.gameObject.transform.localScale = Vector3.zero;
        m_TipTxt.gameObject.transform.localScale = Vector3.zero;
    }

    public void HideResultUI()
    {
        m_ResultUI.SetActive(false);
        m_InformationUI.SetActive(true);
    }
}
