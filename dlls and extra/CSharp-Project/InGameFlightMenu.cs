using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameFlightMenu : WPFMonoBehaviour
{
	public class PartButtonOrder : IComparer<GadgetButton>
	{
		private float middle;

		public PartButtonOrder(float middle)
		{
			this.middle = middle;
		}

		public int Compare(GadgetButton obj1, GadgetButton obj2)
		{
			float placementOrder = middle;
			float placementOrder2 = middle;
			if ((bool)obj1)
			{
				placementOrder = obj1.PlacementOrder;
			}
			if ((bool)obj1)
			{
				placementOrder2 = obj2.PlacementOrder;
			}
			if (placementOrder < placementOrder2)
			{
				return -1;
			}
			if (placementOrder > placementOrder2)
			{
				return 1;
			}
			return 0;
		}
	}

	public Button superContraptionIndexButton;

	[SerializeField]
	private GameObject cheatButton1Star;

	[SerializeField]
	private GameObject cheatButton3Stars;

	[SerializeField]
	private GameObject editorButtons;

	[SerializeField]
	private GameObject snapshotButton;

	[SerializeField]
	private GameObject[] leftButtons;

	[SerializeField]
	private GadgetButtonList buttonList;

	public GadgetButtonList ButtonList => buttonList;

	private void Awake()
	{
		GameObject original = buttonList.Buttons[0].gameObject;
		if (INSettings.GetBool(INFeature.SpecialEggs))
		{
			AddPartButton(original, BasePart.PartType.Egg, BasePart.Direction.Right, 0);
		}
		if (INSettings.GetBool(INFeature.SwitchableWing))
		{
			AddPartButton(original, BasePart.PartType.Wings, BasePart.Direction.Right, 0);
			AddPartButton(original, BasePart.PartType.MetalWing, BasePart.Direction.Right, 0);
		}
		if (INSettings.GetBool(INFeature.SwitchableTail))
		{
			AddPartButton(original, BasePart.PartType.Tailplane, BasePart.Direction.Right, 0);
			AddPartButton(original, BasePart.PartType.MetalTail, BasePart.Direction.Right, 0);
		}
		if (INSettings.GetBool(INFeature.RotatablePumpkin))
		{
			for (int i = 0; i < 4; i++)
			{
				AddPartButton(original, BasePart.PartType.Pumpkin, (BasePart.Direction)i, 0).transform.Find("Gadget").localRotation = Quaternion.AngleAxis(90 * i, Vector3.forward);
			}
		}
		if (INSettings.GetBool(INFeature.RotatableTNT))
		{
			BasePart.PartType partType = BasePart.PartType.TNT;
			Button button = FindButton(partType);
			for (int j = 1; j < 4; j++)
			{
				AddButton(button.gameObject, partType, (BasePart.Direction)j).transform.Find("Gadget").localRotation = Quaternion.AngleAxis(90 * j, Vector3.forward);
			}
		}
		if (INSettings.GetBool(INFeature.RotatableGearbox))
		{
			BasePart.PartType partType2 = BasePart.PartType.Gearbox;
			Button button2 = FindButton(partType2);
			for (int k = 1; k < 4; k++)
			{
				AddButton(button2.gameObject, partType2, (BasePart.Direction)k).transform.Find("Gadget").localRotation = Quaternion.AngleAxis(90 * k, Vector3.forward);
			}
		}
	}

	private GadgetButton FindButton(BasePart.PartType partType)
	{
		foreach (GadgetButton button in buttonList.Buttons)
		{
			if (button.m_partType == partType)
			{
				return button;
			}
		}
		return null;
	}

	private GadgetButton AddButton(GameObject original, BasePart.PartType partType, BasePart.Direction direction)
	{
		GameObject gameObject = Object.Instantiate(original);
		gameObject.transform.parent = original.transform.parent;
		gameObject.transform.localScale = original.transform.localScale;
		GadgetButton component = gameObject.GetComponent<GadgetButton>();
		component.m_partType = partType;
		component.m_direction = direction;
		buttonList.Buttons.Add(gameObject.GetComponent<GadgetButton>());
		return gameObject.GetComponent<GadgetButton>();
	}

	private GadgetButton AddPartButton(GameObject original, BasePart.PartType partType, BasePart.Direction direction, int customIndex)
	{
		GameObject gameObject = Object.Instantiate(original);
		gameObject.transform.parent = original.transform.parent;
		gameObject.transform.localScale = original.transform.localScale;
		GadgetButton component = gameObject.GetComponent<GadgetButton>();
		component.m_partType = partType;
		component.m_direction = direction;
		Transform transform = gameObject.transform.Find("Gadget");
		transform.GetComponent<MeshFilter>().sharedMesh = null;
		Transform obj = Object.Instantiate(WPFMonoBehaviour.gameData.GetCustomPart(partType, customIndex).GetComponent<BasePart>().m_constructionIconSprite.transform);
		obj.parent = transform;
		obj.localPosition = Vector3.zero;
		obj.localScale = new Vector3(1.8f, 1.8f, 1.8f);
		MeshRenderer[] componentsInChildren = obj.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].material.shader = INUnity.TextShader;
		}
		buttonList.Buttons.Add(gameObject.GetComponent<GadgetButton>());
		return gameObject.GetComponent<GadgetButton>();
	}

	private void OnEnable()
	{
		if ((bool)WPFMonoBehaviour.levelManager && (bool)WPFMonoBehaviour.levelManager.ContraptionRunning)
		{
			SetGadgetButtonOrder(WPFMonoBehaviour.levelManager.ContraptionRunning.PartPlacements);
		}
		bool flag = WPFMonoBehaviour.levelManager.CurrentGameMode is CakeRaceMode;
		if (!Singleton<BuildCustomizationLoader>.Instance.CheatsEnabled || WPFMonoBehaviour.levelManager.m_sandbox || flag)
		{
			cheatButton1Star.SetActive(value: false);
			cheatButton3Stars.SetActive(value: false);
		}
		editorButtons.SetActive(value: false);
		leftButtons[0].SetActive(!flag);
		leftButtons[1].SetActive(flag);
		leftButtons[2].SetActive(!flag);
		StartCoroutine(WaitEndOfAwake());
		snapshotButton.SetActive(value: false);
		if (INSettings.GetBool(INFeature.HideRunningButtons))
		{
			for (int i = 0; i < 3; i++)
			{
				leftButtons[i].gameObject.GetComponent<Renderer>().enabled = false;
			}
		}
	}

	private IEnumerator WaitEndOfAwake()
	{
		yield return new WaitForEndOfFrame();
		GridLayout component = leftButtons[0].transform.parent.GetComponent<GridLayout>();
		if (component != null)
		{
			component.UpdateLayout();
		}
	}

	private void SetGadgetButtonOrder(List<Contraption.PartPlacementInfo> parts)
	{
		int num = 0;
		List<GadgetButton> buttons = buttonList.Buttons;
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].UpdateState();
			if (buttons[i].VisibilityCondition != null)
			{
				buttons[i].VisibilityCondition.UpdateState();
			}
			bool flag = false;
			for (int j = 0; j < parts.Count; j++)
			{
				if (CombinedTypeForGadgetButtonOrdering(parts[j].partType) == buttons[i].m_partType && parts[j].direction == buttons[i].m_direction)
				{
					buttons[i].PlacementOrder = j;
					if (parts[j].count > 0)
					{
						flag = true;
						num++;
					}
					break;
				}
			}
			buttons[i].Enabled = flag;
		}
		buttonList.Sort(new PartButtonOrder((float)num / 2f + ((num % 2 != 0) ? 0.5f : 0f)));
	}

	public static BasePart.PartType CombinedTypeForGadgetButtonOrdering(BasePart.PartType originalType)
	{
		BasePart.PartType partType = BasePart.BaseType(originalType);
		switch (partType)
		{
		case BasePart.PartType.EngineBig:
			partType = BasePart.PartType.Engine;
			break;
		case BasePart.PartType.EngineSmall:
			partType = BasePart.PartType.Engine;
			break;
		}
		return partType;
	}

	public void CompleteLevelWithThreeStars()
	{
	}

	public void CompleteLevelWithOneStar()
	{
	}
}
