using System;
using UnityEngine;

public class PlayerLevelRequirement : MonoBehaviour
{
	public Action<bool> OnUnlock;

	[SerializeField]
	private GameObject lockedContainer;

	[SerializeField]
	private GameObject unlockedContainer;

	[SerializeField]
	private Collider lockCollider;

	[SerializeField]
	private TextMesh[] levelLabel;

	[SerializeField]
	private int levelRequirement;

	[SerializeField]
	private string levelRequirementKey = string.Empty;

	private const string CONFIG_LEVEL_REQUIREMENT_KEY = "level_requirements";

	private bool isLocked = true;

	public bool IsLocked => isLocked;

	public int RequiredLevel => levelRequirement;

	public static int GetRequiredLevel(string requirementKey)
	{
		int result = -1;
		if (Singleton<GameConfigurationManager>.Instance.HasValue("level_requirements", requirementKey))
		{
			result = Singleton<GameConfigurationManager>.Instance.GetValue<int>("level_requirements", requirementKey);
		}
		return result;
	}

	public string GetConfigKey()
	{
		return levelRequirementKey;
	}

	private void OnEnable()
	{
		int requiredLevel = GetRequiredLevel(levelRequirementKey);
		if (requiredLevel >= 0)
		{
			levelRequirement = requiredLevel;
		}
		Lock(levelRequirement > Singleton<PlayerProgress>.Instance.Level);
		EventManager.Connect<PlayerProgressEvent>(OnPlayerProgressEvent);
	}

	private void OnDisable()
	{
		EventManager.Disconnect<PlayerProgressEvent>(OnPlayerProgressEvent);
	}

	private void OnPlayerProgressEvent(PlayerProgressEvent data)
	{
		Lock(levelRequirement > Singleton<PlayerProgress>.Instance.Level);
	}

	private void Lock(bool doLock)
	{
		isLocked = doLock;
		if ((bool)lockedContainer)
		{
			lockedContainer.SetActive(doLock);
		}
		if ((bool)unlockedContainer)
		{
			unlockedContainer.SetActive(!doLock);
		}
		if (doLock)
		{
			TextMeshHelper.UpdateTextMeshes(levelLabel, levelRequirement.ToString());
		}
		if (OnUnlock != null)
		{
			OnUnlock(!doLock);
		}
	}
}
