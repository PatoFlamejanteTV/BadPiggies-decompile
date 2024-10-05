using UnityEngine;

public class DisableAfterAnimation : MonoBehaviour
{
	public GameObject target;

	public string animName;

	private void OnEnable()
	{
		AnimationClip animationClip = (((bool)GetComponent<Animation>() && !string.IsNullOrEmpty(animName)) ? GetComponent<Animation>()[animName].clip : ((!GetComponent<Animation>()) ? null : GetComponent<Animation>().clip));
		if ((bool)target && (bool)animationClip)
		{
			animationClip.AddEvent(new AnimationEvent
			{
				time = animationClip.length,
				functionName = "OnAnimFinished"
			});
		}
	}

	private void OnAnimFinished()
	{
		if ((bool)target)
		{
			target.SetActive(value: false);
		}
	}
}
