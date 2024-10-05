using System.Collections;
using UnityEngine;

public class ButtonLock : MonoBehaviour
{
	public string m_buttonId = "undefined";

	public AudioSource m_unlockSound;

	public ParticleSystem m_unlockParticles;

	private bool m_started;

	[SerializeField]
	private RaceLevelSelector m_raceLevelSelector;

	private bool isBeingUnlocked;

	private void OnEnable()
	{
		if (isBeingUnlocked)
		{
			NotifyUnlocked();
		}
	}

	private void OnDisable()
	{
		isBeingUnlocked = GetComponent<Animation>().isPlaying;
	}

	private void Start()
	{
		UpdateState();
		m_started = true;
	}

	public void NotifyUnlocked()
	{
		if (m_started)
		{
			UpdateState();
		}
	}

	private void UpdateState()
	{
		switch (GameProgress.GetButtonUnlockState(m_buttonId))
		{
		case GameProgress.ButtonUnlockState.Unlocked:
			base.gameObject.SetActive(value: false);
			return;
		case GameProgress.ButtonUnlockState.UnlockNow:
			Unlock();
			return;
		}
		base.gameObject.SetActive(value: true);
		if (base.transform.parent.GetComponent<TooltipInfo>() == null)
		{
			base.transform.parent.GetComponent<Button>().Lock(lockState: true);
		}
	}

	private void Unlock()
	{
		base.gameObject.SetActive(value: true);
		GameProgress.SetButtonUnlockState(m_buttonId, GameProgress.ButtonUnlockState.Unlocked);
		base.transform.parent.GetComponent<Button>().Lock(lockState: false);
		RaceLevelButton component = base.transform.parent.GetComponent<RaceLevelButton>();
		if (m_raceLevelSelector != null && GameProgress.IsLevelCompleted(m_raceLevelSelector.m_raceLevels.GetLevelData(component.m_raceLevelIdentifier).SceneName))
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		GetComponent<Animation>().Play();
		if ((bool)m_unlockSound)
		{
			Singleton<AudioManager>.Instance.Play2dEffect(m_unlockSound);
		}
		if ((bool)m_unlockParticles)
		{
			StartCoroutine(StartParticles(GetComponent<Animation>().clip.length));
		}
	}

	private IEnumerator StartParticles(float delay)
	{
		yield return new WaitForSeconds(delay);
		if ((bool)m_unlockParticles)
		{
			m_unlockParticles.Play();
		}
	}
}
