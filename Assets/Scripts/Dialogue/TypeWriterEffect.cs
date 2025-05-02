using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeWriterEffect : MonoBehaviour
{
	[SerializeField]private float typewriterSpeed = 50f;
	
	// Start is called before the first frame update
    public Coroutine Run(string textToType, TMP_Text textLabel)
    {
        return StartCoroutine(TypeText(textToType, textLabel));
    }
    // Update is called once per frame
    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
		textLabel.text = string.Empty;
		
        float t = 0;
        int charIndex = 0;
        
        while (charIndex < textToType.Length)
        {
            t += Time.deltaTime * typewriterSpeed;
            charIndex = Mathf. FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);
            
            textLabel.text = textToType.Substring(0, charIndex);
            
            yield return null;
        }
        textLabel.text = textToType;
    }
}
