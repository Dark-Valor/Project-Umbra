using System.Collections;
using System.Collections.Generic;
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
    public static Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal = true, Transform transform = null)
    {
        if(!angleIsGlobal) angleInDegrees-= transform!=null ? transform.eulerAngles.z : 0;
        return new Vector3(Mathf.Sin(angleInDegrees*Mathf.Deg2Rad),Mathf.Cos(angleInDegrees*Mathf.Deg2Rad));
    }
}
