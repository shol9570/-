using System;
using System.Collections;
using System.Collections.Generic;
using BNG;
using UnityEngine;
using ToolState = Global_Var.CToolGlobal.ToolState;

[RequireComponent(typeof(AudioSource))]
public class CSyringeTool : GrabbableEvents
{
    #region DEFAULT_VALUES
    public Transform m_CylinderPosition;
    public GameObject m_Cylinder = null;
    public Transform m_SyringeNiddle;
    public Collider m_SyringeColl;
    public ParticleSystem m_UseEffect;
    public GameObject m_CureEffect;
    public float m_CurePower = 0.1f;
    private CLiquid m_Liquid;
    public Animation m_InjectAnimation;
    #endregion

    #region CONTROL_VALUES
    public OVRInput.Axis1D m_UseButton;
    public OVRInput.Button m_UseRawButton;
    public float m_CylinderUseSpeed = 0.1f;
    #endregion
    
    #region SOUNDS
    public AudioClip m_StartSound;
    public AudioClip m_StartSoundEmpty;
    #endregion
    
    #region PRIVATE
    private Grabber m_GrabberObj;
    private OVRInput.Controller m_Grabber;
    private bool m_IsSyringeUsing = false;
    private float m_CylinderMaxVolume;
    private Material[] m_SquirtLiquidMat;
    private AudioSource m_UseSoundEffect;
    private AudioSource m_SyringeUseSoundEffect;
    #endregion

    private ToolState m_ToolState = ToolState.IDLE;
    
    void Start()
    {
        m_SquirtLiquidMat = new Material[2];
        m_SquirtLiquidMat[0] = m_UseEffect.GetComponent<ParticleSystemRenderer>().material;
        m_SquirtLiquidMat[1] = m_UseEffect.GetComponent<ParticleSystemRenderer>().trailMaterial;
        m_SyringeUseSoundEffect = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        UseSyringe();
        InjectionSound();
    }

    void InjectionSound()
    {
        if (m_Cylinder == null) return;
        if (m_IsSyringeUsing)
        {
            if(!m_UseSoundEffect.isPlaying) m_UseSoundEffect.Play();
            m_UseSoundEffect.volume = m_UseSoundEffect.volume > m_CylinderMaxVolume ? m_CylinderMaxVolume : m_UseSoundEffect.volume + m_CylinderMaxVolume * Time.deltaTime;
        }
        else
        {
            if (m_UseSoundEffect.volume <= 0f) m_UseSoundEffect.Stop();
            m_UseSoundEffect.volume = m_UseSoundEffect.volume <= 0 ? 0 : m_UseSoundEffect.volume - m_CylinderMaxVolume * Time.deltaTime;
        }
    }
    
