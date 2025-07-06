using UnityEngine;

public class PlayerSittingController : MonoBehaviour
{
    public static PlayerSittingController Instance;

    private PlayerMovement _movement;
    private CharacterController _controller;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isSitting;

    private float sitTime = 0f;
    private float sitInputDelay = 0.2f;

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

        transform.SetPositionAndRotation(sitTarget.sitSpot.position, sitTarget.sitSpot.rotation);

        _movement.canMove = false; // ✅ lock movement
        isSitting = true;
        sitTime = Time.time;

        Debug.Log("[Player] Sat down. Movement disabled.");
    }

    public void StandUp()
    {
        if (!isSitting) return;

        transform.SetPositionAndRotation(originalPosition, originalRotation);

        _movement.canMove = true; // ✅ unlock movement
        isSitting = false;

        Debug.Log("[Player] Stood up. Movement re-enabled.");
    }

    public bool IsSitting() => isSitting;
}
