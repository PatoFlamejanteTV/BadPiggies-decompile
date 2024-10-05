using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyRotationConstraints : MonoBehaviour
{
	private Rigidbody rb;

	private Quaternion startRotation;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		startRotation = base.transform.localRotation;
	}

	private void LateUpdate()
	{
		byte b = (byte)rb.constraints;
		Quaternion localRotation = new Quaternion(((b & 0x10) == 0) ? base.transform.localRotation.x : startRotation.x, ((b & 0x20) == 0) ? base.transform.localRotation.y : startRotation.y, ((b & 0x40) == 0) ? base.transform.localRotation.z : startRotation.z, base.transform.localRotation.w);
		base.transform.localRotation = localRotation;
	}
}
