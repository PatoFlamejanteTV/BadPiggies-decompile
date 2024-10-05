using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PointLightContainer : MonoBehaviour
{
	public Material lightMaterial;

	public Material borderMaterial;

	public GameObject pointLightPrefab;

	public GameObject beamLightPrefab;

	public GameObject borderPrefab;

	private GameObject lightContainer;

	private GameObject lightSkinMesh;

	private Transform root;

	private SkinnedMeshRenderer lightSmr;

	private List<PointLightMask> lights;

	private List<PointLightSource> lightSources;

	private static PointLightContainer instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		root = base.transform;
		UpdateMeshes();
	}

	private void Update()
	{
		for (int i = 0; i < lights.Count; i++)
		{
			if (lights[i].lightSource != null)
			{
				lights[i].transform.position = new Vector3(lights[i].lightSource.position.x, lights[i].lightSource.position.y, lights[i].transform.position.z);
				lights[i].transform.rotation = Quaternion.AngleAxis(lights[i].lightSource.rotation.eulerAngles.z, Vector3.forward);
			}
			lights[i].border.transform.position = lights[i].transform.position + Vector3.forward * 0.5f;
			lights[i].border.transform.rotation = Quaternion.AngleAxis(lights[i].transform.rotation.eulerAngles.z, Vector3.forward);
			if (lights[i].lightType == PointLightMask.LightType.PointLight)
			{
				Vector3 localScale = lights[i].transform.localScale;
				lights[i].border.transform.localScale = new Vector3(localScale.x + lights[i].borderWidth, localScale.y + lights[i].borderWidth, 1f);
			}
			else
			{
				lights[i].border.transform.localScale = lights[i].transform.localScale;
			}
		}
	}

	public void UpdateMeshes()
	{
		if (lightContainer == null)
		{
			lightContainer = new GameObject("LightMeshes");
			lightContainer.transform.parent = root;
			lightContainer.transform.localPosition = Vector3.zero;
			lightContainer.transform.localScale = Vector3.one;
			lightContainer.transform.localRotation = Quaternion.identity;
		}
		if (lightContainer.transform.childCount > 0)
		{
			for (int i = 0; i < lightContainer.transform.childCount; i++)
			{
				Object.Destroy(lightContainer.transform.GetChild(i).gameObject);
			}
		}
		lights = new List<PointLightMask>();
		lightSources = new List<PointLightSource>();
		PointLightSource[] array = Object.FindObjectsOfType<PointLightSource>();
		foreach (PointLightSource item in array)
		{
			lightSources.Add(item);
		}
		for (int k = 0; k < lightSources.Count; k++)
		{
			PointLightSource lightSource = lightSources[k];
			CreateLight(k, lightSource);
		}
		if (lightSkinMesh == null)
		{
			lightSkinMesh = new GameObject("LightSkinnedMesh");
			lightSkinMesh.transform.parent = root;
			lightSkinMesh.transform.localPosition = Vector3.zero;
			lightSkinMesh.transform.localRotation = Quaternion.identity;
			lightSmr = lightSkinMesh.AddComponent<SkinnedMeshRenderer>();
			lightSmr.updateWhenOffscreen = true;
		}
		CollectMeshes();
	}

	private void CreateLight(int id, PointLightSource _lightSource)
	{
		Transform parent = lightContainer.transform;
		GameObject gameObject;
		if (_lightSource.lightType == PointLightMask.LightType.PointLight)
		{
			gameObject = Object.Instantiate(pointLightPrefab);
			gameObject.name = $"PointLight{id:00}";
		}
		else
		{
			gameObject = Object.Instantiate(beamLightPrefab);
			gameObject.name = $"BeamLight{id:00}";
		}
		gameObject.transform.parent = parent;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localRotation = Quaternion.identity;
		PointLightMask component = gameObject.GetComponent<PointLightMask>();
		component.border = Object.Instantiate(borderPrefab);
		component.border.transform.parent = parent;
		component.border.transform.localPosition = new Vector3(0f, 0f, 0.1f);
		component.border.transform.localScale = Vector3.one;
		component.border.transform.localRotation = Quaternion.identity;
		component.border.name = $"Border{id:00}";
		component.radius = _lightSource.size;
		component.borderWidth = _lightSource.borderWidth;
		component.lightSource = _lightSource.transform;
		component.vertexCount = _lightSource.vertexCount;
		_lightSource.lightTransform = component.transform;
		component.lightType = _lightSource.lightType;
		if (component.lightType == PointLightMask.LightType.BeamLight)
		{
			BeamLightMask obj = component as BeamLightMask;
			obj.angle = _lightSource.beamAngle;
			obj.cutHeight = _lightSource.beamCut;
			GameObject obj2 = Object.Instantiate(pointLightPrefab);
			obj2.name = $"BeamLightBase{id:00}";
			obj2.transform.parent = parent;
			obj2.transform.localPosition = Vector3.zero;
			obj2.transform.localScale = Vector3.one;
			obj2.transform.localRotation = Quaternion.identity;
			PointLightMask component2 = obj2.GetComponent<PointLightMask>();
			component2.border = Object.Instantiate(borderPrefab);
			component2.border.transform.parent = parent;
			component2.border.transform.localPosition = new Vector3(0f, 0f, 0.1f);
			component2.border.transform.localScale = Vector3.one;
			component2.border.transform.localRotation = Quaternion.identity;
			component2.border.name = $"Border{id:00}";
			component2.radius = 0.5f;
			component2.borderWidth = 0.5f;
			component2.lightSource = _lightSource.transform;
			component2.vertexCount = _lightSource.vertexCount;
			_lightSource.baseLightTransform = component2.transform;
			component2.lightType = PointLightMask.LightType.PointLight;
			component2.UpdateMesh();
			_lightSource.colliderSize = component2.radius + component2.borderWidth;
			lights.Add(component2);
		}
		component.UpdateMesh();
		if (component.lightType == PointLightMask.LightType.BeamLight)
		{
			BeamLightMask beamLightMask = (BeamLightMask)component;
			_lightSource.colliderSize = beamLightMask.colliderSize;
			_lightSource.beamArcCenter = beamLightMask.arcCenter;
		}
		else
		{
			_lightSource.colliderSize = _lightSource.size + _lightSource.borderWidth;
		}
		lights.Add(component);
	}

	public void CollectMeshes()
	{
		int count = lights.Count;
		Transform[] array = new Transform[count * 2];
		List<BoneWeight> list = new List<BoneWeight>();
		List<Matrix4x4> list2 = new List<Matrix4x4>();
		List<Vector3> list3 = new List<Vector3>();
		List<int> list4 = new List<int>();
		List<int> list5 = new List<int>();
		List<Vector2> list6 = new List<Vector2>();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < count; i++)
		{
			PointLightMask pointLightMask = lights[i];
			MeshFilter meshFilter = pointLightMask.meshFilter;
			Transform transform = (array[num2] = pointLightMask.transform);
			list2.Add(array[num2].worldToLocalMatrix * root.localToWorldMatrix);
			for (int j = 0; j < meshFilter.sharedMesh.vertexCount; j++)
			{
				list3.Add(transform.localPosition + meshFilter.sharedMesh.vertices[j]);
				list6.Add(meshFilter.sharedMesh.uv[j]);
				list.Add(new BoneWeight
				{
					boneIndex0 = num2,
					weight0 = 1f
				});
			}
			for (int k = 0; k < meshFilter.sharedMesh.triangles.Length; k++)
			{
				list4.Add(num + meshFilter.sharedMesh.triangles[k]);
			}
			num += meshFilter.sharedMesh.vertexCount;
			num2++;
		}
		for (int l = 0; l < count; l++)
		{
			PointLightMask pointLightMask2 = lights[l];
			if (!(pointLightMask2.border == null))
			{
				MeshFilter component = pointLightMask2.border.GetComponent<MeshFilter>();
				Transform transform2 = (array[num2] = pointLightMask2.border.transform);
				list2.Add(array[num2].worldToLocalMatrix * root.localToWorldMatrix);
				for (int m = 0; m < component.sharedMesh.vertexCount; m++)
				{
					list3.Add(transform2.localPosition + component.sharedMesh.vertices[m]);
					list6.Add(component.sharedMesh.uv[m]);
					list.Add(new BoneWeight
					{
						boneIndex0 = num2,
						weight0 = 1f
					});
				}
				for (int n = 0; n < component.sharedMesh.triangles.Length; n++)
				{
					list5.Add(num + component.sharedMesh.triangles[n]);
				}
				num += component.sharedMesh.vertexCount;
				num2++;
			}
		}
		Mesh mesh = new Mesh();
		mesh.name = "LightCompoundMesh";
		mesh.vertices = list3.ToArray();
		mesh.subMeshCount = 2;
		mesh.SetTriangles(list4.ToArray(), 0);
		mesh.SetTriangles(list5.ToArray(), 1);
		mesh.uv = list6.ToArray();
		mesh.boneWeights = list.ToArray();
		mesh.bindposes = list2.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		Material[] sharedMaterials = new Material[2] { lightMaterial, borderMaterial };
		lightSmr.sharedMaterials = sharedMaterials;
		lightSmr.bones = array;
		lightSmr.sharedMesh = mesh;
		lightSmr.shadowCastingMode = ShadowCastingMode.Off;
		lightSmr.receiveShadows = false;
		foreach (PointLightSource lightSource in lightSources)
		{
			if (lightSource.lightType == PointLightMask.LightType.PointLight)
			{
				lightSource.lightTransform.localScale = new Vector3(lightSource.size, lightSource.size, 1f);
			}
			else
			{
				lightSource.baseLightTransform.localScale = new Vector3(0.5f, 0.5f, 1f);
			}
			lightSource.Init();
		}
	}
}
