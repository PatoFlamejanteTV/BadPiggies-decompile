using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchematicButton : SliderButton
{
	public float hideSpeed;

	public List<GameObject> toggleList = new List<GameObject>();

	public GameObject button;

	private bool isButtonOut;

	private bool lastIsPlaying;

	private bool coroutineRunning;

	private const string AnimName = "SchematicsButtonSlide";

	private const string ToolBoxAnimName = "ToolBoxButton";

	private List<Vector3> origPositions;

	private List<Vector3> targetPositions;

	private List<Transform> selectedSlotSprites = new List<Transform>();

	private static LevelManager levelManager;

	private readonly int SLOT_COUNT = 3;

	private bool m_openList;

	private static LevelManager LevelManager
	{
		get
		{
			if (levelManager != null)
			{
				return levelManager;
			}
			return levelManager = Object.FindObjectOfType<LevelManager>();
		}
	}

	public static string LastLoadedSlotKey
	{
		get
		{
			if (LevelManager.CurrentGameMode is CakeRaceMode)
			{
				return $"cr_{Singleton<GameManager>.Instance.CurrentSceneName}_LastLoadedContraptionSlotIndex";
			}
			return $"{Singleton<GameManager>.Instance.CurrentSceneName}_LastLoadedContraptionSlotIndex";
		}
	}

	public bool ToolboxOpen => isButtonOut;

	protected override void Start()
	{
		base.Start();
		if (LevelManager != null && !LevelManager.m_sandbox)
		{
			return;
		}
		origPositions = new List<Vector3>();
		targetPositions = new List<Vector3>();
		foreach (GameObject toggle in toggleList)
		{
			origPositions.Add(toggle.transform.position);
			targetPositions.Add(toggle.transform.position + new Vector3(0f, 5f));
		}
		for (int i = 0; i < SLOT_COUNT; i++)
		{
			Transform transform = base.transform.FindChildRecursively($"Slot{i + 1:00}");
			if (!(transform == null))
			{
				Transform transform2 = transform.Find("SlotSelected");
				if (!(transform2 == null))
				{
					selectedSlotSprites.Add(transform2);
					transform2.GetComponent<Renderer>().enabled = false;
				}
			}
		}
		if (!GameProgress.HasKey(LastLoadedSlotKey))
		{
			GameProgress.SetInt(LastLoadedSlotKey, 0);
		}
	}

	private void OnEnable()
	{
		button.transform.Find("Gear").transform.rotation = Quaternion.identity;
		isButtonOut = (lastIsPlaying = (coroutineRunning = false));
		EnableRendererRecursively(base.gameObject, enable: false);
		ActivateToggleList(state: true);
		if (toggleList != null && origPositions != null)
		{
			for (int i = 0; i < toggleList.Count; i++)
			{
				toggleList[i].transform.position = origPositions[i];
			}
		}
		EventManager.Connect<UIEvent>(OnReceivedEvent);
	}

	private void OnDisable()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			Vector3 localPosition = child.localPosition;
			localPosition.x = (localPosition.y = 0f);
			child.localPosition = localPosition;
			child.localRotation = Quaternion.identity;
			child.localScale = Vector3.one * 0.2f;
		}
		EventManager.Disconnect<UIEvent>(OnReceivedEvent);
	}

	private void OnReceivedEvent(UIEvent data)
	{
		if (LevelManager != null && !LevelManager.m_sandbox)
		{
			return;
		}
		int num = -1;
		switch (data.type)
		{
		case UIEvent.Type.LoadContraptionSlot3:
			num = 2;
			break;
		case UIEvent.Type.LoadContraptionSlot2:
			num = 1;
			break;
		case UIEvent.Type.LoadContraptionSlot1:
			num = 0;
			break;
		}
		if (num >= 0)
		{
			GameProgress.SetInt(LastLoadedSlotKey, num);
			for (int i = 0; i < selectedSlotSprites.Count; i++)
			{
				selectedSlotSprites[i].GetComponent<Renderer>().enabled = num == i;
			}
		}
	}

	public void OnPressed()
	{
		if (GetComponent<Animation>().isPlaying || coroutineRunning)
		{
			return;
		}
		EnableRendererRecursively(base.gameObject, enable: true);
		int @int = GameProgress.GetInt(LastLoadedSlotKey);
		for (int i = 0; i < selectedSlotSprites.Count; i++)
		{
			selectedSlotSprites[i].GetComponent<Renderer>().enabled = @int == i;
		}
		bool flag = isButtonOut;
		InitAnimationStates(flag, GetComponent<Animation>()["SchematicsButtonSlide"], button.GetComponent<Animation>()["ToolBoxButton"]);
		button.GetComponent<Animation>().Play();
		GetComponent<Animation>().Play();
		isButtonOut = !isButtonOut;
		_ = isButtonOut;
		for (int j = 0; j < toggleList.Count; j++)
		{
			if (flag)
			{
				StartCoroutine(MoveObject(toggleList[j].transform, origPositions[j]));
			}
			else
			{
				StartCoroutine(MoveObject(toggleList[j].transform, targetPositions[j]));
			}
		}
	}

	private void Update()
	{
		if (!GetComponent<Animation>().isPlaying && lastIsPlaying && !isButtonOut)
		{
			ActivateToggleList(state: true);
		}
		if (m_openList)
		{
			OnPressed();
			m_openList = false;
		}
		lastIsPlaying = GetComponent<Animation>().isPlaying;
	}

	private void ActivateToggleList(bool state)
	{
		foreach (GameObject toggle in toggleList)
		{
			toggle.SetActive(state);
		}
	}

	private void InitAnimationStates(bool reverse, params AnimationState[] states)
	{
		foreach (AnimationState animationState in states)
		{
			animationState.speed = ((!reverse) ? 1 : (-1));
			animationState.time = ((!reverse) ? 0f : animationState.length);
		}
	}

	private void EnableRendererRecursively(GameObject obj, bool enable)
	{
		if ((bool)obj.GetComponent<Renderer>())
		{
			obj.GetComponent<Renderer>().enabled = enable;
		}
		if ((bool)obj.GetComponent<Collider>())
		{
			obj.GetComponent<Collider>().enabled = enable;
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			EnableRendererRecursively(obj.transform.GetChild(i).gameObject, enable);
		}
	}

	public void OpenMenu()
	{
		ActivateToggleList(state: true);
	}

	private IEnumerator MoveObject(Transform tf, Vector3 targetPos)
	{
		coroutineRunning = true;
		Vector3 startPos = tf.position;
		float step = 0f;
		while (step < 1f)
		{
			tf.position = Vector3.Lerp(startPos, targetPos, step);
			step += hideSpeed * Time.deltaTime;
			yield return null;
		}
		tf.position = targetPos;
		coroutineRunning = false;
	}
}
