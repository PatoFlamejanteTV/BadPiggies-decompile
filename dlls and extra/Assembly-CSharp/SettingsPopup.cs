using UnityEngine;

public class SettingsPopup : TextDialog
{
	[SerializeField]
	private TextMesh m_supportId;

	[SerializeField]
	private GameObject m_notConnectedBubble;

	private static bool s_optionsBubbleShowed;

	protected override void Awake()
	{
		base.Awake();
		if (m_notConnectedBubble != null)
		{
			m_notConnectedBubble.SetActive(!s_optionsBubbleShowed);
		}
		EventManager.Connect<PlayerChangedEvent>(OnPlayerChanged);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		OnPlayerChanged(null);
	}

	private void OnPlayerChanged(PlayerChangedEvent data)
	{
		if (m_supportId != null)
		{
			m_supportId.text = "INNOVATION PRODUCED BY 原野与森林";
			m_supportId.font = INUnity.ArialFont;
			m_supportId.fontSize = 64;
			m_supportId.fontStyle = FontStyle.Bold;
			m_supportId.anchor = TextAnchor.MiddleCenter;
			m_supportId.GetComponent<MeshRenderer>().material = m_supportId.font.material;
		}
		GameObject gameObject = GameObject.Find("CustomerIdLabel");
		if (gameObject != null)
		{
			TextMesh component = gameObject.GetComponent<TextMesh>();
			if (component != null)
			{
				component.text = string.Empty;
			}
		}
	}

	public new void Open()
	{
		base.Open();
		if (m_notConnectedBubble != null)
		{
			m_notConnectedBubble.SetActive(value: false);
		}
		s_optionsBubbleShowed = true;
	}

	public new void Close()
	{
		base.Close();
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<PlayerChangedEvent>(OnPlayerChanged);
	}
}
