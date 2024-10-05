using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BridgeStep : LevelRigidbody
{
	[HideInInspector]
	[SerializeField]
	private bool m_isKinematic;

	private void Awake()
	{
		_ = breakForce;
		_ = 0.0001f;
		m_transform = base.transform;
		m_hingeJoint = GetComponent<HingeJoint>();
	}

	private void OnDataLoaded()
	{
	}

	public void Init(bool isKinematic)
	{
		m_isKinematic = isKinematic;
		freezeOnEnd = false;
		SaveState();
		LoadState();
	}

	private void SaveState()
	{
		m_originalPosition = base.transform.localPosition;
		m_originalRotation = base.transform.localRotation;
		if ((bool)m_hingeJoint)
		{
			if (m_originalHingeJointValues == null)
			{
				m_originalHingeJointValues = new HingeJointValues();
			}
			m_originalHingeJointValues.connectedBody = m_hingeJoint.connectedBody;
			m_originalHingeJointValues.anchor = m_hingeJoint.anchor;
			m_originalHingeJointValues.axis = m_hingeJoint.axis;
			m_originalHingeJointValues.connectedAnchor = m_hingeJoint.connectedAnchor;
			m_originalHingeJointValues.autoConfigureConnectedAnchor = m_hingeJoint.autoConfigureConnectedAnchor;
			m_originalHingeJointValues.useMotor = m_hingeJoint.useMotor;
			m_originalHingeJointValues.motor = m_hingeJoint.motor;
			m_originalHingeJointValues.useSpring = m_hingeJoint.useSpring;
			m_originalHingeJointValues.spring = m_hingeJoint.spring;
			m_originalHingeJointValues.useLimits = m_hingeJoint.useLimits;
			m_originalHingeJointValues.limits = m_hingeJoint.limits;
			m_originalHingeJointValues.breakForce = m_hingeJoint.breakForce;
			m_originalHingeJointValues.breakTorque = m_hingeJoint.breakTorque;
		}
	}

	public void LoadState()
	{
		StartCoroutine(LoadStateRoutine());
	}

	private IEnumerator LoadStateRoutine()
	{
		base.rigidbody.isKinematic = true;
		yield return new WaitForFixedUpdate();
		ResetTransform();
		ResetJoints();
		yield return new WaitForFixedUpdate();
		ResetRigidbody();
	}

	private void ResetTransform()
	{
		m_transform = m_transform ?? base.transform;
		m_transform.localPosition = m_originalPosition;
		m_transform.localRotation = m_originalRotation;
	}

	private void ResetJoints()
	{
		if (m_hingeJoint == null && m_originalHingeJointValues != null)
		{
			m_hingeJoint = base.gameObject.AddComponent<HingeJoint>();
		}
		if (m_originalHingeJointValues != null && !(m_hingeJoint == null))
		{
			m_hingeJoint.autoConfigureConnectedAnchor = m_originalHingeJointValues.autoConfigureConnectedAnchor;
			m_hingeJoint.connectedBody = m_originalHingeJointValues.connectedBody;
			m_hingeJoint.anchor = m_originalHingeJointValues.anchor;
			m_hingeJoint.axis = m_originalHingeJointValues.axis;
			m_hingeJoint.connectedAnchor = m_originalHingeJointValues.connectedAnchor;
			m_hingeJoint.motor = m_originalHingeJointValues.motor;
			m_hingeJoint.useMotor = m_originalHingeJointValues.useMotor;
			m_hingeJoint.spring = m_originalHingeJointValues.spring;
			m_hingeJoint.useSpring = m_originalHingeJointValues.useSpring;
			m_hingeJoint.limits = m_originalHingeJointValues.limits;
			m_hingeJoint.useLimits = m_originalHingeJointValues.useLimits;
			m_hingeJoint.breakForce = m_originalHingeJointValues.breakForce;
			m_hingeJoint.breakTorque = m_originalHingeJointValues.breakTorque;
			m_hingeJoint.enableCollision = m_originalHingeJointValues.enableCollision;
			m_hingeJoint.enablePreprocessing = m_originalHingeJointValues.enablePreprocessing;
		}
	}

	private void ResetRigidbody()
	{
		base.rigidbody.Sleep();
		base.rigidbody.isKinematic = m_isKinematic;
		if (!m_isKinematic)
		{
			base.rigidbody.WakeUp();
		}
	}
}
