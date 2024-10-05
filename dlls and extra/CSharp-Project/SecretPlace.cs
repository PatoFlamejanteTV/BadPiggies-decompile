using System.Collections;
using UnityEngine;

public class SecretPlace : OneTimeCollectable
{
	private bool m_disablingGoal;

	private float m_animationTimer;

	[SerializeField]
	private SkullPopup m_skullPopup;

	protected override void Start()
	{
		base.Start();
		m_disablingGoal = false;
		if (GameProgress.GetBool("SECRET_DISCOVERED_" + Singleton<GameManager>.Instance.CurrentSceneName))
		{
			DisableGoal();
		}
	}

	public override void Collect()
	{
		if (!collected)
		{
			if ((bool)collectedEffect)
			{
				Object.Instantiate(collectedEffect, base.transform.position, base.transform.rotation);
			}
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.bonusBoxCollected);
			Singleton<PlayerProgress>.Instance.AddExperience(PlayerProgress.ExperienceType.HiddenSkullFound);
			if (GameProgress.SecretSkullCount() == GameProgress.MaxSkullCount())
			{
				Singleton<PlayerProgress>.Instance.AddExperience(PlayerProgress.ExperienceType.AllHiddenSkullsFound);
			}
			collected = true;
			m_disablingGoal = true;
			m_animationTimer = 0f;
			OnCollected();
		}
	}

	private void Update()
	{
		if (m_disablingGoal)
		{
			m_animationTimer += Time.deltaTime;
			if (m_animationTimer < 0.2f)
			{
				base.transform.localScale += Vector3.one * Time.deltaTime;
				return;
			}
			if (m_animationTimer < 1f)
			{
				base.transform.localScale -= Vector3.one * Time.deltaTime;
				return;
			}
			DisableGoal();
			m_disablingGoal = false;
		}
	}

	public override void OnCollected()
	{
		StartCoroutine(ShowPopup());
		GameProgress.SetBool("SECRET_DISCOVERED_" + Singleton<GameManager>.Instance.CurrentSceneName, value: true);
		GameProgress.AddSecretSkull();
		int n = GameProgress.SecretSkullCount();
		if (Singleton<SocialGameManager>.IsInstantiated())
		{
			Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.BRUSH", 100.0);
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.IS_IT_SECRET", 100.0, (int limit) => n >= limit);
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.SECRET_ADMIRER", 100.0, (int limit) => n >= limit);
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.MASTER_EXPLORER", 100.0, (int limit) => n >= limit);
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.MEGA_MASTER_EXPLORER", 100.0, (int limit) => n >= limit);
		}
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0.5f);
		Object.Instantiate(m_skullPopup.gameObject, WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 50f, Quaternion.identity);
	}

	protected override string GetNameKey()
	{
		return string.Empty;
	}
}
