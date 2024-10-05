using System;
using UnityEngine;

public class MenuLootCrateSlotIndicator : MonoBehaviour
{
	[SerializeField]
	private GameObject shine;

	[SerializeField]
	private int slotCount;

	private void Awake()
	{
		shine.SetActive(value: false);
	}

	private void Start()
	{
		if (Singleton<TimeManager>.IsInstantiated() && Singleton<TimeManager>.Instance.Initialized)
		{
			Check();
		}
		else
		{
			Singleton<TimeManager>.Instance.OnInitialize += Check;
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	private void Check()
	{
		if (AnyLootCrateCollectible(out var index))
		{
			SpawnCrateIcon(GetLootCrateType(index));
		}
		else if (FindNextUnlock(out index))
		{
			SetTimer(index);
		}
	}

	private bool AnyLootCrateCollectible(out int index)
	{
		index = -1;
		for (int i = 0; i < slotCount; i++)
		{
			string slotIdentifier = LootCrateSlot.GetSlotIdentifier(i);
			if (GameProgress.HasKey(slotIdentifier))
			{
				string[] array = GameProgress.GetString(slotIdentifier, string.Empty).Split(',');
				if (array.Length != 0 && array[0] == "Unlocked")
				{
					index = i;
					return true;
				}
				if (Singleton<TimeManager>.Instance.HasTimer(slotIdentifier) && Singleton<TimeManager>.Instance.TimeLeft(slotIdentifier) <= 0f)
				{
					index = i;
					return true;
				}
			}
		}
		return false;
	}

	private LootCrateType GetLootCrateType(int index)
	{
		string[] array = GameProgress.GetString(LootCrateSlot.GetSlotIdentifier(index), string.Empty).Split(',');
		return (LootCrateType)Enum.Parse(typeof(LootCrateType), array[1], ignoreCase: true);
	}

	private bool FindNextUnlock(out int index)
	{
		index = -1;
		float num = float.MaxValue;
		for (int i = 0; i < slotCount; i++)
		{
			string slotIdentifier = LootCrateSlot.GetSlotIdentifier(i);
			if (Singleton<TimeManager>.Instance.HasTimer(slotIdentifier) && Singleton<TimeManager>.Instance.TimeLeft(slotIdentifier) < num)
			{
				index = i;
				num = Singleton<TimeManager>.Instance.TimeLeft(slotIdentifier);
			}
		}
		return index >= 0;
	}

	private void SetTimer(int index)
	{
		string slotIdentifier = LootCrateSlot.GetSlotIdentifier(index);
		if (Singleton<TimeManager>.Instance.HasTimer(slotIdentifier))
		{
			float seconds = Singleton<TimeManager>.Instance.TimeLeft(slotIdentifier);
			StartCoroutine(CoroutineRunner.DelayActionSequence(Check, seconds, realTime: true));
		}
	}

	private void SpawnCrateIcon(LootCrateType type)
	{
		LayerHelper.SetLayer(LootCrateGraphicSpawner.CreateCrateSilhouette(type, base.transform, Vector3.zero, Vector3.one, Quaternion.identity), base.gameObject.layer, children: true);
		LayerHelper.SetLayer(LootCrateGraphicSpawner.CreateCrateSmall(type, base.transform, Vector3.back * 0.1f, Vector3.one, Quaternion.identity), base.gameObject.layer, children: true);
		shine.SetActive(value: true);
	}
}
