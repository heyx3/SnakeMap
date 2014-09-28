using System;
using UnityEngine;


/// <summary>
/// Defines a point on a quadratic bezier curve.
/// </summary>
public class BezierPoint : MonoBehaviour
{
	/// <summary>
	/// An interpolated point on a Bezier curve, along with its horizontal perpendicular.
	/// </summary>
	public class BezierInterpolation
	{
		public Vector3 Pos, Perpendicular;
	}

	public static Vector3 Interpolate(Vector3 start, Vector3 control, Vector3 end, float t)
	{
		float oneMinusT = 1.0f - t;
		return (oneMinusT * oneMinusT * start) + (2.0f * oneMinusT * t * control) + (t * t * end);
	}
	public static BezierInterpolation InterpolateWithPerp(Vector3 start, Vector3 control, Vector3 end, float t)
	{
		BezierInterpolation ret = new BezierInterpolation();

		float oneMinusT = 1.0f - t;
		float tSquared = t * t;
		float twoXOneMinusT = 2.0f * oneMinusT;

		ret.Pos = (oneMinusT * oneMinusT * start) + (twoXOneMinusT * t * control) + (tSquared * end);
		ret.Perpendicular = (twoXOneMinusT * (control - start) + (2.0f * t * (end - control)));

		ret.Perpendicular = Vector3.Cross(ret.Perpendicular.normalized, new Vector3(0.0f, 1.0f, 0.0f));

		return ret;
	}

	
	public BezierPoint Previous = null,
					   Next = null;
	public float GizmoSphereRadius = 2.0f;


	public Transform MyTransform
	{
		get
		{
			if (tr == null) tr = transform;
			return tr;
		}
	}
	private Transform tr = null;

	/// <summary>
	/// Gets whether this is a point (as opposed to a control between two points).
	/// </summary>
	public bool IsPoint
	{
		get
		{
			if (Previous == null) return true;
			else return !Previous.IsPoint;
		}
	}
	/// <summary>
	/// Gets whether this is a control (as opposed to one of two points
	/// that surrounds either side of a control).
	/// </summary>
	public bool IsControl { get { return !IsPoint; } }


	void OnDrawGizmos()
	{
		//Draw this point as a sphere.
		//Use an "error" color if this is not part of a valid point-control-point triplet.
		if ((Previous == null && Next == null) || (Next == null && IsControl) ||
			(Next != null && Next.Next == null && IsPoint))
		{
			Gizmos.color = Color.magenta;
		}
		else if (IsPoint)
		{
			Gizmos.color = Color.grey;
		}
		else
		{
			Gizmos.color = Color.white;
		}
		Gizmos.DrawSphere(transform.position, GizmoSphereRadius);


		//If this is a valid start to a triplet, draw the curve from here to the next point.
		if (Gizmos.color != Color.magenta && Next != null &&
			Next.Next != null && IsPoint)
		{
			Gizmos.color = Color.white;
			Vector3 start = transform.position,
					control = Next.transform.position,
					end = Next.Next.transform.position;

			const int nSegments = 14;
			Vector3 previous = start;
			for (int i = 0; i < nSegments - 1; ++i)
			{
				BezierInterpolation endLine = InterpolateWithPerp(previous, control, end,
																 (i + 1.0f) / (nSegments - 1.0f));
				Gizmos.DrawLine(previous, endLine.Pos);
				Gizmos.DrawLine(endLine.Pos, endLine.Pos + (2.0f * endLine.Perpendicular));

				previous = endLine.Pos;
			}
			Gizmos.DrawLine(previous, end);
		}
	}
}