using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFracturable : MonoBehaviour
{
    public GameObject[] m_Fracturables;

    public int m_Count = 0;
    public GameObject[] m_Decals;
    [Range(0f, 1f)] public float m_FractureDepth;
    [Tooltip("0 to depth what you set.")] public bool m_RandomDepth = false;

    void Start()
    {
        Fracturing();
    }

    //Create fractures with values user set
    void Fracturing()
    {
        //Weight value of each fracturable objects. Weigth is using in choose random position;
        float[] weight = new float[m_Fracturables.Length];
        float max = 0;
        for (int i = 0; i < weight.Length; i++)
        {
            //Weight is estimate by size of object bounds
            Vector3 size = m_Fracturables[i].GetComponent<MeshFilter>().mesh.bounds.size;
            weight[i] = size.x + size.y + size.z;
            max += weight[i];
        }

        for (int j = 0; j < m_Count; j++)
        {
            //First, choose random fracturable object
            float randomObject = Random.Range(0f, max);
            float acc = 0f;
            GameObject target = null;
            for (int i = 0; i < weight.Length; i++)
            {
                acc += weight[i];
                if (randomObject < acc)
                {
                    target = m_Fracturables[i];
                    i = weight.Length;
                }
            }

            //Second, choose random position on object
            Mesh targetMesh = target.GetComponent<MeshFilter>().mesh;
            int randomIdx = Random.Range(0, targetMesh.vertices.Length);
            Vector3 pos = targetMesh.vertices[randomIdx];
            Vector3 normal = targetMesh.normals[randomIdx];
            //Set parent null
            Transform parent = target.transform.parent;
            target.transform.SetParent(null);
            //Vertex position to world position
            pos = target.transform.TransformPoint(pos);
            //Vertex normal to world normal
            normal = target.transform.TransformDirection(normal);
            normal.Normalize();
            target.transform.SetParent(parent);

            //Select random decal
            int randomDecal = Random.Range(0, m_Decals.Length);
            float depth = m_RandomDepth ? Random.Range(0f, m_FractureDepth) : m_FractureDepth;
            //Create decal
            CreateFractureDecal(m_Decals[randomDecal], pos, normal, depth);
        }
    }

    void CreateFractureDecal(GameObject _decal, Vector3 _pos, Vector3 _normal, float _depth)
    {
        GameObject decal = Instantiate(_decal);
        decal.transform.position = _pos;
        //decal.transform.Rotate(Vector3.up, Random.Range(0f, 360f));
        decal.transform.LookAt(_pos + _normal);
        decal.transform.position -= decal.transform.forward * 0.005f;
        decal.GetComponent<MeshRenderer>().material.SetFloat("_DepthStrength", _depth);
        decal.GetComponent<CFractureDecal>().SetProjectDirection(-_normal);
    }
}