    void UseSyringe()
    {
        //Check is grabbed;
        if (m_Grabber == OVRInput.Controller.None) return;
        if (OVRInput.GetDown(m_UseRawButton, m_Grabber))
        {
            if (m_Cylinder != null && m_Liquid.m_LiquidRemain > 0f)
            {
                m_SyringeUseSoundEffect.clip = m_StartSound;
            }
            else
            {
                m_SyringeUseSoundEffect.clip = m_StartSoundEmpty;
            }
            m_SyringeUseSoundEffect.Play();
        }
        if (OVRInput.Get(m_UseButton, m_Grabber) > 0.5f)
        {
            if (m_Cylinder != null && m_Liquid != null ? m_Liquid.m_LiquidRemain > 0f : false)
            {
                if (!m_IsSyringeUsing) m_IsSyringeUsing = true;

                float pressPower = (OVRInput.Get(m_UseButton, m_Grabber) - 0.5f) * 2f;

                //Check is syringe niddle snapped
                CSnappable snappable = this.GetComponent<CSnappable>();
                if (snappable && snappable.m_IsSnapped)
                {
                    CAnimalStatus animalStatus =
                        snappable.m_SnappedZone.m_SnapTarget.GetComponentInParent<CAnimalStatus>();
                    if (m_Liquid.m_LiquidRemain > 0f)
                    {
                        CCylinderType cylinderType = m_Cylinder.GetComponent<CCylinderType>();
                        float hapticStrength = 0;
                        float hapticDuration = 0;
                        int cureInfo = 0;
                        switch (cylinderType.m_AntibioticsType)
                        {
                            case CylinderType.None:
                                break;
                            case CylinderType.FractureAnti:
                                cureInfo = animalStatus.DecreaseFractureAntiCoeff(Time.deltaTime * 0.1f);
                                if (cureInfo > 0)
                                {
                                    hapticStrength = (1f - animalStatus.m_FractureAntiCoeff) * 0.5f;
                                    hapticDuration = Time.deltaTime;
                                }
                                else if (cureInfo == 0)
                                {
                                    GameObject particle = Instantiate(m_CureEffect, m_SyringeNiddle.position,
                                        Quaternion.identity);
                                    hapticStrength = 1f;
                                    hapticDuration = 1f;
                                }
                                break;
                            case CylinderType.InflammationAnti:
                                cureInfo = animalStatus.DecreaseInflammationAntiCoeff(Time.deltaTime * 0.1f);
                                if (cureInfo > 0)
                                {
                                    hapticStrength = (1f - animalStatus.m_InflammationAntiCoeff) * 0.5f;
                                    hapticDuration = Time.deltaTime;
                                }
                                else if (cureInfo == 0)
                                {
                                    GameObject particle = Instantiate(m_CureEffect, m_SyringeNiddle.position,
                                        Quaternion.identity);
                                    hapticStrength = 1f;
                                    hapticDuration = 1f;
                                }
                                break;
                            case CylinderType.FeverReducer:
                                animalStatus.DecreaseTemperature(0.2f * Time.deltaTime);
                                if (m_Liquid.m_LiquidRemain > 0)
                                {
                                    hapticStrength = 1f - m_Liquid.m_LiquidRemain;
                                    hapticDuration = Time.deltaTime;
                                }
                                break;
                            case CylinderType.ColdAnti:
                                cureInfo = animalStatus.DecreaseAntiColdCoeff(Time.deltaTime * 0.1f);
                                if (cureInfo > 0)
                                {
                                    hapticStrength = (1f - animalStatus.m_AntiColdCoeff) * 0.5f;
                                    hapticDuration = Time.deltaTime;
                                }
                                else if (cureInfo == 0)
                                {
                                    GameObject particle = Instantiate(m_CureEffect, m_SyringeNiddle.position,
                                        Quaternion.identity);
                                    hapticStrength = 1f;
                                    hapticDuration = 1f;
                                }
                                break;
                            case CylinderType.Anthelmintic:
                                cureInfo = animalStatus.DecreaseAnthelminticCoeff(Time.deltaTime * 0.1f);
                                if (cureInfo > 0)
                                {
                                    hapticStrength = (1f - animalStatus.m_AnthelminticCoeff) * 0.5f;
                                    hapticDuration = Time.deltaTime;
                                }
                                else if (cureInfo == 0)
                                {
                                    GameObject partice = Instantiate(m_CureEffect, m_SyringeNiddle.position,
                                        Quaternion.identity);
                                    hapticStrength = 1f;
                                    hapticDuration = 1f;
                                }
                                break;
                            case CylinderType.DHPPL:
                                cureInfo = animalStatus.DecreaseDHPPLCoeff(Time.deltaTime * 0.1f);
                                if (cureInfo > 0)
                                {
                                    hapticStrength = (1f - animalStatus.m_DHPPLCoeff) * 0.5f;
                                    hapticDuration = Time.deltaTime;
                                }
                                else if (cureInfo == 0)
                                {
                                    GameObject partice = Instantiate(m_CureEffect, m_SyringeNiddle.position,
                                        Quaternion.identity);
                                    hapticStrength = 1f;
                                    hapticDuration = 1f;
                                }
                                break;
                        }
                        InputBridge.Instance.VibrateController(10f,
                            hapticStrength,
                            hapticDuration, m_GrabberObj.HandSide);
                        //OVRInput.SetControllerVibration(10.0f,1f, m_Grabber);
                    }
                    else
                    {
                        m_ToolState = ToolState.IDLE;
                    }
                }
                else if (m_Liquid.m_LiquidRemain > 0f) //When niddle is not injected
                {
                    m_ToolState = ToolState.NOT_CURING;
                }
                else
                {
                    m_ToolState = ToolState.IDLE;
                }

                //When syringe is using but not curing anything
                if (m_ToolState == ToolState.NOT_CURING)
                {
                    LiquidParticle(true);
                    OVRInput.SetControllerVibration(0f, 0f, m_Grabber);
                }
                else if (m_ToolState == ToolState.IDLE)
                {
                    LiquidParticle(false);
                    OVRInput.SetControllerVibration(0f, 0f, m_Grabber);
                }
                else if (m_ToolState == ToolState.CURING)
                {
                    LiquidParticle(false);
                }

                //Reduce liquid capacity
                m_Liquid.UpdateLiquidCapacity(m_Liquid.m_LiquidRemain - m_CylinderUseSpeed * Time.deltaTime);
                m_InjectAnimation.clip.SampleAnimation(m_InjectAnimation.gameObject, 1f - m_Liquid.m_LiquidRemain);
            }
            else
            {
                LiquidParticle(false);
                OVRInput.SetControllerVibration(0f, 0f, m_Grabber);
            }
        }
        else if(OVRInput.Get(m_UseButton, m_Grabber) < 0.5f)
        {
            if (m_IsSyringeUsing)
            {
                m_ToolState = ToolState.IDLE;
                LiquidParticle(false);
                m_IsSyringeUsing = false;
                OVRInput.SetControllerVibration(0f, 0f, m_Grabber);
            }
        }
    }

