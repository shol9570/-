using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class CSwellable : MonoBehaviour
{
    private Vector3[] m_OrinVertices;
    private Vector3[] m_OrinNormals;
    private Vector4[] m_OrinTangents;
    private Mesh m_Mesh;
    private MeshFilter m_MeshFilter;

    private bool m_IsProBuilderObject = false;
    private CSwellArea m_SwellArea;

    public CSwellArea SWELLAREA
    {
        get
        {
            return m_SwellArea;
        }
    }
    
    void Start()
    {
        m_MeshFilter = this.GetComponent<MeshFilter>();
        m_Mesh = m_MeshFilter.mesh;
        m_OrinVertices = m_Mesh.vertices;
        m_OrinNormals = m_Mesh.normals;
        m_OrinTangents = m_Mesh.tangents;
        if (!this.transform.CompareTag("Swellon")) Debug.LogWarning("You need to set this object's tag to Swellon");
        if (this.GetComponent<ProBuilderMesh>() != null) m_IsProBuilderObject = true;
    }

    //When swell area object and target object overlap, start swell object
    //swell area 오브젝트와 타겟 오브젝트(this)가 겹칠때, 오브젝트 붓기 기능을 실행
    private void OnTriggerStay(Collider other)
    {
        //print("Swellable Hit");

        //Check triggered object is tagged with name that "SwellArea"
        //만약 트리거된 오브젝트가 SwellArea라는 이름으로 태그가 되어 있다면
        if (other.transform.CompareTag("SwellArea"))
        {
            //Swelling this object
            //오브젝트 붓기 기능을 실행
            SwellObject(other.gameObject);
        }
    }
    
    void SwellObject(GameObject _swellArea)
    {
        m_SwellArea = _swellArea.GetComponent<CSwellArea>();
        
        //Get swell radius from swell area object's CSwellArea component
        //Get swell offset value from swell area object's CSwellArea component (offset value is using in defining swelling degree of interpolation)
        //And get center position of swell area object and convert to this object's local position
        //붓기 범위를 swell area 오브젝트의 CSwellArea 컴포넌트에서 가져온다
        //swell area 오브젝트의 CSwellArea 컴포넌트에서 offset 값을 가져온다 (offset은 붓기 보간 정도를 정의하는데 쓰인다)
        //swell area 오브젝트의 중심점 위치를 가져온다
        float radius = _swellArea.GetComponent<CSwellArea>().m_Radius;
        float offset = _swellArea.GetComponent<CSwellArea>().m_Offset;
        Vector3 center = _swellArea.transform.position;
        
        //Adjust object vertices' position according to the setting
        //오브젝트 정점들의 위치를 설정에 맞춰 조정한다
        Vector3[] vertices = m_OrinVertices.Clone() as Vector3[];
        Vector3[] normals = m_OrinNormals.Clone() as Vector3[];
        Vector4[] tangents = m_OrinTangents.Clone() as Vector4[];
        for (int i = 0; i < vertices.Length; i++)
        {
            //Get vertex's local position
            //Get direction of from center of swell area to local position of vertex
            //Get magnitude of direction
            //Normalize direction vector
            //정점의 월드 좌표를 가져온다
            //붓는 중심점으로부터 정점의 월드 좌표까지의 방향 벡터를 구함
            //방향 벡터의 길이로부터 거리 도출
            //방향 벡터를 단위화
            Vector3 vertexPos = this.transform.TransformPoint(vertices[i]);
            Vector3 dirFromCenter = vertexPos - center;
            float dist = dirFromCenter.magnitude;
            dirFromCenter.Normalize();
            
            //Swell object if distance from center is below than raidus length or ignore
            //만약 정점이 구역 안에 있다면 위치를 조정하고 그렇지 않다면 무시한다
            if (dist < radius)
            {
                //Rule of adjusting vertex position is as follow
                //1. Compare dot product result between vertex normal and direction from center to vertex
                //2. Calculate a target vector from vertex to area surface in the direction if dot product result is minus or in the reflected direction by vertex normal
                //3. Linear interpolate vertex position to target vector according to given offsets
                //4. Store adjusted vertex position
                //정점 위치 조정은 아래와 같은 방법으로 이루어진다
                //1. 정점 노멀벡터와 붓는 중심점에서 정점 방향 벡터의 내적을 구해 비교한다
                //2. 내적 값이 음수면 중심으로부터의 방향으로, 그렇지 않다면 반사각 방향으로 조정 목표가 되는 target 위치를 구한다
                //3. 미리 받아온 offset값을 이용해 원래 정점 위치로부터 target 위치까지 보간한다
                //4. 보간된 정점의 위치를 로컬 좌표로 변환하여 저장한다
                Vector3 worldNormal = this.transform.TransformVector(normals[i]);
                float dotResult = Vector3.Dot(dirFromCenter, worldNormal);
                Vector3 target;
                Vector3 adjustedPos;
                if (dotResult > 0f)
                {
                    target = dirFromCenter * (radius - dist) + vertexPos;
                    adjustedPos = Vector3.Lerp(vertexPos, target, offset);
                    vertices[i] = this.transform.InverseTransformPoint(adjustedPos);
                }
                else
                {
                    target = Vector3.Reflect(dirFromCenter, worldNormal) * (radius - dist) + vertexPos;
                    adjustedPos = Vector3.Lerp(vertexPos, target, offset);
                    vertices[i] = this.transform.InverseTransformPoint(adjustedPos);
                }
            }
        }
        
        //Apply mesh datas after all of the vertex data has adjusted
        //모든 정점 데이터가 수정되었다면 메쉬 데이터를 확정한다
        m_Mesh.vertices = vertices;
        m_Mesh.normals = normals;
        m_Mesh.tangents = tangents;
        m_MeshFilter.mesh = m_Mesh;
    }

    //Reset object mesh if trigger exit
    //트리거 범위를 벗어나면 메쉬 모양을 다시 원상복구한다
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("SwellArea"))
        {
            m_Mesh.vertices = m_OrinVertices;
            m_Mesh.normals = m_OrinNormals;
            m_Mesh.tangents = m_OrinTangents;
            m_MeshFilter.mesh = m_Mesh;
            m_SwellArea = null;
        }
    }
}
