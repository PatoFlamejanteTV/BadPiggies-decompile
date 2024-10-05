using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
	private class ParticleManager
	{
		private GameObject m_parent;

		private int maxSystems = 5;

		private ParticleSystem m_prefab;

		private Queue<ParticleSystem> m_playing = new Queue<ParticleSystem>();

		private Queue<ParticleSystem> m_stopped = new Queue<ParticleSystem>();

		public ParticleManager(ParticleSystem prefab, GameObject parent)
		{
			m_parent = parent;
			m_prefab = prefab;
		}

		public void Update()
		{
			if (m_playing.Count > 0)
			{
				ParticleSystem particleSystem = m_playing.Peek();
				if (!particleSystem.isPlaying)
				{
					m_playing.Dequeue();
					m_stopped.Enqueue(particleSystem);
				}
			}
		}

		public void CreateParticles(Vector3 position, bool force = false)
		{
			if (m_stopped.Count > 0)
			{
				ParticleSystem particleSystem = m_stopped.Dequeue();
				particleSystem.transform.position = position;
				particleSystem.time = 0f;
				particleSystem.Play();
				m_playing.Enqueue(particleSystem);
			}
			else if (m_playing.Count < maxSystems || force)
			{
				ParticleSystem component = Object.Instantiate(m_prefab.gameObject, position, Quaternion.identity).GetComponent<ParticleSystem>();
				component.transform.parent = m_parent.transform;
				m_playing.Enqueue(component);
			}
		}
	}

	private GameObject m_snapSprite;

	private GameObject m_krakSprite;

	private GameObject m_bangSprite;

	private GameData gameData;

	private Dictionary<ParticleSystem, ParticleManager> m_particles = new Dictionary<ParticleSystem, ParticleManager>();

	private IEnumerator<ParticleManager> m_particleValues;

	private void Awake()
	{
		gameData = Singleton<GameManager>.Instance.gameData;
		m_snapSprite = Object.Instantiate(gameData.m_snapSprite);
		m_snapSprite.GetComponent<Renderer>().enabled = false;
		m_krakSprite = Object.Instantiate(gameData.m_krakSprite);
		m_krakSprite.GetComponent<Renderer>().enabled = false;
		m_bangSprite = Object.Instantiate(gameData.m_bangSprite);
		m_bangSprite.GetComponent<Renderer>().enabled = false;
		m_particleValues = m_particles.Values.GetEnumerator();
	}

	private void Update()
	{
		m_particleValues.Reset();
		while (m_particleValues.MoveNext())
		{
			m_particleValues.Current.Update();
		}
	}

	public void CreateParticles(GameObject prefab, Vector3 position, bool force = false)
	{
		CreateParticles(prefab.GetComponent<ParticleSystem>(), position, force);
	}

	public void CreateParticles(ParticleSystem prefab, Vector3 position, bool force = false)
	{
		GetParticleManager(prefab).CreateParticles(position, force);
	}

	public void ShowBreakEffect(GameObject sprite, Vector3 position, Quaternion rotation)
	{
		if (sprite == gameData.m_snapSprite)
		{
			if (!m_snapSprite.GetComponent<Renderer>().enabled)
			{
				m_snapSprite.transform.position = position;
				m_snapSprite.transform.rotation = rotation;
				m_snapSprite.GetComponent<TimedHide>().Show();
			}
		}
		else if (sprite == gameData.m_krakSprite)
		{
			if (!m_krakSprite.GetComponent<Renderer>().enabled)
			{
				m_krakSprite.transform.position = position;
				m_krakSprite.transform.rotation = rotation;
				m_krakSprite.GetComponent<TimedHide>().Show();
			}
		}
		else if (sprite == gameData.m_bangSprite && !m_bangSprite.GetComponent<Renderer>().enabled)
		{
			m_bangSprite.transform.position = position;
			m_bangSprite.transform.rotation = rotation;
			m_bangSprite.GetComponent<TimedHide>().Show();
		}
	}

	private ParticleManager GetParticleManager(ParticleSystem prefab)
	{
		if (m_particles.TryGetValue(prefab, out var value))
		{
			return value;
		}
		value = new ParticleManager(prefab, base.gameObject);
		m_particles[prefab] = value;
		m_particleValues = m_particles.Values.GetEnumerator();
		return value;
	}
}
