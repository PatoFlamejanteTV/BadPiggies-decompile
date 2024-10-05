using UnityEngine;

public class RandomRotation : MonoBehaviour
{
	[SerializeField]
	private float maxRotation;

	[SerializeField]
	private float minRotation;

	private float rotationVelocity;

	private Vector3 currentRotation;

	private void Awake()
	{
		rotationVelocity = Random.Range(minRotation, maxRotation);
		currentRotation = base.transform.rotation.eulerAngles;
	}

	private void Update()
	{
		currentRotation.z += rotationVelocity * Time.unscaledDeltaTime;
		base.transform.rotation = Quaternion.Euler(currentRotation);
	}
}
