using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachinePOVExtension : CinemachineExtension
{
    // Start is called before the first frame update
		[SerializeField]
		private float horizontalSpeed = 10f;
		[SerializeField]
		private float verticalSpeed = 10f;
		[SerializeField]
		private float clampAngle = 80f;
		
		private InputManager inputManager;
		private Vector3 startingRotation;
		
		private float delayBeforeInput = 0.2f;
	private float timeSinceStart = 0f;
	private bool initialized = false;
		
		protected override void Awake()
		{
			inputManager = InputManager.Instance;
			base.Awake();
					Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		}
		
	protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
	{
		if (vcam.Follow && stage == CinemachineCore.Stage.Aim)
		{
			timeSinceStart += Time.deltaTime;
			
    if (!initialized)
    {
        startingRotation = transform.localRotation.eulerAngles;
        if (timeSinceStart >= delayBeforeInput)
            initialized = true;
        return; // Skip applying rotation until ready
    }
		
			Vector2 deltaInput = inputManager.GetMouseDelta();
			startingRotation.y += deltaInput.x * horizontalSpeed * Time.deltaTime;
			startingRotation.x -= deltaInput.y * verticalSpeed * Time.deltaTime;
			startingRotation.x = Mathf.Clamp(startingRotation.x, -clampAngle, clampAngle);

			state.RawOrientation = Quaternion.Euler(startingRotation.x, startingRotation.y, 0f);
		}
	}
}
