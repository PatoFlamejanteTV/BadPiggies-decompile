using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTutorial : WPFMonoBehaviour
{
	private enum State
	{
		Initial,
		Waiting,
		Playing,
		Stopped
	}

	public class Pointer
	{
		public delegate void OnPress();

		private GameObject m_pointer;

		private GameObject m_clickIndicator;

		private GameObject m_rotationIcon;

		private float m_pressTimer;

		private bool m_clickIndicatorOn;

		private OnPress onPress;

		public Pointer(GameObject pointer, GameObject clickIndicator, GameObject rotationIcon)
		{
			m_pointer = pointer;
			m_pointer.transform.localScale = 1.05f * Vector3.zero;
			m_clickIndicator = clickIndicator;
			m_rotationIcon = rotationIcon;
		}

		public void SetPressHandler(OnPress onPress)
		{
			this.onPress = onPress;
		}

		public void SetPosition(Vector3 position)
		{
			m_pointer.transform.position = position;
			m_rotationIcon.transform.position = position - 2.75f * Vector3.up + 0.25f * Vector3.right;
		}

		public void Show(bool show)
		{
			m_pointer.SetActive(show);
			m_rotationIcon.SetActive(show);
			if (!show && (bool)m_clickIndicator)
			{
				m_clickIndicator.SetActive(value: false);
			}
		}

		public void Press()
		{
			m_pressTimer = 0f;
			m_clickIndicatorOn = true;
			if ((bool)m_clickIndicator)
			{
				m_clickIndicator.SetActive(value: true);
				m_clickIndicator.transform.position = m_pointer.transform.position;
			}
			m_pointer.transform.localScale = 0.85f * Vector3.one;
			if (onPress != null)
			{
				onPress();
			}
		}

		public void Release()
		{
			m_pointer.transform.localScale = 1.05f * Vector3.one;
		}

		public void Update()
		{
			m_pressTimer += Time.deltaTime;
			if (m_clickIndicatorOn && m_pressTimer >= 0.5f)
			{
				m_clickIndicatorOn = false;
				if ((bool)m_clickIndicator)
				{
					m_clickIndicator.SetActive(value: false);
				}
			}
			m_rotationIcon.transform.Rotate(Vector3.forward, -1f * Time.deltaTime);
		}
	}

	public class PointerTimeLine
	{
		public class PointerEvent
		{
			protected Pointer m_pointer;

			protected bool m_finished;

			public void SetPointer(Pointer pointer)
			{
				m_pointer = pointer;
			}

			public virtual void Start()
			{
			}

			public virtual void Update()
			{
			}

			public virtual bool Finished()
			{
				return m_finished;
			}
		}

		public class Move : PointerEvent
		{
			private float m_time;

			private float m_timer;

			private List<Vector3> m_positions;

			public Move(List<Vector3> positions, float time)
			{
				m_time = time;
				m_positions = positions;
			}

			public override void Start()
			{
				m_timer = 0f;
				m_finished = false;
				if (m_positions.Count > 0)
				{
					m_pointer.SetPosition(m_positions[0]);
				}
				m_pointer.Show(show: true);
			}

			public override void Update()
			{
				m_timer += Time.deltaTime;
				if (m_timer >= m_time)
				{
					m_finished = true;
				}
				float num = MathsUtil.EaseInOutQuad(m_timer, 0f, 1f, m_time);
				num = 0.5f * num + 0.5f * (m_timer / m_time);
				m_pointer.SetPosition(Tutorial.PositionOnSpline(m_positions, num));
			}
		}

		public class Wait : PointerEvent
		{
			private float m_time;

			private float m_timer;

			public Wait(float time)
			{
				m_time = time;
			}

			public override void Start()
			{
				m_finished = false;
				m_timer = 0f;
			}

			public override void Update()
			{
				m_timer += Time.deltaTime;
				if (m_timer > m_time)
				{
					m_finished = true;
				}
			}
		}

		public class Press : PointerEvent
		{
			public override void Start()
			{
				m_pointer.Press();
				m_finished = true;
			}
		}

		public class Release : PointerEvent
		{
			public override void Start()
			{
				m_pointer.Release();
				m_finished = true;
			}
		}

		public class Hide : PointerEvent
		{
			public override void Start()
			{
				m_pointer.Show(show: false);
				m_finished = true;
			}
		}

		private Pointer m_pointer;

		private List<PointerEvent> m_events = new List<PointerEvent>();

		private Vector3 m_position;

		private int m_eventIndex;

		private bool m_finished;

		public PointerTimeLine(Pointer pointer)
		{
			m_pointer = pointer;
		}

		public bool IsFinished()
		{
			return m_finished;
		}

		public void Start()
		{
			m_pointer.Release();
			m_eventIndex = 0;
			if (m_eventIndex < m_events.Count)
			{
				m_events[m_eventIndex].Start();
			}
			m_finished = false;
		}

		public void Update()
		{
			if (m_eventIndex < m_events.Count)
			{
				PointerEvent pointerEvent = m_events[m_eventIndex];
				pointerEvent.Update();
				if (pointerEvent.Finished())
				{
					m_eventIndex++;
					if (m_eventIndex < m_events.Count)
					{
						m_events[m_eventIndex].Start();
					}
					else
					{
						m_finished = true;
					}
				}
			}
			m_pointer.Update();
		}

		public void AddEvent(PointerEvent e)
		{
			e.SetPointer(m_pointer);
			m_events.Add(e);
		}
	}

	public GameObject m_pointerPrefab;

	public GameObject m_clickIndicatorPrefab;

	public GameObject m_rotationIconPrefab;

	private GameObject m_pointerVisual;

	private GameObject m_clickIndicator;

	private GameObject m_rotationIcon;

	private ConstructionUI m_constructionUI;

	private PartSelector m_partselector;

	private Pointer m_pointer;

	private State m_state;

	private BasePart m_targetPart;

	[SerializeField]
	private BasePart.GridRotation m_originalTargetRotation;

	private Vector3 m_originalTargetPosition;

	private PointerTimeLine m_timeline;

	private float m_startTimer = 1f;

	private const string m_rotationTutorialKey = "_Rotation_Tutorial_Completed";

	private List<BasePart.PartType> m_acceptablePartsList;

	private void Start()
	{
		if (GameProgress.GetBool(Singleton<GameManager>.Instance.CurrentSceneName + "_Rotation_Tutorial_Completed"))
		{
			m_state = State.Stopped;
			return;
		}
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChanged);
		m_pointerVisual = Object.Instantiate(m_pointerPrefab);
		m_clickIndicator = Object.Instantiate(m_clickIndicatorPrefab);
		m_rotationIcon = Object.Instantiate(m_rotationIconPrefab);
		m_clickIndicator.SetActive(value: false);
		SetRenderQueue(m_pointerVisual, 3002);
		SetRenderQueue(m_clickIndicator, 3002);
		SetRenderQueue(m_rotationIcon, 3002);
		m_pointer = new Pointer(m_pointerVisual, m_clickIndicator, m_rotationIcon);
		m_acceptablePartsList = new List<BasePart.PartType>();
		m_acceptablePartsList.Add(BasePart.PartType.CokeBottle);
		m_acceptablePartsList.Add(BasePart.PartType.SpringBoxingGlove);
		m_acceptablePartsList.Add(BasePart.PartType.GrapplingHook);
	}

	private void SetRenderQueue(GameObject parent, int queue)
	{
		if ((bool)parent.GetComponent<Renderer>() && (bool)parent.GetComponent<Renderer>().sharedMaterial)
		{
			parent.GetComponent<Renderer>().material.renderQueue = queue;
		}
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			SetRenderQueue(parent.transform.GetChild(i).gameObject, queue);
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(ReceiveGameStateChanged);
	}

	public static Vector3 PositionOnSpline(List<Vector3> controlPoints, float t)
	{
		int count = controlPoints.Count;
		int num = Mathf.FloorToInt(t * (float)(count - 1));
		Vector3 a = controlPoints[Mathf.Clamp(num - 1, 0, count - 1)];
		Vector3 b = controlPoints[Mathf.Clamp(num, 0, count - 1)];
		Vector3 c = controlPoints[Mathf.Clamp(num + 1, 0, count - 1)];
		Vector3 d = controlPoints[Mathf.Clamp(num + 2, 0, count - 1)];
		float i = t * (float)(count - 1) - (float)num;
		return MathsUtil.CatmullRomInterpolate(a, b, c, d, i);
	}

	private void Update()
	{
		if (m_state == State.Waiting)
		{
			foreach (BasePart.PartType acceptableParts in m_acceptablePartsList)
			{
				if (WPFMonoBehaviour.levelManager.ContraptionProto.HasPart(acceptableParts))
				{
					m_startTimer -= Time.deltaTime;
					if (m_startTimer <= 0f)
					{
						SetupTutorial();
					}
				}
			}
			return;
		}
		if (m_state != State.Playing)
		{
			return;
		}
		if ((bool)m_targetPart)
		{
			m_timeline.Update();
			if (m_timeline.IsFinished())
			{
				m_timeline.Start();
			}
			if (m_targetPart.transform.position != m_originalTargetPosition)
			{
				SetupTutorial();
			}
			if (m_targetPart.m_gridRotation == m_originalTargetRotation)
			{
				m_originalTargetRotation = m_targetPart.m_gridRotation;
				m_pointer.Show(show: false);
				m_startTimer = 1f;
				m_state = State.Stopped;
				GameProgress.SetBool(Singleton<GameManager>.Instance.CurrentSceneName + "_Rotation_Tutorial_Completed", value: true);
			}
			return;
		}
		foreach (BasePart.PartType acceptableParts2 in m_acceptablePartsList)
		{
			if (WPFMonoBehaviour.levelManager.ContraptionProto.HasPart(acceptableParts2))
			{
				m_startTimer -= Time.deltaTime;
				if (m_startTimer <= 0f)
				{
					SetupTutorial();
				}
			}
			else
			{
				m_pointer.Show(show: false);
				m_startTimer = 1f;
			}
		}
	}

	private void SetupTutorial()
	{
		foreach (BasePart.PartType acceptableParts in m_acceptablePartsList)
		{
			m_targetPart = WPFMonoBehaviour.levelManager.ContraptionProto.FindPart(acceptableParts);
			if (m_targetPart != null)
			{
				break;
			}
		}
		if (m_targetPart == null)
		{
			throw new MissingReferenceException("Could not find a proper part for rotation tutorial.");
		}
		m_originalTargetPosition = m_targetPart.transform.position;
		m_state = State.Playing;
		m_timeline = new PointerTimeLine(m_pointer);
		Vector3 position = WPFMonoBehaviour.ingameCamera.GetComponent<Camera>().WorldToScreenPoint(m_targetPart.transform.position + 0.3f * Vector3.down);
		Vector3 item = WPFMonoBehaviour.hudCamera.GetComponent<Camera>().ScreenToWorldPoint(position);
		List<Vector3> list = new List<Vector3>();
		list.Add(item);
		m_timeline.AddEvent(new PointerTimeLine.Move(list, 0.5f));
		m_timeline.AddEvent(new PointerTimeLine.Press());
		m_timeline.AddEvent(new PointerTimeLine.Wait(0.5f));
		m_timeline.Start();
	}

	private void ReceiveGameStateChanged(GameStateChanged data)
	{
		if (m_state != State.Stopped)
		{
			if (data.state == LevelManager.GameState.Building)
			{
				StartCoroutine(StartTutorial());
				return;
			}
			m_state = State.Initial;
			m_pointer.Show(show: false);
		}
	}

	private IEnumerator StartTutorial()
	{
		Vector3 cameraPosition;
		do
		{
			cameraPosition = WPFMonoBehaviour.ingameCamera.transform.position;
			yield return new WaitForSeconds(0.2f);
		}
		while (Vector3.Distance(WPFMonoBehaviour.ingameCamera.transform.position, cameraPosition) >= 0.05f);
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Building)
		{
			m_state = State.Waiting;
			m_startTimer = 1f;
		}
	}
}
