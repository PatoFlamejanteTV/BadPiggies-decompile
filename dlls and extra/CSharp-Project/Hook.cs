using UnityEngine;

public class Hook : MonoBehaviour
{
	public enum AttachType
	{
		None,
		Static,
		Dynamic
	}

	private Vector3 m_lockedPosition;

	private Quaternion m_lockedRotation;

	private bool m_stickingAllowed = true;

	private Rigidbody m_rigidbody;

	private FixedJoint m_dynamicJoint;

	private AttachType attachType;

	[SerializeField]
	private BasePart.JointConnectionStrength m_dynamicAttachJointStrength;

	public void Awake()
	{
		m_rigidbody = GetComponent<Rigidbody>();
		attachType = AttachType.None;
	}

	private void FixedUpdate()
	{
		float num = m_rigidbody.velocity.magnitude * Time.fixedDeltaTime;
		if (num > 0.1f && Physics.Raycast(new Ray(base.transform.position, base.transform.right), out var hitInfo, num, ~((LayerMask)(1 << LayerMask.NameToLayer("Light"))).value) && (hitInfo.collider.tag == "Dynamic" || hitInfo.collider.tag == "Contraption"))
		{
			base.transform.parent = null;
			attachType = AttachType.Dynamic;
			if (INSettings.GetBool(INFeature.StableHook) && hitInfo.rigidbody != null)
			{
				m_rigidbody.velocity = hitInfo.rigidbody.velocity;
			}
			else
			{
				m_rigidbody.velocity = Vector3.zero;
			}
			m_rigidbody.angularVelocity = Vector3.zero;
			base.transform.position = hitInfo.point;
			if (hitInfo.rigidbody != null)
			{
				AttachToRigidbody(hitInfo.rigidbody);
			}
		}
	}

	private void AttachToRigidbody(Rigidbody rb)
	{
		if (m_dynamicJoint != null)
		{
			Object.Destroy(m_dynamicJoint);
		}
		m_dynamicJoint = rb.gameObject.AddComponent<FixedJoint>();
		m_dynamicJoint.connectedBody = m_rigidbody;
		m_dynamicJoint.breakForce = GetJointConnectionStrength(m_dynamicAttachJointStrength);
		m_dynamicJoint.enablePreprocessing = false;
	}

	public float GetJointConnectionStrength(BasePart.JointConnectionStrength strength)
	{
		return strength switch
		{
			BasePart.JointConnectionStrength.Weak => WPFMonoBehaviour.gameData.m_jointConnectionStrengthWeak, 
			BasePart.JointConnectionStrength.Normal => WPFMonoBehaviour.gameData.m_jointConnectionStrengthNormal, 
			BasePart.JointConnectionStrength.High => WPFMonoBehaviour.gameData.m_jointConnectionStrengthHigh, 
			BasePart.JointConnectionStrength.Extreme => WPFMonoBehaviour.gameData.m_jointConnectionStrengthExtreme, 
			BasePart.JointConnectionStrength.HighlyExtreme => WPFMonoBehaviour.gameData.m_jointConnectionStrengthHighlyExtreme, 
			_ => 0f, 
		};
	}

	public void OnCollisionEnter(Collision coll)
	{
		if (coll.collider.tag == "Collectable" || coll.collider.tag == "DynamicCollectable")
		{
			return;
		}
		if (coll != null && attachType == AttachType.None && m_stickingAllowed)
		{
			base.transform.parent = null;
			if (coll.rigidbody != null)
			{
				AttachToRigidbody(coll.rigidbody);
				attachType = AttachType.Dynamic;
			}
			else
			{
				m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				attachType = AttachType.Static;
			}
			m_rigidbody.velocity = Vector3.zero;
			m_rigidbody.angularVelocity = Vector3.zero;
			if (coll.contacts.Length != 0)
			{
				base.transform.position = coll.contacts[0].point;
			}
		}
		if (coll.transform.tag == "Goal")
		{
			Physics.IgnoreCollision(coll.collider, GetComponent<Collider>());
		}
	}

	public void OnEnable()
	{
		attachType = AttachType.None;
		allowSticking(allow: true);
		if (base.transform.parent != null)
		{
			base.transform.position = base.transform.parent.position;
		}
	}

	public AttachType GetAttachType()
	{
		return attachType;
	}

	public void SetActive(bool active)
	{
		base.gameObject.SetActive(active);
	}

	public void Reset()
	{
		m_rigidbody.constraints = (RigidbodyConstraints)56;
		if (m_dynamicJoint != null)
		{
			Object.Destroy(m_dynamicJoint);
		}
		allowSticking(allow: false);
		attachType = AttachType.None;
	}

	public void allowSticking(bool allow)
	{
		m_stickingAllowed = allow;
		GetComponent<Collider>().isTrigger = !allow;
	}
}
