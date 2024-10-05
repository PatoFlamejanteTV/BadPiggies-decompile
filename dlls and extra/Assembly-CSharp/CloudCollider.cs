using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCollider : WPFMonoBehaviour
{
	public GameObject particleEffect;

	private List<BasePart> visitedContraptionPartList;

	private Pig pig;

	private void Start()
	{
		EventManager.Connect<GameStateChanged>(OnGameStateChanged);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
	}

	private void OnGameStateChanged(GameStateChanged newState)
	{
		if (newState.state == LevelManager.GameState.Running)
		{
			visitedContraptionPartList = new List<BasePart>();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		BasePart component = other.GetComponent<BasePart>();
		if (!component || visitedContraptionPartList.Contains(component))
		{
			return;
		}
		if (component.m_partType == BasePart.PartType.Pig)
		{
			pig = (Pig)component;
		}
		WPFMonoBehaviour.levelManager.ContraptionRunning.FinishConnectedComponentSearch();
		List<BasePart> parts = WPFMonoBehaviour.levelManager.ContraptionRunning.Parts;
		for (int i = 0; i < parts.Count; i++)
		{
			BasePart basePart = parts[i];
			if ((bool)basePart && basePart.ConnectedComponent == component.ConnectedComponent)
			{
				visitedContraptionPartList.Add(basePart);
				if (basePart.m_partType == BasePart.PartType.Pig)
				{
					pig = (Pig)basePart;
				}
			}
		}
		visitedContraptionPartList.Add(component);
		if (pig != null)
		{
			pig.CheckCameraLimits = false;
		}
		if ((bool)particleEffect)
		{
			WPFMonoBehaviour.effectManager.CreateParticles(particleEffect, component.transform.position, force: true);
			if (Singleton<SocialGameManager>.IsInstantiated())
			{
				Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.WHEN_PIGS_FALL", 100.0);
			}
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.pigFall, other.transform.position);
			StartCoroutine(DelayedToBuilding(5f));
		}
	}

	private IEnumerator DelayedToBuilding(float duration)
	{
		yield return new WaitForSeconds(duration);
		if (pig != null)
		{
			pig.CheckCameraLimits = true;
		}
	}
}
