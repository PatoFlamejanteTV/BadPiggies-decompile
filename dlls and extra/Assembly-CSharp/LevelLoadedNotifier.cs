using System;
using UnityEngine;

public class LevelLoadedNotifier : MonoBehaviour
{
	public static event Action OnLevelLoaded;

	private void Start()
	{
		LevelLoadedNotifier.OnLevelLoaded();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	static LevelLoadedNotifier()
	{
		LevelLoadedNotifier.OnLevelLoaded = delegate
		{
		};
	}
}
