using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateFlashLight : MonoBehaviour
{
    private GameObject _flashLight;

    private void FlashLightControl(bool light)
    {
        if (transform.childCount == 0) return;
        _flashLight = transform.GetChild(0).gameObject;
        _flashLight.SetActive(light);
    }

    private void OnEnable()
    {
        EnemyAI.OnEnemyAttack += FlashLightControl;
    }

    private void OnDisable()
    {
        EnemyAI.OnEnemyAttack -= FlashLightControl;
    }
}
