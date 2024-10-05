using System;
using System.Collections.Generic;
using UnityEngine;

public class BasePart : WPFMonoBehaviour
{
	public enum JointType
	{
		FixedJoint,
		HingeJoint
	}

	public enum PartTier
	{
		Regular,
		Common,
		Rare,
		Epic,
		Legendary
	}

	public enum PartType
	{
		Unknown,
		Balloon,
		Balloons2,
		Balloons3,
		Fan,
		WoodenFrame,
		Bellows,
		CartWheel,
		Basket,
		Sandbag,
		Pig,
		Sandbag2,
		Sandbag3,
		Propeller,
		Wings,
		Tailplane,
		Engine,
		Rocket,
		MetalFrame,
		SmallWheel,
		MetalWing,
		MetalTail,
		Rotor,
		MotorWheel,
		TNT,
		EngineSmall,
		EngineBig,
		NormalWheel,
		Spring,
		Umbrella,
		Rope,
		CokeBottle,
		KingPig,
		RedRocket,
		SodaBottle,
		PoweredUmbrella,
		Egg,
		JetEngine,
		ObsoleteWheel,
		SpringBoxingGlove,
		StickyWheel,
		GrapplingHook,
		Pumpkin,
		Kicker,
		Gearbox,
		GoldenPig,
		PointLight,
		SpotLight,
		TimeBomb,
		MAX
	}

	public enum AutoAlignType
	{
		None,
		Rotate,
		FlipVertically
	}

	public enum Direction
	{
		Right,
		Up,
		Left,
		Down,
		UpRight,
		UpLeft,
		DownLeft,
		DownRight
	}

	public enum GridRotation
	{
		Deg_0,
		Deg_90,
		Deg_180,
		Deg_270,
		Deg_45,
		Deg_135,
		Deg_225,
		Deg_315,
		Deg_Max
	}

	public enum JointConnectionType
	{
		None,
		Source,
		Target
	}

	public enum JointConnectionDirection
	{
		Any,
		Right,
		Up,
		Left,
		Down,
		LeftAndRight,
		UpAndDown,
		None
	}

	public enum JointConnectionStrength
	{
		Weak,
		Normal,
		High,
		Extreme,
		HighlyExtreme
	}

	private static float m_lastTimeUsedCollisionParticles;

	protected static int m_groundLayer = -1;

	protected static int m_iceGroundLayer = -1;

	protected bool m_isOnGround;

	[SerializeField]
	private bool m_jointPreprocessing;

	public bool m_eightWay;

	public int m_coordX;

	public int m_coordY;

	public float m_mass = 1f;

	public float m_interactiveRadius = 0.5f;

	public float m_breakVelocity;

	public float m_powerConsumption;

	public float m_enginePower;

	public float m_ZOffset;

	public int customPartIndex;

	public bool craftable = true;

	public bool lootCrateReward = true;

	public List<string> tags;

	private float m_lastTimeTouchedGround;

	private bool m_isOnIce;

	private AudioSource m_slidingSound;

	[SerializeField]
	private AudioManager.AudioMaterial audioMaterial;

	public JointType m_jointType;

	public PartTier m_partTier;

	public PartType m_partType;

	public AutoAlignType m_autoAlign;

	public bool m_flipped;

	public GridRotation m_gridRotation;

	public int m_gridXmin;

	public int m_gridXmax;

	public int m_gridYmin;

	public int m_gridYmax;

	public bool m_static;

	public JointConnectionStrength m_jointConnectionStrength;

	public JointConnectionType m_jointConnectionType = JointConnectionType.Target;

	public JointConnectionDirection m_jointConnectionDirection;

	public JointConnectionDirection m_customJointConnectionDirection = JointConnectionDirection.None;

	private Contraption m_contraption;

	private bool m_broken;

	private int m_connectedComponent = -1;

	public BasePart m_enclosedPart;

	public BasePart m_enclosedInto;

	public Sprite m_constructionIconSprite;

	protected bool m_valid;

	protected SpriteManager m_spriteManager;

	private Vector3 m_windVelocity;

	public bool VisibleOnPartListBeforeUnlocking
	{
		get
		{
			if (!CustomizationManager.IsPartUnlocked(this))
			{
				if (craftable)
				{
					return lootCrateReward;
				}
				return false;
			}
			return true;
		}
	}

