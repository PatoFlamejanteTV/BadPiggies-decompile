using System.Collections.Generic;
using UnityEngine;

public class HingePlate : BasePart
{
	public enum HingePlateType
	{
		Common,
		Track
	}

	public HingePlateType m_type;

	public int m_rotationIndex;

	private GameObject m_plate1;

	private GameObject m_plate2;

	private Rigidbody m_plateRigidbody1;

	private Rigidbody m_plateRigidbody2;

	private int m_componentIndex;

	private List<(BasePart, Rigidbody, int)> m_connectedParts1;

	private List<(BasePart, Rigidbody, int)> m_connectedParts2;

	private List<Joint> m_joints;

	private static readonly (int, int)[] s_directions = new(int, int)[8]
	{
		(1, 0),
		(1, 1),
		(0, 1),
		(-1, 1),
		(-1, 0),
		(-1, -1),
		(0, -1),
		(1, -1)
	};

	public static (int, int)[] Directions => s_directions;

	public GameObject LeftPlate => m_plate1;

	public GameObject RightPlate => m_plate2;

	public Rigidbody LeftPlateRigidbody => m_plateRigidbody1;

	public Rigidbody RightPlateRigidbody => m_plateRigidbody2;

	public List<(BasePart, Rigidbody, int)> LeftParts => m_connectedParts1;

	public List<(BasePart, Rigidbody, int)> RightParts => m_connectedParts2;

	public int RotationIndex
	{
		get
		{
			return m_rotationIndex;
		}
		set
		{
			int num = value % 6;
			float z = m_plate1.transform.localPosition.z;
			m_rotationIndex = num;
			m_gridRotation = (GridRotation)num;
			if (num == 0 || num == 1)
			{
				m_plate1.transform.localPosition = new Vector3(-0.25f, 0f, z);
				m_plate2.transform.localPosition = new Vector3(0.25f, 0f, z);
				base.transform.localRotation = Quaternion.AngleAxis(90 * num, Vector3.forward);
			}
			else
			{
				m_plate1.transform.localPosition = new Vector3(-0.25f, -0.5f, z);
				m_plate2.transform.localPosition = new Vector3(0.25f, -0.5f, z);
				base.transform.localRotation = Quaternion.AngleAxis(90 * (num - 2), Vector3.forward);
			}
		}
	}

	public new (int, int) Direction
	{
		get
		{
			int rotationIndex = m_rotationIndex;
			if (rotationIndex == 0 || rotationIndex == 1)
			{
				return s_directions[rotationIndex * 2];
			}
			return s_directions[(rotationIndex - 2) * 2];
		}
	}

	public override Vector3 Position => (m_plate1.transform.position + m_plate2.transform.position) * 0.5f;

	public override IEnumerable<Rigidbody> GetRigidbodies()
	{
		yield return m_plateRigidbody1;
		yield return m_plateRigidbody2;
	}

	public static void InitializeStatic()
	{
		List<HingePlate> list = new List<HingePlate>();
		foreach (BasePart part in Contraption.Instance.Parts)
		{
			if (part is HingePlate item)
			{
				list.Add(item);
			}
		}
		int count = list.Count;
		DisjointSet disjointSet = new DisjointSet(count);
		Dictionary<BasePart, int> dictionary = new Dictionary<BasePart, int>(count);
		for (int i = 0; i < count; i++)
		{
			dictionary[list[i]] = i;
		}
		for (int j = 0; j < count; j++)
		{
			HingePlate hingePlate = list[j];
			foreach (var leftPart in hingePlate.LeftParts)
			{
				if (dictionary.TryGetValue(leftPart.Item1, out var value))
				{
					disjointSet.Union(j, value);
				}
			}
			foreach (var rightPart in hingePlate.RightParts)
			{
				if (dictionary.TryGetValue(rightPart.Item1, out var value2))
				{
					disjointSet.Union(j, value2);
				}
			}
		}
		int componentCount;
		int[] componentIndexes = disjointSet.GetComponentIndexes(out componentCount);
		for (int k = 0; k < count; k++)
		{
			int componentIndex = componentIndexes[k];
			list[k].m_componentIndex = componentIndex;
		}
		List<HingePlate>[] array = new List<HingePlate>[componentCount];
		for (int l = 0; l < componentCount; l++)
		{
			array[l] = new List<HingePlate>();
		}
		foreach (HingePlate item2 in list)
		{
			HingePlate hingePlate2 = item2 as HingePlate;
			array[hingePlate2.m_componentIndex].Add(hingePlate2);
		}
		List<Collider> list2 = new List<Collider>();
		for (int m = 0; m < componentCount; m++)
		{
			foreach (HingePlate item3 in array[m])
			{
				if (item3.m_enclosedInto is Frame)
				{
					list2.Add(item3.m_enclosedInto.GetComponent<Collider>());
				}
			}
			foreach (HingePlate item4 in array[m])
			{
				Collider component = item4.m_plate1.GetComponent<Collider>();
				Collider component2 = item4.m_plate2.GetComponent<Collider>();
				foreach (Collider item5 in list2)
				{
					Physics.IgnoreCollision(component, item5);
					Physics.IgnoreCollision(component2, item5);
				}
			}
		}
	}

