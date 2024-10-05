using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompactEpisodeSelector : MonoBehaviour
{
	[SerializeField]
	private GameData gameData;

	[SerializeField]
	private List<GameObject> m_preButtons = new List<GameObject>();

	[SerializeField]
	private List<GameObject> m_episodes = new List<GameObject>();

	[SerializeField]
	private List<GameObject> m_episodesToggled = new List<GameObject>();

	[SerializeField]
	private float Gap;

	[SerializeField]
	private float MinGap = 6f;

	[SerializeField]
	private float MaxGap = 6f;

	[SerializeField]
	private float EdgeMargin = 0.65f;

	[SerializeField]
	private float m_scrollButtonMargin = 0.5f;

	[SerializeField]
	private int m_momentumSlide = 10;

	[SerializeField]
	private float m_limitMargin = 3f;

	[SerializeField]
	private AnimationCurve scalingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float[] m_preButtonsOffsets;

	private int m_episodesPerPage;

	private int m_screenWidth;

	private int m_screenHeight;

	private Camera m_hudCamera;

	private bool m_interacting;

	private Vector2 m_initialInputPos;

	private Vector2 m_lastInputPos;

	private float m_leftLimit;

	private float m_rightLimit;

	private GameObject m_scrollPivot;

	private float m_scrollPivotWidth;

	private Vector2 m_episodeSize;

	private float m_deltaX;

	private bool m_moveToCenter;

	private bool m_movableEpisodes = true;

	[SerializeField]
	private GameObject m_centerEpisode;

	private bool m_focusIsReset;

	public static string CurrentEpisodeKey = "CurrentEpisode";

	public static string CurrentSandboxEpisodeKey = "CurrentSandboxEpisodeIndex";

	public static string IsEpisodeToggledKey = "IsEpisodeRotatorToggled";

	private static CompactEpisodeSelector instance;

	private bool m_isRotated;

	private bool m_isRotating;

	private float m_currentRotation = -1f;

	private bool isInitialized;

	private Animation m_buttonToggleAnimation;

	private float xVelocity;

	private float xMin;

	private float xMax = 60f;

	public bool IsRotated => m_isRotated;

	public bool IsRotating => m_isRotating;

	public List<GameObject> Episodes => m_episodes;

	public static CompactEpisodeSelector Instance => instance;

	private List<GameObject> CurrentEpisodes
	{
		get
		{
			if (m_isRotated)
			{
				return m_episodesToggled;
			}
			return m_episodes;
		}
	}

	private int EpisodeCount
	{
		get
		{
			if (m_isRotated)
			{
				return m_episodesToggled.Count;
			}
			return m_episodes.Count;
		}
	}

	private float HorizontalGap
	{
		get
		{
			float num = 2f * m_hudCamera.orthographicSize * (float)Screen.width / (float)Screen.height;
			float num2 = EdgeMargin + m_episodeSize.x / 2f + m_scrollButtonMargin;
			float num3 = num - 2f * num2;
			float num4 = num3 / (float)(EpisodeCount - 1);
			if (num4 < MinGap)
			{
				MinGap = 6.5f;
				EdgeMargin = 2f;
				num2 = EdgeMargin + m_episodeSize.x / 2f + m_scrollButtonMargin;
				num3 = num - 2f * num2;
				m_episodesPerPage = (int)(num3 / MinGap) + 1;
				if (m_episodesPerPage < EpisodeCount && gameData != null && m_episodesPerPage > gameData.m_episodeLevels.Count)
				{
					m_episodesPerPage = gameData.m_episodeLevels.Count;
				}
				num4 = MinGap;
			}
			else if (num4 > MaxGap)
			{
				num4 = MaxGap;
			}
			return num4;
		}
	}

	private float ScrollPivotWidth => HorizontalGap * (float)EpisodeCount - m_episodeSize.x / 2f;

	private void Awake()
	{
		m_leftLimit = 0f;
		m_rightLimit = 0f;
		instance = this;
		Singleton<GameManager>.Instance.CreateMenuBackground();
		m_hudCamera = GameObject.FindGameObjectWithTag("HUDCamera").GetComponent<Camera>();
		m_scrollPivot = base.transform.Find("ScrollPivot").gameObject;
		SetCenterEpisode(null);
		for (int i = 0; i < m_episodes.Count; i++)
		{
			m_episodes[i] = Object.Instantiate(m_episodes[i]);
			m_episodes[i].transform.parent = m_scrollPivot.transform;
			CompactEpisodeTarget component = m_episodes[i].GetComponent<CompactEpisodeTarget>();
			if (component != null)
			{
				component.episodeSelector = this;
			}
		}
		for (int j = 0; j < m_episodesToggled.Count; j++)
		{
			m_episodesToggled[j] = Object.Instantiate(m_episodesToggled[j]);
			m_episodesToggled[j].transform.parent = m_scrollPivot.transform;
			m_episodesToggled[j].SetActive(value: false);
			CompactEpisodeTarget component2 = m_episodesToggled[j].GetComponent<CompactEpisodeTarget>();
			if (component2 != null)
			{
				component2.episodeSelector = this;
			}
		}
		if (m_preButtons != null && m_preButtons.Count > 0)
		{
			for (int k = 0; k < m_preButtons.Count; k++)
			{
				m_preButtons[k] = Object.Instantiate(m_preButtons[k]);
				m_preButtons[k].transform.parent = m_scrollPivot.transform;
				CompactEpisodeTarget component3 = m_preButtons[k].GetComponent<CompactEpisodeTarget>();
				if (component3 != null)
				{
					component3.episodeSelector = this;
				}
			}
		}
		m_screenWidth = Screen.width;
		m_screenHeight = Screen.height;
		isInitialized = true;
		Layout();
	}

	private void Start()
	{
		if (GameProgress.GetBool("show_content_limit_popup"))
		{
			GameProgress.DeleteKey("show_content_limit_popup");
			LevelInfo.DisplayContentLimitNotification();
		}
		Transform transform = base.transform.Find("RightButtons/SandboxToggle/SandBox_Toggle(Clone)");
		if (transform != null)
		{
			m_buttonToggleAnimation = transform.GetComponent<Animation>();
		}
		if (m_buttonToggleAnimation != null)
		{
			AnimationClip clip = m_buttonToggleAnimation.GetClip("Sandbox_Toggle");
			if (clip != null)
			{
				m_buttonToggleAnimation.clip = clip;
				AnimationState animationState = m_buttonToggleAnimation["Sandbox_Toggle"];
				animationState.enabled = true;
				animationState.speed = 0f;
				animationState.normalizedTime = ((!m_isRotated) ? 0.5f : 1f);
				m_buttonToggleAnimation.Play();
				m_buttonToggleAnimation.Sample();
				m_buttonToggleAnimation.Stop();
			}
		}
	}

	private void OnEnable()
	{
		KeyListener.keyReleased += HandleKeyListenerkeyReleased;
		KeyListener.mouseWheel += HandleKeyListenerMouseWheel;
	}

	private void OnDisable()
	{
		KeyListener.keyReleased -= HandleKeyListenerkeyReleased;
		KeyListener.mouseWheel -= HandleKeyListenerMouseWheel;
	}

	private void Update()
	{
		if (m_screenWidth != Screen.width || m_screenHeight != Screen.height)
		{
			m_screenWidth = Screen.width;
			m_screenHeight = Screen.height;
			Layout();
		}
		GuiManager.Pointer pointer = GuiManager.GetPointer();
		if (pointer.down)
		{
			m_initialInputPos = pointer.position;
			m_lastInputPos = pointer.position;
			m_interacting = true;
			if (Physics.Raycast(m_hudCamera.ScreenPointToRay(pointer.position), out var hitInfo))
			{
				if (hitInfo.collider.transform.name.Equals("SliderToggle"))
				{
					m_interacting = false;
				}
				Transform parent = hitInfo.collider.transform.parent;
				CompactEpisodeTarget component = hitInfo.collider.transform.GetComponent<CompactEpisodeTarget>();
				if ((bool)component || (hitInfo.collider.transform.GetComponent<Button>() == null && (bool)parent && (bool)parent.GetComponent<CompactEpisodeTarget>()))
				{
					if ((bool)component)
					{
						SetCenterEpisode(hitInfo.collider.transform.gameObject);
					}
					else
					{
						SetCenterEpisode(parent.gameObject);
					}
					if (m_centerEpisode == CurrentEpisodes[0] && m_preButtons != null)
					{
						SetCenterEpisode(m_preButtons[0]);
					}
					else if (m_centerEpisode == CurrentEpisodes[EpisodeCount - 1])
					{
						SetCenterEpisode(CurrentEpisodes[EpisodeCount - 2]);
					}
					if (m_isRotated)
					{
						for (int i = 0; i < m_episodesToggled.Count; i++)
						{
							if (m_centerEpisode == m_episodesToggled[i])
							{
								int value = Mathf.Clamp(i, 0, m_episodesToggled.Count - 1);
								UserSettings.SetInt(CurrentSandboxEpisodeKey, value);
							}
						}
					}
				}
			}
		}
		bool flag = TextDialog.DialogOpen || DailyChallengeDialog.DialogOpen || SnoutCoinShopPopup.DialogOpen || LootWheelPopup.DialogOpen;
		if (pointer.dragging && m_interacting && !flag && m_movableEpisodes)
		{
			Vector3 vector = m_hudCamera.ScreenToWorldPoint(m_lastInputPos);
			Vector3 vector2 = m_hudCamera.ScreenToWorldPoint(pointer.position);
			m_lastInputPos = pointer.position;
			m_deltaX = vector2.x - vector.x;
			if (Mathf.Abs(m_deltaX) > 0f)
			{
				MoveEpisodes(m_deltaX);
			}
			Vector3 vector3 = m_hudCamera.ScreenToWorldPoint(m_initialInputPos);
			if (!m_focusIsReset && Mathf.Abs(vector2.x - vector3.x) > 0.1f)
			{
				m_focusIsReset = true;
			}
			if (m_focusIsReset)
			{
				Singleton<GuiManager>.Instance.ResetFocus();
			}
		}
		if (pointer.up && m_interacting)
		{
			m_focusIsReset = false;
			m_interacting = false;
			m_moveToCenter = true;
		}
		if (!m_interacting && m_movableEpisodes && m_momentumSlide > 0 && (bool)m_centerEpisode)
		{
			float x = m_centerEpisode.transform.position.x;
			if (Mathf.Abs(m_deltaX) > 0.1f)
			{
				MoveEpisodes(m_deltaX);
				m_deltaX -= m_deltaX / (float)m_momentumSlide;
			}
			else if (m_centerEpisode != null && m_moveToCenter)
			{
				float num = Mathf.SmoothDamp(x, 0f, ref xVelocity, 0.2f);
				MoveEpisodes(num - x, checkCenter: false);
			}
		}
		ScaleEpisodes();
	}

	private void SetCenterEpisode(GameObject target)
	{
		if (m_preButtons != null && m_preButtons.Count > 0 && target == m_preButtons[0])
		{
			m_centerEpisode = ((!m_isRotated) ? m_preButtons[0] : m_episodesToggled[0]);
		}
		else
		{
			m_centerEpisode = target;
		}
	}

	public void MoveToTarget(Transform newTarget)
	{
		SetCenterEpisode(newTarget.gameObject);
		m_moveToCenter = true;
		m_interacting = false;
	}

	public void MoveToTargetIndex(int index, bool isSandbox = false)
	{
		if (isSandbox)
		{
			index = Mathf.Clamp(index, 0, m_episodesToggled.Count - 1);
			UserSettings.SetInt(CurrentSandboxEpisodeKey, index);
			if (index == 0 && m_preButtons != null)
			{
				SetCenterEpisode(m_preButtons[0]);
			}
			else
			{
				SetCenterEpisode(m_episodesToggled[index]);
			}
		}
		else
		{
			index = Mathf.Clamp(index, 0, m_episodes.Count - 1);
			UserSettings.SetString(CurrentEpisodeKey, m_episodes[index].name);
			if (index == 0 && m_preButtons != null)
			{
				SetCenterEpisode(m_preButtons[0]);
			}
			else
			{
				SetCenterEpisode(m_episodes[index]);
			}
		}
		m_moveToCenter = true;
		m_interacting = false;
	}

	private void MoveEpisodes(float delta, bool checkCenter = true)
	{
		float num = ((m_preButtons == null || m_preButtons.Count <= 1) ? 0f : 2f);
		if (m_scrollPivot.transform.localPosition.x > num + m_limitMargin && delta > 0f)
		{
			if (m_preButtons != null && m_preButtons.Count > 1)
			{
				SetCenterEpisode(m_preButtons[0]);
			}
			else
			{
				SetCenterEpisode(CurrentEpisodes[0]);
			}
			m_deltaX = 0f;
		}
		else if (m_scrollPivot.transform.localPosition.x < 0f - ScrollPivotWidth - m_limitMargin && delta < 0f)
		{
			SetCenterEpisode(CurrentEpisodes[EpisodeCount - 2]);
			m_deltaX = 0f;
		}
		else
		{
			m_scrollPivot.transform.localPosition = new Vector3(m_scrollPivot.transform.localPosition.x + delta, m_scrollPivot.transform.localPosition.y, m_scrollPivot.transform.localPosition.z);
		}
		if (!checkCenter)
		{
			return;
		}
		for (int i = 0; i < EpisodeCount; i++)
		{
			float num2 = Mathf.Abs(CurrentEpisodes[i].transform.position.x);
			if (m_centerEpisode == null || num2 < Mathf.Abs(m_centerEpisode.transform.position.x))
			{
				int num3 = Mathf.Clamp(i, 0, EpisodeCount - 2);
				if (num3 == 0 && m_preButtons != null)
				{
					SetCenterEpisode(m_preButtons[0]);
				}
				else
				{
					SetCenterEpisode(CurrentEpisodes[num3]);
				}
			}
		}
	}

	private void ScaleEpisodes()
	{
		for (int i = 0; i < m_episodes.Count; i++)
		{
			GameObject gameObject = m_episodes[i];
			float time = 1f / (xMax - xMin) * (Mathf.Abs(gameObject.transform.position.x) - xMin);
			float num = Mathf.Clamp01(scalingCurve.Evaluate(time));
			if (float.IsNaN(num))
			{
				num = 0f;
			}
			gameObject.GetComponent<Collider>().enabled = num >= 0.98f;
			gameObject.transform.localScale = new Vector3(num, num, 1f);
		}
		for (int j = 0; j < m_episodesToggled.Count; j++)
		{
			GameObject gameObject2 = m_episodesToggled[j];
			float time2 = 1f / (xMax - xMin) * (Mathf.Abs(gameObject2.transform.position.x) - xMin);
			float num2 = Mathf.Clamp01(scalingCurve.Evaluate(time2));
			if (float.IsNaN(num2))
			{
				num2 = 0f;
			}
			gameObject2.GetComponent<Collider>().enabled = num2 >= 0.98f;
			gameObject2.transform.localScale = new Vector3(num2, num2, 1f);
		}
		if (m_preButtons == null || m_preButtons.Count <= 0)
		{
			return;
		}
		for (int k = 0; k < m_preButtons.Count; k++)
		{
			float time3 = 1f / (xMax - xMin) * (Mathf.Abs(m_preButtons[k].transform.position.x) - xMin);
			float num3 = Mathf.Clamp01(scalingCurve.Evaluate(time3));
			if (float.IsNaN(num3))
			{
				num3 = 0f;
			}
			m_preButtons[k].transform.localScale = new Vector3(num3, num3, 1f);
		}
	}

	[ContextMenu("Layout")]
	private void Layout()
	{
		float horizontalGap = HorizontalGap;
		if (m_episodes[0].GetComponent<Sprite>() == null)
		{
			UnmanagedSprite component = m_episodes[0].GetComponent<UnmanagedSprite>();
			if (component != null)
			{
				m_episodeSize = component.Size;
			}
		}
		else
		{
			m_episodeSize = m_episodes[0].GetComponent<Sprite>().Size;
		}
		for (int i = 0; i < m_episodes.Count; i++)
		{
			m_episodes[i].transform.localPosition = Vector3.right * m_episodeSize.x / 2f + Vector3.right * horizontalGap * i;
		}
		for (int j = 0; j < m_episodesToggled.Count; j++)
		{
			m_episodesToggled[j].transform.localPosition = Vector3.right * m_episodeSize.x / 2f + Vector3.right * horizontalGap * j;
		}
		if (m_preButtons != null && m_preButtons.Count > 0)
		{
			for (int k = 0; k < m_preButtons.Count; k++)
			{
				m_preButtons[k].transform.localPosition = Vector3.left * m_preButtonsOffsets[k] * (k + 1);
			}
		}
		bool @bool = UserSettings.GetBool(IsEpisodeToggledKey);
		if (@bool)
		{
			int @int = UserSettings.GetInt(CurrentSandboxEpisodeKey, 1);
			if (Mathf.Clamp(@int, 0, m_episodesToggled.Count - 1) == 0 && m_preButtons != null)
			{
				SetCenterEpisode(m_preButtons[0]);
			}
			else
			{
				SetCenterEpisode(m_episodesToggled[Mathf.Clamp(@int, 0, m_episodesToggled.Count - 1)]);
			}
		}
		else if (m_preButtons != null && m_preButtons.Count > 1)
		{
			SetCenterEpisode(m_preButtons[0]);
		}
		else
		{
			SetCenterEpisode(m_episodes[0]);
		}
		if (m_centerEpisode != null)
		{
			int num = 0;
			if (@bool)
			{
				for (int l = 0; l < m_episodesToggled.Count; l++)
				{
					if (m_episodesToggled[l] == m_centerEpisode)
					{
						num = Mathf.Clamp(l, 0, m_episodesToggled.Count - 1);
					}
				}
			}
			else
			{
				for (int m = 0; m < m_episodes.Count; m++)
				{
					if (m_episodes[m] == m_centerEpisode)
					{
						num = Mathf.Clamp(m, 0, m_episodes.Count - 1);
					}
				}
			}
			m_scrollPivot.transform.localPosition = -Vector3.right * m_episodeSize.x / 2f - Vector3.right * horizontalGap * num + Vector3.up * m_scrollPivot.transform.localPosition.y;
		}
		MoveEpisodes(0f, checkCenter: false);
		m_moveToCenter = true;
		if (@bool)
		{
			SetRotation(1f);
		}
	}

	private bool isInInteractiveArea(Vector2 touchPos)
	{
		if (touchPos.y > (float)Screen.height * 0.1f)
		{
			return touchPos.y < (float)Screen.height * 0.8f;
		}
		return false;
	}

	public void GoToMainMenu()
	{
		SendExitEpisodeSelectionFlurryEvent();
		Singleton<GameManager>.Instance.LoadMainMenu(showLoadingScreen: false);
	}

	private void HandleKeyListenerkeyReleased(KeyCode obj)
	{
		if (obj == KeyCode.Escape)
		{
			GoToMainMenu();
		}
	}

	private void HandleKeyListenerMouseWheel(float delta)
	{
	}

	public void PrepareRotation()
	{
		_ = m_isRotating;
	}

	public void ReleaseRotation(bool toggleRotation)
	{
		if (!m_isRotating)
		{
			StartCoroutine(RotateSequence(toggleRotation));
		}
	}

	public void SetRotation(float rotation)
	{
		if (!isInitialized)
		{
			return;
		}
		m_currentRotation = rotation;
		bool isRotated = m_isRotated;
		m_isRotated = m_currentRotation >= 0f;
		if (isRotated != m_isRotated)
		{
			OnRotated(m_isRotated);
			UserSettings.SetBool(IsEpisodeToggledKey, m_isRotated);
		}
		float t = ((!m_isRotated) ? (m_currentRotation + 1f) : (1f - m_currentRotation));
		Vector3 vector = ((!m_isRotated) ? Vector3.zero : (Vector3.up * 90f));
		Vector3 vector2 = ((!m_isRotated) ? (Vector3.up * 90f) : Vector3.zero);
		for (int i = 0; i < m_episodes.Count; i++)
		{
			m_episodes[i].transform.localEulerAngles = Vector3.Lerp(vector, vector2, t);
			if (m_episodes[i].activeInHierarchy && m_isRotated)
			{
				m_episodes[i].SetActive(value: false);
			}
			else if (!m_episodes[i].activeInHierarchy && !m_isRotated)
			{
				m_episodes[i].SetActive(value: true);
			}
		}
		for (int j = 0; j < m_episodesToggled.Count; j++)
		{
			m_episodesToggled[j].transform.localEulerAngles = Vector3.Lerp(vector2, vector, t);
			if (!m_episodesToggled[j].activeInHierarchy && m_isRotated)
			{
				m_episodesToggled[j].SetActive(value: true);
			}
			else if (m_episodesToggled[j].activeInHierarchy && !m_isRotated)
			{
				m_episodesToggled[j].SetActive(value: false);
			}
		}
	}

	private IEnumerator RotateSequence(bool toggleRotation)
	{
		if (m_isRotating)
		{
			yield break;
		}
		m_isRotating = true;
		float targetRotation = ((!m_isRotated) ? (-1f) : 1f);
		float newRotation = m_currentRotation;
		if (toggleRotation)
		{
			targetRotation *= -1f;
		}
		float fromState = ((!m_isRotated) ? 0.5f : 0f);
		float toState = ((!m_isRotated) ? 1f : 0.5f);
		AnimationState toggleState = null;
		if (m_buttonToggleAnimation != null)
		{
			AnimationClip clip = m_buttonToggleAnimation.GetClip("Sandbox_Toggle");
			if (clip != null)
			{
				m_buttonToggleAnimation.clip = clip;
				toggleState = m_buttonToggleAnimation["Sandbox_Toggle"];
				toggleState.enabled = true;
				toggleState.speed = 0f;
				toggleState.normalizedTime = 0f;
				m_buttonToggleAnimation.Play();
			}
		}
		float fade = 0f;
		while (fade < 1f)
		{
			if (m_buttonToggleAnimation != null && toggleState != null)
			{
				toggleState.time = Mathf.Lerp(fromState, toState, fade);
				m_buttonToggleAnimation.Sample();
			}
			newRotation = Mathf.Lerp(newRotation, targetRotation, fade);
			SetRotation(newRotation);
			fade += Time.deltaTime * 5f;
			yield return null;
		}
		if (m_buttonToggleAnimation != null && toggleState != null)
		{
			toggleState.normalizedTime = toState;
			m_buttonToggleAnimation.Sample();
			m_buttonToggleAnimation.Stop();
		}
		SetRotation(targetRotation);
		m_isRotating = false;
	}

	private void OnRotated(bool rotated)
	{
		int num = -1;
		for (int i = 0; i < m_episodes.Count; i++)
		{
			if (m_centerEpisode == m_episodes[i])
			{
				num = i;
			}
		}
		if (num < 0)
		{
			for (int j = 0; j < m_episodesToggled.Count; j++)
			{
				if (m_centerEpisode == m_episodesToggled[j])
				{
					num = j;
				}
			}
		}
		int num2 = Mathf.Clamp(num, 0, EpisodeCount - 2);
		if (num2 == 0 && m_preButtons != null)
		{
			SetCenterEpisode(m_preButtons[0]);
		}
		else
		{
			SetCenterEpisode(CurrentEpisodes[num2]);
		}
	}

	public void SendExitEpisodeSelectionFlurryEvent()
	{
	}

	private void OnDrawGizmos()
	{
		if (m_centerEpisode != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(m_centerEpisode.transform.position, 0.5f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(base.transform.position, 0.5f);
		}
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(new Vector3(m_leftLimit, 0f), 0.5f);
		Gizmos.DrawSphere(new Vector3(m_rightLimit, 0f), 0.5f);
	}
}
