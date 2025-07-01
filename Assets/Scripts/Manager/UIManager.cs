using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Settings")]
    [SerializeField] private Image _staminaImg;

    private float _stamina;
    private float _currentStamina;

    public void UpdateStamina(float stamina, float maxStamina)
    {
        _currentStamina = stamina;
        _stamina = maxStamina;
    }

    private void Update()
    {
        _staminaImg.fillAmount = Mathf.Lerp(_staminaImg.fillAmount, _currentStamina / _stamina, Time.deltaTime * 10f);
    }

}
