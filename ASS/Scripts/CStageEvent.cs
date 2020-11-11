using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStageEvent : MonoBehaviour
{
    private bool m_EventRegisted = false;

    private void Awake()
    {
        RegistEvent();
    }
    
    private void OnEnable()
    {
        RegistEvent();
    }
    
    private void OnDisable()
    {
        UnregistEvent();
    }

    private void RegistEvent()
    {
        if (m_EventRegisted) return;
        m_EventRegisted = true;
        GameManager.Manager.sceneLoadedEvent += SceneLoadedEvent;
        GameManager.Manager.sceneBeginLoadEvent += SceneBeginLoadEvent;
        GameManager.Manager.sceneUnloadedEvent += SceneUnloadedEvent;
        GameManager.Manager.sceneBeginUnloadEvent += SceneBeginUnloadEvent;
        GameManager.Manager.resultWindowEvent += ResultWindowEvent;
    }

    private void UnregistEvent()
    {
        if (!m_EventRegisted) return;
        m_EventRegisted = false;
        GameManager.Manager.sceneLoadedEvent -= SceneLoadedEvent;
        GameManager.Manager.sceneBeginLoadEvent -= SceneBeginLoadEvent;
        GameManager.Manager.sceneUnloadedEvent -= SceneUnloadedEvent;
        GameManager.Manager.sceneBeginUnloadEvent -= SceneBeginUnloadEvent;
        GameManager.Manager.resultWindowEvent -= ResultWindowEvent;
    }

    public virtual void SceneLoadedEvent(){}
    
    public virtual void SceneBeginLoadEvent(){}
    
    public virtual void SceneUnloadedEvent(){}
    
    public virtual void SceneBeginUnloadEvent(){}
    
    public virtual void ResultWindowEvent(){}
}
