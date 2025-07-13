using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour
{
    [System.Serializable]
    public struct BatteryColorStage
    {
        public Color color;
        public float threshold; // Battery level required to switch to this color
    }

    [Header("References")]
    [SerializeField] private Light spotlight;
    [SerializeField] private GameObject onVisual;
    [SerializeField] private GameObject offVisual;
    [SerializeField] private Animator animator;

    [Header("Battery Settings")]
    [SerializeField] private int maxLifetime = 100;
    [SerializeField] private float drainRate = 1f;
    [SerializeField] private float currentLifetime;
    private bool isOn = false;
    private bool isDepleted = false;

    [Header("Battery UI")]
    private Image batteryFillImage;
    [SerializeField] private List<BatteryColorStage> colorStages;

    public void Activate()
    {
        this.enabled = true;

        spotlight.enabled = false;
        onVisual.SetActive(false);
        offVisual.SetActive(true);

        if (currentLifetime <= 0f)
        {
            currentLifetime = maxLifetime;
            isDepleted = false;
        }


        if (batteryFillImage == null)
        {
            GameObject uiObj = GameObject.Find("Battery lifetime bar");
            if (uiObj != null)
            {
                batteryFillImage = uiObj.GetComponent<Image>();
                batteryFillImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("[FlashlightController] UI object 'Battery lifetime bar' not found.");
            }
        }

        UpdateAnimatorValues();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            ToggleLight();
        }

        if (isOn)
        {
            DrainBattery();
        }

        FaceCenterOfScreen();
    }

    void ToggleLight()
    {
        if (currentLifetime <= 0f) return;

        isOn = !isOn;
        spotlight.enabled = isOn;
        onVisual.SetActive(isOn);
        offVisual.SetActive(!isOn);
    }

    void DrainBattery()
    {
        currentLifetime -= drainRate * Time.deltaTime;
        currentLifetime = Mathf.Max(currentLifetime, 0f);

        if (currentLifetime <= 0f && !isDepleted)
        {
            isOn = false;
            spotlight.enabled = false;
            onVisual.SetActive(false);
            offVisual.SetActive(true);
            isDepleted = true;

            animator.SetBool("DepletedLight", true); // Transition to Depleted

            // Disable flashlight behavior after depletion
            this.enabled = false;
        }

        UpdateAnimatorValues();
    }

    public void AddBattery(int amount)
    {
        // If the flashlight was disabled, re-enable it
        if (!this.enabled)
            this.enabled = true;

        // Add battery correctly (without resetting to max!)
        currentLifetime += amount;
        currentLifetime = Mathf.Clamp(currentLifetime, 0f, maxLifetime);

        UpdateAnimatorValues();
    }

    void UpdateAnimatorValues()
    {
        if (animator != null)
            animator.SetInteger("Lifetime", Mathf.RoundToInt(currentLifetime));

        if (batteryFillImage != null)
        {
            batteryFillImage.fillAmount = currentLifetime / maxLifetime;

            foreach (var stage in colorStages)
            {
                if (currentLifetime >= stage.threshold)
                {
                    batteryFillImage.color = stage.color;
                    break;
                }
            }
        }
    }

    void FaceCenterOfScreen()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 lookDirection = ray.direction;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        targetRotation *= Quaternion.Euler(90f, -4f, 0f);

        transform.rotation = targetRotation;
    }
}