    //When load cylinder
    //Set position and initialize data
    public void LoadCylinder()
    {
        m_Cylinder = m_CylinderPosition.transform.GetChild(0).gameObject;
        m_Cylinder.transform.position = m_CylinderPosition.position;
        m_Cylinder.transform.rotation = m_CylinderPosition.rotation;
        m_Cylinder.transform.SetParent(m_CylinderPosition);

        m_UseSoundEffect = m_Cylinder.GetComponentInChildren<AudioSource>();
        m_CylinderMaxVolume = m_UseSoundEffect.volume;
        m_Cylinder.GetComponent<AudioSource>().volume = 0f;
        m_Liquid = m_Cylinder.GetComponentInChildren<CLiquid>();
        //Physics.IgnoreCollision(m_SyringeColl, m_Cylinder.GetComponentInChildren<Collider>());

        //m_Cylinder.GetComponent<Grabbable>().enabled = false;
    }

    //When unload cylinder
    public void UnLoadCylinder()
    {
        m_Cylinder = null;
        m_Liquid = null;
        m_UseSoundEffect = null;
        m_InjectAnimation.clip.SampleAnimation(m_InjectAnimation.gameObject, 0f);
    }

    public override void OnGrab(Grabber grabber)
    {
        m_GrabberObj = grabber;
        m_Grabber = grabber.HandSide == 0 ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
        // if (m_GrabberObj != null)
        // {
        //     m_GrabberObj.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        // }
    }

    public override void OnRelease()
    {
        m_ToolState = ToolState.IDLE;
        
        LiquidParticle(false);
        m_Grabber = OVRInput.Controller.None;
        
        //Releasing when using syringe
        if (m_IsSyringeUsing)
        {
            m_IsSyringeUsing = false;
            OVRInput.SetControllerVibration(0f, 0f, m_Grabber);
        }
        
        if (m_GrabberObj != null)
        {
            //m_GrabberObj.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            m_GrabberObj = null;
        }
    }

    void LiquidParticle(bool _on)
    {
        if (_on)
        {
            m_SquirtLiquidMat[0].color = m_Liquid.GetLiquidColor();
            m_SquirtLiquidMat[1].color = m_Liquid.GetLiquidColor();
            if(!m_UseEffect.isEmitting) m_UseEffect.Play(true);
            //if (m_UseEffect.isPaused) m_UseEffect.Pause(false);
        }
        else
        {
            if (m_UseEffect.isPlaying) m_UseEffect.Stop(true);
            //if (!m_UseEffect.isPaused) m_UseEffect.Pause(true);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        
    }
}
