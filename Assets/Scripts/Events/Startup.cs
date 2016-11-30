using UnityEngine;
using System.Collections;

public class Startup : MonoBehaviour {

	public MBAction[] actions;

	void Start () 
	{
		foreach (MBAction action in actions)
		{
			if (action)
				action.Execute();
		}
	}
}
