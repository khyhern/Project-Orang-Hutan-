using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _stamina;
    [SerializeField] private float _maxStamina = 50f;

    #region Internal
    private CharacterController _controller;
    private PlayerConditions _conditions;
    private Vector3 _velocity;
    private float _gravity = -9.81f;
    #endregion

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _conditions = new PlayerConditions();
        _stamina = _maxStamina;
        UIManager.Instance.UpdateStamina(_stamina, _maxStamina);
    }

    private void Update()
    {
        _conditions.IsGrounded = _controller.isGrounded;
        CheckSprint();

        // No stamina no sprint
        if (_stamina == 0f)
        {
            ResetSpeed();
        }

    }

    #region Movement
    public void MovePlayer(Vector2 input)
    {
        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        _controller.Move(transform.TransformDirection(moveDir) * _moveSpeed * Time.deltaTime);

        ApplyGravity();
    }
    private void ApplyGravity()
    {
        _velocity.y += _gravity * Time.deltaTime;
        if (_conditions.IsGrounded && _velocity.y < 0)
        {
            _velocity.y = -1f;
        }
        
        _controller.Move(_velocity * Time.deltaTime);     
    }
    #region Sprinting
    public void Sprint()
    {
        _conditions.IsSprinting = true;
        _moveSpeed *= 2.5f; 
    }

    public void ResetSpeed()
    {
        _moveSpeed = 5f;
        _conditions.IsSprinting = false;
    }

    private void CheckSprint()
    {
        if (_conditions.IsSprinting)
        {
            _stamina -= 25f * Time.deltaTime;
        }
        else
        {
            _stamina += 2f * Time.deltaTime;
        }

        _stamina = Mathf.Clamp(_stamina, 0f, 50f);
        
        UIManager.Instance.UpdateStamina(_stamina, _maxStamina);
    }
    #endregion
    #endregion

}
