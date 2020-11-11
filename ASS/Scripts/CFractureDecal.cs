using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * 1. Set project direction
 * 2. Project mesh vertices' position on meshes of Objects which is in Fracturable Layer
 */
public class CFractureDecal : MonoBehaviour
{
    public float m_FractureDepth = 0.3f;
    public GameObject m_CureEffect;
    
    private MeshFilter m_Mesh;
    private Vector3[] m_OrinVertices;
    private Vector3[] m_OrinNormals;
    private Vector4[] m_OrinTangents;
    private Vector3 m_ProjectDir;

    private CAnimalStatus m_Subordinated;
    public CAnimalStatus Subordinated
    {
        set
        {
            m_Subordinated = value;
        }
    }

    public Vector3 GetProjectDir
    {
        get
        {
            if (m_ProjectDir == null || m_ProjectDir == Vector3.zero)
            {
                m_ProjectDir = -this.transform.forward;
            }

            return m_ProjectDir;
        }
    }

    void Awake()
    {
        m_Mesh = this.GetComponent<MeshFilter>();
        m_OrinVertices = m_Mesh.sharedMesh.vertices;
        m_OrinNormals = m_Mesh.sharedMesh.normals;
        m_OrinTangents = m_Mesh.sharedMesh.tangents;
        UpdateDepth(m_FractureDepth);
    }

    public void InitializeMeshData()
    {
        m_Mesh = this.GetComponent<MeshFilter>();
        m_OrinVertices = m_Mesh.sharedMesh.vertices;
        m_OrinNormals = m_Mesh.sharedMesh.normals;
        m_OrinTangents = m_Mesh.sharedMesh.tangents;
    }

    public void SetProjectDirection(Vector3 _dir)
    {
        m_ProjectDir = _dir;
    }

    public void Project(GameObject _target)
    {
        Vector3[] vertices = m_OrinVertices;
        Vector3[] normals = m_OrinNormals;
        Vector4[] tangents = m_OrinTangents;

        ProjectMesh(_target, m_Mesh.mesh, vertices, normals, tangents, GetProjectDir);
    }
    
    public void ProjectMesh(GameObject _target, Mesh _mesh, Vector3[] _vert, Vector3[] _nor, Vector4[] _tan, Vector3 _projectDir)
    {
        if (_target == null)
        {
            Debug.LogError("[CFractureDecal] No target info");
        }

        Vector3[] vertices = _vert;
        Vector3[] normals = _nor;
        Vector4[] tangents = _tan;
        
        Collider targetColl = _target.GetComponent<Collider>();

        for(int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            Vector3 worldPos = this.transform.position + (Vector3)(this.transform.localToWorldMatrix * vertex);
            Vector3 projectileDirection = (this.transform.position + _projectDir * 0.1f) - worldPos;
            Ray ray = new Ray(worldPos, projectileDirection);
            RaycastHit hit;
            //If ray hit the fracturable object
            if (targetColl.Raycast(ray, out hit, 0.1f))
            {
                //Project vertex position on fracturable object's mesh
                vertices[i] = this.transform.worldToLocalMatrix * (hit.point - projectileDirection * 0.001f - this.transform.position);
            }
            else
            {
                //No idea
            }
        }

        _mesh.vertices = vertices;
        _mesh.normals = normals;
        _mesh.tangents = tangents;
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();
    }

    public void Restore()
    {
        Vector3[] vertices = m_OrinVertices;
        Vector3[] normals = m_OrinNormals;
        Vector4[] tangents = m_OrinTangents;

        RestoreMesh(m_Mesh.mesh, vertices, normals, tangents);
    }

    public void RestoreMesh(Mesh _mesh, Vector3[] _vert, Vector3[] _nor, Vector4[] _tan)
    {
        _mesh.vertices = _vert;
        _mesh.normals = _nor;
        _mesh.tangents = _tan;
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();
    }

    public void UpdateDepth(float _depth)
    {
        this.GetComponent<MeshRenderer>().material.SetFloat("_DepthStrength", _depth);
        m_FractureDepth = _depth;
        if (_depth <= 0)
        {
            this.GetComponent<SphereCollider>().enabled = false;
            StartCoroutine(RemoveDecal());
        }
    }
    
    IEnumerator RemoveDecal()
    {
        Material mat = this.GetComponent<MeshRenderer>().material;
        float fade = 1f;
        while (fade > 0f)
        {
            fade -= Time.deltaTime * 0.5f;
            fade = fade < 0 ? 0f : fade;
            mat.SetFloat("_Fade", fade);
            yield return null;
        }

        GameObject particle = Instantiate(m_CureEffect, this.transform.position, Quaternion.identity);

        m_Subordinated.m_Fractures.Remove(this);

        Destroy(this.gameObject);
    }
}
