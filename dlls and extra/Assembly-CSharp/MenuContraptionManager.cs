using System.Collections.Generic;
using UnityEngine;

public class MenuContraptionManager : MonoBehaviour
{
	private enum State
	{
		Default,
		Zooming,
		Following,
		Returning,
		ReturnToZoom
	}

	public GameData m_gameData;

	public MenuContraptionController m_menuContraptionControllerPrefab;

	private List<TextAsset> m_contraptionAssets = new List<TextAsset>();

	private int m_contraptionIndex;

	private Contraption m_contraption;

	private MenuContraptionController m_contraptionController;

	private float m_timer;

	private Camera m_camera;

	private GameObject m_cameraTarget;

	private Vector3 m_defaultCameraPosition;

	private float m_cameraStartSize;

	private Vector3 m_cameraStartPosition;

	private Vector3 m_zoomOutPosition;

	private State m_state;

	private float m_returnTimer;

	private float m_resourceUnloadTimer;

	private void Start()
	{
		m_camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		m_defaultCameraPosition = m_camera.transform.position;
		m_zoomOutPosition = m_defaultCameraPosition;
		Object[] array = Resources.LoadAll("MenuContraptions");
		foreach (Object @object in array)
		{
			m_contraptionAssets.Add(@object as TextAsset);
		}
		ShuffleContraptions();
		CreateContraption();
	}

	private void SetState(State state)
	{
		m_state = state;
		m_timer = 0f;
		if (state == State.Returning)
		{
			m_cameraStartPosition = m_camera.transform.position;
			m_cameraStartSize = m_camera.orthographicSize;
			m_zoomOutPosition = m_defaultCameraPosition;
		}
		if (state == State.Default)
		{
			m_cameraTarget = null;
		}
	}

	private Vector3 CameraTargetPosition()
	{
		float z = m_camera.transform.position.z;
		float x = m_cameraTarget.transform.position.x;
		float y = m_cameraTarget.transform.position.y;
		float x2 = Mathf.Clamp(x, -100f, 100f);
		y = Mathf.Clamp(y, -10f, 50f);
		return new Vector3(x2, y, z);
	}

	private void Update()
	{
		if (m_contraptionAssets.Count == 0)
		{
			return;
		}
		m_timer += Time.deltaTime;
		m_resourceUnloadTimer += Time.deltaTime;
		if (m_resourceUnloadTimer > 600f)
		{
			m_resourceUnloadTimer = 0f;
			Resources.UnloadUnusedAssets();
		}
		float num = 0.666f;
		float num2 = 8f;
		if (m_state == State.Default)
		{
			if ((bool)m_cameraTarget)
			{
				SetState(State.Zooming);
			}
			else
			{
				CreateContraption();
			}
		}
		else if (m_state == State.Zooming)
		{
			float num3 = 4f;
			float t = MathsUtil.EaseInOutQuad(m_timer, 0f, 1f, num3);
			Vector3 vector = CameraTargetPosition();
			vector = num * vector + (1f - num) * m_defaultCameraPosition;
			m_camera.transform.position = Vector3.Slerp(m_zoomOutPosition, vector, t);
			m_camera.orthographicSize = Mathf.Lerp(15f, num2, t);
			if (m_timer > num3)
			{
				SetState(State.Following);
			}
		}
		else if (m_state == State.ReturnToZoom)
		{
			m_returnTimer += Time.deltaTime;
			float num4 = 5f;
			float t2 = MathsUtil.EaseInOutQuad(m_timer, 0f, 1f, num4);
			Vector3 vector2 = CameraTargetPosition();
			vector2 = num * vector2 + (1f - num) * m_defaultCameraPosition;
			Vector3 vector3 = Vector3.Slerp(m_zoomOutPosition, vector2, t2);
			Vector3 vector4 = vector3;
			float num5 = 1f;
			if (m_returnTimer < 4f)
			{
				float t3 = MathsUtil.EaseInOutQuad(m_returnTimer, 0f, 1f, 4f);
				vector4 = Vector3.Slerp(m_cameraStartPosition, m_zoomOutPosition, t3);
				num5 = MathsUtil.EaseInOutQuad(Mathf.Clamp(m_timer, 0f, 1f), 0f, 1f, 1f);
			}
			m_camera.orthographicSize = Mathf.Lerp(15f, num2, t2);
			m_camera.transform.position = num5 * vector3 + (1f - num5) * vector4;
			if (m_timer > num4)
			{
				SetState(State.Following);
			}
		}
		else if (m_state == State.Following)
		{
			if (!m_cameraTarget)
			{
				SetState(State.Returning);
				return;
			}
			Vector3 vector5 = CameraTargetPosition();
			vector5 = num * vector5 + (1f - num) * m_defaultCameraPosition;
			m_camera.transform.position = vector5;
			m_camera.orthographicSize = num2;
			Vector3 position = m_cameraTarget.transform.position;
			Vector3 position2 = m_camera.transform.position;
			Vector2 vector6 = new Vector2(m_camera.orthographicSize * (float)Screen.width / (float)Screen.height, m_camera.orthographicSize);
			float num6 = -5f + -15f * (float)Screen.width / (float)Screen.height;
			if (position.x > position2.x + vector6.x - 5f || position.x < num6 - 10f || position.y > position2.y + vector6.y - 5f || position.y < -15f)
			{
				m_contraptionController.StartRemoveTimer(4f);
				SetState(State.Returning);
			}
			if (m_timer > 30f)
			{
				m_contraptionController.StartRemoveTimer(4f);
				SetState(State.Returning);
			}
			if (m_contraption.IsMovementStopped())
			{
				m_contraptionController.StartRemoveTimer(4f);
				SetState(State.Returning);
			}
		}
		else if (m_state == State.Returning)
		{
			float num7 = 4f;
			float t4 = MathsUtil.EaseInOutQuad(m_timer, 0f, 1f, num7);
			float num8 = MathsUtil.EaseInOutQuad(m_timer, 0f, 1f, num7 - 1f);
			Vector3 vector7 = Vector3.Slerp(m_cameraStartPosition, m_zoomOutPosition, t4);
			m_camera.orthographicSize = Mathf.Lerp(m_cameraStartSize, 15f, num8);
			Vector3 vector8 = vector7;
			if ((bool)m_cameraTarget)
			{
				vector8 = CameraTargetPosition();
				vector8 = num * vector8 + (1f - num) * m_defaultCameraPosition;
			}
			m_camera.transform.position = num8 * vector7 + (1f - num8) * vector8;
			if (m_timer > num7 - 1f)
			{
				m_returnTimer = m_timer;
				SetState(State.ReturnToZoom);
				CreateContraption();
			}
		}
	}

	private void CreateContraption()
	{
		GameObject gameObject = Object.Instantiate(m_menuContraptionControllerPrefab.gameObject);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		m_contraptionController = gameObject.GetComponent<MenuContraptionController>();
		m_cameraTarget = (m_contraption = gameObject.GetComponent<MenuContraptionController>().CreateContraption(m_contraptionAssets[m_contraptionIndex])).FindPig().gameObject;
		m_contraptionIndex++;
		if (m_contraptionIndex >= m_contraptionAssets.Count)
		{
			m_contraptionIndex = 0;
			ShuffleContraptions();
		}
	}

	private void ShuffleContraptions()
	{
		for (int i = 0; i < m_contraptionAssets.Count - 1; i++)
		{
			int index = Random.Range(i, m_contraptionAssets.Count);
			TextAsset value = m_contraptionAssets[i];
			m_contraptionAssets[i] = m_contraptionAssets[index];
			m_contraptionAssets[index] = value;
		}
	}
}
