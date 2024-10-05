using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameCamera : WPFMonoBehaviour
{
	public enum CameraState
	{
		Follow,
		Pan,
		Zoom,
		Wait,
		Building,
		Preview
	}

	private class PanningData
	{
		public float time;

		public Vector3 delta;

		public PanningData(float time, Vector3 delta)
		{
			this.time = time;
			this.delta = delta;
		}
	}

	protected Vector3 m_integratedVelocity;

	private LevelManager.CameraLimits m_cameraLimits;

	private const float ShowNextTargetCameraDelay = 2f;

	private const bool ZoomToNextBoxWhenCompleted = false;

	private CameraPreview m_cameraPreview;

	private Camera m_camera;

	private SnapshotEffect m_snapshotFX;

	private Vector3 currentPos;

	private Vector3 followPosition;

	private Vector3 panPosition;

	private bool m_freeCameraMode;

	private float m_mouseZoomDelta;

	private float m_keyZoomDelta;

	private float m_zoomGestureDistance;

	private bool isPanning;

	private float currentFOV;

	private float m_autoZoomAmount;

	private bool m_autoZoomEnabled = true;

	private const float CAMERA_MAX_ZOOM = 20f;

	private const float CAMERA_PREVIEW_ZOOM = 9f;

	private float m_cameraBuildZoom;

	private float m_cameraMaxZoom;

	private float m_cameraMinZoom;

	protected CameraState m_state;

	private Vector3 m_panStartPosition;

	private int m_panTouchFingerId;

	private float m_transitionTimer;

	private bool m_isStartingPan;

	private bool m_transitionToPreviewActive;

	private float m_returnToCenterTimer;

	private float m_returnToCenterSpeed;

	private Queue<PanningData> m_panningHistory = new Queue<PanningData>();

	private float m_panningSpeed;

	private Vector3 m_panningVelocity;

	private bool m_returningToDefaultPosition;

	private GameObject[] m_birdsCache;

	public CameraState State => m_state;

	public bool IsShowingBuildGrid(float allowedDistance)
	{
		if (m_state == CameraState.Building)
		{
			if (INSettings.GetBool(INFeature.NewCamera))
			{
				return true;
			}
			Vector3 newPos = m_cameraPreview.ControlPoints[m_cameraPreview.ControlPoints.Count - 1].position;
			float ortoSize = m_cameraBuildZoom;
			EnforceCameraLimits(ref newPos, ref ortoSize);
			Vector2 vector = newPos - currentPos;
			float f = ortoSize - currentFOV;
			if (vector.magnitude < allowedDistance)
			{
				return Mathf.Abs(f) < allowedDistance;
			}
			return false;
		}
		return false;
	}

	private void Start()
	{
		m_camera = GetComponent<Camera>();
		m_camera.orthographic = true;
		m_cameraLimits = WPFMonoBehaviour.levelManager.CurrentCameraLimits;
		m_cameraMaxZoom = Mathf.Min(Mathf.Min(m_cameraLimits.size.x, m_cameraLimits.size.y) / 2f, 20f) * INSettings.GetFloat(INFeature.CameraMaxZoom);
		m_cameraMinZoom = 7f;
		if (Singleton<BuildCustomizationLoader>.Instance.IsHDVersion)
		{
			if (ScreenPlacement.IsAspectRatioNarrowerThan(3f, 2f))
			{
				m_cameraMinZoom = 8.4f;
			}
			else
			{
				m_cameraMinZoom = 7.7f;
			}
		}
		m_camera.orthographicSize = m_cameraMinZoom;
		m_cameraPreview = GetComponent<CameraPreview>();
		m_snapshotFX = GetComponent<SnapshotEffect>();
		if (m_cameraPreview == null && WPFMonoBehaviour.levelManager.GoalPosition != null)
		{
			m_cameraPreview = base.gameObject.AddComponent<CameraPreview>();
			CameraPreview.CameraControlPoint cameraControlPoint = new CameraPreview.CameraControlPoint();
			cameraControlPoint.position = WPFMonoBehaviour.levelManager.GoalPosition.transform.position + WPFMonoBehaviour.levelManager.PreviewOffset;
			cameraControlPoint.zoom = WPFMonoBehaviour.levelManager.PreviewOffset.z / 2f;
			m_cameraPreview.ControlPoints.Add(cameraControlPoint);
			m_cameraPreview.ControlPoints.Add(cameraControlPoint);
			CameraPreview.CameraControlPoint cameraControlPoint2 = new CameraPreview.CameraControlPoint();
			cameraControlPoint2.position = new Vector2(m_cameraLimits.topLeft.x + m_cameraLimits.size.x / 2f, m_cameraLimits.topLeft.y - m_cameraLimits.size.y / 2f);
			cameraControlPoint2.zoom = m_cameraMaxZoom;
			m_cameraPreview.ControlPoints.Add(cameraControlPoint2);
			CameraPreview.CameraControlPoint cameraControlPoint3 = new CameraPreview.CameraControlPoint();
			cameraControlPoint3.position = WPFMonoBehaviour.levelManager.StartingPosition + WPFMonoBehaviour.levelManager.ConstructionOffset;
			cameraControlPoint3.zoom = WPFMonoBehaviour.levelManager.ConstructionOffset.z;
			m_cameraPreview.ControlPoints.Add(cameraControlPoint3);
			m_cameraPreview.ControlPoints.Add(cameraControlPoint3);
			m_cameraPreview.m_animationTime = WPFMonoBehaviour.levelManager.m_previewMoveTime;
			if (m_cameraPreview.m_animationTime < 1f)
			{
				m_cameraPreview.m_animationTime = 1f;
			}
		}
		if (m_cameraPreview != null)
		{
			WPFMonoBehaviour.levelManager.m_previewMoveTime = m_cameraPreview.m_animationTime;
		}
		m_cameraBuildZoom = m_cameraPreview.ControlPoints[m_cameraPreview.ControlPoints.Count - 1].zoom;
		base.gameObject.SetActive(value: true);
		m_camera.orthographicSize = m_cameraBuildZoom * INSettings.GetFloat(INFeature.CameraInitialZoom);
	}

	private void OnEnable()
	{
		EventManager.Connect<BirdWakeUpEvent>(ReceiveBirdWakeUpEvent);
		KeyListener.keyPressed += HandleKeyInput;
		KeyListener.keyHold += HandleKeyInput;
	}

	private void OnDisable()
	{
		EventManager.Disconnect<BirdWakeUpEvent>(ReceiveBirdWakeUpEvent);
		KeyListener.keyPressed -= HandleKeyInput;
		KeyListener.keyHold -= HandleKeyInput;
	}

	private void Update()
	{
		UpdatePosition();
		ProcessInput();
		m_keyZoomDelta = 0f;
	}

	private void ReceiveBirdWakeUpEvent(BirdWakeUpEvent data)
	{
		if (!m_freeCameraMode)
		{
			m_autoZoomEnabled = true;
		}
	}

	private IEnumerator EnablePreviewMode()
	{
		m_transitionToPreviewActive = true;
		while ((double)(9f - currentFOV) > 0.1)
		{
			float num = 9f - currentFOV;
			currentFOV += num * GameTime.DeltaTime * 4f;
			m_camera.orthographicSize = currentFOV;
			yield return new WaitForEndOfFrame();
		}
		m_transitionToPreviewActive = false;
	}

	private void StopTransitionToPreview()
	{
		StopCoroutine("EnablePreviewMode");
		m_transitionToPreviewActive = false;
	}

	public void TakeSnapshot(Action handler)
	{
		m_snapshotFX.enabled = true;
		m_snapshotFX.SnapshotFinished += handler;
	}

	private void ProcessInput()
	{
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Completed)
		{
			m_isStartingPan = false;
			isPanning = false;
			return;
		}
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Running)
		{
			if (m_state == CameraState.Wait)
			{
				if (m_transitionTimer > 0f)
				{
					m_transitionTimer -= GameTime.DeltaTime;
				}
				else
				{
					m_state = CameraState.Follow;
				}
			}
			DoPanning();
		}
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.PreviewWhileBuilding || WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.PreviewWhileRunning)
		{
			DoPanning();
		}
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Building)
		{
			if (!DeviceInfo.UsesTouchInput)
			{
				if (Input.GetAxis("Mouse ScrollWheel") + m_keyZoomDelta < 0f && !WPFMonoBehaviour.levelManager.ConstructionUI.IsDragging())
				{
					WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.PreviewWhileBuilding);
					Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.cameraZoomOut);
				}
			}
			else if (Input.touchCount == 2)
			{
				Touch touch = Input.GetTouch(0);
				Touch touch2 = Input.GetTouch(1);
				Vector2 vector = touch.position - touch2.position;
				Vector2 vector2 = touch.position - touch.deltaPosition - (touch2.position - touch2.deltaPosition);
				float num = vector.magnitude - vector2.magnitude;
				num /= (float)Screen.height;
				m_zoomGestureDistance += num;
				if (m_zoomGestureDistance < -0.25f && !WPFMonoBehaviour.levelManager.ConstructionUI.IsDragging())
				{
					WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.PreviewWhileBuilding);
					Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.cameraZoomOut);
				}
			}
			else
			{
				m_zoomGestureDistance = 0f;
			}
		}
		if (WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.PreviewWhileBuilding)
		{
			return;
		}
		if (!DeviceInfo.UsesTouchInput)
		{
			if (currentFOV <= (INSettings.GetBool(INFeature.NewCamera) ? Mathf.Min(m_cameraMinZoom, m_cameraBuildZoom) : 9f) && Input.GetAxis("Mouse ScrollWheel") + m_keyZoomDelta > 0f)
			{
				StopTransitionToPreview();
				WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.Building);
				Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.cameraZoomIn);
			}
		}
		else if (Input.touchCount == 2 && currentFOV <= (INSettings.GetBool(INFeature.NewCamera) ? Mathf.Min(m_cameraMinZoom, m_cameraBuildZoom) : 10f))
		{
			Touch touch3 = Input.GetTouch(0);
			Touch touch4 = Input.GetTouch(1);
			Vector2 vector3 = touch3.position - touch4.position;
			Vector2 vector4 = touch3.position - touch3.deltaPosition - (touch4.position - touch4.deltaPosition);
			float num2 = vector3.magnitude - vector4.magnitude;
			num2 /= (float)Screen.height;
			m_zoomGestureDistance += num2;
			if (m_zoomGestureDistance > 0.25f)
			{
				StopTransitionToPreview();
				WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.Building);
				Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.cameraZoomIn);
			}
		}
		else
		{
			m_zoomGestureDistance = 0f;
		}
	}

	private void UpdatePosition()
	{
		LevelManager.GameState gameState = LevelManager.GameState.Building;
		if ((bool)WPFMonoBehaviour.levelManager)
		{
			gameState = WPFMonoBehaviour.levelManager.gameState;
		}
		currentPos = base.transform.position;
		currentFOV = m_camera.orthographicSize;
		switch (gameState)
		{
		case LevelManager.GameState.Building:
			if (INSettings.GetBool(INFeature.NewCamera) && m_state == CameraState.Follow)
			{
				bool flag = false;
				BasePart.PartType partType = ((SortedPartType)INSettings.GetInt(INFeature.CameraTargetPartType)).ToPartType();
				foreach (BasePart part in WPFMonoBehaviour.levelManager.ContraptionProto.Parts)
				{
					if (part.m_partType == partType)
					{
						flag = true;
						currentPos = part.transform.position;
					}
				}
				if (!flag)
				{
					currentPos = WPFMonoBehaviour.levelManager.StartingPosition;
				}
				m_mouseZoomDelta = 0f;
			}
			if (m_state != CameraState.Building)
			{
				StopTransitionToPreview();
			}
			m_state = CameraState.Building;
			UpdateGameCamera(ref currentPos, ref currentFOV);
			break;
		case LevelManager.GameState.Preview:
			currentPos = m_cameraPreview.ControlPoints[0].position;
			currentFOV = m_cameraPreview.ControlPoints[0].zoom;
			break;
		case LevelManager.GameState.PreviewMoving:
			m_cameraPreview.UpdateCameraPreview(ref currentPos, ref currentFOV);
			if (m_cameraPreview.Done)
			{
				WPFMonoBehaviour.levelManager.CameraPreviewDone();
				currentFOV = m_cameraBuildZoom * INSettings.GetFloat(INFeature.CameraInitialZoom);
			}
			break;
		case LevelManager.GameState.PreviewWhileBuilding:
			if (m_state == CameraState.Building)
			{
				m_state = CameraState.Preview;
				if (INSettings.GetBool(INFeature.NewCamera))
				{
					m_mouseZoomDelta = 0f;
				}
				else
				{
					StartCoroutine(EnablePreviewMode());
				}
			}
			UpdateGameCamera(ref currentPos, ref currentFOV);
			break;
		case LevelManager.GameState.PreviewWhileRunning:
			m_state = CameraState.Preview;
			UpdateGameCamera(ref currentPos, ref currentFOV);
			break;
		case LevelManager.GameState.Running:
			if (!WPFMonoBehaviour.levelManager.ContraptionRunning.m_cameraTarget || GameTime.DeltaTime == 0f)
			{
				return;
			}
			if (m_state != 0)
			{
				m_state = CameraState.Follow;
				m_freeCameraMode = false;
				m_autoZoomAmount = 0f;
				m_autoZoomEnabled = true;
				panPosition = Vector3.zero;
				m_mouseZoomDelta = 0f;
				m_birdsCache = null;
				if (INSettings.GetBool(INFeature.NewCamera))
				{
					Vector3 position = WPFMonoBehaviour.levelManager.ContraptionRunning.m_cameraTarget.transform.position;
					if (Mathf.Abs(currentPos.x - position.x) > (float)Screen.width / (float)Screen.height * currentFOV || Mathf.Abs(currentPos.y - position.y) > currentFOV)
					{
						m_freeCameraMode = true;
					}
					panPosition = currentPos - position;
				}
			}
			UpdateGameCamera(ref currentPos, ref currentFOV);
			break;
		case LevelManager.GameState.Completed:
			UpdateGameCamera(ref currentPos, ref currentFOV);
			break;
		case LevelManager.GameState.ShowingUnlockedParts:
			if (m_state != CameraState.Building)
			{
				StopTransitionToPreview();
			}
			m_state = CameraState.Building;
			UpdateGameCamera(ref currentPos, ref currentFOV);
			break;
		}
		if (INSettings.GetBool(INFeature.NewCamera))
		{
			currentPos.z = -15f;
		}
		else
		{
			EnforceCameraLimits(ref currentPos, ref currentFOV);
		}
		if (!float.IsNaN(currentPos.x) && !float.IsNaN(currentPos.y) && !float.IsNaN(currentPos.z))
		{
			base.transform.position = currentPos;
		}
		m_camera.orthographicSize = currentFOV;
	}

	private void UpdateGameCamera(ref Vector3 currentPos, ref float currentFOV)
	{
		if (INSettings.GetBool(INFeature.NewCamera))
		{
			if (m_state == CameraState.Follow)
			{
				if (WPFMonoBehaviour.levelManager.ContraptionRunning == null)
				{
					return;
				}
				if (!m_freeCameraMode)
				{
					currentPos = WPFMonoBehaviour.levelManager.ContraptionRunning.m_cameraTarget.transform.position + panPosition;
					if (INSettings.GetBool(INFeature.RotatableCamera))
					{
						base.transform.rotation = WPFMonoBehaviour.levelManager.ContraptionRunning.m_cameraTarget.transform.rotation;
					}
				}
				if (isPanning || m_panningSpeed > 10f)
				{
					Vector3 vector = GetPanDelta();
					if (!INSettings.GetBool(INFeature.DisableCameraAnimation) && !isPanning)
					{
						vector = Time.deltaTime * (m_panningSpeed - 10f) * m_panningVelocity.normalized;
						m_panningSpeed *= Mathf.Pow(0.925f, Time.deltaTime / (1f / 60f));
					}
					panPosition += vector;
					currentPos += vector;
				}
				float num = currentFOV;
				currentFOV = Mathf.Clamp(currentFOV + GetZoomDelta(), m_cameraMinZoom, m_cameraMaxZoom);
				if (currentFOV < num)
				{
					Vector3 vector2 = -0.15f * (currentFOV - num) * (WPFMonoBehaviour.ScreenToZ0(Input.mousePosition) - (currentPos - m_camera.velocity * Time.deltaTime));
					vector2.z = 0f;
					panPosition += vector2;
					currentPos += vector2;
				}
				if (!GuiManager.GetPointer().doubleClick || GuiManager.GetPointer().onWidget)
				{
					return;
				}
				bool flag = true;
				Vector3 position = WPFMonoBehaviour.ScreenToZ0(GuiManager.GetPointer().position);
				BasePart.PartType partType = ((SortedPartType)INSettings.GetInt(INFeature.CameraTargetPartType)).ToPartType();
				foreach (BasePart part in WPFMonoBehaviour.levelManager.ContraptionRunning.Parts)
				{
					if (!part.IsInInteractiveRadius(position) || (bool)part.enclosedPart)
					{
						continue;
					}
					if (part.m_partType == partType)
					{
						WPFMonoBehaviour.levelManager.ContraptionRunning.m_cameraTarget = part;
						if (partType == BasePart.PartType.Pig)
						{
							WPFMonoBehaviour.levelManager.ContraptionRunning.m_pig = part;
						}
						panPosition = currentPos - part.transform.position;
						m_freeCameraMode = false;
					}
					flag = false;
				}
				if (flag)
				{
					if (m_freeCameraMode)
					{
						panPosition = Vector3.zero;
					}
					m_freeCameraMode = !m_freeCameraMode;
				}
			}
			else if (m_state == CameraState.Building)
			{
				ConstructionUI constructionUI = WPFMonoBehaviour.levelManager.ConstructionUI;
				if (constructionUI.SelectedElement == -1 && constructionUI.PointerTime.y < 0.4f)
				{
					DoPanning();
				}
				else
				{
					isPanning = false;
					m_isStartingPan = false;
				}
				if (isPanning || m_panningSpeed > 10f)
				{
					Vector3 vector3 = GetPanDelta();
					if (!INSettings.GetBool(INFeature.DisableCameraAnimation) && !isPanning)
					{
						vector3 = Time.deltaTime * (m_panningSpeed - 10f) * m_panningVelocity.normalized;
						m_panningSpeed *= Mathf.Pow(0.925f, Time.deltaTime / (1f / 60f));
					}
					panPosition += vector3;
					currentPos += vector3;
				}
				if (constructionUI.SelectedElement == -1 && constructionUI.PointerTime.y < 0.4f)
				{
					float num2 = currentFOV;
					currentFOV = Mathf.Clamp(currentFOV + GetZoomDelta(), Mathf.Min(m_cameraMinZoom, m_cameraBuildZoom), m_cameraMaxZoom);
					if (currentFOV < num2)
					{
						Vector3 vector4 = -0.15f * (currentFOV - num2) * (WPFMonoBehaviour.ScreenToZ0(Input.mousePosition) - currentPos);
						vector4.z = 0f;
						panPosition += vector4;
						currentPos += vector4;
					}
				}
			}
			else
			{
				if (m_state != CameraState.Preview)
				{
					return;
				}
				if (isPanning || m_panningSpeed > 10f)
				{
					Vector3 vector5 = GetPanDelta();
					if (!INSettings.GetBool(INFeature.DisableCameraAnimation) && !isPanning)
					{
						vector5 = Time.deltaTime * (m_panningSpeed - 10f) * m_panningVelocity.normalized;
						m_panningSpeed *= Mathf.Pow(0.925f, Time.deltaTime / (1f / 60f));
					}
					panPosition += vector5;
					currentPos += vector5;
				}
				float num3 = currentFOV;
				currentFOV = Mathf.Clamp(currentFOV + GetZoomDelta(), Mathf.Min(m_cameraMinZoom, m_cameraBuildZoom), m_cameraMaxZoom);
				if (currentFOV < num3)
				{
					Vector3 vector6 = -0.15f * (currentFOV - num3) * (WPFMonoBehaviour.ScreenToZ0(Input.mousePosition) - currentPos);
					vector6.z = 0f;
					panPosition += vector6;
					currentPos += vector6;
				}
				if (GuiManager.GetPointer().doubleClick && !GuiManager.GetPointer().onWidget)
				{
					WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.Building);
					Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.cameraZoomIn);
				}
			}
		}
		else
		{
			switch (m_state)
			{
			case CameraState.Follow:
				UpdateCameraFollow(ref currentPos, ref currentFOV);
				break;
			case CameraState.Building:
				UpdateCameraBuilding(ref currentPos, ref currentFOV);
				break;
			case CameraState.Preview:
				UpdateCameraPreview(ref currentPos, ref currentFOV);
				break;
			}
		}
	}

	private void UpdateCameraBuilding(ref Vector3 currentPos, ref float currentFOV)
	{
		Vector3 vector = m_cameraPreview.ControlPoints[m_cameraPreview.ControlPoints.Count - 1].position - (Vector2)currentPos;
		currentPos += vector * GameTime.DeltaTime * 4f;
		float num = m_cameraBuildZoom - currentFOV;
		currentFOV += num * GameTime.DeltaTime * 4f;
	}

	private void ClampToCameraLimits(ref Rect box)
	{
		Rect rect = new Rect(m_cameraLimits.topLeft.x, m_cameraLimits.topLeft.y - m_cameraLimits.size.y, m_cameraLimits.size.x, m_cameraLimits.size.y);
		if (box.xMin < rect.xMin)
		{
			box = Rect.MinMaxRect(rect.xMin, box.yMin, Mathf.Max(rect.xMin, box.xMax), box.yMax);
		}
		if (box.yMin < rect.yMin)
		{
			box = Rect.MinMaxRect(box.xMin, rect.yMin, box.xMax, Mathf.Max(rect.yMin, box.yMax));
		}
		if (box.xMax > rect.xMax)
		{
			box = Rect.MinMaxRect(Mathf.Min(box.xMin, rect.xMax), box.yMin, rect.xMax, box.yMax);
		}
		if (box.yMax > rect.yMax)
		{
			box = Rect.MinMaxRect(box.xMin, Mathf.Min(box.yMin, rect.yMax), box.xMax, rect.yMax);
		}
	}

	private void AddBirdsToBoundingBox(ref Rect box)
	{
		if (m_birdsCache == null)
		{
			m_birdsCache = GameObject.FindGameObjectsWithTag("Bird");
		}
		if (m_birdsCache == null)
		{
			return;
		}
		for (int i = 0; i < m_birdsCache.Length; i++)
		{
			GameObject gameObject = m_birdsCache[i];
			if (gameObject.GetComponent<Bird>().IsDisturbed() && ((Mathf.Abs(gameObject.transform.position.y - currentPos.y) < 20f && Mathf.Abs(gameObject.transform.position.x - currentPos.x) < 20f * (float)Screen.width / (float)Screen.height) || gameObject.GetComponent<Bird>().IsAttacking()))
			{
				Vector3 position = gameObject.transform.position;
				Vector2 min = new Vector2(position.x - 8f, position.y - 2f);
				Vector2 max = new Vector2(position.x + 4f, position.y + 2f);
				Contraption.AddToBoundingBox(ref box, min, max);
			}
		}
	}

	private void AutoZoom(ref Vector3 currentPos, ref float currentFOV)
	{
		if (!m_autoZoomEnabled || m_returningToDefaultPosition)
		{
			return;
		}
		Rect rect = WPFMonoBehaviour.levelManager.ContraptionRunning.BoundingBox();
		Rect box = rect;
		bool flag = false;
		AddBirdsToBoundingBox(ref box);
		if (box.width > rect.width || box.height > box.height)
		{
			flag = true;
		}
		ClampToCameraLimits(ref box);
		Vector3 vector = currentPos - new Vector3((float)Screen.width * currentFOV / (float)Screen.height, currentFOV, 0f);
		Vector3 vector2 = currentPos + new Vector3((float)Screen.width * currentFOV / (float)Screen.height, currentFOV, 0f);
		Rect rect2 = default(Rect);
		float num = 0.3f;
		rect2.xMin = vector.x + num;
		rect2.xMax = vector2.x - num;
		rect2.yMin = vector.y + num;
		rect2.yMax = vector2.y - num;
		float a = -10f;
		a = Mathf.Max(a, vector.x - box.xMin);
		a = Mathf.Max(a, vector.y - box.yMin);
		a = Mathf.Max(a, box.xMax - vector2.x);
		a = Mathf.Max(a, box.yMax - vector2.y);
		a = Mathf.Clamp(a, -4f, 2f);
		if (a > 0f)
		{
			float num2 = ((!flag) ? 10f : 4f) * Time.deltaTime * a;
			if (currentFOV + num2 > 20f)
			{
				num2 = 20f - currentFOV;
			}
			currentFOV += num2;
			m_autoZoomAmount += num2;
		}
		else if (m_autoZoomAmount > 0f && a < -0.5f)
		{
			float b = (a + 0.5f) * Time.deltaTime;
			float a2 = (0f - ((!flag) ? 1f : 2f)) * Time.deltaTime * m_autoZoomAmount;
			a2 = Mathf.Max(a2, b);
			if (currentFOV + a2 < m_cameraMinZoom)
			{
				a2 = m_cameraMinZoom - currentFOV;
			}
			currentFOV += a2;
			m_autoZoomAmount += a2;
		}
	}

	private void UpdateCameraFollow(ref Vector3 currentPos, ref float currentFOV)
	{
		if (WPFMonoBehaviour.levelManager.ContraptionRunning == null)
		{
			return;
		}
		AutoZoom(ref currentPos, ref currentFOV);
		float zoomDelta = GetZoomDelta();
		if (currentFOV < m_cameraMinZoom)
		{
			float num = m_cameraMinZoom - currentFOV;
			currentFOV += 3.5f * num * GameTime.DeltaTime;
		}
		Vector3 position = WPFMonoBehaviour.levelManager.ContraptionRunning.m_cameraTarget.transform.position;
		float f = (currentPos.x - position.x) / (currentFOV * ((float)Screen.width / (float)Screen.height));
		float f2 = (currentPos.y - position.y) / currentFOV;
		if ((Mathf.Abs(f) > 1f || Mathf.Abs(f2) > 1f) && !m_returningToDefaultPosition)
		{
			if (isPanning || zoomDelta != 0f)
			{
				m_freeCameraMode = true;
			}
		}
		else if (m_freeCameraMode)
		{
			m_freeCameraMode = false;
			m_returningToDefaultPosition = true;
			m_returnToCenterSpeed = 50f;
		}
		Vector3 vector = ((!m_freeCameraMode) ? (position + Vector3.ClampMagnitude(WPFMonoBehaviour.levelManager.ContraptionRunning.m_cameraTarget.GetComponent<Rigidbody>().velocity * 1.5f, 20f) - (currentPos - panPosition)) : Vector3.zero);
		currentPos += vector * GameTime.DeltaTime;
		if (isPanning || m_panningSpeed > 10f)
		{
			Vector3 vector2 = GetPanDelta();
			if (!isPanning)
			{
				vector2 = Time.deltaTime * (m_panningSpeed - 10f) * m_panningVelocity.normalized;
				m_panningSpeed *= Mathf.Pow(0.925f, Time.deltaTime / (1f / 60f));
			}
			m_autoZoomEnabled = false;
			m_autoZoomAmount = 0f;
			Vector3 newPos = currentPos + vector2;
			EnforceCameraLimits(ref newPos, ref currentFOV);
			Vector3 vector3 = newPos - (currentPos + vector2);
			vector2 += vector3;
			panPosition += vector2;
			currentPos += vector2;
		}
		if (zoomDelta != 0f)
		{
			if (Mathf.Abs(zoomDelta) > 0.01f)
			{
				m_returningToDefaultPosition = false;
				m_autoZoomEnabled = false;
				m_autoZoomAmount = 0f;
			}
			float num2 = currentFOV;
			currentFOV += zoomDelta;
			currentFOV = Mathf.Clamp(currentFOV, m_cameraMinZoom, m_cameraMaxZoom);
			Vector3 newPos2 = currentPos;
			EnforceCameraLimits(ref newPos2, ref currentFOV);
			Vector3 vector4 = newPos2 - currentPos;
			panPosition += vector4;
			float num3 = currentFOV - num2;
			if (num3 < 0f)
			{
				Vector3 vector5 = WPFMonoBehaviour.ScreenToZ0(Input.mousePosition) - currentPos;
				vector5.z = 0f;
				Vector3 vector6 = -0.15f * num3 * vector5;
				Vector3 newPos3 = currentPos + vector6;
				EnforceCameraLimits(ref newPos3, ref currentFOV);
				Vector3 vector7 = newPos3 - (currentPos + vector6);
				vector6 += vector7;
				panPosition += vector6;
				currentPos += vector6;
			}
		}
		if (GuiManager.GetPointer().doubleClick && !GuiManager.GetPointer().onWidget)
		{
			m_returningToDefaultPosition = true;
			if (m_freeCameraMode)
			{
				m_returnToCenterSpeed = 50f;
			}
		}
		else if (GuiManager.TouchCount != 0)
		{
			m_returningToDefaultPosition = false;
		}
		if (m_returningToDefaultPosition)
		{
			if (m_freeCameraMode)
			{
				m_freeCameraMode = false;
			}
			m_autoZoomEnabled = true;
			Vector3 newPos4 = currentPos - panPosition;
			EnforceCameraLimits(ref newPos4, ref currentFOV);
			if (m_returnToCenterSpeed < 200f)
			{
				m_returnToCenterSpeed += 100f * Time.deltaTime;
			}
			Vector3 vector8 = -panPosition;
			if (vector8.sqrMagnitude > 1f)
			{
				vector8 = vector8.normalized;
			}
			Vector3 vector9 = m_returnToCenterSpeed * Time.deltaTime * vector8;
			Vector3 newPos5 = currentPos + vector9;
			EnforceCameraLimits(ref newPos5, ref currentFOV);
			Vector3 vector10 = newPos5 - (currentPos + vector9);
			vector9 += vector10;
			panPosition += vector9;
			currentPos += vector9;
			if (Vector3.Distance(currentPos, newPos4) < 0.01f && Mathf.Abs(m_cameraMinZoom - currentFOV) < 0.01f)
			{
				m_returningToDefaultPosition = false;
			}
			if (currentFOV > m_cameraMinZoom)
			{
				float value = m_cameraMinZoom - currentFOV;
				value = Mathf.Clamp(value, -1f, 1f);
				currentFOV += 0.2f * m_returnToCenterSpeed * value * Time.deltaTime;
			}
		}
		else if (!isPanning && zoomDelta == 0f)
		{
			m_returnToCenterTimer += Time.deltaTime;
			if (m_returnToCenterTimer > 4f)
			{
				if (m_freeCameraMode)
				{
					m_returnToCenterSpeed = 50f;
				}
				m_freeCameraMode = false;
				if (m_returnToCenterSpeed < 80f)
				{
					m_returnToCenterSpeed += 40f * Time.deltaTime;
				}
				Vector3 vector11 = -panPosition;
				if (vector11.sqrMagnitude > 1f)
				{
					vector11 = vector11.normalized;
				}
				Vector3 vector12 = m_returnToCenterSpeed * Time.deltaTime * vector11;
				Vector3 newPos6 = currentPos + vector12;
				EnforceCameraLimits(ref newPos6, ref currentFOV);
				Vector3 vector13 = newPos6 - (currentPos + vector12);
				vector12 += vector13;
				panPosition += vector12;
				currentPos += vector12;
			}
		}
		else
		{
			m_returnToCenterTimer = 0f;
			m_returnToCenterSpeed = 0f;
		}
	}

	private void UpdateCameraPreview(ref Vector3 currentPos, ref float currentFOV)
	{
		Vector3 vector = GetPanDelta();
		if (!isPanning && m_panningSpeed > 10f)
		{
			vector = Time.deltaTime * (m_panningSpeed - 10f) * m_panningVelocity.normalized;
			m_panningSpeed *= Mathf.Pow(0.925f, Time.deltaTime / (1f / 60f));
		}
		currentPos += vector;
		float zoomDelta = GetZoomDelta();
		if (!m_transitionToPreviewActive || zoomDelta > 0f)
		{
			float num = currentFOV;
			currentFOV += zoomDelta;
			float min = ((!m_transitionToPreviewActive) ? 9f : m_cameraBuildZoom);
			currentFOV = Mathf.Clamp(currentFOV, min, m_cameraMaxZoom);
			float num2 = currentFOV - num;
			if (num2 < 0f)
			{
				Vector3 vector2 = WPFMonoBehaviour.ScreenToZ0(Input.mousePosition) - currentPos;
				vector2.z = 0f;
				Vector3 vector3 = -0.15f * num2 * vector2;
				Vector3 newPos = currentPos + vector3;
				EnforceCameraLimits(ref newPos, ref currentFOV);
				Vector3 vector4 = newPos - (currentPos + vector3);
				vector3 += vector4;
				panPosition += vector3;
				currentPos += vector3;
			}
		}
		if (GuiManager.GetPointer().doubleClick && !GuiManager.GetPointer().onWidget)
		{
			WPFMonoBehaviour.levelManager.SetGameState(LevelManager.GameState.Building);
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.cameraZoomIn);
		}
	}

	private Vector3 GetPanDelta()
	{
		if (!isPanning)
		{
			return Vector3.zero;
		}
		if (!DeviceInfo.UsesTouchInput)
		{
			Vector3 vector = WPFMonoBehaviour.ScreenToZ0(m_panStartPosition) - WPFMonoBehaviour.ScreenToZ0(Input.mousePosition);
			m_panStartPosition = Input.mousePosition;
			CalculatePanningSpeed(vector);
			return vector * INSettings.GetFloat(INFeature.CameraPanSpeed);
		}
		if (Input.touchCount != 1)
		{
			return Vector3.zero;
		}
		Touch touch = Input.GetTouch(0);
		if (touch.fingerId != m_panTouchFingerId)
		{
			return Vector3.zero;
		}
		Vector3 vector2 = WPFMonoBehaviour.ScreenToZ0(m_panStartPosition) - WPFMonoBehaviour.ScreenToZ0(touch.position);
		m_panStartPosition = touch.position;
		CalculatePanningSpeed(vector2);
		return vector2 * INSettings.GetFloat(INFeature.CameraPanSpeed);
	}

	private void CalculatePanningSpeed(Vector3 newDelta)
	{
		while (m_panningHistory.Count > 0 && m_panningHistory.Peek().time < Time.time - 0.1f)
		{
			m_panningHistory.Dequeue();
		}
		m_panningHistory.Enqueue(new PanningData(Time.time, newDelta));
		Vector3 zero = Vector3.zero;
		float time = m_panningHistory.Peek().time;
		float num = 0f;
		foreach (PanningData item in m_panningHistory)
		{
			zero += item.delta;
			num = item.time - time;
		}
		if (num > 0f)
		{
			zero /= num;
		}
		m_panningVelocity = zero;
		m_panningSpeed = zero.magnitude;
	}

	public void HandleKeyInput(KeyCode key)
	{
		switch (key)
		{
		case KeyCode.KeypadPlus:
			m_keyZoomDelta += 1f * Time.deltaTime;
			break;
		case KeyCode.KeypadMinus:
			m_keyZoomDelta -= 1f * Time.deltaTime;
			break;
		}
	}

	private float GetZoomDelta()
	{
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Completed)
		{
			return 0f;
		}
		if (DeviceInfo.UsesTouchInput)
		{
			if (Input.touchCount != 2)
			{
				return 0f;
			}
			try
			{
				if (GuiManager.GetPointer(0).touchUsed || GuiManager.GetPointer(1).touchUsed)
				{
					return 0f;
				}
				Touch touch = Input.GetTouch(0);
				Touch touch2 = Input.GetTouch(1);
				Vector2 vector = touch.position - touch2.position;
				Vector2 vector2 = touch.position - touch.deltaPosition - (touch2.position - touch2.deltaPosition);
				return (0f - (vector.magnitude - vector2.magnitude)) * GameTime.DeltaTime * INSettings.GetFloat(INFeature.CameraZoomSpeed);
			}
			catch
			{
				return 0f;
			}
		}
		m_mouseZoomDelta += -16f * (Input.GetAxis("Mouse ScrollWheel") + m_keyZoomDelta);
		float num = 4f * Time.deltaTime * m_mouseZoomDelta;
		if (m_mouseZoomDelta < 0f)
		{
			num = Mathf.Max(num, m_mouseZoomDelta);
		}
		else if (m_mouseZoomDelta > 0f)
		{
			num = Mathf.Min(num, m_mouseZoomDelta);
		}
		m_mouseZoomDelta -= num;
		return num * INSettings.GetFloat(INFeature.CameraZoomSpeed);
	}

	private void EnforceCameraLimits(ref Vector3 newPos, ref float ortoSize)
	{
		float num = (float)Screen.width / (float)Screen.height;
		float a = m_cameraLimits.size.x / 2f / num;
		float b = m_cameraLimits.size.y / 2f;
		float num2 = Mathf.Min(a, b);
		if (ortoSize > num2)
		{
			ortoSize = num2;
		}
		float min = m_cameraLimits.topLeft.x + ortoSize * num;
		float max = m_cameraLimits.topLeft.x + m_cameraLimits.size.x - ortoSize * num;
		float max2 = m_cameraLimits.topLeft.y - m_camera.orthographicSize;
		float min2 = m_cameraLimits.topLeft.y - m_cameraLimits.size.y + ortoSize;
		newPos.x = Mathf.Clamp(newPos.x, min, max);
		newPos.y = Mathf.Clamp(newPos.y, min2, max2);
		newPos.z = -15f;
	}

	private void DoPanning()
	{
		if (DeviceInfo.UsesTouchInput)
		{
			if (Input.touchCount == 1)
			{
				if (!isPanning)
				{
					if (!m_isStartingPan && Input.GetTouch(0).phase == TouchPhase.Began && !GuiManager.GetPointer().onWidget)
					{
						m_panStartPosition = Input.GetTouch(0).position;
						m_panTouchFingerId = Input.GetTouch(0).fingerId;
						m_isStartingPan = true;
					}
					if (m_isStartingPan && Input.GetTouch(0).fingerId == m_panTouchFingerId && Vector3.Distance(Input.GetTouch(0).position, m_panStartPosition) > 30f)
					{
						isPanning = true;
						m_panningSpeed = 0f;
					}
				}
			}
			else
			{
				m_isStartingPan = false;
				isPanning = false;
			}
			return;
		}
		GuiManager.Pointer pointer = GuiManager.GetPointer();
		if (pointer.down && !pointer.onWidget)
		{
			m_panStartPosition = Input.mousePosition;
			m_isStartingPan = true;
		}
		if (pointer.dragging)
		{
			if (m_isStartingPan && Vector3.Distance(Input.mousePosition, m_panStartPosition) > 30f)
			{
				isPanning = true;
				m_isStartingPan = false;
				m_panningSpeed = 0f;
			}
		}
		else
		{
			isPanning = false;
			m_isStartingPan = false;
		}
	}
}
