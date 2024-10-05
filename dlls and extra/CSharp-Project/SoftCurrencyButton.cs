using System.Collections;
using UnityEngine;

public class SoftCurrencyButton : MonoBehaviour, ICurrencyParticleEffectTarget
{
	public enum Position
	{
		None,
		Top,
		Bottom
	}

	[SerializeField]
	protected float screenPlacementFromTop = 1f;

	[SerializeField]
	protected float screenPlacementFromBottom = 2f;

	[SerializeField]
	protected TextMesh[] labels;

	private int targetAmount;

	private int currentAmount;

	public static float updateInterval = 0.02f;

	protected float nextUpdate;

	private ScreenPlacement screenPlacement;

	protected CurrencyParticleEffect currencyEffect;

	private Transform shinyEffect;

	private Transform plusIcon;

	private AudioSource counterLoopSource;

	public Vector3 lastLocalPosition = Vector3.zero;

	private Vector3 previousPosition = Vector3.zero;

	private bool isShining;

	public float ScreenPlacementFromTop => screenPlacementFromTop;

	public float ScreenPlacementFromBottom => screenPlacementFromBottom;

	public ScreenPlacement Placement => screenPlacement;

	public CurrencyParticleEffect CurrencyEffect => currencyEffect;

	protected virtual void ButtonAwake()
	{
	}

	private void Awake()
	{
		currencyEffect = GetComponent<CurrencyParticleEffect>();
		screenPlacement = GetComponent<ScreenPlacement>();
		if ((bool)currencyEffect)
		{
			currencyEffect.SetTarget(this);
		}
		shinyEffect = base.transform.Find("SoftCurrencyIcon/Shiny");
		plusIcon = base.transform.Find("PlusIcon");
		ShowButton(show: false);
		ButtonAwake();
		EventManager.Connect<PlayerChangedEvent>(OnPlayerChanged);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<PlayerChangedEvent>(OnPlayerChanged);
	}

	protected virtual void OnPlayerChanged(PlayerChangedEvent data)
	{
		UpdateAmount(forceUpdate: true);
	}

	protected virtual void ButtonEnabled()
	{
	}

	private void OnEnable()
	{
		nextUpdate = Time.realtimeSinceStartup + 2f;
		UpdateAmount(forceUpdate: true);
		ButtonEnabled();
	}

	protected virtual void ButtonDisabled()
	{
	}

	private void OnDisable()
	{
		isShining = false;
		if (shinyEffect != null)
		{
			shinyEffect.localScale = Vector3.one * 0.001f;
		}
		ButtonDisabled();
	}

	protected virtual int GetCurrencyCount()
	{
		return 0;
	}

	public void UpdateAmount(bool forceUpdate = false)
	{
		if (labels != null && GameProgress.Initialized)
		{
			targetAmount = GetCurrencyCount();
			nextUpdate = Time.realtimeSinceStartup;
			if (forceUpdate)
			{
				currentAmount = targetAmount;
				TextMeshHelper.UpdateTextMeshes(labels, $"{currentAmount}");
			}
		}
	}

	private IEnumerator ShinyEffect()
	{
		if (isShining || shinyEffect == null)
		{
			yield break;
		}
		isShining = true;
		float fade = 0f;
		while (fade < 1f)
		{
			fade += Time.deltaTime;
			shinyEffect.localEulerAngles = Vector3.forward * fade * 540f;
			if (fade <= 0.5f)
			{
				shinyEffect.localScale = Vector3.Lerp(Vector3.one * 0.001f, Vector3.one, fade * 2f);
			}
			else
			{
				shinyEffect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.001f, (fade - 0.5f) * 2f);
			}
			yield return null;
		}
		shinyEffect.localScale = Vector3.one * 0.001f;
		yield return new WaitForSeconds(Random.Range(10f, 15f));
		isShining = false;
	}

	public virtual AudioSource[] GetHitSounds()
	{
		return null;
	}

	public virtual AudioSource[] GetFlySounds()
	{
		return null;
	}

	protected virtual AudioSource GetLoopSound()
	{
		return null;
	}

	private void PlayLoopSound()
	{
		if (counterLoopSource == null)
		{
			AudioSource loopSound = GetLoopSound();
			if (loopSound != null)
			{
				counterLoopSource = Singleton<AudioManager>.Instance.SpawnLoopingEffect(loopSound, base.transform).GetComponent<AudioSource>();
			}
		}
		float b = ((targetAmount - currentAmount <= 0) ? 0.8f : 1.5f);
		if (counterLoopSource != null)
		{
			counterLoopSource.pitch = Mathf.Lerp(counterLoopSource.pitch, b, Time.deltaTime);
		}
	}

	private void StopLoopSound()
	{
		if (counterLoopSource != null)
		{
			GameObject loopingEffect = counterLoopSource.gameObject;
			Singleton<AudioManager>.Instance.RemoveLoopingEffect(ref loopingEffect);
			counterLoopSource.pitch = 1f;
			counterLoopSource = null;
		}
	}

	public static int GetDeltaAmount(int current, int target)
	{
		int num = Mathf.Abs(target - current);
		int result = 1;
		if (num > 1000)
		{
			result = Random.Range(500, 1000);
		}
		else if (num > 100)
		{
			result = Random.Range(50, 100);
		}
		else if (num > 20)
		{
			result = Random.Range(10, 20);
		}
		else if (num > 5)
		{
			result = Random.Range(1, 5);
		}
		return result;
	}

	protected virtual void OnUpdate()
	{
	}

	private void Update()
	{
		if (!isShining)
		{
			StartCoroutine(ShinyEffect());
		}
		if (currentAmount != targetAmount && Time.realtimeSinceStartup >= nextUpdate && labels != null)
		{
			nextUpdate = Time.realtimeSinceStartup + updateInterval;
			int deltaAmount = GetDeltaAmount(currentAmount, targetAmount);
			if (currentAmount < targetAmount)
			{
				currentAmount += deltaAmount;
			}
			else if (currentAmount > targetAmount)
			{
				currentAmount -= deltaAmount;
			}
			for (int i = 0; i < labels.Length; i++)
			{
				labels[i].text = $"{currentAmount}";
			}
		}
		if (targetAmount - currentAmount != 0)
		{
			PlayLoopSound();
		}
		else
		{
			StopLoopSound();
		}
		OnUpdate();
	}

	public virtual void AddParticles(GameObject target, int amount, float delay = 0f, float burstRate = 0f)
	{
		if (currencyEffect == null)
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
			currencyEffect.AddParticle(target.transform, Random.insideUnitCircle.normalized * Random.Range(20f, 25f), delay + num);
		}
	}

	public void CurrencyParticleAdded(int amount = 1)
	{
		targetAmount += amount;
	}

	public void EnableButton(bool enable)
	{
		if (plusIcon != null)
		{
			plusIcon.gameObject.SetActive(enable);
		}
		if (GetComponent<Collider>() != null)
		{
			GetComponent<Collider>().enabled = enable;
		}
		if (enable)
		{
			nextUpdate = Time.realtimeSinceStartup + 2f;
		}
	}

	public virtual void ShowButton(bool show = true)
	{
		UpdateAmount(forceUpdate: true);
	}

	public void RecoverToPreviousPosition()
	{
	}

	public void SetCurrencyButton(Transform target, Position pos, bool enableButton)
	{
		EnableButton(enableButton);
		ShowButton();
	}
}
