using UnityEngine;
using System.Collections;

public class EnableComponent : MBAction {

	public bool state;
	public MonoBehaviour component;

	public override void Execute ()
	{
		if (component)
			component.enabled = state;
	}
}
