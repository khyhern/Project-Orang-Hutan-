using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraRegister : MonoBehaviour
{
    private void OnEnable()
    {
        CameraManager.Register(GetComponent<CinemachineCamera>());
    }

    private void OnDisable()
    {
        CameraManager.Unregister(GetComponent<CinemachineCamera>());
    }
}
