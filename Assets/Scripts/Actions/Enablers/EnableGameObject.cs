using UnityEngine;
using System.Collections;

public class EnableGameObject : MBAction {

	public bool state;
	public GameObject component;

	public override void Execute ()
	{
		Debug.Log("test");
		if (component)
			component.SetActive(state);
	}
}
