using System;
using UnityEngine;

public class PartListingScrollbutton : Widget
{
	[SerializeField]
	private Transform graphicsTf;

	public Action<float> OnDrag;

	public Action OnDragBegin;

	public Action OnDragEnd;

	private bool interacting;

	private bool receivingInput;

	private InputEvent.EventType lastEvent;

	private void Update()
	{
		if (!interacting)
		{
			base.transform.position = graphicsTf.position + Vector3.back;
		}
	}

	protected override void OnInput(InputEvent input)
	{
		if (input.type == InputEvent.EventType.Drag && !interacting)
		{
			interacting = true;
			if (OnDragBegin != null)
			{
				OnDragBegin();
			}
		}
		if (input.type == InputEvent.EventType.Drag)
		{
			Vector3 position = WPFMonoBehaviour.hudCamera.ScreenToWorldPoint(input.position);
			position = base.transform.parent.InverseTransformPoint(position);
			position.z = -1f;
			base.transform.localPosition = position;
			if (OnDrag != null)
			{
				OnDrag(position.x);
			}
		}
		if (input.type == InputEvent.EventType.Release && interacting)
		{
			interacting = false;
			if (OnDragEnd != null)
			{
				OnDragEnd();
			}
		}
		lastEvent = input.type;
		receivingInput = true;
	}

	private void LateUpdate()
	{
		if (!receivingInput && lastEvent != InputEvent.EventType.Release)
		{
			OnInput(new InputEvent(InputEvent.EventType.Release, Input.mousePosition));
		}
		else
		{
			receivingInput = false;
		}
	}
}
