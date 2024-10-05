using System.Collections;
using UnityEngine;

public class TimedDestructor : MonoBehaviour
{
	[SerializeField]
	private float lifeTime = 1f;

	private void Start()
	{
		StartCoroutine(DoDestroyCountdown());
	}

	private IEnumerator DoDestroyCountdown()
	{
		yield return new WaitForSeconds(lifeTime);
		Object.Destroy(base.gameObject);
	}
}
