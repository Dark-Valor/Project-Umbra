using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(Light2D))]
public class PointLightView : MonoBehaviour
{
    MeshFilter viewMeshFilter;
    Mesh viewMesh;
    Light2D light2D;



    // Start is called before the first frame update
    void Awake()
    {
        viewMesh = new Mesh();
        viewMesh.name = "GenMesh";
        viewMeshFilter = GetComponent<MeshFilter>();
        viewMeshFilter.mesh = viewMesh;
        light2D = GetComponent<Light2D>();
    }


    public void UpdateView(PointLightModel pointLightModel)
    {
        // Light 2D Assignments
        light2D.pointLightInnerRadius = (light2D.pointLightOuterRadius=pointLightModel.ViewRadius)/2;
        light2D.pointLightInnerAngle = light2D.pointLightOuterAngle = pointLightModel.ViewAngle;


        /// Render Mesh Rendering Logic ///
        int vertexCount = pointLightModel.ViewPoints.Count+1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] tris = new int[(vertexCount-2)*3];

        vertices[0] = Vector3.zero;
        for(int i=0;i<vertexCount-1;i++)
        {
            vertices[i+1] = transform.InverseTransformPoint(pointLightModel.ViewPoints[i]); // ???

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

    // Finds the between two raycasts (fat with info)
    SL_EdgeInfo FindEdge(SL_ViewCastInfo minViewCast, SL_ViewCastInfo maxViewCast, PointLightModel pointLightModel)
    {
        float minAngle = minViewCast._angle;
        float maxAngle = maxViewCast._angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i=0;i<pointLightModel.EdgeRes;i++)
        {
            float angle = (minAngle+maxAngle)/2;
            SL_ViewCastInfo newViewCast = ViewCast(angle, pointLightModel.ViewRadius, pointLightModel.TargetMask);

            bool edgeDistThresholdExceeded = Mathf.Abs(minViewCast._dist-newViewCast._dist)>pointLightModel.EdgeDistThresh;
            if(newViewCast._hit==minViewCast._hit && !edgeDistThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast._point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast._point;
            }
        }

        return new SL_EdgeInfo(minPoint,maxPoint);
    }

    // Gets information related to the raycast (if <something> was hit, where the hit happened, how far is the hit, and at what angle)
    SL_ViewCastInfo ViewCast(float globalAngle, float viewRadius, int targetMask)
    {
        Vector3 dir = PU_Utilities.DirFromAngle(globalAngle); // Returns a direction given an angle, seems to be a unit vector as sin & cos have amplitude of 1
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position,dir, viewRadius,targetMask); // This is the raycast (!)

        if(hit2D.collider!=null)
            // Hit the target
            return new SL_ViewCastInfo(true,hit2D.point,hit2D.distance,globalAngle); // Return information about that raycast
        else
            // Did not hit the target
            return new SL_ViewCastInfo(false,transform.position+dir*viewRadius,viewRadius,globalAngle);
    }
}
