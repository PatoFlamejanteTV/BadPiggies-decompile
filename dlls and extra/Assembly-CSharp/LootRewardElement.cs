using System;
using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;

public class LootRewardElement : MonoBehaviour
{
	[SerializeField]
	private Transform iconRoot;

	private bool isDuplicatePart;

	[SerializeField]
	private GameObject openingEffect;

	private LootCrateButton parentButton;

	[SerializeField]
	private ParticleSystem blingEffect;

	[SerializeField]
	private BasePart.PartTier tier;

	public Action onRewardOpened;

	private bool waitingAnimation;

	private SkeletonAnimation skeletonAnimation;

	private SkeletonAnimation openingEffectAnimation;

	public Transform IconRoot => iconRoot;

	public bool IsDuplicatePart
	{
		get
		{
			return isDuplicatePart;
		}
		set
		{
			isDuplicatePart = value;
		}
	}

	public ParticleSystem BlingEffect => blingEffect;

	public BasePart.PartTier Tier => tier;

	private void Awake()
	{
		skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
	}

	private IEnumerator WaitForOpening()
	{
		float waitTimeLeft = 1f;
		while (waitTimeLeft > 0f)
		{
			waitTimeLeft -= GameTime.RealTimeDelta;
			yield return null;
		}
		while (base.transform.parent.localPosition.magnitude > 0.1f)
		{
			yield return null;
		}
		if (onRewardOpened != null)
		{
			onRewardOpened();
		}
		if (openingEffect != null)
		{
			waitTimeLeft = 0.5f;
			while (waitTimeLeft > 0f)
			{
				waitTimeLeft -= GameTime.RealTimeDelta;
				yield return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(openingEffect, base.transform.position, Quaternion.identity);
			gameObject.transform.parent = base.transform.parent;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.right * 1.5f - Vector3.forward * 2f;
			openingEffectAnimation = gameObject.GetComponentInChildren<RealtimeSkeletonAnimation>();
			if (openingEffectAnimation != null)
			{
				openingEffectAnimation.state.End += OnEffectAnimationEnd;
				openingEffectAnimation.state.SetAnimation(0, "Intro1", loop: false);
				openingEffectAnimation.state.AddAnimation(0, "Outro", loop: false, 0f);
			}
		}
		PlayJumpAnimation();
	}

	private void OnEffectAnimationEnd(Spine.AnimationState state, int trackIndex)
	{
		if (openingEffectAnimation != null && state.ToString().Equals("Outro"))
		{
			openingEffectAnimation.state.End -= OnEffectAnimationEnd;
			UnityEngine.Object.Destroy(openingEffectAnimation.transform.parent.gameObject);
		}
	}

	private void OnOpenStart()
	{
		if (parentButton != null)
		{
			LootCrateButton lootCrateButton = parentButton;
			lootCrateButton.onOpeningStart = (Action)Delegate.Remove(lootCrateButton.onOpeningStart, new Action(OnOpenStart));
		}
		StartCoroutine(WaitForOpening());
	}

	public void InitElement(LootCrateButton button)
	{
		parentButton = button;
		LootCrateButton lootCrateButton = parentButton;
		lootCrateButton.onOpeningStart = (Action)Delegate.Combine(lootCrateButton.onOpeningStart, new Action(OnOpenStart));
	}

	private void PlayJumpAnimation()
	{
		if (tier != 0 && tier != BasePart.PartTier.Common)
		{
			if (tier == BasePart.PartTier.Rare)
			{
				skeletonAnimation.state.AddAnimation(0, "Rare_Item", loop: false, 0f);
			}
			if (tier == BasePart.PartTier.Epic)
			{
				skeletonAnimation.state.AddAnimation(0, "Epic_Item", loop: false, 0f);
			}
			waitingAnimation = isDuplicatePart;
			skeletonAnimation.state.End += OnJumpAnimationEnd;
		}
	}

	private void OnJumpAnimationEnd(Spine.AnimationState state, int trackIndex)
	{
		waitingAnimation = false;
		skeletonAnimation.state.End -= OnJumpAnimationEnd;
	}

	public void PlayScrapAnimation()
	{
		if (skeletonAnimation != null)
		{
			skeletonAnimation.state.SetAnimation(0, "Scrap1", loop: false);
		}
		ScrapButton.Instance.AddParticles(base.gameObject, 10, 1f);
		StartCoroutine(DelaySalvageSound());
	}

	private IEnumerator DelaySalvageSound()
	{
		float waitTime = 0.85f;
		while (waitTime > 0f)
		{
			waitTime -= GameTime.RealTimeDelta;
			yield return null;
		}
		Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.salvagePart);
	}

	public IEnumerator WaitJumpAnimation()
	{
		while (waitingAnimation)
		{
			yield return null;
		}
	}
}
