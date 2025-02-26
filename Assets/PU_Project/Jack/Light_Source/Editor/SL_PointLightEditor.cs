using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(SL_PointLight))]
public class SL_PointLightEditor : Editor
{
    void OnSceneGUI()
    {
        SL_PointLight pointLight = (SL_PointLight)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(pointLight.transform.position,Vector3.forward,Vector3.up,360,pointLight.ViewRadius);
        Vector3 viewAngleA = PU_Utilities.DirFromAngle(-pointLight.ViewAngle/2,false,pointLight.transform);
        Vector3 viewAngleB = PU_Utilities.DirFromAngle(pointLight.ViewAngle/2,false,pointLight.transform);

        Handles.DrawLine(pointLight.transform.position,pointLight.transform.position+viewAngleA*pointLight.ViewRadius);
        Handles.DrawLine(pointLight.transform.position,pointLight.transform.position+viewAngleB*pointLight.ViewRadius);

        Handles.color = Color.red;
        // foreach(Transform visibleTarget in pointLight.VisibleTargets)
        //     Handles.DrawLine(pointLight.transform.position,visibleTarget.position);
        foreach(Vector3 visibleTarget in pointLight.VisibleTargets)
            Handles.DrawLine(pointLight.transform.position,visibleTarget);
    }
}
