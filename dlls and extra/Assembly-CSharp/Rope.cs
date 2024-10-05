using System.Collections.Generic;
using UnityEngine;

public class Rope : BasePart
{
	private struct Node
	{
		public GameObject gameObject;

		public Rigidbody rigidbody;

		public Joint joint;

		public Node(GameObject gameObject, Rigidbody rigidbody, Joint joint)
		{
			this.gameObject = gameObject;
			this.rigidbody = rigidbody;
			this.joint = joint;
		}
	}

	public GameObject m_segment;

	public Material m_material;

	public GameObject m_gridVisualizationNode;

	private BasePart m_leftPart;

	private BasePart m_rightPart;

	private List<Node> m_nodes = new List<Node>();

	private LineRenderer m_lineRenderer;

	private LineRenderer m_lineRenderer2;

	private bool m_ropeCut;

	private int m_cutIndex;

	private List<Vector3> m_splinePoints = new List<Vector3>();

	private float m_previousStretchFactor;

	public override Vector3 Position => m_nodes[m_nodes.Count / 2].gameObject.transform.position;

	public BasePart LeftPart => m_leftPart;

	public BasePart RightPart => m_rightPart;

	public GameObject FirstSegment => m_nodes[0].gameObject;

	public GameObject LastSegment => m_nodes[m_nodes.Count - 1].gameObject;

	public bool IsCut()
	{
		return m_ropeCut;
	}

	public bool IsLeftPart(Collider collider)
	{
		if (!m_ropeCut)
		{
			return true;
		}
		for (int i = 0; i < m_cutIndex; i++)
		{
			if (m_nodes[i].gameObject.GetComponent<Collider>() == collider)
			{
				return true;
			}
		}
		return false;
	}

	public static Vector3 PositionOnSpline(List<Vector3> controlPoints, float t)
	{
		int count = controlPoints.Count;
		int num = Mathf.FloorToInt(t * (float)(count - 1));
		Vector3 a = controlPoints[Mathf.Clamp(num - 1, 0, count - 1)];
		Vector3 b = controlPoints[Mathf.Clamp(num, 0, count - 1)];
		Vector3 c = controlPoints[Mathf.Clamp(num + 1, 0, count - 1)];
		Vector3 d = controlPoints[Mathf.Clamp(num + 2, 0, count - 1)];
		float i = t * (float)(count - 1) - (float)num;
		return MathsUtil.CatmullRomInterpolate(a, b, c, d, i);
	}

	public override void Awake()
	{
		base.Awake();
		if ((bool)m_gridVisualizationNode)
		{
			m_gridVisualizationNode.SetActive(value: true);
		}
	}

	private void Start()
	{
		if ((bool)base.rigidbody)
		{
			base.rigidbody.isKinematic = true;
			base.rigidbody.mass = 0f;
		}
		m_lineRenderer = GetComponent<LineRenderer>();
		if (!m_lineRenderer)
		{
			m_lineRenderer = base.gameObject.AddComponent<LineRenderer>();
		}
		m_lineRenderer.useWorldSpace = true;
		m_lineRenderer.SetWidth(0.2f, 0.2f);
		Color color = new Color(255f, 60f, 90f);
		m_lineRenderer.SetColors(color, color);
		m_lineRenderer.sharedMaterial = m_material;
	}

	private void CreateSecondLineRenderer()
	{
		GameObject obj = new GameObject();
		obj.name = "LineRenderer2";
		obj.transform.parent = base.transform;
		m_lineRenderer2 = obj.AddComponent<LineRenderer>();
		m_lineRenderer2.useWorldSpace = true;
		m_lineRenderer2.SetWidth(0.2f, 0.2f);
		Color color = new Color(255f, 60f, 90f);
		m_lineRenderer2.SetColors(color, color);
		m_lineRenderer2.sharedMaterial = m_material;
	}

