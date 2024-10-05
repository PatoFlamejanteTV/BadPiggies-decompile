using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNTBox : Goal
{
	public float m_explosionImpulse;

	public float m_explosionRadius;

	public float m_triggerSpeed;

	public GameObject m_smokeCloud;

	public GameObject m_explosionSpark;

	protected bool m_triggered;

	private Renderer m_renderer;

	private void OnCollisionEnter(Collision c)
	{
		if (c.relativeVelocity.magnitude > m_triggerSpeed)
		{
			Explode();
		}
	}

	private void Awake()
	{
		m_renderer = GetComponent<Renderer>();
	}

	public void Explode()
	{
		if (m_triggered)
		{
			return;
		}
		m_triggered = true;
		Collider[] array = Physics.OverlapSphere(base.transform.position, m_explosionRadius);
		foreach (Collider collider in array)
		{
			GameObject gameObject = FindParentWithRigidBody(collider.gameObject);
			if (gameObject != null)
			{
				int num = CountChildColliders(gameObject, 0);
				AddExplosionForce(gameObject, 1f / (float)num);
			}
			TNT component = collider.GetComponent<TNT>();
			if ((bool)component)
			{
				component.Explode();
			}
		}
		WPFMonoBehaviour.effectManager.CreateParticles(m_smokeCloud, base.transform.position - Vector3.forward * 12f, force: true);
		Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.tntExplosion, base.transform.position);
		CheckForTNTAchievement();
		StartCoroutine(ShineLight());
	}

	private int CountChildColliders(GameObject obj, int count)
	{
		if ((bool)obj.GetComponent<Collider>())
		{
			count++;
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			count = CountChildColliders(obj.transform.GetChild(i).gameObject, count);
		}
		return count;
	}

	private GameObject FindParentWithRigidBody(GameObject obj)
	{
		if ((bool)obj.GetComponent<Rigidbody>())
		{
			return obj;
		}
		if ((bool)obj.transform.parent)
		{
			return FindParentWithRigidBody(obj.transform.parent.gameObject);
		}
		return null;
	}

	private void AddExplosionForce(GameObject target, float forceFactor)
	{
		Vector3 vector = target.transform.position - base.transform.position;
		float f = Mathf.Max(vector.magnitude, 1f);
		float num = forceFactor * m_explosionImpulse / Mathf.Pow(f, 1.5f);
		Rigidbody component = target.GetComponent<Rigidbody>();
		if (component.mass < 0.1f)
		{
			num *= component.mass;
		}
		else if (component.mass < 0.4f)
		{
			num *= component.mass / 0.4f;
		}
		component.AddForce(num * vector.normalized, ForceMode.Impulse);
	}

	public void CheckForTNTAchievement()
	{
		if (!Singleton<SocialGameManager>.IsInstantiated())
		{
			return;
		}
		int brokenTNTs = GameProgress.GetInt("Broken_TNTs") + 1;
		GameProgress.SetInt("Broken_TNTs", brokenTNTs);
		foreach (string item in new List<string> { "grp.BOOM_BOOM_III", "grp.BOOM_BOOM_II", "grp.BOOM_BOOM_I" })
		{
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress(item, 100.0, (int limit) => brokenTNTs >= limit);
		}
	}

	private IEnumerator ShineLight()
	{
		PointLightSource pls = GetComponentInChildren<PointLightSource>();
		if ((bool)pls)
		{
			if (m_renderer != null)
			{
				m_renderer.enabled = false;
			}
			pls.onLightTurnOff = (Action)Delegate.Combine(pls.onLightTurnOff, (Action)delegate
			{
				base.gameObject.SetActive(value: false);
			});
			pls.isEnabled = true;
			yield return new WaitForSeconds(pls.turnOnCurve[pls.turnOnCurve.length - 1].time);
			pls.isEnabled = false;
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	protected override void OnGoalEnter(BasePart part)
	{
	}

	protected override void OnReset()
	{
		m_triggered = false;
		if (m_renderer != null)
		{
			m_renderer.enabled = true;
		}
		base.gameObject.SetActive(value: true);
	}
}
