using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourceParent : MonoBehaviour
{
    // [SerializeField] PointLightSource PLS;
    PointLightSource _PLS;
    [SerializeField] Transform pfPLS;
    [SerializeField] float angle;


    void Start()
    {
        _PLS = Instantiate(pfPLS, null).GetComponent<PointLightSource>();
    }

    void Update()
    {
        _PLS.SetOrigin(transform.position);
        _PLS.StartingAngle = angle;
    }
}
