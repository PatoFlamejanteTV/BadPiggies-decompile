using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public class LootCrateButton : Widget
{
	[SerializeField]
	private string[] skinNames;

	[SerializeField]
	private GameObject pointerPrefab;

	[SerializeField]
	private GameObject clickPrefab;

	[SerializeField]
	private Transform tutorialTarget;

	[SerializeField]
	private GameObject pillow;

	[SerializeField]
	private GameObject[] lootRewardPrefabs;

	[SerializeField]
	private GameObject backfacePrefab;

	[SerializeField]
	private SkeletonAnimation skeletonAnimation;

	[SerializeField]
	private float tutorialDelay;

	[SerializeField]
	private string introAnimationName;

	[SerializeField]
	private string idleAnimationName;

	[SerializeField]
	private string[] hitAnimationNames;

	[SerializeField]
	private string[] openAnimationNames;

	[SerializeField]
	private int maxRewardCount = 8;

	[SerializeField]
	private ParticleSystem explosionEffect;

	[SerializeField]
	private ParticleSystem bounceEffect;

	[SerializeField]
	private AudioSource[] openLaugh;

	private Transform[] iconRoots;

	private Transform[] backfaceRoots;

	private List<LootRewardElement> rewardElements;

	private Tutorial.Pointer pointer;

	private Tutorial.PointerTimeLine tutorial;

	private bool tutorialRunning;

	private bool canBeHit;

	private int hitCounter;

	private int openIndex = -1;

	private Collider collider;

	private LootCrateType lootCrateType;

	private GameData gameData;

	public Action onOpeningDone;

	public Action onOpeningStart;

	public int GainedXP { get; set; }

	private void Awake()
	{
		gameData = WPFMonoBehaviour.gameData;
		rewardElements = new List<LootRewardElement>();
		skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
		collider = GetComponent<Collider>();
		string format = "Root/SkeletonUtility-Root/root/Item{0}_Adjustment/Item{0}/Item{0}_Socket{1}";
		iconRoots = new Transform[maxRewardCount];
		backfaceRoots = new Transform[maxRewardCount];
		for (int i = 1; i <= maxRewardCount; i++)
		{
			Transform transform = base.transform.Find(string.Format(format, i, string.Empty));
			Transform transform2 = base.transform.Find(string.Format(format, i, 2));
			iconRoots[i - 1] = transform;
			backfaceRoots[i - 1] = transform2;
		}
		if (backfacePrefab != null && backfaceRoots != null && backfaceRoots.Length != 0)
		{
			for (int j = 0; j < backfaceRoots.Length; j++)
			{
				GameObject obj = UnityEngine.Object.Instantiate(backfacePrefab);
				obj.transform.parent = backfaceRoots[j];
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localScale = backfacePrefab.transform.localScale;
			}
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(pointerPrefab);
		GameObject gameObject2 = UnityEngine.Object.Instantiate(clickPrefab);
		gameObject.layer = base.gameObject.layer;
		gameObject2.layer = base.gameObject.layer;
		gameObject.GetComponentInChildren<Renderer>().sortingLayerName = "Popup";
		gameObject2.GetComponentInChildren<Renderer>().sortingLayerName = "Popup";
		Tutorial.SetOrderInLayer(gameObject, 3002);
		Tutorial.SetOrderInLayer(gameObject2, 3002);
		pointer = new Tutorial.Pointer(gameObject, gameObject2);
		pointer.Show(show: false);
	}

	private void OnDestroy()
	{
		skeletonAnimation.state.Event -= OnAnimationEvent;
	}

	private void Start()
	{
		canBeHit = false;
	}

	public void Init(LootCrateType crateType)
	{
		openIndex = -1;
		hitCounter = 0;
		collider.enabled = true;
		lootCrateType = crateType;
		int num = (int)lootCrateType;
		skeletonAnimation.initialSkinName = skinNames[num];
		skeletonAnimation.Initialize(overwrite: true);
		skeletonAnimation.state.Event -= OnAnimationEvent;
		skeletonAnimation.state.Event += OnAnimationEvent;
	}

	public void PlayIntro()
	{
		skeletonAnimation.state.SetAnimation(0, introAnimationName, loop: false);
		skeletonAnimation.state.Complete += OnIntroEnd;
	}

	private void OnIntroEnd(Spine.AnimationState state, int trackIndex, int loopCount)
	{
		skeletonAnimation.state.Complete -= OnIntroEnd;
		skeletonAnimation.state.SetAnimation(0, idleAnimationName, loop: true);
		canBeHit = true;
		tutorial = CreateTutorial();
		StartCoroutine(TutorialCoroutine());
	}

	protected override void OnInput(InputEvent input)
	{
		if (!canBeHit || input.type != 0)
		{
			return;
		}
		if (hitAnimationNames != null && hitCounter < hitAnimationNames.Length)
		{
			if (tutorialRunning)
			{
				StopAllCoroutines();
			}
			StartCoroutine(TutorialCoroutine());
			skeletonAnimation.state.SetAnimation(0, hitAnimationNames[hitCounter], loop: false);
			PlayHitSound(hitCounter);
			hitCounter++;
		}
		else if (openAnimationNames != null)
		{
			if (openIndex < 0 || openIndex >= openAnimationNames.Length)
			{
				openIndex = 0;
			}
			skeletonAnimation.state.SetAnimation(0, openAnimationNames[openIndex], loop: false);
			PlayHitSound(hitCounter);
			skeletonAnimation.state.End += OnOpenEnd;
			tutorialRunning = false;
			canBeHit = false;
			collider.enabled = false;
			if (onOpeningStart != null)
			{
				onOpeningStart();
			}
		}
		else if (onOpeningDone != null)
		{
			onOpeningDone();
		}
	}

	private void OnOpenEnd(Spine.AnimationState state, int trackIndex)
	{
		skeletonAnimation.state.End -= OnOpenEnd;
		if (onOpeningDone != null)
		{
			onOpeningDone();
		}
		StartCoroutine(ScrapDuplicateParts());
	}

	private IEnumerator ScrapDuplicateParts()
	{
		int index = 0;
		foreach (LootRewardElement element in rewardElements)
		{
			if (element.IsDuplicatePart)
			{
				yield return element.WaitJumpAnimation();
				element.PlayScrapAnimation();
				float fade = 2f;
				while (fade > 0f)
				{
					fade -= GameTime.RealTimeDelta;
					yield return null;
				}
				if (element.BlingEffect != null)
				{
					element.BlingEffect.Stop();
				}
				for (int num = iconRoots[index].childCount - 1; num >= 0; num--)
				{
					iconRoots[index].GetChild(num).gameObject.SetActive(value: true);
				}
			}
			index++;
		}
	}

	public void SetScrapIcon(int index, GameObject iconPrefab, string label)
	{
		if (index >= 0 && index < rewardElements.Count)
		{
			GameObject gameObject = SetIcon(index, iconPrefab, label, 0, isDuplicatePart: false, addToRewardsList: false);
			if (gameObject != null)
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	public GameObject SetIcon(int index, GameObject iconPrefab, string label = "", int lootRewardPrefabIndex = 0, bool isDuplicatePart = false, bool addToRewardsList = true)
	{
		if (iconRoots == null || index < 0 || index >= iconRoots.Length || iconPrefab == null || lootRewardPrefabs == null || lootRewardPrefabIndex < 0 || lootRewardPrefabIndex >= lootRewardPrefabs.Length)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(lootRewardPrefabs[lootRewardPrefabIndex]);
		gameObject.transform.parent = iconRoots[index];
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = lootRewardPrefabs[lootRewardPrefabIndex].transform.localScale;
		LootRewardElement component = gameObject.GetComponent<LootRewardElement>();
		if (component != null && addToRewardsList)
		{
			component.IsDuplicatePart = isDuplicatePart;
			component.InitElement(this);
			rewardElements.Add(component);
		}
		Transform transform = gameObject.transform.Find("Label");
		if ((bool)transform)
		{
			if (string.IsNullOrEmpty(label))
			{
				transform.gameObject.SetActive(value: false);
			}
			else
			{
				TextMesh component2 = transform.GetComponent<TextMesh>();
				if ((bool)component2)
				{
					component2.text = label;
				}
			}
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(iconPrefab);
		if (component != null)
		{
			gameObject2.transform.parent = component.IconRoot;
		}
		else
		{
			gameObject2.transform.parent = gameObject.transform;
		}
		gameObject2.transform.localPosition = -Vector3.forward * 0.5f;
		gameObject2.transform.localScale = Vector3.one;
		Renderer[] componentsInChildren = gameObject2.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sortingLayerName = "Popup";
			componentsInChildren[i].sortingOrder = 1;
		}
		if (addToRewardsList)
		{
			openIndex++;
		}
		return gameObject;
	}

	private Tutorial.PointerTimeLine CreateTutorial()
	{
		Tutorial.PointerTimeLine pointerTimeLine = new Tutorial.PointerTimeLine(pointer);
		List<Vector3> positions = new List<Vector3>
		{
			tutorialTarget.position + 21f * Vector3.down,
			tutorialTarget.position
		};
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Wait(0.1f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Move(positions, 2.5f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Press());
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Wait(0.5f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Release());
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Wait(0.75f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Hide());
		return pointerTimeLine;
	}

	private IEnumerator TutorialCoroutine()
	{
		float time = 0f;
		tutorialRunning = true;
		pointer.Show(show: false);
		while (time < tutorialDelay)
		{
			time += Time.unscaledDeltaTime;
			yield return null;
		}
		tutorial.Start();
		while (tutorialRunning)
		{
			if (!tutorial.IsFinished())
			{
				tutorial.Update();
			}
			else
			{
				tutorial.Start();
			}
			yield return null;
		}
		pointer.Show(show: false);
	}

	private void PlayHitSound(int index)
	{
		AudioSource audioSource = null;
		switch (lootCrateType)
		{
		case LootCrateType.Wood:
		case LootCrateType.Cardboard:
			audioSource = gameData.commonAudioCollection.woodenCrateHits[index];
			break;
		case LootCrateType.Metal:
		case LootCrateType.Bronze:
			audioSource = gameData.commonAudioCollection.metalCrateHits[index];
			break;
		case LootCrateType.Gold:
			audioSource = gameData.commonAudioCollection.goldenCrateHits[index];
			break;
		case LootCrateType.Glass:
			audioSource = gameData.commonAudioCollection.glassCrateHits[index];
			break;
		case LootCrateType.Marble:
			audioSource = gameData.commonAudioCollection.marbleCrateHits[index];
			break;
		}
		if (audioSource != null)
		{
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(audioSource);
		}
	}

	private void OnAnimationEvent(Spine.AnimationState state, int trackIndex, Spine.Event e)
	{
		switch (e.Data.Name)
		{
		case "Item_Bounce":
		{
			AudioSource[] rewardBounce = gameData.commonAudioCollection.rewardBounce;
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(rewardBounce[UnityEngine.Random.Range(0, rewardBounce.Length)]);
			if ((bool)bounceEffect)
			{
				bounceEffect.Emit(1);
			}
			break;
		}
		case "Crate_Explosion":
		{
			if ((bool)explosionEffect)
			{
				explosionEffect.Emit(5);
			}
			AudioSource audioSource3 = Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(openLaugh[UnityEngine.Random.Range(0, openLaugh.Length)]);
			if (audioSource3 != null)
			{
				audioSource3.transform.position = WPFMonoBehaviour.hudCamera.transform.position;
			}
			break;
		}
		case "Crate_Drop":
		{
			AudioSource[] cratePillowHit = gameData.commonAudioCollection.cratePillowHit;
			AudioSource audioSource2 = Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(cratePillowHit[UnityEngine.Random.Range(0, cratePillowHit.Length)]);
			if (audioSource2 != null)
			{
				audioSource2.transform.position = WPFMonoBehaviour.hudCamera.transform.position;
			}
			if ((bool)PlayerProgressBar.Instance && (bool)pillow && GainedXP > 0)
			{
				PlayerProgressBar.Instance.AddParticles(pillow, GainedXP);
			}
			break;
		}
		case "Pillow_Drop":
		{
			AudioSource[] pillowDrops = gameData.commonAudioCollection.pillowDrops;
			AudioSource audioSource = Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(pillowDrops[UnityEngine.Random.Range(0, pillowDrops.Length)]);
			if (audioSource != null)
			{
				audioSource.transform.position = WPFMonoBehaviour.hudCamera.transform.position;
			}
			break;
		}
		}
	}

	private IEnumerator DelayedPlay(SkeletonAnimation target, string animation, float delay)
	{
		for (float counter = 0f; counter < delay; counter += Time.unscaledDeltaTime)
		{
			yield return null;
		}
		target.state.SetAnimation(0, animation, loop: false);
	}
}
