using UnityEngine;
using Unity.Cinemachine;

public class PlayerSittingController : MonoBehaviour
{
    public static PlayerSittingController Instance;

    private PlayerMovement _movement;
    private CharacterController _controller;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera virtualCamera;

    private CinemachinePanTilt panTilt;

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

        if (virtualCamera != null)
            panTilt = virtualCamera.GetComponent<CinemachinePanTilt>();
    }

    void Update()
    {
        if (InputBlocker.IsInputBlocked) return;

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

        // ✅ Snap camera to chair's Y rotation
        if (panTilt != null)
        {
            Vector3 forward = sitTarget.sitSpot.forward;
            float yAngle = Quaternion.LookRotation(forward).eulerAngles.y;
            panTilt.PanAxis.Value = yAngle;
        }

        _movement.canMove = false;
        isSitting = true;
        sitTime = Time.time;

        Debug.Log("[Player] Sat down. Camera aligned to SitSpot.");
    }

    public void StandUp()
    {
        if (!isSitting) return;

        transform.SetPositionAndRotation(originalPosition, originalRotation);

        _movement.canMove = true;
        isSitting = false;

        Debug.Log("[Player] Stood up.");
    }

    public bool IsSitting() => isSitting;
}
