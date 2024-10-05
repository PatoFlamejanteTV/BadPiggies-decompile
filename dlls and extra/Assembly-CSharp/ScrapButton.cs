using System;
using UnityEngine;

public class ScrapButton : SoftCurrencyButton
{
	[SerializeField]
	private float doubleRewardedPosition;

	[SerializeField]
	private float regularPosition;

	protected static ScrapButton instance;

	private int scrapCountInsideMachine;

	public static ScrapButton Instance => instance;

	protected override void ButtonAwake()
	{
		instance = this;
		if (!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
		{
			if (Singleton<DoubleRewardManager>.Instance.HasDoubleReward)
			{
				OnGainedDoubleReward();
			}
			else
			{
				OnDoubleRewardEnded();
			}
		}
	}

	private void OnDestroy()
	{
		if (Singleton<DoubleRewardManager>.IsInstantiated())
		{
			DoubleRewardManager doubleRewardManager = Singleton<DoubleRewardManager>.Instance;
			doubleRewardManager.OnAdWatched = (Action)Delegate.Remove(doubleRewardManager.OnAdWatched, new Action(OnGainedDoubleReward));
		}
	}

	protected override void ButtonEnabled()
	{
		EventManager.Connect<WorkshopMenu.CraftingMachineEvent>(OnCraftingMachineEvent);
		UpdateCount(GameProgress.GetInt("Machine_scrap_amount"));
		UpdateAmount(forceUpdate: true);
	}

	protected override void ButtonDisabled()
	{
		EventManager.Disconnect<WorkshopMenu.CraftingMachineEvent>(OnCraftingMachineEvent);
	}

	protected override int GetCurrencyCount()
	{
		return GameProgress.ScrapCount() - scrapCountInsideMachine;
	}

	public override AudioSource[] GetHitSounds()
	{
		return WPFMonoBehaviour.gameData.commonAudioCollection.nutHit;
	}

	public override AudioSource[] GetFlySounds()
	{
		return WPFMonoBehaviour.gameData.commonAudioCollection.nutFly;
	}

	protected override AudioSource GetLoopSound()
	{
		return null;
	}

	protected override void OnUpdate()
	{
	}

	private void OnCraftingMachineEvent(WorkshopMenu.CraftingMachineEvent data)
	{
		WorkshopMenu.CraftingMachineAction action = data.action;
		if (action == WorkshopMenu.CraftingMachineAction.RemoveScrap || action == WorkshopMenu.CraftingMachineAction.AddScrap || action == WorkshopMenu.CraftingMachineAction.ResetScrap)
		{
			UpdateCount(data.scrapAmountInMachine);
		}
	}

	private void UpdateCount(int insideMachineAmount)
	{
		scrapCountInsideMachine = insideMachineAmount;
		UpdateAmount();
	}

	public void OpenWorkshop()
	{
	}

	private void OnDoubleRewardEnded()
	{
		DoubleRewardManager doubleRewardManager = Singleton<DoubleRewardManager>.Instance;
		doubleRewardManager.OnAdWatched = (Action)Delegate.Combine(doubleRewardManager.OnAdWatched, new Action(OnGainedDoubleReward));
	}

	private void OnGainedDoubleReward()
	{
		DoubleRewardManager doubleRewardManager = Singleton<DoubleRewardManager>.Instance;
		doubleRewardManager.OnAdWatched = (Action)Delegate.Remove(doubleRewardManager.OnAdWatched, new Action(OnGainedDoubleReward));
	}

	private void UpdatePlacement(float relativePosition)
	{
	}
}
