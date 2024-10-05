using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : BasePart
{
	private static float SPRING_LIMIT_SPRING = 250f;

	private static float SPRING_DAMPING = 20f;

	private static float SPRING_LIMIT = 0.1f;

	private static float SPRING_BOUNCINESS = 1f;

	private static float SPRING_BREAK_FORCE = 250f;

	public GameObject m_endPointPrefab;

	private Rigidbody m_connectedBody;

	private GameObject m_visualization;

	private Vector3 m_localConnectionPoint;

	private Vector3 m_remoteConnectionPoint;

	private bool m_jointBroken;

	private Joint joint;

	private bool m_elastic;

	public override void Awake()
	{
		base.Awake();
		m_visualization = base.transform.Find("SpringVisualization").gameObject;
		m_jointBroken = false;
		if (INSettings.GetBool(INFeature.StrongSpringConnection))
		{
			SPRING_BREAK_FORCE = WPFMonoBehaviour.gameData.m_jointConnectionStrengthHigh * 2f * INSettings.GetFloat(INFeature.ConnectionStrength);
			m_jointConnectionStrength = JointConnectionStrength.High;
		}
	}

	private void Update()
	{
		if ((bool)base.rigidbody && (bool)m_connectedBody)
		{
			Vector3 vector = base.transform.TransformPoint(m_localConnectionPoint);
			Vector3 vector2 = m_connectedBody.transform.TransformPoint(m_remoteConnectionPoint);
			Vector3 localScale = m_visualization.transform.localScale;
			localScale.y = Vector3.Distance(vector, vector2);
			m_visualization.transform.localScale = localScale;
			m_visualization.transform.position = 0.5f * (vector + vector2);
			Vector3 vector3 = vector2 - vector;
			float z = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f + 90f;
			m_visualization.transform.rotation = Quaternion.Euler(0f, 0f, z);
		}
	}

	protected new IEnumerator OnJointBreak(float breakForce)
	{
		base.OnJointBreak(breakForce);
		yield return null;
		if (joint == null)
		{
			CreateSpringBody(Direction.Down);
		}
	}

	public override void EnsureRigidbody()
	{
		base.EnsureRigidbody();
		base.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		if (INSettings.GetBool(INFeature.ElasticSpringConnection))
		{
			m_elastic = m_enclosedInto != null || customPartIndex == 0 || customPartIndex == 2;
		}
	}

	private void FixedUpdate()
	{
		if (m_jointBroken || base.transform == null || m_connectedBody == null || INSettings.GetBool(INFeature.StrongSpringConnection))
		{
			return;
		}
		Vector3 a = base.transform.TransformPoint(m_localConnectionPoint);
		Vector3 b = m_connectedBody.transform.TransformPoint(m_remoteConnectionPoint);
		if (Vector3.Distance(a, b) > 3f && !base.contraption.HasSuperGlue)
		{
			List<Joint> list = base.contraption.FindPartFixedJoints(this);
			for (int i = 0; i < list.Count; i++)
			{
				Object.Destroy(list[i]);
			}
			HandleJointBreak();
			m_jointBroken = true;
		}
	}

	public override Joint CustomConnectToPart(BasePart part)
	{
		if (INSettings.GetBool(INFeature.ElasticSpringConnection))
		{
			m_elastic = m_enclosedInto != null || customPartIndex == 0 || customPartIndex == 2;
		}
		if (INSettings.GetBool(INFeature.ElasticSpringConnection) && m_elastic)
		{
			SpringJoint springJoint = base.gameObject.AddComponent<SpringJoint>();
			springJoint.connectedBody = part.rigidbody;
			springJoint.minDistance = 0f;
			springJoint.maxDistance = 0f;
			springJoint.breakForce = SPRING_BREAK_FORCE;
			springJoint.spring = SPRING_LIMIT_SPRING;
			springJoint.damper = SPRING_DAMPING;
			springJoint.enablePreprocessing = false;
			m_connectedBody = part.rigidbody;
			m_localConnectionPoint = new Vector3(0f, 0.5f, 0f);
			m_remoteConnectionPoint = part.transform.InverseTransformPoint(base.transform.position - 0.5f * base.transform.up);
			joint = springJoint;
			return springJoint;
		}
		ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.connectedBody = part.rigidbody;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Limited;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		configurableJoint.enablePreprocessing = false;
		configurableJoint.configuredInWorldSpace = true;
		configurableJoint.breakForce = SPRING_BREAK_FORCE;
		SoftJointLimitSpring linearLimitSpring = configurableJoint.linearLimitSpring;
		linearLimitSpring.spring = SPRING_LIMIT_SPRING;
		linearLimitSpring.damper = SPRING_DAMPING;
		configurableJoint.linearLimitSpring = linearLimitSpring;
		SoftJointLimit linearLimit = configurableJoint.linearLimit;
		linearLimit.limit = SPRING_LIMIT;
		linearLimit.bounciness = SPRING_BOUNCINESS;
		configurableJoint.linearLimit = linearLimit;
		m_connectedBody = part.rigidbody;
		m_localConnectionPoint = new Vector3(0f, 0.5f, 0f);
		m_remoteConnectionPoint = part.transform.InverseTransformPoint(base.transform.position - 0.5f * base.transform.up);
		joint = configurableJoint;
		return configurableJoint;
	}

	public void CreateSpringBody(Direction direction)
	{
		GameObject gameObject = Object.Instantiate(m_endPointPrefab, base.transform.position, base.transform.rotation);
		if (INSettings.GetBool(INFeature.ElasticSpringConnection) && m_elastic)
		{
			SpringJoint springJoint = base.gameObject.AddComponent<SpringJoint>();
			springJoint.connectedBody = gameObject.GetComponent<Rigidbody>();
			springJoint.minDistance = 0f;
			springJoint.maxDistance = 0f;
			springJoint.breakForce = SPRING_BREAK_FORCE;
			springJoint.spring = SPRING_LIMIT_SPRING;
			springJoint.damper = SPRING_DAMPING;
			springJoint.enablePreprocessing = false;
		}
		else
		{
			ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
			configurableJoint.connectedBody = gameObject.GetComponent<Rigidbody>();
			configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
			configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
			configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
			configurableJoint.xMotion = ConfigurableJointMotion.Locked;
			configurableJoint.yMotion = ConfigurableJointMotion.Limited;
			configurableJoint.zMotion = ConfigurableJointMotion.Locked;
			configurableJoint.enablePreprocessing = false;
			configurableJoint.configuredInWorldSpace = true;
			SoftJointLimitSpring linearLimitSpring = configurableJoint.linearLimitSpring;
			linearLimitSpring.spring = SPRING_LIMIT_SPRING;
			linearLimitSpring.damper = SPRING_DAMPING;
			configurableJoint.linearLimitSpring = linearLimitSpring;
			SoftJointLimit linearLimit = configurableJoint.linearLimit;
			linearLimit.limit = SPRING_LIMIT;
			linearLimit.bounciness = SPRING_BOUNCINESS;
			configurableJoint.linearLimit = linearLimit;
		}
		gameObject.transform.parent = base.transform;
		if (m_connectedBody == null)
		{
			m_connectedBody = gameObject.GetComponent<Rigidbody>();
			m_localConnectionPoint = new Vector3(0f, 0.5f, 0f);
			m_remoteConnectionPoint = gameObject.transform.InverseTransformPoint(base.transform.position - 0.5f * base.transform.up);
		}
		else
		{
			Vector3 position = m_connectedBody.position + (base.transform.position - m_connectedBody.position).normalized * 0.5f;
			m_connectedBody = gameObject.GetComponent<Rigidbody>();
			m_connectedBody.position = position;
			m_localConnectionPoint = new Vector3(0f, 0.5f, 0f);
			m_remoteConnectionPoint = gameObject.transform.InverseTransformPoint(base.transform.position - 0.5f * base.transform.up);
		}
	}
}
