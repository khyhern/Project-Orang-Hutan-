using UnityEngine;
using TMPro;
using System.Collections;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private DialogueObject testDialogue;
    [SerializeField] private bool autoPlay = true;
	
    private TypeWriterEffect typewriterEffect;
    private Coroutine dialogueCoroutine;
    private bool playerInside = false;

    private void Start()
    {
        typewriterEffect = GetComponent<TypeWriterEffect>();
        if (textLabel != null)
            textLabel.enabled = false;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (textLabel != null)
                textLabel.enabled = true;
            if (testDialogue != null && typewriterEffect != null)
            {
                dialogueCoroutine = StartCoroutine(StepThroughDialogue(testDialogue));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (dialogueCoroutine != null)
                StopCoroutine(dialogueCoroutine);
            if (textLabel != null)
            {
                textLabel.text = string.Empty;
                textLabel.enabled = false;
            }
        }
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        yield return new WaitForSeconds(0.2f);
        while (playerInside)
        {
            for (int i = 0; i < dialogueObject.Dialogue.Length && playerInside; i++)
            {
                string dialogue = dialogueObject.Dialogue[i];
                yield return typewriterEffect.Run(dialogue, textLabel);
                if (!playerInside) break;
                if (autoPlay)
                {
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || !playerInside);
                }
            }
        }
        if (textLabel != null)
            textLabel.text = string.Empty;
    }

    private void LateUpdate()
    {
        if (textLabel != null && textLabel.enabled && Camera.main != null)
        {
            // Make the text face the camera
            textLabel.transform.rotation = Quaternion.LookRotation(textLabel.transform.position - Camera.main.transform.position);
        }
    }
}