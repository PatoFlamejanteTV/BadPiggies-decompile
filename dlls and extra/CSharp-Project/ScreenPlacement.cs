using UnityEngine;

public class ScreenPlacement : MonoBehaviour
{
	public enum Anchor
	{
		Left,
		Right
	}

	public Anchor m_anchor;

	public float relativePosition;

	public const float DefaultScreenWidth = 1024f;

	public const float DefaultScreenHeight = 768f;

	public const float DefaultScreenHalfWidth = 512f;

	public const float DefaultScreenHalfHeight = 384f;

	public const float DefaultCameraSize = 10f;

	public const float DefaultViewHalfWidth = 13.333333f;

	private int lastScreenWidth;

	private int lastScreenHeight;

	public static bool IsAspectRatioNarrowerThan(float width, float height)
	{
		return (float)Screen.width / (float)Screen.height < width / height - 0.01f;
	}

	private void OnEnable()
	{
		PlaceHorizontal();
	}

	private void Update()
	{
		if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
		{
			PlaceHorizontal();
		}
	}

	private void PlaceHorizontal()
	{
		if (Screen.width > 0 && Screen.height > 0)
		{
			lastScreenWidth = Screen.width;
			lastScreenHeight = Screen.height;
		}
		float num = 0f;
		GameObject gameObject = GameObject.FindWithTag("HUDCamera");
		if ((bool)gameObject)
		{
			num = gameObject.transform.position.x;
		}
		if (m_anchor == Anchor.Left)
		{
			float num2 = 10f * (float)lastScreenWidth / (float)lastScreenHeight;
			float num3 = (float)lastScreenHeight / 768f;
			Vector3 position = base.transform.position;
			position.x = (-0.5f * (float)lastScreenWidth + num3 * relativePosition) / (0.5f * (float)lastScreenWidth) * num2 + num;
			base.transform.position = position;
		}
		else if (m_anchor == Anchor.Right)
		{
			float num4 = 10f * (float)lastScreenWidth / (float)lastScreenHeight;
			float num5 = (float)lastScreenHeight / 768f;
			Vector3 position2 = base.transform.position;
			position2.x = (0.5f * (float)lastScreenWidth + num5 * relativePosition) / (0.5f * (float)lastScreenWidth) * num4 + num;
			base.transform.position = position2;
		}
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			CalculateHorizontal();
		}
	}

	private void CalculateHorizontal()
	{
		if (m_anchor == Anchor.Left)
		{
			relativePosition = base.transform.position.x / 13.333333f * 512f + 512f;
		}
		else if (m_anchor == Anchor.Right)
		{
			relativePosition = base.transform.position.x / 13.333333f * 512f - 512f;
		}
	}
}
