using System.Collections.Generic;
using UnityEngine;

public class ScrollList : Widget, WidgetListener
{
	private enum Action
	{
		Place,
		DrawGizmos
	}

	private struct ScrollInfo
	{
		public float time;

		public float offset;

		public ScrollInfo(float time, float offset)
		{
			this.time = time;
			this.offset = offset;
		}
	}

	public GameObject leftButton;

	public GameObject rightButton;

	public GameObject scrollButtonOffset;

	public GameObject buttonPrefab;

	public int horizontalCount = 10;

	public Vector2 offset = new Vector2(10f, 10f);

	public int count = 20;

	public int maxRows = 1;

	public bool fillScreenWidth = true;

	public float leftMargin;

	public WidgetListener m_listener;

	private List<GameObject> buttons = new List<GameObject>();

	private bool dragging;

	private bool dragDirectionDetected;

	private bool dragLocked;

	private Vector3 dragStart;

	private float scrollOffsetAtDragStart;

	private float scrollOffset;

	private float scrollTargetOffset;

	private float scrollTargetStart;

	private bool scrollTargetSet;

	private float scrollTargetSetTime;

	private float scrollVelocity;

	private Queue<ScrollInfo> scrollHistory = new Queue<ScrollInfo>();

	private bool canScroll = true;

	private float scrollButtonWidth;

	private float scrollAreaWidth;

	private bool onLeftBorder;

	private bool onRightBorder;

	private bool updatePlacement;

	private int usedRows = 1;

	private float buttonScale = 1f;

	private float screenWidth = -1f;

	private bool isLeftEnabled;

	private bool isRightEnabled;

	public int UsedRows => usedRows;

	public void SetButtonScale(float buttonScale)
	{
		this.buttonScale = buttonScale;
		offset *= buttonScale;
	}

	public void SetMaxRows(int rows)
	{
		maxRows = rows;
	}

	public void ScrollLeft()
	{
		scrollTargetOffset = scrollOffset - 0.5f * scrollAreaWidth;
		updatePlacement = true;
		onRightBorder = false;
		float num = -0.5f * (float)horizontalCount * offset.x;
		float num2 = ScreenWidth() * 0.5f;
		float num3 = num - scrollTargetOffset;
		float num4 = 0f - num2 + scrollButtonWidth;
		if (Mathf.Abs(num3 - num4) < 3f)
		{
			scrollTargetOffset -= 3f;
		}
		if (num3 > num4)
		{
			scrollTargetOffset = num - num4 - 0.01f;
		}
		scrollTargetSet = true;
		scrollTargetStart = scrollOffset;
		scrollTargetSetTime = Time.time;
	}

	public void ScrollRight()
	{
		scrollTargetOffset = scrollOffset + 0.5f * scrollAreaWidth;
		updatePlacement = true;
		onLeftBorder = false;
		float num = 0.5f * (float)horizontalCount * offset.x;
		float num2 = ScreenWidth() * 0.5f;
		float num3 = num - scrollTargetOffset;
		float num4 = num2 - scrollButtonWidth;
		if (Mathf.Abs(num3 - num4) < 3f)
		{
			scrollTargetOffset += 3f;
		}
		if (num3 < num4)
		{
			scrollTargetOffset = num - num4 + 0.01f;
		}
		scrollTargetSet = true;
		scrollTargetStart = scrollOffset;
		scrollTargetSetTime = Time.time;
	}

	public override void SetListener(WidgetListener listener)
	{
		m_listener = listener;
	}

	protected override void OnInput(InputEvent input)
	{
		if (input.type == InputEvent.EventType.Press && canScroll)
		{
			dragging = true;
			dragStart = Singleton<GuiManager>.Instance.FindCamera().ScreenToWorldPoint(GuiManager.GetPointer().position);
			scrollOffsetAtDragStart = scrollOffset;
			dragDirectionDetected = false;
			dragLocked = false;
		}
	}

	public GameObject FindButton(object dragObject)
	{
		foreach (GameObject button in buttons)
		{
			DraggableButton component = button.GetComponent<DraggableButton>();
			if ((bool)component && component.DragObject == dragObject)
			{
				return component.gameObject;
			}
		}
		return null;
	}

