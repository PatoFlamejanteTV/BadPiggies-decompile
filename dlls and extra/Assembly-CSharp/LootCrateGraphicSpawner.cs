using UnityEngine;

public class LootCrateGraphicSpawner : WPFMonoBehaviour
{
	private static LootCrateGraphicSpawner s_instance;

	[SerializeField]
	private GameObject woodCrate;

	[SerializeField]
	private GameObject metalCrate;

	[SerializeField]
	private GameObject goldCrate;

	[SerializeField]
	private GameObject cardboardCrate;

	[SerializeField]
	private GameObject glassCrate;

	[SerializeField]
	private GameObject bronzeCrate;

	[SerializeField]
	private GameObject marbleCrate;

	[SerializeField]
	private GameObject woodCrateSmall;

	[SerializeField]
	private GameObject metalCrateSmall;

	[SerializeField]
	private GameObject goldCrateSmall;

	[SerializeField]
	private GameObject cardboardCrateSmall;

	[SerializeField]
	private GameObject glassCrateSmall;

	[SerializeField]
	private GameObject bronzeCrateSmall;

	[SerializeField]
	private GameObject marbleCrateSmall;

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

	private static LootCrateGraphicSpawner Instance
	{
		get
		{
			if (s_instance == null)
			{
				s_instance = Object.Instantiate(WPFMonoBehaviour.gameData.m_lootCrateGraphicsSpawner).GetComponent<LootCrateGraphicSpawner>();
			}
			return s_instance;
		}
	}

	public static GameObject CreateCrate(LootCrateType type, Transform parent, Vector3 localPosition, Vector3 localScale, Quaternion localRotation)
	{
		if (Instance == null)
		{
			return null;
		}
		GameObject prefab;
		switch (type)
		{
		case LootCrateType.Wood:
			prefab = Instance.woodCrate;
			break;
		case LootCrateType.Metal:
			prefab = Instance.metalCrate;
			break;
		case LootCrateType.Gold:
			prefab = Instance.goldCrate;
			break;
		case LootCrateType.Cardboard:
			prefab = Instance.cardboardCrate;
			break;
		case LootCrateType.Glass:
			prefab = Instance.glassCrate;
			break;
		case LootCrateType.Bronze:
			prefab = Instance.bronzeCrate;
			break;
		case LootCrateType.Marble:
			prefab = Instance.marbleCrate;
			break;
		default:
			return null;
		}
		return CreateObject(prefab, parent, localPosition, localScale, localRotation);
	}

	public static GameObject CreateCrateSmall(LootCrateType type, Transform parent, Vector3 localPosition, Vector3 localScale, Quaternion localRotation)
	{
		if (Instance == null)
		{
			return null;
		}
		GameObject prefab;
		switch (type)
		{
		case LootCrateType.Wood:
			prefab = Instance.woodCrateSmall;
			break;
		case LootCrateType.Metal:
			prefab = Instance.metalCrateSmall;
			break;
		case LootCrateType.Gold:
			prefab = Instance.goldCrateSmall;
			break;
		case LootCrateType.Cardboard:
			prefab = Instance.cardboardCrateSmall;
			break;
		case LootCrateType.Glass:
			prefab = Instance.glassCrateSmall;
			break;
		case LootCrateType.Bronze:
			prefab = Instance.bronzeCrateSmall;
			break;
		case LootCrateType.Marble:
			prefab = Instance.marbleCrateSmall;
			break;
		default:
			return null;
		}
		return CreateObject(prefab, parent, localPosition, localScale, localRotation);
	}

	public static GameObject CreateCrateSilhouette(LootCrateType type, Transform parent, Vector3 localPosition, Vector3 localScale, Quaternion localRotation)
	{
		if (Instance == null)
		{
			return null;
		}
		GameObject prefab;
		switch (type)
		{
		case LootCrateType.Wood:
			prefab = Instance.woodCrateSilhouette;
			break;
		case LootCrateType.Metal:
			prefab = Instance.metalCrateSilhouette;
			break;
		case LootCrateType.Gold:
			prefab = Instance.goldCrateSilhouette;
			break;
		case LootCrateType.Cardboard:
			prefab = Instance.cardboardCrateSilhouette;
			break;
		case LootCrateType.Glass:
			prefab = Instance.glassCrateSilhouette;
			break;
		case LootCrateType.Bronze:
			prefab = Instance.bronzeCrateSilhouette;
			break;
		case LootCrateType.Marble:
			prefab = Instance.marbleCrateSilhouette;
			break;
		default:
			return null;
		}
		return CreateObject(prefab, parent, localPosition, localScale, localRotation);
	}

	private static GameObject CreateObject(GameObject prefab, Transform parent, Vector3 localPosition, Vector3 localScale, Quaternion localRotation)
	{
		GameObject obj = Object.Instantiate(prefab);
		obj.transform.parent = parent;
		obj.transform.localPosition = localPosition;
		obj.transform.localRotation = localRotation;
		obj.transform.localScale = localScale;
		return obj;
	}
}
