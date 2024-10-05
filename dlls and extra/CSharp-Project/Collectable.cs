using UnityEngine;

public class Collectable : Goal
{
	[SerializeField]
	protected ParticleSystem collectedEffect;

	public bool collected;

	protected bool disabled;

	private Vector3 originalPosition;

	private Renderer m_renderer;

	private Renderer[] m_childRenderers;

	private Collider m_collider;

	private Collider[] m_childColliders;

	public bool Collected => collected;

	public bool Disabled => disabled;

	public void OnDataLoaded()
	{
		originalPosition = base.transform.position;
		m_renderer = GetComponent<Renderer>();
		m_childRenderers = GetComponentsOnlyInChildren<Renderer>();
		m_collider = GetComponent<Collider>();
		m_childColliders = GetComponentsOnlyInChildren<Collider>();
		float @float = INSettings.GetFloat(INFeature.TerrainScale);
		originalPosition = new Vector3(originalPosition.x * @float, originalPosition.y * @float, originalPosition.z);
	}

	protected void DisableGoal(bool disable)
	{
		if (m_renderer != null)
		{
			m_renderer.enabled = !disable;
		}
		if (m_collider != null)
		{
			m_collider.enabled = !disable;
		}
		disabled = disable;
		HideChildren(disable);
		Rigidbody component = GetComponent<Rigidbody>();
		if ((bool)component)
		{
			component.isKinematic = disable;
		}
	}

	protected void HideChildren(bool hide)
	{
		for (int i = 0; i < m_childRenderers.Length; i++)
		{
			m_childRenderers[i].enabled = !hide;
		}
		for (int j = 0; j < m_childColliders.Length; j++)
		{
			m_childColliders[j].enabled = !hide;
		}
	}

	protected override void OnGoalEnter(BasePart part)
	{
		if (!collected && WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.Completed)
		{
			if ((bool)collectedEffect)
			{
				Object.Instantiate(collectedEffect, base.transform.position, collectedEffect.transform.rotation);
			}
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.bonusBoxCollected, base.transform.position);
			collected = true;
			EventManager.Send(default(ObjectiveAchieved));
			DisableGoal(disable: true);
		}
	}

	protected override void OnReset()
	{
		collected = false;
		base.transform.position = originalPosition;
		DisableGoal(disable: false);
	}
}
