using UnityEngine;

public struct SL_ViewCastInfo
{
    public bool _hit;
    public Vector3 _point;
    public float _dist;
    public float _angle;

    public SL_ViewCastInfo(bool hit, Vector3 point, float dist, float angle)
    {
        _hit = hit;
        _point = point;
        _dist = dist;
        _angle = angle;
    }
}