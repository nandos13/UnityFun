﻿using UnityEngine;
using System.Collections;

/* AUTHOR: 
 * Jake Perry
 * 
 * DESCRIPTION:
 * A simple physics-gun, similar to that in Garry's Mod. Allows the player to pick up objects in 
 * the game world and move them around. Created for fun :)
 */

public class MatterGun : MonoBehaviour {

	public Transform origin;											// Muzzle point for the gun. This determines where the rendered line will start
	public LineRenderer line;											// LineRenderer component used
	[Range (5.0f, 300.0f)]
	public float range = 30.0f;											// Maximum range of the gun
	[Range (0.0f, 100.0f)]
	public float strength = 100.0f;										// Strength of the gun. TODO: IMPLEMENT THIS!!!

	private Rigidbody rb;												// Stores the currently held rigidbody component
	private Camera cam;													// Reference to the main camera (the player's vision)
	private Vector3 centerScreen = new Vector3();						// The center of the player's screen in pixels

	private Ray ray;													// A ray used to find the objects in front of the player
	private RaycastHit hit;												// The hit info for the object currently being manipulated
	private Vector3 impactPoint = new Vector3();						// Initial offset of the grabbed point, used to calculate current grab offset
	private Quaternion impactRotation;									// Initial rotation of the grabbed object, used to calculate current grab offset
	private Vector3 impactPointOffset = new Vector3();					// Tracks offset of the grabbed point
	private float holdDistance = 0;

	void Start () 
	{
		// Get reference to player's view camera
		cam = Camera.main;

		// Find the center of the screen in pixels
		centerScreen = new Vector3 ((cam.pixelWidth / 2), (cam.pixelHeight / 2), 0);
	}

	void Update ()
	{
		// Update the ray from the player's view to be used for finding and manipulating objects
		if (cam)
			ray = cam.ScreenPointToRay(centerScreen);

		// Find the center of the screen in pixels, incase the player's screen resolution has changed
		centerScreen.x = cam.pixelWidth / 2;
		centerScreen.y = cam.pixelHeight / 2;

		if (rb)		// If the gun is currently holding an object...
		{
			// ... Recalculate the position of the part which was grabbed
			RecalculateGrabPoint();
		}
	}

	void FixedUpdate () 
	{
		if (cam && Input.GetMouseButton(0))		// If the gun is being used...
		{
			if (!rb)		// If the gun is waiting to pick up a new object...
			{
				// ... raycast to find the first object infront of the player
				RaycastHit[] hits = Physics.RaycastAll (ray, range);

				// Ignore any collisions with the player
				hits = hits.IgnoreChildren(this.gameObject);

				if (hits.Length > 0)
				{
					// Get the closest object
					hit = hits[0];

					if (hit.transform)		// Double check the hit exists to avoid null reference exceptions
					{
						// Get the rigidbody of the object the player is aiming at
						rb = hit.transform.GetComponent<Rigidbody>();

						if (rb)		// If a rigidbody was successfully found...
						{
							// ... Store information about the point that was hit. We take the offset of this position from the transform's position.
							impactPoint = hit.point - hit.transform.position;
							impactPointOffset = impactPoint;
							impactRotation = hit.transform.rotation;

							// Also store the current distance between the player and the object
							holdDistance = Vector3.Distance(cam.transform.position, hit.point);
						}
					}
				}
			}

			// Play with the physics of the object
			ExecutePhysics();
		}
		else
		{
			// Gun is not being used, reset changed properties on the rigidbody
			if (rb)
			{
				// TODO: remove outline. Not sure yet how this outline will even be achieved.
			}

			// Drop rigidbody object being manipulated
			rb = null;
		}
	}

	private void ExecutePhysics()		// Applies physics manipulation to the held object
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

			Debug.Log (force.magnitude);

			// Move the rigidbody to the desired position
			rb.AddForce (force, ForceMode.VelocityChange);
		}
	}

	private void RecalculateGrabPoint()
	{
		// Calculate where the initially grabbed point is now in relation to the object's transform position
		//TODO: FINISH THIS SO THE GUN WILL CONTINUE TO HOLD ON TO THE SAME POINT EVEN IF THE OBJECT IS ROTATED WHILE HELD
	}

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(ray.origin, ray.GetPoint(range));
	}
}