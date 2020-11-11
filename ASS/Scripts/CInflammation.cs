using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Inflammation animation
//Inflammation info

public class CInflammation : MonoBehaviour
{
    public float m_Endurance = 2f;
    private bool m_IsAlive = true;
    private Coroutine m_SuckedAnimation;
    private CInflammable m_Host;

    public CInflammable SetHost
    {
        set
        {
            m_Host = value;
        }
    }

    public bool ReduceEndurance(CSuctionTool _suction, float _strength)
    {
        if (!m_IsAlive) return true;
        m_Endurance -= _strength;
        if (m_Endurance <= 0)
        {
            m_IsAlive = false;
            StartCoroutine(SuckedAnimation(_suction));
            return true;
        }
        else if (m_SuckedAnimation == null)
        {
            m_SuckedAnimation = StartCoroutine(SuckingAnimation());
            return false;
        }

        return false;
    }

    IEnumerator SuckingAnimation()
    {
        Vector3 orinLocalPos = this.transform.localPosition;
        Vector3 offset = new Vector3(Random.Range(0f, 0.001f), Random.Range(0f, 0.001f), Random.Range(0f, 0.001f));
        float sin = 0f;
        while (sin < 1f)
        {
            sin += 5f * Time.deltaTime;
            this.transform.position = this.transform.position + offset * Mathf.Sin(sin * 360f * Mathf.Deg2Rad);
            yield return null;
        }
        this.transform.localPosition = orinLocalPos;
        m_SuckedAnimation = null;
    }

    IEnumerator SuckedAnimation(CSuctionTool _suction)
    {
        m_Host.RemoveInflammationFromList(this);
        float suck = 0f;
        Vector3 orinPos = this.transform.position;
        Vector3 targetPos = _suction.m_SuctionPoint.position;
        float ratio = Mathf.Max(Vector3.SqrMagnitude((_suction.m_SuctionPoint.position - this.transform.position) / (_suction.m_RangeExtends.z * _suction.m_RangeExtends.z * 4)), 0f);
        Vector3 up = _suction.m_SuctionPoint.up;
        Vector3 right = _suction.m_SuctionPoint.right;
        float dissolveOffset = 0f;
        while (suck < 1f)
        {
            targetPos = _suction.m_SuctionPoint.position;
            this.transform.position = Vector3.Lerp(orinPos, targetPos, suck);
            this.transform.position += (up * Mathf.Sin(dissolveOffset * Mathf.Deg2Rad) + right * Mathf.Cos(dissolveOffset * Mathf.Deg2Rad)) * 0.02f * (1f - suck);
            suck += Time.deltaTime * _suction.m_SuctionStrength * ratio * 0.2f;
            dissolveOffset += Time.deltaTime * 360f;
            yield return null;
        }

        this.GetComponent<MeshRenderer>().enabled = false;
        AudioSource aS = this.GetComponent<AudioSource>();
        aS.Play();
        yield return new WaitForSeconds(aS.clip.length);
        Destroy(this.gameObject);
    }
}
