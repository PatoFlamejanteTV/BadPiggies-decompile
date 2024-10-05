using UnityEngine;

public class PropertyPanelRunning : PropertyPanel
{
	private static PropertyPanelRunning s_instance;

	private string m_text;

	private Rigidbody m_targetPart;

	private Vector3 m_velocity;

	private Camera m_camera;

	public static PropertyPanelRunning Instance => s_instance;

	public static PropertyPanelRunning Create()
	{
		PropertyPanelRunning propertyPanelRunning = (s_instance = new PropertyPanelRunning());
		propertyPanelRunning.Initialize();
		return propertyPanelRunning;
	}

	protected override void Initialize()
	{
		base.Initialize();
		m_status = StatusCode.Running;
	}

	public override void Start()
	{
		CreateText("PropertyTextRunning", new Vector2(230f, -54f));
		m_targetPart = Contraption.Instance.m_cameraTarget.rigidbody;
		m_velocity = m_targetPart.velocity;
		m_camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}

	public override void FixedUpdate()
	{
		bool flag = INUnity.Language == SystemLanguage.Chinese;
		Contraption instance = Contraption.Instance;
		Vector3 velocity = m_targetPart.velocity;
		Vector3 vector = (velocity - m_velocity) / Time.fixedDeltaTime;
		Vector3 angularVelocity = m_targetPart.angularVelocity;
		float num = 0f;
		Vector2 zero = Vector2.zero;
		int num2 = 0;
		int count = instance.JointMap.Count;
		int num3 = (INSettings.GetBool(INFeature.FrameJoint) ? FrameJointManager.Instance.JointCount : 0);
		Rigidbody[] components = INContraption.Instance.GetComponents<Rigidbody>();
		foreach (Rigidbody obj in components)
		{
			float mass = obj.mass;
			Vector3 worldCenterOfMass = obj.worldCenterOfMass;
			num += mass;
			zero.x += worldCenterOfMass.x * mass;
			zero.y += worldCenterOfMass.y * mass;
			num2++;
		}
		string text4;
		if (flag)
		{
			string text = instance.Parts.Count.ToString();
			string text2 = instance.ConnectedComponentCount.ToString();
			string text3 = count.ToString();
			string[] array = PropertyPanel.FormatStrings(text, text2, text3);
			text = array[0];
			text2 = array[1];
			text3 = array[2];
			text4 = m_versionText + "\n" + PropertyPanel.FormatHeading2("目标部件属性") + "\n" + m_prefix + "速度\u3000 " + velocity.magnitude.ToString(m_format) + " " + velocity.Vector2ToString(m_format) + "\n" + m_prefix + "加速度 " + vector.magnitude.ToString(m_format) + " " + vector.Vector2ToString(m_format) + "\n" + m_prefix + "位置\u3000 " + m_targetPart.position.Vector2ToString(m_format) + "\n" + m_prefix + "角度\u3000 " + m_targetPart.rotation.eulerAngles.z.ToString(m_format) + "\n" + m_prefix + "角速度 " + angularVelocity.z.ToString(m_format) + "\n\n" + PropertyPanel.FormatHeading2("视野属性") + "\n" + m_prefix + "大小\u3000 " + m_camera.orthographicSize.ToString(m_format) + "\n" + m_prefix + "位置\u3000 " + m_camera.transform.position.Vector2ToString(m_format) + "\n\n" + PropertyPanel.FormatHeading2("载具属性") + "\n" + m_prefix + "部件数 " + text + " | " + "刚体数 " + num2 + "\n" + m_prefix + "载具数 " + text2 + " | " + "总质量 " + num.ToString(m_format) + "\n" + m_prefix + "连接数 " + text3 + " | " + "框架连接数 " + num3 + "\n";
		}
		else
		{
			text4 = m_versionText + "\n" + PropertyPanel.FormatHeading2("Camera Target Properties") + "\n" + m_prefix + "Velocity " + velocity.magnitude.ToString(m_format) + " " + velocity.Vector2ToString(m_format) + "\n" + m_prefix + "Acceleration " + vector.magnitude.ToString(m_format) + " " + vector.Vector2ToString(m_format) + "\n" + m_prefix + "Position " + m_targetPart.position.Vector2ToString(m_format) + "\n" + m_prefix + "Angle " + m_targetPart.rotation.eulerAngles.z.ToString(m_format) + "\n" + m_prefix + "Angular Velocity " + angularVelocity.z.ToString(m_format) + "\n\n" + PropertyPanel.FormatHeading2("Camera Properties") + "\n" + m_prefix + "Size " + m_camera.orthographicSize.ToString(m_format) + "\n" + m_prefix + "Position " + m_camera.transform.position.Vector2ToString(m_format) + "\n\n" + PropertyPanel.FormatHeading2("Contraption Properties") + "\n" + m_prefix + "Part Count " + instance.Parts.Count + " | " + "Rigidbody Count " + num2 + "\n" + m_prefix + "Vehicle Count " + instance.ConnectedComponentCount + " | " + "Total Mass " + num.ToString(m_format) + "\n" + m_prefix + "Joint Count " + count.ToString().ToString() + " | " + "Frame Joint Count " + num3 + "\n";
		}
		m_text = text4;
		m_targetPart = Contraption.Instance.m_cameraTarget.rigidbody;
		m_velocity = m_targetPart.velocity;
	}

	public override void Update()
	{
		LevelManager.GameState gameState = WPFMonoBehaviour.levelManager.gameState;
		RectTransform component = m_textMesh.GetComponent<RectTransform>();
		switch (gameState)
		{
		case LevelManager.GameState.PausedWhileRunning:
			m_textMesh.text = m_text;
			component.anchoredPosition = new Vector2(288f, -54f);
			break;
		case LevelManager.GameState.Running:
			m_textMesh.text = m_text;
			component.anchoredPosition = new Vector2(230f, -54f);
			break;
		default:
			m_textMesh.text = string.Empty;
			break;
		}
	}
}
