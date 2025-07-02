using UnityEngine;

public class FriendBehavior : MonoBehaviour
{
    public Animator animator; // Assign in inspector or get in Awake/Start
    public string spriteChildName = "YourSpriteChildName"; // Set this to your sprite child's name

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // Call this when the friend is delivered to a safepoint
    public void OnDeliveredToSafepoint()
    {
        // 1. Delete the child sprite object first
        Transform spriteChild = transform.Find(spriteChildName);
        if (spriteChild != null)
        {
            Destroy(spriteChild.gameObject);
        }

        // 2. Then play the animation
        if (animator != null)
        {
            animator.SetTrigger("Delivered");
        }
    }
}