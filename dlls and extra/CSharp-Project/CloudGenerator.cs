using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : WPFMonoBehaviour
{
	public List<Transform> m_cloudSet = new List<Transform>();

	public int m_maxClouds = 10;

	public float m_cloudVelocity = 10f;

	public float m_cloudLimits = 1000f;

	public float m_farPlane = 100f;

	public float m_height = 100f;

	public bool useCustomParallax = true;

	protected List<Transform> m_currentClouds = new List<Transform>();

	private void Start()
	{
		if (useCustomParallax)
		{
			ParallaxManager.ParallaxCustomLayer pcl = default(ParallaxManager.ParallaxCustomLayer);
			pcl.layer = base.gameObject;
			pcl.speedX = 1f;
			(Object.FindObjectOfType(typeof(ParallaxManager)) as ParallaxManager).RegisterParallaxLayer(pcl);
		}
		for (int i = 0; i < m_maxClouds; i++)
		{
			SpawnCloud();
		}
	}

	private void SpawnCloud()
	{
		Vector3 position = new Vector3(Random.Range(0f - m_cloudLimits, m_cloudLimits), Random.Range(0f - m_height, m_height), Random.Range(0f, m_farPlane));
		position += base.transform.position;
		Transform transform = Object.Instantiate(m_cloudSet[Random.Range(0, m_cloudSet.Count)], position, Quaternion.identity);
		CloudMover component = transform.GetComponent<CloudMover>();
		component.m_velocity = m_cloudVelocity;
		component.m_limits = m_cloudLimits;
		m_currentClouds.Add(transform);
		transform.parent = base.transform;
	}

	public void OnDrawGizmosSelected()
	{
		Color blue = Color.blue;
		blue.a = 0.2f;
		Gizmos.color = blue;
		Vector3 center = base.transform.position + Vector3.forward * m_farPlane * 0.5f;
		Vector3 size = new Vector3(m_cloudLimits * 2f, m_height * 2f, m_farPlane);
		Gizmos.DrawCube(center, size);
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(center, size);
	}
}
