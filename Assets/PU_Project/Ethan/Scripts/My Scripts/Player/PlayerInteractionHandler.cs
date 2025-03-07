using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    public GameObject exclamationMark;

    public string switchTag = "Switch";

    public string npcTag = "NPC";


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Detected: " + other.name);
        if (other.CompareTag(switchTag))
        {
            exclamationMark.SetActive(true);
        }
        else if (other.CompareTag(npcTag))
        {
            exclamationMark.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(switchTag))
        {
            exclamationMark.SetActive(false);
        }
        else if (other.CompareTag(npcTag))
        {
            exclamationMark.SetActive(false);
        }
    }
}
