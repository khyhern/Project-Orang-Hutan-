using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private PlayerLook _playerLook;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerLook = GetComponent<PlayerLook>();
    }

    private void Update()
    {
        _playerMovement.MovePlayer(InputSystem.actions.FindAction("Move").ReadValue<Vector2>());
        _playerLook.Look(InputSystem.actions.FindAction("Look").ReadValue<Vector2>());
    }

    private void OnSprint(InputAction.CallbackContext obj)
    {
        _playerMovement.Sprint();
    }

    private void OnWalk(InputAction.CallbackContext obj)
    {
        _playerMovement.ResetSpeed();
    }

    private void OnEnable()
    {
        InputSystem.actions.FindAction("Sprint").started += OnSprint;
        InputSystem.actions.FindAction("Sprint").canceled += OnWalk;
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Sprint").started -= OnSprint;
        InputSystem.actions.FindAction("Sprint").canceled -= OnWalk;
    }
}
