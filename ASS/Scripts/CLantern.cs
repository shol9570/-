using System.Collections;
using System.Collections.Generic;
using BNG;
using UnityEngine;

public class CLantern : GrabbableEvents
{
    public float m_Radius = 0.1f;
    public float m_RayDistance = 1f;
    
    GameObject m_XrayedObj;

    public GameObject XRAYEDOBJ
    {
        get
        {
            return m_XrayedObj;
        }
    }
    private bool m_SwitchOn = false;
    public bool Switch
    {
        get
        {
            return m_SwitchOn;
        }
    }
    private OVRInput.Controller m_Grabber = OVRInput.Controller.None;
    public OVRInput.RawButton m_UseButton;
    public MeshRenderer m_Effect;
    public AudioSource m_SwitchSound;

    private Grabber m_GrabberObj;
    public CIgnoreCollisionArea m_IgnoreArea;
    
    void Update()
    {
        Xray();
    }

    void Xray()
    {
        if (m_Grabber == OVRInput.Controller.None) return;
        if (OVRInput.GetDown(m_UseButton, m_Grabber))
        {
            m_SwitchOn = !m_SwitchOn;
            m_Effect.GetComponent<MeshRenderer>().enabled = m_SwitchOn;
            m_SwitchSound.Play();

            if (!m_SwitchOn)
            {
                if (m_XrayedObj != null)
                {
                    SetXrayValue(m_XrayedObj, Vector3.zero, 0f, Vector3.zero);
                    m_XrayedObj = null;
                }
            }
        }
        if (m_SwitchOn)
        {
            Vector3 pos = this.transform.position;
            Vector3 dir = this.transform.forward;
            Ray ray = new Ray(pos, dir);
            RaycastHit hit;
            //When found target
            if (Physics.Raycast(ray, out hit, m_RayDistance, 1 << LayerMask.NameToLayer("Xrayable")))
            {
                //print("Lantern Hit");
                float radius = (hit.distance / m_RayDistance) * m_Radius;

                if (SetXrayValue(hit.transform.gameObject, hit.point, radius, dir))
                {
                    m_XrayedObj = hit.transform.gameObject;
                }
                else
                {
                    if (m_XrayedObj != null)
                    {
                        MissTarget();
                    }
                }
            }
            //Or not found target
            else
            {
                //print("Lantern not hit");
                if (m_XrayedObj != null)
                {
                    MissTarget();
                }
            }
        }
    }

    void MissTarget()
    {
        if (m_XrayedObj == null) return;
        //Reset previous target material value
        SetXrayValue(m_XrayedObj, Vector3.zero, 0f, Vector3.zero);
        //Reset ignored collision
        m_IgnoreArea.ResetIgnoredObjectCollision();
        m_XrayedObj = null;
    }

    bool SetXrayValue(GameObject _target, Vector3 _pos, float _radius, Vector3 _dir)
    {
        Material[] mats = _target.TryGetComponent(out MeshRenderer renderer) ? _target.transform.GetComponent<MeshRenderer>().materials : _target.transform.GetComponent<SkinnedMeshRenderer>().materials;
        List<Material> xrayables = new List<Material>();
        foreach (Material mat in mats)
        {
            //print(mat.shader.name);
            if (mat.shader.name == "SHOL/S_Xray" ||
                mat.shader.name == "SHOL/Stylized Surface X-ray")
            {
                xrayables.Add(mat);
            }
        }

        if (xrayables.Count == 0) return false;

        foreach (Material mat in xrayables)
        {
            mat.SetVector("_Center", _pos);
            mat.SetFloat("_Radius", _radius);
            mat.SetVector("_Raydir", _dir);
        }

        return true;
    }

    public override void OnGrab(Grabber grabber)
    {
        m_GrabberObj = grabber;
        //if (m_GrabberObj != null) m_GrabberObj.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        m_Grabber = grabber.HandSide == 0 ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
    }

    public override void OnRelease()
    {
        m_Grabber = OVRInput.Controller.None;
        MissTarget();
        //if (m_GrabberObj != null) m_GrabberObj.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
    }
}
