using UnityEngine;

public class InGameBuildMenu : WPFMonoBehaviour
{
	public struct AutoBuildEvent : EventManager.Event
	{
		public int availableBlueprints;

		public bool usedState;

		public AutoBuildEvent(int availableBlueprints, bool usedState)
		{
			this.availableBlueprints = availableBlueprints;
			this.usedState = usedState;
		}
	}

	public struct ApplySuperGlueEvent : EventManager.Event
	{
		public int availableSuperGlue;

		public bool usedState;

		public ApplySuperGlueEvent(int availableSuperGlue, bool usedState)
		{
			this.availableSuperGlue = availableSuperGlue;
			this.usedState = usedState;
		}
	}

	public struct ApplySuperMagnetEvent : EventManager.Event
	{
		public int availableSuperMagnet;

		public bool usedState;

		public ApplySuperMagnetEvent(int availableSuperMagnet, bool usedState)
		{
			this.availableSuperMagnet = availableSuperMagnet;
			this.usedState = usedState;
		}
	}

	public struct ApplyTurboChargeEvent : EventManager.Event
	{
		public int availableTurboCharge;

		public bool usedState;

		public ApplyTurboChargeEvent(int availableTurboCharge, bool usedState)
		{
			this.availableTurboCharge = availableTurboCharge;
			this.usedState = usedState;
		}
	}

	public struct ApplyNightVisionEvent : EventManager.Event
	{
		public int availableNightVision;

		public bool usedState;

		public ApplyNightVisionEvent(int availableNightVision, bool usedState)
		{
			this.availableNightVision = availableNightVision;
			this.usedState = usedState;
		}
	}

	[SerializeField]
	private PartSelector partSelector;

	[SerializeField]
	private PowerupButton superGlueButton;

	[SerializeField]
	private PowerupButton superMagnetButton;

	[SerializeField]
	private PowerupButton turboChargeButton;

	[SerializeField]
	private PowerupButton superBluePrintButton;

	[SerializeField]
	private PowerupButton nightVisionButton;

	[SerializeField]
	private ToolboxButton toolboxButton;

	[SerializeField]
	private PigMechanic pigMechanic;

	[SerializeField]
	private GameObject tutorialButton;

	[SerializeField]
	private GameObject autoBuildButton;

	[SerializeField]
	private GameObject superBuildSelection;

	[SerializeField]
	private GameObject playButton;

	[SerializeField]
	private GameObject editorButtons;

	[SerializeField]
	private TextMesh[] trackLabel;

	private CakeRaceMode cakeRaceMode;

	public GameObject SuperBuildSelection => superBuildSelection;

	public GameObject AutoBuildButton => autoBuildButton;

	public GameObject TutorialButton => tutorialButton;

	public GameObject PlayButton => playButton;

	public PowerupButton NightVisionButton => nightVisionButton;

	public PowerupButton SuperBluePrintButton => superBluePrintButton;

	public PowerupButton TurboChargeButton => turboChargeButton;

	public PowerupButton SuperMagnetButton => superMagnetButton;

	public PowerupButton SuperGlueButton => superGlueButton;

	public ToolboxButton ToolboxButton => toolboxButton;

	public PartSelector PartSelector => partSelector;

	public PigMechanic PigMechanic => pigMechanic;