	public bool JointPreprocessing => m_jointPreprocessing;

	public virtual Vector3 Position => base.transform.position;

	public AudioManager.AudioMaterial AudioMaterial => audioMaterial;

	public Contraption contraption
	{
		get
		{
			return m_contraption;
		}
		set
		{
			m_contraption = value;
		}
	}

	public int ConnectedComponent
	{
		get
		{
			return m_connectedComponent;
		}
		set
		{
			m_connectedComponent = value;
		}
	}

	public BasePart enclosedPart
	{
		get
		{
			return m_enclosedPart;
		}
		set
		{
			m_enclosedPart = value;
			if ((bool)value)
			{
				value.enclosedInto = this;
			}
		}
	}

	public BasePart enclosedInto
	{
		get
		{
			return m_enclosedInto;
		}
		set
		{
			m_enclosedInto = value;
		}
	}

	public Vector3 WindVelocity
	{
		get
		{
			return m_windVelocity;
		}
		set
		{
			m_windVelocity = value;
		}
	}

	public bool valid
	{
		get
		{
			return m_valid;
		}
		set
		{
			m_valid = value;
		}
	}

	public int StrictConnectedComponent { get; set; }

	public int GeneralConnectedComponent { get; set; }

	public int GeneratorRefCount { get; set; }

	public int GenerationLevel { get; set; }

	public virtual JointConnectionStrength GetJointConnectionStrength()
	{
		return m_jointConnectionStrength;
	}

	public static PartType BaseType(PartType type)
	{
		switch (type)
		{
		case PartType.Balloons2:
		case PartType.Balloons3:
			return PartType.Balloon;
		case PartType.Sandbag2:
		case PartType.Sandbag3:
			return PartType.Sandbag;
		default:
			return type;
		}
	}

	public virtual void Awake()
	{
		ConnectedComponent = -1;
		StrictConnectedComponent = -1;
		GeneralConnectedComponent = -1;
		m_valid = true;
		m_spriteManager = GetComponent<SpriteManager>();
		m_groundLayer = LayerMask.NameToLayer("Ground");
		m_iceGroundLayer = LayerMask.NameToLayer("IceGround");
	}

	public virtual void Initialize()
	{
	}

	public virtual void InitializeEngine()
	{
	}

	public virtual bool CanBeEnabled()
	{
		return false;
	}

	public virtual bool HasOnOffToggle()
	{
		return false;
	}

	public virtual bool IsEnabled()
	{
		return false;
	}

	public virtual void SetEnabled(bool enabled)
	{
	}

	public virtual bool IsPowered()
	{
		return m_powerConsumption > 0f;
	}

	public bool IsEngine()
	{
		return m_enginePower > 0f;
	}

	public virtual Direction EffectDirection()
	{
		return Direction.Right;
	}

	public virtual GridRotation AutoAlignRotation(JointConnectionDirection target)
	{
		if (m_jointConnectionDirection == JointConnectionDirection.None)
		{
			return RotationTo(m_customJointConnectionDirection, target);
		}
		return RotationTo(m_jointConnectionDirection, target);
	}

	public bool HasTag(string tag)
	{
		if (tags != null)
		{
			return tags.Contains(tag);
		}
		return false;
	}

	public bool IsFlipped()
	{
		return m_flipped;
	}

	public virtual void SetFlipped(bool flipped)
	{
		m_flipped = flipped;
		if (m_flipped)
		{
			base.transform.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
		}
		else
		{
			base.transform.localRotation = Quaternion.identity;
		}
		OnFlipped();
	}

	public static Direction ConvertDirection(JointConnectionDirection direction)
	{
		return (Direction)(direction - 1);
	}

	public static bool IsDirection(JointConnectionDirection direction)
	{
		if (direction != JointConnectionDirection.Left && direction != JointConnectionDirection.Right && direction != JointConnectionDirection.Up)
		{
			return direction == JointConnectionDirection.Down;
		}
		return true;
	}

	public JointConnectionDirection GetJointConnectionDirection()
	{
		if (INSettings.GetBool(INFeature.NewTimeBombConnection) && m_partType == PartType.TimeBomb)
		{
			return GlobalJointConnectionDirection((!(m_enclosedInto == null)) ? JointConnectionDirection.UpAndDown : JointConnectionDirection.Any);
		}
		return GlobalJointConnectionDirection(m_jointConnectionDirection);
	}

