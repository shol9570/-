using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CSwellArea : MonoBehaviour
{
    [Range(0f, 1f)]
    public float m_Radius = 1f;

    [Range(0f, 1f)]
    public float m_Offset = 0.3f;
}
