using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public class LootWheelRewardingRoutine : WPFMonoBehaviour
{
	public enum BackgroundType
	{
		Epic,
		Rare,
		Common,
		Regular
	}

	[SerializeField]
	private float showTime;

	[SerializeField]
	private Transform targetPosition;

	[SerializeField]
	private string hornAnimation;

	[SerializeField]
	private string rewardAnimationName;

	[SerializeField]
	private GameObject rewardParticles;

	[SerializeField]
	private GameObject confettiParticles;

	[SerializeField]
	private SkeletonAnimation rewardAnimation;

	[SerializeField]
	private GameObject epicRewardBackground;

	[SerializeField]
	private GameObject rareRewardBackground;

	[SerializeField]
	private GameObject commonRewardBackground;

	[SerializeField]
	private GameObject regularRewardBackground;

	private Transform horns;

	private GameObject icon;

	private SkeletonAnimation[] hornAnimations;

	private Renderer[] hornRenderers;

	private ParticleSystem rewardParticleSystem;

	private ParticleSystem confetti1ParticleSystem;

	private ParticleSystem confetti2ParticleSystem;

	private Dictionary<BackgroundType, GameObject> bgDictionary;

	private Action OnClosed;

	private void Awake()
	{
		horns = base.transform.Find("Horns");
		hornAnimations = horns.gameObject.GetComponentsInChildren<SkeletonAnimation>();
		hornRenderers = horns.gameObject.GetComponentsInChildren<Renderer>();
		if (hornAnimations[0].state == null)
		{
			hornAnimations[0].Initialize(overwrite: true);
		}
		hornAnimations[0].state.Event += OnAnimationEvent;
		horns.gameObject.SetActive(value: false);
		rewardParticleSystem = CreateParticles(rewardParticles, Vector3.zero, base.transform);
		rewardParticleSystem.Stop();
		rewardParticleSystem.playOnAwake = false;
		confetti1ParticleSystem = CreateParticles(confettiParticles, new Vector3(1f / 3f, 0f), base.transform);
		confetti1ParticleSystem.playOnAwake = false;
		confetti1ParticleSystem.Stop();
		confetti2ParticleSystem = CreateParticles(confettiParticles, new Vector3(2f / 3f, 0f), base.transform);
		confetti2ParticleSystem.playOnAwake = false;
		confetti2ParticleSystem.Stop();
		if (rewardAnimation.state == null)
		{
			rewardAnimation.Initialize(overwrite: true);
		}
		rewardAnimation.state.Event += OnAnimationEvent;
		bgDictionary = new Dictionary<BackgroundType, GameObject>
		{
			{
				BackgroundType.Epic,
				epicRewardBackground
			},
			{
				BackgroundType.Rare,
				rareRewardBackground
			},
			{
				BackgroundType.Common,
				commonRewardBackground
			},
			{
				BackgroundType.Regular,
				regularRewardBackground
			}
		};
	}

	private ParticleSystem CreateParticles(GameObject prefab, Vector3 screenPosition, Transform parent)
	{
		GameObject obj = UnityEngine.Object.Instantiate(prefab);
		LayerHelper.SetSortingLayer(obj, "Popup", children: true);
		LayerHelper.SetLayer(obj, base.gameObject.layer, children: true);
		LayerHelper.SetOrderInLayer(obj, 1, children: true);
		Vector3 position = WPFMonoBehaviour.hudCamera.ViewportToWorldPoint(screenPosition);
		obj.transform.position = position;
		obj.transform.parent = parent;
		Vector3 localPosition = obj.transform.localPosition;
		localPosition.z = 0f;
		obj.transform.localPosition = localPosition;
		obj.transform.rotation = Quaternion.identity;
		return obj.GetComponent<ParticleSystem>();
	}

	private void OnEnable()
	{
		Singleton<GuiManager>.Instance.GrabPointer(this);
		Singleton<KeyListener>.Instance.GrabFocus(this);
		KeyListener.keyReleased += HandleKeyReleased;
	}

	private void OnDisable()
	{
		if (Singleton<GuiManager>.IsInstantiated())
		{
			Singleton<GuiManager>.Instance.ReleasePointer(this);
		}
		if (Singleton<KeyListener>.IsInstantiated())
		{
			Singleton<KeyListener>.Instance.ReleaseFocus(this);
		}
		KeyListener.keyReleased -= HandleKeyReleased;
		StopAllCoroutines();
		horns.gameObject.SetActive(value: false);
		if ((bool)icon)
		{
			UnityEngine.Object.Destroy(icon);
		}
		if (OnClosed != null)
		{
			OnClosed();
			OnClosed = null;
		}
		for (int i = 0; i < hornAnimations.Length; i++)
		{
			if ((bool)hornAnimations[i] && hornAnimations[i].state != null)
			{
				hornAnimations[i].state.ClearTracks();
			}
		}
		if (rewardAnimation != null && rewardAnimation.state != null)
		{
			rewardAnimation.state.End -= OnAnimationEnd;
		}
	}

	public void OnPressed()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
			BackgroundMask.Show(show: false, this, string.Empty);
			if (OnClosed != null)
			{
				OnClosed();
			}
		}
	}

	public void ShowRewarding(bool showHorns, int rewardAmount, GameObject target, BackgroundType bgType, Action OnEnd)
	{
		base.gameObject.SetActive(value: true);
		ShowHorns(showHorns);
		BackgroundMask.Show(show: true, this, "Popup", base.transform, Vector3.forward * 0.1f);
		icon = ConstructIcon(target, bgType, rewardAmount);
		icon.transform.parent = targetPosition;
		icon.transform.localPosition = Vector3.zero;
		icon.transform.localRotation = Quaternion.identity;
		icon.transform.localScale = Vector3.one;
		OnClosed = OnEnd;
		ShowAnimation();
	}

	private GameObject ConstructIcon(GameObject target, BackgroundType bgType, int rewardAmount)
	{
		GameObject gameObject = new GameObject();
		gameObject = UnityEngine.Object.Instantiate(bgDictionary[bgType]);
		target.transform.parent = gameObject.transform;
		target.transform.localPosition = Vector3.back * 0.1f;
		target.transform.localRotation = Quaternion.identity;
		target.transform.localScale = Vector3.one;
		if (bgType == BackgroundType.Regular)
		{
			Transform transform = gameObject.transform.Find("Label");
			Transform transform2 = gameObject.transform.Find("Text");
			if (rewardAmount > 1)
			{
				TextMesh[] componentsInChildren = transform2.gameObject.GetComponentsInChildren<TextMesh>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].text = rewardAmount.ToString();
				}
			}
			else
			{
				transform.gameObject.SetActive(value: false);
				transform2.gameObject.SetActive(value: false);
			}
		}
		return gameObject;
	}

	private void ShowAnimation()
	{
		rewardAnimation.state.ClearTracks();
		Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.lootWheelPrizeFly);
		rewardAnimation.state.AddAnimation(0, rewardAnimationName, loop: false, 0f);
		rewardAnimation.state.End += OnAnimationEnd;
	}

	private void OnAnimationEnd(Spine.AnimationState state, int trackIndex)
	{
		if (icon != null)
		{
			rewardParticleSystem.transform.position = icon.transform.position;
			rewardParticleSystem.Play();
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.bonusBoxCollected);
			StartCoroutine(CoroutineRunner.DelayActionSequence(delegate
			{
				OnPressed();
			}, rewardParticleSystem.startLifetime + rewardParticleSystem.duration, realTime: false));
		}
	}

	private void OnAnimationEvent(Spine.AnimationState state, int trackIndex, Spine.Event e)
	{
		switch (e.Data.Name)
		{
		case "jump":
		{
			AudioSource[] rewardBounce = WPFMonoBehaviour.gameData.commonAudioCollection.rewardBounce;
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(rewardBounce[UnityEngine.Random.Range(0, rewardBounce.Length)]);
			break;
		}
		case "confetti2":
			confetti2ParticleSystem.Play();
			break;
		case "confetti1":
			confetti1ParticleSystem.Play();
			break;
		case "ground_hit":
		{
			AudioSource[] cratePillowHit = WPFMonoBehaviour.gameData.commonAudioCollection.cratePillowHit;
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(cratePillowHit[UnityEngine.Random.Range(0, cratePillowHit.Length)]);
			break;
		}
		}
	}

	private void ShowHorns(bool show)
	{
		horns.gameObject.SetActive(show);
		if (show)
		{
			StartCoroutine(ShowHorns());
		}
	}

	private IEnumerator ShowHorns()
	{
		for (int i = 0; i < hornRenderers.Length; i++)
		{
			hornRenderers[i].enabled = false;
		}
		for (int j = 0; j < hornAnimations.Length; j++)
		{
			hornAnimations[j].state.ClearTracks();
			hornAnimations[j].state.AddAnimation(0, hornAnimation, loop: false, 0f);
		}
		yield return null;
		for (int k = 0; k < hornRenderers.Length; k++)
		{
			hornRenderers[k].enabled = true;
		}
		yield return new WaitForSeconds(1f);
		Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.winTrumpet[0]);
	}

	private void HandleKeyReleased(KeyCode key)
	{
		if (key == KeyCode.Escape)
		{
			OnPressed();
		}
	}
}
