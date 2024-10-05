using UnityEngine;

public class HUD : WPFMonoBehaviour
{
	private void Start()
	{
		if ((bool)WPFMonoBehaviour.gameData.m_blueprintPrefab)
		{
			Transform obj = Object.Instantiate(WPFMonoBehaviour.gameData.m_blueprintPrefab, WPFMonoBehaviour.levelManager.StartingPosition, Quaternion.identity);
			obj.name = "BlueprintUI";
			obj.parent = WPFMonoBehaviour.levelManager.transform;
		}
	}
}
