using System;
using UnityEngine;

public class VisibilityCondition : WPFMonoBehaviour
{
	[Serializable]
	public struct ConditionStruct
	{
		public Condition condition;

		public bool not;
	}

	public enum Condition
	{
		None,
		HasValidContraption,
		ShowEngineButton,
		HasRockets,
		IsPausedWhileRunning,
		HasContraption,
		QuestModeCanBuild,
		IsPuzzleMode,
		ShowPauseMenuReplayButton,
		HasMotorWheels,
		HasFans,
		HasPropellers,
		HasRotors,
		ShowBuyBluePrintButton,
		ShowAutoBuildButton,
		ShowTutorialButton,
		IsAutoBuilding,
		CanClearContraption,
		IsNotAutoBuilding,
		ShowBuildModeButtons,
		IAPEnabled,
		ChiefPigExploded,
		ShowSuperMechanicSwitch,
		IsSandbox,
		ShowSchematicsButton,
		EveryPlayAvailable,
		EveryPlayAvailableAndRecorded,
		EveryPlayRecording,
		GameCenterAvailable,
		IsFreeVersion,
		IsHDVersion,
		IsOdyssey,
		IsIOS,
		CheatsEnabled,
		IsDebugBuild,
		CollectedFreeShopLootcrate,
		HasNewParts,
		LessCheats,
		HasNetwork,
		BoughtFieldOfDreams,
		DailyChallengeComplete,
		IsCakeRaceMode,
		IsDecember
	}

	public Condition condition;

	public bool not;

	[SerializeField]
	private bool disableGameObject;

	private Renderer m_renderer;

	private Collider m_collider;

	private Transform m_transform;

	private void Awake()
	{
		m_renderer = base.gameObject.GetComponent<Renderer>();
		m_collider = base.gameObject.GetComponent<Collider>();
		m_transform = base.gameObject.transform;
		if (Singleton<VisibilityConditionManager>.IsInstantiated())
		{
			Singleton<VisibilityConditionManager>.Instance.SubscribeToConditionChange(SetEnabled, condition);
		}
	}

	private void OnDestroy()
	{
		if (Singleton<VisibilityConditionManager>.IsInstantiated())
		{
			Singleton<VisibilityConditionManager>.Instance.UnsubscribeToConditionChange(SetEnabled, condition);
		}
	}

	public void UpdateState()
	{
		if (Singleton<VisibilityConditionManager>.IsInstantiated())
		{
			SetEnabled(condition, Singleton<VisibilityConditionManager>.Instance.GetState(condition));
		}
	}

	private void SetEnabled(Condition condition, bool enabled)
	{
		if (condition != this.condition)
		{
			return;
		}
		bool flag = false;
		if (not)
		{
			enabled = !enabled;
		}
		if ((bool)m_renderer && m_renderer.enabled != enabled)
		{
			flag = true;
			m_renderer.enabled = enabled;
		}
		if ((bool)m_collider && m_collider.enabled != enabled)
		{
			flag = true;
			m_collider.enabled = enabled;
		}
		if (flag || !m_renderer)
		{
			int childCount = m_transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Renderer component = m_transform.GetChild(i).GetComponent<Renderer>();
				if ((bool)component && component.enabled != enabled)
				{
					component.enabled = enabled;
				}
			}
		}
		if (disableGameObject)
		{
			base.gameObject.SetActive(enabled);
		}
	}
}
