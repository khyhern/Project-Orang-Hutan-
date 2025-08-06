using UnityEngine;

public class SitTeleportTester : MonoBehaviour
{
    [Header("Sit Target")]
    [SerializeField] private SitInteractable targetChair;

    [Header("Auto Trigger After Delay")]
    [SerializeField] private bool autoTrigger = true;
    [SerializeField] private float delaySeconds = 5f;

    private void Start()
    {
        if (autoTrigger)
            Invoke(nameof(TriggerTeleport), delaySeconds);
    }

    /// <summary>
    /// Teleports the player to the sit spot and makes them sit.
    /// </summary>
    public void TriggerTeleport()
    {
        if (targetChair == null)
        {
            Debug.LogWarning("[SitTeleportTester] No target chair assigned.");
            return;
        }

        SitTeleportService.TeleportAndSitAt(targetChair);
    }
}
