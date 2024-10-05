using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelRewardData : MonoBehaviour
{
	[Serializable]
	public class SandboxUnlock
	{
		public string levelIdentifier;

		public string sandboxIdentifier;
	}

	public List<SandboxUnlock> sandboxUnlocks;
}
