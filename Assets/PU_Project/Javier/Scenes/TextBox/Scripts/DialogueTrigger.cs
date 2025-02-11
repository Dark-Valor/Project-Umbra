using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public DialogueManager dialogueManager;
    public Button startButton;


    public void TriggerDialouge()
    {
        startButton.interactable = false;
        dialogueManager.StartDialogue(dialogue);
    }
}
