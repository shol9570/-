using System;
using System.Collections;
using System.Collections.Generic;
using BNG;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class CSuctionTool : GrabbableEvents
{
    public OVRInput.Button m_UseButton;
    public GameObject m_UseEffect;
    public float m_SuctionStrength = 1f;
    public Transform m_SuctionPoint;
    
    #region SuckRange
    public Vector3 m_RangeCenter = new Vector3(0,0,0.06f);
    public Vector3 m_RangeExtends = new Vector3(0.05f,0.02f,0.06f);
    #endregion

    #region Sound
    public AudioClip m_StartSound;
    public AudioClip m_LoopSound;
    public AudioClip m_EndSound;
    private AudioSource m_AS;
    #endregion
    
    private OVRInput.Controller m_GrabCtrler = OVRInput.Controller.None;
    private bool m_SwitchOn = false;

    private void Start()
    {
        m_AS = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        Suction();
        //SuckInflammation();
    }
    
    void Suction()
    {
        if (OVRInput.GetDown(m_UseButton, m_GrabCtrler))
        {
            m_AS.clip = m_StartSound;
            m_AS.loop = false;
            if(!m_SwitchOn) m_AS.Play();
            m_SwitchOn = true;
            SetEffect(m_SwitchOn);
        }
        else if (OVRInput.Get(m_UseButton, m_GrabCtrler))
        {
            if (m_AS.clip != m_LoopSound && !m_AS.isPlaying)
            {
                m_AS.clip = m_LoopSound;
                m_AS.loop = true;
                m_AS.Play();
            }
            SuckInflammation();
        }
        else if (OVRInput.GetUp(m_UseButton, m_GrabCtrler))
        {
            m_AS.clip = m_EndSound;
            m_AS.loop = false;
            if(m_SwitchOn) m_AS.Play();
            m_SwitchOn = false;
            SetEffect(m_SwitchOn);
        }
    }

    void SuckInflammation()
    {
        Collider[] colls = Physics.OverlapBox(m_SuctionPoint.TransformPoint(m_RangeCenter), m_RangeExtends, m_SuctionPoint.rotation, 1 << LayerMask.NameToLayer("ToolInteractable"));
        if (colls.Length > 0)
        {
            for (int i = 0; i < colls.Length; i++)
            {
                CInflammable inflammable = colls[i].GetComponent<CInflammable>();
                if (inflammable)
                {
                    inflammable.SuckedByTool(m_SuctionStrength * Time.deltaTime, 0.3f, m_SuctionPoint.forward, this);
                }
            }
        }
    }

    void SetEffect(bool _enable)
    {
        ParticleSystem[] pss = m_UseEffect.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in pss)
        {
            if(_enable) ps.Play();
            else ps.Stop();
        }
    }

    public override void OnGrab(Grabber grabber)
    {
        m_GrabCtrler = grabber.HandSide == ControllerHand.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
    }

    public override void OnRelease()
    {
        m_GrabCtrler = OVRInput.Controller.None;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Selection.activeGameObject != this.gameObject) return;
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[8];
        vertices[0] = new Vector3(m_RangeExtends.x, m_RangeExtends.y, m_RangeExtends.z);
        vertices[1] = new Vector3(-m_RangeExtends.x, m_RangeExtends.y, m_RangeExtends.z);
        vertices[2] = new Vector3(m_RangeExtends.x, -m_RangeExtends.y, m_RangeExtends.z);
        vertices[3] = new Vector3(m_RangeExtends.x, m_RangeExtends.y, -m_RangeExtends.z);
        vertices[4] = new Vector3(-m_RangeExtends.x, -m_RangeExtends.y, m_RangeExtends.z);
        vertices[5] = new Vector3(-m_RangeExtends.x, m_RangeExtends.y, -m_RangeExtends.z);
        vertices[6] = new Vector3(m_RangeExtends.x, -m_RangeExtends.y, -m_RangeExtends.z);
        vertices[7] = new Vector3(-m_RangeExtends.x, -m_RangeExtends.y, -m_RangeExtends.z);
        int[] tris = new int[36]
        {
            0, 1, 2,
            2, 1, 4,
            3, 0, 2,
            3, 2, 6,
            1, 5, 4,
            4, 5, 7,
            5, 3, 6,
            5, 6, 7,
            1, 0, 5,
            0, 3, 5,
            4, 7, 2,
            2, 7, 6
        };
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        Color color = Color.cyan;
        color.a = 0.5f;
        Gizmos.color = color;
        Gizmos.DrawMesh(mesh, m_SuctionPoint.TransformPoint(m_RangeCenter), m_SuctionPoint.rotation);
    }
#endif
}
