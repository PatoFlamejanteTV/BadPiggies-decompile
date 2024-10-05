using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBoxingGlove : BasePart
{
	private enum State
	{
		WindedUp,
		Shoot,
		Winding
	}

	private float m_SpringVisualizationInitZ;

	private int m_BoxingGloveSolverIterCount = -1;

	private GameObject m_Visualization;

	private GameObject m_SpringVisualization;

	private GameObject m_BoxingGlove;

	private Rigidbody m_BoxingGloveRigidbody;

	private GameObject m_BoxingGloveVisualization;

	public GameObject m_BoxingGlovePrefab;

	private bool m_enabled;

	private bool m_CanBeEnabled = true;

	private State m_State;

	public float m_targetDistanceY = 2.5f;

	public float m_targetDeviationX = 0.01f;

	public float m_WindingTime = 1f;

	public float m_ShootTime = 1f;

	public float m_SpringYDrive = 60f;

	public float m_SpringYDriveDamper = 3f;

	public bool m_checkRotation;

	private float m_ShootStartTime;

	private float m_WindingStartTime;

	private List<Joint> m_FixedJointToBreak;

	private BasePart m_EffectDirPart;

	public override bool CanBeEnabled()
	{
		if (INSettings.GetBool(INFeature.SwitchableBoxingGlove))
		{
			if (m_EffectDirPart == null || (m_EffectDirPart.m_partType != PartType.SpringBoxingGlove && (m_EffectDirPart.m_enclosedPart == null || m_EffectDirPart.m_enclosedPart.m_partType != PartType.SpringBoxingGlove)))
			{
				return m_CanBeEnabled;
			}
			return false;
		}
		return m_CanBeEnabled;
	}

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	public override bool HasOnOffToggle()
	{
		return false;
	}

	public override Direction EffectDirection()
	{
		return BasePart.Rotate(Direction.Down, m_gridRotation);
	}

	public override void Awake()
	{
		base.Awake();
		m_SpringVisualization = base.transform.Find("SpringVisualization").gameObject;
		m_SpringVisualization.SetActive(value: false);
		m_SpringVisualizationInitZ = m_SpringVisualization.transform.localPosition.z;
		m_Visualization = base.transform.Find("Visualization").gameObject;
		m_enabled = false;
	}

	public override void EnsureRigidbody()
	{
		m_Visualization.transform.localRotation = Quaternion.identity;
		base.transform.localRotation = Quaternion.AngleAxis(GetRotationAngle(m_gridRotation), Vector3.forward);
		base.EnsureRigidbody();
		base.rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
	}

	public override void PrePlaced()
	{
		base.PrePlaced();
		SetRotation(GridRotation.Deg_0);
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

	private void FlipPosition(Transform target, bool flipX, bool flipY)
	{
		Vector3 localPosition = target.localPosition;
		if (flipX)
		{
			localPosition.x = 0f - Mathf.Abs(localPosition.x);
		}
		if (flipY)
		{
			localPosition.y = 0f - Mathf.Abs(localPosition.y);
		}
		target.localPosition = localPosition;
	}

	public override void ChangeVisualConnections()
	{
		m_SpringVisualization.SetActive(value: false);
	}

	public override void Initialize()
	{
		m_BoxingGlove = Object.Instantiate(m_BoxingGlovePrefab, base.transform.position, base.transform.rotation);
		m_BoxingGloveRigidbody = m_BoxingGlove.GetComponent<Rigidbody>();
		m_BoxingGlove.transform.parent = base.transform;
		m_BoxingGloveVisualization = m_BoxingGlove.transform.Find("Visualization").gameObject;
		CheckRotations();
		m_BoxingGlove.SetActive(value: true);
		InitilizeBoxingGlove();
		if (!(WPFMonoBehaviour.levelManager.ContraptionRunning == null))
		{
			Direction direction = EffectDirection();
			m_EffectDirPart = WPFMonoBehaviour.levelManager.ContraptionRunning.FindPartAt(m_coordX + direction switch
			{
				Direction.Left => -1, 
				Direction.Right => 1, 
				_ => 0, 
			}, m_coordY + direction switch
			{
				Direction.Down => -1, 
				Direction.Up => 1, 
				_ => 0, 
			});
			if ((bool)m_EffectDirPart)
			{
				m_FixedJointToBreak = WPFMonoBehaviour.levelManager.ContraptionRunning.FindPartFixedJoints(m_EffectDirPart);
			}
			m_CanBeEnabled = !WPFMonoBehaviour.levelManager.ContraptionRunning.HasSuperGlue || m_FixedJointToBreak == null || m_FixedJointToBreak.Count <= 0 || !m_FixedJointToBreak[0] || m_EffectDirPart.m_partType == PartType.TNT;
		}
	}

	private void CheckRotations()
	{
		if (m_checkRotation && m_gridRotation == GridRotation.Deg_90)
		{
			FlipRotation(m_Visualization.transform, flipX: true, flipY: false);
			if ((bool)m_BoxingGloveVisualization)
			{
				FlipRotation(m_BoxingGloveVisualization.transform, flipX: false, flipY: false);
			}
		}
		else if (m_checkRotation)
		{
			FlipRotation(m_Visualization.transform, flipX: false, flipY: false);
			if ((bool)m_BoxingGloveVisualization)
			{
				FlipRotation(m_BoxingGloveVisualization.transform, flipX: false, flipY: true);
				FlipPosition(m_BoxingGloveVisualization.transform, flipX: true, flipY: false);
			}
		}
	}

	private void InitilizeBoxingGlove()
	{
		m_BoxingGlove.SetActive(value: true);
		ConfigurableJoint configurableJoint = m_BoxingGlove.GetComponent<ConfigurableJoint>();
		if (configurableJoint == null)
		{
			configurableJoint = m_BoxingGlove.AddComponent<ConfigurableJoint>();
		}
		configurableJoint.connectedBody = base.rigidbody;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Limited;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		SoftJointLimitSpring linearLimitSpring = default(SoftJointLimitSpring);
		SoftJointLimit linearLimit = default(SoftJointLimit);
		linearLimitSpring.spring = 0f;
		linearLimitSpring.damper = 0f;
		linearLimit.bounciness = 0f;
		linearLimit.limit = 1f;
		configurableJoint.linearLimit = linearLimit;
		configurableJoint.linearLimitSpring = linearLimitSpring;
		configurableJoint.yDrive = new JointDrive
		{
			positionSpring = m_SpringYDrive,
			positionDamper = m_SpringYDriveDamper,
			maximumForce = float.MaxValue
		};
		configurableJoint.xDrive = new JointDrive
		{
			positionSpring = 1000f,
			positionDamper = 5f,
			maximumForce = float.MaxValue
		};
		configurableJoint.targetPosition = Vector3.zero;
		configurableJoint.projectionMode = JointProjectionMode.PositionAndRotation;
		configurableJoint.projectionDistance = 0.1f;
		configurableJoint.projectionAngle = 0f;
		configurableJoint.enablePreprocessing = false;
		m_SpringVisualization.SetActive(value: false);
		m_BoxingGloveVisualization.SetActive(value: false);
		IgnoreCollisions();
		m_BoxingGlove.transform.localPosition = Vector3.zero;
		m_BoxingGlove.transform.localRotation = Quaternion.identity;
		if (m_BoxingGloveSolverIterCount < 0)
		{
			m_BoxingGloveSolverIterCount = (int)((float)m_BoxingGloveRigidbody.solverIterations * 1.6f);
		}
		m_BoxingGloveRigidbody.solverIterations = m_BoxingGloveSolverIterCount;
		Collider component = m_BoxingGlove.GetComponent<Collider>();
		if ((bool)component)
		{
			component.enabled = true;
		}
		Rigidbody component2 = m_BoxingGlove.GetComponent<Rigidbody>();
		if ((bool)component2)
		{
			component2.mass = 0.5f;
		}
		m_State = State.WindedUp;
		m_BoxingGlove.SetActive(value: false);
		m_BoxingGloveRigidbody.position = m_BoxingGlove.transform.position;
		m_BoxingGloveRigidbody.rotation = m_BoxingGlove.transform.rotation;
	}

	private void IgnoreCollisions()
	{
		if (GetComponent<Collider>() != null && m_BoxingGlove != null && m_BoxingGlove.activeInHierarchy && m_BoxingGlove.GetComponent<Collider>() != null)
		{
			Physics.IgnoreCollision(GetComponent<Collider>(), m_BoxingGlove.GetComponent<Collider>());
			if (m_enclosedInto != null && m_enclosedInto.GetComponent<Collider>() != null)
			{
				Physics.IgnoreCollision(m_enclosedInto.GetComponent<Collider>(), m_BoxingGlove.GetComponent<Collider>());
			}
		}
	}

	private IEnumerator Shoot(float timeout)
	{
		if (timeout > 0f)
		{
			yield return new WaitForSeconds(timeout);
		}
		if ((bool)m_EffectDirPart && base.ConnectedComponent >= 0 && m_EffectDirPart.ConnectedComponent == base.ConnectedComponent)
		{
			for (int i = 0; i < m_FixedJointToBreak.Count; i++)
			{
				Object.Destroy(m_FixedJointToBreak[i]);
				if ((bool)m_FixedJointToBreak[i])
				{
					BasePart component = m_FixedJointToBreak[i].gameObject.GetComponent<BasePart>();
					if ((bool)component)
					{
						component.HandleJointBreak();
					}
				}
				m_FixedJointToBreak[i] = null;
			}
			m_FixedJointToBreak.Clear();
		}
		m_State = State.Shoot;
		m_enabled = true;
		m_BoxingGlove.SetActive(value: true);
		m_SpringVisualization.SetActive(value: true);
		m_BoxingGloveVisualization.SetActive(value: true);
		m_ShootStartTime = Time.time;
		if (HasTag("Alien_part"))
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.alienPunch, base.transform.position);
		}
		else
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.springBoxingGloveShoot, base.transform.position);
		}
		ConfigurableJoint component2 = m_BoxingGlove.GetComponent<ConfigurableJoint>();
		component2.targetPosition = new Vector3(m_targetDeviationX * ((Random.value >= 0.5f) ? 1f : (-1f)), m_targetDistanceY, 0f);
		SoftJointLimitSpring linearLimitSpring = component2.linearLimitSpring;
		linearLimitSpring.spring = 0.1f;
		component2.linearLimitSpring = linearLimitSpring;
		JointDrive yDrive = component2.yDrive;
		component2.yDrive = yDrive;
		if (m_targetDeviationX != 0f)
		{
			component2.xMotion = ConfigurableJointMotion.Limited;
			JointDrive xDrive = component2.xDrive;
			component2.xDrive = xDrive;
		}
	}

	protected override void OnTouch()
	{
		if (INSettings.GetBool(INFeature.SwitchableBoxingGlove))
		{
			m_enabled = !m_enabled;
			if (m_enabled && CanBeEnabled())
			{
				StartCoroutine(Shoot(0f));
			}
		}
		else if (!m_enabled && CanBeEnabled())
		{
			StartCoroutine(Shoot(0f));
		}
	}

	private void Update()
	{
		IgnoreCollisions();
		switch (m_State)
		{
		case State.Winding:
			if (Time.time - m_WindingStartTime > m_WindingTime || m_BoxingGlove.transform.localPosition.magnitude < 0.1f || m_BoxingGlove.transform.localPosition.y > 0f)
			{
				m_State = State.WindedUp;
				m_enabled = false;
				InitilizeBoxingGlove();
			}
			break;
		case State.Shoot:
			if ((INSettings.GetBool(INFeature.SwitchableBoxingGlove) && !m_enabled) || (!INSettings.GetBool(INFeature.SwitchableBoxingGlove) && Time.time - m_ShootStartTime > m_ShootTime))
			{
				m_State = State.Winding;
				m_WindingStartTime = Time.time;
				Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.springBoxingGloveWinding, base.transform.position);
				ConfigurableJoint component = m_BoxingGlove.GetComponent<ConfigurableJoint>();
				component.targetPosition = Vector3.zero;
				JointDrive yDrive = component.yDrive;
				float num = 10f;
				yDrive.positionDamper = num * 0.25f;
				yDrive.positionSpring = num * m_targetDistanceY;
				component.yDrive = yDrive;
				Collider component2 = m_BoxingGlove.GetComponent<Collider>();
				if ((bool)component2)
				{
					component2.enabled = false;
				}
				Rigidbody component3 = m_BoxingGlove.GetComponent<Rigidbody>();
				if ((bool)component3)
				{
					component3.mass = 0.01f;
				}
			}
			break;
		}
		if (m_State != 0 && (bool)base.rigidbody && (bool)m_BoxingGloveRigidbody)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = m_BoxingGlove.transform.position;
			m_SpringVisualization.transform.rotation = Quaternion.LookRotation(Vector3.back, position2 - position);
			Vector3 localScale = m_SpringVisualization.transform.localScale;
			localScale.y = Vector3.Distance(position, position2);
			m_SpringVisualization.transform.localScale = localScale;
			m_SpringVisualization.transform.position = 0.5f * (position + position2);
			Vector3 localPosition = m_SpringVisualization.transform.localPosition;
			localPosition.z = m_SpringVisualizationInitZ;
			m_SpringVisualization.transform.localPosition = localPosition;
		}
	}
}
