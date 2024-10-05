using UnityEngine;

public class JunkPrize : MonoBehaviour
{
	private void Start()
	{
		Vector3 up = Vector3.up;
		up = Quaternion.AngleAxis(((Random.value >= 0.5f) ? (-1f) : 1f) * Random.Range(30f, 60f), Vector3.forward) * up;
		Vector3 force = up * 15f;
		Rigidbody component = GetComponent<Rigidbody>();
		component.AddForce(force, ForceMode.Impulse);
		component.AddTorque(new Vector3(0f, 0f, 5f), ForceMode.Impulse);
		Object.Destroy(base.gameObject, 3f);
	}
}
