using System;
using UnityEngine;

public class DailyChallengeNextCrates : WPFMonoBehaviour
{
	[SerializeField]
	private GameObject[] cratePositions;

	[SerializeField]
	private GameObject woodCratePrefab;

	[SerializeField]
	private GameObject metalCratePrefab;

	[SerializeField]
	private GameObject goldCratePrefab;

	[SerializeField]
	private GameObject cardboardCratePrefab;

	[SerializeField]
	private GameObject glassCratePrefab;

	[SerializeField]
	private GameObject bronzeCratePrefab;

	[SerializeField]
	private GameObject marbleCratePrefab;

	[SerializeField]
	private GameObject woodCrateSilhouette;

	[SerializeField]
	private GameObject metalCrateSilhouette;

	[SerializeField]
	private GameObject goldCrateSilhouette;

	[SerializeField]
	private GameObject cardboardCrateSilhouette;

	[SerializeField]
	private GameObject glassCrateSilhouette;

	[SerializeField]
	private GameObject bronzeCrateSilhouette;

	[SerializeField]
	private GameObject marbleCrateSilhouette;

	private Transform lootCrates;

	private void Start()
	{
		if (!Singleton<DailyChallenge>.IsInstantiated())
		{
			return;
		}
		lootCrates = new GameObject("LootCrates").transform;
		lootCrates.transform.parent = base.transform;
		lootCrates.transform.localPosition = Vector3.zero;
		lootCrates.transform.localScale = Vector3.one;
		if (Singleton<DailyChallenge>.Instance.Initialized)
		{
			UpdateLootCrates();
			DailyChallenge instance = Singleton<DailyChallenge>.Instance;
			instance.OnDailyChallengeChanged = (Action)Delegate.Combine(instance.OnDailyChallengeChanged, new Action(UpdateLootCrates));
			return;
		}
		DailyChallenge instance2 = Singleton<DailyChallenge>.Instance;
		instance2.OnInitialize = (Action)Delegate.Combine(instance2.OnInitialize, (Action)delegate
		{
			UpdateLootCrates();
			DailyChallenge instance3 = Singleton<DailyChallenge>.Instance;
			instance3.OnDailyChallengeChanged = (Action)Delegate.Combine(instance3.OnDailyChallengeChanged, new Action(UpdateLootCrates));
		});
	}

	private void OnDestroy()
	{
		if (Singleton<DailyChallenge>.IsInstantiated())
		{
			DailyChallenge instance = Singleton<DailyChallenge>.Instance;
			instance.OnDailyChallengeChanged = (Action)Delegate.Remove(instance.OnDailyChallengeChanged, new Action(UpdateLootCrates));
		}
	}

	private void UpdateLootCrates()
	{
		for (int i = 0; i < lootCrates.childCount; i++)
		{
			UnityEngine.Object.Destroy(lootCrates.GetChild(i).gameObject);
		}
		for (int j = 0; j < cratePositions.Length; j++)
		{
			GameObject gameObject;
			GameObject gameObject2;
			switch (Singleton<DailyChallenge>.Instance.TomorrowsLootCrate(j))
			{
			default:
				return;
			case LootCrateType.Wood:
				gameObject = UnityEngine.Object.Instantiate(woodCrateSilhouette);
				gameObject2 = UnityEngine.Object.Instantiate(woodCratePrefab);
				break;
			case LootCrateType.Metal:
				gameObject = UnityEngine.Object.Instantiate(metalCrateSilhouette);
				gameObject2 = UnityEngine.Object.Instantiate(metalCratePrefab);
				break;
			case LootCrateType.Gold:
				gameObject = UnityEngine.Object.Instantiate(goldCrateSilhouette);
				gameObject2 = UnityEngine.Object.Instantiate(goldCratePrefab);
				break;
			case LootCrateType.Cardboard:
				gameObject = UnityEngine.Object.Instantiate(cardboardCrateSilhouette);
				gameObject2 = UnityEngine.Object.Instantiate(cardboardCratePrefab);
				break;
			case LootCrateType.Glass:
				gameObject = UnityEngine.Object.Instantiate(glassCrateSilhouette);
				gameObject2 = UnityEngine.Object.Instantiate(glassCratePrefab);
				break;
			case LootCrateType.Bronze:
				gameObject = UnityEngine.Object.Instantiate(bronzeCrateSilhouette);
				gameObject2 = UnityEngine.Object.Instantiate(bronzeCratePrefab);
				break;
			case LootCrateType.Marble:
				gameObject = UnityEngine.Object.Instantiate(marbleCrateSilhouette);
				gameObject2 = UnityEngine.Object.Instantiate(marbleCratePrefab);
				break;
			}
			gameObject.transform.parent = lootCrates;
			gameObject.transform.position = cratePositions[j].transform.position;
			gameObject.transform.localScale = cratePositions[j].transform.localScale;
			gameObject.transform.localRotation = cratePositions[j].transform.localRotation;
			gameObject.layer = base.gameObject.layer;
			gameObject.GetComponent<Renderer>().sortingLayerName = "Popup";
			gameObject2.transform.parent = lootCrates.transform;
			gameObject2.transform.localPosition = gameObject.transform.localPosition + new Vector3(0f, 0f, -0.1f);
			gameObject2.transform.localScale = cratePositions[j].transform.localScale;
			gameObject2.transform.localRotation = cratePositions[j].transform.localRotation;
			gameObject2.layer = base.gameObject.layer;
			gameObject2.GetComponent<Renderer>().sortingLayerName = "Popup";
		}
	}
}