	public IEnumerable<BasePart> GetConnectedParts()
	{
		foreach (var item in m_connectedParts1)
		{
			yield return item.Item1;
		}
		foreach (var item2 in m_connectedParts2)
		{
			yield return item2.Item1;
		}
	}

	public override void Awake()
	{
		base.Awake();
		m_plate1 = base.transform.Find("Plate1").gameObject;
		m_plate2 = base.transform.Find("Plate2").gameObject;
		PhysicMaterial material = new PhysicMaterial
		{
			dynamicFriction = 0.7f,
			staticFriction = 0.7f,
			frictionCombine = PhysicMaterialCombine.Average
		};
		m_plate1.GetComponent<Collider>().material = material;
		m_plate2.GetComponent<Collider>().material = material;
	}

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override void EnsureRigidbody()
	{
		base.EnsureRigidbody();
		EnsureRigidbodyPlate();
		base.rigidbody.isKinematic = true;
	}

	public override void PrePlaced()
	{
		base.PrePlaced();
		m_autoAlign = (AutoAlignType)(-1);
	}

	public override void Initialize()
	{
		m_plateRigidbody1 = m_plate1.GetComponent<Rigidbody>();
		m_plateRigidbody2 = m_plate2.GetComponent<Rigidbody>();
		Create();
	}

	public override void SetRotation(GridRotation rotation)
	{
		SetCustomRotation((int)rotation);
	}

	public override int GetCustomRotation()
	{
		return m_rotationIndex;
	}

	public override void SetCustomRotation(int rotation)
	{
		int num = rotation % 6;
		float z = m_plate1.transform.localPosition.z;
		m_rotationIndex = num;
		m_gridRotation = (GridRotation)num;
		if (num == 0 || num == 1)
		{
			m_plate1.transform.localPosition = new Vector3(-0.25f, 0f, z);
			m_plate2.transform.localPosition = new Vector3(0.25f, 0f, z);
			base.transform.localRotation = Quaternion.AngleAxis(90 * num, Vector3.forward);
		}
		else
		{
			m_plate1.transform.localPosition = new Vector3(-0.25f, -0.5f, z);
			m_plate2.transform.localPosition = new Vector3(0.25f, -0.5f, z);
			base.transform.localRotation = Quaternion.AngleAxis(90 * (num - 2), Vector3.forward);
		}
	}

	private void EnsureRigidbodyPlate()
	{
		EnsurePlateRigidbody(m_plate1);
		EnsurePlateRigidbody(m_plate2);
	}

	private void EnsurePlateRigidbody(GameObject gameObject)
	{
		Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
		if (rigidbody == null)
		{
			rigidbody = gameObject.AddComponent<Rigidbody>();
		}
		rigidbody.constraints = (RigidbodyConstraints)56;
		rigidbody.mass = 1f;
		rigidbody.drag = 0.2f;
		rigidbody.angularDrag = 0.05f;
		rigidbody.useGravity = true;
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
	}

	private bool ValidatePart(ref BasePart part)
	{
		if (part == null)
		{
			return false;
		}
		if (part.m_enclosedPart != null && part.m_enclosedPart is HingePlate)
		{
			part = part.m_enclosedPart;
		}
		return true;
	}

