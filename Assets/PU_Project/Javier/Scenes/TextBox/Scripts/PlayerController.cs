using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    DialogueManager dialogueManager;

    private UnityEvent<int> onSpacePressed = new UnityEvent<int>();

    // Start is called before the first frame update
    void Start()
    {
        onSpacePressed.AddListener(DisplaySentence);
    }

    // Update is called once per frame
    void Update()
    {
        //If the player presses the space key, the dialogue will start or continue if the dialogue is already started
    
        if(Input.GetKeyUp(KeyCode.Space))
        {
           Debug.Log("Space key was pressed");
            onSpacePressed.Invoke(0);

        //   if(FindObjectOfType<DialogueManager>().isDialogueActive)
        //    {
        //        FindObjectOfType<DialogueManager>().DisplayNextSentence();
        //    }
        //    if(dialogueManager.isDialogueActive)
        //    {
        //        dialogueManager.DisplayNextSentence();
        //    }
            
        }

    }

    void DisplaySentence(int i) {
        Debug.Log("Displaying sentence");
        dialogueManager.DisplayNextSentence();
    }
}

