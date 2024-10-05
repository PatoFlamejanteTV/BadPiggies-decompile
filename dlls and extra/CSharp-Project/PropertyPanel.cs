using TMPro;
using UnityEngine;

public class PropertyPanel : INBehaviour
{
	protected GameObject m_canvas;

	protected TextMeshProUGUI m_textMesh;

	protected string m_format = "F2";

	protected string m_prefix;

	protected string m_versionText;

	protected int m_fontSize;

	protected Color m_textColor;

	protected float m_outlineWidth;

	protected Color m_outlineColor;

	protected virtual void Initialize()
	{
		INContraption.Instance.AddBehaviour(this);
		int versionType = INSettings.VersionType;
		string text = ((INUnity.Language != SystemLanguage.Chinese) ? (versionType switch
		{
			2 => "BPLE Mode-A", 
			1 => "BPLE Mode-O", 
			0 => "Original", 
			_ => "BPLE Mode-B", 
		}) : (versionType switch
		{
			2 => "新创A", 
			1 => "新创O", 
			0 => "原版", 
			_ => "新创B", 
		}));
		m_prefix = "\u3000";
		m_versionText = FormatHeading1(text + " " + INUnity.VersionText);
		m_fontSize = INSettings.GetInt(INFeature.PropertyPanelFontSize);
		m_outlineWidth = INSettings.GetFloat(INFeature.PropertyPanelOutlineWidth);
		try
		{
			m_textColor = INSettings.GetString(INFeature.PropertyPanelColor).ToColor32();
		}
		catch
		{
			m_textColor = INSettings.GetInitialString(INFeature.PropertyPanelColor).ToColor32();
		}
		try
		{
			m_outlineColor = INSettings.GetString(INFeature.PropertyPanelOutlineColor).ToColor32();
		}
		catch
		{
			m_outlineColor = INSettings.GetInitialString(INFeature.PropertyPanelOutlineColor).ToColor32();
		}
	}

	protected void CreateText(string name, Vector2 position)
	{
		GameObject gameObject = Object.Instantiate(INUnity.LoadGameObject("PropertyPanel"));
		gameObject.name = name;
		GameObject gameObject2 = gameObject.transform.Find("Text").gameObject;
		TextMeshProUGUI component = gameObject2.GetComponent<TextMeshProUGUI>();
		gameObject2.GetComponent<RectTransform>().anchoredPosition = position;
		component.fontSize = m_fontSize;
		component.color = m_textColor;
		component.outlineColor = m_outlineColor;
		component.outlineWidth = m_outlineWidth;
		m_canvas = gameObject;
		m_textMesh = component;
	}

	protected static string FormatHeading1(string text)
	{
		return "<b><size=28>" + text + "</size></b>";
	}

	protected static string FormatHeading2(string text)
	{
		return text;
	}

	protected static string[] FormatStrings(params string[] array)
	{
		int num = 0;
		string[] array2 = new string[array.Length];
		foreach (string text in array)
		{
			if (text.Length > num)
			{
				num = text.Length;
			}
		}
		for (int j = 0; j < array.Length; j++)
		{
			string text2 = array[j];
			array2[j] = text2 + new string(' ', num - text2.Length);
		}
		return array2;
	}

	public override void OnEnable()
	{
		if (m_canvas != null)
		{
			m_canvas.SetActive(value: true);
		}
	}

	public override void OnDisable()
	{
		if (m_canvas != null)
		{
			m_canvas.SetActive(value: false);
		}
	}

	public override void OnDestroy()
	{
		if (m_canvas != null)
		{
			Object.Destroy(m_canvas);
		}
	}
}
