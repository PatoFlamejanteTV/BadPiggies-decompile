using System.Collections;
using UnityEngine;

public class SnoutCoinSingle : WPFMonoBehaviour
{
	private const string snoutCoinSinglePrefabPath = "UI/SnoutCoinSingle";

	private static GameObject snoutCoinSinglePrefab;

	[SerializeField]
	private float lifeTime = 2f;

	[SerializeField]
	private float speed = 2f;

	public static void Spawn(Vector3 position, float delay = 0f)
	{
		if (snoutCoinSinglePrefab == null)
		{
			snoutCoinSinglePrefab = Resources.Load<GameObject>("UI/SnoutCoinSingle");
		}
		if (!(snoutCoinSinglePrefab == null))
		{
			if (delay > 0.01f)
			{
				CoroutineRunner.Instance.StartCoroutine(SpawnDelay(position, delay));
			}
			else
			{
				Object.Instantiate(snoutCoinSinglePrefab, position, Quaternion.identity);
			}
		}
	}

	private static IEnumerator SpawnDelay(Vector3 position, float delay)
	{
		yield return new WaitForSeconds(delay);
		Spawn(position);
	}

	private void Start()
	{
		Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinHit, Vector3.zero);
	}

	private void Update()
	{
		lifeTime -= Time.deltaTime;
		base.transform.position += Vector3.up * speed * Time.deltaTime;
		base.transform.localScale = Vector3.Lerp(Vector3.one * 0.01f, Vector3.one, Mathf.Clamp01(lifeTime * 2f));
		if (lifeTime <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
