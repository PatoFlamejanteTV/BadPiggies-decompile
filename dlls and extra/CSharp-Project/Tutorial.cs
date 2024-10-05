using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : WPFMonoBehaviour
{
	private enum State
	{
		Initial,
		OpenBook,
		DragPart,
		DragPig,
		StartContraption,
		StartEngine
	}

	public class Pointer
	{
		public delegate void OnPress();

		private GameObject m_pointer;

		private GameObject m_clickIndicator;

		private float m_pressTimer;

		private bool m_clickIndicatorOn;

		private OnPress onPress;

		public Pointer(GameObject pointer, GameObject clickIndicator)
		{
			m_pointer = pointer;
			m_pointer.transform.localScale = 1.05f * Vector3.zero;
			m_clickIndicator = clickIndicator;
		}

		public void SetPressHandler(OnPress onPress)
		{
			this.onPress = onPress;
		}

		public void SetPosition(Vector3 position)
		{
			m_pointer.transform.position = position;
		}

		public void Show(bool show)
		{
			if ((bool)m_pointer)
			{
				m_pointer.SetActive(show);
			}
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
			m_pressTimer += Time.unscaledDeltaTime;
			if (m_clickIndicatorOn && m_pressTimer >= 0.5f)
			{
				m_clickIndicatorOn = false;
				if ((bool)m_clickIndicator)
				{
					m_clickIndicator.SetActive(value: false);
				}
			}
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
				m_timer += Time.unscaledDeltaTime;
				if (m_timer >= m_time)
				{
					m_finished = true;
				}
				float num = MathsUtil.EaseInOutQuad(m_timer, 0f, 1f, m_time);
				num = 0.5f * num + 0.5f * (m_timer / m_time);
				m_pointer.SetPosition(PositionOnSpline(m_positions, num));
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
				m_timer += Time.unscaledDeltaTime;
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

	public int m_dragTargetX;

	public int m_dragTargetY = 1;

	public int m_pigTargetX;

	public int m_pigTargetY = 1;

	private bool m_playing;

	private bool m_finished;

	private bool m_setupCompleted;

	private PartSelector m_partselector;

	private ConstructionUI m_constructionUI;

	private GameObject m_pointerVisual;

	private GameObject m_clickIndicator;

	private Pointer m_pointer;

	private PointerTimeLine m_tutorialTimeline;

	private PointerTimeLine m_dragPartTimeline;

	private PointerTimeLine m_dragPigTimeline;

	private PointerTimeLine m_startContraptionTimeline;

	private PointerTimeLine m_startEnginesTimeLine;

	private PointerTimeLine m_timeline;

	private State m_state;

	private bool m_tutorialBookOpened;

	private bool m_contraptionStarted;

	private ToolboxButton m_toolBoxButton;

	private bool m_shopIsOpen;

	private void Start()
	{
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChanged);
		EventManager.Connect<UIEvent>(ReceiveUIEvent);
		m_pointerVisual = Object.Instantiate(m_pointerPrefab);
		m_clickIndicator = Object.Instantiate(m_clickIndicatorPrefab);
		m_clickIndicator.SetActive(value: false);
		m_pointerVisual.GetComponentInChildren<Renderer>().sortingLayerName = "Popup";
		m_clickIndicator.GetComponentInChildren<Renderer>().sortingLayerName = "Popup";
		SetOrderInLayer(m_pointerVisual, 3002);
		SetOrderInLayer(m_clickIndicator, 3002);
		m_pointer = new Pointer(m_pointerVisual, m_clickIndicator);
	}

	public void ReceiveUIEvent(UIEvent data)
	{
		switch (data.type)
		{
		case UIEvent.Type.CloseIapMenu:
			m_shopIsOpen = false;
			break;
		case UIEvent.Type.OpenIapMenu:
			m_shopIsOpen = true;
			break;
		}
	}

	public static void SetRenderQueue(GameObject parent, int queue)
	{
		Renderer component = parent.GetComponent<Renderer>();
		if (component != null && component.sharedMaterial != null)
		{
			component.material.renderQueue = queue;
		}
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			SetRenderQueue(parent.transform.GetChild(i).gameObject, queue);
		}
	}

	public static void SetOrderInLayer(GameObject parent, int order)
	{
		Renderer component = parent.GetComponent<Renderer>();
		if (component != null)
		{
			component.sortingOrder = order;
		}
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			SetOrderInLayer(parent.transform.GetChild(i).gameObject, order);
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(ReceiveGameStateChanged);
		EventManager.Disconnect<UIEvent>(ReceiveUIEvent);
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
		if (m_toolBoxButton == null && GameObject.Find("ToolBoxButton") != null)
		{
			m_toolBoxButton = GameObject.Find("ToolBoxButton").GetComponent<ToolboxButton>();
		}
		if (!m_setupCompleted || m_shopIsOpen)
		{
			return;
		}
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Building)
		{
			if ((m_state == State.OpenBook || m_state == State.DragPart || m_state == State.Initial) && WPFMonoBehaviour.levelManager.ContraptionProto.FindPig() != null && !m_contraptionStarted)
			{
				m_pointer.Show(show: false);
				m_state = State.StartContraption;
				m_playing = true;
				m_timeline = m_startContraptionTimeline;
				m_timeline.Start();
			}
			if (!m_playing && (m_state == State.DragPart || m_state == State.Initial) && WPFMonoBehaviour.levelManager.ContraptionProto.Parts.Count == 0 && (bool)m_constructionUI && !m_constructionUI.IsDragging())
			{
				m_playing = true;
				m_state = State.DragPart;
				m_timeline = m_dragPartTimeline;
				m_timeline.Start();
			}
			if (m_state == State.StartContraption && WPFMonoBehaviour.levelManager.ContraptionProto.FindPig() == null)
			{
				m_state = State.Initial;
				m_playing = false;
				m_pointer.Show(show: false);
			}
			if (m_toolBoxButton != null && m_toolBoxButton.ToolboxOpen)
			{
				m_playing = false;
				m_pointer.Show(show: false);
			}
			if (!m_playing && m_state == State.OpenBook)
			{
				if (!m_tutorialBookOpened)
				{
					m_playing = true;
					m_timeline = m_tutorialTimeline;
					m_timeline.Start();
				}
				else
				{
					m_state = State.Initial;
					m_playing = false;
					m_pointer.Show(show: false);
				}
			}
		}
		if (m_playing)
		{
			m_timeline.Update();
			if (m_timeline.IsFinished())
			{
				m_timeline.Start();
			}
			if ((m_state == State.DragPart || m_state == State.DragPig) && m_constructionUI.IsDragging())
			{
				m_playing = false;
				m_pointer.Show(show: false);
			}
			if (m_state == State.StartEngine && (bool)WPFMonoBehaviour.levelManager.ContraptionRunning && WPFMonoBehaviour.levelManager.ContraptionRunning.SomePoweredPartsEnabled())
			{
				m_playing = false;
				m_pointer.Show(show: false);
			}
		}
	}

	private void SetupTutorial()
	{
		m_playing = true;
		InGameGUI inGameGUI = WPFMonoBehaviour.levelManager.InGameGUI;
		m_partselector = inGameGUI.BuildMenu.PartSelector;
		m_constructionUI = WPFMonoBehaviour.levelManager.ConstructionUI;
		GameObject tutorialButton = inGameGUI.BuildMenu.TutorialButton;
		GameObject playButton = inGameGUI.BuildMenu.PlayButton;
		if (!(m_constructionUI == null))
		{
			m_tutorialTimeline = new PointerTimeLine(m_pointer);
			List<Vector3> list = new List<Vector3>();
			list.Add(tutorialButton.transform.position + 21f * Vector3.down);
			list.Add(tutorialButton.transform.position);
			m_tutorialTimeline.AddEvent(new PointerTimeLine.Wait(0.1f));
			m_tutorialTimeline.AddEvent(new PointerTimeLine.Move(list, 2.5f));
			m_tutorialTimeline.AddEvent(new PointerTimeLine.Press());
			m_tutorialTimeline.AddEvent(new PointerTimeLine.Wait(0.5f));
			m_tutorialTimeline.AddEvent(new PointerTimeLine.Release());
			m_tutorialTimeline.AddEvent(new PointerTimeLine.Wait(0.75f));
			m_tutorialTimeline.AddEvent(new PointerTimeLine.Hide());
			m_dragPartTimeline = new PointerTimeLine(m_pointer);
			ConstructionUI.PartDesc partDesc = m_constructionUI.FindPartDesc(BasePart.PartType.WoodenFrame);
			Vector3 position = m_partselector.FindPartButton(partDesc).transform.position;
			Vector3 vector = m_constructionUI.GridPositionToGuiPosition(m_dragTargetX, m_dragTargetY);
			List<Vector3> list2 = new List<Vector3>();
			list2.Add(position + 3f * Vector3.down + 1f * Vector3.left);
			list2.Add(position);
			m_dragPartTimeline.AddEvent(new PointerTimeLine.Move(list2, 1.5f));
			List<Vector3> list3 = new List<Vector3>();
			list3.Add(position);
			list3.Add(0.5f * (position + vector) + 0.5f * Vector3.left);
			list3.Add(vector);
			m_dragPartTimeline.AddEvent(new PointerTimeLine.Press());
			m_dragPartTimeline.AddEvent(new PointerTimeLine.Wait(0.5f));
			m_dragPartTimeline.AddEvent(new PointerTimeLine.Move(list3, 1.75f));
			m_dragPartTimeline.AddEvent(new PointerTimeLine.Wait(0.2f));
			m_dragPartTimeline.AddEvent(new PointerTimeLine.Release());
			m_dragPartTimeline.AddEvent(new PointerTimeLine.Wait(0.5f));
			m_dragPartTimeline.AddEvent(new PointerTimeLine.Hide());
			m_dragPartTimeline.AddEvent(new PointerTimeLine.Wait(2f));
			ConstructionUI.PartDesc partDesc2 = m_constructionUI.FindPartDesc(BasePart.PartType.Pig);
			Vector3 position2 = m_partselector.FindPartButton(partDesc2).transform.position;
			Vector3 vector2 = m_constructionUI.GridPositionToGuiPosition(m_pigTargetX, m_pigTargetY);
			m_dragPigTimeline = new PointerTimeLine(m_pointer);
			List<Vector3> list4 = new List<Vector3>();
			list4.Add(position2 + 3f * Vector3.down + 1f * Vector3.left);
			list4.Add(position2);
			m_dragPigTimeline.AddEvent(new PointerTimeLine.Move(list4, 1.5f));
			List<Vector3> list5 = new List<Vector3>();
			list5.Add(position2);
			list5.Add(0.5f * (position2 + vector2) + 0.5f * Vector3.left);
			list5.Add(vector2);
			m_dragPigTimeline.AddEvent(new PointerTimeLine.Press());
			m_dragPigTimeline.AddEvent(new PointerTimeLine.Wait(0.5f));
			m_dragPigTimeline.AddEvent(new PointerTimeLine.Move(list5, 1.75f));
			m_dragPigTimeline.AddEvent(new PointerTimeLine.Wait(0.2f));
			m_dragPigTimeline.AddEvent(new PointerTimeLine.Release());
			m_dragPigTimeline.AddEvent(new PointerTimeLine.Wait(0.5f));
			m_dragPigTimeline.AddEvent(new PointerTimeLine.Hide());
			m_dragPigTimeline.AddEvent(new PointerTimeLine.Wait(2f));
			m_startContraptionTimeline = new PointerTimeLine(m_pointer);
			List<Vector3> list6 = new List<Vector3>();
			list6.Add(playButton.transform.position + 11f * Vector3.down);
			list6.Add(playButton.transform.position + 5.5f * Vector3.down + 0.5f * Vector3.left);
			list6.Add(playButton.transform.position);
			m_startContraptionTimeline.AddEvent(new PointerTimeLine.Wait(1f));
			m_startContraptionTimeline.AddEvent(new PointerTimeLine.Move(list6, 2f));
			m_startContraptionTimeline.AddEvent(new PointerTimeLine.Press());
			m_startContraptionTimeline.AddEvent(new PointerTimeLine.Wait(0.5f));
			m_startContraptionTimeline.AddEvent(new PointerTimeLine.Release());
			m_startContraptionTimeline.AddEvent(new PointerTimeLine.Wait(0.75f));
			m_startContraptionTimeline.AddEvent(new PointerTimeLine.Hide());
			if (!m_tutorialBookOpened)
			{
				m_state = State.OpenBook;
				m_timeline = m_tutorialTimeline;
				m_timeline.Start();
			}
			else if (WPFMonoBehaviour.levelManager.ContraptionProto.Parts.Count == 0)
			{
				m_state = State.DragPart;
				m_timeline = m_dragPartTimeline;
				m_timeline.Start();
			}
			else if (!m_contraptionStarted)
			{
				m_state = State.StartContraption;
				m_timeline = m_startContraptionTimeline;
				m_timeline.Start();
			}
			else
			{
				m_playing = false;
			}
			m_setupCompleted = true;
		}
	}

	private void ReceiveGameStateChanged(GameStateChanged data)
	{
		m_setupCompleted = false;
		if (data.state == LevelManager.GameState.TutorialBook)
		{
			m_state = State.Initial;
			m_playing = false;
			m_pointer.Show(show: false);
			m_tutorialBookOpened = true;
		}
		else if (data.state == LevelManager.GameState.Building)
		{
			if (!m_finished)
			{
				StartCoroutine(StartTutorial());
			}
		}
		else if (data.state == LevelManager.GameState.Running)
		{
			m_playing = false;
			m_pointer.Show(show: false);
			m_state = State.Initial;
			m_contraptionStarted = true;
		}
		else
		{
			m_playing = false;
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
			SetupTutorial();
		}
	}

	private IEnumerator StartEngineTutorial()
	{
		yield return new WaitForSeconds(0.5f);
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Running)
		{
			m_startEnginesTimeLine = new PointerTimeLine(m_pointer);
			GameObject gameObject = GameObject.Find("300_EnginesButton");
			if ((bool)gameObject)
			{
				List<Vector3> list = new List<Vector3>();
				list.Add(gameObject.transform.position + 11f * Vector3.down);
				list.Add(gameObject.transform.position + 5.5f * Vector3.down + 0.5f * Vector3.left);
				list.Add(gameObject.transform.position);
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Wait(1f));
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Move(list, 2f));
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Press());
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Wait(0.5f));
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Release());
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Wait(0.75f));
				m_startEnginesTimeLine.AddEvent(new PointerTimeLine.Hide());
				m_timeline = m_startEnginesTimeLine;
				m_timeline.Start();
				m_state = State.StartEngine;
				m_playing = true;
			}
		}
	}
}