	public override void Initialize()
	{
		if ((bool)RightPart && (bool)RightPart.GetComponent<Rope>())
		{
			Node node = m_nodes[m_nodes.Count - 1];
			node.gameObject.transform.position = RightPart.GetComponent<Rope>().FirstSegment.transform.position;
			CreateJoint(node.gameObject, RightPart.GetComponent<Rope>().FirstSegment.GetComponent<Rigidbody>());
		}
		base.contraption.ChangeOneShotPartAmount(BasePart.BaseType(m_partType), EffectDirection(), 1);
		if ((bool)m_gridVisualizationNode)
		{
			m_gridVisualizationNode.SetActive(value: false);
		}
	}

	public int FindInteractiveSegment(Vector3 position)
	{
		int result = -1;
		float num = 1000f;
		for (int i = 0; i < m_nodes.Count; i++)
		{
			Vector3 position2 = m_nodes[i].gameObject.transform.position;
			position2.z = 0f;
			float num2 = Vector3.Distance(position2, position);
			if (num2 <= m_interactiveRadius && num2 < num)
			{
				num = num2;
				result = i;
				result = Mathf.Clamp(result, 2, m_nodes.Count - 2);
			}
		}
		return result;
	}

	public override bool IsInInteractiveRadius(Vector3 position)
	{
		return FindInteractiveSegment(position) != -1;
	}

	protected override void OnTouch(bool hasPosition, Vector3 touchPosition)
	{
		if (!m_ropeCut)
		{
			int num = -1;
			if (hasPosition)
			{
				num = FindInteractiveSegment(touchPosition);
			}
			if (num != -1)
			{
				Cut(num);
			}
			else
			{
				Cut(m_nodes.Count / 2);
			}
		}
	}

	private void Update()
	{
		if (!m_ropeCut)
		{
			RenderRope(m_lineRenderer, 0, m_nodes.Count);
			return;
		}
		RenderRope(m_lineRenderer, 0, m_cutIndex);
		RenderRope(m_lineRenderer2, m_cutIndex, m_nodes.Count);
	}

	private void RenderRope(LineRenderer lineRenderer, int startIndex, int endIndex)
	{
		m_splinePoints.Clear();
		Rope rope = null;
		if ((bool)m_leftPart)
		{
			rope = m_leftPart.GetComponent<Rope>();
		}
		for (int i = startIndex; i < endIndex; i++)
		{
			Vector3 position = m_nodes[i].rigidbody.gameObject.transform.position;
			if (i == 0 && (bool)rope)
			{
				position = rope.LastSegment.transform.position;
			}
			position.z = -1f;
			m_splinePoints.Add(position);
		}
		if (m_splinePoints.Count > 1)
		{
			lineRenderer.SetVertexCount(20);
			for (int j = 0; j < 20; j++)
			{
				float t = (float)j / 19f;
				lineRenderer.SetPosition(j, PositionOnSpline(m_splinePoints, t));
			}
		}
		else
		{
			lineRenderer.SetVertexCount(0);
		}
	}

	private Joint CreateJoint(GameObject target, Rigidbody connectedBody)
	{
		HingeJoint hingeJoint = target.AddComponent<HingeJoint>();
		hingeJoint.connectedBody = connectedBody;
		hingeJoint.axis = Vector3.forward;
		hingeJoint.breakForce = float.PositiveInfinity;
		hingeJoint.breakTorque = float.PositiveInfinity;
		hingeJoint.enablePreprocessing = false;
		AddConnectedBodyToJoint(hingeJoint, connectedBody);
		return hingeJoint;
	}

	private void AddConnectedBodyToJoint(Joint joint, Rigidbody connectedBody)
	{
		joint.connectedBody = connectedBody;
		if (connectedBody != null)
		{
			joint.anchor = connectedBody.position - joint.transform.position;
		}
	}