	private bool CanConnectToLeft(BasePart part, int x, int y, out bool flip, out int angle)
	{
		flip = false;
		angle = 0;
		HingePlate hingePlate = part as HingePlate;
		int rotationIndex = m_rotationIndex;
		if ((object)hingePlate == null)
		{
			(int, int) direction = Direction;
			PartType partType = part.m_partType;
			if (m_type == HingePlateType.Common && (partType == PartType.WoodenFrame || partType == PartType.MetalFrame || partType == PartType.Kicker) && x == -direction.Item1)
			{
				return y == -direction.Item2;
			}
			return false;
		}
		int rotationIndex2 = hingePlate.m_rotationIndex;
		switch (rotationIndex)
		{
		case 0:
			if (x == -1 && y == 0)
			{
				return rotationIndex2 == 0;
			}
			return false;
		case 1:
			if (x == 0 && y == -1)
			{
				return rotationIndex2 == 1;
			}
			return false;
		default:
		{
			rotationIndex -= 2;
			rotationIndex2 -= 2;
			int num = (rotationIndex2 - rotationIndex + 4) % 4;
			(int, int) tuple = s_directions[(4 + rotationIndex * 2) % 8];
			(int, int) tuple2 = s_directions[(5 + rotationIndex * 2) % 8];
			(int, int) tuple3 = s_directions[(6 + rotationIndex * 2) % 8];
			flip = (x == tuple.Item1 && y == tuple.Item2 && num == 1) || (x == tuple2.Item1 && y == tuple2.Item2 && num == 2) || (x == tuple3.Item1 && y == tuple3.Item2 && num == 3);
			angle = (flip ? (-90) : 90) * ((num > 2) ? (num - 4) : num);
			if ((x != tuple.Item1 || y != tuple.Item2 || (num != 1 && num != 0)) && (x != tuple2.Item1 || y != tuple2.Item2 || (num != 2 && num != 1)))
			{
				if (x == tuple3.Item1 && y == tuple3.Item2)
				{
					return num == 3;
				}
				return false;
			}
			return true;
		}
		}
	}

	private bool CanConnectToRight(BasePart part, int x, int y, out bool flip, out int angle)
	{
		flip = false;
		angle = 0;
		HingePlate hingePlate = part as HingePlate;
		int rotationIndex = m_rotationIndex;
		if ((object)hingePlate == null)
		{
			(int, int) direction = Direction;
			if (m_type == HingePlateType.Common && (part is Frame || part is Kicker) && x == direction.Item1)
			{
				return y == direction.Item2;
			}
			return false;
		}
		int rotationIndex2 = hingePlate.m_rotationIndex;
		switch (rotationIndex)
		{
		case 0:
			if (x == 1 && y == 0)
			{
				return rotationIndex2 == 0;
			}
			return false;
		case 1:
			if (x == 0 && y == 1)
			{
				return rotationIndex2 == 1;
			}
			return false;
		default:
		{
			rotationIndex -= 2;
			rotationIndex2 -= 2;
			int num = (rotationIndex2 - rotationIndex + 4) % 4;
			(int, int) tuple = s_directions[(6 + rotationIndex * 2) % 8];
			(int, int) tuple2 = s_directions[(7 + rotationIndex * 2) % 8];
			(int, int) tuple3 = s_directions[rotationIndex * 2 % 8];
			flip = (x == tuple.Item1 && y == tuple.Item2 && num == 1) || (x == tuple2.Item1 && y == tuple2.Item2 && num == 2) || (x == tuple3.Item1 && y == tuple3.Item2 && num == 3);
			angle = (flip ? 90 : (-90)) * (num - 2);
			if ((x != tuple.Item1 || y != tuple.Item2 || num != 1) && (x != tuple2.Item1 || y != tuple2.Item2 || (num != 2 && num != 3)))
			{
				if (x == tuple3.Item1 && y == tuple3.Item2)
				{
					if (num != 3)
					{
						return num == 0;
					}
					return true;
				}
				return false;
			}
			return true;
		}
		}
	}

	public bool CanConnectTo(BasePart part)
	{
		int coordX = m_coordX;
		int coordY = m_coordY;
		if (ValidatePart(ref part))
		{
			if (CanConnectToLeft(part, part.m_coordX - coordX, part.m_coordY - coordY, out var flip, out var angle))
			{
				return true;
			}
			if (CanConnectToRight(part, part.m_coordX - coordX, part.m_coordY - coordY, out flip, out angle))
			{
				return true;
			}
		}
		return false;
	}

