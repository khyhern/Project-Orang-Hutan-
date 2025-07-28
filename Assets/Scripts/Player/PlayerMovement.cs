using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _stamina;
    [SerializeField] private float _maxStamina = 50f;

    public HeadBobSystem HeadBobSystem;
    public Transform Body;

    #region Internal
    private CharacterController _controller;
    private PlayerConditions _conditions;
    private Animator _animator;
    private Vector3 _velocity;
    private float _gravity = -9.81f;
    [HideInInspector] public bool isCarryingFriend = false;
    [HideInInspector] public float carryingSpeed = 1.5f;
    private float _defaultMoveSpeed = 3f;

    public bool canMove = true; // ✅ Added movement lock flag
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

        if (_stamina == 0f)
        {
            ResetSpeed();
        }
    }

    public void MovePlayer(Vector2 input)
    {
        if (!canMove) return; // ✅ Prevent movement if sitting

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
        Debug.Log(speedMultiplier * _moveSpeed);
        HeadBobSystem.ReduceHeadBob((speedMultiplier * _moveSpeed) / 7.5f);
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
        _conditions.IsSprinting = true;
        _moveSpeed *= 2.5f;
        HeadBobSystem.IncreaseHeadBob();
        _animator.SetBool("Sprint", true);
    }

    public void ResetSpeed()
    {
        _moveSpeed = isCarryingFriend ? carryingSpeed : _defaultMoveSpeed;
        _conditions.IsSprinting = false;
        HeadBobSystem.ResetHeadBob();
        _animator.SetBool("Sprint", false);
    }

    private void CheckSprint()
    {
        if (_conditions.IsSprinting)
        {
            _stamina -= 20f * Time.deltaTime;
            if (Body.localPosition.z < 2f)
            {
                Body.Translate(Vector3.forward * 1.5f * Time.deltaTime);
            }
        }
        else
        {
            if (Body.localPosition.z > 0f)
            {
                Body.Translate(Vector3.back * 2f * Time.deltaTime);
            }

            _stamina += 20f * Time.deltaTime;
        }

        _stamina = Mathf.Clamp(_stamina, 0f, _maxStamina);
        UIManager.Instance.UpdateStamina(_stamina, _maxStamina);
    }

    public void SetMoveSpeed(float newSpeed)
    {
        _moveSpeed = newSpeed;
        Debug.Log("Player move speed set to: " + _moveSpeed);
    }

    public void PlayMovementSFX()
    {
        if (_conditions.IsGrounded && !_conditions.IsSprinting && _conditions.IsWalking)
        {
            AudioManager.Instance.PlaySFXWalk();
            var sound = new Sound(transform.position, 8f);

            Sounds.MakeSound(sound);
        }
        else if (_conditions.IsGrounded && _conditions.IsSprinting)
        {
            AudioManager.Instance.PlaySFXWalk();
            var sound = new Sound(transform.position, 12f);
            Sounds.MakeSound(sound);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 12f);
    }

    private void OnEnable()
    {
        HeadBobSystem.OnFootStep += PlayMovementSFX;
    }

    private void OnDisable()
    {
        HeadBobSystem.OnFootStep -= PlayMovementSFX;
    }
}
