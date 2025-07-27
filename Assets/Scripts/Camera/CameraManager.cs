using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    static List<CinemachineCamera> cameras = new List<CinemachineCamera>();

    public static CinemachineCamera ActiveCamera = null;

    public static bool IsCameraActive(CinemachineCamera camera)
    {
        return camera == ActiveCamera;
    }

    public static void SwitchCamera(CinemachineCamera newCamera)
    {
        newCamera.Priority = 1;
        ActiveCamera = newCamera;

        foreach (CinemachineCamera camera in cameras)
        {
            if (camera != newCamera)
            {
                camera.Priority = 0;
            }
        }
    }

    public static void Register(CinemachineCamera camera)
    {
        cameras.Add(camera);
    }

    public static void Unregister(CinemachineCamera camera)
    {
        cameras.Remove(camera);
    }
}
