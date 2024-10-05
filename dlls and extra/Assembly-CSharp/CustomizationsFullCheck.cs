using System.Collections;
using Spine.Unity;
using UnityEngine;

public class CustomizationsFullCheck : WPFMonoBehaviour
{
	[SerializeField]
	private GameObject commonTierIndicator;

	[SerializeField]
	private GameObject rareTierIndicator;

	[SerializeField]
	private GameObject epicTierIndicator;

	[SerializeField]
	private SkeletonUtilityBone lamp1;

	[SerializeField]
	private SkeletonUtilityBone lamp2;

	[SerializeField]
	private SkeletonUtilityBone lamp3;

	private bool lamp1Disabled;

	private bool lamp2Disabled;

	private bool lamp3Disabled;

	public bool AllCommon => CustomizationManager.CustomizationCount(BasePart.PartTier.Common, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable) <= 0;

	public bool AllRare => CustomizationManager.CustomizationCount(BasePart.PartTier.Rare, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable) <= 0;

	public bool AllEpic => CustomizationManager.CustomizationCount(BasePart.PartTier.Epic, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable) <= 0;

	private void Awake()
	{
		Check();
	}

	public void Check()
	{
		commonTierIndicator.SetActive(AllCommon);
		rareTierIndicator.SetActive(AllRare);
		epicTierIndicator.SetActive(AllEpic);
		if (!lamp1Disabled && AllCommon)
		{
			StartCoroutine(CreateOverride(lamp1));
			lamp1Disabled = true;
		}
		if (!lamp2Disabled && AllRare)
		{
			StartCoroutine(CreateOverride(lamp2));
			lamp2Disabled = true;
		}
		if (!lamp3Disabled && AllEpic)
		{
			StartCoroutine(CreateOverride(lamp3));
			lamp3Disabled = true;
		}
	}

	private IEnumerator CreateOverride(SkeletonUtilityBone target)
	{
		yield return null;
		GameObject obj = target.skeletonUtility.SpawnBone(target.bone, target.transform.parent, SkeletonUtilityBone.Mode.Override, target.position, target.rotation, target.scale);
		obj.name += " [Override]";
		obj.transform.position = new Vector3(10000f, 0f, 0f);
	}
}
