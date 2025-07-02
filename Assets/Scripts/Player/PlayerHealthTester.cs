using UnityEngine;

public class PlayerHealthTester : MonoBehaviour
{
    private PlayerHealth player;

    private void Start()
    {
        player = PlayerHealth.Instance;

        if (player == null)
        {
            Debug.LogWarning("⚠️ PlayerHealthTester: No PlayerHealth instance found in scene.");
        }
    }

    void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.DamagePart(BodyPart.LeftLeg, 100);
            Debug.Log("Left Leg damaged.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            player.DamagePart(BodyPart.RightLeg, 100);
            Debug.Log("Right Leg damaged.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            player.DamagePart(BodyPart.LeftArm, 100);
            Debug.Log("Left Arm damaged.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            player.DamagePart(BodyPart.RightArm, 100);
            Debug.Log("Right Arm damaged.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            player.DamagePart(BodyPart.Torso, 100);
            Debug.Log("Torso damaged.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            player.DamagePart(BodyPart.Head, 999);
            Debug.Log("Head destroyed.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            player.DamagePlayer(25);
            Debug.Log("Global damage: -25 HP");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("IsDead(): " + player.IsDead());
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            PlayerHealth.Instance.BandageLimb(BodyPart.LeftLeg);
        }

    }
}