	public void Create(BasePart leftPart, BasePart rightPart)
	{
		m_leftPart = leftPart;
		m_rightPart = rightPart;
		Vector3 position = base.transform.position - 0.5f * base.transform.right;
		Quaternion rotation = base.transform.rotation;
		float num = 74.4f;
		rotation = Quaternion.AngleAxis(0f - num, Vector3.forward);
		if (m_gridRotation != 0)
		{
			rotation *= Quaternion.AngleAxis(-90f, Vector3.forward);
		}
		int @int = INSettings.GetInt(INFeature.RopeSegmentCount);
		Joint joint = null;
		if ((bool)leftPart && !leftPart.GetComponent<Rope>())
		{
			joint = CreateJoint(leftPart.gameObject, null);
		}
		for (int i = 0; i < @int; i++)
		{
			if (i == @int - 1 && (bool)rightPart && !rightPart.GetComponent<Rope>())
			{
				position = base.transform.position + 0.5f * base.transform.right;
			}
			GameObject gameObject = Object.Instantiate(m_segment, position, rotation);
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			if (INSettings.GetBool(INFeature.CollidableRope) && (m_partTier == PartTier.Rare || m_partTier == PartTier.Epic))
			{
				gameObject.layer = LayerMask.NameToLayer("Contraption");
			}
			if (joint != null)
			{
				AddConnectedBodyToJoint(joint, component);
				joint = null;
			}
			m_nodes.Add(new Node(gameObject, component, joint));
			if (i == @int - 1 && (bool)rightPart && !rightPart.GetComponent<Rope>())
			{
				joint = CreateJoint(rightPart.gameObject, component);
			}
			position += 0.5f * (rotation * Vector3.right);
			rotation = ((i % 2 != 0) ? Quaternion.AngleAxis(0f - num, Vector3.forward) : Quaternion.AngleAxis(num, Vector3.forward));
			if (m_gridRotation != 0)
			{
				rotation *= Quaternion.AngleAxis(-90f, Vector3.forward);
			}
			gameObject.transform.parent = base.transform;
		}
		if ((bool)base.renderer)
		{
			base.renderer.enabled = false;
		}
	}

	private void Cut(int nodeIndex)
	{
		if (!m_ropeCut)
		{
			base.contraption.ChangeOneShotPartAmount(BasePart.BaseType(m_partType), EffectDirection(), -1);
			m_ropeCut = true;
			m_cutIndex = nodeIndex;
			CreateSecondLineRenderer();
			int cutIndex = m_cutIndex;
			m_lineRenderer.material.mainTextureScale = new Vector2(20f * (float)(cutIndex - 1) / (float)(m_nodes.Count - 1), 1f);
			cutIndex = m_nodes.Count - cutIndex;
			m_lineRenderer2.material.mainTextureScale = new Vector2(20f * (float)(cutIndex - 1) / (float)(m_nodes.Count - 1), 1f);
			base.contraption.FindComponentsConnectedByRope();
		}
	}

	private float Sigmoid(float t)
	{
		return 1f / (1f + Mathf.Exp(0f - t));
	}

	private float ResponseCurve(float t)
	{
		return (Sigmoid(8f * (t - 0.5f)) - 0.01798f) / 0.9641f;
	}

