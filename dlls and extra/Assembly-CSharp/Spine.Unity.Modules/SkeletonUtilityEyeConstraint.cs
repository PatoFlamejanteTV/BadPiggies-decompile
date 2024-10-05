using UnityEngine;

namespace Spine.Unity.Modules;

public class SkeletonUtilityEyeConstraint : SkeletonUtilityConstraint
{
	public Transform[] eyes;

	public float radius = 0.5f;

	public Transform target;

	public Vector3 targetPosition;

	public float speed = 10f;

	private Vector3[] origins;

	private Vector3 centerPoint;

	protected override void OnEnable()
	{
		if (Application.isPlaying)
		{
			base.OnEnable();
			Bounds bounds = new Bounds(eyes[0].localPosition, Vector3.zero);
			origins = new Vector3[eyes.Length];
			for (int i = 0; i < eyes.Length; i++)
			{
				origins[i] = eyes[i].localPosition;
				bounds.Encapsulate(origins[i]);
			}
			centerPoint = bounds.center;
		}
	}

	protected override void OnDisable()
	{
		if (Application.isPlaying)
		{
			base.OnDisable();
		}
	}

	public override void DoUpdate()
	{
		if (target != null)
		{
			targetPosition = target.position;
		}
		Vector3 vector = targetPosition;
		Vector3 vector2 = base.transform.TransformPoint(centerPoint);
		Vector3 vector3 = vector - vector2;
		if (vector3.magnitude > 1f)
		{
			vector3.Normalize();
		}
		for (int i = 0; i < eyes.Length; i++)
		{
			vector2 = base.transform.TransformPoint(origins[i]);
			eyes[i].position = Vector3.MoveTowards(eyes[i].position, vector2 + vector3 * radius, speed * Time.deltaTime);
		}
	}
}
