using System.Collections.Generic;
using UnityEngine;

public class LootWheelPopup : TextDialog
{
	private static LootWheelPopup s_instance;

	private Dictionary<MeshRenderer, Material[]> rendererMaterialPair;

	[SerializeField]
	private LootWheel lootWheel;

	[SerializeField]
	private GameObject enabledDoneButton;

	[SerializeField]
	private GameObject disabledDoneButton;

	[SerializeField]
	private GameObject enabledSpinButton;

	[SerializeField]
	private GameObject disabledSpinButton;

	private bool coinsWasVisible;

	private bool scrapWasVsibile;

	public bool SpinButtonEnabled
	{
		get
		{
			return enabledSpinButton.activeSelf;
		}
		set
		{
			enabledSpinButton.SetActive(value);
			disabledSpinButton.SetActive(!value);
		}
	}

	public string SpinButtonText
	{
		set
		{
			TextMesh[] componentsInChildren = enabledSpinButton.transform.parent.GetComponentsInChildren<TextMesh>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].text = value;
				TextMeshSpriteIcons.EnsureSpriteIcon(componentsInChildren[i]);
			}
		}
	}

	public bool DoneButtonEnabled
	{
		get
		{
			return enabledDoneButton.activeSelf;
		}
		set
		{
			enabledDoneButton.SetActive(value);
			disabledDoneButton.SetActive(!value);
		}
	}

	public bool DoneButtonHidden
	{
		get
		{
			if (enabledDoneButton.activeSelf)
			{
				return disabledDoneButton.activeSelf;
			}
			return false;
		}
		set
		{
			enabledDoneButton.SetActive(!value);
			disabledDoneButton.SetActive(!value);
		}
	}

	public new static bool DialogOpen
	{
		get
		{
			if (!(s_instance == null))
			{
				return s_instance.IsOpen;
			}
			return false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (s_instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		rendererMaterialPair = new Dictionary<MeshRenderer, Material[]>();
		MeshRenderer[] componentsInChildren = enabledSpinButton.transform.parent.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			rendererMaterialPair.Add(componentsInChildren[i], componentsInChildren[i].sharedMaterials);
		}
		EventManager.Connect<LevelUpEvent>(OnLevelUpEvent);
		Object.DontDestroyOnLoad(this);
		s_instance = this;
	}

	protected override void Start()
	{
		base.Start();
		Close();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		scrapWasVsibile = ResourceBar.Instance.IsItemActive(ResourceBar.Item.Scrap);
		if (scrapWasVsibile)
		{
			ResourceBar.Instance.ShowItem(ResourceBar.Item.Scrap, showItem: false);
		}
		EventManager.Send(new UIEvent(UIEvent.Type.OpenedLootWheel));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (scrapWasVsibile)
		{
			ResourceBar.Instance.ShowItem(ResourceBar.Item.Scrap, showItem: true);
		}
		EventManager.Send(new UIEvent(UIEvent.Type.ClosedLootWheel));
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<LevelUpEvent>(OnLevelUpEvent);
	}

	private void OnLevelUpEvent(LevelUpEvent data)
	{
		Open();
	}

	public override void Open()
	{
		base.Open();
		base.transform.position = WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 6f;
		lootWheel.ForceReInit();
		Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.jokerLevelUnlocked);
	}

	public void RefreshSpinButtonTranslation()
	{
		TextMeshLocale[] componentsInChildren = enabledSpinButton.transform.parent.GetComponentsInChildren<TextMeshLocale>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].RefreshTranslation();
		}
	}

	public void ResetSpinButtonTextMaterials()
	{
		if (rendererMaterialPair == null)
		{
			return;
		}
		foreach (KeyValuePair<MeshRenderer, Material[]> item in rendererMaterialPair)
		{
			item.Key.sharedMaterials = item.Value;
		}
	}
}
