using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WireHub : MBAction {

	public MBAction[] OnReceiveSignal;										// A list of actions to execute when this object receives a signal from any connected wires
	[Tooltip("Determines where connected wires will visually attach to when rendered")]
	public Vector3[] connectorPositions = new Vector3[0];					// Visual attach points for wires

	private List<Wiring> connections = new List<Wiring>();					// A list of connected wires
	private bool loopProtection = false;									// Prevents infinite loops from two hubs activating each other

	void Update ()
	{
		// Disable loop protection, allowing the hub to receive signals again
		loopProtection = false;

		// Render connected wires
		foreach (Wiring w in connections)
		{
			if (w != null)
				w.RenderWire();
		}
	}

	public Vector3 ClosestConnectorPos (Vector3 origin)		// Calculate the closest connector to the specified position
	{
		if (connectorPositions.Length == 0)
			return transform.position;
		else
		{
			Vector3 closest = transform.position;
			float closestDist = float.MaxValue;
			Vector3 worldSpacePos = new Vector3();
			for (int i = 0; i < connectorPositions.Length; i++)
			{
				worldSpacePos = OffsetPointWorldPos(i);
				if (Vector3.Distance (worldSpacePos, origin) < closestDist)		// If this position is closer to the parameter origin than the current closest Vector3...
				{
					// ... Store this position as the closest
					closest = worldSpacePos;
					closestDist = Vector3.Distance (worldSpacePos, origin);
				}
			}

			return closest;		// Return the closest vector to origin
		}
	}

	private Vector3 OffsetPointWorldPos (int i)		// Calculate the world position of a connection point, accounting for rotation
	{
		if (i < connectorPositions.Length)		// If the specified index exists in the array of positions...
		{
			// ... Use transform rotation and position to calculate world position
			return ((transform.rotation * connectorPositions[i]) + transform.position);
		}
		else
			return transform.position;		// The connector position does not exist
	}

	public override void Execute ()		// Activates the wire
	{
		Debug.Log("WireHub connected to " + transform.name + " outputting signal.");
		WireHub connectedHub;
		foreach (Wiring w in connections)
		{
			// Find the object attached to this hub via the Wiring "w"
			connectedHub = w.OtherConnection(this);

			if (connectedHub)		// If the hub exists...
				connectedHub.ReceiveSignal();		// Send a signal to that hub
		}
	}

	public void ReceiveSignal ()		// Execute all receive actions
	{
		if (!loopProtection)		// If the hub has not already received a signal this frame
		{
			Debug.Log("WireHub " + transform.name + " received a signal!");
			// Turn on loop protection
			loopProtection = true;

			// Execute receive actions
			foreach (MBAction action in OnReceiveSignal)
			{
				if (action)
					action.Execute();
			}
		}
	}

	public static bool IsConnected (WireHub hubA, WireHub hubB)		// Queries whether the two specified hubs are connected
	{
		foreach (Wiring w in hubA.connections)
		{
			if (w != null)		// Double check the wire exists to avoid null reference exceptions
			{
				if (w.OtherConnection(hubA) == hubB)		// If the wire is a connection from hubA to hubB...
				{
					// ... The specified hubs are connected. Return true
					return true;
				}
			}
		}

		return false;		// Not connected
	}

	public bool AddConnection(Wiring wire)		// Add a connection to this hub
	{
		foreach (Wiring w in connections)
		{
			if (Wiring.SameConnection(w, wire))		// If a wire is found with the same two nodes...
			{
				if (w == wire)		// ... Check if this is the same wire object
				{
					// The wire is already connected to this WireHub
				}
				else
				{
					// The wire is a duplicate of an already existing wire! Remove the wire's connections so it is handled by the garbage collector
					wire.DisableConnections();
				}

				return false;		// Wire was not added
			}
		}

		// If the code gets to this point, no matches were found. Add the wire
		connections.Add(wire);
		Debug.Log("WireHub connected to " + transform.name + " has been connected to another WireHub on " + wire.OtherConnection(this).transform.name);
		return true;		// Wire successfully added
	}

	public bool RemoveConnection(Wiring wire)		// Remove a connection from this hub
	{
		bool removed = false;
		foreach (Wiring w in connections)
		{
			if (Wiring.SameConnection(w, wire))		// If a wire is found with the same two nodes...
			{
				// ... Remove the wire
				removed = true;
				connections.Remove(w);
				w.Remove();
				break;
			}
		}

		return removed;		// Return whether or not any wires were removed
	}

	public bool RemoveConnection(WireHub hub)		// Remove a connection from this hub
	{
		bool removed = false;
		foreach (Wiring w in connections)
		{
			if (w.OtherConnection(this) == hub)		// If this wire is connected to the specified hub...
			{
				// ... Remove the wire
				removed = true;
				connections.Remove(w);
				w.Remove();
				break;
			}
		}

		return removed;		// Return whether or not any wires were removed
	}

	void OnDrawGizmosSelected ()
	{
		// Draw the attachment point
		Gizmos.color = Color.green;
		if (connectorPositions.Length > 0)
		{
			for (int i = 0; i < connectorPositions.Length; i++)
			{
				Gizmos.DrawWireSphere(OffsetPointWorldPos(i), 0.1f);
			}
		}
	}
}
