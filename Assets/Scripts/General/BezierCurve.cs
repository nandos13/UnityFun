using UnityEngine;
using System.Collections;

/* AUTHOR: 
 * Original code snippet found here: http://www.graphicfilth.com/2014/10/3d-bezier-curves-in-unity/
 * Modification & Optimization: Jake Perry
 * 
 * DESCRIPTION:
 * Simple class allowing a bezier curve between two points, using two control points. 
 * This does NOT support any more than two points. Code not written by me.
 */

[System.Serializable]
public class BezierCurve {

	public Vector3 p0;
	public Vector3 p1;
	public Vector3 p2;
	public Vector3 p3;

	// Declare floats to prevent memory allocation each frame
	private float u;
	private float tt;
	private float uu;
	private float uuu;
	private float ttt;
	private Vector3 temp = Vector3.zero;

	public BezierCurve (Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
	{
		p0 = v0;
		p1 = v1;
		p2 = v2;
		p3 = v3;
	}

	public BezierCurve ()
	{
		p0 = Vector3.zero;
		p1 = Vector3.zero;
		p2 = Vector3.zero;
		p3 = Vector3.zero;
	}

	public void SetPoints (Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
	{
		p0 = v0;
		p1 = v1;
		p2 = v2;
		p3 = v3;
	}

	public Vector3 GetPointAtTime (float t)
	{
		if (t > 1 || t < 0)
			Debug.LogWarning("Warning: BezierCurve.GetPointAtTime(t) parameter t should be between 0 and 1");
		
		float u = 1f - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vector3 p = uuu * p0;
		temp = (uu * t * p1);
		p += temp + temp + temp;
		temp = (u * tt * p2);
		p += temp + temp + temp;
		p += ttt * p3;

		return p;
	}
}
