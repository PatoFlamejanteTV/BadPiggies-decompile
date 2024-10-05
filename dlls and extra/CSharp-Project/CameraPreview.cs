using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraPreview : WPFMonoBehaviour
{
	public enum EasingAnimation
	{
		None,
		EasingIn,
		EasingOut,
		EasingInOut
	}

	[Serializable]
	public class CameraControlPoint
	{
		public Vector2 position;

		public float zoom = 2f;

		public EasingAnimation easing;

		public CameraControlPoint()
		{
			zoom = 1f;
			easing = EasingAnimation.EasingInOut;
		}
	}

	public EasingAnimation m_easing;

	[SerializeField]
	private List<CameraControlPoint> m_controlPoints = new List<CameraControlPoint>();

	public float m_animationTime;

	private int m_currentControlPointIndex = 1;

	private float m_timer;

	private bool m_done;

	private int m_fastPreviewMultiplier = 1;

	public bool Done => m_done;

	public List<CameraControlPoint> ControlPoints
	{
		get
		{
			if (Application.isPlaying && WPFMonoBehaviour.levelManager.CustomPreview != null)
			{
				return WPFMonoBehaviour.levelManager.CustomPreview;
			}
			return m_controlPoints;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		m_controlPoints = ControlPoints;
		if (m_controlPoints.Count >= 3)
		{
			m_controlPoints.Insert(0, m_controlPoints[0]);
			m_controlPoints.Add(m_controlPoints[m_controlPoints.Count - 1]);
			if (Singleton<BuildCustomizationLoader>.Instance.IsHDVersion)
			{
				m_controlPoints[m_controlPoints.Count - 1].zoom += 0.35f;
			}
			int num = WPFMonoBehaviour.levelManager.ConstructionUI.PartSelectorRowCount();
			if (num > 1 && WPFMonoBehaviour.levelManager.m_constructionGridRows.Count - 1 >= 6)
			{
				m_controlPoints[m_controlPoints.Count - 1].position += -0.5f * Vector2.up;
				m_controlPoints[m_controlPoints.Count - 1].zoom += 0.5f;
			}
			WPFMonoBehaviour.levelManager.ConstructionUI.SetPartSelectorMaxRowCount(num);
		}
	}

	public void UpdateCameraPreview(ref Vector3 cameraPosition, ref float cameraOrtoSize)
	{
		if (m_done)
		{
			return;
		}
		if (GuiManager.GetPointer().down)
		{
			m_fastPreviewMultiplier = 6;
		}
		Vector2 vector = cameraPosition;
		m_timer += Time.deltaTime * (float)m_fastPreviewMultiplier;
		float num = (vector - new Vector2(m_controlPoints[m_currentControlPointIndex + 1].position.x, m_controlPoints[m_currentControlPointIndex + 1].position.y)).magnitude;
		float num2 = Mathf.Abs(cameraOrtoSize - m_controlPoints[m_currentControlPointIndex + 1].zoom);
		if (m_timer > m_animationTime / (float)(m_controlPoints.Count - 3))
		{
			m_timer = m_animationTime / (float)(m_controlPoints.Count - 3);
			num = 0f;
			num2 = 0f;
		}
		if (num < 0.5f && num2 < 0.5f)
		{
			m_currentControlPointIndex++;
			m_timer = 0f;
			if (m_currentControlPointIndex == m_controlPoints.Count - 2)
			{
				m_done = true;
				return;
			}
		}
		float i = m_easing switch
		{
			EasingAnimation.EasingIn => MathsUtil.EasingInQuad(m_timer, 0f, 1f, m_animationTime / (float)(m_controlPoints.Count - 3)), 
			EasingAnimation.EasingOut => MathsUtil.EasingOutQuad(m_timer, 0f, 1f, m_animationTime / (float)(m_controlPoints.Count - 3)), 
			EasingAnimation.EasingInOut => MathsUtil.EaseInOutQuad(m_timer, 0f, 1f, m_animationTime / (float)(m_controlPoints.Count - 3)), 
			_ => m_timer / (m_animationTime / (float)(m_controlPoints.Count - 3)), 
		};
		float x = MathsUtil.CatmullRomInterpolate(m_controlPoints[m_currentControlPointIndex - 1].position.x, m_controlPoints[m_currentControlPointIndex].position.x, m_controlPoints[m_currentControlPointIndex + 1].position.x, m_controlPoints[m_currentControlPointIndex + 2].position.x, i);
		float y = MathsUtil.CatmullRomInterpolate(m_controlPoints[m_currentControlPointIndex - 1].position.y, m_controlPoints[m_currentControlPointIndex].position.y, m_controlPoints[m_currentControlPointIndex + 1].position.y, m_controlPoints[m_currentControlPointIndex + 2].position.y, i);
		float num3 = MathsUtil.CatmullRomInterpolate(m_controlPoints[m_currentControlPointIndex - 1].zoom, m_controlPoints[m_currentControlPointIndex].zoom, m_controlPoints[m_currentControlPointIndex + 1].zoom, m_controlPoints[m_currentControlPointIndex + 2].zoom, i);
		cameraPosition = new Vector3(x, y, cameraPosition.z);
		cameraOrtoSize = num3;
	}
}
