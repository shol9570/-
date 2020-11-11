using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CInflammable : MonoBehaviour
{
    private Mesh m_Mesh;
    private List<CInflammation> m_Inflammations = new List<CInflammation>();
    public int InflammationCount
    {
        get { return m_Inflammations.Count; }
    }
    private bool m_Infected = false;
    private GameObject m_ClearEffectFactory;
    private int m_CreatedInflammationCount = 0;
    public int MaxInflammationCount
    {
        get
        {
            return m_CreatedInflammationCount;
        }
    }

    private CAnimalStatus m_Subordinated;

    public void RemoveInflammationFromList(CInflammation _t)
    {
        m_Inflammations.Remove(_t);
    }

    public void ClearInflammations()
    {
        for (int i = 0; i < m_Inflammations.Count; i++)
        {
            GameObject inflammation = m_Inflammations[i].gameObject;
            m_Inflammations.Remove(m_Inflammations[i]);
            Destroy(inflammation);
            break;
        }
    }

    public Mesh MESH
    {
        get { return m_Mesh; }
    }

    void Start()
    {
        m_Subordinated = this.GetComponentInParent<CAnimalStatus>();
        m_Mesh = this.GetComponent<MeshFilter>().mesh;
        m_ClearEffectFactory = Resources.Load("Effects/StarParticle") as GameObject;
    }

    public void InflammatoryInfection(int _severity, float _minSize, float _maxSize, GameObject _factory)
    {
        int[] tris = MESH.triangles;
        int triCount = tris.Length / 3;

        for (int i = 0; i < _severity; i++)
        {
            //Choose random polygon(triangle)
            int randomTri = tris[Random.Range(0, triCount) * 3];

            //Vertices and normals
            Vector3 vertex0 = m_Mesh.vertices[randomTri];
            Vector3 vertex1 = m_Mesh.vertices[randomTri + 1];
            Vector3 vertex2 = m_Mesh.vertices[randomTri + 2];
            Vector3 normal0 = m_Mesh.normals[randomTri];
            Vector3 normal1 = m_Mesh.normals[randomTri + 1];
            Vector3 normal2 = m_Mesh.normals[randomTri + 2];

            //Convert to world position and factor
            vertex0 = this.transform.TransformPoint(vertex0);
            vertex1 = this.transform.TransformPoint(vertex1);
            vertex2 = this.transform.TransformPoint(vertex2);
            normal0 = this.transform.TransformDirection(normal0);
            normal1 = this.transform.TransformDirection(normal1);
            normal2 = this.transform.TransformDirection(normal2);

            //3 random factor to calculate random position on polygon(triangle)
            float randomA, randomB, randomC;
            randomA = Random.Range(0f, 1f);
            randomB = Random.Range(0f, 1f);
            randomC = Random.Range(0f, 1f);
            //Random position on polygon
            Vector3 zeroToOneVert = Vector3.Lerp(vertex0, vertex1, randomA);
            Vector3 zeroToTwoVert = Vector3.Lerp(vertex0, vertex2, randomB);
            Vector3 targetPos = Vector3.Lerp(zeroToOneVert, zeroToTwoVert, randomC);
            Vector3 zeroToOneNormal = Vector3.Lerp(normal0, normal1, randomA);
            Vector3 zeroToTwoNormal = Vector3.Lerp(normal0, normal2, randomB);
            Vector3 targetNormal = Vector3.Lerp(zeroToOneNormal, zeroToTwoNormal, randomC);
            targetNormal.Normalize();

            //Create inflammation object
            GameObject targetInflammation = Instantiate(_factory, this.transform);
            targetInflammation.hideFlags = HideFlags.HideInHierarchy;
            targetInflammation.transform.position = targetPos;
            targetInflammation.transform.LookAt(targetPos + targetNormal);
            float randomSize = Random.Range(_minSize, _maxSize); //Random size
            targetInflammation.transform.localScale = new Vector3(randomSize, randomSize, randomSize);
            CInflammation inflammation = targetInflammation.GetComponent<CInflammation>();
            if (!inflammation) inflammation = targetInflammation.AddComponent<CInflammation>();
            inflammation.SetHost = this;
            inflammation.m_Endurance = Random.Range(1f, 2f);

            m_Inflammations.Add(inflammation);
            m_CreatedInflammationCount++;
        }
        
        m_Infected = true;
    }

    public void SuckedByTool(float _strength, float _range, Vector3 _suckDirection, CSuctionTool _tool)
    {
        if (!m_Infected) return;
        //Get sucked-able CInflammations
        float sqrRange = _range * _range;
        foreach (CInflammation inflammation in m_Inflammations)
        {
            if (Vector3.SqrMagnitude(inflammation.transform.position - _tool.m_SuctionPoint.position) > sqrRange) continue;
            float dot = Vector3.Dot(_suckDirection, inflammation.transform.forward);
            if (/*dot < 0f*/ true) { if(inflammation.ReduceEndurance(_tool, _strength)) break; }
        }

        if (m_Inflammations.Count == 0)
        {
            GameObject effect = Instantiate(m_ClearEffectFactory, this.transform.position, Quaternion.identity);
            m_Subordinated.m_Inflamed.Remove(this);
            m_Infected = false;
        }
    }
}