using System.Collections;
using UnityEngine;

public class PullButton : Widget
{
	public enum PullType
	{
		Position,
		Rotation
	}

	[SerializeField]
	private PullType pullType;

	[SerializeField]
	private GameObject callObject;

	[SerializeField]
	private string methodToCall;

	[SerializeField]
	private Transform targetTf;

	[SerializeField]
	private Vector3 originalPosition;

	[SerializeField]
	private Vector3 activatePosition;

	[SerializeField]
	private float tapTresholdTime = 0.1f;

	[SerializeField]
	private float dragScale = 1f;

	[SerializeField]
	private PlayBundleAudio pullStartAudio;

	[SerializeField]
	private PlayBundleAudio pullEndAudio;

	private bool isDragging;

	private bool isLocked;

	private Vector3 lastPosition;

	private Camera hudCamera;

	private float touchStartTime;

	private bool isActivating;

	private void Awake()
	{
		hudCamera = GameObject.Find("HUDCamera").GetComponent<Camera>();
	}

	private void Update()
	{
		if (isDragging || isLocked)
		{
			return;
		}
		switch (pullType)
		{
		case PullType.Rotation:
		{
			float num = targetTf.localEulerAngles.z;
			if (num > 0f)
			{
				num -= 360f;
			}
			targetTf.localEulerAngles = Vector3.Slerp(Vector3.forward * num, originalPosition, Time.deltaTime * 20f);
			break;
		}
		case PullType.Position:
			targetTf.localPosition = Vector3.Lerp(targetTf.localPosition, originalPosition, Time.deltaTime * 20f);
			break;
		}
	}

	public void LockDragging(bool lockDragging)
	{
		isLocked = lockDragging;
	}

	public void SetPositionOffset(Vector3 positionOffset)
	{
		switch (pullType)
		{
		case PullType.Rotation:
			targetTf.localEulerAngles = originalPosition + positionOffset;
			break;
		case PullType.Position:
			targetTf.localPosition = originalPosition + positionOffset;
			break;
		}
	}

	protected override void OnInput(InputEvent input)
	{
		base.OnInput(input);
		if (isLocked)
		{
			return;
		}
		if (input.type == InputEvent.EventType.Press)
		{
			touchStartTime = Time.realtimeSinceStartup;
			lastPosition = hudCamera.ScreenToWorldPoint(input.position);
			isDragging = true;
		}
		else if (input.type == InputEvent.EventType.Release)
		{
			if (Time.realtimeSinceStartup - touchStartTime < tapTresholdTime)
			{
				StartCoroutine(ActivateSequence());
			}
			isDragging = false;
		}
		else if (input.type == InputEvent.EventType.Drag && isDragging)
		{
			Vector3 vector = hudCamera.ScreenToWorldPoint(input.position);
			float num = (vector.y - lastPosition.y) * dragScale;
			lastPosition = vector;
			switch (pullType)
			{
			case PullType.Rotation:
			{
				float num2 = targetTf.localEulerAngles.z;
				if (num2 > 0f)
				{
					num2 -= 360f;
				}
				float num3 = num2 + num;
				if (num3 < activatePosition.z)
				{
					targetTf.localEulerAngles = activatePosition;
					Activate();
				}
				else if (num3 > originalPosition.z)
				{
					targetTf.localEulerAngles = originalPosition;
				}
				else
				{
					targetTf.localEulerAngles = Vector3.forward * num3;
				}
				break;
			}
			case PullType.Position:
				targetTf.Translate(Vector3.up * num);
				if (targetTf.localPosition.y < activatePosition.y)
				{
					targetTf.localPosition = activatePosition;
					Activate();
				}
				else if (targetTf.localPosition.y > originalPosition.y)
				{
					targetTf.localPosition = originalPosition;
				}
				break;
			}
		}
		else if (input.type == InputEvent.EventType.MouseLeave && isDragging)
		{
			isDragging = false;
		}
	}

	public bool IsActivating()
	{
		return isActivating;
	}

	private IEnumerator ActivateSequence()
	{
		if (isLocked)
		{
			yield break;
		}
		if ((bool)pullStartAudio)
		{
			pullStartAudio.Play2dEffect();
		}
		isLocked = true;
		isActivating = true;
		yield return null;
		isDragging = true;
		float fade = 1f;
		while (fade > 0f)
		{
			fade -= Time.deltaTime * 3f;
			switch (pullType)
			{
			case PullType.Rotation:
				targetTf.localEulerAngles = Vector3.Slerp(activatePosition, originalPosition, fade);
				break;
			case PullType.Position:
				targetTf.localPosition = Vector3.Lerp(activatePosition, originalPosition, fade);
				break;
			}
			yield return null;
		}
		isActivating = false;
		Activate();
		isLocked = false;
	}

	private new void Activate()
	{
		isDragging = false;
		if (callObject != null && !string.IsNullOrEmpty(methodToCall))
		{
			callObject.SendMessage(methodToCall, SendMessageOptions.DontRequireReceiver);
		}
		if ((bool)pullEndAudio)
		{
			pullEndAudio.Play2dEffect();
		}
	}
}
