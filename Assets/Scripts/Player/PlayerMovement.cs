using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 5f;

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
    }

    private void Update()
    {
       _conditions.IsGrounded = _controller.isGrounded;
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

    public void Sprint()
    {
        _moveSpeed *= 2f; 
    }

    public void ResetSpeed()
    {
        _moveSpeed = 5f;
    }
    #endregion

}
