using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHear : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float hearingRange = 15f;
    [SerializeField] private LayerMask whatIsEnemy;

    private void Update()
    {
        CheckEnemy();
    }

    private void CheckEnemy()
    {
        if (Physics.CheckSphere(transform.position, hearingRange, whatIsEnemy))
        {          
            AudioManager.Instance.IncreaseVolumeHB();
        }
        else
        {
            AudioManager.Instance.DecreaseVolumeHB();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }
}
