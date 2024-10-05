using UnityEngine;

public class BalloonBalancer : MonoBehaviour
{
	private int m_balloonCount;

	[SerializeField]
	private ConfigurableJoint m_joint;

	private void Update()
	{
		if ((bool)m_joint)
		{
			m_joint.connectedAnchor = base.transform.position;
		}
	}

	public void AddBalloon()
	{
		m_balloonCount++;
		if (!m_joint)
		{
			ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
			configurableJoint.configuredInWorldSpace = false;
			configurableJoint.autoConfigureConnectedAnchor = false;
			configurableJoint.anchor = Vector3.zero;
			configurableJoint.axis = Vector3.forward;
			configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
			configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
			configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
			SoftJointLimit lowAngularXLimit = configurableJoint.lowAngularXLimit;
			lowAngularXLimit.limit = -1f;
			configurableJoint.lowAngularXLimit = lowAngularXLimit;
			SoftJointLimitSpring angularXLimitSpring = configurableJoint.angularXLimitSpring;
			angularXLimitSpring.spring = 100f;
			configurableJoint.angularXLimitSpring = angularXLimitSpring;
			configurableJoint.enablePreprocessing = false;
			m_joint = configurableJoint;
		}
	}

	public void RemoveBalloon()
	{
		m_balloonCount--;
		if ((bool)m_joint && m_balloonCount == 0)
		{
			Object.Destroy(m_joint);
			m_joint = null;
		}
	}

	public void Configure(float powerFactor)
	{
		if ((bool)m_joint)
		{
			SoftJointLimitSpring angularXLimitSpring = m_joint.angularXLimitSpring;
			angularXLimitSpring.spring = (float)m_balloonCount * 20f * powerFactor;
			m_joint.angularXLimitSpring = angularXLimitSpring;
		}
	}
}
