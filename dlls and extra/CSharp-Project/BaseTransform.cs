using UnityEngine;

public class BaseTransform : MonoBehaviour
{
	public Vector3 position;

	public Quaternion rotation;

	public Vector3 localScale;

	public void Awake()
	{
		position = base.transform.position;
		rotation = base.transform.rotation;
		localScale = base.transform.localScale;
	}
}
