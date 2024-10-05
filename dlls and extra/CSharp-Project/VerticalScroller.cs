using UnityEngine;

public class VerticalScroller : Widget
{
	[SerializeField]
	private GameObject root;

	[SerializeField]
	private GameObject[] followRoot;

	[SerializeField]
	private float upperBound = 10f;

	[SerializeField]
	private float lowerBound = -10f;

	[SerializeField]
	private float width = 10f;

	[SerializeField]
	private float upperPadding;

	[SerializeField]
	private float lowerPadding;

	[SerializeField]
	private float snapBackStrength = 10f;

	public float height;

	private Camera hudCamera;

	private bool isDragging;

	private float lastPosition;

	private float velocity;

	public float TotalHeight => upperPadding + lowerPadding + height;

	public float UpperBound => upperBound - upperPadding;

	public float LowerBound => lowerBound;

	private void Awake()
	{
		hudCamera = WPFMonoBehaviour.hudCamera;
		if (lowerBound > upperBound)
		{
			lowerBound = upperBound;
		}
	}

	private void Update()
	{
		if (isDragging && Input.GetMouseButtonUp(0))
		{
			isDragging = false;
		}
		if (isDragging)
		{
			return;
		}
		if (root.transform.localPosition.y < UpperBound)
		{
			float num = UpperBound - root.transform.localPosition.y;
			Move(Vector3.up * Time.deltaTime * num * snapBackStrength);
		}
		else if (root.transform.localPosition.y > TotalHeight + LowerBound)
		{
			float num2 = root.transform.localPosition.y - (TotalHeight + LowerBound);
			Move(Vector3.down * Time.deltaTime * num2 * snapBackStrength);
		}
		else
		{
			if (Mathf.Approximately(velocity, 0f))
			{
				return;
			}
			Move(Vector3.up * Time.deltaTime * velocity);
			if (velocity > 0f)
			{
				velocity -= Time.deltaTime * 10f;
				if (velocity < 0f)
				{
					velocity = 0f;
				}
			}
			else
			{
				velocity += Time.deltaTime * 10f;
				if (velocity > 0f)
				{
					velocity = 0f;
				}
			}
		}
	}

	private void Move(Vector3 deltaMovement)
	{
		root.transform.localPosition += deltaMovement;
		for (int i = 0; i < followRoot.Length; i++)
		{
			followRoot[i].transform.localPosition += deltaMovement;
		}
	}

	public void AddHeight(float delta)
	{
		height += delta;
	}

	protected override void OnInput(InputEvent input)
	{
		switch (input.type)
		{
		case InputEvent.EventType.Drag:
			OnDrag(input.position);
			break;
		case InputEvent.EventType.Release:
			OnRelease();
			break;
		case InputEvent.EventType.Press:
			OnPress(input.position);
			break;
		}
	}

	public void OnPress(Vector3 position)
	{
		if (!isDragging)
		{
			isDragging = true;
			lastPosition = hudCamera.ScreenToWorldPoint(position).y;
		}
	}

	public new void OnRelease()
	{
		isDragging = false;
	}

	public void OnDrag(Vector3 position)
	{
		if (isDragging)
		{
			Vector3 vector = hudCamera.ScreenToWorldPoint(position);
			Move(Vector3.up * (vector.y - lastPosition));
			velocity = vector.y - lastPosition;
			lastPosition = vector.y;
		}
	}

	public void SetLowerPadding(float newValue)
	{
		lowerPadding = newValue;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.position + Vector3.up * lowerBound + Vector3.left * width * 0.5f, base.transform.position + Vector3.up * upperBound + Vector3.left * width * 0.5f);
		Gizmos.DrawLine(base.transform.position + Vector3.up * lowerBound + Vector3.right * width * 0.5f, base.transform.position + Vector3.up * upperBound + Vector3.right * width * 0.5f);
		Gizmos.DrawLine(base.transform.position + Vector3.up * lowerBound + Vector3.left * width * 0.5f, base.transform.position + Vector3.up * lowerBound + Vector3.right * width * 0.5f);
		Gizmos.DrawLine(base.transform.position + Vector3.up * upperBound + Vector3.left * width * 0.5f, base.transform.position + Vector3.up * upperBound + Vector3.right * width * 0.5f);
	}
}
