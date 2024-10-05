using System.Collections.Generic;
using UnityEngine;

public abstract class Goal : WPFMonoBehaviour
{
	public BasePart.PartType m_extraTargetPart;

	protected virtual void Start()
	{
		EventManager.Connect<GameStateChanged>(OnGameStateChanged);
	}

	protected virtual void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
	}

	private void OnTriggerEnter(Collider col)
	{
		BasePart basePart = FindPart(col);
		if ((bool)basePart && (!(base.tag == "Goal") || !(col.transform.tag == "Sharp")))
		{
			WPFMonoBehaviour.levelManager.ContraptionRunning.FinishConnectedComponentSearch();
			CheckIfPartReachedGoal(basePart, col, BasePart.PartType.Pig);
			if (m_extraTargetPart != 0)
			{
				CheckIfPartReachedGoal(basePart, col, m_extraTargetPart);
			}
		}
	}

	protected bool CheckIfPartReachedGoal(BasePart part, Collider collider, BasePart.PartType targetType)
	{
		BasePart basePart = WPFMonoBehaviour.levelManager.ContraptionRunning.FindPart(targetType);
		if (!basePart)
		{
			return false;
		}
		if ((targetType != BasePart.PartType.Pig) ? WPFMonoBehaviour.levelManager.ContraptionRunning.IsConnectedTo(part, collider, basePart) : WPFMonoBehaviour.levelManager.ContraptionRunning.IsConnectedToPig(part, collider))
		{
			OnGoalEnter(basePart);
			return true;
		}
		List<BasePart> parts = WPFMonoBehaviour.levelManager.ContraptionRunning.Parts;
		for (int i = 0; i < parts.Count; i++)
		{
			BasePart basePart2 = parts[i];
			if ((bool)basePart2 && basePart2.ConnectedComponent == part.ConnectedComponent && Vector3.Distance(basePart2.Position, basePart.Position) < 2.5f)
			{
				OnGoalEnter(basePart);
				return true;
			}
		}
		return false;
	}

	protected BasePart FindPart(Collider collider)
	{
		Transform parent = collider.transform;
		while (parent != null)
		{
			BasePart component = parent.GetComponent<BasePart>();
			if ((bool)component)
			{
				return component;
			}
			parent = parent.parent;
		}
		return null;
	}

	private void OnGameStateChanged(GameStateChanged data)
	{
		if (data.state == LevelManager.GameState.Building)
		{
			OnReset();
		}
	}

	protected abstract void OnGoalEnter(BasePart part);

	protected abstract void OnReset();
}
