using System;
using UnityEngine;

public class PlayerProgressBar : MonoBehaviour, ICurrencyParticleEffectTarget
{
	private enum State
	{
		None,
		Regular,
		WaitingLevelActive,
		WaitingLevelInactive
	}

	private static PlayerProgressBar instance;

	private const string LEVEL_UP_LOCALIZATION_KEY = "LOOT_WHEEL_TITLE";

	[SerializeField]
	private Transform experienceFillMeter;

	[SerializeField]
	private TextMesh[] experienceLabel;

	[SerializeField]
	private TextMesh[] levelLabel;

	[SerializeField]
	private Animation pulseAnimation;

	[SerializeField]
	private GameObject shine;

	private float maxFill = 15f;

	private bool delayUpdate;

	private Action<bool> onFinished;

	private bool canLevelUp = true;

	private int particlesRemaining;

	private CurrencyParticleEffect effect;

	private int targetExperience;

	private int currentExperience;

	private int currentRealExperience;

	private float nextUpdate;

	private ResourceBarItem resourceBarItem;

	private State currentState;

	public static PlayerProgressBar Instance => instance;

	public bool CanLevelUp
	{
		set
		{
			canLevelUp = value;
			if (canLevelUp && currentState == State.WaitingLevelInactive)
			{
				SetState(State.WaitingLevelActive);
			}
			else if (!canLevelUp && currentState == State.WaitingLevelActive)
			{
				SetState(State.WaitingLevelInactive);
			}
		}
	}

	public bool Visible => ResourceBar.Instance.IsItemActive(ResourceBar.Item.PlayerProgress);

	public bool Enabled => ResourceBar.Instance.IsItemEnabled(ResourceBar.Item.PlayerProgress);

	private void Awake()
	{
		instance = this;
		currentState = State.None;
		SetState(State.Regular);
		resourceBarItem = GetComponent<ResourceBarItem>();
		effect = GetComponent<CurrencyParticleEffect>();
		effect.SetTarget(this);
		maxFill = experienceFillMeter.localScale.x;
		EventManager.Connect<PlayerProgressEvent>(OnPlayerProgress);
		EventManager.Connect<UIEvent>(OnReceivedUIEvent);
		EventManager.Connect<LevelLoadedEvent>(OnLevelLoaded);
		EventManager.Connect<PlayerChangedEvent>(OnPlayerChanged);
	}

	private void OnPlayerChanged(PlayerChangedEvent data)
	{
		delayUpdate = false;
		SetState(State.Regular);
		UpdateAmount(force: true);
	}

	private void OnEnable()
	{
		nextUpdate = Time.realtimeSinceStartup + 2f;
		UpdateAmount(force: true);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<PlayerProgressEvent>(OnPlayerProgress);
		EventManager.Disconnect<UIEvent>(OnReceivedUIEvent);
		EventManager.Disconnect<LevelLoadedEvent>(OnLevelLoaded);
		EventManager.Disconnect<PlayerChangedEvent>(OnPlayerChanged);
	}

	private void OnPlayerProgress(PlayerProgressEvent data)
	{
		if (!delayUpdate)
		{
			targetExperience = data.experience;
		}
		currentRealExperience = data.experience;
	}

	private void OnReceivedUIEvent(UIEvent data)
	{
		if (!Singleton<GameManager>.IsInstantiated())
		{
			return;
		}
		if (Singleton<GameManager>.Instance.GetGameState() == GameManager.GameState.MainMenu)
		{
			ResourceBar.Instance.ShowItem(ResourceBar.Item.PlayerProgress, showItem: false, enableItem: false);
			return;
		}
		switch (data.type)
		{
		case UIEvent.Type.ClosedLootWheel:
			if (Singleton<PlayerProgress>.Instance.LevelUpPending && ResourceBar.Instance.IsItemEnabled(ResourceBar.Item.PlayerProgress))
			{
				SetState(State.WaitingLevelActive);
			}
			else if (Singleton<PlayerProgress>.Instance.LevelUpPending && !ResourceBar.Instance.IsItemEnabled(ResourceBar.Item.PlayerProgress))
			{
				SetState(State.WaitingLevelInactive);
			}
			else
			{
				SetState(State.Regular);
			}
			break;
		case UIEvent.Type.OpenedLootWheel:
			SetState(State.Regular);
			UpdateAmount(force: true);
			break;
		}
	}

	private void OnLevelLoaded(LevelLoadedEvent data)
	{
		if (data.currentGameState == GameManager.GameState.MainMenu)
		{
			UpdateAmount(force: true);
		}
		UpdateAmount();
	}

	public void LevelUp()
	{
		Singleton<PlayerProgress>.Instance.CheckLevelUp();
	}

	public void DelayUpdate()
	{
		delayUpdate = true;
	}

	public void AddParticles(GameObject target, int amount, float delay = 0f, float burstRate = 0f, Action<bool> onFinished = null)
	{
		delayUpdate = false;
		if (effect == null)
		{
			return;
		}
		float num = 0f;
		for (int i = 0; i < amount; i++)
		{
			if (!Mathf.Approximately(burstRate, 0f) && burstRate > 0f)
			{
				num += 1f / burstRate;
			}
			effect.AddParticle(target.transform, UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(5f, 25f), delay + num);
		}
		if (onFinished != null)
		{
			particlesRemaining = amount;
			this.onFinished = onFinished;
		}
	}

