using UnityEngine;

public class BleedHandler : MonoBehaviour
{
    [Tooltip("Base interval (in seconds) before taking 1 damage from a single bleeding limb.")]
    public float bleedInterval = 2f;

    private float bleedTimer;

    private void Update()
    {
        PlayerHealth player = PlayerHealth.Instance;
        if (player == null || player.IsDead()) return;

        int bleedingLimbCount = player.CountActiveBleedingLimbs();
        if (bleedingLimbCount == 0) return;

        bleedTimer += Time.deltaTime;
        float intervalPerTick = bleedInterval / bleedingLimbCount;

        if (bleedTimer >= intervalPerTick)
        {
            player.DamagePlayer(1);
            Debug.Log($"[Bleed] Bleeding from {bleedingLimbCount} limbs. -1 HP");
            bleedTimer = 0f;
        }
    }
}
