using System.Collections;
using UnityEngine;

public class PigMask : MonoBehaviour
{
	public GameObject[] m_masks;

	public float m_speedThreshold = 1f;

	private GameObject m_mask;

	private void Start()
	{
		if (base.transform.childCount > 0)
		{
			m_mask = base.transform.GetChild(0).gameObject;
		}
		if (m_mask == null && Singleton<GameManager>.IsInstantiated() && Singleton<GameManager>.Instance.GetGameState() == GameManager.GameState.Level && Singleton<GameManager>.Instance.CurrentEpisodeLabel.Equals("5"))
		{
			m_mask = Object.Instantiate(m_masks[Random.Range(0, m_masks.Length)], base.transform.position, base.transform.rotation);
			m_mask.transform.parent = base.transform;
		}
		if (m_mask != null && (bool)WPFMonoBehaviour.levelManager && WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Running)
		{
			StartCoroutine(FlyAway());
		}
	}

	private IEnumerator FlyAway()
	{
		while (WPFMonoBehaviour.levelManager.TimeElapsed <= 0f)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Vector3 lastPos = base.transform.position;
		float lastTime = Time.time;
		while (true)
		{
			yield return new WaitForSeconds(0.1f);
			float time = Time.time;
			float num = time - lastTime;
			if (Vector3.Distance(base.transform.position, lastPos) / num > m_speedThreshold)
			{
				break;
			}
			lastPos = base.transform.position;
			lastTime = time;
		}
		Transform obj = m_mask.transform.Find("Visualization");
		Quaternion rotation = obj.rotation;
		base.transform.parent = null;
		base.transform.localRotation = Quaternion.identity;
		obj.rotation = rotation;
		m_mask.GetComponent<Animation>().Play();
		yield return new WaitForSeconds(1.2f);
		Object.Destroy(base.gameObject);
	}
}
