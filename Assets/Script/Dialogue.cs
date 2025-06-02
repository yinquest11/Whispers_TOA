using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    //Fields
    //Window
    public GameObject window;
    //Indicator
    public GameObject indicator;
    //Text component
    public TMP_Text dialogueText;
    //Diaglogues list
    public List<string> dialogues;
    //writing speed
    public float writingSpeed;
    //Index on dialogue
    private int index;
    //Character index
    private int charIndex;
    //Started indicator
    private bool started;
    //Wait for boolean
    private bool waitForNext;

    private void Awake()
    {
        ToggleIndicator(false);
        ToggleWindow(false);
    }


    private void ToggleWindow(bool show)
    {
        window.SetActive(show);
    }

    public void ToggleIndicator(bool show)
    {
        indicator.SetActive(show);
    }
    //Start Dialogue
    public void StartDialogue()
    { 
        if(started)
        {
            return;
        }
        
        //boolean to indicate that we have started
        started = true;
        //Show the window
        ToggleWindow(true);
        //hide the indicator
        ToggleIndicator(true);
        //Start with the first dialogue
        GetDialogue(0);
 
    }
    private void GetDialogue(int i)
    {
        //start index at zero
        index = i;
        //Reset the charater index
        charIndex = 0;
        //clear the dialogue component text
        dialogueText.text = string.Empty;
        //Start Writing
        StartCoroutine(Writing());

    }
    //End Dialogue

    public void EndDialogue()
    {
        //Started is disabled
        started = false;    
        //disable wait for next
        waitForNext = false;
        //stop all Ienumerators
        StopAllCoroutines(); 
        ToggleWindow(false);
    }
    //Writing logic
    IEnumerator Writing()
    {
        yield return new WaitForSeconds(writingSpeed);

        string currentDialogue = dialogues[index];
        //Write the letter
        dialogueText.text += currentDialogue[charIndex];

        //increase the character index
        charIndex++;

        //Make Sure you have reached the end of the sentence
        if (charIndex<currentDialogue.Length)
        {
            //Wait x Seconds
            yield return new WaitForSeconds(writingSpeed);
            //Restart the same process
            StartCoroutine(Writing());

        }
        else
        {
            waitForNext = true;
        }
       

    }
    private void Update()
    {

        if (!started)
        {
            return;
        }

        if(waitForNext && Input.GetKeyDown(KeyCode.E))
        {
            waitForNext = false;
            index++;

            //check if we are in the scope of dialogue lisy
            if (index < dialogues.Count)
            {
                //if so fetch the next dialogue
                GetDialogue(index);
            }
            else
            {
                //if not end the dialogue process
                ToggleIndicator(true);
                EndDialogue();
            }


        }
    }
}
