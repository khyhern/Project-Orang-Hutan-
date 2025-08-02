using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    public void TeleportTo(Transform targetPosition)
    {
        if (targetPosition == null)
        {
            Debug.LogWarning("[Teleport] Target position is null.");
            return;
        }

        transform.position = targetPosition.position;
        Debug.Log("[Teleport] Player moved to: " + targetPosition.position);
    }

    public void TeleportTo(Vector3 worldPosition)
    {
        transform.position = worldPosition;
        Debug.Log("[Teleport] Player moved to: " + worldPosition);
    }
}
