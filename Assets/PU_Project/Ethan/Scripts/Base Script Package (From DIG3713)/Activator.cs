using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Activator : MonoBehaviour
{
    public string tagName;
    public UnityEvent triggerEvent;
    public UnityEvent exitEvent;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag(tagName))
        {
            triggerEvent.Invoke();
        }
    }

    // Update is called once per frame
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(tagName))
        {
            exitEvent.Invoke();
        }
    }
}
