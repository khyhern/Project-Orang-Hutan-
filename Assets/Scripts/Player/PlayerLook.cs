using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask _whatIsEnemy;

    private Camera _mainCamera;
    private float _seeDistance = 100f;

    public static Action<bool> OnPlayerSeeEnemy;

    private void Start()
    {
        // Find the main camera in the scene
        _mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked; 
    }

    private void Update()
    {
        // Rotate the player towards the camera every frame
        RotatePlayerTowardsCamera();
        SeeEnemy();
    }

    private void RotatePlayerTowardsCamera()
    {
        if (_mainCamera != null)
        {
            Vector3 cameraForward = _mainCamera.transform.forward;
            cameraForward.y = 0f; // Ignore the y-axis rotation

            if (cameraForward != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(cameraForward);
                transform.rotation = newRotation;
            }
        }
    }

    private void SeeEnemy()
    {
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * _seeDistance, Color.red);

        if (Physics.Raycast(ray, out hit, _seeDistance, _whatIsEnemy))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Player sees an enemy: " + hit.collider.name);
                // If the raycast hits an enemy, play the see enemy sound effect
                AudioManager.Instance.PlaySFXseeEnemy();
                OnPlayerSeeEnemy?.Invoke(true);
            }
            else
            {
                // If the raycast does not hit an enemy, invoke with false
                OnPlayerSeeEnemy?.Invoke(false);
            }
        }
    }
}
