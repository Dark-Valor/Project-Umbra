using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
public class PointLightDemoView : MonoBehaviour
{
    MeshFilter viewMeshFilter;
    Mesh viewMesh;

    void Awake()
    {
        viewMesh = new Mesh();
        viewMesh.name = "GenMesh";
        viewMeshFilter = GetComponent<MeshFilter>();
        viewMeshFilter.mesh = viewMesh;
    }

    public void UpdateView(PointLightDemoModel pointLightDemoModel)
    {
        /// Render Mesh Rendering Logic ///
        int vertexCount = pointLightDemoModel.ViewPoints.Count+1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] tris = new int[(vertexCount-2)*3];

        vertices[0] = Vector3.zero;
        for(int i=0;i<vertexCount-1;i++)
        {
            vertices[i+1] = transform.InverseTransformPoint(pointLightDemoModel.ViewPoints[i]); // ???

            if(i<vertexCount-2)
            {
                tris[i*3] = 0;
                tris[i*3+1] = i+1;
                tris[i*3+2] = i+2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = tris;
        viewMesh.RecalculateNormals();
    }
}
