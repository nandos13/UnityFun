using UnityEngine;
using System.Collections;

public class WireGun : UseableItem {

	[Range (5.0f, 100.0f)]
	public float range = 30.0f;						// Maximum range of the gun

	private WireHub nodeA;							// Stores the first selected connection
	private WireHub nodeB;							// Stores the second selected connection

	private Ray ray;								// A ray used to find an object with a WireHub component infront of the player
	private Camera cam;								// Reference to the player's view camera
	private Vector3 centerScreen = new Vector3();	// The center of the player's screen in pixels
	private static GameObject wireRenderer;			// An empty gameObject used to hold linerenderer components for wires

	void Awake ()
	{
		// Instantiate wireRenderer object if it does not already exist
		if (!wireRenderer)
		{
			wireRenderer = new GameObject();
			LineRenderer lr = wireRenderer.AddComponent<LineRenderer>();
			Material blackDiffuse = new Material(Shader.Find("Standard"));
			blackDiffuse.color = Color.black;
			lr.material = blackDiffuse;
			lr.SetVertexCount(0);
			lr.SetWidth(0.02f, 0.02f);
		}
	}

	void Start ()
	{
		// Get reference to player's view camera
		cam = Camera.main;

		// Find the center of the screen in pixels
		centerScreen = new Vector3 ((cam.pixelWidth / 2), (cam.pixelHeight / 2), 0);
	}

	void Update ()
	{
		// Find the center of the screen in pixels, incase the player's screen resolution has changed
		centerScreen.x = cam.pixelWidth / 2;
		centerScreen.y = cam.pixelHeight / 2;

		// Update the ray from the player's view to be used for finding WireHub components
		if (cam)
			ray = cam.ScreenPointToRay(centerScreen);
	}

	public override void Use0 ()		// (Left mouse) Select a connection hub
	{
		if (nodeA == null)		// If no hubs have been selected yet...
		{
			// Find the first hub to attach a wire to
			nodeA = FindHubWithRay();
		}
		else
		{
			// One hub has been selected. Find a second hub to attach a wire to
			nodeB = FindHubWithRay();

			if (nodeB)		// If a second node was found...
			{
				// ... Check if both nodes are the same instance
				if (nodeA == nodeB)
					ClearNodes();
				else
				{
					// Two separate nodes have been selected. Create a new wire (the wire will automatically be attached to both nodes).
					Wiring wire = new Wiring(nodeA, nodeB, wireRenderer);
					// NOTE: Console may warn this variable is declared and never used. Disregard warning! The wire is automatically set up in the constructor

					ClearNodes();
				}
			}
		}
	}

	public override void Use2 ()		// (Middle mouse) Clear current selection
	{
		ClearNodes();
	}

	private void ClearNodes ()
	{
		nodeA = null;
		nodeB = null;
	}

	private WireHub FindHubWithRay ()
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
			RaycastHit hit = hits[0];

			if (hit.transform)		// Double check the hit exists to avoid null reference exceptions
			{
				// Get the WireHub component of the object the player is aiming at
				WireHub hub = hit.transform.GetComponentInParent<WireHub>();

				if (hub)		// If a WireHub was successfully found...
				{
					Debug.Log("Found WireHub on " + hub.transform.name);
					return hub;		// Return this hub
				}
			}
		}

		return null;		// No WireHub component was found on the object the player is aiming at
	}
}
