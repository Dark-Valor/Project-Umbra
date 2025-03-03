using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PU_Utilities
{
    // Code Monkey
    public static Vector3 GetVectorFromAngle(float angle)
    {
        // Angle := [0,360]
        float angleRad = angle*(Mathf.PI/180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y,dir.x)*Mathf.Rad2Deg;
        if(n<0) n+=360;

        return n;
    }

    // Sebastian Lague
    // public static Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal = true, Transform transform = null)
    public static Vector3 DirFromAngle(float angleInDegrees, Transform transform = null)
    {
        // if(!angleIsGlobal) angleInDegrees-= transform!=null ? transform.eulerAngles.z : 0;
        angleInDegrees-= transform!=null ? transform.eulerAngles.z : 0;
        return new Vector3(Mathf.Sin(angleInDegrees*Mathf.Deg2Rad),Mathf.Cos(angleInDegrees*Mathf.Deg2Rad));
    }

    // Finds the between two raycasts (fat with info)
    // Pending a change to EdgeInfo (!)
    public static SL_EdgeInfo FindEdge(SL_ViewCastInfo minViewCast, SL_ViewCastInfo maxViewCast, PointLightModel pointLightModel)
    {
        float minAngle = minViewCast._angle;
        float maxAngle = maxViewCast._angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i=0;i<pointLightModel.EdgeRes;i++)
        {
            float angle = (minAngle+maxAngle)/2;
            // SL_ViewCastInfo newViewCast = ViewCast(angle, pointLightModel.ViewRadius, pointLightModel.TargetMask);
            SL_ViewCastInfo newViewCast = ViewCast(angle, pointLightModel);

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
    // public static SL_ViewCastInfo ViewCast(float globalAngle, float viewRadius, int targetMask)
    public static SL_ViewCastInfo ViewCast(float globalAngle, PointLightModel pointLightModel)
    {
        Vector3 dir = DirFromAngle(globalAngle); // Returns a direction given an angle, seems to be a unit vector as sin & cos have amplitude of 1
        RaycastHit2D hit2D = Physics2D.Raycast(pointLightModel.LightPosition,dir, pointLightModel.ViewRadius,pointLightModel.TargetMask); // This is the raycast (!)

        if(hit2D.collider!=null)
            // Hit the target
            return new SL_ViewCastInfo(true,hit2D.point,hit2D.distance,globalAngle); // Return information about that raycast
        else
            // Did not hit the target
            return new SL_ViewCastInfo(false,pointLightModel.LightPosition+dir*pointLightModel.ViewRadius,pointLightModel.ViewRadius,globalAngle);
    }
}