	public void SetSelection(object targetObject)
	{
		foreach (GameObject button in buttons)
		{
			DraggableButton component = button.GetComponent<DraggableButton>();
			if ((bool)component && component.DragObject == targetObject)
			{
				Select(component, targetObject);
				component.Select();
			}
		}
	}

	public void Select(Widget widget, object targetObject)
	{
		foreach (GameObject button in buttons)
		{
			Widget component = button.GetComponent<Widget>();
			if ((bool)component && component != widget)
			{
				component.Deselect();
			}
		}
		if (m_listener != null)
		{
			m_listener.Select(widget, targetObject);
		}
	}

	public void ResetSelection()
	{
		foreach (GameObject button in buttons)
		{
			Widget component = button.GetComponent<Widget>();
			if ((bool)component)
			{
				component.Deselect();
			}
		}
	}

	public void StartDrag(Widget widget, object targetObject)
	{
		if (m_listener != null)
		{
			m_listener.StartDrag(widget, targetObject);
		}
	}

	public void CancelDrag(Widget widget, object targetObject)
	{
		if (m_listener != null)
		{
			m_listener.CancelDrag(widget, targetObject);
		}
	}

	public void Drop(Widget widget, Vector3 dropPosition, object targetObject)
	{
		if (m_listener != null)
		{
			m_listener.Drop(widget, dropPosition, targetObject);
		}
	}

	public void AddButton(Widget button)
	{
		button.transform.localScale *= buttonScale;
		button.SetListener(this);
		buttons.Add(button.gameObject);
		PlaceButtons(Action.Place);
	}

	public void RemoveButton(Widget button)
	{
		button.SetListener(null);
		buttons.Remove(button.gameObject);
		Object.Destroy(button.gameObject);
		PlaceButtons(Action.Place);
	}

	private float ScreenWidth()
	{
		if (fillScreenWidth)
		{
			return 20f * (float)Screen.width / (float)Screen.height - leftMargin;
		}
		if (screenWidth <= 0f)
		{
			float x = leftButton.transform.Find("Button").GetComponent<Collider>().bounds.min.x;
			float x2 = rightButton.transform.Find("Button").GetComponent<Collider>().bounds.max.x;
			screenWidth = Mathf.Abs(x - x2);
		}
		return screenWidth;
	}

	private void Start()
	{
		Vector3 vector = leftButton.transform.Find("Button").GetComponent<Sprite>().Size;
		scrollButtonWidth = Mathf.Abs((leftButton.transform.Find("Button").transform.rotation * vector).x);
		if (fillScreenWidth && (float)horizontalCount * offset.x * 0.5f > 10f * (float)Screen.width / (float)Screen.height - leftMargin)
		{
			Vector3 localPosition = base.transform.localPosition;
			localPosition.x += 0.5f * leftMargin;
			base.transform.localPosition = localPosition;
			localPosition = scrollButtonOffset.transform.localPosition;
			localPosition.x += 0.5f * leftMargin;
			scrollButtonOffset.transform.localPosition = localPosition;
		}
		scrollAreaWidth = ScreenWidth() - 2.5f * scrollButtonWidth;
		float num = -0.5f * (float)horizontalCount * offset.x;
		float num2 = 0f - ScreenWidth() * 0.5f + scrollButtonWidth;
		scrollOffset = num - num2 - 0.01f;
		SetBorderStates();
	}

