using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interaction : MBAction {

	public string hoverMessage = "";
	public List<MBAction> actions = new List<MBAction> ();

	void Start ()
	{
		Collider c = GetComponent<Collider>();
		if (!c)
			Debug.LogWarning("An interaction script was attached to object: " + transform.name + " which does not have a collider. A collider is requred for an interaction script to work properly");
	}

	public override void Execute ()
	{
		foreach (MBAction action in actions)
		{
			if (action)
				action.Execute();
		}
	}
}
