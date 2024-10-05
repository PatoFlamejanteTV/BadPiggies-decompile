using System;
using System.Collections.Generic;
using UnityEngine;

public class INRuntimeGameData : Singleton<INRuntimeGameData>
{
	[SerializeField]
	private GameObject m_partContainer;

	[SerializeField]
	private GameObject m_partIconContainer;

	private GameData m_gameData;

	public static bool IsInitialized
	{
		get
		{
			if (Singleton<INRuntimeGameData>.instance != null)
			{
				return Singleton<INRuntimeGameData>.instance.m_gameData != null;
			}
			return false;
		}
	}

	public GameData GameData => m_gameData;

	public GameObject PartContainer => m_partContainer;

	public GameObject PartIconContainer => m_partIconContainer;

	private void Awake()
	{
		SetAsPersistant();
		m_gameData = CreateGameData();
		InitializeSettings();
	}

	private void InitializeSettings()
	{
		InitializePart(INFeature.WoodenBox, SetWoodenBox);
		InitializePart(INFeature.MetalBox, SetMetalBox);
		InitializePart(INFeature.BracketFrame, SetBracketFrame);
		InitializePart(INFeature.ColoredFrame, SetColoredFrame);
		InitializePart(INFeature.OffRoadWheel, SetOffRoadWheel);
		InitializePart(INFeature.HingePlate, SetHingePlate);
		InitializePart(INFeature.MultipartGenerator, SetMultiPartGenerator);
		InitializePart(INFeature.AutoGun, SetAutoGun);
		InitializePart(INFeature.DecelerationLight, SetDecelerationLight);
	}

	private void InitializePart(INFeature feature, Action action)
	{
		INSettings.AddListener(feature, action);
		if (INSettings.GetBool(feature))
		{
			action();
		}
	}

	private void SetColoredFrame()
	{
		if (INSettings.GetBool(INFeature.ColoredFrame))
		{
			List<BasePart> partList = m_gameData.GetCustomPart(BasePart.PartType.MetalFrame).PartList;
			BasePart component = INUnity.LoadGameObject("Part_ColoredFrame").GetComponent<BasePart>();
			float num = INSettings.GetFloat(INFeature.ColoredFrameAlpha);
			float num2 = INSettings.GetFloat(INFeature.ColoredFrameForegroundAlpha);
			float num3 = INSettings.GetFloat(INFeature.ColoredFrameBackgroundAlpha);
			for (int i = 0; i < 120; i++)
			{
				int num4 = i - 118;
				int num5 = i / 36;
				int num6 = i % 18;
				string text = (i + 13).ToString();
				BasePart basePart = CreatePart(component);
				basePart.customPartIndex = i + 12;
				basePart.m_partTier = num5 switch
				{
					2 => BasePart.PartTier.Epic, 
					1 => BasePart.PartTier.Rare, 
					0 => BasePart.PartTier.Common, 
					_ => BasePart.PartTier.Legendary, 
				};
				if (num4 >= 0)
				{
					text = (num4 + 133).ToString();
					basePart.customPartIndex = num4 + 132;
					basePart.m_partTier = BasePart.PartTier.Regular;
				}
				basePart.gameObject.name = "Part_MetalFrame_" + text + "_SET";
				Color color;
				if (num5 < 3)
				{
					float s = ((i / 18 % 2 == 0) ? 0.7f : 0.4f);
					color = Color.HSVToRGB((float)num6 / 18f, s, num5 switch
					{
						1 => 0.6f, 
						0 => 0.9f, 
						_ => 0.3f, 
					});
				}
				else
				{
					color = Color.Lerp(Color.white, Color.black, (float)num6 * 0.1f);
				}
				if (num4 >= 0)
				{
					color = Color.white;
					num *= ((num4 == 0) ? 0.5f : 0f);
					num2 *= 0.5f;
					num3 *= 0.5f;
				}
				(basePart as ColoredFrame).Color = color;
				basePart.GetComponent<MeshRenderer>().material.shader = INUnity.UnlitSolidColorShader;
				Sprite constructionIconSprite = basePart.m_constructionIconSprite;
				constructionIconSprite.name = "Icon_MetalFrame_" + text;
				MeshRenderer component2 = constructionIconSprite.GetComponent<MeshRenderer>();
				MeshRenderer component3 = constructionIconSprite.transform.Find("Background").GetComponent<MeshRenderer>();
				component2.material.name = "IngameAtlas3_MetalFrame_" + text;
				component2.material.shader = INUnity.UnlitSolidColorShader;
				component2.material.color = new Color(color.r, color.g, color.b, num);
				component3.material.name = "IngameAtlas3_MetalFrame_Background_" + text;
				component3.material.color = new Color(color.r, color.g, color.b, num3);
				basePart.m_constructionIconSprite = constructionIconSprite;
				partList.Add(basePart);
			}
		}
		else
		{
			for (int j = 12; j <= 129; j++)
			{
				RemoveCustomPart(BasePart.PartType.MetalFrame, j);
			}
			RemoveCustomPart(BasePart.PartType.MetalFrame, 132);
			RemoveCustomPart(BasePart.PartType.MetalFrame, 133);
		}
	}

