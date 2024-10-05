using UnityEngine;

public class GoldenPigCollectable : Collectable
{
	private void OnTriggerEnter(Collider col)
	{
		BasePart basePart = FindPart(col);
		if ((bool)basePart && (!(base.tag == "Goal") || !(col.transform.tag == "Sharp")))
		{
			WPFMonoBehaviour.levelManager.ContraptionRunning.FinishConnectedComponentSearch();
			CheckIfPartReachedGoal(basePart, col, BasePart.PartType.GoldenPig);
		}
	}
}
