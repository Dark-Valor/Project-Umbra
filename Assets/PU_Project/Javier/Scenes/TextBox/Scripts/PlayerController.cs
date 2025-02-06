using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //If the player presses the space key, the dialogue will start or continue if the dialogue is already started
    
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key was pressed");

            if(FindObjectOfType<DialogueManager>().isDialogueActive)
            {
                FindObjectOfType<DialogueManager>().DisplayNextSentence();
            }
            
        }

    }
}

