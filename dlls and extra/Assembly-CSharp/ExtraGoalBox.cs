using System.Collections.Generic;
using UnityEngine;

public class ExtraGoalBox : GoalBox
{
	private void OnTriggerEnter(Collider col)
	{
		BasePart basePart = FindPart(col);
		if ((bool)basePart && (!(base.tag == "Goal") || !(col.transform.tag == "Sharp")))
		{
			WPFMonoBehaviour.levelManager.ContraptionRunning.FinishConnectedComponentSearch();
			if (m_extraTargetPart != 0)
			{
				CheckIfPartReachedGoal(basePart, col, m_extraTargetPart);
			}
		}
	}

	protected new bool CheckIfPartReachedGoal(BasePart part, Collider collider, BasePart.PartType targetType)
	{
		BasePart basePart = WPFMonoBehaviour.levelManager.ContraptionRunning.FindPart(targetType);
		if (!basePart)
		{
			return false;
		}
		if (WPFMonoBehaviour.levelManager.ContraptionRunning.IsConnectedTo(part, collider, basePart))
		{
			OnGoalEnter(basePart);
			return true;
		}
		List<BasePart> parts = WPFMonoBehaviour.levelManager.ContraptionRunning.Parts;
		for (int i = 0; i < parts.Count; i++)
		{
			BasePart basePart2 = parts[i];
			if ((bool)basePart2 && basePart2.ConnectedComponent == part.ConnectedComponent)
			{
				OnGoalEnter(basePart);
				return true;
			}
		}
		return false;
	}

	protected override void OnGoalEnter(BasePart part)
	{
		if (!collected)
		{
			WPFMonoBehaviour.levelManager.NotifyGoalReachedByPart(part.m_partType);
			m_flagObject.GetComponent<Animation>().Play();
			if ((bool)m_goalAchievement)
			{
				m_goalAchievement.GetComponent<Animation>().Play();
			}
			m_goalParticles.Stop();
			collected = true;
			DisableGoal();
		}
	}
}