	public JointConnectionDirection GetCustomJointConnectionDirection()
	{
		return GlobalJointConnectionDirection(m_customJointConnectionDirection);
	}

	public JointConnectionDirection GlobalJointConnectionDirection(JointConnectionDirection localDirection)
	{
		if (localDirection == JointConnectionDirection.Any || localDirection == JointConnectionDirection.None)
		{
			return localDirection;
		}
		JointConnectionDirection jointConnectionDirection = localDirection;
		switch (localDirection)
		{
		case JointConnectionDirection.LeftAndRight:
			if (m_gridRotation == GridRotation.Deg_90 || m_gridRotation == GridRotation.Deg_270)
			{
				jointConnectionDirection = JointConnectionDirection.UpAndDown;
			}
			break;
		case JointConnectionDirection.UpAndDown:
			if (m_gridRotation == GridRotation.Deg_90 || m_gridRotation == GridRotation.Deg_270)
			{
				jointConnectionDirection = JointConnectionDirection.LeftAndRight;
			}
			break;
		default:
			jointConnectionDirection = (JointConnectionDirection)(((int)(localDirection - 1) + (int)m_gridRotation) % 4 + 1);
			break;
		}
		if (m_flipped)
		{
			switch (jointConnectionDirection)
			{
			case JointConnectionDirection.Left:
				return JointConnectionDirection.Right;
			case JointConnectionDirection.Right:
				return JointConnectionDirection.Left;
			case JointConnectionDirection.Up:
				return JointConnectionDirection.Down;
			case JointConnectionDirection.Down:
				return JointConnectionDirection.Up;
			}
		}
		return jointConnectionDirection;
	}

	public static Direction InverseDirection(Direction direction)
	{
		return direction switch
		{
			Direction.Right => Direction.Left, 
			Direction.Up => Direction.Down, 
			Direction.Left => Direction.Right, 
			Direction.Down => Direction.Up, 
			_ => Direction.Right, 
		};
	}

	public bool CanConnectTo(Direction direction)
	{
		switch (GetJointConnectionDirection())
		{
		case JointConnectionDirection.Any:
			return true;
		case JointConnectionDirection.Right:
			return direction == Direction.Right;
		case JointConnectionDirection.Up:
			return direction == Direction.Up;
		case JointConnectionDirection.Left:
			return direction == Direction.Left;
		case JointConnectionDirection.Down:
			return direction == Direction.Down;
		case JointConnectionDirection.LeftAndRight:
			if (direction != Direction.Left)
			{
				return direction == Direction.Right;
			}
			return true;
		case JointConnectionDirection.UpAndDown:
			if (direction != Direction.Up)
			{
				return direction == Direction.Down;
			}
			return true;
		case JointConnectionDirection.None:
			return false;
		default:
			return false;
		}
	}

	public bool CanCustomConnectTo(Direction direction)
	{
		switch (GetCustomJointConnectionDirection())
		{
		case JointConnectionDirection.Any:
			return true;
		case JointConnectionDirection.Right:
			return direction == Direction.Right;
		case JointConnectionDirection.Up:
			return direction == Direction.Up;
		case JointConnectionDirection.Left:
			return direction == Direction.Left;
		case JointConnectionDirection.Down:
			return direction == Direction.Down;
		case JointConnectionDirection.LeftAndRight:
			if (direction != Direction.Left)
			{
				return direction == Direction.Right;
			}
			return true;
		case JointConnectionDirection.UpAndDown:
			if (direction != Direction.Up)
			{
				return direction == Direction.Down;
			}
			return true;
		case JointConnectionDirection.None:
			return false;
		default:
			return false;
		}
	}

	public static Direction Rotate(Direction direction, GridRotation rotation)
	{
		return (Direction)(((int)direction + (int)rotation) % 4);
	}

	public static Direction RotateWithEightDirections(Direction direction, GridRotation rotation)
	{
		return (Direction)(((int)direction + (int)rotation) % 8);
	}

