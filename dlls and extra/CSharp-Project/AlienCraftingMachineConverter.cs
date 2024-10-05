using System;
using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;

public class AlienCraftingMachineConverter : WPFMonoBehaviour
{
	public Action OnBeginUpgrade;

	public Action OnEndUpgrade;

	public Action OnMachineBehindCurtain;

	[SerializeField]
	private SkeletonAnimation m_curtainAnimation;

	[SerializeField]
	private string m_curtainIntroAnimationName;

	[SerializeField]
	private string m_curtainOutroAnimationName;

	[SerializeField]
	private SkeletonAnimation m_craftingMachineAnimation;

	[SerializeField]
	private string m_alienSkinName;

	[SerializeField]
	private CustomizationsFullCheck customizationsCheck;

	[SerializeField]
	private GameObject normalScrapCounter;

	[SerializeField]
	private GameObject alienScrapCounter;

	[SerializeField]
	private GameObject alienPartSilhouette;

	[SerializeField]
	private GameObject[] m_collectIndicators;

	[SerializeField]
	private ParticleSystem dustParticles;

	private Renderer[] m_curtainRenderers;

	private bool m_showingRoutine;

	private bool m_isAlienMachine;

	public bool RoutineShown
	{
		get
		{
			return GameProgress.GetBool("AlienCraftingMachineShown");
		}
		set
		{
			GameProgress.SetBool("AlienCraftingMachineShown", value);
		}
	}

	public bool ShowingRoutine => m_showingRoutine;

	public bool IsAlienMachine
	{
		get
		{
			if (customizationsCheck.AllCommon && customizationsCheck.AllRare)
			{
				return customizationsCheck.AllEpic;
			}
			return false;
		}
	}

	private void Awake()
	{
		m_curtainRenderers = m_curtainAnimation.gameObject.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < m_curtainRenderers.Length; i++)
		{
			m_curtainRenderers[i].enabled = false;
		}
	}

	private void Start()
	{
		if (IsAlienMachine && !RoutineShown)
		{
			EventManager.Connect<WorkshopMenu.CraftingMachineEvent>(OnCraftingMachineEvent);
		}
		EventManager.Connect<UIEvent>(OnUIEvenet);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<WorkshopMenu.CraftingMachineEvent>(OnCraftingMachineEvent);
		EventManager.Disconnect<UIEvent>(OnUIEvenet);
	}

	[ContextMenu("Upgrade to alien machine")]
	private void ContextMenuShow()
	{
		StartCoroutine(Show());
	}

	private void OnCraftingMachineEvent(WorkshopMenu.CraftingMachineEvent data)
	{
		if (data.action == WorkshopMenu.CraftingMachineAction.Idle)
		{
			EventManager.Disconnect<WorkshopMenu.CraftingMachineEvent>(OnCraftingMachineEvent);
			Check();
		}
	}

	private void OnUIEvenet(UIEvent data)
	{
		if (data.type == UIEvent.Type.ClosedLootWheel)
		{
			Check();
		}
	}

	public void Check()
	{
		if (IsAlienMachine && !RoutineShown)
		{
			StartCoroutine(Show());
		}
	}

	private IEnumerator Show()
	{
		m_showingRoutine = true;
		if (OnBeginUpgrade != null)
		{
			OnBeginUpgrade();
		}
		m_curtainAnimation.state.End += OnIntroEnd;
		m_curtainAnimation.state.SetAnimation(0, m_curtainIntroAnimationName, loop: false);
		yield return null;
		for (int i = 0; i < m_curtainRenderers.Length; i++)
		{
			m_curtainRenderers[i].enabled = true;
		}
	}

	private void OnIntroEnd(Spine.AnimationState state, int trackIndex)
	{
		m_curtainAnimation.state.End -= OnIntroEnd;
		AudioSource craftPart = WPFMonoBehaviour.gameData.commonAudioCollection.craftPart;
		Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(craftPart);
		dustParticles.Play();
		ConvertToAlien();
		if (OnMachineBehindCurtain != null)
		{
			OnMachineBehindCurtain();
		}
		CoroutineRunner.Instance.DelayAction(delegate
		{
			dustParticles.Stop();
			m_curtainAnimation.state.End += OnOutroEnd;
			m_curtainAnimation.state.SetAnimation(0, m_curtainOutroAnimationName, loop: false);
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.alienMachineReveal);
		}, craftPart.clip.length, realTime: false);
	}

	public void ConvertToAlien()
	{
		m_craftingMachineAnimation.initialSkinName = m_alienSkinName;
		m_craftingMachineAnimation.Initialize(overwrite: true);
		alienScrapCounter.SetActive(value: true);
		normalScrapCounter.SetActive(value: false);
		alienPartSilhouette.SetActive(value: true);
		for (int i = 0; i < m_collectIndicators.Length; i++)
		{
			m_collectIndicators[i].SetActive(value: false);
		}
	}

	private void OnOutroEnd(Spine.AnimationState state, int trackIndex)
	{
		m_curtainAnimation.state.End -= OnOutroEnd;
		m_isAlienMachine = true;
		RoutineShown = true;
		m_showingRoutine = false;
		if (OnEndUpgrade != null)
		{
			OnEndUpgrade();
		}
	}
}
