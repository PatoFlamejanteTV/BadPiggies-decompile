using System;
using System.Collections.Generic;
using UnityEngine;

public class Contraption : WPFMonoBehaviour
{
	public struct JointConnection
	{
		public BasePart partA;

		public BasePart partB;

		public Joint joint;
	}

	public struct ConnectedComponent
	{
		public bool hasEngine;

		public bool hasGearbox;

		public Gearbox gearbox;

		public float enginePower;

		public float powerConsumption;

		public List<MotorWheel> motorWheels;

		public int partCount;

		public float groundTouchTime;
	}

	public class PartPlacementInfo
	{
		public BasePart.PartType partType;

		public BasePart.Direction direction;

		public Vector3 averagePosition = Vector3.zero;

		public int count;

		public PartPlacementInfo(BasePart.PartType partType, BasePart.Direction direction, Vector3 averagePosition, int count)
		{
			this.partType = partType;
			this.direction = direction;
			this.averagePosition = averagePosition;
			this.count = count;
		}
	}

	private class PartOrder : IComparer<PartPlacementInfo>
	{
		public int Compare(PartPlacementInfo obj1, PartPlacementInfo obj2)
		{
			if (InGameFlightMenu.CombinedTypeForGadgetButtonOrdering(obj1.partType) == BasePart.PartType.Balloon || InGameFlightMenu.CombinedTypeForGadgetButtonOrdering(obj2.partType) == BasePart.PartType.Balloon)
			{
				return 0;
			}
			if (obj1.averagePosition.x < obj2.averagePosition.x)
			{
				return -1;
			}
			if (obj1.averagePosition.x > obj2.averagePosition.x)
			{
				return 1;
			}
			return 0;
		}
	}

	private class ConnectionSearchState
	{
		public Queue<int> components = new Queue<int>();

		public HashSet<int> visited = new HashSet<int>();
	}

	public bool m_jointDetached;

	public Dictionary<int, BasePart> m_runtimePartMap;

	public BasePart m_cameraTarget;

	public BasePart m_pig;

	public int m_enginesAmount;

	protected List<BasePart> m_parts = new List<BasePart>();

	protected Dictionary<int, BasePart> m_partMap = new Dictionary<int, BasePart>();

	protected List<BasePart> m_integralParts = new List<BasePart>();

	protected Camera m_gameCamera;

	protected bool m_running;

	protected List<JointConnection> m_jointMap = new List<JointConnection>();

	private float m_enginePowerFactor = 1f;

	private ContraptionDataset m_contraptionDataSet;

	private List<Rope> m_ropes = new List<Rope>();

	private float m_powerConsumption;

	private float m_stopTimer;

	private List<ConnectedComponent> m_connectedComponents = new List<ConnectedComponent>();

	private List<int> m_componentsConnectedByRope = new List<int>();

	private Dictionary<BasePart.PartType, int[]> m_oneShotPartAmount = new Dictionary<BasePart.PartType, int[]>();

	private Dictionary<BasePart.PartType, int[]> m_poweredPartAmount = new Dictionary<BasePart.PartType, int[]>();

	private Dictionary<BasePart.PartType, int[]> m_pushablePartAmount = new Dictionary<BasePart.PartType, int[]>();

	private int m_droppedSandbagLayer;

	private bool m_broken;

	private List<PartPlacementInfo> m_partPlacements = new List<PartPlacementInfo>();

	private int m_staticPartCount;

	private ConnectionSearchState m_connectionSearchState = new ConnectionSearchState();

	[SerializeField]
	[HideInInspector]
	private Glue.Type m_currentGlue;

	[SerializeField]
	[HideInInspector]
	private bool m_hasSuperMagnet;

	[SerializeField]
	[HideInInspector]
	private bool m_hasTurboCharge;

	[SerializeField]
	[HideInInspector]
	private bool m_hasNightVision;

	private GameObject nightVisionGogglesPrefab;

	private GameObject nightVisionGoggles;

	private bool m_contraptionTouchingGround;

	private Dictionary<int, bool> m_contraptionHangingFromHook = new Dictionary<int, bool>();

	private int m_numberOfSwings;

	protected float m_timeLastCollided;

	private bool m_lastValidation;

	private bool m_isValidationRequired = true;

	public static Contraption Instance { get; private set; }

	public int ConnectedComponentCount { get; private set; }

	public int StrictConnectedComponentCount { get; private set; }

	public int GeneralConnectedComponentCount { get; private set; }

	public List<ConnectedComponent> ConnectedComponents => m_connectedComponents;

	public Dictionary<int, BasePart> RuntimePartMap
	{
		get
		{
			return m_runtimePartMap;
		}
		set
		{
			m_runtimePartMap = value;
		}
	}

	public ContraptionDataset DataSet => m_contraptionDataSet;

	public List<BasePart> Parts => m_parts;

