using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DessertSelector : MonoBehaviour, WidgetListener
{
	public GameObject m_dessertButtonPrefab;

	public GameData m_gameData;

	public GameObject m_Reward;

	public GameObject m_FoodPrefab;

	public GameObject m_menu;

	public GameObject m_BuyButton;

	public float m_InitialScale = 3f;

	public float m_GrowStepMul = 1.1f;

	public float m_GrowStepAdd;

	public float m_GrowLimit = 6f;

	public float m_GrowDuration = 0.5f;

	private float m_CurGrowScale = 3f;

	private float scaleAnimTime = -1f;

	private Vector3 scaleStart = Vector3.one;

	private bool isEating;

	private Pig.Expressions m_defaultExpression;

	private bool m_draggingDessert;

	private float m_waitForFoodTimer;

	private GameObject m_dessertInScene;

	[Range(0f, 1f)]
	public float m_PrizeProbability = 0.1f;

	[Range(0f, 1f)]
	public float m_PrizeProbabilityDesktop = 0.25f;

	public List<FeedingPrize> m_FeedingPrizes = new List<FeedingPrize>();

	public List<FeedingPrize> m_FeedingPrizesChina = new List<FeedingPrize>();

	[SerializeField]
	private List<FeedingPrize> m_FeedingPrizesOdyssey = new List<FeedingPrize>();

	public GameObject[] m_JunkPrizes = new GameObject[0];

	private float m_totalRangeWidth = 100f;

	public float m_IdleTimeout = 5f;

	public float m_IdleAnimTimeoutMin = 5f;

	public float m_IdleAnimTimeoutMax = 10f;

	private float m_IdleTime;

	private float m_IdleAnimTime;

	private float m_CurIdleAnimTimeOut;

	private bool m_IsIdleEnabled = true;

	public Pig.Expressions[] m_IdleExpressions = new Pig.Expressions[4]
	{
		Pig.Expressions.Blink,
		Pig.Expressions.Panting,
		Pig.Expressions.Laugh,
		Pig.Expressions.Snooze
	};

	public AudioSource m_GrowSoundFx;

	public PigExpressionSound[] expressionSounds;

	[HideInInspector]
	public readonly float KingPigInitialWeight = 100f;

	private ScrollList m_scrollList;

	private GameObject m_kingPig;

	private Camera m_hudCam;

	private Camera m_mainCam;

	private int availableDessertsCount;

	private GameObject m_Cursor;

	private AudioSource m_LastExpressionSound;

	private Pig.Expressions m_LastAudioExpression;

	private void Awake()
	{
		if (DeviceInfo.IsDesktop)
		{
			m_PrizeProbability = m_PrizeProbabilityDesktop;
		}
		m_scrollList = m_menu.transform.Find("ScrollList").GetComponent<ScrollList>();
		m_scrollList.SetListener(this);
		m_hudCam = Singleton<GuiManager>.Instance.FindCamera();
		m_mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		m_kingPig = (UnityEngine.Object.FindObjectOfType(typeof(KingPig)) as KingPig).gameObject;
		GameObject.FindGameObjectWithTag("KingPigMouth").GetComponent<CapsuleCollider>().enabled = true;
		m_CurGrowScale = GameProgress.GetFloat("KingPigFeedScale", m_InitialScale);
		m_kingPig.transform.parent.localScale = m_CurGrowScale * Vector3.one;
		int value = Singleton<GameConfigurationManager>.Instance.GetValue<int>("king_pig_prize_probabilities", "Prize");
		if (value > 0)
		{
			m_PrizeProbability = (float)value / 100f;
		}
		float num = 0f;
		List<FeedingPrize> list = (Singleton<BuildCustomizationLoader>.Instance.IsChina ? m_FeedingPrizesChina : ((!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey) ? m_FeedingPrizes : m_FeedingPrizesOdyssey));
		foreach (FeedingPrize item in list)
		{
			int value2 = Singleton<GameConfigurationManager>.Instance.GetValue<int>("king_pig_prize_probabilities", item.type.ToString());
			if (value2 > 0)
			{
				item.rangeWidth = value2;
			}
			num += item.rangeWidth;
		}
		m_totalRangeWidth = num;
		ScrapButton.Instance.EnableButton(enable: false);
	}

	private void Start()
	{
		CreateDessertButtonList();
		m_kingPig.GetComponent<KingPig>().SetExpression(Pig.Expressions.Normal);
		KeyListener.keyPressed += HandleKeyPressed;
	}

	private void OnDestroy()
	{
		KeyListener.keyPressed -= HandleKeyPressed;
	}

	private void CreateDessertButtonList()
	{
		availableDessertsCount = 0;
		foreach (GameObject dessert in m_gameData.m_desserts)
		{
			int num = GameProgress.DessertCount(dessert.GetComponent<Dessert>().saveId);
			availableDessertsCount += num;
			if (num > 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_dessertButtonPrefab);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(dessert.GetComponent<Dessert>().prefabIcon);
				gameObject2.transform.parent = gameObject.transform;
				gameObject2.transform.localScale = new Vector3(1.75f, 1.75f, 1f);
				gameObject2.transform.localPosition = new Vector3(0f, 0f, -0.1f);
				gameObject2.GetComponent<MeshRenderer>().sortingOrder = 1;
				gameObject.GetComponent<DraggableButton>().Icon = gameObject2;
				gameObject.GetComponent<DraggableButton>().DragIconPrefab = gameObject2;
				gameObject.GetComponent<DraggableButton>().DragIconScale = 1.75f;
				gameObject.GetComponent<DraggableButton>().DragObject = dessert;
				Transform obj = gameObject.transform.Find("PartCount");
				obj.GetComponent<TextMesh>().text = num.ToString();
				obj.GetComponent<MeshRenderer>().sortingOrder = 3;
				gameObject.transform.parent = m_scrollList.transform;
				m_scrollList.AddButton(gameObject.GetComponent<Widget>());
			}
		}
	}

	public void Select(Widget widget, object targetObject)
	{
	}

	public void StartDrag(Widget widget, object targetObject)
	{
		SetIdle(isEnabled: false);
		KingPig component = m_kingPig.GetComponent<KingPig>();
		component.followMouse = true;
		if (!IsEating())
		{
			component.SetExpression(Pig.Expressions.WaitForFood);
		}
		m_defaultExpression = Pig.Expressions.WaitForFood;
		EventManager.Send(new DragStartedEvent(BasePart.PartType.Balloon));
		m_draggingDessert = true;
		m_waitForFoodTimer = 1.5f;
	}

	public void CancelDrag(Widget widget, object targetObject)
	{
		SetIdle(isEnabled: true);
		m_kingPig.GetComponent<KingPig>().followMouse = false;
		if (!IsEating())
		{
			m_kingPig.GetComponent<KingPig>().SetExpression(Pig.Expressions.Normal);
		}
		m_defaultExpression = Pig.Expressions.Normal;
		m_draggingDessert = false;
	}

	private void DelayAction(Action action, float time = 1f)
	{
		StartCoroutine(DelayActionCorutine(action, time));
	}

	private IEnumerator DelayActionCorutine(Action action, float time)
	{
		yield return new WaitForSeconds(time);
		action();
	}

	public void Drop(Widget widget, Vector3 position, object targetObject)
	{
		SetIdle(isEnabled: true);
		m_kingPig.GetComponent<KingPig>().followMouse = false;
		m_defaultExpression = Pig.Expressions.Normal;
		m_draggingDessert = false;
		Vector3 vector = m_hudCam.WorldToScreenPoint(position);
		Vector3 position2 = m_mainCam.ScreenToWorldPoint(vector);
		position2.z = m_kingPig.transform.position.z;
		Rect rect = new Rect(0f, 0f, m_hudCam.pixelWidth, m_hudCam.pixelHeight);
		if ((bool)m_FoodPrefab && rect.Contains(vector))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_FoodPrefab, position2, Quaternion.identity);
			gameObject.GetComponent<DessertFood>().m_DessertButton = widget;
			gameObject.GetComponent<DessertFood>().m_DessertSelector = this;
			GameObject obj = UnityEngine.Object.Instantiate(widget.GetComponent<DraggableButton>().Icon, position2, Quaternion.identity);
			obj.transform.parent = gameObject.transform;
			obj.layer = 0;
			obj.transform.localScale = new Vector3(2.5f, 2.5f, 1f);
			m_dessertInScene = gameObject;
		}
	}

	private void SetButtonCount(Widget button, int count)
	{
		button.transform.Find("PartCount").GetComponent<TextMesh>().text = count.ToString();
	}

	private string GetKingPigTotalText()
	{
		return GameProgress.EatenDessertsCount().ToString();
	}

	private FeedingPrize SelectFeedingPrize()
	{
		float num = UnityEngine.Random.value * m_totalRangeWidth;
		float num2 = 0f;
		List<FeedingPrize> list = (Singleton<BuildCustomizationLoader>.Instance.IsChina ? m_FeedingPrizesChina : ((!Singleton<BuildCustomizationLoader>.Instance.IsOdyssey) ? m_FeedingPrizes : m_FeedingPrizesOdyssey));
		foreach (FeedingPrize item in list)
		{
			num2 += item.rangeWidth;
			if (num <= num2)
			{
				return item;
			}
		}
		return null;
	}

	private void CheckAchievements()
	{
		if (Singleton<SocialGameManager>.IsInstantiated())
		{
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.FEEDER", 100.0, (int limit) => GameProgress.EatenDessertsCount() >= limit);
		}
	}

	private FeedingPrize.PrizeType GiveReward(GameObject dessert)
	{
		FeedingPrize.PrizeType result = FeedingPrize.PrizeType.None;
		int num = GameProgress.EatenDessertsCount();
		if (UnityEngine.Random.value < m_PrizeProbability || num <= 1 || dessert.name == "GoldenCake")
		{
			FeedingPrize feedingPrize;
			if (num <= 1)
			{
				feedingPrize = null;
				foreach (FeedingPrize feedingPrize3 in m_FeedingPrizes)
				{
					if (feedingPrize3.type == FeedingPrize.PrizeType.SuperGlue)
					{
						feedingPrize = feedingPrize3;
						break;
					}
				}
			}
			else
			{
				feedingPrize = SelectFeedingPrize();
			}
			FeedingPrize feedingPrize2 = feedingPrize;
			if (dessert.name == "GoldenCake" && feedingPrize2.type == FeedingPrize.PrizeType.Junk)
			{
				feedingPrize2 = m_FeedingPrizes[1];
			}
			if (feedingPrize2 != null)
			{
				if (feedingPrize2.type != FeedingPrize.PrizeType.Junk && feedingPrize2.type != 0)
				{
					GameObject gameObject = m_Reward.transform.Find("Offset/AnimationNode").gameObject;
					if (gameObject.transform.childCount > 0)
					{
						UnityEngine.Object.Destroy(gameObject.transform.GetChild(0).gameObject);
					}
					GameObject gameObject2 = UnityEngine.Object.Instantiate(feedingPrize2.icon, gameObject.transform.position, gameObject.transform.rotation);
					gameObject2.transform.parent = gameObject.transform;
					gameObject2.transform.localScale = Vector3.one * feedingPrize2.iconScale;
					CurrencyParticleBurst burst = null;
					string customTypeOfGain = "King Pig feeding prize";
					switch (feedingPrize2.type)
					{
					case FeedingPrize.PrizeType.SuperGlue:
						GameProgress.AddSuperGlue(1);
						if (Singleton<IapManager>.Instance != null)
						{
							Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.SuperGlueSingle, 1, customTypeOfGain);
						}
						break;
					case FeedingPrize.PrizeType.SuperMagnet:
						GameProgress.AddSuperMagnet(1);
						if (Singleton<IapManager>.Instance != null)
						{
							Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.SuperMagnetSingle, 1, customTypeOfGain);
						}
						break;
					case FeedingPrize.PrizeType.TurboCharge:
						GameProgress.AddTurboCharge(1);
						if (Singleton<IapManager>.Instance != null)
						{
							Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.TurboChargeSingle, 1, customTypeOfGain);
						}
						break;
					case FeedingPrize.PrizeType.SuperMechanic:
						GameProgress.AddBluePrints(1);
						if (Singleton<IapManager>.Instance != null)
						{
							Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.BlueprintSingle, 1, customTypeOfGain);
						}
						break;
					case FeedingPrize.PrizeType.NightVision:
						GameProgress.AddNightVision(1);
						if (Singleton<IapManager>.Instance != null)
						{
							Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.NightVisionSingle, 1, customTypeOfGain);
						}
						break;
					case FeedingPrize.PrizeType.SnoutCoins:
					{
						int value3 = Singleton<GameConfigurationManager>.Instance.GetValue<int>("king_pig_snout_reward", "min");
						int value4 = Singleton<GameConfigurationManager>.Instance.GetValue<int>("king_pig_snout_reward", "max");
						int num3 = UnityEngine.Random.Range(value3, value4 + 1);
						if (num3 <= 0)
						{
							break;
						}
						if (Singleton<DoubleRewardManager>.Instance.HasDoubleReward)
						{
							num3 *= 2;
						}
						GameProgress.AddSnoutCoins(num3);
						GameObject gameObject4 = GameObject.FindGameObjectWithTag("KingPigMouth");
						if (gameObject4 != null)
						{
							Camera main2 = Camera.main;
							Camera camera2 = Singleton<GuiManager>.Instance.FindCamera();
							if (main2 == null || camera2 == null)
							{
								break;
							}
							Vector3 vector2 = gameObject4.transform.position * (1f / main2.orthographicSize);
							gameObject2.transform.parent = null;
							gameObject2.transform.position = vector2 * camera2.orthographicSize;
						}
						burst = gameObject2.GetComponent<CurrencyParticleBurst>();
						if (burst != null)
						{
							burst.SetBurst(num3, 10f);
						}
						break;
					}
					case FeedingPrize.PrizeType.Scrap:
					{
						int value = Singleton<GameConfigurationManager>.Instance.GetValue<int>("king_pig_scrap_reward", "min");
						int value2 = Singleton<GameConfigurationManager>.Instance.GetValue<int>("king_pig_scrap_reward", "max");
						int num2 = UnityEngine.Random.Range(value, value2 + 1);
						if (num2 <= 0)
						{
							break;
						}
						GameProgress.AddScrap(num2);
						GameObject gameObject3 = GameObject.FindGameObjectWithTag("KingPigMouth");
						if (gameObject3 != null)
						{
							Camera main = Camera.main;
							Camera camera = Singleton<GuiManager>.Instance.FindCamera();
							if (main == null || camera == null)
							{
								break;
							}
							Vector3 vector = gameObject3.transform.position * (1f / main.orthographicSize);
							gameObject2.transform.parent = null;
							gameObject2.transform.position = vector * camera.orthographicSize;
						}
						burst = gameObject2.GetComponent<CurrencyParticleBurst>();
						if (burst != null)
						{
							burst.SetBurst(num2, 10f);
						}
						break;
					}
					}
					if (feedingPrize2.type == FeedingPrize.PrizeType.SnoutCoins || feedingPrize2.type == FeedingPrize.PrizeType.Scrap)
					{
						DelayAction(delegate
						{
							if (burst != null)
							{
								burst.Burst();
							}
						}, 1.3f);
					}
					else
					{
						DelayAction(delegate
						{
							m_Reward.SetActive(value: true);
						}, m_GrowDuration);
					}
					if (PlayerProgressBar.Instance != null && Singleton<PlayerProgress>.IsInstantiated())
					{
						Vector3 position = GameObject.FindGameObjectWithTag("KingPigMouth").transform.position * (1f / m_mainCam.orthographicSize);
						position *= m_hudCam.orthographicSize;
						PlayerProgressBar.Instance.DelayUpdate();
						int amount = Singleton<PlayerProgress>.Instance.AddExperience(PlayerProgress.ExperienceType.KingBurp);
						PlayerProgressBar.Instance.AddParticles(position, amount, 1.3f);
					}
					if (Singleton<SocialGameManager>.IsInstantiated())
					{
						Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.KINGS_FAVORITE", 100.0);
					}
				}
				result = feedingPrize2.type;
			}
		}
		return result;
	}

	public bool IsEating()
	{
		return isEating;
	}

	private void SetIdle(bool isEnabled)
	{
		if (m_IsIdleEnabled != isEnabled)
		{
			if (isEnabled)
			{
				m_IdleTime = 0f;
			}
			m_IsIdleEnabled = isEnabled;
		}
	}

	private IEnumerator PlayChewingAnim(float time)
	{
		KingPig kp = m_kingPig.GetComponent<KingPig>();
		PlayKingPigExpressionAudioEffect(Pig.Expressions.Chew);
		yield return StartCoroutine(kp.PlayAnimation(Pig.Expressions.Chew, time));
		kp.SetExpression(m_defaultExpression);
	}

	private IEnumerator PlayChewThenShrinkAndBurpAnim(float chewTime, float burpTime, FeedingPrize.PrizeType prizeType)
	{
		KingPig kp = m_kingPig.GetComponent<KingPig>();
		PlayKingPigExpressionAudioEffect(Pig.Expressions.Chew);
		yield return StartCoroutine(kp.PlayAnimation(Pig.Expressions.Chew, chewTime));
		DelayAction(delegate
		{
			scaleAnimTime = 0f;
			if (prizeType == FeedingPrize.PrizeType.Junk || prizeType == FeedingPrize.PrizeType.None)
			{
				GameObject obj = GameObject.FindGameObjectWithTag("KingPigMouth");
				int num = UnityEngine.Random.Range(0, m_JunkPrizes.Length);
				Vector3 position = obj.transform.position;
				position.z = -2f;
				UnityEngine.Object.Instantiate(m_JunkPrizes[num], position, Quaternion.identity);
			}
		}, 0.2f);
		PlayKingPigExpressionAudioEffect(Pig.Expressions.Burp);
		yield return StartCoroutine(kp.PlayAnimation(Pig.Expressions.Burp, burpTime));
		kp.SetExpression(m_defaultExpression);
	}

	private IEnumerator PlayMissAnim(float time)
	{
		KingPig kp = m_kingPig.GetComponent<KingPig>();
		PlayKingPigExpressionAudioEffect(Pig.Expressions.Fear);
		yield return StartCoroutine(kp.PlayAnimation(Pig.Expressions.Fear, time));
		kp.SetExpression(m_defaultExpression);
	}

	private IEnumerator PlayIdleAnim()
	{
		KingPig kp = m_kingPig.GetComponent<KingPig>();
		Pig.Expressions expressions = m_IdleExpressions[UnityEngine.Random.Range(0, m_IdleExpressions.Length)];
		PlayKingPigExpressionAudioEffect(expressions);
		yield return StartCoroutine(kp.PlayAnimation(expressions));
		kp.SetExpression(m_defaultExpression);
	}

	private void PlayKingPigExpressionAudioEffect(Pig.Expressions expr)
	{
		if ((bool)m_LastExpressionSound)
		{
			if (m_LastAudioExpression == expr)
			{
				return;
			}
			if (m_LastAudioExpression == Pig.Expressions.WaitForFood)
			{
				m_LastExpressionSound.Stop();
			}
		}
		PigExpressionSound pigExpressionSound = null;
		PigExpressionSound[] array = expressionSounds;
		foreach (PigExpressionSound pigExpressionSound2 in array)
		{
			if (pigExpressionSound2.expression == expr)
			{
				pigExpressionSound = pigExpressionSound2;
				break;
			}
		}
		if (pigExpressionSound != null && pigExpressionSound.soundFx != null && pigExpressionSound.soundFx.Length != 0)
		{
			AudioSource effectSource = pigExpressionSound.soundFx[UnityEngine.Random.Range(0, pigExpressionSound.soundFx.Length)];
			m_LastExpressionSound = Singleton<AudioManager>.Instance.Play2dEffect(effectSource);
			m_LastAudioExpression = expr;
		}
	}

	public void MissDessert(Widget widget)
	{
		if (!IsEating())
		{
			m_defaultExpression = Pig.Expressions.Normal;
			StartCoroutine(PlayMissAnim(0.5f));
		}
	}

	public void EatDessert(Widget widget)
	{
		if (IsEating() || widget == null)
		{
			return;
		}
		isEating = true;
		GameObject gameObject = (GameObject)widget.GetComponent<DraggableButton>().DragObject;
		GameProgress.EatDesserts(gameObject.GetComponent<Dessert>().saveId, 1);
		int num = GameProgress.DessertCount(gameObject.GetComponent<Dessert>().saveId);
		SetButtonCount(widget, num);
		CheckAchievements();
		availableDessertsCount--;
		StartCoroutine(PlayChewingAnim(1f));
		if (num <= 0)
		{
			if (widget.GetComponent<DraggableButton>().isDragging)
			{
				CancelDrag(widget, null);
			}
			m_scrollList.RemoveButton(widget);
		}
		FeedingPrize.PrizeType prizeType = GiveReward(gameObject);
		m_CurGrowScale = m_CurGrowScale * m_GrowStepMul + m_GrowStepAdd;
		if (m_CurGrowScale > m_GrowLimit || prizeType != 0)
		{
			m_CurGrowScale = m_InitialScale;
			StartCoroutine(PlayChewThenShrinkAndBurpAnim(1f, m_GrowDuration, prizeType));
		}
		else
		{
			if ((bool)m_GrowSoundFx)
			{
				Singleton<AudioManager>.Instance.Play2dEffect(m_GrowSoundFx);
			}
			scaleAnimTime = 0f;
		}
		GameProgress.SetFloat("KingPigFeedScale", m_CurGrowScale);
		scaleStart = m_kingPig.transform.parent.localScale;
	}

	private void Update()
	{
		if (m_BuyButton != null)
		{
			if (Singleton<BuildCustomizationLoader>.Instance.IsChina && availableDessertsCount == 0)
			{
				m_BuyButton.SetActive(value: true);
			}
			else
			{
				m_BuyButton.SetActive(value: false);
			}
		}
		if (scaleAnimTime >= 0f)
		{
			scaleAnimTime += Time.deltaTime / m_GrowDuration;
			if (scaleAnimTime > 1f)
			{
				isEating = false;
				scaleAnimTime = -1f;
			}
			else
			{
				m_kingPig.transform.parent.localScale = Vector3.Lerp(scaleStart, Vector3.one * m_CurGrowScale, scaleAnimTime);
			}
		}
		if (m_IsIdleEnabled)
		{
			m_IdleTime += Time.deltaTime;
			if (m_IdleTime >= m_IdleTimeout)
			{
				m_IdleTime = m_IdleTimeout;
				m_IdleAnimTime += Time.deltaTime;
				if (m_IdleAnimTime > m_CurIdleAnimTimeOut)
				{
					m_IdleAnimTime = 0f;
					m_CurIdleAnimTimeOut = UnityEngine.Random.Range(m_IdleAnimTimeoutMin, m_IdleAnimTimeoutMax);
					StartCoroutine(PlayIdleAnim());
				}
			}
		}
		if (m_draggingDessert)
		{
			GuiManager.Pointer pointer = GuiManager.GetPointer();
			Vector3 position = m_mainCam.ScreenToWorldPoint(pointer.position);
			EventManager.Send(new DraggingPartEvent(BasePart.PartType.Balloon, position));
			KingPig component = m_kingPig.GetComponent<KingPig>();
			if (component.m_currentExpression == Pig.Expressions.Normal)
			{
				component.SetExpression(Pig.Expressions.WaitForFood);
			}
			if (component.m_currentExpression == Pig.Expressions.WaitForFood)
			{
				m_waitForFoodTimer -= Time.deltaTime;
				if (m_waitForFoodTimer <= 0f)
				{
					PlayKingPigExpressionAudioEffect(Pig.Expressions.WaitForFood);
					m_waitForFoodTimer = 7f;
				}
			}
		}
		else if ((bool)m_dessertInScene)
		{
			Vector3 position2 = m_dessertInScene.transform.position;
			EventManager.Send(new DraggingPartEvent(BasePart.PartType.Balloon, position2));
		}
	}

	private void HandleKeyPressed(KeyCode obj)
	{
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused && (bool)m_scrollList)
		{
			m_scrollList.ResetSelection();
		}
	}
}
