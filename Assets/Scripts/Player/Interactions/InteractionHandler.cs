using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using InControl;

/* DESCRIPTION:
 * This script is intended to go on the player. 
 * Raycasts forward and finds all instances of an Interaction script within a
 * certain radius. The script then determines which instance is closest and will
 * call the Execute () function on said script.
 */

public class InteractionHandler : MBAction {

	[Range (0, 20), Tooltip ("How far the player can reach to interact with objects")]
	public float reach = 5;
	[Range (0, 5), Tooltip ("Interactions within this radius of where the mouse is pointing will be considered")]
	public float radius = 2;

	[Tooltip ("Only allows Interactable objects within a specified angle of the player's view to be useable. This will prevent objects behind the player from being identified.")]
	public bool forceMaxAngle = true;
	[Range (1.0f, 10.0f)]
	public float maxAngle = 3.0f;

	private Camera mainCam;
	public Text HUDText;

	private Interaction closest;
	private Vector3 debugSpherePos = Vector3.zero;
	private bool pressed = false;
	//private InputDevice inputDevice;

	void Start ()
	{
		mainCam = Camera.main;
		debugSpherePos = transform.position;
	}

	void Update () 
	{
		// Raycast forward
		RaycastHit hit = new RaycastHit();
		RaycastHit[] hits = Physics.RaycastAll (new Ray (mainCam.transform.position, mainCam.transform.forward), reach);

		// Find the first hit that is not a part of the player
		hits = hits.IgnoreChildren (this.gameObject);
		if (hits.Length > 0)
			hit = hits[0];

		if (hit.collider)
		{
			// Find all instances of Interaction scripts around the raycast hit point
			debugSpherePos = hit.point;
			findClosest (hit.point);
		}
		else
		{
			// The player's aim is not over any object within the reach distance
			// Find all instances of Interaction scripts around the player
			debugSpherePos = transform.position;
			findClosest (transform.position);
		}

		//inputDevice = InputManager.ActiveDevice;
		bool interactUsed = (Input.GetAxisRaw ("Interact") > 0);
		//if (!interactUsed)
		//	interactUsed = (inputDevice.Action3);
		if (interactUsed)
		{
			if (!pressed)
			{
				pressed = true;
				Execute ();
			}
		}
		else
			pressed = false;
	}

	public override void Execute ()
	{
		if (closest)
			closest.Execute();
	}

	private void findClosest (Vector3 pos)
	{
		Collider[] withinRadius = Physics.OverlapSphere (pos, radius);
		List<Interaction> scripts = new List<Interaction>();

		// Find all Interaction scripts on any collider in withinRadius
		Interaction temp;		// Memory declaration optimization
		foreach (Collider c in withinRadius)
		{
			temp = c.GetComponent<Interaction>();
			if (temp)
			{
				// Should we only accept Interactable objects within an angle of the players view?
				if (forceMaxAngle)
				{
					// Check angle from player's view
					float a = Vector3.Angle (mainCam.transform.forward + mainCam.transform.position, temp.transform.position);
					if (a <= maxAngle)
						scripts.Add (temp);
				}
				else
					scripts.Add (temp);
			}
		}

		if (scripts.Count > 0)
		{
			// Find the closest script to where the player is looking
			float dist = float.MaxValue;
			float currDist = 0;
			foreach (Interaction i in scripts)
			{
				if (i)
				{
					currDist = Vector3.Distance (pos, i.transform.position);
					if ((currDist < dist))
						closest = i;
				}
			}
		}
		else
			closest = null;

		// Handle HUD element
		updateHudText (closest);
	}

	private void updateHudText (Interaction script)
	{
		/* Updates a text element on the HUD to show the player which item they will be interacting with */
		if (HUDText)
		{
			if (closest)
				HUDText.text = ("'E' " + closest.hoverMessage);
			else
				HUDText.text = "";
		}
	}
	 
	void OnDrawGizmosSelected ()
	{
		// Dray the ray used to find Interactable scripts
		Gizmos.color = Color.black;
		if (debugSpherePos == transform.position)
			Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere (debugSpherePos, radius);
	}
}
