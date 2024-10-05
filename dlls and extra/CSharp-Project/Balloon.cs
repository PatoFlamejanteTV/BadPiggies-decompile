using System;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : BasePart
{
	public float m_force = 10f;

	public float m_maximumSpeed = 10f;

	public bool m_inWorldCoordinates = true;

	public Vector3 m_direction = Vector3.up;

	public int m_numberOfBalloons = 1;

	public bool m_popped;

	public bool m_ghostBalloon;

	public GameObject m_actualVisualizationNode;

	public GameObject m_gridVisualizationNode;

	public Material m_stringMaterial;

	protected BasePart m_connectedPart;

	protected Vector3 m_connectedLocalPos;

	protected BalloonBalancer m_balancer;

	protected SpringJoint m_springJoint;

	protected RopeVisualization m_rope;

	public override void Awake()
	{
		base.Awake();
		base.enabled = false;
		if ((bool)m_actualVisualizationNode && (bool)m_gridVisualizationNode)
		{
			SetRenderersInChildred(m_actualVisualizationNode, enable: false);
			SetRenderersInChildred(m_gridVisualizationNode, enable: true);
		}
	}

	private void SetRenderersInChildred(GameObject target, bool enable)
	{
		if (target == null)
		{
			return;
		}
		Renderer[] componentsInChildren = target.GetComponentsInChildren<Renderer>();
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = enable;
			}
		}
	}

	public override bool IsIntegralPart()
	{
		return false;
	}

	public override void PrePlaced()
	{
		base.PrePlaced();
		if (INSettings.GetBool(INFeature.RotatableBalloon))
		{
			m_autoAlign = AutoAlignType.Rotate;
		}
	}

	public override void Initialize()
	{
		if ((bool)m_actualVisualizationNode && (bool)m_gridVisualizationNode)
		{
			SetRenderersInChildred(m_gridVisualizationNode, enable: false);
			SetRenderersInChildred(m_actualVisualizationNode, enable: true);
		}
		int i = 1;
		int x = 0;
		int y = 1;
		int @int = INSettings.GetInt(INFeature.BalloonConnectionDistance);
		if (INSettings.GetBool(INFeature.RotatableBalloon))
		{
			BasePart.GetDirection((GridRotation)((int)(m_gridRotation + 1) % 4), out x, out y);
		}
		for (; i < @int + 1; i++)
		{
			if ((bool)m_connectedPart)
			{
				break;
			}
			m_connectedPart = base.contraption.FindPartAt(m_coordX - i * x, m_coordY - i * y);
			if ((bool)m_connectedPart && !m_connectedPart.IsPartOfChassis() && m_connectedPart.m_partType != PartType.Pig && m_connectedPart.m_partType != PartType.Kicker)
			{
				m_connectedPart = null;
			}
		}
		m_partType = PartType.Balloon;
		base.contraption.ChangeOneShotPartAmount(BasePart.BaseType(m_partType), EffectDirection(), 1);
		if (m_numberOfBalloons > 1)
		{
			GameObject obj = UnityEngine.Object.Instantiate(base.gameObject);
			obj.transform.position = base.transform.position;
			Balloon component = obj.GetComponent<Balloon>();
			component.m_numberOfBalloons = m_numberOfBalloons - 1;
			base.contraption.AddRuntimePart(component);
			obj.transform.parent = base.contraption.transform;
		}
		if (!base.gameObject.GetComponent<SphereCollider>())
		{
			base.gameObject.AddComponent<SphereCollider>().radius = 0.5f;
		}
		if (!base.rigidbody)
		{
			base.gameObject.AddComponent<Rigidbody>();
		}
		base.rigidbody.mass = 0.1f;
		base.rigidbody.drag = 2f;
		base.rigidbody.angularDrag = 0.5f;
		base.rigidbody.constraints = (RigidbodyConstraints)48;
		if ((bool)m_connectedPart)
		{
			m_connectedPart.EnsureRigidbody();
			Vector3 position = base.transform.position;
			float num = Vector3.Distance(m_connectedPart.transform.position, position) - 0.5f;
			float num2 = 0f;
			Vector3 vector;
			if (m_connectedPart.m_partType == PartType.Pig)
			{
				vector = Vector3.zero;
				num2 = 0.3f;
			}
			else
			{
				vector = Vector3.up * 0.5f;
			}
			base.transform.position = m_connectedPart.transform.position + vector;
			m_springJoint = base.gameObject.AddComponent<SpringJoint>();
			m_springJoint.connectedBody = m_connectedPart.rigidbody;
			base.contraption.AddJointToMap(this, m_connectedPart, m_springJoint);
			float maxDistance = UnityEngine.Random.Range(0.8f, 1.2f) * num + num2;
			m_springJoint.minDistance = 0f;
			m_springJoint.maxDistance = maxDistance;
			m_springJoint.anchor = Vector3.up * -0.5f;
			m_springJoint.spring = 100f;
			m_springJoint.damper = 10f;
			m_springJoint.enablePreprocessing = false;
			m_balancer = m_connectedPart.gameObject.GetComponent<BalloonBalancer>();
			if (!m_balancer)
			{
				m_balancer = m_connectedPart.gameObject.AddComponent<BalloonBalancer>();
			}
			m_balancer.AddBalloon();
			Transform transform = base.transform;
			if ((bool)m_actualVisualizationNode)
			{
				transform = m_actualVisualizationNode.transform;
			}
			m_rope = transform.gameObject.AddComponent<RopeVisualization>();
			m_connectedLocalPos = m_connectedPart.transform.InverseTransformPoint(base.transform.position);
			m_springJoint.autoConfigureConnectedAnchor = false;
			m_springJoint.connectedAnchor = m_connectedLocalPos;
			m_rope.m_stringMaterial = m_stringMaterial;
			m_rope.m_pos1Anchor = Vector3.up * -0.5f + 1.1f * Vector3.forward;
			m_rope.m_pos2Transform = m_connectedPart.transform;
			m_rope.m_pos2Anchor = m_connectedLocalPos + 1.1f * Vector3.forward;
			base.transform.position = position + UnityEngine.Random.Range(-1f, 1f) * Vector3.forward + UnityEngine.Random.Range(-1f, 1f) * Vector3.right * 0.5f;
		}
		m_force *= INSettings.GetFloat(INFeature.BalloonForce);
	}

	public void ConfigureExtraBalanceJoint(float powerFactor)
	{
		if ((bool)m_balancer)
		{
			m_balancer.Configure(powerFactor);
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

	public void FixedUpdate()
	{
		float num = LimitForceForSpeed(m_force, m_direction);
		base.rigidbody.AddForce(num * m_direction, ForceMode.Force);
		if (m_rope != null && (m_springJoint == null || m_connectedPart == null))
		{
			UnityEngine.Object.Destroy(m_rope.GetComponent<LineRenderer>());
			UnityEngine.Object.Destroy(m_rope);
			if (m_springJoint != null)
			{
				UnityEngine.Object.Destroy(m_springJoint);
			}
		}
	}

	protected override void OnTouch()
	{
		Pop();
	}

	public override void OnCollisionEnter(Collision coll)
	{
		if (INSettings.GetBool(INFeature.UnpoppableBalloon) && customPartIndex == 7)
		{
			return;
		}
		ContactPoint[] contacts = coll.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			if (contactPoint.otherCollider.gameObject.layer == BasePart.m_groundLayer || contactPoint.otherCollider.gameObject.layer == BasePart.m_iceGroundLayer || contactPoint.otherCollider.CompareTag("Sharp"))
			{
				Pop();
				break;
			}
		}
	}

	public void Pop()
	{
		if (!m_popped)
		{
			m_popped = true;
			AudioSource effectSource = ((!m_ghostBalloon) ? WPFMonoBehaviour.gameData.commonAudioCollection.balloonPop : WPFMonoBehaviour.gameData.commonAudioCollection.ghostBalloonPop[UnityEngine.Random.Range(0, WPFMonoBehaviour.gameData.commonAudioCollection.ghostBalloonPop.Length)]);
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(effectSource, base.transform.position);
			WPFMonoBehaviour.effectManager.CreateParticles(WPFMonoBehaviour.gameData.m_ballonParticles, base.transform.position);
			base.contraption.ChangeOneShotPartAmount(BasePart.BaseType(m_partType), EffectDirection(), -1);
			if ((bool)m_balancer)
			{
				m_balancer.RemoveBalloon();
			}
			CheckBalloonPopperAchievement();
			base.contraption.RemovePart(this);
			HandleJointBreak(playEffects: false);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public override void EnsureRigidbody()
	{
		if (base.rigidbody == null)
		{
			base.rigidbody = base.gameObject.AddComponent<Rigidbody>();
		}
		base.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
	}

	public void CheckBalloonPopperAchievement()
	{
		if (!Singleton<SocialGameManager>.IsInstantiated() || !Singleton<GameManager>.Instance.IsInGame())
		{
			return;
		}
		int poppedBalloons = GameProgress.GetInt("Popped_Balloons") + 1;
		GameProgress.SetInt("Popped_Balloons", poppedBalloons);
		((Action<List<string>>)delegate(List<string> achievements)
		{
			foreach (string achievement in achievements)
			{
				if (Singleton<SocialGameManager>.Instance.TryReportAchievementProgress(achievement, 100.0, (int limit) => poppedBalloons >= limit))
				{
					break;
				}
			}
		})(new List<string> { "grp.POPPERS_THEORY_III", "grp.POPPERS_THEORY_II", "grp.POPPERS_THEORY_I" });
	}
}
