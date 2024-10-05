using UnityEngine;

public class OnPurchaseSucceeded : WPFMonoBehaviour
{
	public InGameBuildMenu buildMenu;

	[SerializeField]
	private IapManager.InAppPurchaseItemType[] items;

	[SerializeField]
	private GameObject particlesPrefab;

	private ParticleSystem particles;

	private void Awake()
	{
		GameObject gameObject = Object.Instantiate(particlesPrefab);
		particles = gameObject.GetComponent<ParticleSystem>();
		particles.Stop();
		IapManager.onPurchaseSucceeded += OnPurchase;
	}

	private void OnDestroy()
	{
		IapManager.onPurchaseSucceeded -= OnPurchase;
	}

	private void OnPurchase(IapManager.InAppPurchaseItemType item)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i] == item)
			{
				particles.transform.position = base.transform.position;
				particles.Play();
				Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinPurchase);
				if (buildMenu != null)
				{
					buildMenu.RefreshPowerUpCounts();
				}
				break;
			}
		}
	}
}
