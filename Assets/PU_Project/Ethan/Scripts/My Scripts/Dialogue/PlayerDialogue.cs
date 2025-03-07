using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDialogue : MonoBehaviour
{
    [SerializeField]
    DialogueManager dialogueManager;

    private UnityEvent onSpacePressed = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        onSpacePressed.AddListener(DisplaySentence);
    }

    // Update is called once per frame
    void Update()
    {
        //If the player presses the space key, the dialogue will start or continue if the dialogue is already started
    
        if(Input.GetKeyUp(KeyCode.V))
        {
           Debug.Log("Space key was pressed");
            onSpacePressed?.Invoke();
            
        }

    }

    void DisplaySentence() {
        Debug.Log("Displaying sentence");
        dialogueManager.DisplayNextSentence();
    }
}

