using UnityEngine;

public class PartLevitation : WPFMonoBehaviour
{
	[SerializeField]
	private float force;

	[SerializeField]
	private float enclosedForce;

	private BasePart part;

	private void Awake()
	{
		part = GetComponent<BasePart>();
	}

	private void FixedUpdate()
	{
		if (base.rigidbody != null)
		{
			float num = ((!(part.enclosedInto == null)) ? enclosedForce : force);
			base.rigidbody.AddForce(Vector3.up * num, ForceMode.Force);
		}
	}
}
