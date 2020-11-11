using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CVertexPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        foreach (Vector3 v in mesh.vertices)
        {
            print(v);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
