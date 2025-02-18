using UnityEngine;
using System.Collections;

public class Rotator_Planet : MonoBehaviour
{
    [SerializeField] private float speed = 1f;

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime * new Vector3(0, 0, -90));
    }
}