using System;
using UnityEngine;

public class SandboxSkullLevelButton : MonoBehaviour
{
	[SerializeField]
	private CollectableType collectableType;

	public string m_sandboxIdentifier;

	[SerializeField]
	private SandboxSelector m_sandboxSelector;

	[SerializeField]
	private TextMesh m_starsText;

	[SerializeField]
	private TextMesh m_Text;

	[SerializeField]
	private int m_Limit;

	private Transform starSet;

	private Transform collectSet;

	private void Start()
	{
		if (base.transform.parent != null)
		{
			m_sandboxSelector = base.transform.parent.GetComponent<SandboxSelector>();
		}
		starSet = base.transform.Find("StarSet");
		collectSet = base.transform.Find("Set");
		m_Limit = 10;
		bool isOdyssey = Singleton<BuildCustomizationLoader>.Instance.IsOdyssey;
		bool flag = false;
		string collectable = string.Empty;
		string arg = string.Empty;
		switch (collectableType)
		{
		case CollectableType.Statue:
			flag = GameProgress.SecretStatueCount() >= m_Limit || GameProgress.GetSandboxUnlocked(m_sandboxIdentifier);
			collectable = "Statues";
			arg = "[statue]";
			break;
		case CollectableType.Skull:
			flag = GameProgress.SecretSkullCount() >= m_Limit || GameProgress.GetSandboxUnlocked(m_sandboxIdentifier);
			collectable = "Skulls";
			arg = "[skull]";
			break;
		}
		GameProgress.ButtonUnlockState buttonUnlockState = GameProgress.GetButtonUnlockState("SandboxLevelButton_" + m_sandboxIdentifier);
		if (flag && buttonUnlockState == GameProgress.ButtonUnlockState.Locked && GetComponent<UnlockSandboxSequence>() == null)
		{
			base.gameObject.AddComponent<UnlockSandboxSequence>();
		}
		if (flag)
		{
			GetComponent<Button>().MethodToCall.SetMethod(m_sandboxSelector.gameObject.GetComponent<SandboxSelector>(), "LoadSandboxLevel", m_sandboxIdentifier);
			if (collectSet != null)
			{
				collectSet.gameObject.SetActive(value: false);
			}
			if (starSet != null)
			{
				starSet.gameObject.SetActive(value: true);
			}
			string text = WPFMonoBehaviour.gameData.m_sandboxLevels.GetLevelData(m_sandboxIdentifier).m_starBoxCount.ToString();
			m_starsText.text = GameProgress.SandboxStarCount(m_sandboxSelector.FindLevelFile(m_sandboxIdentifier)) + "/" + text;
			return;
		}
		if (starSet != null)
		{
			starSet.gameObject.SetActive(value: false);
		}
		int num = 0;
		switch (collectableType)
		{
		case CollectableType.Statue:
			num = GameProgress.SecretStatueCount();
			break;
		case CollectableType.Skull:
			num = GameProgress.SecretSkullCount();
			break;
		}
		m_Text.text = $"{arg} {num}/{m_Limit}";
		m_Text.SendMessage("TextUpdated", SendMessageOptions.DontRequireReceiver);
		if (isOdyssey)
		{
			TooltipInfo component = GetComponent<TooltipInfo>();
			if (component != null)
			{
				Button component2 = GetComponent<Button>();
				component2.MethodToCall.SetMethod(component, "Show");
				component2.Lock(lockState: false);
			}
			return;
		}
		string id = ((collectableType != CollectableType.Statue) ? "sandbox_unlock_skull_collectable" : "sandbox_unlock_statue_collectable");
		int num2 = Singleton<VirtualCatalogManager>.Instance.GetProductPrice(id);
		if (num2 <= 0)
		{
			num2 = 50;
		}
		int cost = (m_Limit - num) * num2;
		AddUnlockPopup(GetComponent<Button>(), m_sandboxIdentifier, collectable, num, m_Limit, cost, () => GameProgress.SnoutCoinCount() >= cost);
	}

	private void AddUnlockPopup(Button button, string levelName, string collectable, int collected, int required, int cost, Func<bool> requirements)
	{
		GameData gameData = Singleton<GameManager>.Instance.gameData;
		if (gameData.m_specialSandboxUnlockDialog != null && gameData.m_genericButtonPrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(gameData.m_specialSandboxUnlockDialog);
			SpecialSandboxUnlockDialog dialog = gameObject.GetComponent<SpecialSandboxUnlockDialog>();
			gameObject.transform.position = new Vector3(0f, 0f, -10f);
			button.MethodToCall.SetMethod(dialog, "Open");
			dialog.Type = (collectable.Equals("Skulls") ? SpecialSandboxUnlockDialog.UnlockType.Skull : SpecialSandboxUnlockDialog.UnlockType.Statue);
			dialog.Collected = collected;
			dialog.Required = required;
			dialog.Cost = cost;
			dialog.ShowConfirmEnabled = () => true;
			dialog.RebuildTexts();
			dialog.Close();
			CompactEpisodeTarget target = GetComponent<CompactEpisodeTarget>();
			dialog.SetOnConfirm(delegate
			{
				if (!GameProgress.GetSandboxUnlocked(levelName) && requirements() && GameProgress.UseSnoutCoins(cost))
				{
					GameProgress.SetSandboxUnlocked(levelName, unlocked: true);
					GameProgress.SetButtonUnlockState("SandboxLevelButton_" + levelName, GameProgress.ButtonUnlockState.Locked);
					target.SetAsLastTarget();
					Singleton<GameManager>.Instance.ReloadCurrentLevel(showLoadingScreen: true);
					EventManager.Connect<LevelLoadedEvent>(DelayedPurchaseSound);
					dialog.Close();
				}
				else if (!requirements() && Singleton<IapManager>.IsInstantiated())
				{
					dialog.Close();
					Singleton<IapManager>.Instance.OpenShopPage(dialog.Open, "SnoutCoinShop");
				}
				else
				{
					dialog.Close();
				}
			});
		}
		if (collectSet != null)
		{
			string text = $"[snout] {cost}";
			TextMesh[] componentsInChildren = collectSet.GetComponentsInChildren<TextMesh>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].text = text;
				TextMeshSpriteIcons.EnsureSpriteIcon(componentsInChildren[i]);
			}
		}
	}

	private void DelayedPurchaseSound(LevelLoadedEvent data)
	{
		EventManager.Disconnect<LevelLoadedEvent>(DelayedPurchaseSound);
		Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinUse);
	}
}
