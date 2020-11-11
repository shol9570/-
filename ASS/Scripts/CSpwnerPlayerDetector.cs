using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSpwnerPlayerDetector : MonoBehaviour
{
    public CToolSpawner m_Spawner;
    
    private void OnTriggerEnter(Collider other)
    {
        m_Spawner.OnPlayerDetected();
    }

    private void OnTriggerExit(Collider other)
    {
        m_Spawner.OnPlayerMissed();
    }
}
