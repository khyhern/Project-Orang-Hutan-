using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CutsceneEnemyAttacker : MonoBehaviour
{
    [Header("Cutscene Damage Settings")]
    [SerializeField] private int damageAmount = 10;

    [Header("Camera & Effects")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private GameObject bloodEffect;
    [SerializeField] private Light redLight;
    [SerializeField] private Light whiteLight;
    [SerializeField] private CinemachineCamera cameraPlayer;

    public void DamagePlayer()
    {
        Debug.Log($"[CutsceneEnemyAttacker] Damaging player for {damageAmount} HP");

        // Lights & Blood FX
        whiteLight.enabled = false;
        redLight.enabled = true;
        bloodEffect.SetActive(true);

        // Audio
        AudioManager.Instance.PlaySFXBlood();
        AudioManager.Instance.PlaySFXBreath();

        // Damage Player
        PlayerHealth.Instance?.DamagePlayer(damageAmount);

        // Screen Shake
        StartCoroutine(PlayImpulseShake());

        // Camera reset and cleanup
        StartCoroutine(CutsceneExit());
    }

    private IEnumerator PlayImpulseShake()
    {
        float time = 0f;
        while (time < 4f)
        {
            Vector3 shake = new Vector3(
                Random.Range(-0.3f, 0.3f),
                Random.Range(-0.3f, 0.3f),
                0.2f
            );

            impulseSource.GenerateImpulseWithVelocity(shake);
            yield return new WaitForSeconds(0.3f);
            time += 0.3f;
        }
    }

    private IEnumerator CutsceneExit()
    {
        yield return new WaitForSeconds(2.5f);

        redLight.enabled = false;
        bloodEffect.SetActive(false);

        yield return new WaitForSeconds(1f);
        CameraManager.SwitchCamera(cameraPlayer);
    }
}