	private void Awake()
	{
		EventManager.Connect<UIEvent>(OnUpsellDialogChanged);
		EventManager.Connect<PartPlacedEvent>(OnPartPlaced);
		EventManager.Connect<PartRemovedEvent>(OnPartRemoved);
		if (INSettings.GetBool(INFeature.SaveButton))
		{
			GameObject gameObject = INUnity.LoadGameObject("SaveButton");
			GameObject obj = Object.Instantiate(gameObject);
			obj.name = gameObject.name;
			obj.transform.parent = base.transform;
			float num = (float)Screen.width / (float)Screen.height * 10f;
			obj.transform.localPosition = new Vector3(num - 1.9869f, 4f, 0f);
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<UIEvent>(OnUpsellDialogChanged);
		EventManager.Disconnect<PartPlacedEvent>(OnPartPlaced);
		EventManager.Disconnect<PartRemovedEvent>(OnPartRemoved);
	}

	private void OnPartRemoved(PartRemovedEvent data)
	{
		if (!INSettings.GetBool(INFeature.NewAlienEgg) && data.partType == BasePart.PartType.Egg && !WPFMonoBehaviour.levelManager.CurrentGameMode.ContraptionProto.HasPart(BasePart.PartType.Egg, BasePart.PartTier.Legendary) && WPFMonoBehaviour.levelManager.CurrentGameMode.ContraptionProto.HasAlienGlue)
		{
			WPFMonoBehaviour.levelManager.CurrentGameMode.ContraptionProto.RemoveSuperGlue();
		}
	}

	private void OnPartPlaced(PartPlacedEvent data)
	{
		if (!INSettings.GetBool(INFeature.NewAlienEgg) && data.partType == BasePart.PartType.Egg && data.partTier == BasePart.PartTier.Legendary && !WPFMonoBehaviour.levelManager.CurrentGameMode.ContraptionProto.HasSuperGlue)
		{
			WPFMonoBehaviour.levelManager.CurrentGameMode.ContraptionProto.ApplySuperGlue(Glue.Type.Alien);
		}
	}

	private void OnUpsellDialogChanged(UIEvent data)
	{
		if (data.type == UIEvent.Type.OpenUnlockFullVersionIapMenu)
		{
			base.gameObject.SetActive(value: false);
		}
		else if (data.type == UIEvent.Type.CloseUnlockFullVersionIapMenu && WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.Completed)
		{
			base.gameObject.SetActive(value: true);
		}
	}

	private void Start()
	{
		if ((bool)WPFMonoBehaviour.levelManager)
		{
			superGlueButton.SetAvailable(WPFMonoBehaviour.levelManager.SuperGlueAllowed);
			superMagnetButton.SetAvailable(WPFMonoBehaviour.levelManager.SuperMagnetAllowed);
			turboChargeButton.SetAvailable(WPFMonoBehaviour.levelManager.TurboChargeAllowed);
			superBluePrintButton.SetAvailable(WPFMonoBehaviour.levelManager.SuperBluePrintsAllowed);
			nightVisionButton.SetAvailable(WPFMonoBehaviour.levelManager.m_darkLevel);
		}
	}

	private void SetButtonAmountText(PowerupButton powerupButton, int amount)
	{
		string text = ((amount != 0 || !Singleton<IapManager>.IsInstantiated()) ? amount.ToString() : string.Empty);
		if (INSettings.GetBool(INFeature.InfiniteTools))
		{
			powerupButton.SetText(string.Empty);
		}
		else
		{
			powerupButton.SetText(text);
		}
	}

	private Transform FindRecursively(Transform root, string name)
	{
		Transform transform = root.Find(name);
		if ((bool)transform)
		{
			return transform;
		}
		int childCount = root.childCount;
		for (int i = 0; i < childCount; i++)
		{
			transform = FindRecursively(root.GetChild(i), name);
			if ((bool)transform)
			{
				return transform;
			}
		}
		return null;
	}

	private void OnEnable()
	{
		cakeRaceMode = WPFMonoBehaviour.levelManager.CurrentGameMode as CakeRaceMode;
		if (!INSettings.GetBool(INFeature.NewAlienEgg) && WPFMonoBehaviour.levelManager.CurrentGameMode.ContraptionProto.HasPart(BasePart.PartType.Egg, BasePart.PartTier.Legendary))
		{
			WPFMonoBehaviour.levelManager.CurrentGameMode.ContraptionProto.ApplySuperGlue(Glue.Type.Alien);
		}
		SetButtonAmountText(superBluePrintButton, GameProgress.BluePrintCount());
		SetButtonAmountText(superGlueButton, GameProgress.SuperGlueCount());
		SetButtonAmountText(superMagnetButton, GameProgress.SuperMagnetCount());
		SetButtonAmountText(turboChargeButton, GameProgress.TurboChargeCount());
		SetButtonAmountText(nightVisionButton, GameProgress.NightVisionCount());
		if ((bool)WPFMonoBehaviour.levelManager && (bool)WPFMonoBehaviour.levelManager.ContraptionProto)
		{
			superGlueButton.SetUsedState(WPFMonoBehaviour.levelManager.ContraptionProto.HasRegularGlue);
			superMagnetButton.SetUsedState(WPFMonoBehaviour.levelManager.ContraptionProto.HasSuperMagnet);
			turboChargeButton.SetUsedState(WPFMonoBehaviour.levelManager.ContraptionProto.HasTurboCharge);
			nightVisionButton.SetUsedState(WPFMonoBehaviour.levelManager.ContraptionProto.HasNightVision);
		}
		else
		{
			superGlueButton.SetUsedState(used: false);
			superMagnetButton.SetUsedState(used: false);
			turboChargeButton.SetUsedState(used: false);
			nightVisionButton.SetUsedState(used: false);
		}
		bool @bool = GameProgress.GetBool(Singleton<GameManager>.Instance.CurrentSceneName + "_autobuild_available");
		superBluePrintButton.SetUsedState(@bool);
		SetSuperAutoBuildAvailable(@bool);
		EventManager.Connect<AutoBuildEvent>(SetSuperAutoBuildButtonAmount);
		EventManager.Connect<ApplySuperGlueEvent>(SetSuperGlueButtonAmount);
		EventManager.Connect<ApplySuperMagnetEvent>(SetSuperMagnetButtonAmount);
		EventManager.Connect<ApplyTurboChargeEvent>(SetTurboChargeButtonAmount);
		EventManager.Connect<ApplyNightVisionEvent>(SetNightVisionButtonAmount);
		OnPurchaseSucceeded component = superGlueButton.GetComponent<OnPurchaseSucceeded>();
		if (component != null)
		{
			component.buildMenu = this;
		}
		component = superMagnetButton.GetComponent<OnPurchaseSucceeded>();
		if (component != null)
		{
			component.buildMenu = this;
		}
		component = turboChargeButton.GetComponent<OnPurchaseSucceeded>();
		if (component != null)
		{
			component.buildMenu = this;
		}
		component = nightVisionButton.GetComponent<OnPurchaseSucceeded>();
		if (component != null)
		{
			component.buildMenu = this;
		}
		component = superBluePrintButton.GetComponent<OnPurchaseSucceeded>();
		if (component != null)
		{
			component.buildMenu = this;
		}
		if ((bool)editorButtons)
		{
			editorButtons.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		EventManager.Disconnect<AutoBuildEvent>(SetSuperAutoBuildButtonAmount);
		EventManager.Disconnect<ApplySuperGlueEvent>(SetSuperGlueButtonAmount);
		EventManager.Disconnect<ApplySuperMagnetEvent>(SetSuperMagnetButtonAmount);
		EventManager.Disconnect<ApplyTurboChargeEvent>(SetTurboChargeButtonAmount);
		EventManager.Disconnect<ApplyNightVisionEvent>(SetNightVisionButtonAmount);
	}

	public void SetSuperAutoBuildAvailable(bool available)
	{
		if (available)
		{
			autoBuildButton.gameObject.SetActive(value: false);
			superBuildSelection.gameObject.SetActive(value: true);
		}
	}

	public void RefreshPowerUpCounts()
	{
		SetSuperAutoBuildButtonAmount(new AutoBuildEvent(GameProgress.BluePrintCount(), usedState: false));
		SetSuperGlueButtonAmount(new ApplySuperGlueEvent(GameProgress.SuperGlueCount(), usedState: false));
		SetSuperMagnetButtonAmount(new ApplySuperMagnetEvent(GameProgress.SuperMagnetCount(), usedState: false));
		SetTurboChargeButtonAmount(new ApplyTurboChargeEvent(GameProgress.TurboChargeCount(), usedState: false));
		SetNightVisionButtonAmount(new ApplyNightVisionEvent(GameProgress.NightVisionCount(), usedState: false));
	}

	private void SetSuperAutoBuildButtonAmount(AutoBuildEvent eventData)
	{
		SetButtonAmountText(superBluePrintButton, eventData.availableBlueprints);
		superBluePrintButton.SetUsedState(eventData.usedState);
	}

	private void SetSuperGlueButtonAmount(ApplySuperGlueEvent eventData)
	{
		SetButtonAmountText(superGlueButton, eventData.availableSuperGlue);
		superGlueButton.SetUsedState(eventData.usedState);
	}

	private void SetSuperMagnetButtonAmount(ApplySuperMagnetEvent eventData)
	{
		SetButtonAmountText(superMagnetButton, eventData.availableSuperMagnet);
		superMagnetButton.SetUsedState(eventData.usedState);
	}

	private void SetTurboChargeButtonAmount(ApplyTurboChargeEvent eventData)
	{
		SetButtonAmountText(turboChargeButton, eventData.availableTurboCharge);
		turboChargeButton.SetUsedState(eventData.usedState);
	}

	private void SetNightVisionButtonAmount(ApplyNightVisionEvent eventData)
	{
		SetButtonAmountText(nightVisionButton, eventData.availableNightVision);
		nightVisionButton.SetUsedState(eventData.usedState);
	}

	private void SetTrackIndex()
	{
	}

	public void NextTrack()
	{
	}

	public void PreviousTrack()
	{
	}
}
