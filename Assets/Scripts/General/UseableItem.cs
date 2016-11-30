using UnityEngine;
using System.Collections;

public abstract class UseableItem : MonoBehaviour {

	[HideInInspector] public bool use0 = false;
	[HideInInspector] public bool use1 = false;
	[HideInInspector] public bool use2 = false;

	public virtual void Use0() {}

	public virtual void Use1() {}

	public virtual void Use2() {}
}
