using System;
using UnityEngine;

public class CrateCompassIndicator : WPFMonoBehaviour
{
	private const float DEFAULT_CAMERA_SIZE = 10f;

	private const float MAX_DISTANCE = 50f;

	[SerializeField]
	private float m_offset;

	[SerializeField]
	private Transform m_spriteParent;

	private LootCrate m_crate;

	private GameObject m_currentSprite;

	private float m_viewHalfWidth;

	private float m_viewHalfHeight;

	private bool m_done;

	private LineRenderer m_meter;

	private float m_normalizedDistance;

	public bool Done => m_done;

	private void Awake()
	{
		m_viewHalfWidth = 10f * (float)Screen.width / (float)Screen.height;
		m_viewHalfHeight = m_viewHalfWidth * (float)Screen.height / (float)Screen.width;
		m_meter = base.transform.Find("CrateCompassArrow/DistanceMeter").GetComponent<LineRenderer>();
	}

	private void Update()
	{
		if (m_crate == null || m_crate.collected || m_crate.transform.position.y < -20f)
		{
			TurnOff();
			return;
		}
		UpdateArrow();
		DrawMeter(m_normalizedDistance);
	}

	public void AttachToCrate(LootCrate crate)
	{
		m_crate = crate;
		SetCrateImage();
	}

	public void Show()
	{
		if (m_crate == null || m_crate.Collected)
		{
			TurnOff();
		}
		else if (!base.gameObject.activeInHierarchy)
		{
			SetCrateImage();
			UpdateArrow();
			base.gameObject.SetActive(value: true);
		}
	}

	public void Hide()
	{
		if (base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void TurnOff()
	{
		m_done = true;
		base.gameObject.SetActive(value: false);
	}

	private void SetCrateImage()
	{
		if (!(m_crate == null) && !(m_currentSprite != null))
		{
			m_currentSprite = LootCrateGraphicSpawner.CreateCrateSmall(m_crate.CrateType, m_spriteParent, Vector3.zero, Vector3.one, Quaternion.identity);
			LayerHelper.SetLayer(m_currentSprite, base.gameObject.layer, children: true);
		}
	}

	private void DrawMeter(float ratio)
	{
		int num = (int)(ratio * 2f * (float)Math.PI / ((float)Math.PI / 36f)) + 1;
		m_meter.SetVertexCount(num);
		for (int i = 0; i < num; i++)
		{
			float f = (float)Math.PI / 2f - (float)i * ((float)Math.PI / 36f);
			float x = 0.9f * Mathf.Cos(f);
			float y = 0.9f * Mathf.Sin(f);
			m_meter.SetPosition(i, new Vector3(x, y));
		}
	}

	private void UpdateArrow()
	{
		if (!(m_crate == null))
		{
			float num = 0f;
			float num2 = 0f;
			if ((bool)WPFMonoBehaviour.hudCamera)
			{
				num = WPFMonoBehaviour.hudCamera.transform.position.x;
				num2 = WPFMonoBehaviour.hudCamera.transform.position.y;
			}
			Vector3 position = WPFMonoBehaviour.mainCamera.transform.position;
			Vector2 vector = m_crate.transform.position - position;
			vector.Normalize();
			float num3 = Mathf.Min(Mathf.Abs(m_viewHalfWidth / vector.x), Mathf.Abs(m_viewHalfHeight / vector.y));
			vector.Scale(new Vector3(num3 + m_offset, num3 + m_offset, 0f));
			float orthographicSize = WPFMonoBehaviour.mainCamera.orthographicSize;
			float x = Mathf.Clamp(Mathf.Abs(m_crate.transform.position.x - position.x), orthographicSize * (float)Screen.width / (float)Screen.height, 50f) - orthographicSize * (float)Screen.width / (float)Screen.height;
			float y = Mathf.Clamp(Mathf.Abs(m_crate.transform.position.y - position.y), orthographicSize, 50f) - orthographicSize;
			m_normalizedDistance = Mathf.Clamp01(new Vector3(x, y, 0f).magnitude / 50f);
			base.transform.position = new Vector3(num + vector.x, num2 + vector.y, base.transform.position.z);
			Transform child = base.transform.GetChild(0);
			if (child != null)
			{
				child.localRotation = Quaternion.LookRotation(child.forward, new Vector3(vector.x, vector.y, 0f));
			}
		}
	}

	public bool CheckCrateOnScreen()
	{
		Vector3 position = Camera.main.transform.position;
		if (Mathf.Abs(m_crate.transform.position.y - position.y) < Camera.main.orthographicSize)
		{
			return Mathf.Abs(m_crate.transform.position.x - position.x) < Camera.main.orthographicSize * (float)Screen.width / (float)Screen.height;
		}
		return false;
	}
}
