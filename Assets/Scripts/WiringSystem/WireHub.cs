using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WireHub : MBAction {

	public MBAction[] OnReceiveSignal;										// A list of actions to execute when this object receives a signal from any connected wires
	[Tooltip("Determines where connected wires will visually attach to when rendered")]
	public Vector3 connectorOffset;											// Visual attach point for wires
	public Vector3 GetConnectorPosition
	{
		// Property returns world coords of the connection offset
		get { return (connectorOffset + transform.position); }
	}

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
			}
		}

		return removed;		// Return whether or not any wires were removed
	}

	void OnDrawGizmosSelected ()
	{
		// Draw the attachment point
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere((transform.position + connectorOffset), 0.1f);
	}
}
