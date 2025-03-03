using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast2D_Test : MonoBehaviour
{
    [SerializeField] float radius = 1f;
    Collider2D prevHit2D = null;

    void Update()
    {
        Vector3 dir = PU_Utilities.DirFromAngle(-transform.eulerAngles.z);
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position,dir,radius);

        if(hit2D && hit2D.collider!=prevHit2D)
        {
            Debug.Log("Hit: " + hit2D.collider.name);
            Debug.Log("Centroid: " + hit2D.centroid);
            Debug.Log("Distance: " + hit2D.distance);
            Debug.Log("Fraction: " + hit2D.fraction);
            Debug.Log("Normal: " + hit2D.normal);
            Debug.Log("Point: " + hit2D.point);
            Debug.Log("Rigidbody: " + hit2D.rigidbody);
            Debug.Log("Transform: " + hit2D.transform);

            Debug.DrawLine(transform.position, hit2D.point, Color.red);
            prevHit2D = hit2D.collider;
        }
        else
            Debug.DrawLine(transform.position, transform.position+dir*radius, Color.red);

    }
}
