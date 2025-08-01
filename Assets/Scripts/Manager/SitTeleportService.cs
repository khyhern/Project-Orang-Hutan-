using UnityEngine;

public static class SitTeleportService
{
    /// <summary>
    /// Makes the player sit at the given SitInteractable.
    /// </summary>
    public static void TeleportAndSitAt(SitInteractable sitTarget)
    {
        if (sitTarget == null || sitTarget.sitSpot == null)
        {
            Debug.LogWarning("[SitTeleportService] Invalid sit target.");
            return;
        }

        var player = PlayerSittingController.Instance;
        if (player == null)
        {
            Debug.LogWarning("[SitTeleportService] PlayerSittingController not found.");
            return;
        }

        player.SitAt(sitTarget);
    }
}
