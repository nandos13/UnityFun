using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * 
 * This is a simple base class for other classes to inherit from.
 * 
 * Any child class of MBAcion will also have an Execute() function which can be used to
 * run code on particular events using the following syntax:
 * 
 * 		public class example : MBAction {
 * 		
 * 			public override void Execute ()
 * 			{
 * 				// CODE HERE
 * 			}
 * 		
 * 		}
 *
 * The OnDamage script, for example, can be used to run this code by
 * adding the MBAcion child script as the onDamageScript variable.
 * Doing this will call the scripts Execute () function when the object takes damage
 * 
 * 
 * Using this, different objects can react to events like taking damage differently, or
 * have multiple reactions to taking damage.
 */

public class MBAction : MonoBehaviour {

	// Execute can be called externally at any point
	public virtual void Execute () {}
}
