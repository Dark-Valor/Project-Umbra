using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
public class PointLightSource : MonoBehaviour
{
    Mesh _mesh;
    MeshFilter _meshFilter;

    [SerializeField]
    LayerMask _layerMask;
    [SerializeField]
    Vector3 _origin;
    [SerializeField]
    float _startingAngle;
    public float StartingAngle
    {
        get{return _startingAngle;}
        set{_startingAngle=value;}
    }
    float _fov;



    void Start()
    {
        _mesh = new Mesh();
        _mesh.name = "GenMesh";
        _meshFilter = GetComponent<MeshFilter>();
        _meshFilter.mesh = _mesh;
        _origin = Vector3.zero;
        _fov = 90f;
    }

    void LateUpdate()
    {
        // Vector3 origin = Vector3.zero;
        // Vector3 origin = transform.localPosition;
        int rayNum = 10;
        // float angle = 0f;
        float angle = _startingAngle;
        float angleIncr = _fov/rayNum;
        float viewDist = 1f;
        

        Vector3[] vertices = new Vector3[rayNum+2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] tris = new int[rayNum*3];

        vertices[0] = _origin;

        int vertexIdx = 1;
        int triIdx = 0;
        for(int i=0;i<=rayNum;i++)
        {
            // Vector3 vertex = origin + PU_Utilities.GetVectorFromAngle(angle)*viewDist;
            Vector3 vertex;
            // RaycastHit2D hit2D = Physics2D.Raycast(origin,PU_Utilities.GetVectorFromAngle(angle),viewDist);
            RaycastHit2D hit2D = Physics2D.Raycast(_origin,PU_Utilities.GetVectorFromAngle(angle),viewDist, _layerMask);
            
            if(hit2D.collider==null)
            {
                // No Hit
                vertex = _origin + PU_Utilities.GetVectorFromAngle(angle)*viewDist;
            }
            else
            {
                // Hit Object
                Debug.Log("hit2D name: " + hit2D.collider.name);
                vertex = hit2D.point;
            }
            

            vertices[vertexIdx] = vertex;
            if(i>0)
            {
                tris[triIdx+0] = 0;             // Origin
                tris[triIdx+1] = vertexIdx-1;   // Previous
                tris[triIdx+2] = vertexIdx;     // Current

                triIdx += 3;
            }


            vertexIdx++;
            angle -= angleIncr; // CCW Rotation
        }


        // vertices[0] = Vector3.zero;
        // vertices[1] = new Vector3(1,0);
        // vertices[2] = new  Vector3(0,-1);

        // tris[0] = 0;
        // tris[1] = 1;
        // tris[2] = 2;

        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = tris;
    }

    /// BEHAVIOUR(S) ///
    
    public void SetOrigin(Vector3 origin)
        => _origin = origin;

    public void SetDirection(Vector3 dir)
        => _startingAngle = PU_Utilities.GetAngleFromVectorFloat(dir)-_fov/2f;
}