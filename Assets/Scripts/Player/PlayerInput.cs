using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        _playerMovement.MovePlayer(InputSystem.actions.FindAction("Move").ReadValue<Vector2>());
    }

    private void OnSprint(InputAction.CallbackContext obj)
    {
        _playerMovement.Sprint();
    }

    private void OnWalk(InputAction.CallbackContext obj)
    {
        _playerMovement.ResetSpeed();
    }

    private void MovemntInputController(bool canMove)
    {
        if (!canMove)
        {
            InputSystem.actions.FindAction("Move").Disable();
            InputSystem.actions.FindAction("Sprint").Disable();
        }
        else
        {
            InputSystem.actions.FindAction("Move").Enable();
            InputSystem.actions.FindAction("Sprint").Enable();
        }
        
    }

    private void OnEnable()
    {
        InputSystem.actions.FindAction("Sprint").started += OnSprint;
        InputSystem.actions.FindAction("Sprint").canceled += OnWalk;
        EnemyAI.OnEnemyAttack += MovemntInputController;
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Sprint").started -= OnSprint;
        InputSystem.actions.FindAction("Sprint").canceled -= OnWalk;
        EnemyAI.OnEnemyAttack -= MovemntInputController;
    }
}
