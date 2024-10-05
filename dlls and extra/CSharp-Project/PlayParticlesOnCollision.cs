using System.Collections.Generic;
using UnityEngine;

public class PlayParticlesOnCollision : WPFMonoBehaviour
{
	public enum CollisionEvent
	{
		Enter,
		Stay,
		Exit
	}

	public enum Action
	{
		PlayByEffectManager,
		ShowEffectByEffectManager,
		PlayParticleSystem,
		Instantinate,
		SpawnAudioOneShotEffect
	}

	private bool m_enabled = true;

	private float m_LastPlayTime;

	public List<Action> m_ActionList = new List<Action>();

	public GameObject m_ParticlesPrefab;

	public AudioSource m_AudioOneShotEffect;

	public LayerMask m_CollisionLayerMask;

	public float m_Timeout;

	public bool m_PlayOnEachContact;

	public CollisionEvent m_CollisionEvent;

	public float m_MinRelativeSpeed;

	public float m_ZOffset;

	public bool Enabled
	{
		get
		{
			return m_enabled;
		}
		set
		{
			m_LastPlayTime = 0f;
			m_enabled = value;
		}
	}

	private bool HandleContactPoint(ContactPoint cp, Collision coll)
	{
		bool result = false;
		if ((m_CollisionLayerMask.value & (1 << cp.otherCollider.gameObject.layer)) != 0)
		{
			if (Time.time - m_LastPlayTime >= m_Timeout && coll.relativeVelocity.magnitude > m_MinRelativeSpeed)
			{
				Vector3 point = cp.point;
				point.z += m_ZOffset;
				using (List<Action>.Enumerator enumerator = m_ActionList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						switch (enumerator.Current)
						{
						case Action.PlayByEffectManager:
							WPFMonoBehaviour.effectManager.CreateParticles(m_ParticlesPrefab, point);
							break;
						case Action.ShowEffectByEffectManager:
							WPFMonoBehaviour.effectManager.ShowBreakEffect(m_ParticlesPrefab, point, Quaternion.identity);
							break;
						case Action.PlayParticleSystem:
							m_ParticlesPrefab.GetComponent<ParticleSystem>().Play();
							break;
						case Action.Instantinate:
							Object.Instantiate(m_ParticlesPrefab, point, Quaternion.identity);
							break;
						case Action.SpawnAudioOneShotEffect:
							if ((bool)m_AudioOneShotEffect)
							{
								Singleton<AudioManager>.Instance.SpawnOneShotEffect(m_AudioOneShotEffect, point);
							}
							break;
						}
					}
				}
				m_LastPlayTime = Time.time;
			}
			result = !m_PlayOnEachContact;
		}
		return result;
	}

	private void HandleCollisionEvent(CollisionEvent collEvent, Collision coll)
	{
		if (!m_enabled || m_CollisionEvent != collEvent)
		{
			return;
		}
		ContactPoint[] contacts = coll.contacts;
		foreach (ContactPoint cp in contacts)
		{
			if (HandleContactPoint(cp, coll))
			{
				break;
			}
		}
	}

	public void OnCollisionEnter(Collision coll)
	{
		HandleCollisionEvent(CollisionEvent.Enter, coll);
	}

	public void OnCollisionStay(Collision coll)
	{
		HandleCollisionEvent(CollisionEvent.Stay, coll);
	}

	public void OnCollisionExit(Collision coll)
	{
		HandleCollisionEvent(CollisionEvent.Exit, coll);
	}
}
