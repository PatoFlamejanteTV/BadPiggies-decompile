using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MeshCombine : Singleton<MeshCombine>
{
	private const int VERTEX_LIMIT = 65534;

	private List<int> groundDepths;

	private void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void Start()
	{
		SetAsPersistant();
		CombineScene();
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		CombineScene();
	}

	private void CombineScene()
	{
		List<List<GameObject>> list = FindGrounds();
		for (int i = 0; i < list.Count; i++)
		{
			List<GameObject> list2 = new List<GameObject>();
			for (int j = 0; j < list[i].Count; j++)
			{
				list2.AddRange(FindChilds(list[i][j]));
			}
			List<List<GameObject>> list3 = SortGameObjectsByMaterial(list2);
			for (int k = 0; k < list3.Count; k++)
			{
				Combine(list3[k].ToArray());
			}
		}
		List<List<GameObject>> list4 = FindProps();
		for (int l = 0; l < list4.Count; l++)
		{
			List<List<GameObject>> list5 = SortGameObjectsByMaterial(list4[l]);
			for (int m = 0; m < list5.Count; m++)
			{
				Combine(list5[m].ToArray());
			}
		}
		StartCoroutine(DelayedAction(delegate
		{
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}));
	}

	private List<GameObject> FindChilds(GameObject parent)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			GameObject gameObject = parent.transform.GetChild(i).gameObject;
			if (gameObject.tag == "Static" && gameObject.GetComponent<Renderer>() != null)
			{
				list.Add(gameObject);
			}
			list.AddRange(FindChilds(gameObject));
		}
		return list;
	}

	private void Combine(GameObject[] objects)
	{
		List<MeshFilter> list = new List<MeshFilter>();
		float num = 0f;
		for (int i = 0; i < objects.Length; i++)
		{
			num = ((i != 0) ? ((num + objects[i].transform.position.z) * 0.5f) : objects[i].transform.position.z);
			MeshFilter component = objects[i].GetComponent<MeshFilter>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		int num2 = 0;
		bool flag = true;
		while (flag)
		{
			List<MeshFilter> list2 = new List<MeshFilter>();
			while (list.Count > 0 && num2 + list[0].sharedMesh.vertexCount < 65534)
			{
				MeshFilter meshFilter = FindMeshFilter(list, 65534 - num2);
				if (!(meshFilter != null))
				{
					break;
				}
				if (!list2.Contains(meshFilter))
				{
					list2.Add(meshFilter);
					num2 += meshFilter.sharedMesh.vertexCount;
					list.Remove(meshFilter);
				}
			}
			Combine(list2.ToArray(), num);
			num2 = 0;
			flag = list.Count > 0;
		}
	}

	private void Combine(MeshFilter[] meshFilters, float depth)
	{
		if (meshFilters.Length == 0)
		{
			return;
		}
		CombineInstance[] array = new CombineInstance[meshFilters.Length];
		for (int i = 0; i < array.Length; i++)
		{
			meshFilters[i].transform.position -= Vector3.forward * depth;
			array[i].mesh = meshFilters[i].sharedMesh;
			array[i].transform = meshFilters[i].transform.localToWorldMatrix;
			meshFilters[i].GetComponent<Renderer>().enabled = false;
		}
		GameObject obj = new GameObject("CombinedMesh_" + meshFilters[0].GetComponent<Renderer>().sharedMaterial.name);
		obj.transform.position += Vector3.forward * depth;
		MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
		obj.AddComponent<MeshRenderer>();
		obj.GetComponent<Renderer>().sharedMaterial = meshFilters[0].GetComponent<Renderer>().sharedMaterial;
		meshFilter.sharedMesh = new Mesh();
		meshFilter.sharedMesh.name = "CombinedMesh";
		meshFilter.sharedMesh.CombineMeshes(array);
		for (int j = 0; j < meshFilters.Length; j++)
		{
			if (meshFilters[j].gameObject.GetComponent<PointLightSource>() == null)
			{
				UnityEngine.Object.Destroy(meshFilters[j].gameObject);
			}
		}
	}

	private MeshFilter FindMeshFilter(List<MeshFilter> meshFilters, int vertexLimit)
	{
		for (int i = 0; i < meshFilters.Count; i++)
		{
			if (meshFilters[i].sharedMesh.vertexCount <= vertexLimit)
			{
				return meshFilters[i];
			}
		}
		return null;
	}

	private List<List<GameObject>> SortGameObjectsByMaterial(List<GameObject> gameObjects)
	{
		List<string> list = new List<string>();
		List<List<GameObject>> list2 = new List<List<GameObject>>();
		for (int i = 0; i < gameObjects.Count; i++)
		{
			if (!(gameObjects[i].GetComponent<Renderer>() == null))
			{
				string empty = string.Empty;
				empty = ((!(gameObjects[i].GetComponent<Renderer>().sharedMaterial.mainTexture != null)) ? gameObjects[i].GetComponent<Renderer>().sharedMaterial.name : (gameObjects[i].GetComponent<Renderer>().sharedMaterial.name + "_" + gameObjects[i].GetComponent<Renderer>().sharedMaterial.mainTexture.name));
				if (!list.Contains(empty))
				{
					list.Add(empty);
				}
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			list2.Add(new List<GameObject>());
		}
		for (int k = 0; k < gameObjects.Count; k++)
		{
			if (!(gameObjects[k].GetComponent<Renderer>() == null))
			{
				string empty2 = string.Empty;
				empty2 = ((!(gameObjects[k].GetComponent<Renderer>().sharedMaterial.mainTexture != null)) ? gameObjects[k].GetComponent<Renderer>().sharedMaterial.name : (gameObjects[k].GetComponent<Renderer>().sharedMaterial.name + "_" + gameObjects[k].GetComponent<Renderer>().sharedMaterial.mainTexture.name));
				if (!list2[list.IndexOf(empty2)].Contains(gameObjects[k]))
				{
					list2[list.IndexOf(empty2)].Add(gameObjects[k]);
				}
			}
		}
		return list2;
	}

	private List<List<GameObject>> FindGrounds()
	{
		List<GameObject> list = new List<GameObject>(GameObject.FindGameObjectsWithTag("Ground"));
		List<List<GameObject>> list2 = new List<List<GameObject>>();
		List<int> list3 = new List<int>();
		groundDepths = new List<int>();
		groundDepths.Add(0);
		if (list.Count == 0)
		{
			return list2;
		}
		List<string> list4 = new List<string>();
		for (int i = 0; i < list.Count; i++)
		{
			int num = (int)(list[i].transform.position.z * 100f);
			bool flag = false;
			for (int j = 0; j < groundDepths.Count; j++)
			{
				if (groundDepths[j] == num)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				int num2 = -1;
				for (int k = 0; k < groundDepths.Count; k++)
				{
					if (groundDepths[k] > num)
					{
						num2 = k;
						break;
					}
				}
				if (num2 >= 0)
				{
					groundDepths.Insert(num2, num);
				}
				else
				{
					groundDepths.Add(num);
				}
			}
			list3.Add(num);
			if (!list4.Contains(GenerateGroundName(list[i], num)))
			{
				list4.Add(GenerateGroundName(list[i], num));
			}
		}
		for (int l = 0; l < list4.Count; l++)
		{
			list2.Add(new List<GameObject>());
		}
		for (int m = 0; m < list.Count; m++)
		{
			list2[list4.IndexOf(GenerateGroundName(list[m], list3[m]))].Add(list[m]);
		}
		return list2;
	}

	private string GenerateGroundName(GameObject go, int depth)
	{
		if (depth >= 0)
		{
			return $"{go.name}_{depth}";
		}
		return go.name;
	}

	private List<List<GameObject>> FindProps()
	{
		List<GameObject> list = new List<GameObject>(GameObject.FindGameObjectsWithTag("Prop"));
		List<List<GameObject>> list2 = new List<List<GameObject>>();
		if (list.Count == 0)
		{
			return list2;
		}
		List<bool> list3 = new List<bool>();
		for (int i = 0; i < list.Count; i++)
		{
			list3.Add(item: false);
		}
		for (int j = 0; j < groundDepths.Count; j++)
		{
			List<GameObject> list4 = new List<GameObject>();
			for (int k = 0; k < list.Count; k++)
			{
				int num = (int)(list[k].transform.position.z * 100f);
				if (list3[k] || num >= groundDepths[j])
				{
					continue;
				}
				int num2 = -1;
				for (int l = 0; l < list4.Count; l++)
				{
					if ((int)(list4[l].transform.position.z * 100f) < num)
					{
						num2 = l;
						break;
					}
				}
				if (num2 >= 0)
				{
					list4.Insert(num2, list[k]);
				}
				else
				{
					list4.Add(list[k]);
				}
				list3[k] = true;
			}
			if (list4.Count > 0)
			{
				list2.Add(list4);
			}
		}
		return list2;
	}

	private IEnumerator DelayedAction(Action action)
	{
		yield return null;
		action?.Invoke();
	}
}
