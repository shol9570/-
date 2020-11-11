using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerMove : MonoBehaviour
{
    public float m_MoveSpeed = 2f;
    public float m_SprintSpeed = 4f;

    CharacterController m_CharCtrler;
    Camera m_CharCam;
    float GRAVITY = 9.8f;

    void Start()
    {
        m_CharCtrler = this.GetComponent<CharacterController>();
        m_CharCam = Camera.main;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;
        Vector3 moveDir = (inputDir.z * m_CharCam.transform.forward + inputDir.x * m_CharCam.transform.right);
        moveDir.y = 0;
        moveDir.Normalize();

        float speed = Input.GetKey(KeyCode.LeftShift) ? m_SprintSpeed : m_MoveSpeed;
        m_CharCtrler.Move((speed * moveDir + Vector3.down * GRAVITY) * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision _coll)
    {
        if (_coll.transform.CompareTag("Door"))
        {

        }
    }
}
