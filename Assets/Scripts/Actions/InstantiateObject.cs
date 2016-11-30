using UnityEngine;
using System.Collections;

public class InstantiateObject : MBAction {

	public GameObject obj;
	public Vector3 spawnOffset = Vector3.zero;
	public Vector3 spawnRotation = Vector3.zero;

	public override void Execute ()
	{
		if (obj)
		{
			// Instantiate a new object
			GameObject newObj = (GameObject)Instantiate(obj, (transform.position + spawnOffset), Quaternion.Euler(spawnRotation)) as GameObject;
		}
	}

	void OnDrawGizmosSelected ()
	{
		// Draw a sphere at the spawn offset
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position + spawnOffset, 0.2f);
	}
}
