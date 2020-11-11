using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEvenetManager : MonoBehaviour
{
    public delegate void Invoke();

    public event Invoke sceneLoadedEvent;
    public event Invoke sceneBeginLoadEvent;
    public event Invoke sceneUnloadedEvent;
    public event Invoke sceneBeginUnloadEvent;
    public event Invoke resultWindowEvent;

    public void OnSceneLoaded()
    {
        if (sceneLoadedEvent != null) sceneLoadedEvent();
    }

    public void OnSceneBeginLoad()
    {
        if (sceneBeginLoadEvent != null) sceneBeginLoadEvent();
    }

    public void OnSceneUnloaded()
    {
        if (sceneUnloadedEvent != null) sceneUnloadedEvent();
    }

    public void OnSceneBeginUnload()
    {
        if (sceneBeginUnloadEvent != null) sceneBeginUnloadEvent();
    }

    public void OnResultWindowEvent()
    {
        if (resultWindowEvent != null) resultWindowEvent();
    }
}
