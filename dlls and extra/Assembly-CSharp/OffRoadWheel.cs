using System;
using System.Collections.Generic;
using UnityEngine;

public class OffRoadWheel : BasePart
{
	public float m_force;

	public bool m_enabled;

	public float m_maximumSpeed;

	private float m_maximumForce;

	private Vector3 m_lastPosition = Vector3.zero;

	private List<Vector3> m_lastContactDirection;

	private float m_radius;

	private float m_circumference;

	private Transform m_wheelPivot;

	private Transform m_fakeWheelPivot;

	private float m_spinSpeed;

	private Vector3 m_lastForceDirection;

	private float m_angle;

	private Collider m_supportCollider;

	private bool m_hasContact = true;

	private float m_thrust;

	private float m_thrustTimer;

	private Rigidbody colliderRigidbody;

	private AudioSource loopingWheelSound;

	private bool m_grounded;

	private List<BasePart> m_ignoredCollisionParts;

	public float m_springStiffness = 150f;

	public bool HasContact => m_hasContact;

	public override bool CanBeEnabled()
	{
		return m_maximumSpeed > 0f;
	}

	public override bool HasOnOffToggle()
	{
		return true;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	public override void Awake()
	{
		base.Awake();
		m_lastContactDirection = new List<Vector3>();
		SetScale(INSettings.GetFloat(INFeature.OffRoadWheelScale));
	}

	private void Start()
	{
		m_wheelPivot = base.transform.Find("WheelPivot");
		m_fakeWheelPivot = base.transform.Find("FakeWheelPivot");
		m_radius = GetComponent<SphereCollider>().radius;
		m_circumference = (float)Math.PI * 2f * m_radius;
		if ((bool)base.transform.Find("SupportCollider"))
		{
			m_supportCollider = base.transform.Find("SupportCollider").GetComponent<Collider>();
		}
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public override void PostInitialize()
	{
		if (INSettings.GetBool(INFeature.NonCollisionOffRoadWheel))
		{
			foreach (BasePart part in base.contraption.Parts)
			{
				IgnoreCollisionRecursive(base.collider, part.gameObject);
			}
			return;
		}
		int num = -1;
		int num2 = 1;
		int num3 = -1;
		int num4 = 0;
		int num5 = num;
		int num6 = num2;
		int num7 = num3;
		int num8 = num4;
		switch (m_gridRotation)
		{
		case GridRotation.Deg_90:
			num5 = -num4;
			num6 = -num3;
			num7 = num;
			num8 = num2;
			break;
		case GridRotation.Deg_180:
			num5 = -num2;
			num6 = -num;
			num7 = -num4;
			num8 = -num3;
			break;
		case GridRotation.Deg_270:
			num5 = num3;
			num6 = num4;
			num7 = -num2;
			num8 = -num;
			break;
		}
		m_ignoredCollisionParts = new List<BasePart>();
		for (int i = num5; i <= num6; i++)
		{
			for (int j = num7; j <= num8; j++)
			{
				BasePart basePart = base.contraption.FindPartAt(m_coordX + i, m_coordY + j);
				if (basePart != null && basePart.ConnectedComponent == base.ConnectedComponent)
				{
					BasePart basePart2 = basePart.enclosedPart;
					IgnoreCollisionRecursive(base.collider, basePart.gameObject);
					m_ignoredCollisionParts.Add(basePart);
					if (basePart2 != null)
					{
						IgnoreCollisionRecursive(base.collider, basePart2.gameObject);
						m_ignoredCollisionParts.Add(basePart2);
					}
				}
			}
		}
	}

	private void IgnoreCollisionRecursive(Collider collider, GameObject part)
	{
		if ((bool)part.GetComponent<Collider>() && part.GetComponent<Collider>() != collider)
		{
			Physics.IgnoreCollision(collider, part.GetComponent<Collider>());
			for (int i = 0; i < part.transform.childCount; i++)
			{
				IgnoreCollisionRecursive(collider, part.transform.GetChild(i).gameObject);
			}
		}
	}

	private void SpawnSound()
	{
		loopingWheelSound = Singleton<AudioManager>.Instance.SpawnCombinedLoopingEffect(WPFMonoBehaviour.gameData.commonAudioCollection.motorWheelLoop, base.transform).GetComponent<AudioSource>();
		loopingWheelSound.volume = 0f;
	}

	public override void InitializeEngine()
	{
		float enginePowerFactor = base.contraption.GetEnginePowerFactor(this);
		m_maximumForce = m_force * enginePowerFactor;
		m_maximumSpeed = 15f * enginePowerFactor;
	}

	private void ReinitializeEngine()
	{
		float enginePowerFactor = base.contraption.GetEnginePowerFactor(this);
		m_maximumForce = m_force * enginePowerFactor;
	}

	public override void OnCollisionStay(Collision collisionInfo)
	{
		base.OnCollisionStay(collisionInfo);
		for (int i = 0; i < collisionInfo.contacts.Length; i++)
		{
			if (collisionInfo.contacts[i].otherCollider != m_supportCollider)
			{
				m_lastContactDirection.Add((collisionInfo.contacts[i].point - m_lastPosition).normalized);
				break;
			}
		}
	}

	public override void OnLightEnter(Vector2 point)
	{
		Vector2 vector = m_lastPosition;
		m_lastContactDirection.Add((point - vector).normalized);
	}

	public override Joint CustomConnectToPart(BasePart part)
	{
		ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.connectedBody = part.rigidbody;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Limited;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		SoftJointLimitSpring linearLimitSpring = configurableJoint.linearLimitSpring;
		linearLimitSpring.spring = m_springStiffness;
		linearLimitSpring.damper = 5f;
		configurableJoint.linearLimitSpring = linearLimitSpring;
		SoftJointLimit linearLimit = configurableJoint.linearLimit;
		linearLimit.limit = 0f;
		linearLimit.bounciness = 0f;
		configurableJoint.linearLimit = linearLimit;
		return configurableJoint;
	}

	private void Update()
	{
		if (!base.contraption || !base.contraption.IsRunning)
		{
			return;
		}
		if (m_ignoredCollisionParts != null)
		{
			for (int num = m_ignoredCollisionParts.Count - 1; num >= 0; num--)
			{
				BasePart basePart = m_ignoredCollisionParts[num];
				if (basePart == null)
				{
					m_ignoredCollisionParts.RemoveAt(num);
				}
				else if (basePart.ConnectedComponent != base.ConnectedComponent)
				{
					Collider[] componentsInChildren = basePart.GetComponentsInChildren<Collider>();
					foreach (Collider collider in componentsInChildren)
					{
						Physics.IgnoreCollision(base.collider, collider, ignore: false);
					}
					m_ignoredCollisionParts.RemoveAt(num);
				}
			}
		}
		if (m_spinSpeed == 0f)
		{
			m_spinSpeed = SpeedInDirection(m_lastForceDirection);
		}
		float z = base.transform.rotation.eulerAngles.z;
		m_angle += -360f * m_spinSpeed / m_circumference * Time.deltaTime;
		if ((bool)m_wheelPivot)
		{
			m_wheelPivot.transform.localRotation = Quaternion.AngleAxis(0f - z + m_angle, Vector3.forward);
		}
		if ((bool)m_fakeWheelPivot)
		{
			float angle = 0f - z + Mathf.Sin(2f * m_angle * ((float)Math.PI / 180f)) * 8f;
			m_fakeWheelPivot.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		UpdateSoundEffect();
	}

	protected override void UpdateSoundEffect()
	{
		float num = 1f;
		if (Mathf.Abs(m_spinSpeed) > num && m_grounded)
		{
			if (!loopingWheelSound)
			{
				SpawnSound();
			}
			loopingWheelSound.volume = 0.15f * (Mathf.Abs(m_spinSpeed) / num - 1f);
		}
		else if ((bool)loopingWheelSound)
		{
			loopingWheelSound.volume = 0f;
		}
	}

	private void FixedUpdate()
	{
		if (!base.contraption || !base.contraption.IsRunning)
		{
			return;
		}
		ReinitializeEngine();
		if (m_enabled)
		{
			m_thrustTimer += Time.deltaTime * 1f;
			m_thrustTimer = Mathf.Min(m_thrustTimer, 1f);
			m_thrust = Mathf.Pow(m_thrustTimer, 0.4f);
			if (base.contraption.ConnectedToGearbox(this) && base.contraption.GetGearbox(this).IsEnabled())
			{
				m_thrust = 0f - m_thrust;
			}
		}
		else
		{
			m_thrustTimer = 0f;
			m_thrust = 0f;
		}
		m_lastPosition = m_wheelPivot.transform.position;
		bool flag = true;
		foreach (Vector3 item in m_lastContactDirection)
		{
			if (!Physics.Raycast(m_wheelPivot.transform.position, item, out var hitInfo, m_radius + 0.1f) || !(hitInfo.collider != m_supportCollider))
			{
				continue;
			}
			flag = false;
			Vector3 vector = Vector3.Cross(hitInfo.normal, Vector3.forward);
			float num = SpeedInDirection(vector);
			m_lastForceDirection = vector;
			m_spinSpeed = 0f;
			m_hasContact = true;
			colliderRigidbody = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
			if (m_enabled && m_maximumSpeed > 0f && num < m_maximumSpeed && num > 0f - m_maximumSpeed)
			{
				float f = 1f - Mathf.Abs(num) / m_maximumSpeed;
				f = Mathf.Pow(f, 0.5f);
				float num2 = m_thrust * m_maximumForce * f;
				base.rigidbody.AddForceAtPosition(num2 * vector, hitInfo.point, ForceMode.Force);
				if (!colliderRigidbody && hitInfo.collider.transform.parent != null)
				{
					colliderRigidbody = hitInfo.collider.transform.parent.GetComponent<Rigidbody>();
				}
				if (colliderRigidbody != null && !colliderRigidbody.isKinematic)
				{
					colliderRigidbody.AddForceAtPosition((0f - num2) * vector, hitInfo.point, ForceMode.Force);
				}
			}
			m_grounded = true;
		}
		if (flag)
		{
			m_hasContact = false;
			colliderRigidbody = null;
			if (m_enabled)
			{
				m_spinSpeed = m_maximumSpeed;
				if (base.contraption.ConnectedToGearbox(this) && base.contraption.GetGearbox(this).IsEnabled())
				{
					m_spinSpeed = 0f - m_maximumSpeed;
				}
			}
			else
			{
				m_spinSpeed *= 0.98f;
			}
		}
		m_lastContactDirection.Clear();
	}

	private float SpeedInDirection(Vector3 direction)
	{
		Vector3 velocity = base.rigidbody.velocity;
		if ((bool)colliderRigidbody)
		{
			velocity -= colliderRigidbody.velocity;
		}
		return Vector3.Dot(velocity, direction);
	}

	protected override void OnTouch()
	{
		if (!m_enabled)
		{
			base.rigidbody.WakeUp();
		}
		SetEnabled(!m_enabled);
	}

	public override void SetEnabled(bool enabled)
	{
		m_enabled = enabled;
		base.contraption.UpdateEngineStates(base.ConnectedComponent);
	}

	private void SetScale(float scale)
	{
		float num = 0.5f;
		SphereCollider component = GetComponent<SphereCollider>();
		component.center = new Vector3(0f, num - (num + 0.5f) * scale, 0f);
		component.radius = 0.9f * scale;
		Transform obj = base.transform.Find("WheelPivot");
		obj.localPosition = new Vector3(0f, num - (num + 0.5f) * scale, 0f);
		obj.localScale = new Vector3(scale, scale, 1f);
		Transform obj2 = base.transform.Find("Support");
		obj2.localPosition = new Vector3(0f, num - (num - 0.15f) * scale, 0f);
		obj2.localScale = new Vector3(scale, scale, 1f);
	}
}
