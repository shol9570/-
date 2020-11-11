using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CLiquid : MonoBehaviour
{
    public Color m_LiquidColor;
    
    public Transform m_Plane;
    private MeshRenderer m_LiquidMeshRenderer;
    private MeshFilter m_LiquidMeshFilter;

    public Transform m_LiquidTarget;
    public bool m_GetCurrentOffset = false;
    public Vector3 m_PosOffset;
    public Vector3 m_RotOffset;
    public Vector3 m_FaceDirection;

    [Space]
    public float m_BounceDistLimit = 0.01f;
    [Range(0f, 1f)] public float m_BounceStrength = 0.1f;
    [Range(0f, 1f)] public float m_BounceDrag = 0.1f;
    [Range(0f, 90f)] public float m_FluttingAngleLimit = 90f;

    private float m_SqrBounceDistLimit;
    private Quaternion m_OrinPlaneRotation;
    
    [Space]
    [Range(0f ,1f)]public float m_LiquidRemain = 1f;

    public bool m_BottleEffect = false;
    
    private Vector3 m_LiquidAcc;

    void Awake()
    {
        m_LiquidRemain = 1f;
    }
    
    void Start()
    {
        if (m_Plane == null)
        {
            GameObject plane = new GameObject("LiquidPlane");
            m_Plane = plane.transform;
            m_Plane.position = m_LiquidTarget.TransformPoint(m_PosOffset);
            m_Plane.eulerAngles = m_RotOffset;
        }
        else if (m_GetCurrentOffset)
        {
            m_PosOffset = m_Plane.localPosition;
            m_RotOffset = m_Plane.localEulerAngles;
            m_FaceDirection = m_Plane.forward;
        }

        m_Plane.SetParent(null);

        m_LiquidMeshRenderer = m_LiquidTarget.GetComponent<MeshRenderer>();
        m_LiquidMeshFilter = m_LiquidTarget.GetComponent<MeshFilter>();
        m_LiquidMeshFilter.mesh.RecalculateBounds();
        
        #region INITIALIZE_LIQUID

        m_OrinPlaneRotation = Quaternion.LookRotation(m_FaceDirection);
        
        //Squarize variables
        m_SqrBounceDistLimit = m_BounceDistLimit * m_BounceDistLimit;

        #endregion
        
        UpdateLiquidColor();
        m_Plane.gameObject.hideFlags = HideFlags.HideInHierarchy; //Hide plane in hirachy view
    }
    
    void Update()
    {
        Flut();
        UpdateLiquidMaterialProperties();
    }

    void UpdateLiquidMaterialProperties()
    {
        m_LiquidMeshRenderer.material.SetVector("_PlanePosition", this.transform.InverseTransformPoint(m_Plane.position));
        m_LiquidMeshRenderer.material.SetVector("_PlaneNormal", this.transform.InverseTransformDirection(m_Plane.forward));
    }
    
    // void Flut()
    // {
    //     if (m_LiquidRemain <= 0) m_LiquidMeshRenderer.enabled = false;
    //     else m_LiquidMeshRenderer.enabled = true;
    //     
    //     Vector3 posOffset = m_PosOffset;
    //     Vector3 center = m_LiquidMeshFilter.mesh.bounds.center;
    //     Vector3 extents = m_LiquidMeshFilter.mesh.bounds.extents;
    //     
    //     //Need detail!!! WIP!!!!
    //     //Liquid Capacity
    //     float max = extents.y * 2f;
    //     posOffset.y = max * m_LiquidRemain - extents.y;
    //     
    //     //Bottle Effect
    //     if (m_BottleEffect)
    //     {
    //         Vector3 ratioX = new Vector3(1f, extents.x / extents.y, extents.x / extents.z);
    //         Vector3 ratioY = new Vector3(extents.y / extents.x, 1f, extents.y / extents.z);
    //         Vector3 ratioZ = new Vector3(extents.z / extents.x, extents.z / extents.y, 1f);
    //         Vector3 adjusted = new Vector3(
    //             m_LiquidTarget.right.x * ratioX.x * posOffset.x + m_LiquidTarget.right.y * ratioX.y * posOffset.y + m_LiquidTarget.right.z * ratioX.z * posOffset.z,
    //             m_LiquidTarget.up.x * ratioY.x * posOffset.x + m_LiquidTarget.up.y * ratioY.y * posOffset.y + m_LiquidTarget.up.z * ratioY.z * posOffset.z,
    //             m_LiquidTarget.forward.x * ratioZ.x * posOffset.x + m_LiquidTarget.forward.y * ratioZ.y * posOffset.y + m_LiquidTarget.forward.z * ratioZ.z * posOffset.z);
    //         posOffset.x = adjusted.x;
    //         posOffset.y = adjusted.y;
    //         posOffset.z = adjusted.z;
    //     }
    //
    //     //Position
    //     Vector3 targetPos = m_LiquidTarget.TransformPoint(posOffset);
    //     Vector3 dir = targetPos - m_Plane.position;
    //     float sqrtLength = dir.sqrMagnitude;
    //     if (sqrtLength > m_JointEndurance)
    //     {
    //         dir.Normalize();
    //         if (m_JointDistanceLimit != 0 && sqrtLength > m_JointDistanceLimit) //Distance limitation
    //         {
    //             Vector3 mov = dir * (sqrtLength - m_JointDistanceLimit);
    //             m_Plane.position += mov;
    //         }
    //         else
    //         {
    //             Vector3 acc = dir * m_JointStrength * sqrtLength;
    //             m_LiquidAcc += acc;
    //             Vector3 mov = m_LiquidAcc * Time.deltaTime;
    //             mov = Vector3.Lerp(mov.sqrMagnitude > sqrtLength ? dir * sqrtLength : mov, mov, m_Bounceness); //Bounce
    //             m_Plane.position += mov;
    //         }
    //     }
    //
    //     m_LiquidAcc *= 1 - m_JointDrag;
    //     if (m_LiquidAcc.sqrMagnitude < m_JointDeadZone) m_LiquidAcc = Vector3.zero;
    //     //Rotation
    //     Vector3 targetRot = m_RotOffset + new Vector3(Mathf.Clamp(m_LiquidAcc.z * m_FluttingImpact, -1f, 1f) * m_FluttingLimit,
    //         Mathf.Clamp(m_LiquidAcc.x * m_FluttingImpact, -1f, 1f) * m_FluttingLimit,
    //         0f);
    //     m_Plane.eulerAngles = Vector3.Lerp(m_RotOffset, targetRot, m_FluttingStrength * Mathf.Clamp(m_LiquidAcc.sqrMagnitude, 0, 1f));
    // }
    
    // void Flut()
    // {
    //     if (m_LiquidRemain <= 0) return;
    //     
    //     //Basic properties
    //     Vector3 posOffset = m_PosOffset;
    //     Vector3 center = m_LiquidMeshFilter.mesh.bounds.center;
    //     Vector3 extents = m_LiquidMeshFilter.mesh.bounds.extents;
    //
    //     //Remained liquid capacity
    //     float max = extents.y * 2f;
    //     posOffset.y = max * m_LiquidRemain - extents.y;
    //     
    //     //Bottle Effect
    //     if (m_BottleEffect)
    //     {
    //         Vector3 ratioX = new Vector3(1f, extents.x / extents.y, extents.x / extents.z);
    //         Vector3 ratioY = new Vector3(extents.y / extents.x, 1f, extents.y / extents.z);
    //         Vector3 ratioZ = new Vector3(extents.z / extents.x, extents.z / extents.y, 1f);
    //         Vector3 adjusted = new Vector3(
    //             m_LiquidTarget.right.x * ratioX.x * posOffset.x + m_LiquidTarget.right.y * ratioX.y * posOffset.y + m_LiquidTarget.right.z * ratioX.z * posOffset.z,
    //             m_LiquidTarget.up.x * ratioY.x * posOffset.x + m_LiquidTarget.up.y * ratioY.y * posOffset.y + m_LiquidTarget.up.z * ratioY.z * posOffset.z,
    //             m_LiquidTarget.forward.x * ratioZ.x * posOffset.x + m_LiquidTarget.forward.y * ratioZ.y * posOffset.y + m_LiquidTarget.forward.z * ratioZ.z * posOffset.z);
    //         posOffset.x = adjusted.x;
    //         posOffset.y = adjusted.y;
    //         posOffset.z = adjusted.z;
    //     }
    //     
    //     m_PosOffset = posOffset;
    //
    //     Vector3 targetPos = m_LiquidTarget.TransformPoint(posOffset);
    //
    //     //Liquid accelation
    //     Vector3 dir = targetPos - m_Plane.position;
    //     float dist = dir.sqrMagnitude;
    //     dir.Normalize();
    //     if (dist > m_JointDeadZone * m_JointDeadZone) //Liquid range limitation
    //     {
    //         m_Plane.position = targetPos - dir * m_JointDeadZone;
    //         dist = m_JointDeadZone;
    //     }
    //     Vector3 acc = dir * m_JointStrength * Mathf.Clamp(dist / (m_JointDeadZone * m_JointDeadZone), 0f, 1f);
    //     m_LiquidAcc += acc * Time.deltaTime;
    //     
    //     //Set plane position and rotation
    //     m_Plane.position += m_LiquidAcc * Time.deltaTime;
    //     m_Plane.rotation = Quaternion.LookRotation(m_FaceDirection);
    //     
    //     //Compute of accelation resistance
    //     m_LiquidAcc *= (1f - m_JointDrag);
    // }

    void Flut()
    {
        m_LiquidAcc *= 1f - m_BounceDrag;
        
        Vector3 posOffset = m_PosOffset;
        Vector3 center = m_LiquidMeshFilter.mesh.bounds.center;
        Vector3 extents = m_LiquidMeshFilter.mesh.bounds.extents;

        Vector3 localUp = this.transform.InverseTransformDirection(m_FaceDirection);
        Vector3 upward = new Vector3(localUp.x > 0 ? extents.x : -extents.x, localUp.y > 0 ? extents.y : -extents.y, localUp.z > 0 ? extents.z : -extents.z);
        Vector3 downward = -upward;

        Vector3 targetPos = this.transform.TransformPoint(Vector3.Lerp(downward, upward, m_LiquidRemain));
        Vector3 dir = targetPos - m_Plane.position;
        float dist = dir.sqrMagnitude;
        dir.Normalize();

        //adjust position
        if (dist > m_SqrBounceDistLimit)
        {
            m_Plane.position = targetPos - dir * m_BounceDistLimit;
            dist = (targetPos - m_Plane.position).sqrMagnitude;
        }

        m_LiquidAcc += dir * m_BounceStrength * (dist / m_SqrBounceDistLimit);

        m_Plane.transform.position += m_LiquidAcc * Time.deltaTime;

        Quaternion targetRot = m_OrinPlaneRotation;
        targetRot *= Quaternion.Euler(new Vector3(m_LiquidAcc.z, m_LiquidAcc.x, m_LiquidAcc.y) * m_FluttingAngleLimit);
        
        m_Plane.rotation = targetRot;
    }

    public Color GetLiquidColor()
    {
        return m_LiquidMeshRenderer.material.color;
    }
    
    public void UpdateLiquidColor()
    {
        m_LiquidMeshRenderer.material.color = m_LiquidColor;
    }

    public void UpdateLiquidCapacity(float _v)
    {
        m_LiquidRemain = _v;
        if (m_LiquidRemain <= 0) m_LiquidMeshRenderer.enabled = false;
        else m_LiquidMeshRenderer.enabled = true;
    }

    private void OnDestroy()
    {
        if(m_Plane != null) Destroy(m_Plane.gameObject);
    }
}