	private void SetWoodenBox()
	{
		if (INSettings.GetBool(INFeature.WoodenBox))
		{
			AddCustomPart(CreatePart("Part_WoodenFrame_11_SET"));
		}
		else
		{
			RemoveCustomPart(BasePart.PartType.WoodenFrame, 10);
		}
	}

	private void SetMetalBox()
	{
		if (INSettings.GetBool(INFeature.MetalBox))
		{
			AddCustomPart(CreatePart("Part_MetalFrame_131_SET"));
		}
		else
		{
			RemoveCustomPart(BasePart.PartType.MetalFrame, 130);
		}
	}

	private void SetBracketFrame()
	{
		if (INSettings.GetBool(INFeature.BracketFrame))
		{
			AddCustomPart(CreatePart("Part_MetalFrame_132_SET"));
		}
		else
		{
			RemoveCustomPart(BasePart.PartType.MetalFrame, 131);
		}
	}

	private void SetOffRoadWheel()
	{
		if (INSettings.GetBool(INFeature.OffRoadWheel))
		{
			AddCustomPart(CreatePart("Part_MotorWheel_08_SET"));
		}
		else
		{
			RemoveCustomPart(BasePart.PartType.MotorWheel, 7);
		}
	}

	private void SetHingePlate()
	{
		if (INSettings.GetBool(INFeature.HingePlate))
		{
			AddCustomPart(CreatePart("Part_Rope_05_SET"));
			AddCustomPart(CreatePart("Part_Rope_06_SET"));
			AddCustomPart(CreatePart("Part_Rope_07_SET"));
			AddCustomPart(CreatePart("Part_Rope_08_SET"));
		}
		else
		{
			RemoveCustomPart(BasePart.PartType.Rope, 4);
			RemoveCustomPart(BasePart.PartType.Rope, 5);
			RemoveCustomPart(BasePart.PartType.Rope, 6);
			RemoveCustomPart(BasePart.PartType.Rope, 7);
		}
	}

	private void SetAutoGun()
	{
		if (INSettings.GetBool(INFeature.AutoGun))
		{
			AddCustomPart(CreatePart("Part_GrapplingHook_07_SET"));
		}
		else
		{
			RemoveCustomPart(BasePart.PartType.GrapplingHook, 6);
		}
	}

	private void SetMultiPartGenerator()
	{
		if (INSettings.GetBool(INFeature.MultipartGenerator))
		{
			for (int i = 0; i < 3; i++)
			{
				BasePart basePart = CreatePart("Part_GrapplingHook_09_SET");
				if (i != 0)
				{
					basePart.gameObject.name = $"Part_GrapplingHook_{i + 9}_SET";
					basePart.customPartIndex = i + 8;
					basePart.m_constructionIconSprite.gameObject.name = $"Icon_GrapplingHook_{i + 9}";
					INSerializedSprite component = basePart.GetComponent<INSerializedSprite>();
					component.m_name = $"MultiPartGenerator{i + 1}_Sprite";
					component.CreateMesh();
					INSerializedSprite componentInChildren = basePart.m_constructionIconSprite.GetComponentInChildren<INSerializedSprite>();
					componentInChildren.m_name = $"MultiPartGenerator{i + 1}_IconSprite";
					componentInChildren.CreateMesh();
				}
				basePart.m_partTier = BasePart.PartTier.Common;
				AddCustomPart(basePart);
			}
		}
		else
		{
			for (int j = 0; j < 3; j++)
			{
				RemoveCustomPart(BasePart.PartType.GrapplingHook, j + 8);
			}
		}
	}

