using UnityEngine;
using System.Collections;

/* AUTHOR: 
 * Jake Perry
 * 
 * DESCRIPTION:
 * Class allows two objects with WireHub components to be wired together. This allows
 * buttons, etc to activate other objects they are connected to.
 */

[System.Serializable]
public class Wiring {

	private GameObject lrHolder;					// An empty game object instantiated to hold the line renderer (otherwise the linerenderer is always null)
	private LineRenderer lr;						// Used to render a wire between the two nodes
	private int lastFrameRendered = 0;				// Tracks the last frame the line was rendered on to avoid double rendering on a single frame

	private WireHub nodeA;							// Connection 1
	private WireHub nodeB;							// Connection 2

	public Wiring (WireHub a, WireHub b, GameObject wireRendeder)		// Constructor
	{
		// Update this wire
		nodeA = a;
		nodeB = b;

		// Add the connection to both wire hubs
		if (a.AddConnection(this))
			b.AddConnection(this);

		// Set up line renderer
		lrHolder = GameObject.Instantiate(wireRendeder);
		lr = lrHolder.GetComponent<LineRenderer>();
	}

	public void RenderWire ()		// Calculate wire curve and render it
	{
		if (lastFrameRendered != Time.frameCount && nodeA && nodeB)		// If both nodes exist and the line has not already been rendered this frame...
		{
			// ... Record this frame as the last render frame. This will prevent this wire from being rendered more than once this frame
			lastFrameRendered = Time.frameCount;

			if (lr)
			{
				lr.SetVertexCount(2);
				Vector3[] positions = {nodeA.GetConnectorPosition, nodeB.GetConnectorPosition};
				lr.SetPositions(positions);
				// TODO: Calculate a line (with slack) from the two WireHub connector offset vectors
				// TODO: Render line from these two points
			}
		}
	}

	public void Remove ()		// Removes the wire from both connected nodes. The wire will later be cleaned up by garbage collection
	{
		if (nodeA)
			nodeA.RemoveConnection(this);

		if (nodeB)
			nodeB.RemoveConnection(this);

		// Remove the line renderer
		lr.enabled = false;
	}

	public static bool SameConnection (Wiring wA, Wiring wB)		// Determine if the two wires connect the same hubs
	{
		if (wA != null && wB != null)
		{
			if (wA.nodeA == wB.nodeA && wA.nodeB == wB.nodeB)
				return true;

			if (wA.nodeA == wB.nodeB && wA.nodeB == wB.nodeA)
				return true;
		}
		return false;
	}

	public WireHub OtherConnection (WireHub hub)		// Return the hub connected to hub
	{
		if (nodeA == hub)		// If the first node on this wire is the specified hub...
			return nodeB;		// ... Return the second node hub
		else if (nodeB == hub)		// If the second node on this wire is the specified hub...
			return nodeA;		// ... Return the first node hub
		else
		{
			// The specified hub is not actually connected via this wire, return null
			return null;
		}
	}

	public void DisableConnections()		// Removes node data
	{
		nodeA = null;
		nodeB = null;
	}
}