	public void AddParticles(Vector3 position, int amount, float delay = 0f, float burstRate = 0f, Action<bool> onFinished = null)
	{
		delayUpdate = false;
		if (effect == null)
		{
			return;
		}
		float num = 0f;
		for (int i = 0; i < amount; i++)
		{
			if (!Mathf.Approximately(burstRate, 0f) && burstRate > 0f)
			{
				num += 1f / burstRate;
			}
			effect.AddParticle(position, UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(20f, 25f), delay + num);
		}
		if (onFinished != null)
		{
			particlesRemaining = amount;
			this.onFinished = onFinished;
		}
	}

	private void Update()
	{
		if (!Singleton<PlayerProgress>.IsInstantiated())
		{
			return;
		}
		if (currentExperience == Singleton<PlayerProgress>.Instance.ExperienceToNextLevel && Singleton<PlayerProgress>.Instance.LevelUpPending && currentState == State.Regular)
		{
			SetState((!ResourceBar.Instance.IsItemEnabled(ResourceBar.Item.PlayerProgress) || !canLevelUp) ? State.WaitingLevelInactive : State.WaitingLevelActive);
		}
		if (currentExperience != targetExperience && Time.realtimeSinceStartup >= nextUpdate && resourceBarItem.IsShowing && currentState == State.Regular)
		{
			nextUpdate = Time.realtimeSinceStartup + SoftCurrencyButton.updateInterval;
			int deltaAmount = SoftCurrencyButton.GetDeltaAmount(currentExperience, targetExperience);
			if (currentExperience < targetExperience)
			{
				currentExperience += deltaAmount;
			}
			else if (currentExperience > targetExperience)
			{
				currentExperience -= deltaAmount;
			}
			int experienceToNextLevel = Singleton<PlayerProgress>.Instance.ExperienceToNextLevel;
			int level = Singleton<PlayerProgress>.Instance.Level;
			currentExperience = Mathf.Min(currentExperience, experienceToNextLevel);
			TextMeshHelper.UpdateTextMeshes(levelLabel, level.ToString());
			TextMeshHelper.UpdateTextMeshes(experienceLabel, $"{currentExperience}/{experienceToNextLevel}");
			float num = Mathf.Clamp((float)currentExperience / (float)experienceToNextLevel, 0.001f, 1f);
			experienceFillMeter.localScale = Vector3.forward + Vector3.up + Vector3.right * num * maxFill;
		}
	}

	private void SetLevelUpText()
	{
		TextMeshHelper.UpdateTextMeshes(experienceLabel, "LOOT_WHEEL_TITLE", refreshTranslations: true);
	}

	private void EnableWobble(bool enable)
	{
		if (enable)
		{
			pulseAnimation.Play();
			return;
		}
		pulseAnimation.Stop();
		base.transform.localScale = Vector3.one;
	}

	private void EnableShine(bool enable)
	{
		shine.SetActive(enable);
	}

	private void SetState(State state)
	{
		if (state != currentState)
		{
			switch (state)
			{
			case State.WaitingLevelInactive:
				EnableWobble(enable: false);
				EnableShine(enable: true);
				SetLevelUpText();
				break;
			case State.WaitingLevelActive:
				EnableWobble(enable: true);
				EnableShine(enable: true);
				SetLevelUpText();
				break;
			case State.Regular:
				EnableWobble(enable: false);
				EnableShine(enable: false);
				break;
			}
			currentState = state;
		}
	}

	public void CurrencyParticleAdded(int amount = 1)
	{
		targetExperience += amount;
		particlesRemaining = Mathf.Clamp(particlesRemaining - amount, 0, int.MaxValue);
		if (onFinished != null && particlesRemaining <= 0)
		{
			onFinished(Visible);
			UpdateAmount(force: true);
			onFinished = null;
		}
	}

	public void UpdateAmount(bool force = false)
	{
		if (!Singleton<PlayerProgress>.IsInstantiated())
		{
			return;
		}
		nextUpdate = Time.realtimeSinceStartup;
		targetExperience = currentRealExperience;
		if (!force || currentState != State.Regular)
		{
			return;
		}
		int experienceToNextLevel = Singleton<PlayerProgress>.Instance.ExperienceToNextLevel;
		currentRealExperience = Singleton<PlayerProgress>.Instance.Experience;
		targetExperience = currentRealExperience;
		currentExperience = targetExperience;
		TextMeshHelper.UpdateTextMeshes(levelLabel, Singleton<PlayerProgress>.Instance.Level.ToString());
		if (Singleton<PlayerProgress>.Instance.LevelUpPending)
		{
			experienceFillMeter.localScale = Vector3.forward + Vector3.up + Vector3.right * maxFill;
			if (ResourceBar.Instance.IsItemEnabled(ResourceBar.Item.PlayerProgress) && canLevelUp)
			{
				SetState(State.WaitingLevelActive);
			}
			else
			{
				SetState(State.WaitingLevelInactive);
			}
		}
		else
		{
			TextMeshHelper.UpdateTextMeshes(experienceLabel, $"{targetExperience}/{experienceToNextLevel}");
			float num = Mathf.Clamp((float)currentExperience / (float)experienceToNextLevel, 0.001f, 1f);
			experienceFillMeter.localScale = Vector3.forward + Vector3.up + Vector3.right * num * maxFill;
		}
	}

	private void OnEnableButton(GameObject sender)
	{
		if (currentState == State.WaitingLevelInactive)
		{
			SetState(State.WaitingLevelActive);
		}
	}

	private void OnDisableButton(GameObject sender)
	{
		if (currentState == State.WaitingLevelActive)
		{
			SetState(State.WaitingLevelInactive);
		}
	}

	public AudioSource[] GetHitSounds()
	{
		return WPFMonoBehaviour.gameData.commonAudioCollection.xpGain;
	}

	public AudioSource[] GetFlySounds()
	{
		return WPFMonoBehaviour.gameData.commonAudioCollection.nutFly;
	}
}
