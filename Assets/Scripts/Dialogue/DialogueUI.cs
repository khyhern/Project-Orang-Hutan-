using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textLabel;
	[SerializeField] private DialogueObject testDialogue;
	
	private TypeWriterEffect typewriterEffect;

    private void Start()
    {
        typewriterEffect = GetComponent<TypeWriterEffect>();
		ShowDialogue(testDialogue);
    }
	
	public void ShowDialogue(DialogueObject dialogueObject)
	{
		StartCoroutine(StepThroughDialogue(dialogueObject));
	}
	
	private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
	{
		yield return new WaitForSeconds(3);
		
		foreach (string dialogue in dialogueObject.Dialogue)
		{
			yield return typewriterEffect.Run(dialogue, textLabel);
			yield return new WaitUntil(()=> Input.GetKeyDown(KeyCode.Space));
		}
	}
}