using UnityEngine;

public class VerticalScrollButton : Widget
{
	[SerializeField]
	private float dragStartLimit = 0.2f;

	private float dragStartedAt = -1f;

	private VerticalScroller vScroller;

	public void SetScroller(VerticalScroller scroller)
	{
		vScroller = scroller;
	}

	protected override void OnInput(InputEvent input)
	{
		switch (input.type)
		{
		case InputEvent.EventType.Drag:
			if (Mathf.Abs(dragStartedAt - input.position.y) > dragStartLimit)
			{
				dragStartedAt = -1f;
				vScroller.OnDrag(input.position);
			}
			break;
		case InputEvent.EventType.Release:
			vScroller.OnRelease();
			if (Mathf.Abs(dragStartedAt - input.position.y) <= dragStartLimit)
			{
				SendMessage("VerticalScrollButtonActivate", SendMessageOptions.DontRequireReceiver);
			}
			break;
		case InputEvent.EventType.Press:
			vScroller.OnPress(input.position);
			dragStartedAt = input.position.y;
			break;
		}
	}
}
