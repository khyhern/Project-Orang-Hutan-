using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
	[SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text textLabel;
	[SerializeField] private DialogueObject testDialogue;
	[SerializeField] private bool autoPlay = true;
	
	private TypeWriterEffect typewriterEffect;

    private void Start()
    {
        typewriterEffect = GetComponent<TypeWriterEffect>();
		CloseDialogueBox();
		ShowDialogue(testDialogue);
    }
	
	public void ShowDialogue(DialogueObject dialogueObject)
	{
		dialogueBox.SetActive(true);
		StartCoroutine(StepThroughDialogue(dialogueObject));
	}
	
	private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
	{
		yield return new WaitForSeconds(0.5f);
		
		for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];

            // Trigger event at the start of the last sentence
            if (i == dialogueObject.Dialogue.Length - 1)
            {
                OnLastSentenceStart(); // Call your event logic here
            }

            yield return typewriterEffect.Run(dialogue, textLabel);

            if (autoPlay)
            {
                yield return new WaitForSeconds(1f); // Short delay before continuing
            }
            else
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            }
        }
		
		CloseDialogueBox();
	}
	
	private void CloseDialogueBox()
	{
		dialogueBox.SetActive(false);
		textLabel.text = string.Empty;
	}
	
	    private void OnLastSentenceStart()
    {
        // Example: spawn a GameObject or trigger some logic
        Debug.Log("Last sentence started!");
        // Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}