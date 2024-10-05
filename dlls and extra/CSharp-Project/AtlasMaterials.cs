using System;
using System.Collections.Generic;
using UnityEngine;

public class AtlasMaterials : MonoBehaviour
{
	public enum MaterialType
	{
		Normal,
		Dimmed,
		PartRender,
		PartZ,
		Gray,
		Shiny,
		Alpha
	}

	private static AtlasMaterials _instance;

	[SerializeField]
	private Texture2D emptyTexture;

	[SerializeField]
	private GameObject loadingScreen;

	[SerializeField]
	private List<Material> normalMaterials;

	[SerializeField]
	private List<Material> dimmedRenderQueueMaterials;

	[SerializeField]
	private List<Material> renderQueueMaterials;

	[SerializeField]
	private List<Material> partQueueZMaterials;

	[SerializeField]
	private List<Material> grayMaterials;

	[SerializeField]
	private List<Material> shineMaterials;

	[SerializeField]
	private List<Material> alphaMaterials;

	private Dictionary<string, Material> cachedMaterialInstances;

	private Dictionary<int, List<Material>> materialInstances;

	private Dictionary<int, string> atlasMaterialReferencePaths = new Dictionary<int, string>
	{
		{ 0, "GUISystem/RefIngameAtlas" },
		{ 1, "GUISystem/RefIngameAtlas2" },
		{ 2, "GUISystem/RefIngameAtlas3" },
		{ 3, "GUISystem/RefMenuAtlas" },
		{ 4, "GUISystem/RefMenuAtlas2" }
	};

