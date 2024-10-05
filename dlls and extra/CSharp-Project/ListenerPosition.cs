using UnityEngine;

public class ListenerPosition : MonoBehaviour
{
	private const float FixedPositionZ = -1f;

	private Transform cachedTransform;

	private void Start()
	{
		cachedTransform = base.transform;
	}

	private void Update()
	{
		cachedTransform.localPosition = new Vector3(0f, 0f, cachedTransform.parent.transform.InverseTransformPoint(Vector3.zero).z + -1f);
	}
}
