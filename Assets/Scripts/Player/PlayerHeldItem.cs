using UnityEngine;
using System.Collections;

public class PlayerHeldItem : MonoBehaviour {

	public UseableItem currentItem;

	void Start () 
	{
		
	}

	void Update () 
	{
		if (currentItem)		// If the player has an item equipped...
		{
			// ... Alter boolean states based on mouse buttons held down
			if (Input.GetMouseButton(0))
				currentItem.use0 = true;
			if (Input.GetMouseButton(1))
				currentItem.use1 = true;
			if (Input.GetMouseButton(2))
				currentItem.use2 = true;

			// Call functions based on mouse buttons clicked
			if (Input.GetMouseButtonDown(0))
				currentItem.Use0();
			if (Input.GetMouseButtonDown(1))
				currentItem.Use1();
			if (Input.GetMouseButtonDown(2))
				currentItem.Use2();
		}
	}
}