	public static GridRotation RotationTo(JointConnectionDirection source, JointConnectionDirection target)
	{
		if (source == JointConnectionDirection.Any || target == JointConnectionDirection.Any || source == JointConnectionDirection.None || target == JointConnectionDirection.None)
		{
			return GridRotation.Deg_0;
		}
		switch (source)
		{
		case JointConnectionDirection.LeftAndRight:
			if (target == JointConnectionDirection.UpAndDown || target == JointConnectionDirection.Up || target == JointConnectionDirection.Down)
			{
				return GridRotation.Deg_90;
			}
			return GridRotation.Deg_0;
		default:
			return (GridRotation)((target - source + 4) % 4);
		case JointConnectionDirection.UpAndDown:
			if (target == JointConnectionDirection.LeftAndRight || target == JointConnectionDirection.Left || target == JointConnectionDirection.Right)
			{
				return GridRotation.Deg_90;
			}
			return GridRotation.Deg_0;
		}
	}

	public float GetRotationAngle(GridRotation rotation)
	{
		return rotation switch
		{
			GridRotation.Deg_0 => 0f, 
			GridRotation.Deg_90 => 90f, 
			GridRotation.Deg_180 => 180f, 
			GridRotation.Deg_270 => 270f, 
			GridRotation.Deg_45 => 45f, 
			GridRotation.Deg_135 => 135f, 
			GridRotation.Deg_225 => 225f, 
			GridRotation.Deg_315 => 315f, 
			_ => 0f, 
		};
	}

	public virtual void SetRotation(GridRotation rotation)
	{
		m_gridRotation = rotation;
		base.transform.localRotation = Quaternion.AngleAxis(GetRotationAngle(rotation), Vector3.forward);
	}

	public void RotateClockwise()
	{
		switch (m_gridRotation)
		{
		case GridRotation.Deg_0:
			if (!m_eightWay)
			{
				SetRotation(GridRotation.Deg_270);
			}
			else
			{
				SetRotation(GridRotation.Deg_315);
			}
			break;
		case GridRotation.Deg_90:
			if (!m_eightWay)
			{
				SetRotation(GridRotation.Deg_0);
			}
			else
			{
				SetRotation(GridRotation.Deg_45);
			}
			break;
		case GridRotation.Deg_180:
			if (!m_eightWay)
			{
				SetRotation(GridRotation.Deg_90);
			}
			else
			{
				SetRotation(GridRotation.Deg_135);
			}
			break;
		case GridRotation.Deg_270:
			if (!m_eightWay)
			{
				SetRotation(GridRotation.Deg_180);
			}
			else
			{
				SetRotation(GridRotation.Deg_225);
			}
			break;
		case GridRotation.Deg_45:
			SetRotation(GridRotation.Deg_0);
			break;
		case GridRotation.Deg_135:
			SetRotation(GridRotation.Deg_90);
			break;
		case GridRotation.Deg_225:
			SetRotation(GridRotation.Deg_180);
			break;
		case GridRotation.Deg_315:
			SetRotation(GridRotation.Deg_270);
			break;
		}
	}

	public virtual bool IsInInteractiveRadius(Vector3 position)
	{
		Vector3 a = base.transform.position - base.rigidbody.velocity * Time.deltaTime;
		a.z = 0f;
		return Vector3.Distance(a, position) <= m_interactiveRadius;
	}

	public void ProcessTouch()
	{
		if ((!WPFMonoBehaviour.levelManager || WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Running) && (!INSettings.GetBool(INFeature.PartGenerator) || GeneratorRefCount <= 0))
		{
			OnTouch();
			OnTouch(hasPosition: false, Vector3.zero);
		}
	}

	public void ProcessTouch(Vector3 touchPosition)
	{
		if (!WPFMonoBehaviour.levelManager || WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Running)
		{
			OnTouch();
			OnTouch(hasPosition: true, touchPosition);
		}
	}

	protected virtual void OnTouch()
	{
	}

	protected virtual void OnTouch(bool hasPosition, Vector3 touchPosition)
	{
	}

