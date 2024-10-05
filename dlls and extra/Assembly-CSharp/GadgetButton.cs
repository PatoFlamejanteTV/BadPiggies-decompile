using UnityEngine;

public class GadgetButton : Button
{
	public BasePart.PartType m_partType;

	public BasePart.Direction m_direction;

	public BasePart.PartTier m_partTier;

	public SpriteAnimation m_buttonAnimationPrefab;

	private float m_placementOrder;

	private LevelManager levelManager;

	private GameObject m_gadgetSprite;

	private float m_stateUpdateTimer;

	private bool m_partsEnabled;

	private bool m_enabled;

	private float m_enabledTimer;

	private SpriteAnimation m_spriteAnimation;

	private Collider[] colliders;

	private Renderer[] renderers;

	private VisibilityCondition visibilityCondition;

	public bool Enabled
	{
		get
		{
			return m_enabled;
		}
		set
		{
			colliders = GetComponentsInChildren<Collider>();
			renderers = GetComponentsInChildren<Renderer>();
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].enabled = value;
			}
			for (int j = 0; j < renderers.Length; j++)
			{
				renderers[j].enabled = value;
			}
			m_enabled = value;
		}
	}

	public VisibilityCondition VisibilityCondition => visibilityCondition;

	public float PlacementOrder
	{
		get
		{
			return m_placementOrder;
		}
		set
		{
			m_placementOrder = value;
			m_stateUpdateTimer = 0.01f * m_placementOrder;
		}
	}

	public override bool AllowMultitouch()
	{
		return true;
	}

	protected override void ButtonAwake()
	{
		m_spriteAnimation = base.gameObject.AddComponent<SpriteAnimation>();
		m_spriteAnimation.CopyFrom(m_buttonAnimationPrefab);
		if (soundEffect == null)
		{
			soundEffect = Singleton<GameManager>.Instance.gameData.commonAudioCollection.gadgetButtonClick;
		}
		levelManager = WPFMonoBehaviour.levelManager;
		colliders = GetComponentsInChildren<Collider>();
		renderers = GetComponentsInChildren<Renderer>();
		visibilityCondition = GetComponent<VisibilityCondition>();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		if ((m_partType == BasePart.PartType.RedRocket || m_partType == BasePart.PartType.Rocket) && Singleton<SocialGameManager>.IsInstantiated())
		{
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.CRASH_COURSE", 100.0, (int limit) => levelManager.ContraptionProto.GetPartCount(m_partType) >= limit);
		}
		EventManager.Send(new GadgetControlEvent(m_partType, m_direction));
	}

	private void OnEnable()
	{
		m_gadgetSprite = base.transform.Find("Gadget").gameObject;
		UpdateState();
	}

	protected override void ButtonUpdate()
	{
		m_stateUpdateTimer += Time.deltaTime;
		if (m_stateUpdateTimer >= 0.2f)
		{
			UpdateState();
			m_stateUpdateTimer = 0f;
		}
		if (!m_partsEnabled)
		{
			return;
		}
		m_enabledTimer += Time.deltaTime;
		if (m_partType == BasePart.PartType.Bellows)
		{
			Vector3 localScale = m_gadgetSprite.transform.localScale;
			localScale.y = Bellows.CompressionScale(m_enabledTimer);
			m_gadgetSprite.transform.localScale = localScale;
			if (m_enabledTimer > 0.8f + ((!Bellows.HasAlienBellows) ? 0.3f : 0.15f))
			{
				m_enabledTimer = 0f;
			}
		}
		else
		{
			m_gadgetSprite.transform.localPosition = (Vector3)Random.insideUnitCircle * 0.075f + new Vector3(0f, 0f, -0.1f);
		}
	}

	public void UpdateState()
	{
		if (m_partType != BasePart.PartType.Engine)
		{
			UpdateGadgetState();
		}
		else if ((bool)levelManager && (bool)levelManager.ContraptionRunning)
		{
			bool flag = levelManager.ContraptionRunning.AllPoweredPartsEnabled();
			if (flag)
			{
				m_spriteAnimation.Play("On");
			}
			else
			{
				m_spriteAnimation.Play("Off");
			}
			m_partsEnabled = flag;
		}
	}

	private void UpdateGadgetState()
	{
		if (!levelManager || !levelManager.ContraptionRunning)
		{
			return;
		}
		bool flag = ((m_partTier != 0) ? levelManager.ContraptionRunning.AnyPartsEnabled(m_partType, m_partTier, m_direction) : levelManager.ContraptionRunning.AnyPartsEnabled(m_partType, m_direction));
		if (!flag && m_partsEnabled)
		{
			m_gadgetSprite.transform.localPosition = new Vector3(0f, 0f, -0.1f);
			m_gadgetSprite.transform.localScale = Vector3.one;
			m_enabledTimer = 0f;
		}
		if (flag != m_partsEnabled)
		{
			if (flag)
			{
				m_spriteAnimation.Play("On");
			}
			else
			{
				m_spriteAnimation.Play("Off");
			}
		}
		m_partsEnabled = flag;
		if (m_partType != BasePart.PartType.Engine)
		{
			if (!levelManager.m_showOnlyEngineButton)
			{
				Enabled = levelManager.ContraptionRunning.HasActiveParts(m_partType, m_direction);
			}
			else
			{
				Enabled = false;
			}
		}
	}
}
