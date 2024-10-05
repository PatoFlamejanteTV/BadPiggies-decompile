using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
	public float speed = 90f;

	private float lastRealtimeSinceStartup;

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = Time.realtimeSinceStartup - lastRealtimeSinceStartup;
		lastRealtimeSinceStartup = realtimeSinceStartup;
		base.transform.Rotate(Vector3.back, num * speed);
	}
}
