using UnityEngine;

public class PlayerSittingController : MonoBehaviour
{
    public static PlayerSittingController Instance;

    private PlayerMovement _movement;
    private CharacterController _controller;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isSitting;

    private float sitTime = 0f;                     // Timestamp of when player sat
    private float sitInputDelay = 0.2f;             // Minimum delay before pressing E is allowed

    void Awake()
    {
        Instance = this;
        _movement = GetComponent<PlayerMovement>();
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isSitting && Time.time - sitTime > sitInputDelay && Input.GetKeyDown(KeyCode.E))
        {
            StandUp();
        }
    }

    public void SitAt(SitInteractable sitTarget)
    {
        if (isSitting || sitTarget.sitSpot == null) return;

        originalPosition = transform.position;
        originalRotation = transform.rotation;

        _movement.enabled = false;
        _controller.enabled = false;

        transform.SetPositionAndRotation(sitTarget.sitSpot.position, sitTarget.sitSpot.rotation);

        _controller.enabled = true;
        isSitting = true;
        sitTime = Time.time; // record the sit time

        Debug.Log($"[Player] Sat down. Press [E] after {sitInputDelay}s to stand up.");
    }

    public void StandUp()
    {
        if (!isSitting) return;

        _controller.enabled = false;
        transform.SetPositionAndRotation(originalPosition, originalRotation);
        _controller.enabled = true;

        _movement.enabled = true;
        isSitting = false;

        Debug.Log("[Player] Stood up.");
    }

    public bool IsSitting() => isSitting;
}
