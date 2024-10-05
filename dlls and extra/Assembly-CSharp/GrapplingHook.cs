using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : BasePart
{
	private enum State
	{
		WindedUp,
		Shoot,
		Winding,
		Rewind,
		Detach,
		Idle
	}

	private struct TemplatePartData
	{
		public BasePart Part;

		public bool Enabled;

		public BasePart EnclosedPart;

		public bool EnclosedPartEnabled;

		public TemplatePartData(BasePart part, BasePart enclosedPart)
			: this(part, enabled: false, enclosedPart, enclosedPartEnabled: false)
		{
		}

		public TemplatePartData(BasePart part, bool enabled, BasePart enclosedPart, bool enclosedPartEnabled)
		{
			Part = part;
			Enabled = enabled;
			EnclosedPart = enclosedPart;
			EnclosedPartEnabled = enclosedPartEnabled;
		}

		public BasePart GetTemplatePart()
		{
			if (EnclosedPartEnabled)
			{
				return EnclosedPart;
			}
			if (Enabled)
			{
				return Part;
			}
			return null;
		}
	}

	private float m_SpringVisualizationInitZ;

	private int m_GrapplingHookSolverIterCount = -1;

	private GameObject m_Visualization;

	private GameObject m_SpringVisualization;

	private GameObject m_GrapplingHook;

	private Rigidbody m_GrapplingHookRigidbody;

	private Hook m_GrapplingHookScript;

	public GameObject m_GrapplingHookPrefab;

	private bool m_enabled;

	private bool m_CanBeEnabled = true;

	private State m_State;

	public float m_rewindingTime = 4f;

	public float m_ShootTime = 2.5f;

	private Vector3 m_direction = Vector3.right;

	private FixedJoint[] m_FixedJointToBreak = new FixedJoint[32];

	private GameObject m_leftAttachment;

	private GameObject m_rightAttachment;

	private GameObject m_topAttachment;

	private GameObject m_bottomAttachment;

	private GameObject m_bottomLeftAttachment;

	private GameObject m_bottomRightAttachment;

	private GameObject m_topLeftAttachment;

	private GameObject m_topRightAttachment;

	private GameObject m_loopingReelSound;

	private ConfigurableJoint m_joint;

	private float m_shotStartTime;

	private float m_ReWindingStartTime;

	private float m_hookDistance;

	public float m_springStrength;

	public float m_minimumDistance;

	public float m_maxHookDistance;

	public float m_maxWindingSpeed;

	private float m_plungerResetTimer = 0.1f;

	private float m_plungerTimer;

	private float m_shootTime;

	private Vector2Int m_coordDirection;

	private Renderer[] m_renderers;

	private TemplatePartData m_templatePart;

	private List<TemplatePartData> m_templateParts;

	public override IEnumerable<Renderer> GetRenderers()
	{
		return m_renderers;
	}

	public Renderer[] GetRendererArray()
	{
		return m_renderers;
	}

	public override bool CanBeEnabled()
	{
		return m_CanBeEnabled;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	public override bool HasOnOffToggle()
	{
		return false;
	}

	public override Direction EffectDirection()
	{
		return BasePart.RotateWithEightDirections(Direction.Right, m_gridRotation);
	}

	public override void Awake()
	{
		base.Awake();
		m_leftAttachment = base.transform.Find("LeftAttachment").gameObject;
		m_rightAttachment = base.transform.Find("RightAttachment").gameObject;
		m_topAttachment = base.transform.Find("TopAttachment").gameObject;
		m_bottomAttachment = base.transform.Find("BottomAttachment").gameObject;
		m_bottomLeftAttachment = base.transform.Find("BottomLeftAttachment").gameObject;
		m_bottomRightAttachment = base.transform.Find("BottomRightAttachment").gameObject;
		m_topLeftAttachment = base.transform.Find("TopLeftAttachment").gameObject;
		m_topRightAttachment = base.transform.Find("TopRightAttachment").gameObject;
		m_leftAttachment.SetActive(value: false);
		m_rightAttachment.SetActive(value: false);
		m_topAttachment.SetActive(value: false);
		m_bottomAttachment.SetActive(value: false);
		m_bottomLeftAttachment.SetActive(value: false);
		m_bottomRightAttachment.SetActive(value: false);
		m_topLeftAttachment.SetActive(value: false);
		m_topRightAttachment.SetActive(value: false);
		m_SpringVisualization = base.transform.Find("SpringVisualization").gameObject;
		m_SpringVisualization.SetActive(value: false);
		m_SpringVisualizationInitZ = m_SpringVisualization.transform.localPosition.z;
		m_eightWay = true;
		GameObject gameObject = base.transform.Find("Visualization").gameObject;
		if (gameObject != null)
		{
			m_Visualization = gameObject;
		}
		m_enabled = false;
	}

	public override GridRotation AutoAlignRotation(JointConnectionDirection target)
	{
		return target switch
		{
			JointConnectionDirection.Right => GridRotation.Deg_180, 
			JointConnectionDirection.Up => GridRotation.Deg_0, 
			JointConnectionDirection.Left => GridRotation.Deg_0, 
			JointConnectionDirection.Down => GridRotation.Deg_90, 
			_ => GridRotation.Deg_0, 
		};
	}

	public override void EnsureRigidbody()
	{
		m_Visualization.transform.localRotation = Quaternion.identity;
		base.transform.localRotation = Quaternion.AngleAxis(GetRotationAngle(m_gridRotation), Vector3.forward);
		base.EnsureRigidbody();
		base.rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
	}

	public override void PrePlaced()
	{
		base.PrePlaced();
		SetRotation(GridRotation.Deg_0);
	}

	public override void ChangeVisualConnections()
	{
		if (INSettings.GetBool(INFeature.EnclosableParts) && m_enclosedInto != null)
		{
			m_leftAttachment.SetActive(value: false);
			m_rightAttachment.SetActive(value: false);
			m_topAttachment.SetActive(value: false);
			m_bottomAttachment.SetActive(value: false);
			m_bottomLeftAttachment.SetActive(value: false);
			m_bottomRightAttachment.SetActive(value: false);
			m_topLeftAttachment.SetActive(value: false);
			m_topRightAttachment.SetActive(value: false);
			m_SpringVisualization.SetActive(value: false);
			return;
		}
		bool flag = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Up, m_gridRotation));
		bool flag2 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Down, m_gridRotation));
		bool flag3 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Left, m_gridRotation));
		bool flag4 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Right, m_gridRotation));
		bool flag5 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.UpLeft, m_gridRotation));
		bool flag6 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.DownLeft, m_gridRotation));
		bool flag7 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.UpRight, m_gridRotation));
		bool flag8 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.DownRight, m_gridRotation));
		bool flag9 = m_gridRotation == GridRotation.Deg_135 || m_gridRotation == GridRotation.Deg_45 || m_gridRotation == GridRotation.Deg_225 || m_gridRotation == GridRotation.Deg_315;
		m_leftAttachment.SetActive(flag3 && !flag9);
		m_rightAttachment.SetActive(flag4 && !flag9);
		m_topAttachment.SetActive(flag && !flag9);
		m_bottomAttachment.SetActive((flag2 && !flag9) || (!flag && !flag3 && !flag4 && !flag9));
		m_bottomLeftAttachment.SetActive(flag6 && flag9);
		m_bottomRightAttachment.SetActive(flag8 && flag9);
		m_topLeftAttachment.SetActive(flag5 && flag9);
		m_topRightAttachment.SetActive(flag7 && flag9);
		if (!flag && !flag6 && !flag3 && !flag4 && !flag5 && !flag6 && !flag7 && !flag8)
		{
			m_bottomAttachment.SetActive(value: true);
		}
		m_SpringVisualization.SetActive(value: false);
	}

	public override void Initialize()
	{
		m_GrapplingHook = Object.Instantiate(m_GrapplingHookPrefab, base.transform.position, base.transform.rotation);
		m_GrapplingHookRigidbody = m_GrapplingHook.GetComponent<Rigidbody>();
		m_GrapplingHook.transform.parent = base.transform;
		m_GrapplingHookScript = m_GrapplingHook.GetComponent<Hook>();
		m_GrapplingHook.SetActive(value: true);
		InitializeGrapplingHook();
		if (WPFMonoBehaviour.levelManager.ContraptionRunning == null)
		{
			return;
		}
		Direction direction = EffectDirection();
		BasePart basePart = WPFMonoBehaviour.levelManager.ContraptionRunning.FindPartAt(m_coordX + direction switch
		{
			Direction.Left => -1, 
			Direction.Right => 1, 
			_ => 0, 
		}, m_coordY + direction switch
		{
			Direction.Down => -1, 
			Direction.Up => 1, 
			_ => 0, 
		});
		m_CanBeEnabled = !WPFMonoBehaviour.levelManager.ContraptionRunning.HasSuperGlue || !m_FixedJointToBreak[0] || basePart.m_partType == PartType.TNT;
		Renderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		m_renderers = componentsInChildren;
		if (!INSettings.GetBool(INFeature.PartGenerator) || !(m_enclosedInto != null))
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		m_templateParts = new List<TemplatePartData>();
		if (m_gridRotation == GridRotation.Deg_0 || m_gridRotation == GridRotation.Deg_45 || m_gridRotation == GridRotation.Deg_315)
		{
			num = 1;
		}
		else if (m_gridRotation == GridRotation.Deg_135 || m_gridRotation == GridRotation.Deg_180 || m_gridRotation == GridRotation.Deg_225)
		{
			num = -1;
		}
		if (m_gridRotation == GridRotation.Deg_45 || m_gridRotation == GridRotation.Deg_90 || m_gridRotation == GridRotation.Deg_135)
		{
			num2 = 1;
		}
		else if (m_gridRotation == GridRotation.Deg_225 || m_gridRotation == GridRotation.Deg_270 || m_gridRotation == GridRotation.Deg_315)
		{
			num2 = -1;
		}
		m_coordDirection = new Vector2Int(num, num2);
		if (INSettings.GetBool(INFeature.MultipartGenerator) && this.IsMultipartGenerator())
		{
			int @int = INSettings.GetInt(INFeature.MultipartGeneratorDetectionDistance);
			for (int i = 1; i <= @int; i++)
			{
				int x = m_coordX - num * i;
				int y = m_coordY - num2 * i;
				BasePart basePart2 = base.contraption.FindPartAt(x, y);
				if (basePart2 != null)
				{
					m_templateParts.Add(new TemplatePartData(basePart2, basePart2.m_enclosedPart));
				}
			}
		}
		else
		{
			int x2 = m_coordX - num;
			int y2 = m_coordY - num2;
			BasePart basePart3 = base.contraption.FindPartAt(x2, y2);
			if (basePart3 != null)
			{
				m_templatePart = new TemplatePartData(basePart3, basePart3.m_enclosedPart);
			}
		}
	}

	private void InitializeGrapplingHook()
	{
		m_GrapplingHook.SetActive(value: true);
		m_joint = m_GrapplingHook.GetComponent<ConfigurableJoint>();
		if (m_joint == null)
		{
			m_joint = m_GrapplingHook.AddComponent<ConfigurableJoint>();
		}
		m_joint.projectionMode = JointProjectionMode.PositionAndRotation;
		m_joint.xMotion = ConfigurableJointMotion.Free;
		m_joint.yMotion = ConfigurableJointMotion.Free;
		m_joint.zMotion = ConfigurableJointMotion.Locked;
		SoftJointLimitSpring linearLimitSpring = default(SoftJointLimitSpring);
		SoftJointLimit linearLimit = default(SoftJointLimit);
		linearLimit.bounciness = 1f;
		linearLimitSpring.spring = m_springStrength;
		linearLimitSpring.damper = 1.5f;
		linearLimit.limit = m_maxHookDistance;
		m_joint.linearLimit = linearLimit;
		m_joint.linearLimitSpring = linearLimitSpring;
		m_joint.connectedBody = base.rigidbody;
		m_joint.enablePreprocessing = false;
		m_SpringVisualization.SetActive(value: false);
		IgnoreCollisions();
		m_GrapplingHook.transform.position = base.transform.position;
		m_GrapplingHook.transform.rotation = base.transform.rotation;
		if (m_GrapplingHookSolverIterCount < 0)
		{
			m_GrapplingHookSolverIterCount = (int)((float)m_GrapplingHookRigidbody.solverIterations * 1.6f);
		}
		m_GrapplingHookRigidbody.solverIterations = m_GrapplingHookSolverIterCount;
		m_State = State.WindedUp;
		m_GrapplingHook.SetActive(value: false);
		m_GrapplingHookRigidbody.position = m_GrapplingHook.transform.position;
		m_GrapplingHookRigidbody.rotation = m_GrapplingHook.transform.rotation;
	}

	private void IgnoreCollisions()
	{
		if (base.collider != null && m_GrapplingHook != null && m_GrapplingHook.activeInHierarchy && m_GrapplingHook.GetComponent<Collider>() != null)
		{
			Physics.IgnoreCollision(base.collider, m_GrapplingHook.GetComponent<Collider>());
			if (m_enclosedInto != null && m_enclosedInto.collider != null)
			{
				Physics.IgnoreCollision(m_enclosedInto.collider, m_GrapplingHook.GetComponent<Collider>());
			}
		}
	}

	private void Shoot()
	{
		if (INSettings.GetBool(INFeature.PartGenerator) && m_enclosedInto != null)
		{
			if (base.GenerationLevel >= INSettings.GetInt(INFeature.PartGeneratorMaxGenerationLevel))
			{
				return;
			}
			Vector3 right = base.transform.right;
			BasePart templatePart = m_templatePart.GetTemplatePart();
			if (m_partTier == PartTier.Regular)
			{
				float @float = INSettings.GetFloat(INFeature.PartGeneratorCoolingTime);
				if (!(templatePart != null) || !(Time.time > @float + m_shootTime))
				{
					return;
				}
				m_shootTime = Time.time;
				BasePart basePart = templatePart;
				for (int i = 0; i < 2; i++)
				{
					if (i == 1)
					{
						basePart = null;
					}
					if (basePart != null)
					{
						BasePart basePart2 = INContraption.SetRuntimePartInternal(base.rigidbody.position + right, new Vector2Int(basePart.m_coordX, basePart.m_coordY), basePart.m_gridRotation, basePart.m_flipped, basePart.m_partType, basePart.customPartIndex);
						basePart2.transform.rotation = basePart.transform.rotation;
						basePart2.rigidbody.velocity = basePart.rigidbody.velocity;
						basePart2.rigidbody.AddForce(right * INSettings.GetFloat(INFeature.PartGeneratorForce), ForceMode.VelocityChange);
					}
				}
				return;
			}
			int x = m_coordDirection.x;
			int y = m_coordDirection.y;
			if (this.IsMultipartGenerator())
			{
				int num = customPartIndex - 8;
				int num2 = ((int[])INSettings.GetObject(INFeature.MultipartGeneratorDistances))[num];
				int @int = INSettings.GetInt(INFeature.MultipartGeneratorDetectionDistance);
				{
					foreach (TemplatePartData templatePart2 in m_templateParts)
					{
						templatePart = templatePart2.GetTemplatePart();
						if (templatePart != null)
						{
							int x2 = templatePart.m_coordX - m_coordX + (num2 + @int) * x;
							int y2 = templatePart.m_coordY - m_coordY + (num2 + @int) * y;
							PartGeneratorManager.Instance.GeneratePart(templatePart, this, x2, y2, discrete: false);
							if (CanGenerateChassis() && templatePart.m_enclosedInto != null)
							{
								PartGeneratorManager.Instance.GeneratePart(templatePart.m_enclosedInto, this, x2, y2, discrete: false);
							}
							m_enabled = true;
						}
					}
					return;
				}
			}
			if (templatePart != null)
			{
				int num3 = customPartIndex - 1;
				int num4 = ((int[])INSettings.GetObject(INFeature.PartGeneratorDistances))[num3];
				int x3 = num4 * x;
				int y3 = num4 * y;
				PartGeneratorManager.Instance.GeneratePart(templatePart, this, x3, y3, discrete: false);
				if (CanGenerateChassis() && templatePart.m_enclosedInto != null)
				{
					PartGeneratorManager.Instance.GeneratePart(templatePart.m_enclosedInto, this, x3, y3, discrete: false);
				}
				m_enabled = true;
			}
		}
		else
		{
			m_State = State.Shoot;
			m_enabled = true;
			m_SpringVisualization.SetActive(value: true);
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.grapplingHookLaunch, base.transform.position);
			m_shotStartTime = Time.time;
			Vector3 vector = base.transform.TransformDirection(m_direction);
			m_GrapplingHook.transform.parent = base.transform;
			m_GrapplingHook.transform.position = base.transform.position;
			m_GrapplingHook.transform.rotation = base.transform.rotation;
			m_GrapplingHook.SetActive(value: true);
			m_Visualization.SetActive(value: false);
			m_GrapplingHookRigidbody.AddForce(vector.normalized * 20f, ForceMode.Impulse);
		}
	}

	protected override void OnTouch()
	{
		if (!m_enabled && CanBeEnabled() && Time.time - m_plungerTimer > m_plungerResetTimer)
		{
			Shoot();
		}
		else
		{
			if (m_State != State.Shoot && m_State != State.Winding && m_State != State.Idle)
			{
				return;
			}
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.grapplingHookMiss, base.transform.position);
			m_State = State.Rewind;
			base.contraption.SetHangingFromHook(hanging: false, GetInstanceID());
			m_ReWindingStartTime = Time.time;
			if (Singleton<SocialGameManager>.IsInstantiated() && base.contraption.IsConnectedToPig(this, GetComponent<Collider>()) && !base.contraption.GetHangingFromHook() && Mathf.Abs(base.rigidbody.velocity.x) > 3f)
			{
				base.contraption.SetSwings(base.contraption.GetSwings() + 1);
				Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.SWINGIN_PIGGIES", 100.0);
				BasePart basePart = base.contraption.FindPartOfType(PartType.KingPig);
				if (base.contraption.GetSwings() > 1 && (bool)basePart && base.contraption.IsConnectedToPig(basePart, basePart.GetComponent<Collider>()))
				{
					Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.KING_PIG_OF_THE_JUNGLE", 100.0);
				}
			}
		}
	}

	private void Update()
	{
		if (m_State != 0 && (bool)base.rigidbody && (bool)m_GrapplingHookRigidbody)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = m_GrapplingHook.transform.position;
			m_SpringVisualization.transform.rotation = Quaternion.LookRotation(Vector3.back, position2 - position);
			Vector3 localScale = m_SpringVisualization.transform.localScale;
			localScale.y = Vector3.Distance(position, position2) * 1.25f;
			m_SpringVisualization.transform.localScale = localScale;
			m_SpringVisualization.transform.position = 0.5f * (position + position2);
			Vector3 localPosition = m_SpringVisualization.transform.localPosition;
			localPosition.z = m_SpringVisualizationInitZ;
			m_SpringVisualization.transform.localPosition = localPosition;
		}
	}

	private void FixedUpdate()
	{
		IgnoreCollisions();
		switch (m_State)
		{
		case State.WindedUp:
			if (!m_Visualization.activeSelf && Time.time - m_plungerTimer > m_plungerResetTimer)
			{
				PlaySound(WPFMonoBehaviour.gameData.commonAudioCollection.grapplingHookReset);
				m_Visualization.SetActive(value: true);
			}
			break;
		case State.Shoot:
			if (m_GrapplingHookScript.GetAttachType() != 0)
			{
				m_State = State.Winding;
				m_hookDistance = HookDistance();
				Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.grapplingHookAttach, m_GrapplingHook.transform.position);
				m_loopingReelSound = Singleton<AudioManager>.Instance.SpawnLoopingEffect(WPFMonoBehaviour.gameData.commonAudioCollection.grapplingHookReeling, base.transform);
				SoftJointLimit linearLimit3 = m_joint.linearLimit;
				linearLimit3.limit = m_hookDistance;
				m_joint.linearLimit = linearLimit3;
			}
			else if ((m_GrapplingHookScript.GetAttachType() == Hook.AttachType.None && HookDistance() > m_maxHookDistance) || Time.time - m_shotStartTime > m_ShootTime)
			{
				Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.grapplingHookMiss, base.transform.position);
				m_ReWindingStartTime = Time.time;
				m_State = State.Rewind;
			}
			break;
		case State.Winding:
		{
			float num = HookDistance();
			if (base.contraption.IsConnectedToPig(this, GetComponent<Collider>()))
			{
				base.contraption.SetHangingFromHook(hanging: true, GetInstanceID());
			}
			else
			{
				base.contraption.SetHangingFromHook(hanging: false, GetInstanceID());
			}
			if (num < m_minimumDistance)
			{
				if (m_loopingReelSound != null)
				{
					Singleton<AudioManager>.Instance.StopLoopingEffect(m_loopingReelSound.GetComponent<AudioSource>());
				}
				if (m_joint != null)
				{
					SoftJointLimit linearLimit = m_joint.linearLimit;
					linearLimit.limit = m_minimumDistance;
					m_joint.linearLimit = linearLimit;
					m_joint.zMotion = ConfigurableJointMotion.Locked;
					m_joint.xMotion = ConfigurableJointMotion.Limited;
					m_joint.yMotion = ConfigurableJointMotion.Limited;
				}
			}
			else
			{
				if (!(m_GrapplingHookScript != null) || m_GrapplingHookScript.GetAttachType() == Hook.AttachType.None)
				{
					break;
				}
				if ((double)num > 1.1 * (double)m_maxHookDistance)
				{
					PlaySound(WPFMonoBehaviour.gameData.commonAudioCollection.grapplingHookDetach);
					m_State = State.Rewind;
					base.contraption.SetHangingFromHook(hanging: false, GetInstanceID());
					break;
				}
				m_joint.zMotion = ConfigurableJointMotion.Locked;
				m_joint.xMotion = ConfigurableJointMotion.Limited;
				m_joint.yMotion = ConfigurableJointMotion.Limited;
				if (!(m_minimumDistance < num))
				{
					break;
				}
				SoftJointLimit linearLimit2 = m_joint.linearLimit;
				linearLimit2.limit -= m_maxWindingSpeed * Time.fixedDeltaTime;
				if (linearLimit2.limit < m_minimumDistance)
				{
					linearLimit2.limit = m_minimumDistance;
					if (m_loopingReelSound != null)
					{
						Singleton<AudioManager>.Instance.StopLoopingEffect(m_loopingReelSound.GetComponent<AudioSource>());
					}
				}
				m_joint.linearLimit = linearLimit2;
			}
			break;
		}
		case State.Rewind:
			if (m_joint != null)
			{
				if (m_loopingReelSound != null)
				{
					Singleton<AudioManager>.Instance.StopLoopingEffect(m_loopingReelSound.GetComponent<AudioSource>());
				}
				if (Time.time - m_ReWindingStartTime < m_rewindingTime && HookDistance() > 1f)
				{
					m_GrapplingHookScript.Reset();
					m_GrapplingHookRigidbody.AddForceAtPosition((base.transform.position - m_GrapplingHook.transform.position).normalized * 40f, m_GrapplingHook.transform.position, ForceMode.Force);
					float magnitude = m_GrapplingHookRigidbody.velocity.magnitude;
					m_GrapplingHookRigidbody.velocity = (base.transform.position - m_GrapplingHook.transform.position).normalized * magnitude;
				}
				else
				{
					m_State = State.Detach;
				}
			}
			break;
		case State.Detach:
			if (m_loopingReelSound != null)
			{
				Singleton<AudioManager>.Instance.StopLoopingEffect(m_loopingReelSound.GetComponent<AudioSource>());
			}
			m_State = State.WindedUp;
			m_enabled = false;
			InitializeGrapplingHook();
			m_GrapplingHookRigidbody.velocity = Vector3.zero;
			m_GrapplingHookScript.Reset();
			m_plungerTimer = Time.time;
			break;
		}
		if (base.contraption != null && base.contraption.GetTouchingGround())
		{
			base.contraption.SetTouchingGround(touching: false);
			base.contraption.SetSwings(0);
		}
		if (!base.contraption || !base.contraption.IsRunning || !INSettings.GetBool(INFeature.PartGenerator) || !(m_enclosedInto != null))
		{
			return;
		}
		if (INSettings.GetBool(INFeature.MultipartGenerator) && this.IsMultipartGenerator())
		{
			for (int i = 0; i < m_templateParts.Count; i++)
			{
				TemplatePartData template = m_templateParts[i];
				UpdateTemplatePart(ref template);
				m_templateParts[i] = template;
			}
		}
		else
		{
			UpdateTemplatePart(ref m_templatePart);
		}
	}

	private void PlaySound(AudioSource soundEffect)
	{
		Singleton<AudioManager>.Instance.SpawnOneShotEffect(soundEffect, m_GrapplingHook.transform.position);
	}

	private float HookDistance()
	{
		return (m_GrapplingHook.transform.position - base.transform.position).magnitude;
	}

	private void OnDestroy()
	{
		if (m_GrapplingHook != null)
		{
			Object.Destroy(m_GrapplingHook);
		}
	}

	private bool CanGenerateChassis()
	{
		BasePart basePart = m_enclosedInto;
		if (basePart != null && basePart.m_partType == PartType.MetalFrame)
		{
			return basePart.customPartIndex != 0;
		}
		return false;
	}

	private void UpdateTemplatePart(ref TemplatePartData template)
	{
		for (int i = 0; i < 2; i++)
		{
			BasePart basePart = ((i == 0) ? template.Part : template.EnclosedPart);
			bool flag = ((i == 0) ? template.Enabled : template.EnclosedPartEnabled);
			bool flag2 = basePart != null && basePart.GeneralConnectedComponent == base.GeneralConnectedComponent;
			if (i == 0)
			{
				template.Enabled = flag2;
			}
			else
			{
				template.EnclosedPartEnabled = flag2;
			}
			if (!(basePart != null))
			{
				continue;
			}
			if (flag2 && !flag)
			{
				if (basePart.IsEnabled())
				{
					basePart.SetEnabled(enabled: false);
				}
				basePart.GeneratorRefCount++;
			}
			else if (!flag2 && flag)
			{
				basePart.GeneratorRefCount--;
			}
		}
	}

	public void OnPartGenerated()
	{
		m_enabled = false;
	}
}
