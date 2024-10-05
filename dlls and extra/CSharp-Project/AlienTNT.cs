using System.Collections;
using UnityEngine;

public class AlienTNT : TNT
{
	private float m_time;

	public override void Explode()
	{
		if (m_triggered)
		{
			return;
		}
		if (INSettings.GetBool(INFeature.AlienTNTNOTGate) && m_enclosedInto != null && m_enclosedInto.IsAlienMetalFrame() && m_time != 0f)
		{
			m_time = Time.time + 0.2f;
			return;
		}
		m_triggered = true;
		Collider[] array = Physics.OverlapSphere(base.transform.position, m_explosionRadius * INSettings.GetFloat(INFeature.AlienTNTExplosionRadius));
		foreach (Collider collider in array)
		{
			GameObject gameObject = FindParentWithRigidBody(collider.gameObject);
			if (gameObject != null)
			{
				int num = CountChildColliders(gameObject, 0);
				AddExplosionForce(gameObject, INSettings.GetFloat(INFeature.AlienTNTExplosionForce) / (float)num);
			}
			TNT component = collider.GetComponent<TNT>();
			if ((bool)component && !(component is AlienTNT) && (!INSettings.GetBool(INFeature.PartGenerator) || component.GeneratorRefCount <= 0))
			{
				component.Explode();
			}
			if (INSettings.GetBool(INFeature.AlienTNTTriggerGun))
			{
				ExplodingGrapplingHook component2 = collider.GetComponent<ExplodingGrapplingHook>();
				if ((bool)component2 && !component2.IsAutoGun() && (collider.transform.position - (base.transform.position + collider.transform.right)).sqrMagnitude < 0.78f)
				{
					StartCoroutine(TouchPart(component2, 0.2f));
				}
			}
		}
		Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.tntExplosion, base.transform.position);
		WPFMonoBehaviour.effectManager.CreateParticles(smokeCloud, base.transform.position - Vector3.forward * 5f, force: true);
		if ((bool)extraEffect)
		{
			WPFMonoBehaviour.effectManager.CreateParticles(extraEffect, base.transform.position - Vector3.forward * 4f, force: true);
		}
		CheckForTNTAchievement();
		StartCoroutine(ShineLight());
	}

	public override void OnCollisionEnter(Collision c)
	{
	}

	protected new virtual void LateUpdate()
	{
		m_triggered = false;
	}

	private void FixedUpdate()
	{
		if (!INSettings.GetBool(INFeature.AlienTNTNOTGate))
		{
			return;
		}
		if (!base.contraption || !base.contraption.IsRunning)
		{
			if (m_enclosedInto != null && m_enclosedInto.IsAlienMetalFrame() && m_time != 1f)
			{
				base.transform.Find("Visualization").GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 0.5f);
				m_time = 1f;
				return;
			}
			if (m_time == 1f)
			{
				base.transform.Find("Visualization").GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, 1f);
			}
			m_time = 0f;
		}
		else if (m_enclosedInto != null && m_enclosedInto.IsAlienMetalFrame() && Time.time > m_time + 0.2f)
		{
			m_time = 0f;
			Explode();
			m_time = Time.time;
		}
	}

	protected override IEnumerator ShineLight()
	{
		PointLightSource pls = GetComponentInChildren<PointLightSource>();
		if ((bool)pls)
		{
			pls.isEnabled = true;
			yield return new WaitForSeconds(pls.turnOnCurve[pls.turnOnCurve.length - 1].time);
			pls.isEnabled = false;
		}
	}

	private IEnumerator TouchPart(BasePart part, float time)
	{
		yield return new WaitForSeconds(time);
		part.ProcessTouch();
	}
}
