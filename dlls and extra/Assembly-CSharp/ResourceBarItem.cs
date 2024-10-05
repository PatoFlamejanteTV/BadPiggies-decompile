using UnityEngine;

public class ResourceBarItem : WPFMonoBehaviour
{
	[SerializeField]
	private GameObject enabledObjects;

	[SerializeField]
	private float paddingLeft;

	[SerializeField]
	private float paddingRight;

	private bool show = true;

	private Vector3 targetPosition = Vector3.zero;

	private bool isInit;

	public float Width => paddingLeft + paddingRight;

	public float PaddingLeft => paddingLeft;

	public float PaddingRight => paddingRight;

	public bool IsShowing => show;

	public bool IsEnabled { get; private set; }

	public void SetItem(bool show, bool enable)
	{
		this.show = show;
		IsEnabled = enable;
		base.transform.localPosition = Vector3.right * base.transform.localPosition.x + Vector3.up * ((!show) ? 3f : 0f);
		if (enabledObjects != null)
		{
			enabledObjects.SetActive(enable);
		}
		if (base.collider != null)
		{
			base.collider.enabled = enable;
		}
		if (show)
		{
			SendMessage("OnShowButton", base.gameObject, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			SendMessage("OnHideButton", base.gameObject, SendMessageOptions.DontRequireReceiver);
		}
		if (enable)
		{
			SendMessage("OnEnableButton", base.gameObject, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			SendMessage("OnDisableButton", base.gameObject, SendMessageOptions.DontRequireReceiver);
		}
		isInit = true;
	}

	public void SetHorizontalPosition(float horizontalPosition)
	{
		base.transform.localPosition = new Vector3(horizontalPosition, base.transform.localPosition.y, base.transform.localPosition.z);
	}

	private void OnDrawGizmos()
	{
		if (base.transform != null)
		{
			Gizmos.color = Color.white;
			float num = paddingRight - paddingLeft;
			Gizmos.DrawWireCube(base.transform.position + Vector3.right * num * 0.5f, Vector3.right * Width + Vector3.up * 2f);
		}
	}
}
