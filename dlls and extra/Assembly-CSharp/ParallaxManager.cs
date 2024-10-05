using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
	public struct ParallaxCustomLayer
	{
		public GameObject layer;

		public float speedX;
	}

	protected GameObject[] m_backgroundLayerFurther;

	protected GameObject[] m_backgroundLayerFar;

	protected GameObject[] m_backgroundLayerNear;

	protected GameObject[] m_backgroundLayerSky;

	protected GameObject[] m_backgroundLayerForeground;

	protected GameObject[] m_backgroundLayerFixedFollowCamera;

	protected float m_fgLimitY;

	protected List<ParallaxCustomLayer> m_miscellanousLayer = new List<ParallaxCustomLayer>();

	protected Vector3 m_offset;

	protected Vector3 m_oldPosition;

	private void Start()
	{
		m_backgroundLayerFurther = GameObject.FindGameObjectsWithTag("ParallaxLayerFurther");
		m_backgroundLayerFar = GameObject.FindGameObjectsWithTag("ParallaxLayerFar");
		m_backgroundLayerNear = GameObject.FindGameObjectsWithTag("ParallaxLayerNear");
		m_backgroundLayerSky = GameObject.FindGameObjectsWithTag("ParallaxLayerSky");
		m_backgroundLayerFixedFollowCamera = GameObject.FindGameObjectsWithTag("ParallaxLayerFixedFollowCamera");
		int num = GameObject.FindGameObjectsWithTag("ParallaxLayerForeground").Length;
		m_backgroundLayerForeground = GameObject.FindGameObjectsWithTag("ParallaxLayerForeground");
		GameObject[] backgroundLayerForeground = m_backgroundLayerForeground;
		for (int i = 0; i < backgroundLayerForeground.Length; i++)
		{
			backgroundLayerForeground[i].AddComponent<BaseTransform>();
		}
		backgroundLayerForeground = m_backgroundLayerFar;
		for (int i = 0; i < backgroundLayerForeground.Length; i++)
		{
			backgroundLayerForeground[i].AddComponent<BaseTransform>();
		}
		backgroundLayerForeground = m_backgroundLayerNear;
		for (int i = 0; i < backgroundLayerForeground.Length; i++)
		{
			backgroundLayerForeground[i].AddComponent<BaseTransform>();
		}
		backgroundLayerForeground = m_backgroundLayerFurther;
		for (int i = 0; i < backgroundLayerForeground.Length; i++)
		{
			backgroundLayerForeground[i].AddComponent<BaseTransform>();
		}
		backgroundLayerForeground = m_backgroundLayerSky;
		for (int i = 0; i < backgroundLayerForeground.Length; i++)
		{
			backgroundLayerForeground[i].AddComponent<BaseTransform>();
		}
		backgroundLayerForeground = m_backgroundLayerFixedFollowCamera;
		for (int i = 0; i < backgroundLayerForeground.Length; i++)
		{
			backgroundLayerForeground[i].AddComponent<BaseTransform>();
		}
		foreach (ParallaxCustomLayer item in m_miscellanousLayer)
		{
			item.layer.AddComponent<BaseTransform>();
		}
		if (num > 0)
		{
			m_fgLimitY = m_backgroundLayerForeground[0].transform.position.y;
		}
		m_oldPosition = base.transform.position;
	}

	private void SetHorizontalPosition(GameObject[] objects, float scale)
	{
		foreach (GameObject gameObject in objects)
		{
			Vector3 position = gameObject.transform.position;
			position.x = gameObject.GetComponent<BaseTransform>().position.x + m_offset.x * scale;
			gameObject.transform.position = position;
		}
	}

	private void Update()
	{
		float num = base.transform.position.x - m_oldPosition.x;
		float num2 = base.transform.position.y - m_oldPosition.y;
		m_offset.x += num;
		if (num != 0f)
		{
			SetHorizontalPosition(m_backgroundLayerForeground, -0.4f);
			SetHorizontalPosition(m_backgroundLayerFurther, 0.7f);
			SetHorizontalPosition(m_backgroundLayerFar, 0.6f);
			SetHorizontalPosition(m_backgroundLayerNear, 0.4f);
			SetHorizontalPosition(m_backgroundLayerSky, 0.8f);
			SetHorizontalPosition(m_backgroundLayerFixedFollowCamera, 1f);
			for (int i = 0; i < m_miscellanousLayer.Count; i++)
			{
				ParallaxCustomLayer parallaxCustomLayer = m_miscellanousLayer[i];
				Vector3 position = parallaxCustomLayer.layer.transform.position;
				position.x = parallaxCustomLayer.layer.GetComponent<BaseTransform>().position.x + m_offset.x * parallaxCustomLayer.speedX;
				parallaxCustomLayer.layer.transform.position = position;
			}
		}
		if (num2 != 0f)
		{
			for (int j = 0; j < m_backgroundLayerForeground.Length; j++)
			{
				GameObject obj = m_backgroundLayerForeground[j];
				Vector3 position2 = obj.transform.position;
				if (position2.y <= m_fgLimitY)
				{
					position2 -= Vector3.up * num2 * 0.2f;
				}
				else
				{
					position2.y = m_fgLimitY;
				}
				obj.transform.position = position2;
			}
		}
		m_oldPosition = base.transform.position;
	}

	public void RegisterParallaxLayer(ParallaxCustomLayer pcl)
	{
		m_miscellanousLayer.Add(pcl);
		pcl.layer.AddComponent<BaseTransform>();
	}
}
