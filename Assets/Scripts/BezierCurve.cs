using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Generates a bezier path for a given collection of bezier points.
/// </summary>
public class BezierCurve
{
	public IEnumerable<Vector3> BezierPoints { get { return bezierPoints; } }
	private Vector3[] bezierPoints = new Vector3[0];

	public BezierCurve(List<BezierPoint> points)
	{
		bezierPoints = new Vector3[points.Count];
		for (int i = 0; i < points.Count; ++i)
			bezierPoints[i] = points[i].MyTransform.position;
	}
}