	public void FixedUpdate()
	{
		if (m_nodes.Count == 0)
		{
			return;
		}
		float num = Mathf.Max(m_nodes[0].rigidbody.velocity.magnitude, m_nodes[m_nodes.Count - 1].rigidbody.velocity.magnitude);
		Rope rope = ((!(m_rightPart != null)) ? null : m_rightPart.GetComponent<Rope>());
		if ((bool)rope)
		{
			Vector3 position = m_nodes[m_nodes.Count - 1].rigidbody.position;
			Vector3 position2 = rope.FirstSegment.GetComponent<Rigidbody>().position;
			if (Vector3.Distance(position, position2) > 0.1f)
			{
				rope.FirstSegment.GetComponent<Rigidbody>().position = position;
			}
		}
		for (int i = 0; i < m_nodes.Count; i++)
		{
			Rigidbody rigidbody = m_nodes[i].rigidbody;
			Rigidbody rigidbody2 = ((i != 0) ? m_nodes[i - 1].rigidbody : rigidbody);
			Rigidbody rigidbody3 = ((i != m_nodes.Count - 1) ? m_nodes[i + 1].rigidbody : rigidbody);
			float num2 = (rigidbody.velocity - 0.4f * rigidbody2.velocity - 0.4f * rigidbody3.velocity).sqrMagnitude;
			if (INSettings.GetBool(INFeature.StableRope))
			{
				float num3 = 50f / Time.deltaTime * rigidbody.mass * rigidbody.velocity.magnitude;
				if (num2 > num3)
				{
					num2 = num3;
				}
			}
			rigidbody.AddForce(-0.02f * num2 * rigidbody.velocity.normalized, ForceMode.Force);
			rigidbody.drag = 0.5f + 6f / (1f + 4f * num * num);
		}
		for (int j = 1; j < m_nodes.Count; j++)
		{
			if (!m_ropeCut || j != m_cutIndex)
			{
				Rigidbody rigidbody4 = m_nodes[j - 1].rigidbody;
				Rigidbody rigidbody5 = m_nodes[j].rigidbody;
				Vector3 vector = rigidbody5.gameObject.transform.position - rigidbody4.gameObject.transform.position;
				float magnitude = vector.magnitude;
				if (magnitude > 0.5f)
				{
					magnitude -= 0.5f;
					magnitude = ((!(magnitude < 1f)) ? Mathf.Pow(magnitude, 2f) : Mathf.Pow(magnitude, 0.75f));
					magnitude = Mathf.Clamp(magnitude, 0f, 2f);
					vector = magnitude * vector.normalized;
					Vector3 vector2 = -vector;
					rigidbody4.AddForce(70f * vector, ForceMode.Force);
					rigidbody5.AddForce(70f * vector2, ForceMode.Force);
				}
				else if (magnitude < 0.4f)
				{
					rigidbody4.AddForce(50f * (magnitude - 0.4f) * vector.normalized, ForceMode.Force);
					rigidbody5.AddForce(-50f * (magnitude - 0.4f) * vector.normalized, ForceMode.Force);
				}
			}
		}
		if (!m_ropeCut)
		{
			PullEndpoints();
		}
	}

	private void PullEndpoints()
	{
		float num = 0f;
		for (int i = 1; i < m_nodes.Count; i++)
		{
			Rigidbody rigidbody = m_nodes[i - 1].rigidbody;
			Rigidbody rigidbody2 = m_nodes[i].rigidbody;
			if (!rigidbody || !rigidbody2)
			{
				continue;
			}
			float num2 = Vector3.Distance(rigidbody.position, rigidbody2.position);
			if (!INSettings.GetBool(INFeature.UnbreakableRope))
			{
				if (i > 1 && i < m_nodes.Count - 2)
				{
					if (num2 > 1.25f)
					{
						Cut(i);
						return;
					}
				}
				else if (num2 > 1.75f)
				{
					Cut(i);
					return;
				}
			}
			num += num2;
		}
		float num3 = 0.5f * (float)m_nodes.Count;
		float num4 = num / (1.18f * num3) - 1f;
		float num5 = 0.4f;
		num4 = num5 * ResponseCurve(num4 / num5);
		num4 = Mathf.Clamp(num4, 0f, num5);
		float num6 = num4 - m_previousStretchFactor;
		m_previousStretchFactor = num4;
		if (num > 1.18f * num3)
		{
			Rigidbody rigidbody3 = m_nodes[0].rigidbody;
			Rigidbody obj = m_nodes[1].rigidbody;
			Vector3 position = rigidbody3.position;
			Vector3 position2 = obj.position;
			if (num6 < 0f)
			{
				num4 *= 0.7f;
			}
			float num7 = Mathf.Clamp(750f * num4, 0f, 300f);
			Vector3 force = num7 * (position2 - position).normalized;
			if ((bool)LeftPart)
			{
				rigidbody3.AddForceAtPosition(force, position, ForceMode.Force);
			}
			rigidbody3 = m_nodes[m_nodes.Count - 1].rigidbody;
			Rigidbody obj2 = m_nodes[m_nodes.Count - 2].rigidbody;
			position = rigidbody3.position;
			position2 = obj2.position;
			force = num7 * (position2 - position).normalized;
			if ((bool)RightPart)
			{
				rigidbody3.AddForceAtPosition(force, position, ForceMode.Force);
			}
		}
	}
}
