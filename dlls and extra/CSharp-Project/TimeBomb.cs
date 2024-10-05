using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBomb : BasePart
{
	public class BombOutOfBounds : EventManager.Event
	{
	}

	[SerializeField]
	private float m_explosionImpulse;

	[SerializeField]
	private float m_explosionRadius;

	[SerializeField]
	private float m_triggerSpeed;

	[SerializeField]
	private GameObject smokeCloudPrefab;

	private GameObject m_Visualization;

	[SerializeField]
	private bool m_checkRotation;

	private bool m_triggered;

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override bool ValidatePart()
	{
		return true;
	}

	public override void Awake()
	{
		base.Awake();
		m_Visualization = base.transform.Find("Visualization").gameObject;
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
	}

	public override void Initialize()
	{
		base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), 1);
		EventManager.Connect<GameStateChanged>(OnGameStateChanged);
		CheckRotations();
	}

	private void OnGameStateChanged(GameStateChanged data)
	{
		if (data.state == LevelManager.GameState.CakeRaceCompleted)
		{
			EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
			Explode();
		}
	}

	protected override void OnTouch()
	{
		Explode();
	}

	public void Explode()
	{
		if (m_triggered)
		{
			return;
		}
		m_triggered = true;
		base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), -1);
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
			if ((bool)component && (!INSettings.GetBool(INFeature.PartGenerator) || component.GeneratorRefCount <= 0))
			{
				component.Explode();
			}
		}
		Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.tntExplosion, base.transform.position);
		WPFMonoBehaviour.effectManager.CreateParticles(smokeCloudPrefab, base.transform.position - Vector3.forward * 12f, force: true);
		CheckForAchievements();
		base.contraption.RemovePart(this);
		List<Joint> list = base.contraption.FindPartJointsFast(this);
		if (list.Count > 0)
		{
			for (int j = 0; j < list.Count; j++)
			{
				bool flag = list[j].gameObject == this || list[j].connectedBody == this;
				if (!float.IsInfinity(list[j].breakForce) || flag)
				{
					UnityEngine.Object.Destroy(list[j]);
				}
			}
			HandleJointBreak();
		}
		else
		{
			HandleJointBreak(playEffects: false);
		}
		StartCoroutine(ShineLight());
		EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
		EventManager.Send(default(TimeBombExplodeEvent));
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
		Pig component2 = target.GetComponent<Pig>();
		if ((bool)component2)
		{
			component2.PrepareForTNT(base.transform.position, num);
			num *= 1.15f;
		}
		component.AddForce(num * vector.normalized, ForceMode.Impulse);
	}

	public void CheckForAchievements()
	{
		if (Singleton<SocialGameManager>.IsInstantiated())
		{
			Singleton<GameManager>.Instance.IsInGame();
		}
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		LevelManager.CameraLimits currentCameraLimits = WPFMonoBehaviour.levelManager.CurrentCameraLimits;
		if (position.x > currentCameraLimits.topLeft.x + currentCameraLimits.size.x * 1.1f || position.x < currentCameraLimits.topLeft.x - currentCameraLimits.size.x * 0.1f)
		{
			EventManager.Send(new BombOutOfBounds());
		}
	}

	private IEnumerator ShineLight()
	{
		PointLightSource pls = GetComponentInChildren<PointLightSource>();
		if ((bool)pls)
		{
			MeshRenderer componentInChildren = base.transform.GetComponentInChildren<MeshRenderer>();
			if ((bool)componentInChildren)
			{
				componentInChildren.enabled = false;
			}
			pls.onLightTurnOff = (Action)Delegate.Combine(pls.onLightTurnOff, (Action)delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			pls.isEnabled = true;
			yield return new WaitForSeconds(pls.turnOnCurve[pls.turnOnCurve.length - 1].time);
			pls.isEnabled = false;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public override void SetRotation(GridRotation rotation)
	{
		m_gridRotation = rotation;
		m_Visualization.transform.localRotation = Quaternion.AngleAxis(GetRotationAngle(rotation), Vector3.forward);
		CheckRotations();
	}

	private void FlipRotation(Transform target, bool flipX, bool flipY)
	{
		Vector3 localScale = target.localScale;
		if (flipX)
		{
			localScale.x = 0f - Mathf.Abs(localScale.x);
		}
		else
		{
			localScale.x = Mathf.Abs(localScale.x);
		}
		if (flipY)
		{
			localScale.y = 0f - Mathf.Abs(localScale.y);
		}
		else
		{
			localScale.y = Mathf.Abs(localScale.y);
		}
		target.localScale = localScale;
	}

	private void CheckRotations()
	{
		if (m_checkRotation && m_gridRotation == GridRotation.Deg_90)
		{
			FlipRotation(m_Visualization.transform, flipX: true, flipY: false);
		}
		else if (m_checkRotation)
		{
			FlipRotation(m_Visualization.transform, flipX: false, flipY: false);
		}
	}
}
