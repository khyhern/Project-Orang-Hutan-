using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class HeadBobSystem : MonoBehaviour
{
    [Header("Settings")]
    [Range(0.001f, 0.01f)]
    [SerializeField] private float _amount = 0.002f;

    [Range(1f, 30f)]
    [SerializeField] private float _frequency = 10f;

    [Range(10f, 100f)]
    [SerializeField] private float _smoothness = 10f;

    private Vector3 _originalPosition;
    private bool _isTriggered;
    private float Sin;
    private float _startFrequency;
    private float _startAmount;
    private float _startSmoothness;

    public static Action OnFootStep;

    private void Start()
    {
        _originalPosition = transform.localPosition;
    }

    private void Update()
    {
        CheckForHeadBobTrigger();
        StopHeadBob();
    }

    private void CheckForHeadBobTrigger()
    {
         
        float inputMagnitude = InputSystem.actions.FindAction("Move").ReadValue<Vector2>().magnitude;
        if (inputMagnitude > 0)
        {
            StartHeadBob();
        }
    }

    private Vector3 StartHeadBob()
    {
        Vector3 pos = Vector3.zero;

        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * _frequency) * _amount * 1.4f, _smoothness * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * _frequency/2f) * _amount * 1.6f, _smoothness * Time.deltaTime);
        transform.localPosition += pos;

        // Footsteps sound effect
        Sin = Mathf.Sin(Time.time * _frequency);
        if (Sin > 0.97f && !_isTriggered)
        {
            _isTriggered = true;
            Debug.Log("Footstep sound triggered");
            OnFootStep?.Invoke();
        }
        else if (_isTriggered == true && Sin < -0.97f)
        {
            _isTriggered = false;
        }

        return pos;
    }

    private void StopHeadBob()
    {
        if (transform.localPosition == _originalPosition) return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, _originalPosition, 1 * Time.deltaTime);
    }

    public void ReduceHeadBob(float speedModifier)
    {
        _frequency = 20f * speedModifier;
        _amount = 0.01f * speedModifier;
    }

    public void IncreaseHeadBob()
    {
        _startFrequency = _frequency;
        _startAmount = _amount;
        _startSmoothness = _smoothness;

        _frequency *= 2.2f;
        _amount *= 1.5f;
        _smoothness *= 1.5f;
    }

    public void ResetHeadBob()
    {
        _frequency = _startFrequency;
        _amount = _startAmount;
        _smoothness = _startSmoothness;
    }
}
