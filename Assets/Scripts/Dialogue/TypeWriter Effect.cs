using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeWriterEffect : MonoBehaviour
{
    // Start is called before the first frame update
    public void Run(String textToType, TMP_Text textLabel)
    {
        startCoroutine(TypeText(textToType, textLabel));
    }
    // Update is called once per frame
    private IEnumerator TypeText(String textToType, TMP_Text textLabel)
    {
        float t = 0;
		int charIndex = 0;
		
		while (charIndex < textToType.Length)
		{
			t += time.deltaTime;
			charIndex = MathfFloorRoInt(t);
			charIdex = Mathf.Clamp(charIndex, 0, textToType.Length)
			
			textLabel.text = textTotype.Substring(0, charIndex);
			
			yield return null;
		}
    }
	
	textLabel.text = textToType;
}
