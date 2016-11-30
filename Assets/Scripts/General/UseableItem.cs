using UnityEngine;
using System.Collections;

public abstract class UseableItem : MonoBehaviour {

	[HideInInspector] public bool use0 = false;
	[HideInInspector] public bool use1 = false;
	[HideInInspector] public bool use2 = false;

	public string itemName = "";

	public virtual void Use0() {}

	public virtual void Use1() {}

	public virtual void Use2() {}

	public virtual void Unequip() {}
}