	public virtual void OnCollisionEnter(Collision c)
	{
		Collider collider = c.collider;
		GameObject gameObject = collider.gameObject;
		int layer = gameObject.layer;
		BasePart component = collider.GetComponent<BasePart>();
		if (layer == m_groundLayer || gameObject.CompareTag("Ground"))
		{
			if (contraption.IsConnectedToPig(this, base.collider))
			{
				m_lastTimeTouchedGround = Time.time;
				contraption.SetTouchingGround(touching: true);
			}
			m_isOnGround = true;
		}
		if ((bool)component && component.ConnectedComponent == ConnectedComponent)
		{
			return;
		}
		m_isOnIce = gameObject.CompareTag("IceSurface");
		PlayCollisionAudio(this, c);
		if (!(Time.time - m_lastTimeUsedCollisionParticles > 0.25f))
		{
			return;
		}
		int num = c.contacts.Length;
		for (int i = 0; i < num; i++)
		{
			ContactPoint contactPoint = c.contacts[i];
			if (contactPoint.otherCollider.CompareTag("Untagged") && (bool)base.rigidbody)
			{
				m_lastTimeUsedCollisionParticles = Time.time;
				WPFMonoBehaviour.effectManager.CreateParticles(WPFMonoBehaviour.gameData.m_dustParticles, base.transform.position - Vector3.forward);
				float num2 = Vector3.Dot(base.rigidbody.GetPointVelocity(contactPoint.point), contactPoint.normal);
				if (m_breakVelocity > 0f && num2 > m_breakVelocity && !m_broken)
				{
					OnBreak();
					m_broken = true;
				}
				break;
			}
		}
	}

	public virtual void OnCollisionStay(Collision c)
	{
		int layer = c.collider.gameObject.layer;
		if (layer == m_groundLayer || layer == m_iceGroundLayer)
		{
			m_lastTimeTouchedGround = Time.time;
			m_contraption.SetGroundTouchTime(this);
		}
	}

	public virtual void OnCollisionExit(Collision c)
	{
		GameObject gameObject = c.gameObject;
		int layer = gameObject.layer;
		if (layer == m_iceGroundLayer)
		{
			m_isOnIce = false;
		}
		if (layer == m_groundLayer || gameObject.CompareTag("Ground"))
		{
			m_isOnGround = false;
		}
	}

	protected void LateUpdate()
	{
		UpdateSoundEffect();
	}

