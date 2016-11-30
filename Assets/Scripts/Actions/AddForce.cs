using UnityEngine;
using System.Collections;

public class AddForce : MBAction {

	public Vector3 force = Vector3.zero;
	public ForceMode mode = ForceMode.Force;

	public override void Execute ()
	{
		Rigidbody rb = transform.GetComponent<Rigidbody>();
		if (rb)
			rb.AddForce(force, mode);
	}
}
