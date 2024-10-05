using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : BasePropulsion
{
	public Vector3 m_direction = Vector3.up;

	public bool m_enabled;

	public bool m_explodes;

	public float m_boostForce = 10f;

	public float m_ignitionTime = 1f;

	public float m_boostDuration = 2f;

	public float m_boostEndDuration = 0.5f;

	public float m_maximumSpeed;

	public float m_explosionImpulse;

	public float m_explosionRadius;

	public float m_loopAudioTimeLimit;

	public Transform m_particlesIgnition;

	public Transform m_particlesFiring;

	public Transform m_particlesSmoke;

	public AudioSource m_loopAudio;

	public AudioSource m_launchAudio;

	public AudioSource m_shakeAudio;

	public GameObject m_smokeCloud;

	protected bool m_boostUsed;

	protected Vector3 m_origScale;

	protected float m_timeBoostStarted;

	protected float m_currentScale;

	protected bool m_firedRocket;

	public ParticleSystem m_particlesIgnitionInstance;

	public ParticleSystem m_particlesFiringInstance;

	private GameObject m_leftAttachment;

	private GameObject m_rightAttachment;

	private GameObject m_topAttachment;

	private GameObject m_bottomAttachment;

	private GameObject m_visualization;

	private GameObject m_loopAudioObject;

	private AudioSource m_launchAudioInstance;

	public Renderer m_content;

	public Renderer m_content2;

	private float m_currentAlpha;

	private float m_currentAlpha2;

	private PointLightSource pls;

	private BasePart m_targetPart;

	private float m_angle;

	public override bool IsEnabled()
	{
		if (INSettings.GetBool(INFeature.SwitchableCokeSodaRocket))
		{
			return m_enabled;
		}
		return Time.time - m_timeBoostStarted < m_ignitionTime + m_boostDuration + m_boostEndDuration;
	}

	public override Direction EffectDirection()
	{
		return BasePart.Rotate(Direction.Right, m_gridRotation);
	}

	public override void Awake()
	{
		base.Awake();
		m_leftAttachment = base.transform.Find("LeftAttachment").gameObject;
		m_rightAttachment = base.transform.Find("RightAttachment").gameObject;
		m_topAttachment = base.transform.Find("TopAttachment").gameObject;
		m_bottomAttachment = base.transform.Find("BottomAttachment").gameObject;
		m_leftAttachment.SetActive(value: false);
		m_rightAttachment.SetActive(value: false);
		m_topAttachment.SetActive(value: false);
		m_bottomAttachment.SetActive(value: false);
		Transform transform = base.transform.Find("BottleVisualization");
		if ((bool)transform)
		{
			m_visualization = transform.gameObject;
		}
		m_origScale = base.transform.localScale;
		m_timeBoostStarted = -1000f;
		m_boostUsed = false;
		m_currentAlpha = 1f;
		m_currentAlpha2 = 1f;
		m_enabled = false;
		if ((bool)m_content2)
		{
			m_content2.material.color = new Color(0f, 0f, 0f, 0f);
		}
	}

	private void OnDestroy()
	{
		if ((bool)m_loopAudioObject)
		{
			Singleton<AudioManager>.Instance.StopLoopingEffect(m_loopAudioObject.GetComponent<AudioSource>());
			UnityEngine.Object.Destroy(m_loopAudioObject);
		}
	}

	public override void ChangeVisualConnections()
	{
		if (INSettings.GetBool(INFeature.EnclosableParts) && m_enclosedInto != null)
		{
			m_leftAttachment.SetActive(value: false);
			m_rightAttachment.SetActive(value: false);
			m_topAttachment.SetActive(value: false);
			m_bottomAttachment.SetActive(value: false);
			return;
		}
		bool flag = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Up, m_gridRotation));
		bool flag2 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Down, m_gridRotation));
		bool flag3 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Left, m_gridRotation));
		bool flag4 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Right, m_gridRotation));
		m_leftAttachment.SetActive(flag3);
		m_rightAttachment.SetActive(flag4);
		m_topAttachment.SetActive(flag);
		m_bottomAttachment.SetActive(flag2 || (!flag && !flag3 && !flag4));
		m_leftAttachment.GetComponent<BoxCollider>().isTrigger = !m_leftAttachment.activeInHierarchy;
		m_rightAttachment.GetComponent<BoxCollider>().isTrigger = !m_rightAttachment.activeInHierarchy;
		m_topAttachment.GetComponent<BoxCollider>().isTrigger = !m_topAttachment.activeInHierarchy;
		m_bottomAttachment.GetComponent<BoxCollider>().isTrigger = !m_bottomAttachment.activeInHierarchy;
	}

	public override GridRotation AutoAlignRotation(JointConnectionDirection target)
	{
		return target switch
		{
			JointConnectionDirection.Right => GridRotation.Deg_90, 
			JointConnectionDirection.Up => GridRotation.Deg_0, 
			JointConnectionDirection.Left => GridRotation.Deg_90, 
			JointConnectionDirection.Down => GridRotation.Deg_0, 
			_ => GridRotation.Deg_0, 
		};
	}

	public override void Initialize()
	{
		base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), 1);
		if (m_partType == PartType.CokeBottle && m_partTier == PartTier.Legendary)
		{
			m_boostForce = INSettings.GetFloat(INFeature.AlienCokeForceValue);
			m_maximumSpeed = INSettings.GetFloat(INFeature.AlienCokeSpeedValue);
		}
		else if (m_partType == PartType.SodaBottle && m_partTier == PartTier.Legendary)
		{
			m_boostForce = INSettings.GetFloat(INFeature.AlienSodaForceValue);
			m_maximumSpeed = INSettings.GetFloat(INFeature.AlienSodaSpeedValue);
		}
		else if (m_partType == PartType.CokeBottle || m_partType == PartType.SodaBottle)
		{
			m_boostForce *= INSettings.GetFloat(INFeature.CokeSodaForce);
			m_maximumSpeed *= INSettings.GetFloat(INFeature.CokeSodaSpeed);
		}
		else if (m_partType == PartType.Rocket || m_partType == PartType.RedRocket)
		{
			m_boostForce *= INSettings.GetFloat(INFeature.RocketForce);
			m_maximumSpeed *= INSettings.GetFloat(INFeature.RocketSpeed);
		}
	}

	public void Update()
	{
		float num = Time.time - m_timeBoostStarted;
		if (m_boostUsed && (bool)m_content && m_currentAlpha > 0f)
		{
			m_currentAlpha -= Time.deltaTime;
			if (m_currentAlpha < 0f)
			{
				m_currentAlpha = 0f;
			}
			m_content.material.color = new Color(m_currentAlpha, m_currentAlpha, m_currentAlpha, m_currentAlpha);
			if ((bool)m_content2)
			{
				m_content2.material.color = new Color(1f - m_currentAlpha, 1f - m_currentAlpha, 1f - m_currentAlpha, 1f - m_currentAlpha);
			}
		}
		if (!(num < m_ignitionTime) && m_boostUsed && (bool)m_content2 && m_currentAlpha2 > 0f)
		{
			m_currentAlpha2 -= Time.deltaTime;
			if (m_currentAlpha2 < 0f)
			{
				m_currentAlpha2 = 0f;
			}
			Color color = new Color(m_currentAlpha2, m_currentAlpha2, m_currentAlpha2, m_currentAlpha2);
			m_content2.material.color = color;
		}
	}

	public void FixedUpdate()
	{
		bool @bool = INSettings.GetBool(INFeature.SwitchableCokeSodaRocket);
		if (!m_enabled)
		{
			m_targetPart = null;
			m_angle = float.NaN;
			return;
		}
		float num = Time.time - m_timeBoostStarted;
		if (num < m_ignitionTime && m_visualization != null)
		{
			m_visualization.transform.localPosition = UnityEngine.Random.insideUnitCircle * 0.1f;
			if (!@bool)
			{
				return;
			}
		}
		float num2 = 1f;
		if (num > m_ignitionTime)
		{
			if (m_boostUsed)
			{
				if (m_particlesIgnitionInstance != null && m_particlesIgnitionInstance.isPlaying)
				{
					m_particlesIgnitionInstance.Stop();
				}
				if (m_visualization != null)
				{
					m_visualization.transform.localPosition = Vector3.zero;
					Transform transform = m_visualization.transform.Find("Cork");
					if (transform != null)
					{
						transform.parent = base.transform.parent;
						transform.GetComponent<Cork>().Fly(-20f * base.transform.right, 200f, 0.75f);
					}
				}
			}
			if (@bool || num < m_ignitionTime + m_boostDuration + m_boostEndDuration)
			{
				if (!m_particlesFiringInstance.isPlaying)
				{
					m_particlesFiringInstance.Play();
				}
			}
			else if (m_particlesFiringInstance != null)
			{
				m_particlesFiringInstance.Stop();
			}
		}
		if (!@bool && num > m_ignitionTime + m_boostDuration + m_boostEndDuration)
		{
			if (m_explodes)
			{
				Explode();
			}
			UnityEngine.Object.Destroy(m_particlesIgnitionInstance.gameObject);
			UnityEngine.Object.Destroy(m_particlesFiringInstance.gameObject);
			m_enabled = false;
		}
		if (!@bool && num > m_ignitionTime + m_boostDuration)
		{
			num2 = 1f - (num - m_boostDuration - m_ignitionTime) / m_boostEndDuration;
		}
		Vector3 zero = Vector3.zero;
		Vector3 position = base.transform.position + zero * 0.5f;
		Vector3 vector = base.transform.TransformDirection(m_direction);
		float num3 = LimitForceForSpeed(num2 * m_boostForce, vector);
		base.rigidbody.AddForceAtPosition(num3 * vector, position, ForceMode.Force);
		INFixedUpdate();
	}

	private void INFixedUpdate()
	{
		bool num = INSettings.GetBool(INFeature.AvoidanceRocket) && this.IsAvoidanceRocket() && !this.IsSinglePart();
		bool flag = INSettings.GetBool(INFeature.TrackingRocket) && this.IsTrackingRocket() && !this.IsSinglePart();
		if (num)
		{
			float @float = INSettings.GetFloat(INFeature.AvoidanceRocketAngle);
			float float2 = INSettings.GetFloat(INFeature.AvoidanceRocketMaxDistance);
			float num2 = 400f;
			float num3 = 2f;
			Vector3 direction = base.transform.TransformDirection(m_direction);
			if (customPartIndex == 1)
			{
				if (RaycastWithParts(base.transform.position, direction, out var hitInfo, float2))
				{
					Vector3 velocity = base.rigidbody.velocity;
					if (hitInfo.rigidbody != null)
					{
						velocity -= hitInfo.rigidbody.velocity;
					}
					float num4 = velocity.x * direction.x + velocity.y * direction.y + 8f;
					if (num4 > 0f)
					{
						float num5 = ((hitInfo.distance > 4f) ? (hitInfo.distance - 3f) : 1f);
						float num6 = num5 / num4;
						if (num6 < num3)
						{
							float num7 = ((num6 < num3 * 0.5f) ? (num4 * num4 / (2f * num5)) : (-2f * num5 / (num3 * num3) + 2f * num4 / num3));
							num7 *= 8f;
							num7 = 0f - ((num7 < num2) ? num7 : num2);
							base.rigidbody.AddForce(new Vector3(num7 * direction.x, num7 * direction.y), ForceMode.Force);
						}
					}
				}
			}
			else
			{
				int num8 = 6;
				bool[] array = new bool[num8];
				float[] array2 = new float[num8];
				Vector2[] array3 = new Vector2[num8];
				float num9 = -0.5f * @float;
				for (int i = 0; i < num8; i++)
				{
					float f = num9 * ((float)Math.PI / 180f);
					float num10 = Mathf.Cos(f);
					float num11 = Mathf.Sin(f);
					Vector2 vector = new Vector2(direction.x * num10 - direction.y * num11, direction.x * num11 + direction.y * num10);
					RaycastHit hitInfo2;
					bool flag2 = (array[i] = RaycastWithParts(base.transform.position, vector, out hitInfo2, float2));
					array3[i] = vector;
					array2[i] = (flag2 ? hitInfo2.distance : float.PositiveInfinity);
					num9 += @float / (float)(num8 - 1);
				}
				int num12 = -1;
				float num13 = float.PositiveInfinity;
				for (int j = 0; j < num8; j++)
				{
					if (array[j] && array2[j] < num13)
					{
						num12 = j;
						num13 = array2[j];
					}
				}
				if (num12 != -1)
				{
					Vector3 velocity2 = base.rigidbody.velocity;
					Vector2 vector2 = array3[num12];
					float num14 = velocity2.x * vector2.x + velocity2.y * vector2.y + 8f;
					if (num14 > 0f)
					{
						float num15 = ((num12 < num8 / 2) ? 1f : (-1f));
						float num16 = ((num13 > 4f) ? (num13 - 3f) : 1f) / num14;
						if (num16 < num3)
						{
							float num17 = ((num16 < num3 * 0.5f) ? (1f / num16) : (-4f * num16 / (num3 * num3) + 4f / num3));
							num17 *= 150f;
							num17 = ((num17 < 400f) ? num17 : 400f) * num15;
							base.rigidbody.AddForce(new Vector3(num17 * (0f - direction.y), num17 * direction.x), ForceMode.Force);
						}
					}
				}
			}
		}
		if (!flag)
		{
			return;
		}
		float f2 = INSettings.GetFloat(INFeature.TrackingRocketAngle) * 0.5f * ((float)Math.PI / 180f);
		float float3 = INSettings.GetFloat(INFeature.TrackingRocketMaxForce);
		float float4 = INSettings.GetFloat(INFeature.TrackingRocketMaxDistance);
		Vector3 position = base.transform.position;
		Vector3 velocity3 = base.rigidbody.velocity;
		Vector3 vector3 = base.transform.TransformDirection(m_direction);
		float num18 = ((m_targetPart == null) ? 0f : (CalculatePartPriority(m_targetPart) * 1.5f));
		float num19 = 0f;
		float num20 = Mathf.Cos(f2);
		BasePart basePart = null;
		int num21 = LayerMask.NameToLayer("Ground");
		foreach (BasePart part in base.contraption.Parts)
		{
			Vector3 vector4 = part.transform.position - position;
			if (part.HasMultipleRigidbodies())
			{
				vector4 = part.Position - position;
			}
			vector4.z = 0f;
			float magnitude = vector4.magnitude;
			if (!MarkerManager.IsInSameTeamStatic(this, part) && part.m_partType != PartType.Rope && part.gameObject.layer != num21 && magnitude < float4 && vector4.x * vector3.x + vector4.y * vector3.y > num20 * magnitude)
			{
				float num22 = CalculatePartPriority(part);
				if (num22 > num19 && num22 > num18)
				{
					num19 = num22;
					basePart = part;
				}
			}
			else if (part == m_targetPart)
			{
				m_targetPart = null;
			}
		}
		if (basePart != null && basePart != m_targetPart)
		{
			m_targetPart = basePart;
			m_angle = float.NaN;
		}
		if (!(m_targetPart != null))
		{
			return;
		}
		if (customPartIndex == 1)
		{
			Vector3 vector5 = m_targetPart.rigidbody.velocity - velocity3;
			vector5.z = 0f;
			Vector3 vector6 = m_targetPart.transform.position - position;
			vector6.z = 0f;
			float magnitude2 = vector6.magnitude;
			magnitude2 = ((magnitude2 > 1f) ? magnitude2 : 1f);
			Vector3 force = 8f * vector5 + 16f * ((magnitude2 > 16f) ? 1f : (16f / magnitude2)) * vector6;
			float magnitude3 = force.magnitude;
			if (magnitude3 > float3)
			{
				force *= float3 / magnitude3;
			}
			base.rigidbody.AddForce(force, ForceMode.Force);
			return;
		}
		float num23 = (float)Math.PI;
		Vector3 vector7 = m_targetPart.rigidbody.velocity - velocity3;
		vector7.z = 0f;
		Vector3 vector8 = m_targetPart.transform.position - position;
		vector8.z = 0f;
		Vector3 vector9 = vector7 + 2f * vector8;
		float x = vector3.x * vector9.x + vector3.y * vector9.y;
		float num24 = Mathf.Atan2(vector3.x * vector9.y - vector3.y * vector9.x, x);
		if (!float.IsNaN(m_angle))
		{
			float num25 = num24 - m_angle;
			num25 = ((num25 < 0f - num23) ? (num25 + 2f * num23) : ((num25 > num23) ? (num25 - 2f * num23) : num25));
			float num26 = num25 * 1600f + num24 * 400f;
			num26 = ((num26 < 0f - float3) ? (0f - float3) : ((num26 > float3) ? float3 : num26));
			base.rigidbody.AddForce(new Vector3(num26 * (0f - vector3.y), num26 * vector3.x), ForceMode.Force);
		}
		m_angle = num24;
	}

	public bool RaycastWithParts(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
	{
		int num = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("IceGround"));
		if (!(m_enclosedInto != null) || m_enclosedInto.IsTransparentFrame())
		{
			return Physics.Raycast(origin, direction, out hitInfo, maxDistance, num);
		}
		int num2 = LayerMask.NameToLayer("Contraption");
		num |= 1 << num2;
		RaycastHit[] array = Physics.RaycastAll(origin, direction, maxDistance, num);
		int num3 = -1;
		float num4 = float.PositiveInfinity;
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			if (!(raycastHit.distance < num4))
			{
				continue;
			}
			Rigidbody rigidbody = raycastHit.rigidbody;
			if (rigidbody != null && rigidbody.gameObject.layer == num2)
			{
				BasePart component = rigidbody.GetComponent<BasePart>();
				if (component == null || component.ConnectedComponent == base.ConnectedComponent || !MarkerManager.IsInSameTeamStatic(component, this))
				{
					continue;
				}
			}
			num4 = raycastHit.distance;
			num3 = i;
		}
		if (num3 == -1)
		{
			hitInfo = default(RaycastHit);
			return false;
		}
		hitInfo = array[num3];
		return true;
	}

	private float CalculatePartPriority(BasePart part)
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = part.transform.position;
		float magnitude = new Vector2(position2.x - position.x, position2.y - position.y).magnitude;
		magnitude = ((magnitude > 1f) ? magnitude : 1f);
		return (float)base.contraption.ComponentPartCount(part.ConnectedComponent) / magnitude;
	}

	private float LimitForceForSpeed(float forceMagnitude, Vector3 forceDir)
	{
		Vector3 velocity = base.rigidbody.velocity;
		float num = Vector3.Dot(velocity.normalized, forceDir);
		if (num > 0f)
		{
			Vector3 vector = velocity * num;
			if (vector.magnitude > m_maximumSpeed)
			{
				return forceMagnitude / (1f + vector.magnitude - m_maximumSpeed);
			}
		}
		return forceMagnitude;
	}

	protected override void OnTouch()
	{
		if (INSettings.GetBool(INFeature.SwitchableCokeSodaRocket))
		{
			m_enabled = !m_enabled;
			m_boostUsed = !m_boostUsed;
			if (!m_enabled)
			{
				m_particlesIgnitionInstance.Stop();
				m_particlesFiringInstance.Stop();
				m_currentAlpha = 1f;
				m_currentAlpha2 = 1f;
				m_targetPart = null;
				if (m_launchAudioInstance != null)
				{
					UnityEngine.Object.Destroy(m_launchAudioInstance.gameObject);
				}
				return;
			}
			m_particlesIgnitionInstance.Play();
			m_timeBoostStarted = Time.time;
			if (m_explodes)
			{
				Explode();
			}
		}
		else
		{
			if (m_boostUsed)
			{
				return;
			}
			m_enabled = !m_enabled;
			m_particlesIgnitionInstance.Play();
			m_timeBoostStarted = Time.time;
			m_boostUsed = true;
			base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), -1);
		}
		if (m_loopAudio != null)
		{
			float length;
			if (m_shakeAudio != null)
			{
				length = m_shakeAudio.clip.length;
				Singleton<AudioManager>.Instance.SpawnOneShotEffect(m_shakeAudio, base.transform);
			}
			else
			{
				length = m_launchAudio.clip.length;
			}
			StartCoroutine(CoroutineRunner.DelayActionSequence(delegate
			{
				m_loopAudioObject = Singleton<AudioManager>.Instance.SpawnLoopingEffect(m_loopAudio, base.transform);
				AudioSource loopAudioSource = m_loopAudioObject.GetComponent<AudioSource>();
				float originalVolume = loopAudioSource.volume;
				if (m_loopAudioTimeLimit > 0f)
				{
					StartCoroutine(CoroutineRunner.DeltaAction(m_loopAudioTimeLimit, realTime: false, delegate(float t)
					{
						loopAudioSource.volume = originalVolume * t;
					}));
				}
			}, length, realTime: false));
		}
		else
		{
			m_launchAudioInstance = Singleton<AudioManager>.Instance.SpawnOneShotEffect(m_launchAudio, base.transform);
		}
		StartCoroutine(ShineRocketLight());
	}

	private IEnumerator ShineRocketLight()
	{
		pls = GetComponentInChildren<PointLightSource>();
		if ((bool)pls)
		{
			yield return new WaitForSeconds(m_ignitionTime);
			pls.isEnabled = true;
			yield return new WaitForSeconds(m_boostDuration + m_boostEndDuration);
			pls.isEnabled = false;
		}
	}

	public void Explode()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, m_explosionRadius * INSettings.GetFloat(INFeature.RocketExplosionRadius));
		foreach (Collider collider in array)
		{
			GameObject gameObject = FindParentWithRigidBody(collider.gameObject);
			if (gameObject != null)
			{
				int num = CountChildColliders(gameObject, 0);
				AddExplosionForce(gameObject, INSettings.GetFloat(INFeature.RocketExplosionForce) / (float)num);
			}
			TNT component = collider.GetComponent<TNT>();
			if ((bool)component && (!INSettings.GetBool(INFeature.PartGenerator) || component.GeneratorRefCount <= 0))
			{
				component.Explode();
			}
		}
		WPFMonoBehaviour.effectManager.CreateParticles(m_smokeCloud, base.transform.position - Vector3.forward * 12f, force: true);
		Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.tntExplosion, base.transform.position);
		CheckForTNTAchievement();
		StartCoroutine(ShineExplosionLight());
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

	private IEnumerator ShineExplosionLight()
	{
		PointLightSource pls = GetComponentInChildren<PointLightSource>();
		if ((bool)pls)
		{
			pls.size = 4f;
			if ((bool)base.renderer)
			{
				base.renderer.enabled = false;
			}
			pls.onLightTurnOff = (Action)Delegate.Combine(pls.onLightTurnOff, (Action)delegate
			{
				UnityEngine.Object.Destroy(base.gameObject);
			});
			pls.isEnabled = true;
			yield return new WaitForSeconds(pls.turnOnCurve[pls.turnOnCurve.length - 1].time);
			pls.isEnabled = false;
		}
		else if (!INSettings.GetBool(INFeature.SwitchableCokeSodaRocket))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
