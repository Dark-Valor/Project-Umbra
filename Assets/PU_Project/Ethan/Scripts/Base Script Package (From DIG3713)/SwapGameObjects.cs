using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapGameObjects : MonoBehaviour
{
    public GameObject objectToEnable, objectToDisable;
    [Tooltip("Name of the tag on the gameobject that triggers the enabling.")]
    public string collisionTag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(collisionTag))
        {
            objectToEnable.SetActive(true);
            objectToDisable.SetActive(false);
        }
    }
}
