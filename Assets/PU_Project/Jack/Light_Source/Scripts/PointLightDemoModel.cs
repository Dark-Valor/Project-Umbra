using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointLightDemoModel
{
    Vector3 lightPosition;
    public Vector3 LightPosition
    {
        get{return lightPosition;}
        set{lightPosition = value;}
    }
    [SerializeField] [Range (1,50)] float viewRadius;
    public float ViewRadius
    {
        get{return viewRadius;}
        set{viewRadius = value;}
    }
    [SerializeField] [Range (2,360)] float viewAngle;
    public float ViewAngle
    {
        get{return viewAngle;}
        set{viewAngle=value;}
    }
    [SerializeField] float meshResolution;
    public float MesRes
    {
        get{return meshResolution;}
        set{meshResolution=value;}
    }
    [SerializeField] int edgeResolution;
    public int EdgeRes
    {
        get{return edgeResolution;}
        set{edgeResolution=value;}
    }
    [SerializeField] float edgeDistThreshold;
    public float EdgeDistThresh
    {
        get{return edgeDistThreshold;}
        set{edgeDistThreshold=value;}
    }
    [SerializeField] LayerMask targetMask;
    public LayerMask TargetMask
    {
        get{return targetMask;}
        set{targetMask=value;}
    }
    [SerializeField] LayerMask obstacleMask;
    public LayerMask ObstacleMask
    {
        get{return obstacleMask;}
        set{obstacleMask=value;}
    }
    MeshFilter viewMeshFilter;
    Mesh viewMesh;
    List<Vector3> viewPoints = new List<Vector3>();                                     // A list of all the points that our raycast hits; for mesh construction
    public List<Vector3> ViewPoints
    {
        get{return viewPoints;}
        set{viewPoints=value;}
    }
    [SerializeField] [Range (0,1)] float colorOpacity=1f;
    public float ColorOpacity
    {
        get{return colorOpacity;}
        set{colorOpacity=value;}
    }
    // List<Transform> visibleTargets = new List<Transform>();
    List<Vector3> visibleTargets = new List<Vector3>();
    public List<Vector3> VisibleTargets
    {
        get{return visibleTargets;}
        set{visibleTargets=value;}
    }
    [SerializeField] float delay=.2f;
    public float Delay
    {
        get{return delay;}
        set{delay=value;}
    }
    List<GameObject> Illuminated;

    // There is no need for this because all attributes are exposed to the Editor; Wonder if this should be a scriptable object
    // Scriptable object would be too global, I think. This is better as a runtime local attributes that can be adjusted on a per Object bases
    // public void UpdateModel()
    // {
    // }
}
