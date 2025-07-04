using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light spotlight;
    public GameObject onVisual;
    public GameObject offVisual;

    private bool isOn = false;

    public void Activate()
    {
        this.enabled = true;

        // Ensure it starts off
        spotlight.enabled = false;
        onVisual.SetActive(false);
        offVisual.SetActive(true);
    }

    void Update()
    {
        if (PressedOtherNumberKey())
        {
            UnequipFlashlight();
        }

        if (Input.GetMouseButtonDown(2))
        {
            ToggleLight();
        }

        FaceCenterOfScreen();
    }

    bool PressedOtherNumberKey()
    {
        for (int i = 0; i <= 9; i++)
        {
            KeyCode key = KeyCode.Alpha0 + i;
            if (key != KeyCode.Alpha1 && Input.GetKeyDown(key))
                return true;
        }
        return false;
    }

    void ToggleLight()
    {
        isOn = !isOn;
        spotlight.enabled = isOn;
        onVisual.SetActive(isOn);
        offVisual.SetActive(!isOn);
    }

    void FaceCenterOfScreen()
    {
        if (Camera.main == null) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 lookDirection = ray.direction;

        // Face the same direction as the camera (with optional offset)
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        // Optionally apply X-axis correction if flashlight model is tilted
        targetRotation *= Quaternion.Euler(90f, 0f, 0f); // Adjust this if needed

        transform.rotation = targetRotation;
    }

    void UnequipFlashlight()
    {
        Destroy(gameObject);
    }
}
