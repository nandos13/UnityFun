using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Locks the cursor when the game regains focus
 */

public class LockCursorOnClick : MonoBehaviour {

	void Start () 
	{
		
	}

	void Update () 
	{
		if (Time.timeScale > 0)
		{
			if (Input.GetMouseButtonDown(0))
				Cursor.lockState = CursorLockMode.Locked;
		}
	}
}
