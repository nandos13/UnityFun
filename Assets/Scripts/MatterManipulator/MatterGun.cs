using UnityEngine;
using System.Collections;

/* AUTHOR: 
 * Jake Perry
 * 
 * DESCRIPTION:
 * A simple physics-gun, similar to that in Garry's Mod. Allows the player to pick up objects in 
 * the game world and move them around. Created for fun :)
 */

public class MatterGun : UseableItem {

	public Transform origin;											// Muzzle point for the gun. This determines where the rendered line will start
	public LineRenderer line;											// LineRenderer component used
	[Range (5.0f, 300.0f)]
	public float range = 30.0f;											// Maximum range of the gun
	[Range (0.0f, 100.0f)]
	public float strength = 100.0f;										// Strength of the gun

	private Rigidbody rb;												// Stores the currently held rigidbody component
	private Camera cam;													// Reference to the main camera (the player's vision)
	private Vector3 centerScreen = new Vector3();						// The center of the player's screen in pixels
	private PlayerAim aimScript;										// Reference to the aim script. This allows aiming to be disabled while rotating an object
	private PlayerHeldItem heldScript;									// Reference to the held item script. This allows weapon changing through scrolling to be disabled when an object is held

	private Ray ray;													// A ray used to find the objects in front of the player
	private RaycastHit hit;												// The hit info for the object currently being manipulated
	private Vector3 impactPointLocal = new Vector3();					// Offset of the grabbed point in local-space
	private Vector3 impactPointOffset = new Vector3();					// Tracks world-space offset of the grabbed point
	private CollisionDetectionMode rbMode;								// Stores the collision mode of the rigidbody so it can be reverted when dropped
	private float holdDistance = 0;										// Tracks how far away from the player the object should be held
	private bool throwLockMouse = false;								// Disables an object from being picked up immediately after being thrown with the gun

	void Start () 
	{
		// Get reference to player's view camera
		cam = Camera.main;

		// Find the center of the screen in pixels
		centerScreen = new Vector3 ((cam.pixelWidth / 2), (cam.pixelHeight / 2), 0);

		// Find the player's aiming script
		aimScript = transform.GetComponentInParent<PlayerAim>();

		// Find the player's held item script
		heldScript = transform.GetComponentInParent<PlayerHeldItem>();
	}

	void Update ()
	{
		// Find the center of the screen in pixels, incase the player's screen resolution has changed
		centerScreen.x = cam.pixelWidth / 2;
		centerScreen.y = cam.pixelHeight / 2;

		// Update the ray from the player's view to be used for finding and manipulating objects
		if (cam)
			ray = cam.ScreenPointToRay(centerScreen);

		if (rb)		// If the gun is currently holding an object...
		{
			// ... Recalculate the position of the part which was grabbed
			RecalculateGrabPoint();

			// Disable weapon switching with the scroll wheel, then apply zoom
			if (heldScript)
				heldScript.enableScrollChange = false;
			HandleZoom();
		}
		else
		{
			// Allow weapon changing with the scroll wheel
			if (heldScript)
				heldScript.enableScrollChange = true;
		}
	}

	void FixedUpdate () 
	{
		if (cam)
		{
			if (use0 && !throwLockMouse)		// If the gun is being used, and throw lock is not enabled...
			{
				if (!rb)		// If the gun is waiting to pick up a new object...
				{
					RaycastGetRigidBody();
				}

				// Allow the player to move the object
				HandleMovement();

				// Allow the player to throw the object
				HandleThrow();
			}
			else
			{
				// Gun is not being used, reset changed properties on the rigidbody
				if (rb)
				{
					// TODO: remove outline here. Not sure yet how this outline will even be achieved.

					// Reset collision detection mode
					rb.collisionDetectionMode = rbMode;

					// Reset interpolation settings
					rb.interpolation = RigidbodyInterpolation.None;
				}

				// Unlock mouse pickup
				if (!use0)
					throwLockMouse = false;

				// Drop rigidbody object being manipulated
				rb = null;
			}

			// Allow the player to rotate the object
			HandleRotation();
		}
	}

