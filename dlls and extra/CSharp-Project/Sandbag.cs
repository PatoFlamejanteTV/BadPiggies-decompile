using UnityEngine;

public class Sandbag : BasePart
{
	public bool m_inWorldCoordinates = true;

	public Vector3 m_direction = Vector3.up;

	public int m_numberOfBalloons = 1;

	public bool m_dropped;

	public Material m_stringMaterial;

	public GameObject m_actualVisualizationNode;

	public GameObject m_gridVisualizationNode;

	protected BasePart m_connectedPart;

	protected Vector3 m_connectedLocalPos;

	public override void Awake()
	{
		base.Awake();
		if ((bool)m_actualVisualizationNode && (bool)m_gridVisualizationNode)
		{
			m_actualVisualizationNode.SetActive(value: false);
			m_gridVisualizationNode.SetActive(value: true);
		}
	}

	public override bool IsIntegralPart()
	{
		return false;
	}

	public bool IsAttached()
	{
		return !m_dropped;
	}

	public override void PrePlaced()
	{
		base.PrePlaced();
		if (INSettings.GetBool(INFeature.RotatableSandbag))
		{
			m_autoAlign = AutoAlignType.Rotate;
		}
	}

	public override void Initialize()
	{
		if ((bool)m_actualVisualizationNode && (bool)m_gridVisualizationNode)
		{
			m_gridVisualizationNode.SetActive(value: false);
			m_actualVisualizationNode.SetActive(value: true);
		}
		int i = 1;
		int num = 0;
		int num2 = 1;
		int @int = INSettings.GetInt(INFeature.SandbagConnectionDistance);
		if (INSettings.GetBool(INFeature.RotatableSandbag))
		{
			if (m_gridRotation == GridRotation.Deg_90)
			{
				num = -1;
				num2 = 0;
			}
			else if (m_gridRotation == GridRotation.Deg_180)
			{
				num = 0;
				num2 = -1;
			}
			else if (m_gridRotation == GridRotation.Deg_270)
			{
				num = 1;
				num2 = 0;
			}
		}
		for (; i < @int + 1; i++)
		{
			if ((bool)m_connectedPart)
			{
				break;
			}
			m_connectedPart = base.contraption.FindPartAt(m_coordX + i * num, m_coordY + i * num2);
			if ((bool)m_connectedPart && !m_connectedPart.IsPartOfChassis() && m_connectedPart.m_partType != PartType.Pig)
			{
				m_connectedPart = null;
			}
		}
		if ((bool)m_connectedPart)
		{
			m_dropped = false;
			base.contraption.ChangeOneShotPartAmount(BasePart.BaseType(m_partType), EffectDirection(), 1);
		}
		else
		{
			m_dropped = true;
			base.gameObject.layer = LayerMask.NameToLayer("DroppedSandbag");
		}
		m_partType = PartType.Sandbag;
		if (m_numberOfBalloons > 1)
		{
			GameObject obj = Object.Instantiate(base.gameObject);
			obj.transform.position = base.transform.position;
			Sandbag component = obj.GetComponent<Sandbag>();
			component.m_numberOfBalloons = m_numberOfBalloons - 1;
			base.contraption.AddRuntimePart(component);
			obj.transform.parent = base.contraption.transform;
		}
		if (!base.gameObject.GetComponent<SphereCollider>())
		{
			SphereCollider sphereCollider = base.gameObject.AddComponent<SphereCollider>();
			sphereCollider.radius = 0.13f;
			sphereCollider.center = new Vector3(0f, -0.1f, 0f);
		}
		if (!base.rigidbody)
		{
			base.rigidbody = base.gameObject.AddComponent<Rigidbody>();
		}
		base.rigidbody.mass = m_mass;
		base.rigidbody.drag = 1f;
		base.rigidbody.angularDrag = 10f;
		base.rigidbody.constraints = (RigidbodyConstraints)56;
		if ((bool)m_connectedPart)
		{
			Vector3 position = base.transform.position;
			base.transform.position = m_connectedPart.transform.position - Vector3.up * 0.5f;
			SpringJoint springJoint = base.gameObject.AddComponent<SpringJoint>();
			springJoint.connectedBody = m_connectedPart.rigidbody;
			m_connectedLocalPos = m_connectedPart.transform.InverseTransformPoint(base.transform.position);
			Vector3 vector;
			float maxDistance;
			switch (m_numberOfBalloons)
			{
			default:
				vector = new Vector3(0f, -0.35f, -0.03f);
				maxDistance = 0.65f;
				break;
			case 2:
				vector = new Vector3(0.35f, -0.3f, -0.02f);
				maxDistance = 0.55f;
				break;
			case 1:
				vector = new Vector3(-0.15f, -0.15f, -0.01f);
				maxDistance = 0.5f;
				break;
			}
			springJoint.minDistance = 0f;
			springJoint.maxDistance = maxDistance;
			springJoint.anchor = Vector3.up * 0.5f;
			springJoint.spring = 100f;
			springJoint.damper = 10f;
			base.transform.position = position + vector;
			LineRenderer lineRenderer = base.gameObject.AddComponent<LineRenderer>();
			lineRenderer.material = m_stringMaterial;
			lineRenderer.SetVertexCount(2);
			lineRenderer.SetWidth(0.05f, 0.05f);
			lineRenderer.SetColors(Color.black, Color.black);
		}
		else
		{
			Vector3 vector2 = m_numberOfBalloons switch
			{
				2 => new Vector3(0.35f, -0.3f, -0.02f), 
				1 => new Vector3(-0.15f, -0.15f, -0.01f), 
				_ => new Vector3(0f, -0.35f, -0.03f), 
			};
			base.transform.position += vector2;
		}
	}

	protected override void OnTouch()
	{
		Drop();
	}

	public void Drop()
	{
		if (!m_dropped)
		{
			m_dropped = true;
			SpringJoint component = GetComponent<SpringJoint>();
			base.contraption.ChangeOneShotPartAmount(BasePart.BaseType(m_partType), EffectDirection(), -1);
			base.gameObject.layer = LayerMask.NameToLayer("DroppedSandbag");
			if ((bool)component && (bool)component.connectedBody)
			{
				component.connectedBody.AddForce(5f * Vector3.up, ForceMode.Impulse);
				base.rigidbody.AddForce(-4f * Vector3.up, ForceMode.Impulse);
			}
			if ((bool)component)
			{
				Object.Destroy(component);
			}
			LineRenderer component2 = GetComponent<LineRenderer>();
			if ((bool)component2)
			{
				Object.Destroy(component2);
			}
		}
	}

	public new void LateUpdate()
	{
		base.LateUpdate();
		SpringJoint component = GetComponent<SpringJoint>();
		if ((bool)component)
		{
			LineRenderer component2 = GetComponent<LineRenderer>();
			Vector3 position = base.transform.position + base.transform.up * 0.4f;
			if ((bool)component.connectedBody)
			{
				Vector3 position2 = component.connectedBody.transform.TransformPoint(m_connectedLocalPos);
				component2.SetPosition(0, position);
				component2.SetPosition(1, position2);
			}
		}
	}
}
