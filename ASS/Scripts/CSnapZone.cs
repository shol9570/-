using System;
using System.Collections;
using System.Collections.Generic;
using BNG;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CSnapZone : MonoBehaviour
{
    [System.Serializable]
    public struct WhitelistInfo
    {
        public string m_Target;
        public GameObject m_Preview;
        public bool m_IsPrefab;
    }

    public Transform m_SnapTarget; //Can be null if doesn't need to be child
    public List<WhitelistInfo> m_Whitelist;
    public bool m_IsHaveToBeHeld = true;
    public float m_SnapDist = 0.5f;
    private float m_SqrSnapDist; //Squarized snap distance
    public Transform m_SnapIcon;
    public TextMeshProUGUI m_Icon;
    public Gradient m_IconColor;
    private float m_DiffBetweenInnerAndOuter;

    private SphereCollider m_Trigger;

    private GameObject m_SnappedObj;
    private Grabbable m_SnappedGrabbable;
    private CSnappable m_SnappedSnappable;

    void Start()
    {
        m_Trigger = this.GetComponent<SphereCollider>();
        if(m_Trigger.radius < m_SnapDist) Debug.LogWarning("[CSnapZone] Outer radius is lower than inner radius. Please increase sphere collider's radius figure.");
        if (m_SnapTarget != null) this.transform.SetParent(m_SnapTarget);
        //Squarize distance variable.
        m_SqrSnapDist = m_SnapDist * m_SnapDist;
        m_DiffBetweenInnerAndOuter = m_Trigger.radius - m_SnapDist;
    }

    private void Update()
    {
        Snap();
        
        //Set snap icon looking rotation and icon color up.
        if (m_SnappedObj != null)
        {
            m_SnapIcon.transform.LookAt(Camera.main.transform.position);
            m_Icon.gameObject.SetActive(true);
            float diffBetweenSnapObjAndSnapZone =
                Vector3.SqrMagnitude(m_SnappedObj.transform.position - this.transform.position) - m_SqrSnapDist;
            float ratio = diffBetweenSnapObjAndSnapZone / m_DiffBetweenInnerAndOuter; //It couldn't be linear cause obtained the ratio from the square value.
            m_Icon.color = m_IconColor.Evaluate(ratio);
        }
        else
        {
            m_Icon.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        Snap();
    }

    private void FixedUpdate()
    {
        Snap();
    }

    void Snap()
    {
        if (m_SnappedObj == null) return;
        if (Vector3.SqrMagnitude(this.transform.position - m_SnappedObj.transform.position) > m_SqrSnapDist) //If is out of snap range, return this fucntion.
        {
            if (!m_SnappedSnappable) return;
            if (m_SnappedSnappable.m_SnappedZone == this)
            {
                m_SnappedSnappable.m_IsSnapped = false;
                m_SnappedSnappable.m_SnappedZone = null;
            }
            return;
        }
        else //If is in snap range, check has been snapped to another snap zone. If not, initialize the snap info.
        {
            //If snapped obj is already snapped to another snap zone. It will be not snapped.
            if (!m_SnappedSnappable) return;
            if (m_SnappedSnappable.m_SnappedZone != this)
            {
                if (m_SnappedSnappable.m_IsSnapped) return;
                if (m_SnappedSnappable.m_SnappedZone != null) return;
                m_SnappedSnappable.m_IsSnapped = true;
                m_SnappedSnappable.m_SnappedZone = this;
                if (m_IsHaveToBeHeld) //Haptic when is snapped
                {
                    InputBridge.Instance.VibrateController(0.3f, 0.3f, 0.1f,
                        m_SnappedGrabbable.GetControllerHand(m_SnappedGrabbable.GetPrimaryGrabber()));
                }
            }
        }
        if (m_IsHaveToBeHeld)
        {
            if (!IsGrabbed(m_SnappedGrabbable))
            {
                m_SnappedObj = null;
                return;
            }
        }

        m_SnappedObj.transform.position = this.transform.position;
        m_SnappedObj.transform.rotation = this.transform.rotation;
    }

    bool IsGrabbed(Grabbable _target)
    {
        return _target.BeingHeld;
    }

    void SetSnappedObject(GameObject _target)
    {
        m_SnappedObj = _target;
        if (m_IsHaveToBeHeld) m_SnappedGrabbable = _target.GetComponent<Grabbable>();
        m_SnappedSnappable = _target.GetComponent<CSnappable>();
        if (!m_SnappedSnappable) m_SnappedSnappable = _target.AddComponent<CSnappable>();
    }

    void UnsetSnappedObject(GameObject _target)
    {
        m_SnappedObj = null;
        if (m_IsHaveToBeHeld) m_SnappedGrabbable = null;
        if (m_SnappedSnappable)
        {
            m_SnappedSnappable.m_IsSnapped = false;
            m_SnappedSnappable.m_SnappedZone = null;
        }
        m_SnappedSnappable = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Tool")) return;
        //Check is held.
        if (m_IsHaveToBeHeld)
        {
            if (!IsGrabbed(other.attachedRigidbody.GetComponent<Grabbable>())) return;
        }

        //Check is in whitelist if whitelist count is over 0.
        if (m_Whitelist.Count > 0)
        {
            bool isInWhitelist = false;
            for (int i = 0; i < m_Whitelist.Count && !isInWhitelist; i++)
            {
                if (m_Whitelist[i].m_IsPrefab)
                {
                    if (other.attachedRigidbody.gameObject.name.Contains(m_Whitelist[i].m_Target)) isInWhitelist = true;
                }
                else if (other.attachedRigidbody.gameObject.name.Contains(m_Whitelist[i].m_Target)) isInWhitelist = true;
            }

            if (!isInWhitelist) return;
        }

        SetSnappedObject(other.attachedRigidbody.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.attachedRigidbody) return;
        if (other.attachedRigidbody.gameObject == m_SnappedObj) UnsetSnappedObject(m_SnappedObj);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Selection.activeGameObject != this.gameObject) return;
        float innerRaidus = m_SnapDist;
        float outerRadius = this.GetComponent<SphereCollider>().radius;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, innerRaidus);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, outerRadius);
    }
#endif
}