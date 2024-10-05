using UnityEngine;

public class Engine : BasePart
{
	public bool m_running;

	private Transform m_visualizationPart;

	private Vector3 m_visualizationPartPosition;

	private bool m_engineBroken;

	private GameObject loopingSound;

	private AudioSource m_engineSound;

	private AudioManager audioManager;

	public ParticleSystem smokeEmitter;

	public ParticleSystem flameEmitter;

	private float m_power;

	public override bool CanBeEnabled()
	{
		return true;
	}

	public override bool HasOnOffToggle()
	{
		return true;
	}

	public override bool IsEnabled()
	{
		return m_running;
	}

	public override bool IsIntegralPart()
	{
		return true;
	}

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override bool ValidatePart()
	{
		return m_enclosedInto != null;
	}

	public override void Initialize()
	{
		m_power = 10000f;
		base.contraption.m_enginesAmount++;
		m_visualizationPart = base.transform.GetChild(0);
		m_visualizationPartPosition = m_visualizationPart.localPosition;
	}

	public override void OnDetach()
	{
		m_engineBroken = true;
		if (m_running)
		{
			SetEnabled(enabled: false);
		}
		base.contraption.m_enginesAmount--;
		audioManager.RemoveCombinedLoopingEffect(m_engineSound, loopingSound);
		base.OnDetach();
	}

	private void Start()
	{
		audioManager = Singleton<AudioManager>.Instance;
		switch (m_partType)
		{
		case PartType.EngineBig:
			m_engineSound = WPFMonoBehaviour.gameData.commonAudioCollection.V8Engine;
			break;
		case PartType.Engine:
			m_engineSound = WPFMonoBehaviour.gameData.commonAudioCollection.engine;
			break;
		case PartType.EngineSmall:
			if (HasTag("Alien"))
			{
				m_engineSound = WPFMonoBehaviour.gameData.commonAudioCollection.alienEngineLoop;
			}
			else
			{
				m_engineSound = WPFMonoBehaviour.gameData.commonAudioCollection.electricEngine;
			}
			break;
		}
	}

	private void Update()
	{
		if (m_running && !loopingSound)
		{
			loopingSound = audioManager.SpawnCombinedLoopingEffect(m_engineSound, base.gameObject.transform);
			loopingSound.GetComponent<AudioSource>().pitch = Mathf.Clamp(0.8f + 0.1f * (float)base.contraption.m_enginesAmount, 0f, 1f);
		}
		else if (!m_running && (bool)loopingSound)
		{
			audioManager.RemoveCombinedLoopingEffect(m_engineSound, loopingSound);
			loopingSound = null;
		}
		if (m_running)
		{
			PlayEngineAnimation();
		}
	}

	private void PlayEngineAnimation()
	{
		if (Time.deltaTime > 0f)
		{
			m_visualizationPart.localPosition = m_visualizationPartPosition + (Vector3)Random.insideUnitCircle * 0.1f;
		}
	}

	protected override void OnTouch()
	{
		if (base.contraption.ActivateAllPoweredParts(base.ConnectedComponent) == 0)
		{
			SetEnabled(!m_running);
		}
	}

	public override void SetEnabled(bool enabled)
	{
		m_running = enabled && !m_engineBroken;
		if (smokeEmitter != null)
		{
			if (m_running)
			{
				smokeEmitter.Play();
			}
			else
			{
				smokeEmitter.Stop();
			}
		}
		if (flameEmitter != null)
		{
			if (m_running)
			{
				flameEmitter.Play();
			}
			else
			{
				flameEmitter.Stop();
			}
		}
	}
}
