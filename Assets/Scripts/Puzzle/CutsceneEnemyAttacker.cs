using UnityEngine;

public class CutsceneEnemyAttacker : MonoBehaviour
{
    [Header("Cutscene Damage Settings")]
    [SerializeField] private int damageAmount = 10;

    public void DamagePlayer()
    {
        Debug.Log($"[CutsceneEnemyAttacker] Damaging player for {damageAmount} HP.");

        // Apply direct damage to the player — replace this with your actual method
        PlayerHealth.Instance?.DamagePlayer(damageAmount);
    }
}
