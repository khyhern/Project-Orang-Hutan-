using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private Camera raycastCamera;

    private IInteractable currentTarget;

    private void Awake()
    {
        if (raycastCamera == null)
            raycastCamera = Camera.main;
    }

    private void Start()
    {
        // Auto-assign text box by name if not set manually
        if (messageText == null)
        {
            GameObject obj = GameObject.Find("InteractionDialogue");
            if (obj != null && obj.TryGetComponent(out TextMeshProUGUI tmp))
            {
                messageText = tmp;
            }
            else
            {
                Debug.LogWarning("[Drawer] 'InteractionDialogue' TextMeshProUGUI not found.");
            }
        }
    }

    private void Update()
    {
        CheckForInteractable();

        if (InputBlocker.IsInputBlocked) return;

        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact();
        }
    }

    private void CheckForInteractable()
    {
        if (raycastCamera == null) return;

        Ray ray = new Ray(raycastCamera.transform.position, raycastCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.green);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                if (!InteractionManager.IsGroupEnabled(interactable.GetInteractionGroup()))
                {
                    interactText.enabled = false;
                    currentTarget = null;
                    return;
                }

                // Check for 'showPrompt' if this is a Door
                if (interactable is Door doorKeyObj && !doorKeyObj.showPrompt)
                {
                    interactText.enabled = false;
                    currentTarget = null;
                    return;
                }


                currentTarget = interactable;

                if (interactText != null)
                {
                    if (interactable is IDescriptiveInteractable descriptive)
                    {
                        interactText.text = $"Press [{interactKey}] to {descriptive.GetInteractionVerb()} {descriptive.GetObjectName()}";
                    }
                    else if (interactable is MonoBehaviour mb && mb.CompareTag("Consumable"))
                    {
                        interactText.text = $"Press [{interactKey}] to pick up {mb.gameObject.name}";
                    }
                    else
                    {
                        interactText.text = $"Press [{interactKey}] to interact";
                    }

                    interactText.enabled = true;
                }

                return;
            }
        }

        currentTarget = null;
        if (interactText != null)
            interactText.enabled = false;
    }

    public void ShowMessage(string message, float duration)
    {
        StopAllCoroutines(); // Stop previous fades if any
        StartCoroutine(ShowMessageRoutine(message, duration));
    }

    private System.Collections.IEnumerator ShowMessageRoutine(string message, float duration)
    {
        messageText.text = message;
        messageText.alpha = 1f;

        yield return new WaitForSeconds(duration);

        float fadeTime = 1f;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            messageText.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }

        messageText.text = "";
    }
}
