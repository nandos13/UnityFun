using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHeldItem : MonoBehaviour {

	public Text currentItemText;
	public UseableItem[] heldItems;
	[HideInInspector] public bool enableScrollChange = true;

	private UseableItem currentItem;
	private int currentIndex = 0;

	void Start () 
	{
		// Equip the first held item
		if (heldItems.Length > 0)
			currentItem = heldItems[0];
	}

	void Update () 
	{
		// Check for weapon changes
		if (enableScrollChange)
		{
			int newWeapon = currentIndex;
			if (Input.GetAxis("Mouse ScrollWheel") < 0)
				newWeapon++;
			else if (Input.GetAxis("Mouse ScrollWheel") > 0)
				newWeapon--;

			// Only allow a maximum change of 1
			newWeapon = Mathf.Clamp(newWeapon, currentIndex - 1, currentIndex + 1);
			// Ensure the new index will not link to inaccessible memory
			newWeapon = Mathf.Clamp(newWeapon, 0, heldItems.Length - 1);

			if (newWeapon != currentIndex)		// If the weapon was changed...
			{
				// ... Unequip the current item
				ButtonsUp();

				// Change weapon
				currentIndex = newWeapon;
				currentItem = heldItems[currentIndex];
			}
		}

		// Use current item
		if (currentItem)		// If the player has an item equipped...
		{
			// ... Alter boolean states based on mouse buttons held down
			currentItem.use0 = Input.GetMouseButton(0);
			currentItem.use1 = Input.GetMouseButton(1);
			currentItem.use2 = Input.GetMouseButton(2);

			// Call functions based on mouse buttons clicked
			if (Input.GetMouseButtonDown(0))
				currentItem.Use0();
			if (Input.GetMouseButtonDown(1))
				currentItem.Use1();
			if (Input.GetMouseButtonDown(2))
				currentItem.Use2();

			// Update HUD text
			if (currentItemText)
			{
				currentItemText.text = "Currently holding: " + currentItem.itemName;
			}
		}
		else
		{
			// Update HUD text
			if (currentItemText)
			{
				currentItemText.text = "";
			}
		}
	}

	private void ButtonsUp()		// Set all booleans tracking button state to false
	{
		currentItem.use0 = false;
		currentItem.use1 = false;
		currentItem.use2 = false;

		currentItem.Unequip();
	}
}
