using System;
using System.Collections;
using System.Collections.Generic;
using BNG;
using OVRTouchSample;
using UnityEngine;

public class CFractureCureTool : GrabbableEvents
{
    public OVRInput.Axis1D m_UseButton;
    public float m_Range = 0.4f;
    public float m_Size = 0.3f;
    public float m_CureStrength = 0.1f;
    public AudioSource m_ToolSound;
    public MeshRenderer m_Effect;

    private Grabber m_GrabberObj;
    private OVRInput.Controller m_Grabber = OVRInput.Controller.None;
    
    private void Update()
    {
        ToolCore();
    }

    void ToolCore()
    {
        //Check is grabbed;
        if (m_Grabber == OVRInput.Controller.None) return;
        if(OVRInput.Get(m_UseButton, m_Grabber) > 0.5f)
        {
            //When using
            if(!m_ToolSound.isPlaying) m_ToolSound.Play();
            m_Effect.enabled = true;
            //Cure fracture
            Ray ray = new Ray(this.transform.position, this.transform.forward);
            Vector3 dir = this.transform.forward;
            Vector3 size = new Vector3(m_Size * 0.5f, m_Size * 0.5f, m_Range * 0.5f);
            RaycastHit[] hits = Physics.BoxCastAll(this.transform.position + dir * m_Range * 0.5f, size, dir,
                this.transform.rotation, 0f, 1 << LayerMask.NameToLayer("ToolInteractable"));
            for (int i = 0; i < (hits.Length > 0 ? 1 : 0); i++)
            {
                if (!hits[i].transform.CompareTag("Fracture")) continue; //If not fracture mesh, skip this hit
                if (Vector3.Dot(hits[i].transform.forward, this.transform.forward) > Mathf.Cos(120 * Mathf.Deg2Rad)) continue; //If fracture is over than 60 degree, skip curing
                //this.transform.localScale = Vector3.one * 1.1f;
                CFractureDecal fd = hits[i].transform.GetComponent<CFractureDecal>();
                float depth = fd.m_FractureDepth;
                if (depth == 0f) continue;
                float updateDepth = depth - m_CureStrength * Time.deltaTime;
                updateDepth = updateDepth < 0 ? 0f : updateDepth;
                fd.UpdateDepth(updateDepth);
            }
        }
        else
        {
            //When not using
            //this.transform.localScale = Vector3.one;
            m_Effect.enabled = false;
            m_ToolSound.Stop();
        }
    }

    public override void OnGrab(Grabber grabber)
    {
        m_GrabberObj = grabber;
        m_Grabber = grabber.HandSide == ControllerHand.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
        //if (m_GrabberObj != null) m_GrabberObj.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
    }
    
    public override void OnRelease()
    {
        m_Grabber = OVRInput.Controller.None;
        //if (m_GrabberObj != null) m_GrabberObj.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        Idle();
    }

    void Idle()
    {
        m_Effect.enabled = false;
        //this.transform.localScale = Vector3.one;
        m_ToolSound.Stop();
    }
}