	public void Create()
	{
		int coordX = m_coordX;
		int coordY = m_coordY;
		GameObject plate = m_plate1;
		GameObject plate2 = m_plate2;
		Contraption contraption = base.contraption;
		m_connectedParts1 = new List<(BasePart, Rigidbody, int)>();
		m_connectedParts2 = new List<(BasePart, Rigidbody, int)>();
		m_joints = new List<Joint>();
		for (int i = 0; i < 8; i++)
		{
			(int, int) tuple = s_directions[i];
			BasePart part = contraption.FindPartAt(coordX + tuple.Item1, coordY + tuple.Item2);
			if (ValidatePart(ref part))
			{
				if (CanConnectToLeft(part, tuple.Item1, tuple.Item2, out var flip, out var angle))
				{
					Rigidbody component = ((!(part is HingePlate hingePlate)) ? part.gameObject : (flip ? hingePlate.m_plate1 : hingePlate.m_plate2)).GetComponent<Rigidbody>();
					m_connectedParts1.Add((part, component, angle));
				}
				if (CanConnectToRight(part, tuple.Item1, tuple.Item2, out flip, out angle))
				{
					Rigidbody component2 = ((!(part is HingePlate hingePlate2)) ? part.gameObject : (flip ? hingePlate2.m_plate2 : hingePlate2.m_plate1)).GetComponent<Rigidbody>();
					m_connectedParts2.Add((part, component2, angle));
				}
			}
		}
		CreateJoint(plate, plate2, JointType.FixedJoint, 0, new Vector3(0.25f, 0f));
		CreateJoint(plate2, plate, JointType.FixedJoint, 0, new Vector3(-0.25f, 0f));
		for (int j = 0; j < 2; j++)
		{
			GameObject lhs = ((j == 0) ? plate : plate2);
			List<(BasePart, Rigidbody, int)> obj = ((j == 0) ? m_connectedParts1 : m_connectedParts2);
			Vector3 anchor = new Vector3((j == 0) ? (-0.25f) : 0.25f, 0f);
			foreach (var item2 in obj)
			{
				JointType type = ((!INSettings.GetBool(INFeature.StableHingePlate)) ? JointType.HingeJoint : JointType.FixedJoint);
				Joint item = CreateJoint(lhs, item2.Item2.gameObject, type, item2.Item3, anchor);
				m_joints.Add(item);
			}
		}
	}

	private Joint CreateJoint(GameObject lhs, GameObject rhs, JointType type, int angle, Vector3 anchor)
	{
		Rigidbody component = rhs.GetComponent<Rigidbody>();
		float breakForce = (base.contraption.HasSuperGlue ? float.PositiveInfinity : (base.contraption.GetJointConnectionStrength(m_jointConnectionStrength) * INSettings.GetFloat(INFeature.ConnectionStrength) * 4f));
		switch (type)
		{
		case JointType.FixedJoint:
		{
			FixedJoint fixedJoint = lhs.AddComponent<FixedJoint>();
			fixedJoint.connectedBody = component;
			fixedJoint.anchor = anchor;
			fixedJoint.axis = Vector3.forward;
			fixedJoint.breakForce = breakForce;
			fixedJoint.breakTorque = float.PositiveInfinity;
			fixedJoint.enablePreprocessing = false;
			return fixedJoint;
		}
		case JointType.HingeJoint:
		{
			angle %= 180;
			HingeJoint hingeJoint = lhs.AddComponent<HingeJoint>();
			hingeJoint.connectedBody = component;
			hingeJoint.anchor = anchor;
			hingeJoint.axis = Vector3.forward;
			hingeJoint.breakForce = breakForce;
			hingeJoint.breakTorque = float.PositiveInfinity;
			hingeJoint.enablePreprocessing = false;
			hingeJoint.useLimits = true;
			hingeJoint.limits = new JointLimits
			{
				min = (float)angle - 15f,
				max = (float)angle + 15f
			};
			return hingeJoint;
		}
		default:
			return null;
		}
	}

	private void FixedUpdate()
	{
		if (!base.contraption || !base.contraption.IsRunning)
		{
			return;
		}
		foreach (HingeJoint joint in m_joints)
		{
			if (joint != null)
			{
				Rigidbody component = joint.GetComponent<Rigidbody>();
				Rigidbody connectedBody = joint.connectedBody;
				float mass = component.mass;
				float mass2 = connectedBody.mass;
				Vector3 velocity = component.velocity;
				Vector3 velocity2 = connectedBody.velocity;
				float num = velocity2.x - velocity.x;
				float num2 = velocity2.y - velocity.y;
				float num3 = 0.2f * mass * mass2 / (mass + mass2);
				float num4 = num3 * Mathf.Sqrt(num * num + num2 * num2);
				num3 = ((num4 < 10f) ? num3 : (10f / num4));
				component.AddForce(new Vector3(num3 * num, num3 * num2), ForceMode.Impulse);
				connectedBody.AddForce(new Vector3((0f - num3) * num, (0f - num3) * num2), ForceMode.Impulse);
			}
		}
	}
}
