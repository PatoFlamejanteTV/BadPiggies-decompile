using UnityEngine;

public class SpotLight : BasePart
{
	private bool activated;

	private GameObject m_leftAttachment;

	private GameObject m_rightAttachment;

	private GameObject m_topAttachment;

	private GameObject m_bottomAttachment;

	private GameObject m_bottomLeftAttachment;

	private GameObject m_bottomRightAttachment;

	private GameObject m_topLeftAttachment;

	private GameObject m_topRightAttachment;

	private PointLightSource lightSource;

	[SerializeField]
	private GameObject m_lightConeContainer;

	private float m_lightConeTargetScale;

	public EntityLight EntityLight { get; set; }

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
		lightSource = GetComponentInChildren<PointLightSource>();
		if (m_lightConeContainer != null)
		{
			m_lightConeContainer.transform.localScale = Vector3.one * m_lightConeTargetScale;
		}
	}

	public override GridRotation AutoAlignRotation(JointConnectionDirection target)
	{
		return target switch
		{
			JointConnectionDirection.Right => GridRotation.Deg_90, 
			JointConnectionDirection.Up => GridRotation.Deg_180, 
			JointConnectionDirection.Left => GridRotation.Deg_270, 
			JointConnectionDirection.Down => GridRotation.Deg_0, 
			_ => GridRotation.Deg_0, 
		};
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
	}

	private void FixedUpdate()
	{
		if ((bool)base.contraption && m_lightConeContainer != null)
		{
			if (INSettings.GetBool(INFeature.LightSystem) && !this.IsSinglePart())
			{
				m_lightConeContainer.transform.localScale = Vector3.zero;
			}
			else
			{
				m_lightConeContainer.transform.localScale = Vector3.Lerp(m_lightConeContainer.transform.localScale, Vector3.forward + Vector3.up + Vector3.right * m_lightConeTargetScale, Time.deltaTime * 30f);
			}
		}
	}

	public override bool CanBeEnabled()
	{
		return true;
	}

	public override bool HasOnOffToggle()
	{
		return true;
	}

	public override bool IsEnabled()
	{
		return activated;
	}

	public override bool IsIntegralPart()
	{
		return false;
	}

	public override bool IsPowered()
	{
		return false;
	}

	public override void OnDetach()
	{
		base.OnDetach();
	}

	protected override void OnTouch()
	{
		Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.toggleLamp, base.transform.position);
		SetEnabled(!activated);
	}

	public override void SetEnabled(bool enabled)
	{
		activated = enabled;
		lightSource.isEnabled = activated;
		base.contraption.UpdateEngineStates(base.ConnectedComponent);
		m_lightConeTargetScale = ((!activated) ? 0.001f : 3f);
	}

	public override void PrePlaced()
	{
		base.PrePlaced();
		if (INSettings.GetBool(INFeature.LightSystem))
		{
			EntityLight entityLight = base.gameObject.AddComponent<EntityLight>();
			if (m_partTier == PartTier.Regular)
			{
				entityLight.Type = 0;
				entityLight.Width = INSettings.GetFloat(INFeature.LinearLightWidth1) * 0.5f;
				entityLight.Length = INSettings.GetFloat(INFeature.LinearLightLength2);
			}
			else if (m_partTier == PartTier.Common)
			{
				entityLight.Type = 0;
				entityLight.Width = INSettings.GetFloat(INFeature.LinearLightWidth1) * 0.5f;
				entityLight.Length = INSettings.GetFloat(INFeature.LinearLightLength1);
			}
			else if (m_partTier == PartTier.Rare)
			{
				entityLight.Type = 1;
				entityLight.Width = INSettings.GetFloat(INFeature.LinearLightWidth2) * 0.5f;
				entityLight.Length = INSettings.GetFloat(INFeature.LinearLightLength2);
			}
			else if (m_partTier == PartTier.Epic)
			{
				entityLight.Type = 1;
				entityLight.Width = INSettings.GetFloat(INFeature.LinearLightWidth2) * 0.5f;
				entityLight.Length = INSettings.GetFloat(INFeature.LinearLightLength1);
			}
		}
	}

	public override void Initialize()
	{
		if (INSettings.GetBool(INFeature.LightSystem) && this.IsEntityLight())
		{
			EntityLight = GetComponent<EntityLight>();
		}
	}
}
