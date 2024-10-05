using System;
using System.Collections;
using UnityEngine;

public class RaceLevelButton : MonoBehaviour
{
	public string m_raceLevelIdentifier;

	public int m_levelNumber;

	[SerializeField]
	private RaceLevelSelector m_raceLevelSelector;

	[SerializeField]
	private TextMesh m_starsText;

	[SerializeField]
	private AnimationClip shake;

	private void Start()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = GameProgress.GetRaceLevelUnlocked(m_raceLevelIdentifier) || GameProgress.AllLevelsUnlocked();
		bool flag4 = LevelInfo.IsContentLimited(-1, m_levelNumber);
		bool flag5 = GameProgress.IsLevelAdUnlocked(m_raceLevelIdentifier);
		int levelIndex = m_raceLevelSelector.m_raceLevels.GetLevelIndex(m_raceLevelIdentifier);
		if (levelIndex > 0)
		{
			RaceLevels.LevelData levelData = m_raceLevelSelector.m_raceLevels.Levels[levelIndex - 1];
			int @int = GameProgress.GetInt(levelData.SceneName + "_stars");
			if (GameProgress.GetRaceLevelUnlocked(levelData.m_identifier) && @int > 0)
			{
				flag = true;
			}
			if (GameProgress.GetRaceLevelUnlocked(levelData.m_identifier) || GameProgress.AllLevelsUnlocked())
			{
				flag2 = true;
			}
		}
		if (!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey && flag2 && !flag3)
		{
			int cost = Singleton<VirtualCatalogManager>.Instance.GetProductPrice("road_hogs_level_unlock");
			AddRoadHogsUnlockDialog(GetComponent<Button>(), m_raceLevelIdentifier, cost, () => GameProgress.SnoutCoinCount() >= cost);
		}
		else if (!flag3)
		{
			base.gameObject.AddComponent<Animation>().AddClip(shake, shake.name);
			ButtonAnimation buttonAnimation = base.gameObject.AddComponent<ButtonAnimation>();
			buttonAnimation.PlayWholeAnimation = true;
			buttonAnimation.ActivateAnimationName = shake.name;
			base.gameObject.AddComponent<InactiveButton>();
		}
		if (!flag4 && flag && !flag3)
		{
			GameProgress.SetRaceLevelUnlocked(m_raceLevelIdentifier, unlocked: true);
		}
		if (flag4 && (flag || flag2 || flag5))
		{
			flag4 = false;
		}
		GameProgress.ButtonUnlockState buttonUnlockState = GameProgress.GetButtonUnlockState("RaceLevelButton_" + m_raceLevelIdentifier);
		if (flag3 && buttonUnlockState == GameProgress.ButtonUnlockState.Locked && !flag4)
		{
			StartCoroutine(UnlockNowSequence());
		}
		if ((flag3 && !flag4) || !base.transform.Find("Lock"))
		{
			Button component = GetComponent<Button>();
			component.MethodToCall.SetMethod(m_raceLevelSelector.gameObject.GetComponent<RaceLevelSelector>(), "LoadRaceLevel", m_raceLevelIdentifier);
			string sceneName = WPFMonoBehaviour.gameData.FindRaceLevel(m_raceLevelIdentifier).SceneName;
			int int2 = GameProgress.GetInt(m_raceLevelSelector.FindLevelFile(m_raceLevelIdentifier) + "_stars");
			bool flag6 = GameProgress.HasCollectedSnoutCoins(sceneName, 0);
			bool flag7 = GameProgress.HasCollectedSnoutCoins(sceneName, 1);
			bool flag8 = GameProgress.HasCollectedSnoutCoins(sceneName, 2);
			GameObject[] array = new GameObject[6]
			{
				component.transform.Find("StarSet/Star1").gameObject,
				component.transform.Find("StarSet/Star2").gameObject,
				component.transform.Find("StarSet/Star3").gameObject,
				component.transform.Find("CoinSet/Star1").gameObject,
				component.transform.Find("CoinSet/Star2").gameObject,
				component.transform.Find("CoinSet/Star3").gameObject
			};
			int num = 0;
			if (flag6)
			{
				num++;
			}
			if (flag7)
			{
				num++;
			}
			if (flag8)
			{
				num++;
			}
			for (int i = 0; i < 3; i++)
			{
				bool flag9 = i + 1 <= int2;
				bool flag10 = i + 1 <= num || Singleton<BuildCustomizationLoader>.Instance.IsOdyssey;
				array[i].SetActive(flag9 && !flag10);
				array[i + 3].SetActive(flag9 && flag10);
			}
			string sceneName2 = m_raceLevelSelector.m_raceLevels.GetLevelData(m_raceLevelIdentifier).SceneName;
			if (GameProgress.HasBestTime(sceneName2))
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.Clamp(GameProgress.GetBestTime(sceneName2), 0f, 3599.99f));
				base.transform.Find("BestTime").GetComponent<TextMesh>().text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds / 10:D2}";
				base.transform.Find("BestTime").gameObject.SetActive(value: true);
				base.transform.Find("TimeBG").gameObject.SetActive(value: true);
			}
		}
		else
		{
			base.transform.Find("StarSet").gameObject.SetActive(value: false);
			base.transform.Find("CoinSet").gameObject.SetActive(value: false);
			base.transform.Find("BestTime").gameObject.SetActive(value: false);
			base.transform.Find("TimeBG").gameObject.SetActive(value: false);
		}
	}

	private void AddRoadHogsUnlockDialog(Button button, string levelIdentifier, int price, Func<bool> requirements)
	{
		GameData gameData = Singleton<GameManager>.Instance.gameData;
		if (!(gameData.m_roadHogsUnlockDialog != null) || !(gameData.m_genericButtonPrefab != null))
		{
			return;
		}
		base.transform.Find("Finger").gameObject.GetComponent<Renderer>().enabled = true;
		GameObject gameObject = UnityEngine.Object.Instantiate(gameData.m_roadHogsUnlockDialog);
		gameObject.transform.position = new Vector3(0f, 0f, -15f);
		GameObject obj = UnityEngine.Object.Instantiate(gameData.m_genericButtonPrefab);
		obj.transform.parent = base.transform;
		obj.transform.localPosition = Vector3.zero + new Vector3(0f, 0f, -1f);
		obj.transform.localRotation = Quaternion.identity;
		obj.GetComponent<BoxCollider>().size = base.gameObject.GetComponent<BoxCollider>().size;
		TextDialog dialog = gameObject.GetComponent<TextDialog>();
		button.MethodToCall.SetMethod(dialog, "Open");
		obj.GetComponent<Button>().MethodToCall.SetMethod(dialog, "Open");
		dialog.ConfirmButtonText = $"[snout] {price}";
		dialog.ShowConfirmEnabled = () => true;
		dialog.Close();
		dialog.SetOnConfirm(delegate
		{
			if (!GameProgress.GetRaceLevelUnlocked(levelIdentifier) && !GameProgress.IsLevelAdUnlocked(levelIdentifier) && requirements() && GameProgress.UseSnoutCoins(price))
			{
				GameProgress.SetRaceLevelUnlocked(levelIdentifier, unlocked: true);
				GameProgress.SetButtonUnlockState("RaceLevelButton_" + levelIdentifier, GameProgress.ButtonUnlockState.Locked);
				Singleton<GameManager>.Instance.ReloadCurrentLevel(showLoadingScreen: true);
				EventManager.Connect<LevelLoadedEvent>(DelayedPurchaseSound);
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

	private void DelayedPurchaseSound(LevelLoadedEvent data)
	{
		EventManager.Disconnect<LevelLoadedEvent>(DelayedPurchaseSound);
		Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.snoutCoinUse);
	}

	private IEnumerator UnlockNowSequence()
	{
		yield return null;
		GameProgress.UnlockButton("RaceLevelButton_" + m_raceLevelIdentifier);
		base.transform.Find("Lock").GetComponent<ButtonLock>().NotifyUnlocked();
	}
}
