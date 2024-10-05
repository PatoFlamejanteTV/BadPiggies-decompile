using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RewardDialog : MonoBehaviour
{
	private const string REWARD_PREFAB = "UI/Amazon/DailyReward";

	private const float OFFSET = 6.8f;

	private const float SCALING_WIDTH = 11f;

	private const string LOC_REWARD_FIRST_DAY = "REWARD_FIRST_DAY";

	private const string LOC_REWARD_DAY = "REWARD_DAY";

	private const string LOC_REWARD_CLAIM = "REWARD_CLAIM";

	private const string LOC_REWARD_LATER = "REWARD_LATER";

	private const string LOC_REWARD_ERROR = "REWARD_ERROR";

	public readonly string BundleRewardInfo = "IAP_BUNDLES_PAGE_TEXT";

	public readonly Dictionary<PrizeType, string> RewardInfos = new Dictionary<PrizeType, string>
	{
		{
			PrizeType.SuperGlue,
			"IAP_SUPER_GLUE_PAGE_TEXT"
		},
		{
			PrizeType.SuperMagnet,
			"IAP_SUPER_MAGNET_PAGE_TEXT"
		},
		{
			PrizeType.SuperMechanic,
			"IAP_SUPER_BLUEPRINT_PAGE_TEXT"
		},
		{
			PrizeType.TurboCharge,
			"IAP_TURBO_CHARGE_PAGE_TEXT"
		},
		{
			PrizeType.NightVision,
			"IAP_NIGHTVISION_PAGE_TEXT"
		}
	};

	[SerializeField]
	private Transform rewardContainer;

	[SerializeField]
	private Transform mainMenu;

	[SerializeField]
	private Transform closeButton;

	[SerializeField]
	private float translationTime;

	[SerializeField]
	private Transform loadingIcon;

	[SerializeField]
	private AnimationCurve translationCurve;

	[SerializeField]
	private AnimationCurve scalingCurve;

	[SerializeField]
	private AnimationCurve giveRewardCurve;

	[SerializeField]
	private GameObject infoText;

	[SerializeField]
	private GameObject claimButtonText;

	[SerializeField]
	private Renderer claimButtonRenderer;

	[SerializeField]
	private Sprite claimButtonSprite;

	[SerializeField]
	private string claimButtonActiveSprite;

	[SerializeField]
	private string claimButtonInactiveSprite;

	[SerializeField]
	private TextMesh[] dayTexts;

	[SerializeField]
	private TextMesh errorText;

	[SerializeField]
	private Transform centerDayBG;

	[SerializeField]
	private Renderer hideFirstDayMask;

	[SerializeField]
	private CustomSpritePanel panel;

	[SerializeField]
	private GameObject collectSound;

	[SerializeField]
	private GameObject moveSound;

	private TextMesh infoTxt;

	private TextMesh claimBtnTxt;

	private Transform[] dailyRewards;

	private bool claimRewardSequencePlaying;

	private bool waitingToClaimReward;

	private bool timedOut;

	private float waitTime = 15f;

	private StringBuilder sb;

	private int lastTimeUpdated;

	private void Awake()
	{
		infoTxt = infoText.GetComponent<TextMesh>();
		infoTxt.text = string.Empty;
		claimBtnTxt = claimButtonText.GetComponent<TextMesh>();
		sb = new StringBuilder();
	}

	private void UpdateTextMeshLocale(TextMesh textMesh, string localeKey, int maxRowCharacters = -1, string postfix = "")
	{
		if (textMesh == null)
		{
			return;
		}
		textMesh.text = localeKey;
		TextMeshLocale component = textMesh.GetComponent<TextMeshLocale>();
		if (component != null)
		{
			component.RefreshTranslation();
			if (TextMeshHelper.UsesKanjiCharacters() && localeKey.Equals("REWARD_DAY"))
			{
				component.Postfix = string.Empty;
				textMesh.text = string.Format(textMesh.text, postfix.Replace(" ", string.Empty));
			}
			else
			{
				component.Postfix = ((!string.IsNullOrEmpty(postfix)) ? postfix : string.Empty);
			}
		}
		TextMeshHelper.Wrap(textMesh, maxRowCharacters);
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			UpdateTextMeshLocale(errorText, "REWARD_ERROR", 24, string.Empty);
		}
	}

	private void Start()
	{
		PopulateDailyRewards();
		UpdateScales();
		base.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		Singleton<GuiManager>.Instance.GrabPointer(this);
		Singleton<KeyListener>.Instance.GrabFocus(this);
		KeyListener.keyReleased += HandleKeyReleased;
		if (!Singleton<RewardSystem>.IsInstantiated())
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			StartCoroutine(OnEnableRoutine());
		}
	}

	private void OnDisable()
	{
		RewardSystem.FreezeResetTime = false;
		StopAllCoroutines();
		if (Singleton<GuiManager>.Instance != null)
		{
			Singleton<GuiManager>.Instance.ReleasePointer(this);
		}
		if (Singleton<KeyListener>.Instance != null)
		{
			Singleton<KeyListener>.Instance.ReleaseFocus(this);
		}
		KeyListener.keyReleased -= HandleKeyReleased;
	}

	private IEnumerator OnEnableRoutine()
	{
		loadingIcon.gameObject.SetActive(value: true);
		rewardContainer.gameObject.SetActive(value: false);
		infoTxt.text = string.Empty;
		claimButtonRenderer.gameObject.SetActive(value: false);
		closeButton.gameObject.SetActive(value: false);
		errorText.GetComponent<Renderer>().enabled = false;
		hideFirstDayMask.enabled = true;
		timedOut = false;
		Singleton<RewardSystem>.Instance.RefreshData();
		panel.width = 16f;
		SetDayTexts(2);
		yield return null;
		SetDayTexts(-1);
		centerDayBG.localScale = Vector3.one + Vector3.right * 0.8f;
		float timeLeftToShowCloseButton = 2f;
		bool isShowingCloseButton = false;
		bool canShowCloseButton = true;
		float timeCounter = 0f;
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			timeCounter = waitTime;
		}
		while (!Singleton<RewardSystem>.Instance.HasTime)
		{
			timeLeftToShowCloseButton -= Time.unscaledDeltaTime;
			if (!isShowingCloseButton && timeLeftToShowCloseButton <= 0f)
			{
				isShowingCloseButton = false;
				closeButton.gameObject.SetActive(value: true);
			}
			timeCounter += Time.unscaledDeltaTime;
			if (timeCounter > waitTime)
			{
				errorText.GetComponent<Renderer>().enabled = true;
				closeButton.gameObject.SetActive(value: true);
				loadingIcon.gameObject.SetActive(value: false);
				timedOut = true;
				yield return null;
				UpdateTextMeshLocale(errorText, "REWARD_ERROR", 24, string.Empty);
				yield break;
			}
			yield return null;
		}
		if (Singleton<RewardSystem>.Instance.IsRewardReady())
		{
			ReadyToClaim();
		}
		else
		{
			claimBtnTxt.text = string.Empty;
		}
		if (claimRewardSequencePlaying || waitingToClaimReward)
		{
			canShowCloseButton = false;
			closeButton.gameObject.SetActive(value: false);
		}
		RewardSystem.FreezeResetTime = true;
		rewardContainer.gameObject.SetActive(value: true);
		claimButtonRenderer.gameObject.SetActive(value: true);
		loadingIcon.gameObject.SetActive(value: false);
		PopulateDailyRewards();
		UpdateScales();
		SetDayActive(RewardSystem.CurrentLevel, instant: true);
		if (!isShowingCloseButton && canShowCloseButton)
		{
			while (timeLeftToShowCloseButton > 0f)
			{
				timeLeftToShowCloseButton -= Time.deltaTime;
				yield return null;
			}
			closeButton.gameObject.SetActive(value: true);
		}
	}

	private void Update()
	{
		UpdateClaimText();
	}

	private void SetInfoText(PrizeType prizeType, bool isBundle = false)
	{
		string empty = string.Empty;
		if (isBundle)
		{
			empty = BundleRewardInfo;
		}
		else
		{
			if (!RewardInfos.ContainsKey(prizeType))
			{
				infoTxt.text = string.Empty;
				return;
			}
			empty = RewardInfos[prizeType];
		}
		UpdateTextMeshLocale(infoTxt, empty, (!Singleton<Localizer>.Instance.CurrentLocale.Equals("ja-JP")) ? 24 : 18, string.Empty);
	}

	private void PopulateDailyRewards()
	{
		List<DailyRewardBundle> rewards = Singleton<RewardSystem>.Instance.Rewards;
		if (dailyRewards != null)
		{
			for (int i = 0; i < dailyRewards.Length; i++)
			{
				if (dailyRewards[i] != null)
				{
					UnityEngine.Object.Destroy(dailyRewards[i].gameObject);
				}
			}
		}
		dailyRewards = new Transform[rewards.Count];
		UnityEngine.Object original = Resources.Load("UI/Amazon/DailyReward");
		for (int j = 0; j < rewards.Count; j++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			gameObject.transform.parent = rewardContainer.transform;
			gameObject.transform.localPosition = new Vector3((float)j * 6.8f, 0f);
			gameObject.layer = rewardContainer.gameObject.layer;
			Reward component = gameObject.GetComponent<Reward>();
			component.SetRewards(rewards[j].GetRewards(j));
			component.SetState(RewardSystem.GetRewardStateForDay(j));
			dailyRewards[j] = gameObject.transform;
		}
	}

	public void ClaimReward()
	{
		if (!claimRewardSequencePlaying && Singleton<RewardSystem>.Instance.IsRewardReady())
		{
			claimBtnTxt.text = string.Empty;
			claimButtonSprite.m_id = claimButtonInactiveSprite;
			claimButtonSprite.RebuildMesh();
			claimButtonRenderer.gameObject.SetActive(value: false);
			SetDayActive(RewardSystem.CurrentLevel, instant: false);
			Singleton<RewardSystem>.Instance.ClaimReward();
			if (collectSound != null && collectSound.GetComponent<AudioSource>() != null)
			{
				Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(collectSound.GetComponent<AudioSource>());
			}
		}
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
		mainMenu.gameObject.SetActive(value: false);
	}

	public void Close()
	{
		if (!waitingToClaimReward)
		{
			RewardSystem.FreezeResetTime = false;
			base.gameObject.SetActive(value: false);
			mainMenu.gameObject.SetActive(value: true);
		}
	}

	public void SetDayActive(int day, bool instant)
	{
		if (instant)
		{
			rewardContainer.localPosition = new Vector3((0f - (float)day) * 6.8f, rewardContainer.localPosition.y);
			UpdateScales();
			panel.width = ((day > 0) ? 21f : 16f);
			hideFirstDayMask.enabled = day <= 0;
			centerDayBG.localScale = Vector3.one + Vector3.right * ((day > 0) ? 0f : 0.8f);
			SetDayTexts(day);
			if (!Singleton<RewardSystem>.Instance.IsRewardReady())
			{
				UpdateTextMeshLocale(infoTxt, "REWARD_LATER", (!Singleton<Localizer>.Instance.CurrentLocale.Equals("ja-JP")) ? 24 : 18, string.Empty);
			}
		}
		else
		{
			Singleton<RewardSystem>.Instance.StartCoroutine(SetDay(day));
		}
	}

	private IEnumerator SetDay(int day)
	{
		closeButton.gameObject.SetActive(value: false);
		claimRewardSequencePlaying = true;
		waitingToClaimReward = false;
		yield return Singleton<RewardSystem>.Instance.StartCoroutine(ClaimRewardSequence(day));
		if (day + 1 >= RewardSystem.AmountOfDays)
		{
			Singleton<RewardSystem>.Instance.RefreshData();
			PopulateDailyRewards();
			day = -1;
		}
		bool isShowingCloseButton = false;
		float timeLeftToShowCloseButton = 2f;
		while (!Singleton<RewardSystem>.Instance.HasTime)
		{
			timeLeftToShowCloseButton -= Time.deltaTime;
			if (!isShowingCloseButton && timeLeftToShowCloseButton <= 0f)
			{
				isShowingCloseButton = false;
				closeButton.gameObject.SetActive(value: true);
			}
			yield return null;
		}
		float targetX = (0f - (float)(day + 1)) * 6.8f;
		float counter = 0f;
		float moveDistance = Mathf.Abs(rewardContainer.localPosition.x) - Mathf.Abs(targetX);
		float startX = rewardContainer.localPosition.x;
		if (moveSound != null && moveSound.GetComponent<AudioSource>() != null)
		{
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(moveSound.GetComponent<AudioSource>());
		}
		float startingPanelWidth = panel.width;
		float panelTargetWidth = ((day >= 0) ? 21f : 16f);
		float startingDayBGWidth = centerDayBG.localScale.x - 1f;
		float dayBGTargetWidth = ((day >= 0) ? 0f : 0.8f);
		bool updatedDayTexts = false;
		for (; counter < translationTime; counter += Time.deltaTime)
		{
			panel.width = Mathf.Lerp(startingPanelWidth, panelTargetWidth, counter / (translationTime * 0.5f));
			centerDayBG.localScale = Vector3.one + Vector3.right * Mathf.Lerp(startingDayBGWidth, dayBGTargetWidth, (counter + 0.2f) / translationTime);
			rewardContainer.localPosition = new Vector3(startX + translationCurve.Evaluate(counter) * moveDistance, rewardContainer.localPosition.y, 0f);
			UpdateScales();
			if (!updatedDayTexts && counter > translationTime * 0.25f)
			{
				hideFirstDayMask.enabled = false;
				updatedDayTexts = true;
				SetDayTexts(day + 1);
			}
			yield return null;
		}
		hideFirstDayMask.enabled = day < 0;
		panel.width = panelTargetWidth;
		rewardContainer.localPosition = new Vector3(targetX, rewardContainer.localPosition.y);
		UpdateScales();
		UpdateTextMeshLocale(infoTxt, "REWARD_LATER", (!Singleton<Localizer>.Instance.CurrentLocale.Equals("ja-JP")) ? 24 : 18, string.Empty);
		claimRewardSequencePlaying = false;
		claimButtonRenderer.gameObject.SetActive(value: true);
		closeButton.gameObject.SetActive(value: true);
	}

	private void SetDayTexts(int day)
	{
		for (int i = 0; i < dayTexts.Length; i++)
		{
			dayTexts[i].transform.parent.gameObject.SetActive((day == 0 && i == 1) || day > 0);
			int num = day + i;
			if (num < 1 || num > 30)
			{
				dayTexts[i].text = "-";
			}
			else if (i == 1 && day == 0)
			{
				dayTexts[i].transform.localScale = Vector3.one * GetLocalizationScale(firstDay: true);
				SetDayText(dayTexts[i], "REWARD_FIRST_DAY", -1);
			}
			else
			{
				dayTexts[i].transform.localScale = Vector3.one * GetLocalizationScale(firstDay: false);
				SetDayText(dayTexts[i], "REWARD_DAY", num);
			}
		}
	}

	private float GetLocalizationScale(bool firstDay)
	{
		if (Singleton<Localizer>.Instance == null)
		{
			if (firstDay)
			{
				return 0.65f;
			}
			return 0.8f;
		}
		switch (Singleton<Localizer>.Instance.CurrentLocale)
		{
		case "en-EN":
			if (firstDay)
			{
				return 0.8f;
			}
			return 1f;
		case "ja-JP":
			if (firstDay)
			{
				return 0.9f;
			}
			return 0.9f;
		case "zh-CN":
			if (firstDay)
			{
				return 1f;
			}
			return 1.3f;
		default:
			if (firstDay)
			{
				return 0.65f;
			}
			return 0.8f;
		}
	}

	private void SetDayText(TextMesh tm, string localeKey, int day)
	{
		UpdateTextMeshLocale(tm, localeKey, -1, (day <= 0) ? string.Empty : $" {day}");
	}

	private IEnumerator ClaimRewardSequence(int day)
	{
		if (dailyRewards == null || day < 0 || day >= dailyRewards.Length)
		{
			yield break;
		}
		Reward reward = dailyRewards[day].GetComponent<Reward>();
		if (reward == null || reward.RewardIcon == null)
		{
			yield break;
		}
		List<DailyReward> rewards = Singleton<RewardSystem>.Instance.Rewards[day].GetRewards(day);
		bool isBundle = rewards.Count > 1;
		foreach (DailyReward item in rewards)
		{
			Animation componentInChildren = reward.GetComponentInChildren<Animation>();
			if (componentInChildren != null)
			{
				componentInChildren.Play();
			}
			Transform rewardTf;
			if (isBundle)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(reward.GetRewardPrefab(item.prize), reward.RewardIcon.transform.position, Quaternion.identity);
				rewardTf = gameObject.transform;
			}
			else
			{
				rewardTf = reward.RewardIcon.transform;
			}
			reward.RewardIcon.claimNowSprite.SetActive(value: true);
			reward.RewardIcon.disabledSprite.SetActive(value: false);
			Vector3 originalIconPosition = rewardTf.localPosition;
			float startYIcon = originalIconPosition.y;
			Vector3 originalCountPosition = reward.RewardCount.transform.localPosition;
			float startYCount = originalCountPosition.y;
			reward.SetRewardCount(item.prizeCount, "+");
			float fade = 0f;
			while (fade < 1f)
			{
				rewardTf.localPosition = new Vector3(rewardTf.localPosition.x, startYIcon + translationCurve.Evaluate(fade) * 2f, originalIconPosition.z - 0.5f);
				reward.RewardCount.transform.localPosition = new Vector3(reward.RewardCount.transform.localPosition.x, startYCount + translationCurve.Evaluate(fade) * 2f, originalCountPosition.z - 1f);
				fade += Time.deltaTime;
				yield return null;
			}
			rewardTf.localPosition = originalIconPosition;
			reward.RewardCount.transform.localPosition = originalCountPosition;
			if (isBundle)
			{
				UnityEngine.Object.Destroy(rewardTf.gameObject);
			}
		}
		reward.SetState(RewardSystem.GetRewardStateForDay(day));
		yield return new WaitForSeconds(0.2f);
	}

	private void UpdateScales()
	{
		if (dailyRewards != null)
		{
			for (int i = 0; i < dailyRewards.Length; i++)
			{
				float time = Mathf.Clamp(Mathf.Abs(dailyRewards[i].position.x - base.transform.position.x), 0f, 11f) / 11f;
				time = scalingCurve.Evaluate(time);
				dailyRewards[i].localScale = new Vector3(time, time, 1f);
			}
		}
	}

	private void UpdateClaimText()
	{
		if (timedOut || waitingToClaimReward || claimRewardSequencePlaying || !Singleton<RewardSystem>.Instance.HasTime)
		{
			return;
		}
		int num = Singleton<RewardSystem>.Instance.SecondsToNextReward();
		if (num == lastTimeUpdated)
		{
			return;
		}
		lastTimeUpdated = num;
		if (num > 0)
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, num);
			sb.Remove(0, sb.Length);
			int num2 = Mathf.RoundToInt((float)timeSpan.TotalHours);
			int minutes = timeSpan.Minutes;
			int seconds = timeSpan.Seconds;
			if (num2 > 0)
			{
				sb.Append($"{num2}h ");
			}
			if (minutes > 0)
			{
				sb.Append($"{minutes}m ");
			}
			if (seconds >= 0)
			{
				sb.Append($"{seconds}s ");
			}
			claimBtnTxt.transform.localScale = Vector3.one * 0.16f;
			claimBtnTxt.text = sb.ToString();
			claimButtonSprite.m_id = claimButtonInactiveSprite;
			claimButtonSprite.RebuildMesh();
		}
		else
		{
			ReadyToClaim();
		}
	}

	private void ReadyToClaim()
	{
		waitingToClaimReward = true;
		closeButton.gameObject.SetActive(value: false);
		claimButtonSprite.m_id = claimButtonActiveSprite;
		claimButtonSprite.RebuildMesh();
		claimBtnTxt.gameObject.SetActive(value: true);
		UpdateTextMeshLocale(claimBtnTxt, "REWARD_CLAIM", -1, string.Empty);
		claimBtnTxt.transform.localScale = Vector3.one * 0.24f;
		int currentLevel = RewardSystem.CurrentLevel;
		if (dailyRewards != null && currentLevel >= 0 && currentLevel < dailyRewards.Length)
		{
			dailyRewards[currentLevel].GetComponent<Reward>().SetState(RewardSystem.GetRewardStateForDay(currentLevel));
			List<DailyReward> rewards = Singleton<RewardSystem>.Instance.Rewards[currentLevel].GetRewards(currentLevel);
			bool isBundle = rewards.Count > 1;
			SetInfoText(rewards[0].prize, isBundle);
		}
	}

	private void HandleKeyReleased(KeyCode obj)
	{
		if (obj == KeyCode.Escape)
		{
			Close();
		}
	}
}
