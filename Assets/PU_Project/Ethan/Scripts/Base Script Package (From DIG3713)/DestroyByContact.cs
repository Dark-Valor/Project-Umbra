using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
    public string tagToIgnore;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToIgnore))
        {
            return;
        }
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}