using System;
using UnityEngine;

public class BirdCompassIndicator : WPFMonoBehaviour
{
	private Bird m_bird;

	private GameObject m_hudCamera;

	public float m_offset;

	public const float DefaultCameraSize = 10f;

	private float m_viewHalfWidth;

	private float m_viewHalfHeight;

	private bool m_done;

	private LineRenderer m_meter;

	private SpriteAnimation m_compassAnimation;

	private SpriteAnimation m_animation;

	public bool Done => m_done;

	public void AttachToBird(Bird bird)
	{
		m_bird = bird;
	}

	public void Show()
	{
		if (m_bird == null || m_bird.IsCollided())
		{
			TurnOff();
		}
		else if (!base.gameObject.activeInHierarchy)
		{
			SetBirdImage();
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

	private void SetBirdImage()
	{
		string text = m_bird.GetBirdType().ToString();
		if (m_bird.IsSleeping())
		{
			text += "_sleep";
			m_compassAnimation.Play("Sleep");
		}
		else
		{
			text += "_awake";
			m_compassAnimation.Play("Awake");
		}
		m_animation.Play(text);
	}

	private void Awake()
	{
		m_viewHalfWidth = 10f * (float)Screen.width / (float)Screen.height;
		m_viewHalfHeight = m_viewHalfWidth * (float)Screen.height / (float)Screen.width;
		m_animation = GetComponent<SpriteAnimation>();
		m_compassAnimation = base.transform.Find("BirdCompassArrow").GetComponent<SpriteAnimation>();
		m_meter = base.transform.Find("BirdCompassArrow/WakeUpMeter").GetComponent<LineRenderer>();
	}

	private void Update()
	{
		if (m_bird == null || m_bird.IsCollided() || m_bird.transform.position.y < -20f)
		{
			TurnOff();
			return;
		}
		SetBirdImage();
		DrawMeter(m_bird.WakeUpProgress());
		UpdateArrow();
	}

	private void TurnOff()
	{
		m_done = true;
		base.gameObject.SetActive(value: false);
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
		if (!(m_bird == null))
		{
			float num = 0f;
			float num2 = 0f;
			m_hudCamera = GameObject.FindWithTag("HUDCamera");
			if ((bool)m_hudCamera)
			{
				num = m_hudCamera.transform.position.x;
				num2 = m_hudCamera.transform.position.y;
			}
			Vector2 vector = m_bird.transform.position - Camera.main.transform.position;
			vector.Normalize();
			float num3 = Mathf.Min(Mathf.Abs(m_viewHalfWidth / vector.x), Mathf.Abs(m_viewHalfHeight / vector.y));
			vector.Scale(new Vector3(num3 + m_offset, num3 + m_offset, 0f));
			base.transform.position = new Vector3(num + vector.x, num2 + vector.y, base.transform.position.z);
			Transform child = base.transform.GetChild(0);
			if (child != null)
			{
				child.localRotation = Quaternion.LookRotation(child.forward, new Vector3(vector.x, vector.y, 0f));
			}
		}
	}

	public bool CheckBirdOnScreen()
	{
		Vector3 position = Camera.main.transform.position;
		if (Mathf.Abs(m_bird.transform.position.y - position.y) < Camera.main.orthographicSize)
		{
			return Mathf.Abs(m_bird.transform.position.x - position.x) < Camera.main.orthographicSize * (float)Screen.width / (float)Screen.height;
		}
		return false;
	}
}
