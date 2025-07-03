using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Animator _animator;
    private Vector3 _velocity;
    private float _gravity = -9.81f;
    [HideInInspector] public bool isCarryingFriend = false;
    [HideInInspector] public float carryingSpeed = 2.5f;
    private float _defaultMoveSpeed = 5f;
    

    public PlayerConditions Conditions => _conditions;
    #endregion

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _conditions = new PlayerConditions();
        _stamina = _maxStamina;
        _animator = GetComponent<Animator>();
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
        if (input != Vector2.zero)
        {
            _conditions.IsWalking = true;
            _animator.SetBool("Walk", true);
        }
        else
        {
            _conditions.IsWalking = false;
            _animator.SetBool("Walk", false);
        }

        float speedMultiplier = 1f;

        if (PlayerHealth.Instance != null)
        {
            speedMultiplier = PlayerHealth.Instance.GetMovementSpeedMultiplier();
        }

        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        _controller.Move(transform.TransformDirection(moveDir) * _moveSpeed * speedMultiplier * Time.deltaTime);
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
        if (isCarryingFriend)
            _moveSpeed = carryingSpeed;
        else
            _moveSpeed = _defaultMoveSpeed;

        _conditions.IsSprinting = false;
        Debug.Log("Player move speed reset to: " + _moveSpeed);
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
   

    public void SetMoveSpeed(float newSpeed)
    {
        _moveSpeed = newSpeed;
        Debug.Log("Player move speed set to: " + _moveSpeed);
    }

    #endregion
    #region Move SFX
    public void PlayWalkSFX()
    {
        if (_conditions.IsGrounded && _conditions.IsSprinting == false && _conditions.IsWalking)
        {
            AudioManager.Instance.PlaySFXWalk();
            var sound = new Sound(transform.position, 10f);

            Sounds.MakeSound(sound);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 10f);
    }
    #endregion
    #endregion

}
