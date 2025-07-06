using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
 
public class PointerController : MonoBehaviour
{
    [Header("QTE Settings")]
    public Transform pointA; 
    public Transform pointB; 
    public RectTransform safeZone; 
    public float moveSpeed = 100f; 
    
    [Header("Visual Feedback")]
    public GameObject failureImage; 
    
    [Header("Safe Zone Management")]
    public GameObject SafeZone; 
    public GameObject FakeSafeZone; 
    
    [Header("Extra Behaviors")]
    public GameObject fakeSafepoints; // First behavior: activate fake safepoints
    public Animator safepointAnimator1; // Second behavior: play safepoint animation 1
    public Animator safepointAnimator2; // Third behavior: play safepoint animation 2
    public string animation1Name = "SafepointAnimation1"; // Animation trigger name for animator 1
    public string animation2Name = "SafepointAnimation2"; // Animation trigger name for animator 2
    
    [Header("Success Actions")]
    public GameObject qteCanvas; // Reference to the QTE canvas to close
    public QTETrigger qteTrigger; // Reference to the QTE trigger (door)
    
    private float direction = 1f; 
    private RectTransform pointerTransform;
    private Vector3 targetPosition;
    private Color originalFailureColor;
    private bool isQTEComplete = false;
    private AudioSource audioSource;
    private Transform mainCamera;
    private bool behaviorsActivated = false;
 
    void Start()
    {
        pointerTransform = GetComponent<RectTransform>();
        targetPosition = pointB.position;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) { audioSource = gameObject.AddComponent<AudioSource>(); }
        
        if (Camera.main != null) mainCamera = Camera.main.transform;
        
        if (failureImage != null)
        {
            Image failureImageComponent = failureImage.GetComponent<Image>();
            if (failureImageComponent != null)
            {
                originalFailureColor = failureImageComponent.color;
            }
        }
        
        
        if (SafeZone != null)
        {
            SafeZone.SetActive(true);
            safeZone = SafeZone.GetComponent<RectTransform>();
        }
        
        // Activate extra behaviors when QTE starts
        ActivateExtraBehaviors();
    }
    
    void ActivateExtraBehaviors()
    {
        if (behaviorsActivated) return;
        
        Debug.Log("Activating random extra behavior for QTE!");
        
        // Randomly select one of the three behaviors
        int randomBehavior = Random.Range(0, 3); // 0, 1, or 2
        
        switch (randomBehavior)
        {
            case 0:
                // First behavior: Activate fake safepoints
                if (fakeSafepoints != null)
                {
                    fakeSafepoints.SetActive(true);
                    Debug.Log("Random behavior selected: Fake safepoints activated!");
                }
                break;
                
            case 1:
                // Second behavior: Play safepoint animation 1
                if (safepointAnimator1 != null)
                {
                    safepointAnimator1.SetTrigger(animation1Name);
                    Debug.Log("Random behavior selected: Safepoint animation 1 triggered!");
                }
                break;
                
            case 2:
                // Third behavior: Play safepoint animation 2
                if (safepointAnimator2 != null)
                {
                    safepointAnimator2.SetTrigger(animation2Name);
                    Debug.Log("Random behavior selected: Safepoint animation 2 triggered!");
                }
                break;
        }
        
        behaviorsActivated = true;
    }
 
    void Update()
    {
        if (isQTEComplete) return;
        
        pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
        {
            targetPosition = pointB.position;
            direction = 1f;
        }
        else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
        {
            targetPosition = pointA.position;
            direction = -1f;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckSuccess();
        }
    }
 
    void CheckSuccess()
    {
        if (isQTEComplete) return;
        
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null))
        {
            OnSuccess();
        }
        else
        {
            OnFailure();
        }
    }
    
    void OnSuccess()
    {
        Debug.Log("Success!");
        
        // Close the QTE canvas
        if (qteCanvas != null)
        {
            qteCanvas.SetActive(false);
            Debug.Log("QTE Canvas closed!");
        }
        
        // Trigger success in QTETrigger (door will handle its own animation)
        if (qteTrigger != null)
        {
            qteTrigger.OnQTESuccess();
        }
        
        // Mark QTE as complete
        isQTEComplete = true;
    }
    
    void OnFailure()
    {
        Debug.Log("Fail!");
        
        
        if (failureImage != null)
        {
            StartCoroutine(FlashFailureImage());
        }
    }
    
    IEnumerator FlashFailureImage()
    {
        Image failureImageComponent = failureImage.GetComponent<Image>();
        if (failureImageComponent != null)
        {
            failureImageComponent.color = Color.red;
            
            yield return new WaitForSeconds(0.3f);
            
            failureImageComponent.color = originalFailureColor;
        }
    }
    
}