	public static AtlasMaterials Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = Resources.Load<GameObject>("Utility/AtlasMaterials");
				if (gameObject != null)
				{
					GameObject obj = UnityEngine.Object.Instantiate(gameObject);
					UnityEngine.Object.DontDestroyOnLoad(obj);
					_instance = obj.GetComponent<AtlasMaterials>();
				}
			}
			return _instance;
		}
	}

	public static bool IsInstantiated => _instance != null;

	public List<Material> NormalMaterials => normalMaterials;

	public List<Material> DimmedRenderQueueMaterials => dimmedRenderQueueMaterials;

	public List<Material> RenderQueueMaterials => renderQueueMaterials;

	public List<Material> PartQueueZMaterials => partQueueZMaterials;

	public List<Material> GrayMaterials => grayMaterials;

	public List<Material> ShineMaterials => shineMaterials;

	public List<Material> AlphaMaterials => alphaMaterials;

	public Material GetMaterial(Material source, MaterialType materialType)
	{
		if (source == null)
		{
			return null;
		}
		int num = -1;
		for (int num2 = normalMaterials.Count - 1; num2 >= 0; num2--)
		{
			if (source.name.StartsWith(normalMaterials[num2].name))
			{
				num = num2;
				break;
			}
		}
		if (num < 0)
		{
			return null;
		}
		switch (materialType)
		{
		case MaterialType.Normal:
			if (num < normalMaterials.Count)
			{
				return normalMaterials[num];
			}
			break;
		case MaterialType.Dimmed:
			if (num < dimmedRenderQueueMaterials.Count)
			{
				return dimmedRenderQueueMaterials[num];
			}
			break;
		case MaterialType.PartRender:
			if (num < renderQueueMaterials.Count)
			{
				return renderQueueMaterials[num];
			}
			break;
		case MaterialType.PartZ:
			if (num < partQueueZMaterials.Count)
			{
				return partQueueZMaterials[num];
			}
			break;
		case MaterialType.Gray:
			if (num < grayMaterials.Count)
			{
				return grayMaterials[num];
			}
			break;
		case MaterialType.Shiny:
			if (num < shineMaterials.Count)
			{
				return shineMaterials[num];
			}
			break;
		case MaterialType.Alpha:
			if (num < alphaMaterials.Count)
			{
				return alphaMaterials[num];
			}
			break;
		}
		return null;
	}

	public Material GetCachedMaterialInstance(Material source, MaterialType materialType)
	{
		if (source == null)
		{
			return null;
		}
		if (cachedMaterialInstances == null)
		{
			cachedMaterialInstances = new Dictionary<string, Material>();
		}
		string materialKey = GetMaterialKey(source, materialType);
		if (cachedMaterialInstances.TryGetValue(materialKey, out var value))
		{
			return value;
		}
		Material material = GetMaterial(source, materialType);
		if (material == null)
		{
			return null;
		}
		value = new Material(material);
		cachedMaterialInstances.Add(materialKey, value);
		AddMaterialInstance(value);
		return value;
	}

	private string GetMaterialKey(Material source, MaterialType materialType)
	{
		if (source.name.Contains("_"))
		{
			return $"{source.name.Split('_')[0]}_{materialType.ToString()}";
		}
		return $"{source.name}_{materialType.ToString()}";
	}

	public void AddMaterialInstance(Material materialInstance)
	{
		if (materialInstance == null)
		{
			return;
		}
		if (materialInstances == null)
		{
			materialInstances = new Dictionary<int, List<Material>>();
		}
		int indexForMaterial = GetIndexForMaterial(materialInstance);
		if (indexForMaterial >= 0 && indexForMaterial < atlasMaterialReferencePaths.Count)
		{
			if (materialInstances.TryGetValue(indexForMaterial, out var value))
			{
				value.Add(materialInstance);
				return;
			}
			value = new List<Material>();
			value.Add(materialInstance);
			materialInstances.Add(indexForMaterial, value);
		}
	}

	public void RemoveMaterialInstance(Material materialInstance)
	{
		if (!(materialInstance == null))
		{
			if (materialInstances == null)
			{
				materialInstances = new Dictionary<int, List<Material>>();
			}
			int indexForMaterial = GetIndexForMaterial(materialInstance);
			if (indexForMaterial >= 0 && indexForMaterial < atlasMaterialReferencePaths.Count && materialInstances.TryGetValue(indexForMaterial, out var value))
			{
				value.Remove(materialInstance);
			}
		}
	}

	private int GetIndexForMaterial(Material source)
	{
		if (source != null && source.mainTexture != null)
		{
			for (int num = normalMaterials.Count - 1; num >= 0; num--)
			{
				if (source.mainTexture.name.StartsWith(normalMaterials[num].name))
				{
					return num;
				}
			}
		}
		return -1;
	}

	private void OnApplicationPause(bool paused)
	{
		ReleaseTextureMemory(paused);
	}

	private void ReleaseTextureMemory(bool release)
	{
		if (loadingScreen != null && WPFMonoBehaviour.hudCamera != null)
		{
			loadingScreen.transform.position = WPFMonoBehaviour.hudCamera.transform.position + Vector3.forward;
			loadingScreen.SetActive(release);
		}
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	private void SetEmptyAtlases(bool setEmpty)
	{
		for (int i = 0; i < atlasMaterialReferencePaths.Count; i++)
		{
			Texture mainTexture = emptyTexture;
			if (!setEmpty && atlasMaterialReferencePaths[i] != null)
			{
				Material material = Resources.Load<Material>(atlasMaterialReferencePaths[i]);
				if (material != null)
				{
					mainTexture = material.mainTexture;
				}
			}
			if (i < normalMaterials.Count)
			{
				normalMaterials[i].mainTexture = mainTexture;
			}
			if (i < dimmedRenderQueueMaterials.Count)
			{
				dimmedRenderQueueMaterials[i].mainTexture = mainTexture;
			}
			if (i < renderQueueMaterials.Count)
			{
				renderQueueMaterials[i].mainTexture = mainTexture;
			}
			if (i < partQueueZMaterials.Count)
			{
				partQueueZMaterials[i].mainTexture = mainTexture;
			}
			if (i < grayMaterials.Count)
			{
				grayMaterials[i].mainTexture = mainTexture;
			}
			if (i < shineMaterials.Count)
			{
				shineMaterials[i].mainTexture = mainTexture;
			}
			if (i < alphaMaterials.Count)
			{
				alphaMaterials[i].mainTexture = mainTexture;
			}
			if (materialInstances != null && materialInstances.TryGetValue(i, out var value) && value != null)
			{
				for (int j = 0; j < value.Count; j++)
				{
					value[j].mainTexture = mainTexture;
				}
			}
		}
	}
}
