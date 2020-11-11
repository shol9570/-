using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStarParticle : MonoBehaviour
{
    void Start()
    {
        this.gameObject.hideFlags = HideFlags.HideInHierarchy;
        Destroy(this.gameObject, 4f);
    }
}
