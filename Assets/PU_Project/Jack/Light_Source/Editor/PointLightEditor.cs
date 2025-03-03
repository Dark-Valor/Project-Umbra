using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(PointLightController))]
public class PointLightEditor : Editor
{
    void OnSceneGUI()
    {
        PointLightController pointLight = (PointLightController)target;
        PointLightModel pointLightModel = pointLight.PointLightModel;
        Handles.color = Color.white;
        Handles.DrawWireArc(pointLight.transform.position,Vector3.forward,Vector3.up,360,pointLightModel.ViewRadius);
        // Vector3 viewAngleA = PU_Utilities.DirFromAngle(-pointLightModel.ViewAngle/2,false,pointLight.transform);
        // Vector3 viewAngleB = PU_Utilities.DirFromAngle(pointLightModel.ViewAngle/2,false,pointLight.transform);
        Vector3 viewAngleA = PU_Utilities.DirFromAngle(-pointLightModel.ViewAngle/2,pointLight.transform);  // Angle relative to a given gameobject transform (local space)
        Vector3 viewAngleB = PU_Utilities.DirFromAngle(pointLightModel.ViewAngle/2,pointLight.transform);   // Angle relative to a given gameobject transform (local space)

        Handles.DrawLine(pointLight.transform.position,pointLight.transform.position+viewAngleA*pointLightModel.ViewRadius);
        Handles.DrawLine(pointLight.transform.position,pointLight.transform.position+viewAngleB*pointLightModel.ViewRadius);

        Handles.color = Color.red;
        // foreach(Transform visibleTarget in pointLight.VisibleTargets)
        //     Handles.DrawLine(pointLight.transform.position,visibleTarget.position);
        foreach(Vector3 visibleTarget in pointLightModel.VisibleTargets)
            Handles.DrawLine(pointLight.transform.position,visibleTarget);
    }
}