	private void SetDecelerationLight()
	{
		if (INSettings.GetBool(INFeature.DecelerationLight))
		{
			AddCustomPart(CreatePart("Part_PointLight_06_SET"));
		}
		else
		{
			RemoveCustomPart(BasePart.PartType.PointLight, 5);
		}
	}

	public GameData CreateGameData()
	{
		GameData gameData = UnityEngine.Object.Instantiate(Singleton<GameManager>.Instance.gameData);
		for (int i = 0; i < gameData.m_parts.Count; i++)
		{
			gameData.m_parts[i] = CreatePart(gameData.m_parts[i]).gameObject;
		}
		foreach (CustomPartInfo customPart in gameData.m_customParts)
		{
			List<BasePart> partList = customPart.PartList;
			for (int j = 0; j < partList.Count; j++)
			{
				partList[j] = CreatePart(partList[j]);
			}
		}
		return gameData;
	}

	public BasePart CreatePart(BasePart original)
	{
		BasePart basePart = UnityEngine.Object.Instantiate(original);
		basePart.name = original.name;
		basePart.transform.parent = m_partContainer.transform;
		basePart.gameObject.SetActive(value: false);
		Sprite constructionIconSprite = original.m_constructionIconSprite;
		if (constructionIconSprite != null)
		{
			Sprite sprite = UnityEngine.Object.Instantiate(constructionIconSprite);
			sprite.name = constructionIconSprite.name;
			sprite.transform.parent = m_partIconContainer.transform;
			Vector3 position = sprite.transform.position;
			position.z = -130f;
			sprite.transform.position = position;
			basePart.m_constructionIconSprite = sprite;
		}
		return basePart;
	}

	public BasePart CreatePart(GameObject original)
	{
		return CreatePart(original.GetComponent<BasePart>());
	}

	public BasePart CreatePart(string path)
	{
		return CreatePart(INUnity.LoadGameObject(path));
	}

	public void AddCustomPart(BasePart newPart)
	{
		m_gameData.GetCustomPart(newPart.m_partType).PartList.Add(newPart);
	}

	public void ReplaceCustomPart(BasePart newPart)
	{
		List<BasePart> partList = m_gameData.GetCustomPart(newPart.m_partType).PartList;
		for (int i = 0; i < partList.Count; i++)
		{
			BasePart basePart = partList[i];
			if (basePart.customPartIndex == newPart.customPartIndex)
			{
				partList[i] = newPart;
				if (basePart.m_constructionIconSprite != null)
				{
					UnityEngine.Object.Destroy(basePart.m_constructionIconSprite.gameObject);
				}
				UnityEngine.Object.Destroy(basePart.gameObject);
				break;
			}
		}
	}

	public void RemoveCustomPart(BasePart.PartType type, int customIndex)
	{
		List<BasePart> partList = m_gameData.GetCustomPart(type).PartList;
		for (int i = 0; i < partList.Count; i++)
		{
			BasePart basePart = partList[i];
			if (basePart.customPartIndex == customIndex)
			{
				partList.RemoveAt(i);
				if (basePart.m_constructionIconSprite != null)
				{
					UnityEngine.Object.Destroy(basePart.m_constructionIconSprite.gameObject);
				}
				UnityEngine.Object.Destroy(basePart.gameObject);
				break;
			}
		}
	}

	public BasePart GetCustomPart(BasePart.PartType type, int customIndex)
	{
		if (customIndex <= 0)
		{
			GameObject part = m_gameData.GetPart(type);
			if (!(part != null))
			{
				return null;
			}
			return part.GetComponent<BasePart>();
		}
		foreach (BasePart part2 in m_gameData.GetCustomPart(type).PartList)
		{
			if (part2.customPartIndex == customIndex)
			{
				return part2;
			}
		}
		return null;
	}

	public int GetCustomPartIndex(BasePart.PartType type, string partName)
	{
		GameObject part = m_gameData.GetPart(type);
		if (part != null && part.name.Equals(partName))
		{
			return 0;
		}
		foreach (BasePart part2 in m_gameData.GetCustomPart(type).PartList)
		{
			if (part2.name.Equals(partName))
			{
				return part2.customPartIndex;
			}
		}
		return -1;
	}
}
