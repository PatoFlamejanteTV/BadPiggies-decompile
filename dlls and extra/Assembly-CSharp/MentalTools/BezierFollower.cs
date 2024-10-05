using UnityEngine;

namespace MentalTools;

public class BezierFollower : MonoBehaviour
{
	public BezierCurve bezier;

	public float step = 0.01f;

	private float t;

	private Transform tf;

	private Vector3 targetPosition = Vector3.zero;

	private void Start()
	{
		tf = base.transform;
		if (!(bezier == null))
		{
			targetPosition = bezier.Curve.GetPoint(0f, bezier.loop, bezier.CachedTf.position);
			tf.position = targetPosition;
		}
	}

	private void Update()
	{
		if (bezier == null)
		{
			return;
		}
		while (Vector3.Distance(tf.position, targetPosition) <= step)
		{
			t += Time.deltaTime;
			if (t > 1f)
			{
				t = 0f;
			}
			targetPosition = bezier.Curve.GetPoint(t, bezier.loop, bezier.CachedTf.position);
		}
		tf.position = Vector3.MoveTowards(tf.position, targetPosition, step);
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(tf.position, targetPosition);
		}
	}
}
