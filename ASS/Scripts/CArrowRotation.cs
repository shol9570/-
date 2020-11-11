using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CArrowRotation : MonoBehaviour
{
    public float m_Speed = 1f;
    
    void Update()
    {
        this.transform.Rotate(Vector3.right * 360 * Time.deltaTime / m_Speed, Space.Self);
    }
}
