using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerView : MonoBehaviour
{
    public float m_RotateSpeed = 30f;
    public float m_VerticalViewRange = 60f;

    Camera m_CharCam;

    void Start()
    {
        m_CharCam = Camera.main;
    }

    void Update()
    {
        RotateCharAndCam();
    }

    void RotateCharAndCam()
    {
        float h = Input.GetAxis("Mouse X");
        float v = -Input.GetAxis("Mouse Y");

        this.transform.Rotate(Vector3.up * h * m_RotateSpeed * Time.deltaTime);
        Vector3 localEuler = m_CharCam.transform.localEulerAngles;
        float localAngle = localEuler.x;
        if (localAngle > 180f) localAngle -= 360f;
        float angle = localAngle + v * m_RotateSpeed * Time.deltaTime;
        angle = Mathf.Clamp(angle, -m_VerticalViewRange, m_VerticalViewRange);
        m_CharCam.transform.localEulerAngles = new Vector3(angle, localEuler.y, localEuler.z);

    }
}