	public void PlayCollisionAudio(BasePart collisionPart, Collision collisionData)
	{
		if (collisionData.relativeVelocity.magnitude < 2.5f)
		{
			return;
		}
		AudioSource[] array = null;
		AudioSource[] array2 = null;
		switch (collisionPart.AudioMaterial)
		{
		default:
			array = null;
			array2 = null;
			break;
		case AudioManager.AudioMaterial.Wood:
			array = WPFMonoBehaviour.gameData.commonAudioCollection.collisionWoodHit;
			array2 = WPFMonoBehaviour.gameData.commonAudioCollection.collisionWoodDamage;
			break;
		case AudioManager.AudioMaterial.Metal:
			array = WPFMonoBehaviour.gameData.commonAudioCollection.collisionMetalHit;
			array2 = WPFMonoBehaviour.gameData.commonAudioCollection.collisionMetalDamage;
			break;
		}
		float num = 1f;
		if (this is GoldenPig)
		{
			array = WPFMonoBehaviour.gameData.commonAudioCollection.goldenPigHit;
			array2 = array;
			num = 1.5f;
		}
		if (array == null || array2 == null)
		{
			return;
		}
		float num2 = 0f;
		Vector3 soundPosition = Vector3.zero;
		foreach (ContactPoint collisionDatum in collisionData)
		{
			float num3 = Vector3.Dot(collisionData.relativeVelocity, collisionDatum.normal);
			if (num3 > num2)
			{
				num2 = num3 * num;
				soundPosition = collisionDatum.point;
			}
		}
		if (num2 > 8f)
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(array2, soundPosition);
		}
		else if (num2 > 2.5f)
		{
			AudioSource audioSource = Singleton<AudioManager>.Instance.SpawnOneShotEffect(array, soundPosition);
			if ((bool)audioSource)
			{
				audioSource.volume = (num2 - 2f) / 8f;
			}
		}
	}

	public void PlayBreakAudio(BasePart breakingPart)
	{
		AudioSource[] array = breakingPart.AudioMaterial switch
		{
			AudioManager.AudioMaterial.Wood => WPFMonoBehaviour.gameData.commonAudioCollection.collisionWoodDestroy, 
			AudioManager.AudioMaterial.Metal => WPFMonoBehaviour.gameData.commonAudioCollection.collisionMetalBreak, 
			_ => null, 
		};
		if (array != null)
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(array, breakingPart.transform.position);
		}
	}

	public virtual void PostInitialize()
	{
		AddShineEffect();
	}

	public virtual void PrePlaced()
	{
		if (!INSettings.GetBool(INFeature.DisableAlienPartParticles))
		{
			for (int i = 0; i < tags.Count; i++)
			{
				if (tags[i] == "Alien_part")
				{
					GameObject obj = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_alienPartParticles);
					obj.transform.parent = base.transform;
					obj.transform.localPosition = Vector3.back * 0.1f;
					obj.transform.localRotation = Quaternion.identity;
					obj.GetComponent<ParticleSystem>().startDelay = UnityEngine.Random.Range(0f, 2f);
				}
			}
		}
		if (WPFMonoBehaviour.levelManager != null && WPFMonoBehaviour.levelManager.CurrentGameMode is CakeRaceMode && Singleton<CakeRaceKingsFavorite>.Instance.CurrentFavorite.m_partType == m_partType && Singleton<CakeRaceKingsFavorite>.Instance.CurrentFavorite.m_partTier == m_partTier)
		{
			GameObject obj2 = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_heartParticles);
			obj2.transform.parent = base.transform;
			obj2.transform.localPosition = Vector3.back;
			obj2.transform.localRotation = Quaternion.identity;
		}
	}

	private void AddShineEffect()
	{
		if (WPFMonoBehaviour.levelManager != null && HasTag("Gold"))
		{
			SpriteShineEffect.AddOneTimeShine(base.gameObject);
		}
	}

	public static Vector3 GetDirectionVector(Direction direction)
	{
		return direction switch
		{
			Direction.Right => Vector3.right, 
			Direction.Up => Vector3.up, 
			Direction.Left => -Vector3.right, 
			Direction.Down => -Vector3.up, 
			_ => Vector3.up, 
		};
	}

	public virtual bool IsIntegralPart()
	{
		return true;
	}

	public virtual bool CanEncloseParts()
	{
		return false;
	}

	public virtual bool CanBeEnclosed()
	{
		if (INSettings.GetBool(INFeature.EnclosableParts))
		{
			if (INSettings.GetBool(INFeature.WoodenBox) && m_partType == PartType.WoodenFrame && customPartIndex == 10)
			{
				return true;
			}
			if (INSettings.GetBool(INFeature.MetalBox) && m_partType == PartType.MetalFrame && customPartIndex == 130)
			{
				return true;
			}
			if (m_partType != PartType.WoodenFrame)
			{
				return m_partType != PartType.MetalFrame;
			}
			return false;
		}
		return false;
	}

	public virtual bool IsPartOfChassis()
	{
		return false;
	}

	public virtual JointConnectionType GetJointConnectionType()
	{
		return m_jointConnectionType;
	}

	public virtual bool ValidatePart()
	{
		return true;
	}

	public virtual void EnsureRigidbody()
	{
		if (base.rigidbody == null)
		{
			base.rigidbody = base.gameObject.AddComponent<Rigidbody>();
		}
		base.rigidbody.constraints = (RigidbodyConstraints)56;
		base.rigidbody.mass = m_mass;
		base.rigidbody.drag = 0.2f;
		base.rigidbody.angularDrag = 0.05f;
		base.rigidbody.useGravity = true;
		base.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		if (base.gameObject.layer == LayerMask.NameToLayer("Default") || base.gameObject.layer == LayerMask.NameToLayer("Contraption"))
		{
			base.gameObject.layer = LayerMask.NameToLayer("Contraption");
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Contraption");
			}
		}
	}

	public virtual Joint CustomConnectToPart(BasePart part)
	{
		return null;
	}

	public virtual void OnChangeConnections()
	{
		if (m_jointConnectionDirection != 0 && m_jointConnectionDirection != JointConnectionDirection.None && !contraption.CanConnectTo(this, GetJointConnectionDirection()))
		{
			contraption.AutoAlign(this);
		}
		ChangeVisualConnections();
	}

	public virtual void ChangeVisualConnections()
	{
	}

	public void OnEnclosedPartDetached()
	{
	}

	public virtual void OnDetach()
	{
	}

	protected virtual void OnJointBreak(float breakForce)
	{
		HandleJointBreak();
	}

	protected virtual void OnFlipped()
	{
	}

	public virtual void OnPartPlaced()
	{
	}

	public void HandleJointBreak(bool playEffects = true)
	{
		CheckForBrokenPartsAchievement();
		contraption.UpdateConnectedComponents();
		if (playEffects)
		{
			PlayBreakAudio(this);
			Vector3 position = base.transform.position;
			Vector3 normalized = base.rigidbody.velocity.normalized;
			GameObject sprite = ((m_partType != PartType.WoodenFrame && m_partType != PartType.Pig) ? WPFMonoBehaviour.gameData.m_snapSprite : WPFMonoBehaviour.gameData.m_krakSprite);
			WPFMonoBehaviour.effectManager.ShowBreakEffect(sprite, position - 2f * normalized + new Vector3(0f, 0f, -10f), Quaternion.AngleAxis(UnityEngine.Random.Range(-30f, 30f), Vector3.forward));
		}
	}

	public virtual void OnBreak()
	{
	}

	protected virtual void UpdateSoundEffect()
	{
		if (!contraption || !contraption.IsRunning)
		{
			return;
		}
		float num = Mathf.Abs(base.rigidbody.velocity.magnitude);
		float num2 = 1f;
		float num3 = 10f;
		if (m_isOnIce && num > num2)
		{
			if (!m_slidingSound)
			{
				SpawnSlidingSound();
			}
			if ((bool)m_slidingSound)
			{
				m_slidingSound.volume = 0.25f * (Mathf.Clamp(num, num2, num3) / num3);
			}
		}
		else if ((bool)m_slidingSound)
		{
			if (Time.time - m_lastTimeTouchedGround < 0.4f)
			{
				m_slidingSound.volume = 0.25f * Mathf.Clamp01(1f - (Time.time - m_lastTimeTouchedGround) / 0.3f);
			}
			else
			{
				m_slidingSound.volume = 0f;
			}
		}
	}

	private void SpawnSlidingSound()
	{
		if ((bool)m_slidingSound)
		{
			m_slidingSound.Stop();
		}
		m_slidingSound = Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.partSlideOnIceLoop, base.transform);
		if ((bool)m_slidingSound)
		{
			m_slidingSound.volume = 0f;
		}
	}

	public void CheckForBrokenPartsAchievement()
	{
		if (!Singleton<SocialGameManager>.IsInstantiated() || !Singleton<GameManager>.Instance.IsInGame())
		{
			return;
		}
		int brokenParts = GameProgress.GetInt("Broken_Parts") + 1;
		GameProgress.SetInt("Broken_Parts", brokenParts);
		((Action<List<string>>)delegate(List<string> achievements)
		{
			foreach (string achievement in achievements)
			{
				if (Singleton<SocialGameManager>.Instance.TryReportAchievementProgress(achievement, 100.0, (int limit) => brokenParts >= limit))
				{
					break;
				}
			}
		})(new List<string> { "grp.VETERAN_WRECKER", "grp.QUALIFIED_WRECKER", "grp.JUNIOR_WRECKER" });
	}

	public string GetAnalyticsName()
	{
		return base.name;
	}

	public virtual IEnumerable<Rigidbody> GetRigidbodies()
	{
		yield return base.rigidbody;
	}

	public virtual IEnumerable<Collider> GetColliders()
	{
		yield return base.collider;
	}

	public virtual IEnumerable<Renderer> GetRenderers()
	{
		yield return base.renderer;
	}

	public virtual int GetCustomRotation()
	{
		return 0;
	}

	public virtual void SetCustomRotation(int rotation)
	{
	}

	public virtual void OnLightEnter(Vector2 point)
	{
	}

	public static (int, int) GetDirection(GridRotation rotation)
	{
		return rotation switch
		{
			GridRotation.Deg_0 => (1, 0), 
			GridRotation.Deg_45 => (1, 1), 
			GridRotation.Deg_90 => (0, 1), 
			GridRotation.Deg_135 => (-1, 1), 
			GridRotation.Deg_180 => (-1, 0), 
			GridRotation.Deg_225 => (-1, -1), 
			GridRotation.Deg_270 => (0, -1), 
			GridRotation.Deg_315 => (1, -1), 
			_ => default((int, int)), 
		};
	}

	public static void GetDirection(GridRotation rotation, out int x, out int y)
	{
		(x, y) = GetDirection(rotation);
	}
}
