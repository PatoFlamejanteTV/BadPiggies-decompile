using UnityEngine;

public class SnoutButton : SoftCurrencyButton
{
	protected static SnoutButton instance;

	public static SnoutButton Instance => instance;

	protected override void ButtonAwake()
	{
		instance = this;
	}

	protected override void ButtonEnabled()
	{
		IapManager.onPurchaseSucceeded += OnPurchase;
	}

	protected override void ButtonDisabled()
	{
		IapManager.onPurchaseSucceeded -= OnPurchase;
	}

	protected override int GetCurrencyCount()
	{
		return GameProgress.SnoutCoinCount();
	}

	public override AudioSource[] GetHitSounds()
	{
		return WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinHit;
	}

	public override AudioSource[] GetFlySounds()
	{
		return WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinFly;
	}

	protected override AudioSource GetLoopSound()
	{
		return null;
	}

	protected override void OnUpdate()
	{
	}

	private void OnPurchase(IapManager.InAppPurchaseItemType type)
	{
		UpdateAmount();
		if (!Singleton<IapManager>.Instance.SnoutCoinPurchasable(type))
		{
			PlayPurchaseSound();
		}
	}

	private void PlayPurchaseSound()
	{
		if (!(WPFMonoBehaviour.gameData == null) && !(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinPurchase == null))
		{
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinPurchase);
		}
	}

	public void OpenSnoutCoinPopup()
	{
		Singleton<IapManager>.Instance.OpenShopPage(base.RecoverToPreviousPosition, "SnoutCoinShop");
	}

	public override void AddParticles(GameObject target, int amount, float delay = 0f, float burstRate = 0f)
	{
		if (!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
		{
			base.AddParticles(target, amount, delay, burstRate);
		}
	}

	public override void ShowButton(bool show = true)
	{
		ResourceBar.Instance.ShowItem(ResourceBar.Item.SnoutCoin, !Singleton<BuildCustomizationLoader>.Instance.IsOdyssey && show);
	}

	private void OnShowButton()
	{
		UpdateAmount(forceUpdate: true);
	}
}