	private void RaycastGetRigidBody ()
	{
		// ... raycast to find the first object in front of the player
		RaycastHit[] hits = Physics.RaycastAll (ray, range);

		// Ignore any collisions with the player
		hits = hits.IgnoreChildren(this.gameObject);

		if (hits.Length > 0)
		{
			// Order the array by distance from the player. (The order of a RaycastAll is not guaranteed and will sometimes be ordered backwards)
			hits = hits.OrderByDistance(ray.origin);

			// Get the closest object
			hit = hits[0];

			if (hit.transform)		// Double check the hit exists to avoid null reference exceptions
			{
				// Get the rigidbody of the object the player is aiming at
				rb = hit.transform.GetComponent<Rigidbody>();

				if (rb)		// If a rigidbody was successfully found...
				{
					// ... Store information about the point that was hit. We take the offset of this position from the transform's position.
					impactPointLocal = rb.transform.InverseTransformVector(hit.point - hit.transform.position);
					RecalculateGrabPoint();

					rbMode = rb.collisionDetectionMode;
					rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
					rb.interpolation = RigidbodyInterpolation.Interpolate;

					// Also store the current distance between the player and the object
					holdDistance = Vector3.Distance(cam.transform.position, hit.point);

					// TODO: OUTLINE THE OBJECT IN SOME WAY TO SHOW IT IS SELECTED
				}
			}
		}
	}

	private void HandleMovement()		// Applies physics manipulation to the held object
	{
		if (rb)		// If the gun is currently holding an object...
		{
			// ... Find the point directly in front of the player at the current hold distance
			Vector3 holdPointWorld = ray.GetPoint(holdDistance);

			// Stop any rotation on the object
			if (!rb.freezeRotation)
			{
				rb.freezeRotation = true;
				rb.freezeRotation = false;
			}

			// Remove any velocity
			rb.velocity = Vector3.zero;

			// Find vector from the grab point's current position to the desired destination
			Vector3 grabPointToDestination = holdPointWorld - (rb.transform.position + impactPointOffset);

			// Calculate how much force to apply to the object
			Vector3 force = (grabPointToDestination * strength);

			// Move the rigidbody to the desired position
			rb.AddForce (force, ForceMode.VelocityChange);
		}
	}

	private void RecalculateGrabPoint()
	{
		impactPointOffset = rb.transform.TransformVector(impactPointLocal);
	}

	private void HandleThrow()		// Throw the object away from the player
	{
		if (rb && use1)
		{
			// Lock mouse pickup, so the object is not immediately captured again. The user will have to let go of the mouse button to pick objects up again
			throwLockMouse = true;

			// Apply a force in the direction the player is facing
			rb.AddForce (ray.direction * strength * 2, ForceMode.VelocityChange);

			// Release the object
			rb = null;
		}
	}

	private void HandleZoom()		// Zoom the object closer to or further away from the player, based on scroll wheel input
	{
		float zoom = Input.GetAxis("Mouse ScrollWheel");
		if (zoom != 0)
		{
			holdDistance += zoom * 10;
			holdDistance = Mathf.Clamp (holdDistance, 4, range);
			//TODO: Take into account collider bounds with minimum distance
		}
	}

	private void HandleRotation()		// Allow the player to rotate the object if they are holding down the scroll wheel
	{
		if (rb && use2)		// If an object is held and the player is rotating...
		{
			// ... Disable the player's aiming
			if (aimScript)
				aimScript.enabled = false;

			// Rotate the object based on the mouse movement
			Vector3 moveDirection = new Vector3 (Input.GetAxis("Mouse Y"), -(Input.GetAxis("Mouse X")), 0);
			moveDirection *= 10;
			Debug.Log("MoveDir: " + moveDirection);
			Debug.Log("Rotation: " + rb.rotation.eulerAngles);
			//TODO: FINISH IMPLEMENTING ROTATION. CURRENT STUCK ON BUG WHERE ROTATING VIA QUATERNION ROTATION GETS STUCK AT 90 AND 270 DEGREES??
			rb.MoveRotation(Quaternion.Euler(moveDirection + rb.rotation.eulerAngles));
		}
		else
		{
			// Enable the player's aiming
			if (aimScript)
				aimScript.enabled = true;
		}
	}

	public override void Unequip ()
	{
		rb = null;
	}

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(ray.origin, ray.GetPoint(range));
	}
}
