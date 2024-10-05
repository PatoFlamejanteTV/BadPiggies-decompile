using UnityEngine;

public class TooltipInfo : MonoBehaviour
{
	[SerializeField]
	private bool useAttachedButton;

	[SerializeField]
	private string tooltipLocaleKey = "default";

	[SerializeField]
	private Transform tooltipTarget;

	private Tooltip tooltip;

	private void Awake()
	{
		if (useAttachedButton)
		{
			Button component = base.transform.GetComponent<Button>();
			if (component == null && base.transform.parent != null)
			{
				component = base.transform.parent.GetComponent<Button>();
			}
			if (component != null)
			{
				component.enabled = true;
				component.MethodToCall.SetMethod(this, "Show");
			}
		}
	}

	public void Show()
	{
		if (tooltip == null)
		{
			GameObject gameObject = Object.Instantiate(WPFMonoBehaviour.gameData.m_tooltipPrefab);
			gameObject.transform.position = base.transform.position;
			tooltip = gameObject.GetComponent<Tooltip>();
			tooltip.SetLocaleKey(tooltipLocaleKey);
			tooltip.SetTarget((!(tooltipTarget != null)) ? base.transform : tooltipTarget);
		}
	}
}
