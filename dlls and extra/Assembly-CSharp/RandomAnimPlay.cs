using System.Collections;
using UnityEngine;

public class RandomAnimPlay : MonoBehaviour
{
	public string animationName;

	public float minTimeout = 2f;

	public float maxTimeout = 5f;

	private void Start()
	{
		StartCoroutine(PlayAnimation());
	}

	private IEnumerator PlayAnimation()
	{
		while (true)
		{
			yield return new WaitForSeconds(minTimeout + Random.value * (maxTimeout - minTimeout));
			if (animationName != null && !GetComponent<Animation>().IsPlaying(animationName))
			{
				GetComponent<Animation>().Play(animationName);
			}
		}
	}
}
