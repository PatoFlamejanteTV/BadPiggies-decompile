using System.Collections.Generic;
using UnityEngine;

public class PropertyPanelBuilding : PropertyPanel
{
	private static PropertyPanelBuilding s_instance;

	private string m_text;

	private Camera m_camera;

	public static PropertyPanelBuilding Instance => s_instance;

	public static PropertyPanelBuilding Create()
	{
		PropertyPanelBuilding propertyPanelBuilding = (s_instance = new PropertyPanelBuilding());
		propertyPanelBuilding.Initialize();
		return propertyPanelBuilding;
	}

	protected override void Initialize()
	{
		base.Initialize();
		m_status = StatusCode.Building;
	}

	public override void Start()
	{
		CreateText("PropertyTextBuilding", new Vector2(38f, -216f));
		m_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}

	public override void FixedUpdate()
	{
		string text = ((INUnity.Language == SystemLanguage.Chinese) ? "无" : "None");
		Contraption instance = Contraption.Instance;
		BasePart basePart = FindTargetPart();
		string text2 = ((basePart != null) ? basePart.transform.position.Vector2ToString(m_format) : text);
		string text3 = ((basePart != null) ? new Vector2Int(basePart.m_coordX, basePart.m_coordY).ToString() : text);
		string text4 = ((INUnity.Language != SystemLanguage.Chinese) ? (m_versionText + "\n" + PropertyPanel.FormatHeading2("Camera Target Properties") + "\n" + m_prefix + "Position " + text2 + "\n" + m_prefix + "Building Position " + text3 + "\n\n" + PropertyPanel.FormatHeading2("Camera Properties") + "\n" + m_prefix + "Size " + m_camera.orthographicSize.ToString(m_format) + "\n" + m_prefix + "Position " + m_camera.transform.position.Vector2ToString(m_format) + "\n\n" + PropertyPanel.FormatHeading2("Contraption Properties") + "\n" + m_prefix + "Part Count " + instance.Parts.Count + "\n") : (m_versionText + "\n" + PropertyPanel.FormatHeading2("目标部件属性") + "\n" + m_prefix + "位置\u3000 " + text2 + "\n" + m_prefix + "建造位置 " + text3 + "\n\n" + PropertyPanel.FormatHeading2("视野属性") + "\n" + m_prefix + "大小\u3000 " + m_camera.orthographicSize.ToString(m_format) + "\n" + m_prefix + "位置\u3000 " + m_camera.transform.position.Vector2ToString(m_format) + "\n\n" + PropertyPanel.FormatHeading2("载具属性") + "\n" + m_prefix + "部件数 " + instance.Parts.Count + "\n"));
		m_text = text4;
	}

	public override void Update()
	{
		LevelManager.GameState gameState = WPFMonoBehaviour.levelManager.gameState;
		RectTransform component = m_textMesh.GetComponent<RectTransform>();
		switch (gameState)
		{
		case LevelManager.GameState.Building:
			m_textMesh.text = m_text;
			component.anchoredPosition = new Vector2(38f, -216f);
			break;
		case LevelManager.GameState.PausedWhileBuilding:
			m_textMesh.text = m_text;
			component.anchoredPosition = new Vector2(38f, -324f);
			break;
		default:
			m_textMesh.text = string.Empty;
			break;
		}
	}

	private BasePart FindTargetPart()
	{
		BasePart.PartType partType = ((SortedPartType)INSettings.GetInt(INFeature.CameraTargetPartType)).ToPartType();
		List<BasePart> parts = Contraption.Instance.Parts;
		for (int num = parts.Count - 1; num >= 0; num--)
		{
			BasePart basePart = parts[num];
			if (basePart != null && basePart.m_partType == partType)
			{
				return basePart;
			}
		}
		return null;
	}
}
