using System;
using UnityEngine;

public class WorkshopIntro : MonoBehaviour
{
	public static Action OnPressedOk;

	[SerializeField]
	private GameObject okButton;

	private int reward;

	private void Awake()
	{
		if (Singleton<GameConfigurationManager>.Instance.HasData)
		{
			Initialize();
			return;
		}
		GameConfigurationManager instance = Singleton<GameConfigurationManager>.Instance;
		instance.OnHasData = (Action)Delegate.Combine(instance.OnHasData, new Action(Initialize));
	}

	private void Start()
	{
		ChangeButtonText();
	}

	private void OnEnable()
	{
		BackgroundMask.Show(show: true, this, "Popup", base.transform, Vector3.forward);
		EventManager.Send(new UIEvent(UIEvent.Type.OpenedWorkshopIntroduction));
	}

	private void OnDisable()
	{
		BackgroundMask.Show(show: false, this, string.Empty);
		EventManager.Send(new UIEvent(UIEvent.Type.ClosedWorkshopIntroduction));
	}

	private void OnDestroy()
	{
		if (Singleton<GameConfigurationManager>.IsInstantiated())
		{
			GameConfigurationManager instance = Singleton<GameConfigurationManager>.Instance;
			instance.OnHasData = (Action)Delegate.Remove(instance.OnHasData, new Action(Initialize));
		}
	}

	private void Initialize()
	{
		GetComponent<TextDialog>().onClose += OnPressOk;
		reward = Singleton<GameConfigurationManager>.Instance.GetValue<int>("part_crafting_prices", "Common");
	}

	private void OnPressOk()
	{
		GetComponent<TextDialog>().onClose -= OnPressOk;
		int num = Mathf.Clamp(reward, 0, 200);
		ScrapButton.Instance.AddParticles(base.gameObject, num, 0f, num);
		GameProgress.AddScrap(reward);
		if (OnPressedOk != null)
		{
			OnPressedOk();
		}
	}

	private void ChangeButtonText()
	{
		if (!(okButton == null))
		{
			TextMesh component = okButton.GetComponent<TextMesh>();
			TextMeshLocale component2 = okButton.GetComponent<TextMeshLocale>();
			if (!(component == null) && !(component2 == null))
			{
				component2.RefreshTranslation(component.text);
				component2.enabled = false;
				component.text = string.Format(component.text, reward);
			}
		}
	}
}
