using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleEventResponse : MonoBehaviour
{
    private SpriteRenderer sprite;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void ChangeColor()
    {
        Debug.Log("CHANGING COLOR");
        if (sprite.color != Color.red) { sprite.color = Color.red; }
        else if (sprite.color != Color.green) { sprite.color = Color.green; }
    }
}
