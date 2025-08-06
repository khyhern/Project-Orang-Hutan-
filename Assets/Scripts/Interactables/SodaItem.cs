using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SodaItem : MonoBehaviour, IDescriptiveInteractable
{
    [SerializeField] private string itemID = "StaminaPack";
    [SerializeField] private float restoreAmount = 20f;

    public void Interact()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();

        if (player != null)
        {
            // Use reflection to access private _stamina field
            var staminaField = typeof(PlayerMovement).GetField("_stamina", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var maxStaminaField = typeof(PlayerMovement).GetField("_maxStamina", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (staminaField != null && maxStaminaField != null)
            {
                float current = (float)staminaField.GetValue(player);
                float max = (float)maxStaminaField.GetValue(player);

                current += restoreAmount;
                current = Mathf.Clamp(current, 0f, max);

                staminaField.SetValue(player, current);
                UIManager.Instance.UpdateStamina(current, max);

                Debug.Log($"[StaminaItem] Restored {restoreAmount} stamina.");
            }
            else
            {
                Debug.LogWarning("[StaminaItem] Couldn't find stamina fields.");
            }
        }
        else
        {
            Debug.LogWarning("[StaminaItem] No PlayerMovement found in scene.");
        }

        Destroy(gameObject); // Remove stamina item from scene
    }

    public string GetInteractionVerb() => "use";
    public string GetObjectID() => itemID;
    public string GetObjectName() => itemID.ToLower();
    public InteractionGroup GetInteractionGroup() => InteractionGroup.Default;
}