	private void Update()
	{
		if (scrollTargetSet)
		{
			float num = Mathf.Abs(scrollTargetStart - scrollTargetOffset) / (0.5f * scrollAreaWidth);
			if (num < 0.1f)
			{
				num = 0.1f;
			}
			float num2 = Mathf.Pow(4f * (Time.time - scrollTargetSetTime) / num, 1f);
			if (num2 > 1f)
			{
				num2 = 1f;
				scrollTargetSet = false;
			}
			scrollOffset = Mathf.Lerp(scrollTargetStart, scrollTargetOffset, num2);
			PlaceButtons(Action.Place);
		}
		if (dragging)
		{
			Vector3 b = Singleton<GuiManager>.Instance.FindCamera().ScreenToWorldPoint(GuiManager.GetPointer().position);
			if (dragDirectionDetected && dragLocked)
			{
				float num3 = dragStart.x - b.x;
				if (num3 != 0f)
				{
					onLeftBorder = false;
					onRightBorder = false;
				}
				scrollOffset = scrollOffsetAtDragStart + num3;
				while (scrollHistory.Count > 0)
				{
					ScrollInfo scrollInfo = scrollHistory.Peek();
					if (scrollInfo.time >= Time.time - 0.1f)
					{
						float num4 = Time.time - scrollInfo.time;
						if (num4 > 0f)
						{
							scrollVelocity = (scrollOffset - scrollInfo.offset) / num4;
						}
						break;
					}
					scrollHistory.Dequeue();
				}
				scrollHistory.Enqueue(new ScrollInfo(Time.time, scrollOffset));
				PlaceButtons(Action.Place);
			}
			if (!dragDirectionDetected && Vector3.Distance(dragStart, b) > 0.5f)
			{
				dragDirectionDetected = true;
				if (Mathf.Abs(dragStart.x - b.x) < 2.5f * (b.y - dragStart.y))
				{
					dragging = false;
				}
				else
				{
					dragLocked = true;
				}
			}
			if (GuiManager.GetPointer().up || (dragDirectionDetected && !dragLocked))
			{
				dragging = false;
			}
		}
		else if (Mathf.Abs(scrollVelocity) > 0.01f)
		{
			float num5 = Time.deltaTime * scrollVelocity;
			if (num5 != 0f)
			{
				onLeftBorder = false;
				onRightBorder = false;
			}
			scrollOffset += num5;
			scrollVelocity *= Mathf.Pow(0.9f, Time.deltaTime / (1f / 60f));
			PlaceButtons(Action.Place);
		}
		SetBorderStates();
		if (updatePlacement)
		{
			PlaceButtons(Action.Place);
			updatePlacement = false;
		}
	}

	public void Clear()
	{
		foreach (GameObject button in buttons)
		{
			Object.Destroy(button);
		}
		buttons.Clear();
	}

	private void OnDrawGizmos()
	{
		if ((bool)buttonPrefab)
		{
			PlaceButtons(Action.DrawGizmos);
		}
		if (!Application.isPlaying && fillScreenWidth)
		{
			float y = 1f;
			if ((bool)leftButton)
			{
				y = leftButton.transform.Find("Button").GetComponent<Collider>().bounds.size.y;
			}
			float num = -13.333333f + leftMargin;
			float num2 = 13.333333f;
			Vector3 position = base.transform.position;
			position.x = 0.5f * (num + num2);
			Gizmos.DrawWireCube(position, new Vector3(num2 - num, y, 0f));
		}
	}

	private void SetBorderStates()
	{
		float num = -0.5f * (float)horizontalCount * offset.x;
		float num2 = ScreenWidth() * 0.5f;
		if (num > 0f - num2)
		{
			if (leftButton.activeInHierarchy || rightButton.activeInHierarchy)
			{
				if (scrollOffset != 0f || scrollVelocity != 0f)
				{
					scrollOffset = 0f;
					scrollVelocity = 0f;
					PlaceButtons(Action.Place);
				}
				leftButton.SetActive(value: false);
				rightButton.SetActive(value: false);
				canScroll = false;
			}
		}
		else if (!leftButton.activeInHierarchy || !rightButton.activeInHierarchy)
		{
			leftButton.SetActive(value: true);
			EnableScrollButtonLeft(enable: true);
			rightButton.SetActive(value: true);
			EnableScrollButtonRight(enable: true);
			canScroll = true;
		}
		if (canScroll)
		{
			float num3 = num - scrollOffset;
			float num4 = 0f - num2 + scrollButtonWidth;
			if (num3 - num4 > 0.001f)
			{
				scrollOffset = num - num4;
				scrollVelocity = 0f;
				EnableScrollButtonLeft(enable: false);
				onLeftBorder = true;
				PlaceButtons(Action.Place);
			}
			else if (!onLeftBorder)
			{
				EnableScrollButtonLeft(enable: true);
			}
			float num5 = 0f - num;
			float num6 = num5 - scrollOffset;
			float num7 = num2 - scrollButtonWidth;
			if (num6 - num7 < -0.001f)
			{
				scrollOffset = num5 - num7;
				scrollVelocity = 0f;
				EnableScrollButtonRight(enable: false);
				onRightBorder = true;
				PlaceButtons(Action.Place);
			}
			else if (!onRightBorder)
			{
				EnableScrollButtonRight(enable: true);
			}
		}
		else
		{
			scrollOffset = 0f;
		}
	}

