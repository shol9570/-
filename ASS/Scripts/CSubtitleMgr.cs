using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class CSubtitleMgr : MonoBehaviour
{
    public static CSubtitleMgr instance;
    
    public Transform m_TrackingSpace;
    public Transform m_SubtitlePanel;
    public GameObject m_SubtitleFactory;
    public Transform m_FollowTarget;
    public float m_Distance = 1f;
    public float m_MinimumDistance = 0.5f;
    public float m_MaximumDistance = 1.5f;
    [Range(0f, 90f)] public float m_MaximumAngle = 60f;
    [Range(0f, 5f)] public float m_LerpTime = 0.5f;

    private Quaternion m_PreviousAngle;
    private Vector3 m_PreviousPos;
    private Vector3 m_TargetPosition;
    private Quaternion m_TargetRotation;
    private float m_LerpTimer = 0f;
    private List<SUBTITLEINFO> m_Subtitles = new List<SUBTITLEINFO>();
    private AudioSource m_NarrationSource;

    struct SUBTITLEINFO
    {
        public string text;
        public float startTime;
        public float duration;
        public Coroutine coroutineID;
    }
    
    IEnumerator Start()
    {
        if (instance == null) instance = this;
        SetTargetPosRot();
        m_MaximumDistance *= m_MaximumDistance; //Squaring distance;
        m_MinimumDistance *= m_MinimumDistance; //Squaring distance;
        m_NarrationSource = this.GetComponent<AudioSource>();
        
        /*
        yield return new WaitForSeconds(3f);
        AddSubtitle("<color=#55ff33>손님</color><color=#111111> : </color>뭐라도 테스트용으로 말할게요.", 10f);
        yield return new WaitForSeconds(5f);
        AddSubtitle("<color=#2958df>플레이어</color><color=#111111> : </color>아무 말이나 블라블라블라블라......", 15f);
        */
        yield return null;
    }

    void Update()
    {
        if (!AngleCheck() || !DistanceCheck())
        {
            SetTargetPosRot();
        }
        m_LerpTimer += Time.deltaTime / m_LerpTime;
        LerpToTargetPosRot();
        UpdateSubtitleText();
    }

    void LerpToTargetPosRot()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, m_TargetPosition, m_LerpTimer);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, m_TargetRotation, m_LerpTimer);
    }

    public void SetTargetPosRot()
    {
        m_PreviousAngle = m_FollowTarget.rotation;
        m_PreviousPos = m_FollowTarget.position + m_FollowTarget.forward * m_Distance;
        m_TargetPosition = m_FollowTarget.position + m_FollowTarget.forward * m_Distance;
        m_TargetRotation = Quaternion.LookRotation(m_FollowTarget.forward, Vector3.up);
        m_LerpTimer = 0f;
    }

    bool AngleCheck()
    {
        return Quaternion.Angle(m_PreviousAngle, m_FollowTarget.rotation) < m_MaximumAngle;
    }

    bool DistanceCheck()
    {
        float squareDist = (m_PreviousPos - m_FollowTarget.position).sqrMagnitude;
        return squareDist < m_MaximumDistance && squareDist > m_MinimumDistance;
    }

    // void InitializeElementsParameter()
    // {
    //     CanvasRenderer[] elementsRenderers = this.GetComponentsInChildren<CanvasRenderer>();
    //     foreach (CanvasRenderer renderer in elementsRenderers)
    //     {
    //         for (int i = 0; i < renderer.materialCount; i++)
    //         {
    //             renderer.GetMaterial(i).SetInt("")
    //         }
    //     }
    // }

    void UpdateSubtitleText()
    {
        
    }

    public void AddSubtitle(string _text, float _duration)
    {
        SUBTITLEINFO subInfo = new SUBTITLEINFO();
        subInfo.text = _text;
        subInfo.duration = _duration;
        subInfo.startTime = Time.time;
        subInfo.coroutineID = StartCoroutine(SubtitleControl(subInfo));
        m_Subtitles.Add(subInfo);
    }

    public void AddSubtitle(string _text, float _duration, AudioClip _narration)
    {
        if (m_NarrationSource != null) m_NarrationSource.PlayOneShot(_narration);
        AddSubtitle(_text, _duration);
    }

    IEnumerator SubtitleControl(SUBTITLEINFO _info)
    {
        GameObject subtitle = Instantiate(m_SubtitleFactory, m_SubtitlePanel);
        //Canvas.ForceUpdateCanvases();
        RectTransform rect = subtitle.GetComponent<RectTransform>();
        Vector2 deltaSize = rect.sizeDelta;
        Text text = subtitle.GetComponent<Text>();
        text.text = _info.text;
        float height = text.preferredHeight;
//        print(height);
        CanvasRenderer renderer = text.GetComponent<CanvasRenderer>();
        float alpha = 0f;
        renderer.SetAlpha(alpha);
        
        if (_info.duration > 2f)
        {
            while (_info.startTime + 1f > Time.time)
            {
                alpha += Time.deltaTime;
                renderer.SetAlpha(alpha);
                deltaSize.y += height * Time.deltaTime;
                rect.sizeDelta = deltaSize;
                //Canvas.ForceUpdateCanvases();
                yield return null;
            }

            alpha = 1f;
            renderer.SetAlpha(alpha);
            deltaSize.y = height;
            rect.sizeDelta = deltaSize;
            //Canvas.ForceUpdateCanvases();
            yield return new WaitForSeconds(_info.duration - 2f);
            
            while (_info.startTime + _info.duration > Time.time)
            {
                alpha -= Time.deltaTime;
                renderer.SetAlpha(alpha);
                yield return null;
            }
        }
        else
        {
            deltaSize.y = height;
            rect.sizeDelta = deltaSize;
            //Canvas.ForceUpdateCanvases();
            yield return new WaitForSeconds(_info.duration);
        }
        Destroy(subtitle);
        m_Subtitles.Remove(_info);
    }
}
