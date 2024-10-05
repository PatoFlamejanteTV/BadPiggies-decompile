using UnityEngine;

public class ContentLock : MonoBehaviour
{
	public void Activate(bool forceLimit = false)
	{
		if (LevelInfo.IsContentLimited(base.transform.parent.GetComponent<EpisodeButton>().Index) || forceLimit)
		{
			OverrideButtonAction();
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OverrideButtonAction()
	{
		Button component = base.transform.parent.GetComponent<Button>();
		if (component != null)
		{
			component.MethodToCall.SetMethod(base.gameObject.GetComponent<ContentLock>(), "ShowContentLimitNotification");
		}
	}

	public void ShowContentLimitNotification()
	{
		LevelInfo.DisplayContentLimitNotification();
	}
}
