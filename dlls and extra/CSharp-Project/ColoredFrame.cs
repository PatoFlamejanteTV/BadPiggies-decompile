using UnityEngine;

public class ColoredFrame : Frame
{
	[SerializeField]
	private bool m_initialized;

	[SerializeField]
	private Color m_color;

	[SerializeField]
	private Color m_transparentColor;

	private INSettings.WrappedSettingData m_alpha;

	private INSettings.WrappedSettingData m_foregroundAlpha;

	private INSettings.WrappedSettingData m_backgroundAlpha;

	private BasePart m_coloredPart;

	private MeshRenderer m_renderer;

	private MeshRenderer m_foregroundRenderer;

	private MeshRenderer m_backgroundRenderer;

	private (MeshRenderer, Material)[] m_coloredPartMaterials;

	public Color Color
	{
		get
		{
			return m_color;
		}
		set
		{
			m_color = value;
		}
	}

	public Color TransparentColor => m_transparentColor;

	private void Start()
	{
		m_renderer = GetComponent<MeshRenderer>();
		m_foregroundRenderer = base.transform.Find("Foreground").GetComponent<MeshRenderer>();
		m_backgroundRenderer = base.transform.Find("Background").GetComponent<MeshRenderer>();
		m_alpha = INSettings.GetSettingsData(INFeature.ColoredFrameAlpha);
		m_foregroundAlpha = INSettings.GetSettingsData(INFeature.ColoredFrameForegroundAlpha);
		m_backgroundAlpha = INSettings.GetSettingsData(INFeature.ColoredFrameBackgroundAlpha);
		if (!m_initialized)
		{
			m_initialized = true;
			if (this.IsTransparentFrame())
			{
				float @float = INSettings.GetFloat(INFeature.TransparentFrameAlpha);
				m_color.a = @float;
				m_transparentColor = m_color;
			}
			Color color = m_color;
			color.a *= ((customPartIndex != 133) ? m_alpha.GetFloat() : 0f);
			m_renderer.material.color = color;
			Color color2 = m_color;
			color2.a *= m_foregroundAlpha.GetFloat();
			m_foregroundRenderer.material.color = color2;
			Color color3 = m_color;
			color3.a *= m_backgroundAlpha.GetFloat();
			m_backgroundRenderer.material.color = color3;
		}
	}

	public override void PostInitialize()
	{
		if (INSettings.GetBool(INFeature.NonCollisionColoredFrame))
		{
			GetComponent<Collider>().enabled = false;
		}
	}

	private void FixedUpdate()
	{
		bool flag = true;
		BasePart basePart = m_enclosedPart;
		float @float = INSettings.GetFloat(INFeature.EnclosedPartColorBlend);
		if (basePart != null)
		{
			bool num = basePart.IsWoodenBox();
			bool flag2 = basePart.IsMetalBox();
			flag = !(num || flag2);
			if (m_coloredPart == null)
			{
				if (INSettings.GetBool(INFeature.CanColorEnclosedPart))
				{
					MeshRenderer[] componentsInChildren = basePart.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
					m_coloredPartMaterials = new(MeshRenderer, Material)[componentsInChildren.Length];
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						MeshRenderer meshRenderer = componentsInChildren[i];
						if (meshRenderer.name != "INLight")
						{
							m_coloredPartMaterials[i] = (meshRenderer, meshRenderer.sharedMaterial);
							float num2 = ((m_color.a > 0.5f) ? m_color.a : 0.5f);
							meshRenderer.material.shader = INUnity.UnlitColorTransparentGrayOverlayShader;
							meshRenderer.material.color = new Color(m_color.r, m_color.g, m_color.b, num2 * meshRenderer.material.color.a);
							meshRenderer.material.SetFloat("_Blend", @float);
						}
					}
				}
				else
				{
					m_coloredPartMaterials = null;
				}
				m_coloredPart = basePart;
			}
		}
		m_renderer.enabled = flag;
		m_foregroundRenderer.enabled = flag;
		m_backgroundRenderer.enabled = flag;
	}

	public void UpdateRenderers()
	{
		Color color = m_color;
		color.a *= ((customPartIndex != 133) ? m_alpha.GetFloat() : 0f);
		m_renderer.material.color = color;
		Color color2 = m_color;
		color2.a *= m_foregroundAlpha.GetFloat();
		m_foregroundRenderer.material.color = color2;
		Color color3 = m_color;
		color3.a *= m_backgroundAlpha.GetFloat();
		m_backgroundRenderer.material.color = color3;
		if (m_coloredPartMaterials == null)
		{
			return;
		}
		(MeshRenderer, Material)[] coloredPartMaterials = m_coloredPartMaterials;
		for (int i = 0; i < coloredPartMaterials.Length; i++)
		{
			(MeshRenderer, Material) tuple = coloredPartMaterials[i];
			(Renderer, Material) tuple2 = (tuple.Item1, tuple.Item2);
			var (renderer, _) = tuple2;
			if (renderer != null)
			{
				float num = ((m_color.a > 0.5f) ? m_color.a : 0.5f);
				renderer.material.color = new Color(m_color.r, m_color.g, m_color.b, num * tuple2.Item2.color.a);
			}
		}
	}

	private void OnDestroy()
	{
		if (!(m_coloredPart != null) || m_coloredPartMaterials == null)
		{
			return;
		}
		(MeshRenderer, Material)[] coloredPartMaterials = m_coloredPartMaterials;
		for (int i = 0; i < coloredPartMaterials.Length; i++)
		{
			(MeshRenderer, Material) tuple = coloredPartMaterials[i];
			if (tuple.Item1 != null)
			{
				tuple.Item1.material = tuple.Item2;
			}
		}
	}
}
