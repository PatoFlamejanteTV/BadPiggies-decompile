using Spine;
using Spine.Unity;
using UnityEngine;

public class AnimationLoop : MonoBehaviour
{
	[SerializeField]
	private SkeletonAnimation skeletonAnimation;

	[SerializeField]
	private float interval;

	[SerializeField]
	private string animationName;

	private bool animationPlaying;

	private float counter;

	private void Start()
	{
		skeletonAnimation.state.SetAnimation(0, animationName, loop: false);
		skeletonAnimation.state.End += OnAnimationEnd;
		animationPlaying = true;
	}

	private void Update()
	{
		if (!animationPlaying)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				skeletonAnimation.state.SetAnimation(0, animationName, loop: false);
			}
		}
	}

	private void OnAnimationEnd(Spine.AnimationState state, int trackIndex)
	{
		counter = interval;
		animationPlaying = false;
	}
}
