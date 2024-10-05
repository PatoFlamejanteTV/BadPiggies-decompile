using System.Collections;
using UnityEngine;

public class SecretStatue : OneTimeCollectable
{
	[SerializeField]
	private GameObject statuePopup;

	private PointLightSource pls;

	protected override void Start()
	{
		base.Start();
		pls = GetComponent<PointLightSource>();
		if (GameProgress.GetBool("SECRET_DISCOVERED_" + Singleton<GameManager>.Instance.CurrentSceneName + "_statue"))
		{
			DisableGoal();
		}
	}

	private void OnTriggerEnter(Collider c)
	{
		CheckIfSeen(c);
	}

	private void OnTriggerStay(Collider c)
	{
		CheckIfSeen(c);
	}

	private void CheckIfSeen(Collider c)
	{
		if (disabled || WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Completed)
		{
			return;
		}
		LightTrigger component = c.GetComponent<LightTrigger>();
		if (!component)
		{
			return;
		}
		PointLightSource lightSource = component.LightSource;
		if ((bool)lightSource && lightSource.lightType == PointLightMask.LightType.PointLight)
		{
			Collect();
		}
		else
		{
			if (lightSource.lightType != PointLightMask.LightType.BeamLight)
			{
				return;
			}
			if (Vector3.Distance(base.transform.position, lightSource.beamArcCenter) < lightSource.colliderSize)
			{
				Collect();
				return;
			}
			float beamAngle = lightSource.beamAngle;
			Vector3 vector = Vector3.up * base.transform.position.y + Vector3.right * base.transform.position.x;
			Vector3 vector2 = Vector3.up * c.transform.position.y + Vector3.right * c.transform.position.x;
			if (Vector3.Angle(vector - vector2, lightSource.transform.up) < beamAngle * 0.5f)
			{
				Collect();
			}
		}
	}

	protected override void DisableGoal(bool disable = true)
	{
		disabled = disable;
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i] is Collider)
			{
				(components[i] as Collider).enabled = !disable;
			}
		}
		pls.usesCurves = !disable;
		pls.isEnabled = disable;
	}

	public override void Collect()
	{
		if (!collected)
		{
			if ((bool)collectedEffect)
			{
				Object.Instantiate(collectedEffect, base.transform.position, base.transform.rotation);
			}
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.secretStatueFound);
			Singleton<PlayerProgress>.Instance.AddExperience(PlayerProgress.ExperienceType.HiddenStatueFound);
			if (GameProgress.SecretStatueCount() == GameProgress.MaxStatueCount())
			{
				Singleton<PlayerProgress>.Instance.AddExperience(PlayerProgress.ExperienceType.AllHiddenStatuesFound);
			}
			collected = true;
			pls.isEnabled = collected;
			OnCollected();
		}
	}

	public override void OnCollected()
	{
		StartCoroutine(ShowPopup());
		GameProgress.SetBool("SECRET_DISCOVERED_" + Singleton<GameManager>.Instance.CurrentSceneName + "_statue", value: true);
		GameProgress.AddSecretStatue();
		int count = GameProgress.SecretStatueCount();
		if (Singleton<SocialGameManager>.IsInstantiated())
		{
			Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.BELONGS_IN_MUSEUM", 100.0);
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.ARCHEOLOGIST", 100.0, (int limit) => count >= limit);
		}
	}

	private IEnumerator ShowPopup()
	{
		yield return new WaitForSeconds(0.5f);
		Object.Instantiate(statuePopup.gameObject, WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward * 50f, Quaternion.identity);
	}

	protected override string GetNameKey()
	{
		return string.Empty;
	}
}