	public bool HasSuperMagnet
	{
		get
		{
			return m_hasSuperMagnet;
		}
		set
		{
			if (m_hasSuperMagnet == value)
			{
				return;
			}
			m_hasSuperMagnet = value;
			if (m_hasSuperMagnet)
			{
				Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.SuperMagnetApplied);
			}
			foreach (BasePart part in m_parts)
			{
				if (part.m_partType == BasePart.PartType.Pig)
				{
					SuperMagnet component = part.GetComponent<SuperMagnet>();
					if (m_hasSuperMagnet && component == null)
					{
						part.gameObject.AddComponent<SuperMagnet>();
					}
					else if (!m_hasSuperMagnet && component != null)
					{
						UnityEngine.Object.Destroy(component);
					}
				}
			}
		}
	}

	public bool HasNightVision
	{
		get
		{
			return m_hasNightVision;
		}
		set
		{
			if (m_hasNightVision != value)
			{
				m_hasNightVision = value;
				if (m_hasNightVision)
				{
					Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.toggleNightVision);
				}
				ApplyNightVisionGoggles();
			}
		}
	}

	public bool HasSuperGlue => m_currentGlue != Glue.Type.None;

	public bool HasAlienGlue => m_currentGlue == Glue.Type.Alien;

	public bool HasRegularGlue => m_currentGlue == Glue.Type.Regular;

	public bool HasTurboCharge
	{
		get
		{
			return m_hasTurboCharge;
		}
		set
		{
			if (m_hasTurboCharge != value)
			{
				m_hasTurboCharge = value;
				if (m_hasTurboCharge)
				{
					ApplyTurboCharge();
				}
				else
				{
					ResetTurboCharge();
				}
			}
		}
	}

	public bool HasGluedParts
	{
		get
		{
			if (m_currentGlue != 0)
			{
				return Glue.ContraptionHasGluedParts(this);
			}
			return false;
		}
	}

	public bool IsRunning => m_running;

	public bool HasEngine => m_enginesAmount > 0;

	public Glue.Type CurrentGlue => m_currentGlue;

	public List<JointConnection> JointMap => m_jointMap;

	public Dictionary<int, BasePart> PartMap => m_partMap;

	public List<PartPlacementInfo> PartPlacements => m_partPlacements;

	public float PowerConsumption => m_powerConsumption;

	public event Action ConnectedComponentsChangedEvent;

	public void FixedUpdate()
	{
		if (!m_running)
		{
			return;
		}
		if (m_jointDetached)
		{
			UpdateJointMap();
			m_jointDetached = false;
		}
		else
		{
			if (!INSettings.GetBool(INFeature.DynamicPowerSystem) || !(INContraption.Instance != null) || !INContraption.Instance.Enabled)
			{
				return;
			}
			for (int i = 0; i < ConnectedComponentCount; i++)
			{
				float num = 0f;
				foreach (BasePart connectedPart in this.GetConnectedParts(i))
				{
					if (connectedPart.IsEnabled())
					{
						num += connectedPart.m_powerConsumption;
					}
				}
				ConnectedComponent value = m_connectedComponents[i];
				value.powerConsumption = num;
				m_connectedComponents[i] = value;
			}
			InitializeEngines();
		}
	}

	public void UpdateJointMap()
	{
		m_broken = true;
		for (int num = m_jointMap.Count - 1; num >= 0; num--)
		{
			JointConnection jointConnection = m_jointMap[num];
			if (jointConnection.joint == null || jointConnection.partA == null || jointConnection.partB == null)
			{
				if (jointConnection.partA != null && jointConnection.partA.enclosedInto != null && jointConnection.partA.enclosedInto == jointConnection.partB)
				{
					jointConnection.partA.enclosedInto.enclosedPart = null;
					jointConnection.partA.enclosedInto.OnEnclosedPartDetached();
					jointConnection.partA.m_enclosedInto = null;
					jointConnection.partA.OnDetach();
				}
				if (jointConnection.partB != null && jointConnection.partB.enclosedInto != null && jointConnection.partB.enclosedInto == jointConnection.partA)
				{
					jointConnection.partB.enclosedInto.enclosedPart = null;
					jointConnection.partB.enclosedInto.OnEnclosedPartDetached();
					jointConnection.partB.m_enclosedInto = null;
					jointConnection.partB.OnDetach();
				}
				m_jointMap.RemoveAt(num);
			}
		}
		FindConnectedComponents();
		InitializeEngines();
		CountActiveParts();
		FindComponentsConnectedByRope();
	}

	public bool ConnectedToGearbox(BasePart part)
	{
		int connectedComponent = part.ConnectedComponent;
		return m_connectedComponents[connectedComponent].hasGearbox;
	}

	public Gearbox GetGearbox(BasePart part)
	{
		int connectedComponent = part.ConnectedComponent;
		return m_connectedComponents[connectedComponent].gearbox;
	}

	public void SetCameraTarget(BasePart target)
	{
		m_cameraTarget = target;
	}

	public void SetBroken()
	{
		m_broken = true;
	}

	public void ChangeOneShotPartAmount(BasePart.PartType type, BasePart.Direction direction, int change)
	{
		if (m_oneShotPartAmount.TryGetValue(type, out var value))
		{
			value[(int)direction] += change;
		}
		else if (change > 0)
		{
			value = new int[8];
			value[(int)direction] = change;
			m_oneShotPartAmount[type] = value;
		}
	}

	private static byte[] GetBytesFromString(string str)
	{
		byte[] array = new byte[str.Length * 2];
		Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
		return array;
	}

	public void ChangePoweredPartAmount(BasePart.PartType type, BasePart.Direction direction, int change)
	{
		if (m_poweredPartAmount.TryGetValue(type, out var value))
		{
			value[(int)direction] += change;
		}
		else if (change > 0)
		{
			value = new int[8];
			value[(int)direction] = change;
			m_poweredPartAmount[type] = value;
		}
	}

	public void ChangePushablePartAmount(BasePart.PartType type, BasePart.Direction direction, int change)
	{
		if (m_pushablePartAmount.TryGetValue(type, out var value))
		{
			value[(int)direction] += change;
		}
		else if (change > 0)
		{
			value = new int[8];
			value[(int)direction] = change;
			m_pushablePartAmount[type] = value;
		}
	}

	public bool HasActiveParts(BasePart.PartType type, BasePart.Direction direction)
	{
		if (!HasOneShotParts(type, direction) && !HasPoweredParts(type, direction))
		{
			return HasPushableParts(type, direction);
		}
		return true;
	}

	public bool HasOneShotParts(BasePart.PartType type, BasePart.Direction direction)
	{
		if (m_oneShotPartAmount.TryGetValue(type, out var value))
		{
			return value[(int)direction] > 0;
		}
		return false;
	}

	public bool HasPoweredParts(BasePart.PartType type, BasePart.Direction direction)
	{
		if (m_poweredPartAmount.TryGetValue(type, out var value))
		{
			return value[(int)direction] > 0;
		}
		return false;
	}

	public bool HasPushableParts(BasePart.PartType type, BasePart.Direction direction)
	{
		if (m_pushablePartAmount.TryGetValue(type, out var value))
		{
			return value[(int)direction] > 0;
		}
		return false;
	}

	public bool IsMovementStopped()
	{
		return m_stopTimer > 0.5f;
	}

	public bool HasPart(BasePart.PartType type)
	{
		for (int i = 0; i < m_parts.Count; i++)
		{
			if (m_parts[i].m_partType == type)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasPart(BasePart.PartType type, BasePart.PartTier tier)
	{
		for (int i = 0; i < m_parts.Count; i++)
		{
			if (m_parts[i].m_partType == type && m_parts[i].m_partTier == tier)
			{
				return true;
			}
		}
		return false;
	}

	public int GetPartCount(BasePart.PartType type)
	{
		int num = 0;
		for (int i = 0; i < m_parts.Count; i++)
		{
			if (m_parts[i].m_partType == type)
			{
				num++;
			}
		}
		return num;
	}

	public bool IsBroken()
	{
		return m_broken;
	}

	public void SetGroundTouchTime(BasePart part)
	{
		int connectedComponent = part.ConnectedComponent;
		if (connectedComponent >= 0 && connectedComponent < m_connectedComponents.Count)
		{
			ConnectedComponent value = m_connectedComponents[connectedComponent];
			value.groundTouchTime = Time.time;
			m_connectedComponents[connectedComponent] = value;
		}
	}

	public float GetGroundTouchTime(int componentIndex)
	{
		if (componentIndex >= 0 && componentIndex < m_connectedComponents.Count)
		{
			return m_connectedComponents[componentIndex].groundTouchTime;
		}
		return 0f;
	}

	public float GetEnginePowerFactor(BasePart part)
	{
		if (INSettings.GetBool(INFeature.DynamicPowerSystem))
		{
			ConnectedComponent connectedComponent = m_connectedComponents[part.ConnectedComponent];
			float enginePower = connectedComponent.enginePower;
			float powerConsumption = connectedComponent.powerConsumption;
			float num = 0f;
			if (powerConsumption > 1f)
			{
				num = Mathf.Min(enginePower / powerConsumption, 10f * INSettings.GetFloat(INFeature.EnginePowerLimit));
			}
			else if (enginePower > 0f)
			{
				num = 1f;
			}
			return Mathf.Pow(num, (num > 1f) ? 0.585f : 0.75f);
		}
		int connectedComponent2 = part.ConnectedComponent;
		if (connectedComponent2 >= 0 && connectedComponent2 < m_connectedComponents.Count)
		{
			ConnectedComponent connectedComponent3 = m_connectedComponents[connectedComponent2];
			float num2 = 0f;
			float num3 = connectedComponent3.powerConsumption;
			for (int i = 0; i < connectedComponent3.motorWheels.Count; i++)
			{
				MotorWheel motorWheel = connectedComponent3.motorWheels[i];
				if (!motorWheel.HasContact)
				{
					num3 -= 0.9f * motorWheel.m_powerConsumption;
				}
			}
			if (num3 > 1f)
			{
				num2 = Mathf.Min(connectedComponent3.enginePower / num3, 10f * INSettings.GetFloat(INFeature.EnginePowerLimit));
			}
			else if (connectedComponent3.enginePower > 0f)
			{
				num2 = 1f;
			}
			if (num2 > 1f)
			{
				return Mathf.Pow(num2, 0.585f);
			}
			return Mathf.Pow(num2, 0.75f);
		}
		return 0f;
	}

	public void AddRuntimePart(BasePart part)
	{
		m_parts.Add(part);
	}

	private void Awake()
	{
		m_contraptionDataSet = new ContraptionDataset();
		m_gameCamera = Camera.main;
		m_droppedSandbagLayer = LayerMask.NameToLayer("DroppedSandbag");
		nightVisionGogglesPrefab = Resources.Load<GameObject>("Prefabs/NightVisionGoggles");
		Instance = this;
		INContraption.Create(this);
	}

	public int NumOfIntegralParts()
	{
		return m_integralParts.Count;
	}

	public int DynamicPartCount()
	{
		return m_parts.Count - m_staticPartCount;
	}

	public void IncreaseStaticPartCount()
	{
		m_staticPartCount++;
	}

	public BasePart FindPig()
	{
		return FindPart(BasePart.PartType.Pig);
	}

	public BasePart FindPart(BasePart.PartType type)
	{
		foreach (BasePart part in m_parts)
		{
			if (part != null && part.m_partType == type)
			{
				return part;
			}
		}
		return null;
	}

	public static void AddToBoundingBox(ref Rect box, Vector2 min, Vector2 max)
	{
		if (min.x < box.xMin)
		{
			box.xMin = min.x;
		}
		if (min.y < box.yMin)
		{
			box.yMin = min.y;
		}
		if (max.x > box.xMax)
		{
			box.xMax = max.x;
		}
		if (max.y > box.yMax)
		{
			box.yMax = max.y;
		}
	}

	public Rect BoundingBox()
	{
		if ((bool)m_pig && m_cameraTarget == m_pig)
		{
			Vector3 position = m_pig.transform.position;
			Rect box = new Rect(position.x, position.y, 0f, 0f);
			for (int i = 0; i < m_parts.Count; i++)
			{
				BasePart basePart = m_parts[i];
				if ((bool)basePart && basePart.m_partType != BasePart.PartType.Rope && m_componentsConnectedByRope.Contains(basePart.ConnectedComponent) && basePart.m_partType != BasePart.PartType.Spring)
				{
					Vector3 position2 = basePart.transform.position;
					Vector2 min = new Vector2(position2.x - 0.5f, position2.y - 0.5f);
					Vector2 max = new Vector2(position2.x + 0.5f, position2.y + 0.5f);
					AddToBoundingBox(ref box, min, max);
				}
			}
			return box;
		}
		if ((bool)m_cameraTarget)
		{
			return new Rect(m_cameraTarget.transform.position.x - 0.5f, m_cameraTarget.transform.position.y - 0.5f, 1f, 1f);
		}
		return default(Rect);
	}

	public bool CanConnectTo(BasePart part1, BasePart part2, BasePart.Direction direction)
	{
		bool num;
		if (part1 != null && part2 != null)
		{
			if (!part1.CanConnectTo(direction))
			{
				return false;
			}
			if (!part2.CanConnectTo(BasePart.InverseDirection(direction)))
			{
				return false;
			}
			if (part1.m_jointConnectionType != 0 && part2.m_jointConnectionType != 0 && (part2.m_jointConnectionType == BasePart.JointConnectionType.Source || part1.m_jointConnectionType == BasePart.JointConnectionType.Source))
			{
				return true;
			}
			if ((part1.m_partType == BasePart.PartType.Wings || part1.m_partType == BasePart.PartType.MetalWing) && (part2.m_partType == BasePart.PartType.Wings || part2.m_partType == BasePart.PartType.MetalWing))
			{
				if (INSettings.GetBool(INFeature.RotatableWing))
				{
					if (part1.m_gridRotation == part2.m_gridRotation)
					{
						if (part1.m_gridRotation != 0 && part1.m_gridRotation != BasePart.GridRotation.Deg_180)
						{
							if (direction != BasePart.Direction.Left)
							{
								num = direction == BasePart.Direction.Right;
								goto IL_00c2;
							}
						}
						else if (direction != BasePart.Direction.Down)
						{
							num = direction == BasePart.Direction.Up;
							goto IL_00c2;
						}
						goto IL_00c4;
					}
				}
				else if (direction == BasePart.Direction.Down || direction == BasePart.Direction.Up)
				{
					return true;
				}
			}
		}
		goto IL_00d0;
		IL_00c4:
		return true;
		IL_00c2:
		if (num)
		{
			goto IL_00c4;
		}
		goto IL_00d0;
		IL_00d0:
		return false;
	}

	public bool CanConnectTo(BasePart part, BasePart.JointConnectionDirection direction)
	{
		if (INSettings.GetBool(INFeature.EnclosableParts) && part.enclosedInto != null)
		{
			return true;
		}
		switch (direction)
		{
		case BasePart.JointConnectionDirection.Any:
			return true;
		case BasePart.JointConnectionDirection.LeftAndRight:
			if (!CanConnectTo(part, BasePart.Direction.Left))
			{
				return CanConnectTo(part, BasePart.Direction.Right);
			}
			return true;
		case BasePart.JointConnectionDirection.UpAndDown:
			if (!CanConnectTo(part, BasePart.Direction.Up))
			{
				return CanConnectTo(part, BasePart.Direction.Down);
			}
			return true;
		default:
			return CanConnectTo(part, BasePart.ConvertDirection(direction));
		}
	}

	public bool CanConnectTo(BasePart part, BasePart.Direction direction)
	{
		int coordX = part.m_coordX;
		int coordY = part.m_coordY;
		switch (direction)
		{
		case BasePart.Direction.Right:
		{
			BasePart part5 = FindPartAt(coordX + 1, coordY);
			return CanConnectTo(part, part5, direction);
		}
		case BasePart.Direction.Up:
		{
			BasePart part4 = FindPartAt(coordX, coordY + 1);
			return CanConnectTo(part, part4, direction);
		}
		case BasePart.Direction.Left:
		{
			BasePart part3 = FindPartAt(coordX - 1, coordY);
			return CanConnectTo(part, part3, direction);
		}
		case BasePart.Direction.Down:
		{
			BasePart part2 = FindPartAt(coordX, coordY - 1);
			return CanConnectTo(part, part2, direction);
		}
		default:
			return false;
		}
	}

	public BasePart GetPartAt(BasePart part, BasePart.Direction direction)
	{
		int coordX = part.m_coordX;
		int coordY = part.m_coordY;
		return direction switch
		{
			BasePart.Direction.Right => FindPartAt(coordX + 1, coordY), 
			BasePart.Direction.Up => FindPartAt(coordX, coordY + 1), 
			BasePart.Direction.Left => FindPartAt(coordX - 1, coordY), 
			BasePart.Direction.Down => FindPartAt(coordX, coordY - 1), 
			_ => null, 
		};
	}

	public void StartContraption()
	{
		m_broken = false;
		m_stopTimer = 0f;
		m_parts = new List<BasePart>(GetComponentsInChildren<BasePart>());
		m_ropes.Clear();
		m_powerConsumption = 0f;
		m_enginesAmount = 0;
		if (m_hasTurboCharge)
		{
			m_enginePowerFactor = WPFMonoBehaviour.gameData.m_turboChargePowerFactor;
		}
		foreach (BasePart part in m_parts)
		{
			Vector3 localPosition = part.transform.localPosition;
			int x = Mathf.RoundToInt(localPosition.x);
			int y = Mathf.RoundToInt(localPosition.y);
			SetPartPos(x, y, part);
			if (part.enclosedInto != null)
			{
				SetPartPos(x, y, part.enclosedInto);
			}
			if (part.IsIntegralPart())
			{
				m_integralParts.Add(part);
			}
			part.gameObject.tag = "Contraption";
			for (int i = 0; i < part.transform.childCount; i++)
			{
				part.transform.GetChild(i).gameObject.tag = "Contraption";
			}
			int @int = INSettings.GetInt(INFeature.CameraTargetPartType);
			if (part.m_partType == ((SortedPartType)@int).ToPartType())
			{
				m_cameraTarget = part;
			}
			if (part.m_partType == BasePart.PartType.Pig)
			{
				m_pig = part;
			}
			m_powerConsumption += part.m_powerConsumption;
		}
		foreach (BasePart part2 in m_parts)
		{
			int coordX = part2.m_coordX;
			int coordY = part2.m_coordY;
			m_contraptionDataSet.AddPart(coordX, coordY, (int)part2.m_partType, part2.customPartIndex, part2.m_gridRotation, part2.m_flipped);
			part2.contraption = this;
			part2.EnsureRigidbody();
		}
		foreach (BasePart part3 in m_parts)
		{
			int coordX2 = part3.m_coordX;
			int coordY2 = part3.m_coordY;
			BasePart.JointConnectionDirection customJointConnectionDirection = part3.GetCustomJointConnectionDirection();
			if (part3.m_jointConnectionType != 0)
			{
				BasePart basePart = FindPartAt(coordX2 + 1, coordY2);
				BasePart basePart2 = FindPartAt(coordX2, coordY2 - 1);
				if (CanConnectTo(part3, basePart, BasePart.Direction.Right))
				{
					BasePart.JointConnectionDirection customJointConnectionDirection2 = basePart.GetCustomJointConnectionDirection();
					if (customJointConnectionDirection == BasePart.JointConnectionDirection.Right || customJointConnectionDirection == BasePart.JointConnectionDirection.LeftAndRight)
					{
						AddCustomConnectionBetweenParts(part3, basePart);
					}
					else if (customJointConnectionDirection2 == BasePart.JointConnectionDirection.Left || customJointConnectionDirection2 == BasePart.JointConnectionDirection.LeftAndRight)
					{
						AddCustomConnectionBetweenParts(basePart, part3);
					}
					else
					{
						AddFixedJoint(part3, basePart);
					}
				}
				if (CanConnectTo(part3, basePart2, BasePart.Direction.Down))
				{
					BasePart.JointConnectionDirection customJointConnectionDirection3 = basePart2.GetCustomJointConnectionDirection();
					if (customJointConnectionDirection == BasePart.JointConnectionDirection.Down || customJointConnectionDirection == BasePart.JointConnectionDirection.UpAndDown)
					{
						AddCustomConnectionBetweenParts(part3, basePart2);
					}
					else if (customJointConnectionDirection3 == BasePart.JointConnectionDirection.Up || customJointConnectionDirection3 == BasePart.JointConnectionDirection.UpAndDown)
					{
						AddCustomConnectionBetweenParts(basePart2, part3);
					}
					else
					{
						AddFixedJoint(part3, basePart2);
					}
				}
				if (INSettings.GetBool(INFeature.AllDirectionsConnection))
				{
					BasePart basePart3 = FindPartAt(coordX2 - 1, coordY2);
					BasePart basePart4 = FindPartAt(coordX2, coordY2 + 1);
					if (CanConnectTo(part3, basePart3, BasePart.Direction.Left))
					{
						BasePart.JointConnectionDirection customJointConnectionDirection4 = basePart3.GetCustomJointConnectionDirection();
						if (customJointConnectionDirection == BasePart.JointConnectionDirection.Left || customJointConnectionDirection == BasePart.JointConnectionDirection.LeftAndRight)
						{
							AddCustomConnectionBetweenParts(part3, basePart3);
						}
						else if (customJointConnectionDirection4 == BasePart.JointConnectionDirection.Right || customJointConnectionDirection4 == BasePart.JointConnectionDirection.LeftAndRight)
						{
							AddCustomConnectionBetweenParts(basePart3, part3);
						}
						else
						{
							AddFixedJoint(part3, basePart3);
						}
					}
					if (CanConnectTo(part3, basePart4, BasePart.Direction.Up))
					{
						BasePart.JointConnectionDirection customJointConnectionDirection5 = basePart4.GetCustomJointConnectionDirection();
						if (customJointConnectionDirection == BasePart.JointConnectionDirection.Up || customJointConnectionDirection == BasePart.JointConnectionDirection.UpAndDown)
						{
							AddCustomConnectionBetweenParts(part3, basePart4);
						}
						else if (customJointConnectionDirection5 == BasePart.JointConnectionDirection.Down || customJointConnectionDirection5 == BasePart.JointConnectionDirection.UpAndDown)
						{
							AddCustomConnectionBetweenParts(basePart4, part3);
						}
						else
						{
							AddFixedJoint(part3, basePart4);
						}
					}
				}
				if (part3.m_partType == BasePart.PartType.Rope && part3 is Rope)
				{
					Rope rope = (Rope)part3;
					BasePart basePart5;
					BasePart basePart6;
					if (customJointConnectionDirection == BasePart.JointConnectionDirection.LeftAndRight)
					{
						basePart5 = FindPartAt(coordX2 - 1, coordY2);
						basePart6 = FindPartAt(coordX2 + 1, coordY2);
					}
					else
					{
						basePart5 = FindPartAt(coordX2, coordY2 + 1);
						basePart6 = FindPartAt(coordX2, coordY2 - 1);
					}
					if ((bool)basePart5 && basePart5 is Rope && basePart5.m_gridRotation != part3.m_gridRotation)
					{
						basePart5 = null;
					}
					if ((bool)basePart5 && !(basePart5 is Rope) && !(basePart5 is Frame) && !(basePart5 is Kicker))
					{
						basePart5 = null;
					}
					if ((bool)basePart6 && basePart6 is Rope && basePart6.m_gridRotation != part3.m_gridRotation)
					{
						basePart6 = null;
					}
					if ((bool)basePart6 && !(basePart6 is Rope) && !(basePart6 is Frame) && !(basePart6 is Kicker))
					{
						basePart6 = null;
					}
					rope.Create(basePart5, basePart6);
					m_ropes.Add(rope);
				}
			}
			if (part3.m_partType == BasePart.PartType.Spring && part3.m_enclosedInto == null)
			{
				BasePart.Direction direction = BasePart.ConvertDirection(part3.GetCustomJointConnectionDirection());
				BasePart partAt = GetPartAt(part3, direction);
				if (!partAt || !CanConnectTo(part3, partAt, direction))
				{
					(part3 as Spring).CreateSpringBody(direction);
				}
			}
			if (part3.m_partType == BasePart.PartType.Pig && (bool)WPFMonoBehaviour.levelManager && WPFMonoBehaviour.levelManager.m_disablePigCollisions && part3.m_enclosedInto != null)
			{
				part3.gameObject.layer = LayerMask.NameToLayer("NonCollidingPart");
			}
			if (part3.m_partType != BasePart.PartType.KingPig && part3.m_partType != BasePart.PartType.GoldenPig)
			{
				continue;
			}
			for (int j = 0; j <= 1; j++)
			{
				for (int k = -2; k <= 2; k += 4)
				{
					BasePart basePart7 = FindPartAt(coordX2 + k, coordY2 + j);
					if (basePart7 != null && (basePart7.m_partType == BasePart.PartType.Wings || basePart7.m_partType == BasePart.PartType.MetalWing) && basePart7.collider != null)
					{
						Physics.IgnoreCollision(part3.collider, basePart7.collider);
					}
				}
				for (int l = -1; l <= 1; l++)
				{
					if (l == 0 && j == 0)
					{
						continue;
					}
					BasePart basePart8 = FindPartAt(coordX2 + l, coordY2 + j);
					if (!(basePart8 != null))
					{
						continue;
					}
					if (basePart8.m_partType == BasePart.PartType.WoodenFrame || basePart8.m_partType == BasePart.PartType.MetalFrame)
					{
						if (j == 0)
						{
							AddFixedJoint(part3, basePart8);
						}
						else
						{
							Physics.IgnoreCollision(part3.collider, basePart8.collider);
						}
					}
					else if (basePart8.m_partType == BasePart.PartType.Spring && basePart8.collider != null)
					{
						Physics.IgnoreCollision(part3.collider, basePart8.collider);
					}
				}
			}
		}
		INContraption.Instance.IsRunning = true;
		INContraption.Instance.Initialize();
		for (int m = 0; m < m_parts.Count; m++)
		{
			BasePart basePart9 = m_parts[m];
			basePart9.contraption = this;
			basePart9.enabled = true;
			basePart9.Initialize();
		}
		FindConnectedComponents();
		FindComponentsConnectedByRope();
		CalculatePartPlacement();
		InitializeEngines();
		CountActiveParts();
		if (m_currentGlue != 0)
		{
			MakeUnbreakable();
		}
		INContraption.Instance.PostInitialize();
		for (int n = 0; n < m_parts.Count; n++)
		{
			m_parts[n].PostInitialize();
		}
		if (m_oneShotPartAmount.TryGetValue(BasePart.PartType.Balloon, out var value))
		{
			int num = value[0];
			if (num > 0)
			{
				foreach (BasePart part4 in m_parts)
				{
					if (part4.m_partType == BasePart.PartType.Balloon)
					{
						(part4 as Balloon).ConfigureExtraBalanceJoint(1f / (float)num);
					}
				}
			}
		}
		m_running = true;
	}

	public void SaveContraption(string currentContraptionName)
	{
		WPFPrefs.SaveContraptionDataset(currentContraptionName, m_contraptionDataSet);
		GameProgress.Save();
	}

	public void AddCustomConnectionBetweenParts(BasePart part1, BasePart part2)
	{
		part2.EnsureRigidbody();
		Joint joint = part1.CustomConnectToPart(part2);
		AddJointToMap(part1, part2, joint);
	}

	public void CalculatePartPlacement()
	{
		m_partPlacements.Clear();
		for (int i = 0; i < m_parts.Count; i++)
		{
			AddPartPlacement(m_parts[i]);
		}
		foreach (PartPlacementInfo partPlacement in m_partPlacements)
		{
			partPlacement.averagePosition /= (float)partPlacement.count;
		}
		m_partPlacements.Sort(new PartOrder());
	}

	private void AddPartPlacement(BasePart part)
	{
		BasePart.PartType partType = InGameFlightMenu.CombinedTypeForGadgetButtonOrdering(part.m_partType);
		foreach (PartPlacementInfo partPlacement in m_partPlacements)
		{
			if (partPlacement.partType == partType && partPlacement.direction == part.EffectDirection())
			{
				partPlacement.averagePosition += part.transform.position;
				partPlacement.count++;
				return;
			}
		}
		m_partPlacements.Add(new PartPlacementInfo(partType, part.EffectDirection(), part.transform.position, 1));
	}

	public int EnginePoweredPartTypeCount()
	{
		int num = 0;
		foreach (KeyValuePair<BasePart.PartType, int[]> item in m_poweredPartAmount)
		{
			if (item.Key != BasePart.PartType.Bellows)
			{
				if (item.Value[0] > 0)
				{
					num++;
				}
				if (item.Value[1] > 0)
				{
					num++;
				}
				if (item.Value[2] > 0)
				{
					num++;
				}
				if (item.Value[3] > 0)
				{
					num++;
				}
			}
		}
		return num;
	}

	public void CountActiveParts()
	{
		foreach (KeyValuePair<BasePart.PartType, int[]> item in m_poweredPartAmount)
		{
			Array.Clear(item.Value, 0, item.Value.Length);
		}
		foreach (KeyValuePair<BasePart.PartType, int[]> item2 in m_pushablePartAmount)
		{
			Array.Clear(item2.Value, 0, item2.Value.Length);
		}
		foreach (BasePart part in m_parts)
		{
			if (part.CanBeEnabled() && !part.IsEngine())
			{
				if (part.IsPowered())
				{
					ChangePoweredPartAmount(part.m_partType, part.EffectDirection(), 1);
				}
				else
				{
					ChangePushablePartAmount(part.m_partType, part.EffectDirection(), 1);
				}
			}
		}
	}

	public bool SomePoweredPartsEnabled()
	{
		for (int i = 0; i < m_parts.Count; i++)
		{
			if (m_parts[i].m_powerConsumption > 0f && m_parts[i].IsEnabled())
			{
				return true;
			}
		}
		return false;
	}

	public bool AllPoweredPartsEnabled()
	{
		bool result = false;
		for (int i = 0; i < m_parts.Count; i++)
		{
			if (m_parts[i].m_powerConsumption > 0f)
			{
				if (!m_parts[i].IsEnabled() && m_parts[i].CanBeEnabled())
				{
					return false;
				}
				result = true;
			}
		}
		return result;
	}

	public bool AnyPartsEnabled(BasePart.PartType type, BasePart.Direction direction)
	{
		for (int i = 0; i < m_parts.Count; i++)
		{
			if (m_parts[i].m_partType == type && m_parts[i].EffectDirection() == direction && m_parts[i].IsEnabled())
			{
				return true;
			}
		}
		return false;
	}

	public bool AnyPartsEnabled(BasePart.PartType type, BasePart.PartTier tier, BasePart.Direction direction)
	{
		for (int i = 0; i < m_parts.Count; i++)
		{
			if (m_parts[i].m_partType == type && m_parts[i].m_partTier == tier && m_parts[i].EffectDirection() == direction && m_parts[i].IsEnabled())
			{
				return true;
			}
		}
		return false;
	}

	public void ActivatePartType(BasePart.PartType type, BasePart.Direction direction)
	{
		if (type == BasePart.PartType.Engine)
		{
			ActivateAllPoweredParts();
			return;
		}
		if (type == BasePart.PartType.Balloon || type == BasePart.PartType.Sandbag)
		{
			ActivateOnePartOfType(type);
			return;
		}
		int num = 0;
		foreach (BasePart part in m_parts)
		{
			if (part.m_partType == type && part.EffectDirection() == direction && !part.IsEnabled() && part.CanBeEnabled() && part.HasOnOffToggle() && (!INSettings.GetBool(INFeature.PartGenerator) || part.GeneratorRefCount <= 0))
			{
				num++;
			}
		}
		bool startAll = num > 0;
		ActivateParts((BasePart part) => part.m_partType == type && part.EffectDirection() == direction && (startAll != part.IsEnabled() || !part.HasOnOffToggle()));
	}

	public void ActivateParts(Func<BasePart, bool> predicate)
	{
		foreach (BasePart item in new List<BasePart>(m_parts))
		{
			if ((bool)item && predicate(item))
			{
				item.ProcessTouch();
			}
		}
	}

	private void ActivateOnePartOfType(BasePart.PartType type)
	{
		Vector3 zero = Vector3.zero;
		float num = 0f;
		foreach (BasePart part in m_parts)
		{
			if (part != null && part.ConnectedComponent == m_pig.ConnectedComponent)
			{
				zero += part.transform.position;
				num += 1f;
			}
		}
		if (num > 0f)
		{
			zero /= num;
		}
		float num2 = 0f;
		BasePart basePart = null;
		foreach (BasePart part2 in m_parts)
		{
			if ((bool)part2 && part2.m_partType == type && (part2.m_partType != BasePart.PartType.Sandbag || part2.GetComponent<Sandbag>().IsAttached()))
			{
				float num3 = Vector3.Distance(part2.transform.position, zero);
				if (num3 > num2)
				{
					num2 = num3;
					basePart = part2;
				}
			}
		}
		if ((bool)basePart)
		{
			basePart.ProcessTouch();
		}
	}

	public void ActivateAllPoweredParts()
	{
		int num = 0;
		foreach (BasePart part in m_parts)
		{
			if (part.CanBeEnabled() && !part.IsEnabled() && part.IsPowered())
			{
				num++;
			}
		}
		bool flag = num > 0;
		foreach (BasePart part2 in m_parts)
		{
			if (!part2.CanBeEnabled() || !part2.IsPowered())
			{
				continue;
			}
			if (flag)
			{
				if (!part2.IsEnabled())
				{
					part2.SetEnabled(enabled: true);
				}
			}
			else if (part2.IsEnabled())
			{
				part2.SetEnabled(enabled: false);
			}
		}
	}

	public void TurnOffAllPoweredParts()
	{
		for (int i = 0; i < m_parts.Count; i++)
		{
			if (!(m_parts[i] == null) && (m_parts[i].IsPowered() || m_parts[i].IsEngine()) && m_parts[i].IsEnabled())
			{
				m_parts[i].SetEnabled(enabled: false);
			}
		}
	}

	public int ActivateAllPoweredParts(int connectedComponent)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < m_parts.Count; i++)
		{
			if (m_parts[i].ConnectedComponent == connectedComponent && m_parts[i].CanBeEnabled() && !m_parts[i].IsEnabled() && m_parts[i].IsPowered())
			{
				num2++;
			}
		}
		bool flag = num2 > 0;
		for (int j = 0; j < m_parts.Count; j++)
		{
			if (m_parts[j].ConnectedComponent != connectedComponent || !m_parts[j].CanBeEnabled() || !m_parts[j].IsPowered())
			{
				continue;
			}
			num++;
			if (flag)
			{
				if (!m_parts[j].IsEnabled())
				{
					m_parts[j].SetEnabled(enabled: true);
				}
			}
			else if (m_parts[j].IsEnabled())
			{
				m_parts[j].SetEnabled(enabled: false);
			}
		}
		return num;
	}

	public void UpdateEngineStates(int connectedComponent)
	{
		int num = 0;
		for (int i = 0; i < m_parts.Count; i++)
		{
			BasePart basePart = m_parts[i];
			if (basePart.ConnectedComponent == connectedComponent && basePart.CanBeEnabled() && basePart.IsEnabled() && basePart.IsPowered())
			{
				num++;
			}
		}
		EnableComponentEngines(connectedComponent, num > 0);
	}

	private void EnableComponentEngines(int connectedComponent, bool enable)
	{
		for (int i = 0; i < m_parts.Count; i++)
		{
			BasePart basePart = m_parts[i];
			if (basePart.IsEngine() && basePart.ConnectedComponent == connectedComponent)
			{
				basePart.SetEnabled(enable);
			}
		}
	}

	public void FindComponentsConnectedByRope()
	{
		m_componentsConnectedByRope.Clear();
		if (m_ropes.Count != 0 || (bool)m_pig)
		{
			Queue<int> components = m_connectionSearchState.components;
			HashSet<int> visited = m_connectionSearchState.visited;
			components.Clear();
			visited.Clear();
			components.Enqueue(m_pig.ConnectedComponent);
			while (components.Count > 0)
			{
				int num = components.Dequeue();
				m_componentsConnectedByRope.Add(num);
				visited.Add(num);
				FindRopeConnections(num, ref components, visited);
			}
		}
	}

	public bool IsConnectedTo(BasePart part1, Collider collider1, BasePart part2)
	{
		if (!part1 || !part2)
		{
			return false;
		}
		if (part1.m_partType != BasePart.PartType.Rope)
		{
			return IsConnectedTo(part1, part2);
		}
		Rope component = part1.GetComponent<Rope>();
		if (component.IsCut())
		{
			if (component.IsLeftPart(base.collider))
			{
				if ((bool)component.LeftPart)
				{
					return IsConnectedTo(component.LeftPart, part2);
				}
			}
			else if ((bool)component.RightPart)
			{
				return IsConnectedTo(component.RightPart, part2);
			}
			return false;
		}
		if ((bool)component.LeftPart)
		{
			return IsConnectedTo(component.LeftPart, part2);
		}
		if ((bool)component.RightPart)
		{
			return IsConnectedTo(component.RightPart, part2);
		}
		return false;
	}

	public bool IsConnectedTo(BasePart part1, BasePart part2)
	{
		Queue<int> components = m_connectionSearchState.components;
		HashSet<int> visited = m_connectionSearchState.visited;
		components.Clear();
		visited.Clear();
		components.Enqueue(part1.ConnectedComponent);
		while (components.Count > 0)
		{
			int num = components.Dequeue();
			if (num == part2.ConnectedComponent)
			{
				return true;
			}
			visited.Add(num);
			FindRopeConnections(num, ref components, visited);
		}
		return false;
	}

	public bool IsConnectedToPig(BasePart part, Collider collider = null)
	{
		if (!part)
		{
			return false;
		}
		if (part.m_partType != BasePart.PartType.Rope || (INSettings.GetBool(INFeature.HingePlate) && part is HingePlate))
		{
			return m_componentsConnectedByRope.Contains(part.ConnectedComponent);
		}
		Rope component = part.GetComponent<Rope>();
		if (component.IsCut())
		{
			if (component.IsLeftPart(collider))
			{
				if ((bool)component.LeftPart)
				{
					return m_componentsConnectedByRope.Contains(component.LeftPart.ConnectedComponent);
				}
			}
			else if ((bool)component.RightPart)
			{
				return m_componentsConnectedByRope.Contains(component.RightPart.ConnectedComponent);
			}
			return false;
		}
		if ((bool)component.LeftPart)
		{
			return m_componentsConnectedByRope.Contains(component.LeftPart.ConnectedComponent);
		}
		if ((bool)component.RightPart)
		{
			return m_componentsConnectedByRope.Contains(component.RightPart.ConnectedComponent);
		}
		return false;
	}

	private void FindRopeConnections(int component, ref Queue<int> components, HashSet<int> ignoreList)
	{
		for (int i = 0; i < m_ropes.Count; i++)
		{
			Rope rope = m_ropes[i];
			if (!(rope != null) || rope.IsCut())
			{
				continue;
			}
			if (rope.LeftPart != null && rope.RightPart != null)
			{
				int connectedComponent = rope.LeftPart.ConnectedComponent;
				int connectedComponent2 = rope.RightPart.ConnectedComponent;
				int connectedComponent3 = rope.ConnectedComponent;
				if (connectedComponent == component)
				{
					if (!ignoreList.Contains(connectedComponent2))
					{
						components.Enqueue(connectedComponent2);
					}
				}
				else if (connectedComponent2 == component)
				{
					if (!ignoreList.Contains(connectedComponent))
					{
						components.Enqueue(connectedComponent);
					}
				}
				else if (connectedComponent3 == component)
				{
					if (!ignoreList.Contains(connectedComponent))
					{
						components.Enqueue(connectedComponent);
					}
					if (!ignoreList.Contains(connectedComponent2))
					{
						components.Enqueue(connectedComponent2);
					}
				}
			}
			else if (((bool)rope.LeftPart || (bool)rope.RightPart) && rope.ConnectedComponent == component)
			{
				if ((bool)rope.RightPart && !ignoreList.Contains(rope.RightPart.ConnectedComponent))
				{
					components.Enqueue(rope.RightPart.ConnectedComponent);
				}
				if ((bool)rope.LeftPart && !ignoreList.Contains(rope.LeftPart.ConnectedComponent))
				{
					components.Enqueue(rope.LeftPart.ConnectedComponent);
				}
			}
		}
	}

	public void FinishConnectedComponentSearch()
	{
	}

	private void FindConnectedComponents()
	{
		List<BasePart> parts = m_parts;
		int count = parts.Count;
		DisjointSet disjointSet = new DisjointSet(count);
		Dictionary<BasePart, int> dictionary = new Dictionary<BasePart, int>(count);
		for (int i = 0; i < count; i++)
		{
			dictionary[parts[i]] = i;
		}
		foreach (JointConnection item2 in m_jointMap)
		{
			if (item2.partA != null && item2.partB != null && dictionary.TryGetValue(item2.partA, out var value) && dictionary.TryGetValue(item2.partB, out var value2))
			{
				disjointSet.Union(value, value2);
			}
		}
		if (INSettings.GetBool(INFeature.AutoConnector))
		{
			for (int j = 0; j < count; j++)
			{
				BasePart basePart = parts[j];
				if (basePart.m_partType != BasePart.PartType.Kicker || !basePart.IsAutoConnector())
				{
					continue;
				}
				foreach (BasePart connectedPart in ((Kicker)basePart).GetConnectedParts())
				{
					if (connectedPart != null && dictionary.TryGetValue(connectedPart, out var value3))
					{
						disjointSet.Union(j, value3);
					}
				}
			}
		}
		int[] array = new int[count];
		disjointSet.GetComponentIndexes(array, out var componentCount);
		for (int k = 0; k < count; k++)
		{
			parts[k].StrictConnectedComponent = array[k];
		}
		StrictConnectedComponentCount = componentCount;
		if (INSettings.GetBool(INFeature.HingePlate) && INSettings.GetBool(INFeature.CanHingePlateConnectComponents))
		{
			for (int l = 0; l < count; l++)
			{
				BasePart basePart2 = parts[l];
				if (!basePart2.IsHingePlate())
				{
					continue;
				}
				foreach (BasePart connectedPart2 in ((HingePlate)basePart2).GetConnectedParts())
				{
					if (connectedPart2 != null && dictionary.TryGetValue(connectedPart2, out var value4))
					{
						disjointSet.Union(l, value4);
					}
				}
			}
		}
		if (INSettings.GetBool(INFeature.SeparatorConnection))
		{
			for (int m = 0; m < count; m++)
			{
				BasePart basePart3 = parts[m];
				if (basePart3.m_partType != BasePart.PartType.Kicker || !basePart3.IsElasticConnector())
				{
					continue;
				}
				foreach (BasePart connectedPart3 in ((Kicker)basePart3).GetConnectedParts())
				{
					if (connectedPart3 != null && dictionary.TryGetValue(connectedPart3, out var value5))
					{
						disjointSet.Union(m, value5);
					}
				}
			}
		}
		disjointSet.GetComponentIndexes(array, out componentCount);
		ConnectedComponentCount = componentCount;
		m_connectedComponents = new List<ConnectedComponent>(componentCount);
		for (int n = 0; n < componentCount; n++)
		{
			m_connectedComponents.Add(new ConnectedComponent
			{
				motorWheels = new List<MotorWheel>()
			});
		}
		for (int num = 0; num < count; num++)
		{
			int num2 = array[num];
			BasePart basePart4 = parts[num];
			basePart4.ConnectedComponent = num2;
			ConnectedComponent value6 = m_connectedComponents[num2];
			value6.partCount++;
			value6.powerConsumption += basePart4.m_powerConsumption;
			value6.enginePower += basePart4.m_enginePower * m_enginePowerFactor;
			if (basePart4.m_enginePower > 0f && basePart4.m_partType != BasePart.PartType.Pig)
			{
				value6.hasEngine = true;
			}
			if (basePart4.m_partType == BasePart.PartType.Gearbox && basePart4 is Gearbox gearbox)
			{
				value6.hasGearbox = true;
				value6.gearbox = gearbox;
			}
			if (basePart4.m_partType == BasePart.PartType.MotorWheel && basePart4 is MotorWheel item)
			{
				value6.motorWheels.Add(item);
			}
			m_connectedComponents[num2] = value6;
		}
		this.ConnectedComponentsChangedEvent?.Invoke();
		for (int num3 = 0; num3 < count; num3++)
		{
			BasePart basePart5 = parts[num3];
			if (basePart5.m_partType == BasePart.PartType.Rope && basePart5 is Rope rope)
			{
				if (rope.LeftPart != null && dictionary.TryGetValue(rope.LeftPart, out var value7))
				{
					disjointSet.Union(num3, value7);
				}
				if (rope.RightPart != null && dictionary.TryGetValue(rope.RightPart, out value7))
				{
					disjointSet.Union(num3, value7);
				}
			}
		}
		disjointSet.GetComponentIndexes(array, out componentCount);
		GeneralConnectedComponentCount = componentCount;
		for (int num4 = 0; num4 < count; num4++)
		{
			parts[num4].GeneralConnectedComponent = array[num4];
		}
	}

	public bool PartIsConnected(BasePart part)
	{
		for (int i = 0; i < m_jointMap.Count; i++)
		{
			if (m_jointMap[i].partA == part)
			{
				return true;
			}
			if (m_jointMap[i].partB == part)
			{
				return true;
			}
		}
		return false;
	}

	private void InitializeEngines()
	{
		foreach (BasePart part in m_parts)
		{
			part.InitializeEngine();
		}
	}

	public void StopContraption()
	{
		for (int i = 0; i < m_parts.Count; i++)
		{
			BasePart basePart = m_parts[i];
			if (basePart != null)
			{
				UnityEngine.Object.Destroy(basePart.gameObject);
			}
		}
		m_running = false;
	}

	public float GetJointConnectionStrength(BasePart.JointConnectionStrength strength)
	{
		return strength switch
		{
			BasePart.JointConnectionStrength.Weak => WPFMonoBehaviour.gameData.m_jointConnectionStrengthWeak, 
			BasePart.JointConnectionStrength.Normal => WPFMonoBehaviour.gameData.m_jointConnectionStrengthNormal * ((INSettings.GetFloat(INFeature.ConnectionStrength) > 1f) ? 2f : 1f), 
			BasePart.JointConnectionStrength.High => WPFMonoBehaviour.gameData.m_jointConnectionStrengthHigh, 
			BasePart.JointConnectionStrength.Extreme => WPFMonoBehaviour.gameData.m_jointConnectionStrengthExtreme, 
			BasePart.JointConnectionStrength.HighlyExtreme => WPFMonoBehaviour.gameData.m_jointConnectionStrengthHighlyExtreme, 
			_ => 0f, 
		};
	}

	public void AddFixedJoint(BasePart part, BasePart other)
	{
		other.EnsureRigidbody();
		Joint joint;
		if (part.m_jointType == BasePart.JointType.HingeJoint || other.m_jointType == BasePart.JointType.HingeJoint)
		{
			HingeJoint hingeJoint = part.gameObject.AddComponent<HingeJoint>();
			hingeJoint.autoConfigureConnectedAnchor = false;
			hingeJoint.anchor = part.transform.InverseTransformPoint(other.transform.position) * 0.5f;
			hingeJoint.connectedAnchor = other.transform.InverseTransformPoint(part.transform.position) * 0.5f;
			JointLimits limits = hingeJoint.limits;
			limits.min = -0.1f;
			limits.max = 0.1f;
			hingeJoint.limits = limits;
			hingeJoint.axis = Vector3.forward;
			hingeJoint.useLimits = true;
			joint = hingeJoint;
		}
		else
		{
			joint = part.gameObject.AddComponent<FixedJoint>();
		}
		joint.connectedBody = other.rigidbody;
		joint.enablePreprocessing = part.JointPreprocessing && other.JointPreprocessing;
		float jointConnectionStrength = GetJointConnectionStrength(part.GetJointConnectionStrength());
		float jointConnectionStrength2 = GetJointConnectionStrength(other.GetJointConnectionStrength());
		float breakForce = jointConnectionStrength + jointConnectionStrength2;
		joint.breakForce = breakForce;
		AddJointToMap(part, other, joint);
	}

	public void MoveOnGrid(int dx, int dy)
	{
		m_partMap.Clear();
		if (INSettings.GetBool(INFeature.InfiniteGrid))
		{
			foreach (BasePart part in m_parts)
			{
				SetPartPos(part.m_coordX + dx, part.m_coordY + dy, part);
			}
			return;
		}
		foreach (BasePart part2 in m_parts)
		{
			int num = part2.m_coordX + dx;
			int num2 = part2.m_coordY + dy;
			if (num >= WPFMonoBehaviour.levelManager.GridXMin && num <= WPFMonoBehaviour.levelManager.GridXMax && num2 >= 0 && num2 < WPFMonoBehaviour.levelManager.GridHeight)
			{
				SetPartPos(num, num2, part2);
			}
		}
	}

	public bool CanMoveOnGrid(int dx, int dy)
	{
		if (m_parts.Count == 0)
		{
			return false;
		}
		if (INSettings.GetBool(INFeature.InfiniteGrid))
		{
			return true;
		}
		foreach (BasePart part in m_parts)
		{
			int num = part.m_coordX + dx;
			int num2 = part.m_coordY + dy;
			if (num < WPFMonoBehaviour.levelManager.GridXMin || num > WPFMonoBehaviour.levelManager.GridXMax || num2 < 0 || num2 >= WPFMonoBehaviour.levelManager.GridHeight)
			{
				return false;
			}
		}
		return true;
	}

	private void SetPartPos(int x, int y, BasePart part)
	{
		if (part != null)
		{
			part.m_coordX = x;
			part.m_coordY = y;
			float num = (0f - (float)(x + 2 * y)) / 100000f;
			part.transform.localPosition = new Vector3(x, y, -0.1f + part.m_ZOffset + num);
		}
		if (part == null || part.m_enclosedInto == null)
		{
			int key = x + (y << 16);
			m_partMap[key] = part;
		}
	}

	public BasePart FindPartAt(int x, int y)
	{
		int key = x + (y << 16);
		BasePart value;
		if (m_runtimePartMap != null)
		{
			m_runtimePartMap.TryGetValue(key, out value);
			return value;
		}
		m_partMap.TryGetValue(key, out value);
		return value;
	}

	public bool IsPartTypeAt(int x, int y, BasePart.PartType type, BasePart.GridRotation rotation)
	{
		int key = x + (y << 16);
		if (m_partMap.TryGetValue(key, out var value) && (bool)value && value.m_partType == type)
		{
			return value.m_gridRotation == rotation;
		}
		return false;
	}

	public BasePart FindPartOfType(BasePart.PartType type)
	{
		foreach (BasePart part in m_parts)
		{
			if (part.m_partType == type)
			{
				return part;
			}
		}
		return null;
	}

	public List<BasePart> FindNeighbours(int x, int y)
	{
		List<BasePart> list = new List<BasePart>();
		BasePart basePart = FindPartAt(x - 1, y);
		if (basePart != null)
		{
			list.Add(basePart);
		}
		basePart = FindPartAt(x + 1, y);
		if (basePart != null)
		{
			list.Add(basePart);
		}
		basePart = FindPartAt(x, y - 1);
		if (basePart != null)
		{
			list.Add(basePart);
		}
		basePart = FindPartAt(x, y + 1);
		if (basePart != null)
		{
			list.Add(basePart);
		}
		return list;
	}

	public void RefreshNeighbours(int x, int y)
	{
		m_isValidationRequired = true;
		BasePart basePart = FindPartAt(x - 1, y);
		if (basePart != null)
		{
			basePart.OnChangeConnections();
		}
		BasePart basePart2 = FindPartAt(x + 1, y);
		if (basePart2 != null)
		{
			basePart2.OnChangeConnections();
		}
		BasePart basePart3 = FindPartAt(x, y + 1);
		if (basePart3 != null)
		{
			basePart3.OnChangeConnections();
		}
		BasePart basePart4 = FindPartAt(x, y - 1);
		if (basePart4 != null)
		{
			basePart4.OnChangeConnections();
		}
		if (m_currentGlue != 0)
		{
			Glue.ShowSuperGlue(this, m_currentGlue);
		}
	}

	public void RefreshNeighboursVisual(int x, int y)
	{
		BasePart basePart = FindPartAt(x - 1, y);
		if (basePart != null)
		{
			basePart.ChangeVisualConnections();
		}
		BasePart basePart2 = FindPartAt(x + 1, y);
		if (basePart2 != null)
		{
			basePart2.ChangeVisualConnections();
		}
		BasePart basePart3 = FindPartAt(x, y + 1);
		if (basePart3 != null)
		{
			basePart3.ChangeVisualConnections();
		}
		BasePart basePart4 = FindPartAt(x, y - 1);
		if (basePart4 != null)
		{
			basePart4.ChangeVisualConnections();
		}
		if (m_currentGlue != 0)
		{
			Glue.ShowSuperGlue(this, m_currentGlue);
		}
	}

	public BasePart SetPartAt(int x, int y, BasePart newPart)
	{
		return SetPartAt(x, y, newPart, refreshNeighbours: true);
	}

	public BasePart SetPartAt(int x, int y, BasePart newPart, bool refreshNeighbours)
	{
		BasePart basePart = FindPartAt(x, y);
		BasePart basePart2 = null;
		newPart.transform.parent = base.transform;
		newPart.contraption = this;
		SetPartPos(x, y, newPart);
		m_parts.Add(newPart);
		if (newPart.m_partType == BasePart.PartType.Pig && m_hasSuperMagnet && newPart.GetComponent<SuperMagnet>() == null)
		{
			newPart.gameObject.AddComponent<SuperMagnet>();
		}
		if (newPart.IsEngine() && m_enginePowerFactor > 1f)
		{
			AddTurboChargeEffect(newPart.gameObject);
		}
		if (newPart.m_partType == BasePart.PartType.Pig && m_hasNightVision)
		{
			ApplyNightVisionGoggles();
		}
		if (basePart != null)
		{
			BasePart enclosedPart = basePart.enclosedPart;
			if (basePart.CanBeEnclosed() && newPart.CanEncloseParts())
			{
				newPart.enclosedPart = basePart;
				if (refreshNeighbours)
				{
					basePart.OnChangeConnections();
				}
			}
			else if (newPart.CanBeEnclosed() && basePart.CanEncloseParts())
			{
				basePart.enclosedPart = newPart;
				SetPartPos(x, y, basePart);
				if (refreshNeighbours)
				{
					basePart.OnChangeConnections();
				}
				basePart2 = enclosedPart;
			}
			else
			{
				if (enclosedPart != null && newPart.CanEncloseParts())
				{
					newPart.enclosedPart = enclosedPart;
					basePart.enclosedPart = null;
				}
				basePart2 = basePart;
			}
		}
		if (basePart2 != null)
		{
			m_parts.Remove(basePart2);
		}
		if (refreshNeighbours)
		{
			newPart.OnChangeConnections();
			RefreshNeighbours(x, y);
		}
		m_isValidationRequired = true;
		return basePart2;
	}

	public BasePart RemovePartsAt(int x, int y)
	{
		BasePart basePart = FindPartAt(x, y);
		SetPartPos(x, y, null);
		if ((bool)basePart)
		{
			m_parts.Remove(basePart);
			if ((bool)basePart.enclosedPart)
			{
				m_parts.Remove(basePart.enclosedPart);
			}
		}
		return basePart;
	}

	public BasePart RemovePartAt(int x, int y)
	{
		BasePart basePart = FindPartAt(x, y);
		if ((bool)basePart)
		{
			BasePart enclosedPart = basePart.enclosedPart;
			if ((bool)enclosedPart)
			{
				basePart.enclosedPart = null;
				enclosedPart.enclosedInto = null;
				SetPartPos(x, y, basePart);
				basePart = enclosedPart;
			}
			else
			{
				SetPartPos(x, y, null);
			}
		}
		if ((bool)basePart)
		{
			m_parts.Remove(basePart);
		}
		RefreshNeighbours(x, y);
		return basePart;
	}

	public void RemovePart(BasePart part)
	{
		if ((bool)part)
		{
			if ((bool)part.enclosedPart)
			{
				part.enclosedPart.enclosedInto = null;
				part.enclosedPart = null;
			}
			m_parts.Remove(part);
		}
	}

	public void RemoveAllDynamicParts()
	{
		List<BasePart> list = new List<BasePart>();
		foreach (BasePart part in m_parts)
		{
			if (!part.m_static)
			{
				UnityEngine.Object.Destroy(part.gameObject);
				list.Add(part);
			}
		}
		foreach (BasePart item in list)
		{
			RemovePartAt(item.m_coordX, item.m_coordY);
		}
	}

	public void AutoAlign(BasePart part)
	{
		if (part.enclosedInto != null || part.m_autoAlign == BasePart.AutoAlignType.None)
		{
			return;
		}
		if (part.m_autoAlign == BasePart.AutoAlignType.Rotate)
		{
			BasePart.JointConnectionDirection jointConnectionDirection = BasePart.JointConnectionDirection.Any;
			if (IsConnectionSourceTo(part.m_coordX - 1, part.m_coordY, BasePart.Direction.Right))
			{
				jointConnectionDirection = BasePart.JointConnectionDirection.Left;
			}
			else if (IsConnectionSourceTo(part.m_coordX + 1, part.m_coordY, BasePart.Direction.Left))
			{
				jointConnectionDirection = BasePart.JointConnectionDirection.Right;
			}
			else if (IsConnectionSourceTo(part.m_coordX, part.m_coordY + 1, BasePart.Direction.Down))
			{
				jointConnectionDirection = BasePart.JointConnectionDirection.Up;
			}
			else if (IsConnectionSourceTo(part.m_coordX, part.m_coordY - 1, BasePart.Direction.Up))
			{
				jointConnectionDirection = BasePart.JointConnectionDirection.Down;
			}
			if ((bool)part.GetComponent<Rope>())
			{
				if (IsPartTypeAt(part.m_coordX - 1, part.m_coordY, BasePart.PartType.Rope, BasePart.GridRotation.Deg_0))
				{
					jointConnectionDirection = BasePart.JointConnectionDirection.Left;
				}
				else if (IsPartTypeAt(part.m_coordX + 1, part.m_coordY, BasePart.PartType.Rope, BasePart.GridRotation.Deg_0))
				{
					jointConnectionDirection = BasePart.JointConnectionDirection.Right;
				}
				else if (IsPartTypeAt(part.m_coordX, part.m_coordY + 1, BasePart.PartType.Rope, BasePart.GridRotation.Deg_270))
				{
					jointConnectionDirection = BasePart.JointConnectionDirection.Up;
				}
				else if (IsPartTypeAt(part.m_coordX, part.m_coordY - 1, BasePart.PartType.Rope, BasePart.GridRotation.Deg_270))
				{
					jointConnectionDirection = BasePart.JointConnectionDirection.Down;
				}
			}
			if (jointConnectionDirection == BasePart.JointConnectionDirection.Any)
			{
				return;
			}
			BasePart.GridRotation gridRotation = part.AutoAlignRotation(jointConnectionDirection);
			if ((bool)part.GetComponent<Rope>())
			{
				if (gridRotation == BasePart.GridRotation.Deg_180)
				{
					gridRotation = BasePart.GridRotation.Deg_0;
				}
				if (gridRotation == BasePart.GridRotation.Deg_90)
				{
					gridRotation = BasePart.GridRotation.Deg_270;
				}
			}
			part.SetRotation(gridRotation);
			part.OnChangeConnections();
			RefreshNeighboursVisual(part.m_coordX, part.m_coordY);
		}
		else if (part.m_autoAlign == BasePart.AutoAlignType.FlipVertically)
		{
			if (IsChassisPart(part.m_coordX + 1, part.m_coordY))
			{
				part.SetFlipped(flipped: false);
			}
			else if (IsChassisPart(part.m_coordX - 1, part.m_coordY))
			{
				part.SetFlipped(flipped: true);
			}
		}
	}

	public bool Flip(BasePart part)
	{
		bool result = false;
		if (part.m_autoAlign == (BasePart.AutoAlignType)(-1))
		{
			part.SetCustomRotation(part.GetCustomRotation() + 1);
			part.OnChangeConnections();
			RefreshNeighboursVisual(part.m_coordX, part.m_coordY);
			result = true;
		}
		else if (part.m_autoAlign == BasePart.AutoAlignType.FlipVertically)
		{
			for (int i = 0; i < 3; i++)
			{
				part.SetFlipped(!part.IsFlipped());
				result = i != 1;
				if (part.m_jointConnectionDirection == BasePart.JointConnectionDirection.Any || CanConnectTo(part, BasePart.ConvertDirection(part.GetJointConnectionDirection())))
				{
					break;
				}
			}
		}
		else if (part.m_autoAlign == BasePart.AutoAlignType.Rotate)
		{
			if (part.m_jointConnectionDirection == BasePart.JointConnectionDirection.Any)
			{
				part.RotateClockwise();
				part.OnChangeConnections();
				RefreshNeighboursVisual(part.m_coordX, part.m_coordY);
				result = true;
			}
			else
			{
				for (int j = 0; j < 5; j++)
				{
					part.RotateClockwise();
					result = j != 3;
					if (part is Rope)
					{
						if (part.m_gridRotation == BasePart.GridRotation.Deg_0 || part.m_gridRotation == BasePart.GridRotation.Deg_270)
						{
							break;
						}
					}
					else if (CanConnectTo(part, part.GetJointConnectionDirection()))
					{
						break;
					}
				}
				RefreshNeighboursVisual(part.m_coordX, part.m_coordY);
			}
		}
		if (INSettings.GetBool(INFeature.AutoSetPartBuildingRotation))
		{
			ContraptionExtensionData extensionData = INContraption.Instance.ExtensionData;
			if (extensionData != null)
			{
				(int, int) key = ((int)part.m_partType, part.customPartIndex);
				extensionData.PartRotations[key] = ((int)part.m_gridRotation, part.m_flipped);
			}
		}
		return result;
	}

	private bool IsChassisPart(int coordX, int coordY)
	{
		BasePart basePart = FindPartAt(coordX, coordY);
		if ((bool)basePart)
		{
			return basePart.IsPartOfChassis();
		}
		return false;
	}

	private bool IsConnectionSourceTo(int coordX, int coordY, BasePart.Direction direction)
	{
		BasePart basePart = FindPartAt(coordX, coordY);
		if ((bool)basePart && basePart.m_jointConnectionType == BasePart.JointConnectionType.Source)
		{
			if (!basePart.CanConnectTo(direction))
			{
				return basePart.CanCustomConnectTo(direction);
			}
			return true;
		}
		return false;
	}

	public bool CanPlaceSpecificPartAt(int coordX, int coordY, BasePart newPart)
	{
		BasePart basePart = FindPartAt(coordX, coordY);
		if ((bool)basePart && (bool)basePart.enclosedPart)
		{
			basePart = basePart.enclosedPart;
		}
		if ((bool)basePart)
		{
			if (!basePart.m_static || (basePart.CanEncloseParts() && newPart.CanBeEnclosed()))
			{
				if (basePart.CanBeEnclosed() && newPart.CanEncloseParts())
				{
					return true;
				}
				return true;
			}
			return false;
		}
		return true;
	}

	public void Update()
	{
		if (!m_running)
		{
			return;
		}
		if (m_pig != null)
		{
			if (m_pig.rigidbody.velocity.magnitude < 0.2f)
			{
				m_stopTimer += Time.deltaTime;
			}
			else
			{
				m_stopTimer = 0f;
			}
		}
		for (int i = 0; i < GuiManager.PointerCount; i++)
		{
			GuiManager.Pointer pointer = GuiManager.GetPointer(i);
			if (!pointer.down || pointer.onWidget)
			{
				continue;
			}
			Vector3 position = pointer.position;
			position.z = m_gameCamera.farClipPlane;
			Vector3 vector = WPFMonoBehaviour.ScreenToZ0(position);
			foreach (BasePart part in m_parts)
			{
				if ((bool)part && part.IsInInteractiveRadius(vector) && !part.enclosedPart && part.gameObject.layer != m_droppedSandbagLayer)
				{
					part.ProcessTouch(vector);
					EventManager.Send(default(UserInputEvent));
					break;
				}
			}
		}
	}

	public void SetVisible(bool visible)
	{
		if (visible && !base.gameObject.activeSelf)
		{
			Instance = this;
			INContraption.Create(this);
		}
		base.gameObject.SetActive(visible);
	}

	private void InternalSetVisible(Transform t, bool enable)
	{
		if (enable)
		{
			t.gameObject.SetActive(value: true);
		}
		for (int i = 0; i < t.childCount; i++)
		{
			InternalSetVisible(t.GetChild(i), enable);
		}
		if (!enable)
		{
			t.gameObject.SetActive(value: false);
		}
	}

	public void RefreshConnections()
	{
		foreach (BasePart part in m_parts)
		{
			part.OnChangeConnections();
		}
	}

	private void CopyActiveStates(GameObject original, GameObject clone)
	{
		clone.SetActive(original.activeSelf);
		for (int i = 0; i < original.transform.childCount; i++)
		{
			CopyActiveStates(original.transform.GetChild(i).gameObject, clone.transform.GetChild(i).gameObject);
		}
	}

	public void SetTouchingGround(bool touching)
	{
		m_contraptionTouchingGround = touching;
	}

	public bool GetTouchingGround()
	{
		return m_contraptionTouchingGround;
	}

	public void SetHangingFromHook(bool hanging, int id)
	{
		m_contraptionHangingFromHook[id] = hanging;
	}

	public bool GetHangingFromHook()
	{
		return m_contraptionHangingFromHook.ContainsValue(value: true);
	}

	public void SetSwings(int swing)
	{
		m_numberOfSwings = swing;
	}

	public int GetSwings()
	{
		return m_numberOfSwings;
	}

	public Contraption Clone()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject);
		CopyActiveStates(base.gameObject, gameObject);
		return gameObject.GetComponent<Contraption>();
	}

	public bool ConsiderCollided()
	{
		return Time.time - m_timeLastCollided < 0.3f;
	}

	public bool ValidateContraption()
	{
		if (!m_isValidationRequired)
		{
			return m_lastValidation;
		}
		if (INSettings.GetBool(INFeature.CancelPartValidation))
		{
			bool flag = false;
			bool flag2 = false;
			BasePart.PartType partType = ((SortedPartType)INSettings.GetInt(INFeature.CameraTargetPartType)).ToPartType();
			foreach (BasePart part in m_parts)
			{
				if (part.m_partType == BasePart.PartType.Pig)
				{
					flag = true;
				}
				if (part.m_partType == partType)
				{
					flag2 = true;
				}
			}
			m_lastValidation = flag && flag2;
			return m_lastValidation;
		}
		m_isValidationRequired = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		List<BasePart> list = new List<BasePart>();
		List<BasePart> list2 = new List<BasePart>();
		bool flag7 = true;
		foreach (BasePart part2 in m_parts)
		{
			if (part2.m_partType == BasePart.PartType.Pig)
			{
				flag3 = true;
			}
			if (part2.m_partType == BasePart.PartType.Egg)
			{
				flag4 = true;
			}
			if (part2.m_partType == BasePart.PartType.Pumpkin)
			{
				flag5 = true;
			}
			if (part2.m_partType == BasePart.PartType.TimeBomb)
			{
				flag6 = true;
			}
			if (part2.IsPartOfChassis())
			{
				if (list.Count == 0)
				{
					list.Add(part2);
				}
				list2.Add(part2);
			}
			if (!part2.ValidatePart())
			{
				flag7 = false;
			}
		}
		bool flag8;
		bool flag9;
		do
		{
			flag8 = false;
			flag9 = true;
			foreach (BasePart item in list2)
			{
				if (list.Contains(item))
				{
					continue;
				}
				foreach (BasePart item2 in FindNeighbours(item.m_coordX, item.m_coordY))
				{
					if ((bool)item2 && list.Contains(item2))
					{
						flag8 = true;
						list.Add(item);
						break;
					}
				}
				flag9 = false;
			}
		}
		while (flag8);
		if (WPFMonoBehaviour.levelManager.RequireConnectedContraption && !flag9)
		{
			return m_lastValidation = false;
		}
		if (!flag7 || !flag3 || (WPFMonoBehaviour.levelManager.EggRequired && !flag4) || (WPFMonoBehaviour.levelManager.PumpkinRequired && !flag5) || (WPFMonoBehaviour.levelManager.CurrentGameMode is CakeRaceMode && !flag6))
		{
			return m_lastValidation = false;
		}
		return m_lastValidation = true;
	}

	public void MakeUnbreakable()
	{
		foreach (JointConnection item in m_jointMap)
		{
			if (item.joint != null)
			{
				item.joint.breakForce = float.PositiveInfinity;
			}
		}
	}

	public List<Joint> FindPartFixedJoints(BasePart part)
	{
		return this.FindPartFixedJointsFast(part);
	}

	public void DestroyAllJoints()
	{
		foreach (JointConnection item in m_jointMap)
		{
			if (item.joint != null)
			{
				UnityEngine.Object.Destroy(item.joint);
			}
		}
		m_jointMap.Clear();
	}

	public void AddJointToMap(BasePart endJointA, BasePart endJointB, Joint joint)
	{
		joint.breakForce *= INSettings.GetFloat(INFeature.ConnectionStrength);
		JointConnection item = default(JointConnection);
		item.partA = endJointA;
		item.partB = endJointB;
		item.joint = joint;
		m_jointMap.Add(item);
		this.AddJointEdge(endJointA, endJointB, joint);
	}

	public bool HasComponentEngine(int componentIndex)
	{
		if (componentIndex >= 0 && componentIndex < m_connectedComponents.Count)
		{
			return m_connectedComponents[componentIndex].hasEngine;
		}
		return false;
	}

	public int ComponentPartCount(int componentIndex)
	{
		if (componentIndex >= 0 && componentIndex < m_connectedComponents.Count)
		{
			return m_connectedComponents[componentIndex].partCount;
		}
		return 0;
	}

	public bool HasPoweredPartsRunning(int componentIndex)
	{
		for (int i = 0; i < m_parts.Count; i++)
		{
			BasePart basePart = m_parts[i];
			if (basePart.ConnectedComponent == componentIndex && basePart.IsEnabled() && basePart.IsPowered())
			{
				return true;
			}
		}
		return false;
	}

	public void ApplySuperGlue(Glue.Type type)
	{
		if (m_currentGlue == Glue.Type.None)
		{
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.SuperGlueApplied);
			if (Singleton<SocialGameManager>.IsInstantiated())
			{
				Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.INDESTRUCTIBLE", 100.0);
			}
		}
		m_currentGlue = type;
		Glue.ShowSuperGlue(this, type);
	}

	public void RemoveSuperGlue()
	{
		m_currentGlue = Glue.Type.None;
		Glue.RemoveSuperGlue(this);
	}

	private void AddTurboChargeEffect(GameObject part)
	{
		if (part.transform.Find(WPFMonoBehaviour.gameData.m_turboChargeEffect.name) == null)
		{
			GameObject obj = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_turboChargeEffect, part.transform.position, Quaternion.identity);
			obj.name = WPFMonoBehaviour.gameData.m_turboChargeEffect.name;
			obj.transform.parent = part.transform;
		}
	}

	private void ResetTurboCharge()
	{
		m_enginePowerFactor = 1f;
		foreach (BasePart part in m_parts)
		{
			if (part.IsEngine())
			{
				Transform transform = part.transform.Find(WPFMonoBehaviour.gameData.m_turboChargeEffect.name);
				if ((bool)transform)
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
		}
	}

	private void ApplyTurboCharge()
	{
		if (m_enginePowerFactor != WPFMonoBehaviour.gameData.m_turboChargePowerFactor)
		{
			if (Singleton<SocialGameManager>.IsInstantiated())
			{
				Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.SUPER_CHARGED", 100.0);
			}
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.TurboChargeApplied);
		}
		m_enginePowerFactor = WPFMonoBehaviour.gameData.m_turboChargePowerFactor;
		foreach (BasePart part in m_parts)
		{
			if (part.IsEngine())
			{
				AddTurboChargeEffect(part.gameObject);
			}
		}
	}

	private void ApplyNightVisionGoggles()
	{
		foreach (BasePart part in m_parts)
		{
			if (part.m_partType != BasePart.PartType.Pig)
			{
				continue;
			}
			if (nightVisionGogglesPrefab == null)
			{
				nightVisionGogglesPrefab = Resources.Load<GameObject>("Prefabs/NightVisionGoggles");
			}
			if (m_hasNightVision && nightVisionGoggles == null)
			{
				nightVisionGoggles = UnityEngine.Object.Instantiate(nightVisionGogglesPrefab);
				nightVisionGoggles.transform.parent = part.transform.Find("PigVisualization/Face/MaskHolder");
				nightVisionGoggles.transform.localPosition = Vector3.up * 0.02f;
				nightVisionGoggles.transform.localScale = Vector3.one;
				PigHat componentInChildren = part.transform.GetComponentInChildren<PigHat>(includeInactive: true);
				if ((bool)componentInChildren)
				{
					componentInChildren.Show(show: false);
				}
			}
			else if (!m_hasNightVision && nightVisionGoggles != null)
			{
				UnityEngine.Object.Destroy(nightVisionGoggles);
				PigHat componentInChildren2 = part.transform.GetComponentInChildren<PigHat>(includeInactive: true);
				if ((bool)componentInChildren2)
				{
					componentInChildren2.Show(show: true);
				}
			}
		}
	}
}
