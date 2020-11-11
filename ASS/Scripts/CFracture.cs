using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Need : Fracture mesh (high polygon)
 *        Height Map
 *        Fracture texture
 * Description : 1. Create an fracture object with fracture mesh
 *               2. Plane projection to bone and match vertices to the mesh (like match road mesh to the terrain mesh
 *               3. Rendering height map
 *               4. Fracture object must be render first than bone
 *               5. Subject alpha of height map while curing
 *               6. Destroy object after curing done
 *               
 * 
 */


public class CFracture
{
    //Singletone
    public static CFracture instance;
    CFracture()
    {
        if (instance != null) return;
        instance = this;
    }

    public CFracture GetInstance
    {
        get
        {
            return instance ?? new CFracture();
        }
    }

    private Vector3 m_TargetVertex;

    //Create fracture at random position of mesh surface
    //골절 효과를 주어진 오브젝트의 메쉬 표면 중 랜덤한 위치에 생성한다
    void CreateFractureAtRandomPosition(GameObject _obj, float _radius, float _near, float _far)
    {
        //Get object's mesh
        Mesh mesh = _obj.GetComponent<MeshFilter>().mesh;

        //Get vertices and choose random vertex
        Vector3[] vertices = mesh.vertices;
        int targetIdx = Random.Range(0, vertices.Length);
        Vector3 targetVertect = vertices[targetIdx];
        
        //Get plane with target vertex's normal and tangent vector
        Vector3 v = mesh.tangents[targetIdx].normalized;
        Vector3 w = -mesh.normals[targetIdx].normalized;
        Vector3 u = Vector3.Cross(v, w).normalized;
        
        //Projectile vertices to plane (ignore if normal dot is below 0)
        //Create texture with projectiling result
        //Set new uv with projectiling result (vertices' of out of range uv value is 0)

        //Create new fracture material and add to mesh renderer
    }
}
