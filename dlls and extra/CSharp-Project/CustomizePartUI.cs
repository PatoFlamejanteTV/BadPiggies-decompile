using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomizePartUI : WPFMonoBehaviour
{
	public struct PartCustomizationEvent : EventManager.Event
	{
		public BasePart.PartType customizedPart;

		public int customPartIndex;

		public PartCustomizationEvent(BasePart.PartType customizedPart, int customPartIndex)
		{
			this.customizedPart = customizedPart;
			this.customPartIndex = customPartIndex;
		}
	}

	private const float MOVEMENT_LIMIT = 1.5f;

	public CustomizePartWidget customPartWidget;

	private bool initButtonsDone;

	private Action<BasePart.PartType> onButtonPressed;

	private PartListing cachedPartListing;

	public void InitButtons(PartListing partListing, Action<BasePart.PartType> onButtonPressed)
	{
		cachedPartListing = partListing;
		this.onButtonPressed = onButtonPressed;
		if (initButtonsDone)
		{
			return;
		}
		if (customPartWidget != null)
		{
			Transform transform = cachedPartListing.transform.Find("Close");
			if ((bool)transform)
			{
				customPartWidget.closeButton = transform.GetComponent<Button>();
			}
		}
		for (int i = 0; i < Enum.GetNames(typeof(BasePart.PartType)).Length; i++)
		{
			for (int j = 0; j < Enum.GetNames(typeof(BasePart.PartTier)).Length; j++)
			{
				BasePart.PartType partType = (BasePart.PartType)i;
				List<GameObject> partTierInstances = cachedPartListing.GetPartTierInstances(partType, (BasePart.PartTier)j);
				if (partTierInstances == null)
				{
					continue;
				}
				for (int k = 0; k < partTierInstances.Count; k++)
				{
					bool flag = j == 0;
					int customPartIndex = WPFMonoBehaviour.gameData.GetCustomPartIndex(partType, partTierInstances[k].name);
					if (j > 0)
					{
						flag = CustomizationManager.IsPartUnlocked(WPFMonoBehaviour.gameData.GetCustomPart(partType, customPartIndex));
					}
					BoxCollider component = partTierInstances[k].GetComponent<BoxCollider>();
					if (component != null)
					{
						component.enabled = flag;
						partTierInstances[k].GetComponent<Button>().MethodToCall.SetMethod(this, "CustomButtonPressed", new object[2] { i, customPartIndex });
					}
				}
			}
		}
		cachedPartListing.CreateSelectionIcons();
		initButtonsDone = true;
	}

	public void CustomButtonPressed(int partTypeIndex, int partIndex)
	{
		if (cachedPartListing.LastMovement > 1.5f)
		{
			return;
		}
		BasePart customPart = WPFMonoBehaviour.gameData.GetCustomPart((BasePart.PartType)partTypeIndex, partIndex);
		cachedPartListing.UpdateSelectionIcon((BasePart.PartType)partTypeIndex, customPart.name);
		cachedPartListing.PlaySelectionAudio((BasePart.PartType)partTypeIndex, customPart.name);
		if (!CustomizationManager.IsPartUsed(customPart))
		{
			cachedPartListing.ShowExperienceParticles((BasePart.PartType)partTypeIndex, customPart.name);
			CustomizationManager.SetPartUsed(customPart, used: true);
		}
		if (CustomizationManager.IsPartNew(customPart))
		{
			CustomizationManager.SetPartNew(customPart, isNew: false);
		}
		CustomizationManager.SetLastUsedPartIndex((BasePart.PartType)partTypeIndex, partIndex);
		EventManager.Send(new PartCustomizationEvent((BasePart.PartType)partTypeIndex, partIndex));
		if (onButtonPressed != null)
		{
			onButtonPressed((BasePart.PartType)partTypeIndex);
			if (customPartWidget != null)
			{
				customPartWidget.ClosePastList();
			}
		}
	}
}
