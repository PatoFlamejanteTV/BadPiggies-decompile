using System.Collections.Generic;
using UnityEngine;

public class FanPropeller : BasePropulsion
{
	public float m_force;

	public Direction m_forceDirection;

	public bool m_isRotor;

	public bool m_enabled;

	public float m_defaultSpeed;

	public Transform m_fanVisualization;

	protected Quaternion m_origRot;

	protected float m_angle;

	private Vector3 m_originalDirection;

	private Vector3 m_rotorTargetDirection;

	private float m_maximumRotationSpeed;

	private float m_rotationSpeed;

	private float m_maximumForce;

	private float m_maximumSpeed;

	private GameObject loopingSound;

	private AudioSource loopingSoundPrefab;

	private float powerFactor;

	private const float LowPowerSoundLimit = 1f;

	public ParticleSystem smokeCloud;

	public override bool CanBeEnabled()
	{
		return m_maximumRotationSpeed > 0f;
	}

	public override bool HasOnOffToggle()
	{
		return true;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	public override Direction EffectDirection()
	{
		return BasePart.Rotate(m_forceDirection, m_gridRotation);
	}

	public override void Awake()
	{
		base.Awake();
		m_enabled = false;
	}

	public override void Initialize()
	{
		m_origRot = m_fanVisualization.transform.localRotation;
		m_enabled = false;
		Vector3 directionVector = BasePart.GetDirectionVector(m_forceDirection);
		m_originalDirection = base.transform.TransformDirection(directionVector);
		m_rotorTargetDirection = m_originalDirection;
		if (m_rotorTargetDirection.y < 0f)
		{
			m_rotorTargetDirection.y = 1f;
		}
	}

	public override void InitializeEngine()
	{
		powerFactor = base.contraption.GetEnginePowerFactor(this);
		if (powerFactor > 1f)
		{
			powerFactor = Mathf.Pow(powerFactor, 0.75f);
		}
		m_maximumSpeed = powerFactor * m_defaultSpeed;
		m_maximumForce = m_force * powerFactor;
		m_maximumRotationSpeed = 1000f * powerFactor;
		if (m_partType == PartType.Fan)
		{
			m_maximumForce *= INSettings.GetFloat(INFeature.FanForce);
			m_maximumSpeed *= INSettings.GetFloat(INFeature.FanSpeed);
		}
		else if (m_partType == PartType.Propeller)
		{
			m_maximumForce *= INSettings.GetFloat(INFeature.PropellerForce);
			m_maximumSpeed *= INSettings.GetFloat(INFeature.PropellerSpeed);
		}
		else if (m_partType == PartType.Rotor)
		{
			m_maximumForce *= INSettings.GetFloat(INFeature.RotorForce);
			m_maximumSpeed *= INSettings.GetFloat(INFeature.RotorSpeed);
		}
		if (m_maximumRotationSpeed > 0f)
		{
			m_maximumRotationSpeed += 700f;
			return;
		}
		m_angle = 0f;
		if (m_enabled)
		{
			SetEnabled(toggle: false);
		}
	}

	public void FixedUpdate()
	{
		if (!base.contraption || !base.contraption.IsRunning)
		{
			return;
		}
		if (m_enabled)
		{
			m_rotationSpeed = m_maximumRotationSpeed;
		}
		else if (m_rotationSpeed < 450f)
		{
			m_rotationSpeed *= 0.9f;
		}
		else
		{
			m_rotationSpeed *= 0.98f;
		}
		m_angle += m_rotationSpeed * Time.deltaTime;
		if (m_angle > 180f)
		{
			m_angle -= 360f;
		}
		if (!m_enabled)
		{
			if (m_isRotor)
			{
				base.rigidbody.angularDrag = 1f;
			}
			return;
		}
		if (m_isRotor)
		{
			base.rigidbody.angularDrag = 1000f;
		}
		Vector3 directionVector = BasePart.GetDirectionVector(m_forceDirection);
		Vector3 vector = base.transform.TransformDirection(directionVector);
		Vector3 position = base.transform.position + vector * 0.5f;
		Vector3 vector2 = vector;
		if (m_isRotor && Vector3.Dot(vector2, m_rotorTargetDirection) > 0f)
		{
			vector2 = 0.5f * (vector2 + m_rotorTargetDirection);
		}
		float num = LimitForceForSpeed(m_maximumForce, vector2);
		float num2 = 1f;
		if (m_forceDirection == Direction.Left)
		{
			float num3 = Mathf.Clamp(Vector3.Dot(base.rigidbody.velocity, -vector2), -2f, 2f);
			Vector3 position2 = base.transform.position;
			position2.z = 0f;
			int layerMask = ((m_enclosedInto != null) ? (-5) : ((1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("IceGround"))));
			RaycastHit hitInfo;
			if (INSettings.GetBool(INFeature.StableLevitationFan) && m_partType == PartType.Fan && (customPartIndex == 1 || customPartIndex == 2 || customPartIndex == 4))
			{
				Vector3 position3 = base.transform.position;
				position3.z = -0.1f;
				if (m_enabled && EntityLight.RaycastWithLights(position3, -vector2, out hitInfo, 16f, layerMask))
				{
					Vector3 vector3 = -hitInfo.normal;
					Vector3 vector4 = hitInfo.point - base.transform.position;
					float num4 = vector4.x * vector3.x + vector4.y * vector3.y;
					Vector3 vector5 = ((hitInfo.rigidbody == null) ? (-base.rigidbody.velocity) : (hitInfo.rigidbody.velocity - base.rigidbody.velocity));
					float num5 = vector5.x * vector3.x + vector5.y * vector3.y;
					float num6 = ((customPartIndex == 1) ? 1f : ((customPartIndex == 2) ? 2f : 3f));
					float num7 = 16f * num5 + 128f * (num4 - num6);
					float num8 = ((num4 > 8f) ? (-37.5f * num4 + 600f) : 300f);
					num7 = ((num7 < 0f - num8) ? (0f - num8) : ((num7 > num8) ? num8 : num7));
					Vector3 vector6 = num7 * vector3;
					base.rigidbody.AddForce(vector6);
					if (hitInfo.rigidbody != null)
					{
						hitInfo.rigidbody.AddForce(-vector6);
					}
				}
			}
			else if (m_enabled && Physics.Raycast(position2, -vector2, out hitInfo, 1f, layerMask) && EntityLight.CheckRaycast(in hitInfo))
			{
				num2 = 1f + (2f + num3) * Mathf.Pow(1f - hitInfo.distance, 1.5f);
			}
		}
		if (m_enclosedInto == null && m_partType == PartType.Rotor && m_enabled)
		{
			Vector3 velocity = base.rigidbody.velocity;
			if (velocity.magnitude > m_maximumSpeed && Vector3.Dot(vector2, velocity) > 0f)
			{
				float num9 = velocity.magnitude - m_maximumSpeed;
				base.rigidbody.AddForceAtPosition(-4f * num9 * num9 * velocity.normalized, position, ForceMode.Force);
			}
		}
		base.rigidbody.AddForceAtPosition(num * num2 * vector2, position, ForceMode.Force);
		if (!INSettings.GetBool(INFeature.ReactionFan) || m_partType != PartType.Fan || m_partTier != PartTier.Epic)
		{
			return;
		}
		float num10 = 0f;
		Rigidbody rigidbody = base.rigidbody;
		List<(float, Vector2, Rigidbody, Rigidbody)> list = new List<(float, Vector2, Rigidbody, Rigidbody)>();
		Rigidbody[] components = INContraption.Instance.GetComponents<Rigidbody>();
		foreach (Rigidbody rigidbody2 in components)
		{
			Vector3 position4 = rigidbody2.position;
			float num11 = position4.x - position.x;
			float num12 = position4.y - position.y;
			float num13 = num11 * vector.x + num12 * vector.y;
			float num14 = num12 * vector.x - num11 * vector.y;
			float num15 = Mathf.Sqrt(num11 * num11 + num12 * num12);
			float num16 = 0f - num13 - ((num14 > 0f) ? num14 : (0f - num14));
			if (num15 < 8f && num16 > 0f && rigidbody2 != rigidbody)
			{
				num16 *= (1f - num15 / 8f) / (num15 * num15);
				num16 = powerFactor * 32f * ((num16 > 1f) ? 1f : num16);
				num10 += num16;
				list.Add((num16, new Vector2(num11 / num15, num12 / num15), rigidbody2, rigidbody));
			}
		}
		float num17 = ((num10 > 200f) ? (200f / num10) : 1f);
		foreach (var item2 in list)
		{
			float num18 = item2.Item1 * num17;
			Vector2 item = item2.Item2;
			item2.Item3.AddForce(new Vector3(num18 * item.x, num18 * item.y));
			item2.Item4.AddForce(new Vector3((0f - num18) * item.x, (0f - num18) * item.y));
		}
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
		SetEnabled(!m_enabled);
	}

	public override void SetEnabled(bool toggle)
	{
		if (toggle && m_maximumRotationSpeed == 0f)
		{
			return;
		}
		m_enabled = toggle;
		if (!m_enabled)
		{
			m_rotationSpeed = 800f;
			m_angle = 292.3f;
			if ((bool)smokeCloud)
			{
				smokeCloud.Stop();
			}
		}
		else if ((bool)smokeCloud)
		{
			smokeCloud.Play();
		}
		base.contraption.UpdateEngineStates(base.ConnectedComponent);
		if (m_enabled)
		{
			PlayPropellerSound();
		}
		else if ((bool)loopingSound)
		{
			Singleton<AudioManager>.Instance.RemoveCombinedLoopingEffect(loopingSoundPrefab, loopingSound);
		}
		Billboard component = GetComponent<Billboard>();
		if ((bool)component)
		{
			component.enabled = !m_enabled;
		}
	}

	public void PlayPropellerSound()
	{
		AudioManager instance = Singleton<AudioManager>.Instance;
		switch (m_partType)
		{
		default:
			loopingSoundPrefab = ((m_partTier != PartTier.Legendary) ? WPFMonoBehaviour.gameData.commonAudioCollection.propeller : WPFMonoBehaviour.gameData.commonAudioCollection.alienFan);
			break;
		case PartType.Rotor:
			loopingSoundPrefab = ((m_partTier != PartTier.Legendary) ? WPFMonoBehaviour.gameData.commonAudioCollection.rotorLoop : WPFMonoBehaviour.gameData.commonAudioCollection.alienRotor);
			break;
		case PartType.Fan:
			loopingSoundPrefab = ((m_partTier != PartTier.Legendary) ? WPFMonoBehaviour.gameData.commonAudioCollection.fan : WPFMonoBehaviour.gameData.commonAudioCollection.alienFan);
			break;
		}
		loopingSound = instance.SpawnCombinedLoopingEffect(loopingSoundPrefab, base.gameObject.transform);
	}

	protected new void LateUpdate()
	{
		base.LateUpdate();
		Vector3 axis = ((!m_isRotor) ? Vector3.right : Vector3.up);
		m_fanVisualization.transform.localRotation = m_origRot * Quaternion.AngleAxis(m_angle, axis);
	}
}
