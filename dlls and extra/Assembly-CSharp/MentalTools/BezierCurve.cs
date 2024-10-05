using System;
using UnityEngine;

namespace MentalTools;

[ExecuteInEditMode]
public class BezierCurve : MonoBehaviour
{
	public int bezierPointCount = 10;

	public bool loop;

	[SerializeField]
	private Bezier bezierCurve;

	[NonSerialized]
	private Transform tf;

	public Bezier Curve
	{
		get
		{
			return bezierCurve;
		}
		set
		{
			bezierCurve = value;
		}
	}

	public Transform CachedTf => tf;

	private void Awake()
	{
		tf = base.transform;
	}

	private void OnDataLoaded()
	{
		BezierMesh component = GetComponent<BezierMesh>();
		if (component != null)
		{
			component.CreateMesh();
		}
	}
}
