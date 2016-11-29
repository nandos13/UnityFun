using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Added custom functionality for pre-existing Unity Engine classes.
 */

public static class Extensions {

	public static Transform Search (this Transform target, string transformName)
	{
		/* This function will search through all children of a specified transform
		 * and if a child with the specified name is found, returns that transform.
		 */
		return RecursiveSearch (target, transformName);
	}

	private static Transform RecursiveSearch (Transform target, string transformName)
	{
		/* Recursively searches through all children of a transform to find
		 * a child with the specified name.
		 */
		if (target.name == transformName)
			return target;
		foreach (Transform t in target) 
		{
			if (RecursiveSearch (t, transformName) != null)
				return RecursiveSearch (t, transformName);
		}
		return null;
	}

	private static void ListAddGetComponents<T> (Transform target, ref List<T> list)
	{
		/* Adds all components of type T contained in target to the List<T> list */
		if (target)
		{
			T[] targetsComponents = target.GetComponents<T> ();
			foreach (T comp in targetsComponents)
				list.Add (comp);
		}
	}

	public static RaycastHit ApplyTagFilter (this RaycastHit[] target, List<string> tags, bool ignoreMode = true)
	{
		/* Apply a tag mask to a list of RaycastHits and return the first
		 * instance of RaycastHit that should not be ignored, based on
		 * specified list "tags" and collision mode "ignoreMode"
		 */

		RaycastHit result = new RaycastHit();

		// Loop through each raycastHit in the array
		foreach (RaycastHit hit in target)
		{
			bool ignore = false;
			if (ignoreMode)
				ignore = true;

			foreach (string str in tags)
			{
				// Is the hit tag found in tags list?
				if (hit.transform.tag == str)
				{
					// Toggle bool ignore
					ignore = !ignore;
					break;
				}
			}

			// Is the hit being ignored?
			if (!ignore)
			{
				result = hit;
				break;
			}
		}

		// Return match
		return result;
	}

	public static RaycastHit ApplyTagFilter (this RaycastHit[] target, string[] tags, bool ignoreMode = true)
	{
		/* Apply a tag mask to a list of RaycastHits and return the first
		 * instance of RaycastHit that should not be ignored, based on
		 * specified list "tags" and collision mode "mode"
		 */

		RaycastHit result = new RaycastHit();

		// Loop through each raycastHit in the array
		foreach (RaycastHit hit in target)
		{
			bool ignore = false;
			if (ignoreMode)
				ignore = true;

			foreach (string str in tags)
			{
				// Is the hit tag found in tags list?
				if (hit.transform.tag == str)
				{
					// Toggle bool ignore
					ignore = !ignore;
					break;
				}
			}

			// Is the hit being ignored?
			if (!ignore)
			{
				result = hit;
				break;
			}
		}

		// Return match
		return result;
	}

	public static RaycastHit[] IgnoreChildren (this RaycastHit[] target, GameObject parent)
	{
		/* Return an array of RaycastHit that does not contain any
		 * children of the specified GameObject "parent"
		 */

		List<Transform> children = (parent.GetComponentsInChildren<Transform> ()).ToList ();

		List<RaycastHit> newHitsList = new List<RaycastHit> ();

		foreach (RaycastHit hit in target)
		{
			bool ignore = false;

			foreach (Transform go in children)
			{
				if (go == hit.transform)
				{
					ignore = true;
					break;
				}
			}

			if (!ignore)
				newHitsList.Add(hit);
		}

		return newHitsList.ToArray();
	}

	public static T[] ToArray<T> (this List<T> target)
	{
		/* Convert a List of type T to an array containing type T */

		T[] result = new T[target.Count];

		for (int i = 0; i < result.Length; i++)
		{
			if (i < target.Count)
				result[i] = target[i];
		}

		return result;
	}

	public static List<T> ToList<T> (this T[] target)
	{
		/* Convert an array containing type T to a List of type T */

		List<T> result = new List<T>();

		foreach (T data in target)
		{
			result.Add(data);
		}

		return result;
	}
}
