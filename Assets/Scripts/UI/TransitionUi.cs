using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionUi : MonoBehaviour
{

    public GameObject animatorLO;
    public GameObject transitionObject; // Assign your Canvas/Image GameObject in the inspector

    void Awake()
    {
        if (transitionObject != null)
        {
            Animator animator = animatorLO.GetComponent<Animator>();
            animator.SetTrigger("Active");
            StartCoroutine(HideAfterDelay(1f));
        }
        else
        {
            Debug.LogWarning("TransitionObject not assigned!");
        }
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        transitionObject.SetActive(false);
    }
}
