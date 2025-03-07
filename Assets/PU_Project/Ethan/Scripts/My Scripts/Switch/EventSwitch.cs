using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSwitch : MonoBehaviour
{
    public string playerTag = "Player";

    public UnityEvent sampleEvent;

    private bool playerDetected = false;

    private void Update()
    {
        if (playerDetected && Input.GetKeyDown(KeyCode.C))
        {
            sampleEvent?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("PLAYER DETECTED");
            playerDetected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("PLAYER NOT DETECTED");
            playerDetected = false;
        }
    }
}
