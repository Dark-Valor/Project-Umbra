using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText; //UI element that contains the name of the character speaking
    public TextMeshProUGUI dialogueText; //UI element that contains the dialouge
    public GameObject dialougeBox; //UI element that contains the dialouge
    public bool isDialogueActive = false; //Boolean that checks if the dialogue is active or not
    private Queue<string> sentences; //Queue that contains the sentences of the dialogue
    
    [SerializeField]
    public float typingSpeed = 0.01f; //Speed of the typing effect
    public int index; //Index of the current sentence
    
    public Action nextSentence;

    private Coroutine typing;
    public Button dialogueStartButton;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        //Debug.Log("Starting conversation with " + dialogue.name);

        index = 0;

        dialougeBox.SetActive(true);

        nameText.text = dialogue.name;

        sentences.Clear();

        isDialogueActive = true;

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }   

    public void DisplayNextSentence()
    {
        if (typing != null)
            StopCoroutine(typing);

        if (sentences.Count == 0)
        {
            dialogueStartButton.interactable = true;
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        typing = StartCoroutine(TypeSentence(sentence));
    }

    void EndDialogue()
    {
        dialougeBox.SetActive(false);
        isDialogueActive = false;
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
    
}