	private void EnableScrollButtonLeft(bool enable)
	{
		if (isLeftEnabled != enable)
		{
			isLeftEnabled = enable;
			EnableRendererRecursively(leftButton, enable);
		}
	}

	private void EnableScrollButtonRight(bool enable)
	{
		if (isRightEnabled != enable)
		{
			isRightEnabled = enable;
			EnableRendererRecursively(rightButton, enable);
		}
	}

	private void EnableRendererRecursively(GameObject obj, bool enable)
	{
		Renderer component = obj.GetComponent<Renderer>();
		if ((bool)component)
		{
			component.enabled = enable;
		}
		Collider component2 = obj.GetComponent<Collider>();
		if ((bool)component2)
		{
			component2.enabled = enable;
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			EnableRendererRecursively(obj.transform.GetChild(i).gameObject, enable);
		}
	}

	private void PlaceScrollButtons()
	{
		Vector3 localPosition = scrollButtonOffset.transform.localPosition;
		if (usedRows > 1)
		{
			localPosition.y = 0.5f * (float)(usedRows - 1) * offset.y;
		}
		else
		{
			localPosition.y = 0f;
		}
		float num = ScreenWidth() * 0.5f;
		Vector3 localPosition2 = leftButton.transform.localPosition;
		localPosition2.x = 0f - num + scrollButtonWidth * 0.5f;
		leftButton.transform.localPosition = localPosition2;
		localPosition2 = rightButton.transform.localPosition;
		localPosition2.x = num - scrollButtonWidth * 0.5f;
		rightButton.transform.localPosition = localPosition2;
		scrollButtonOffset.transform.localPosition = localPosition;
	}

	private void PlaceButtons(Action action)
	{
		if (action == Action.Place)
		{
			horizontalCount = buttons.Count;
			scrollAreaWidth = ScreenWidth() - 2.5f * scrollButtonWidth;
			int num = (int)scrollAreaWidth / (int)offset.x - 3;
			if (horizontalCount > num)
			{
				usedRows = maxRows;
				horizontalCount = horizontalCount / usedRows + ((horizontalCount % usedRows != 0) ? 1 : 0);
			}
			else
			{
				usedRows = 1;
			}
			PlaceScrollButtons();
		}
		int num2 = 0;
		int num3 = 0;
		Vector3 position = base.transform.position;
		position.x -= 0.5f * ((float)(horizontalCount - 1) * offset.x) + scrollOffset;
		position.y -= 0.5f * buttonPrefab.GetComponent<Sprite>().Size.y;
		position.y += (float)(usedRows - 1) * offset.y;
		Vector3 vector = position;
		int num4 = count;
		if (action == Action.Place)
		{
			num4 = buttons.Count;
		}
		for (int i = 0; i < num4; i++)
		{
			if (action == Action.Place)
			{
				buttons[i].transform.position = vector;
			}
			else
			{
				Gizmos.DrawWireCube(vector, buttonPrefab.GetComponent<Sprite>().Size);
			}
			vector.x += offset.x;
			num2++;
			if (num2 >= horizontalCount)
			{
				num2 = 0;
				vector.x = position.x;
				vector.y -= offset.y;
				num3++;
				if (num3 >= usedRows)
				{
					vector = new Vector3(100000f, 0f, 0f);
				}
			}
		}
	}

	private void OnEnable()
	{
		ScreenWidth();
		isLeftEnabled = (isRightEnabled = true);
		EnableScrollButtonLeft(enable: false);
		EnableScrollButtonRight(enable: false);
		SetBorderStates();
	}
}
