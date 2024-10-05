using System.Collections.Generic;
using UnityEngine;

public class SuperMagnet : WPFMonoBehaviour
{
	public float m_radius = 5f;

	public float m_speedMin = 3f;

	public float m_speedMax = 8f;

	private List<GameObject> m_collisions;

	private float m_lastTimeCheckCollisions;

	private void Start()
	{
		m_lastTimeCheckCollisions = 0f;
		m_collisions = new List<GameObject>(16);
		if (base.transform.Find(WPFMonoBehaviour.gameData.m_superMagnetEffect.name) == null)
		{
			GameObject obj = Object.Instantiate(WPFMonoBehaviour.gameData.m_superMagnetEffect, base.transform.position, base.transform.rotation);
			obj.name = WPFMonoBehaviour.gameData.m_superMagnetEffect.name;
			obj.transform.parent = base.transform;
		}
	}

	private void OnDestroy()
	{
		Transform transform = base.transform.Find(WPFMonoBehaviour.gameData.m_superMagnetEffect.name);
		if ((bool)transform)
		{
			Object.Destroy(transform.gameObject);
		}
	}

	private void Update()
	{
		if (WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.Running || !(Time.time - m_lastTimeCheckCollisions > 0.5f))
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, m_radius);
		foreach (Collider collider in array)
		{
			if (((bool)collider.GetComponent<OneTimeCollectable>() || (bool)collider.GetComponent<Collectable>()) && collider.GetComponent<SecretPlace>() == null && collider.GetComponent<SecretStatue>() == null && !m_collisions.Contains(collider.gameObject))
			{
				m_collisions.Add(collider.gameObject);
			}
		}
		m_lastTimeCheckCollisions = Time.time;
	}

	private void FixedUpdate()
	{
		if (WPFMonoBehaviour.levelManager == null || WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.Running || m_collisions == null)
		{
			return;
		}
		for (int i = 0; i < m_collisions.Count; i++)
		{
			GameObject gameObject = m_collisions[i];
			if (!(gameObject == null))
			{
				Vector3 vector = base.transform.position - gameObject.transform.position;
				float num = Vector3.Distance(base.transform.position, gameObject.transform.position);
				float num2 = Mathf.Lerp(m_speedMax, m_speedMin, num / m_radius);
				gameObject.transform.position += Time.fixedDeltaTime * num2 * vector.normalized;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, m_radius);
	}
}
