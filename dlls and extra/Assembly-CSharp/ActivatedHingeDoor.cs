using UnityEngine;

public class ActivatedHingeDoor : MonoBehaviour
{
	private enum RotationState
	{
		Kinematic,
		Opening,
		Closing
	}

	[SerializeField]
	private int m_pressureID;

	[SerializeField]
	private float m_activatedAngle;

	[SerializeField]
	private bool m_inverted;

	[SerializeField]
	private float m_torque = 100f;

	private Transform m_door;

	private Rigidbody m_doorRigidbody;

	private Quaternion m_startingRotation;

	private Quaternion m_activatedRotation;

	private HingeJoint m_joint;

	private RotationState m_rotationState = RotationState.Closing;

	private void OnDataLoaded()
	{
		m_door = base.transform.Find("Door");
		m_doorRigidbody = m_door.GetComponent<Rigidbody>();
		m_startingRotation = m_door.localRotation;
		m_joint = m_door.GetComponent<HingeJoint>();
		JointLimits limits = default(JointLimits);
		if (m_activatedAngle > 0f)
		{
			limits.max = m_activatedAngle;
			limits.min = 0f;
		}
		if (m_activatedAngle < 0f)
		{
			limits.max = 0f;
			limits.min = m_activatedAngle;
		}
		if ((bool)m_joint)
		{
			m_joint.limits = limits;
		}
		m_activatedRotation = Quaternion.AngleAxis(m_activatedAngle, Vector3.forward);
		EventManager.Connect<PressureButtonPressed>(OnPressurePlatePress);
		EventManager.Connect<PressureButtonReleased>(OnPressurePlateReleased);
		if (m_inverted)
		{
			m_rotationState = RotationState.Opening;
		}
		m_rotationState = RotationState.Kinematic;
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<PressureButtonPressed>(OnPressurePlatePress);
		EventManager.Disconnect<PressureButtonReleased>(OnPressurePlateReleased);
	}

	private void OnPressurePlatePress(PressureButtonPressed pressed)
	{
		if (m_pressureID == pressed.id)
		{
			if (m_inverted)
			{
				m_rotationState = RotationState.Closing;
			}
			else
			{
				m_rotationState = RotationState.Opening;
			}
		}
	}

	private void OnPressurePlateReleased(PressureButtonReleased released)
	{
		if (m_pressureID == released.id)
		{
			if (m_inverted)
			{
				m_rotationState = RotationState.Opening;
			}
			else
			{
				m_rotationState = RotationState.Closing;
			}
		}
	}

	private void FixedUpdate()
	{
		float num = Mathf.Abs(m_activatedAngle) - Mathf.Abs(m_joint.angle);
		switch (m_rotationState)
		{
		case RotationState.Kinematic:
			if (!m_doorRigidbody.isKinematic)
			{
				m_doorRigidbody.isKinematic = true;
			}
			break;
		case RotationState.Opening:
			m_doorRigidbody.isKinematic = false;
			if (num >= 1f)
			{
				m_doorRigidbody.AddTorque(m_activatedAngle * Vector3.forward * m_torque);
				break;
			}
			m_rotationState = RotationState.Kinematic;
			m_door.localRotation = m_activatedRotation;
			break;
		case RotationState.Closing:
			m_doorRigidbody.isKinematic = false;
			if (Mathf.Abs(m_activatedAngle) - num > 1f)
			{
				m_doorRigidbody.AddTorque((0f - m_activatedAngle) * Vector3.forward * m_torque);
				break;
			}
			m_rotationState = RotationState.Kinematic;
			m_door.localRotation = m_startingRotation;
			break;
		}
	}
}
