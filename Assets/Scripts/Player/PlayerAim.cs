using UnityEngine;
using System.Collections;
//using InControl;

/* DESCRIPTION:
 * Handles player look behaviour.
 * Rotates the player for horizontal axis, and tilts the cameras and gun
 * for any movement on the vertical axis.
 */

public class PlayerAim : MonoBehaviour {

	private Vector2 mouseMovement;		// Tracks the current angle of the camera
	private Vector2 smoothV;			// Used to store the movement difference each frame

	[Range (0.1f, 10.0f)]
	public float sensitivity = 4.0f;	// Look sensitivity

	[Range (0.1f, 10.0f)]
	public float controllerSensitivity = 1.0f;	// Look sensitivity when using a controller

	[Range(1.0f, 10.0f)]
	public float smoothing = 1.0f;		// 

	public Camera WorldViewCam;
	public Camera ViewModelCam;

	//private InputDevice inputDevice;

	void Start () 
	{
		smoothV.x = transform.rotation.eulerAngles.y;
		mouseMovement.x = transform.rotation.eulerAngles.y;

        //Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

	void Update () 
	{
		/* InControl integration. Allows controller support but requires InControl asset.
		 * 
		inputDevice = InputManager.ActiveDevice;
		Vector2 moveDirection;
		// Get input from controller
		moveDirection = new Vector2 (inputDevice.RightStickX, inputDevice.RightStickY);

		// Get input from mouse (if running on pc)
		if (!Application.isConsolePlatform && moveDirection.magnitude <= 0)
		{
			moveDirection = new Vector2 (Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
			moveDirection = Vector2.Scale (moveDirection, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
		}
		else
			moveDirection = Vector2.Scale (moveDirection, new Vector2(controllerSensitivity * smoothing, controllerSensitivity * smoothing));

		* 
		* End InControl Code
		*/

		Vector2 moveDirection = new Vector2 (Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
		moveDirection = Vector2.Scale (moveDirection, new Vector2(sensitivity * smoothing, sensitivity * smoothing));


		if (smoothing > 1.0f) 
		{
			// Apply smoothing
			smoothV.x = Mathf.Lerp (smoothV.x, moveDirection.x, 1.0f/smoothing);
			smoothV.y = Mathf.Lerp (smoothV.y, moveDirection.y, 1.0f/smoothing);
		} else 
		{
			// Ignore smoothing
			smoothV.x = Mathf.Lerp (smoothV.x, moveDirection.x, 1.0f);
			smoothV.y = Mathf.Lerp (smoothV.y, moveDirection.y, 1.0f);
		}

		mouseMovement += smoothV;		// Apply the movement to the current angle

		// CLAMP camera to prevent vertical rotation past the direct upwards direction (preventing looking backwards with an upside down view)
		mouseMovement.y = Mathf.Clamp(mouseMovement.y, -88.0f, 88.0f);

		if (WorldViewCam)
			WorldViewCam.transform.localRotation = Quaternion.AngleAxis (-mouseMovement.y, Vector3.right);
		if (ViewModelCam)
			ViewModelCam.transform.localRotation = Quaternion.AngleAxis (-mouseMovement.y, Vector3.right);
		transform.localRotation = Quaternion.AngleAxis (mouseMovement.x, transform.up);
		
	}
}
