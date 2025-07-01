using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _xSensitivity = 20f;
    [SerializeField] private float _ySensitivity = 20f;

    private Camera _camera;
    private float _rotationX = 0f;

    private void Start()
    {
        _camera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
    }

    public void Look(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        _rotationX -= (mouseY * _ySensitivity) * Time.deltaTime;
        _rotationX = Mathf.Clamp(_rotationX, -80f, 80f);

        _camera.transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);

        transform.Rotate(Vector3.up * (mouseX * _xSensitivity) * Time.deltaTime);
    }
}
