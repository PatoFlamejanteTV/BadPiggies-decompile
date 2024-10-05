using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
	public enum DataType
	{
		None,
		Terrain,
		PrefabOverrides
	}

	[SerializeField]
	private string m_sceneName;

	[SerializeField]
	private GameObject m_singletonSpawner;

	[SerializeField]
	private List<GameObject> m_prefabs = new List<GameObject>();

	[SerializeField]
	private List<UnityEngine.Object> m_references = new List<UnityEngine.Object>();

	[SerializeField]
	private BundleDataObject m_data;

	private static bool m_isLoadingLevel;

	public string AssetBundleName => m_data.AssetBundle;

	public string SceneName => m_sceneName;

	public static bool IsLoadingLevel()
	{
		return m_isLoadingLevel;
	}

	public void SetSceneName(string sceneName)
	{
		m_sceneName = sceneName;
	}

	public void SetSingletonSpawner(GameObject singletonSpawner)
	{
		m_singletonSpawner = singletonSpawner;
	}

	public void AddPrefab(GameObject prefab)
	{
		m_prefabs.Add(prefab);
	}

	public void ClearPrefabs()
	{
		m_prefabs.Clear();
	}

	public void SetReferences(List<UnityEngine.Object> references)
	{
		m_references = references;
	}

	public Dictionary<GameObject, int> GetPrefabMapping()
	{
		Dictionary<GameObject, int> dictionary = new Dictionary<GameObject, int>();
		for (int i = 0; i < m_prefabs.Count; i++)
		{
			dictionary[m_prefabs[i]] = i;
		}
		return dictionary;
	}

	private void Awake()
	{
		if ((bool)m_singletonSpawner)
		{
			UnityEngine.Object.Instantiate(m_singletonSpawner);
		}
		RenderSettings.ambientLight = Color.white;
		TextAsset textAsset = m_data.LoadValue<TextAsset>();
		using (MemoryStream input = new MemoryStream(textAsset.bytes, writable: false))
		{
			BinaryReader reader = new BinaryReader(input);
			m_isLoadingLevel = true;
			ReadLevel(reader);
			m_isLoadingLevel = false;
		}
		Resources.UnloadAsset(textAsset);
		base.gameObject.SetActive(value: false);
	}

	private void ReadLevel(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			ReadObject(null, reader);
		}
	}

	private void ReadObject(GameObject parent, BinaryReader reader)
	{
		short num = reader.ReadInt16();
		if (num == 0)
		{
			ReadPrefabInstance(parent, reader);
		}
		else
		{
			ReadParentObject(num, parent, reader);
		}
	}

	private void ReadPrefabInstance(GameObject parent, BinaryReader reader)
	{
		string text = reader.ReadString();
		short index = reader.ReadInt16();
		GameObject original = m_prefabs[index];
		Vector3 position = ReadVector3(reader);
		Vector3 euler = ReadVector3(reader);
		Vector3 localScale = ReadVector3(reader);
		GameObject gameObject = UnityEngine.Object.Instantiate(original);
		gameObject.name = text;
		if (parent != null)
		{
			gameObject.transform.parent = parent.transform;
		}
		gameObject.transform.position = position;
		gameObject.transform.rotation = Quaternion.Euler(euler);
		gameObject.transform.localScale = localScale;
		ReadData(gameObject, reader);
	}

	private void ReadParentObject(short childCount, GameObject parent, BinaryReader reader)
	{
		string text = reader.ReadString();
		Vector3 position = ReadVector3(reader);
		GameObject gameObject = new GameObject();
		gameObject.name = text;
		if (parent != null)
		{
			gameObject.transform.parent = parent.transform;
		}
		gameObject.transform.position = position;
		for (int i = 0; i < childCount; i++)
		{
			ReadObject(gameObject, reader);
		}
	}

	private Vector2 ReadVector2(BinaryReader reader)
	{
		Vector2 result = default(Vector2);
		result.x = reader.ReadSingle();
		result.y = reader.ReadSingle();
		return result;
	}

	private Vector3 ReadVector3(BinaryReader reader)
	{
		Vector3 result = default(Vector3);
		result.x = reader.ReadSingle();
		result.y = reader.ReadSingle();
		result.z = reader.ReadSingle();
		return result;
	}

	private Color ReadColor(BinaryReader reader)
	{
		uint num = reader.ReadUInt32();
		Color result = default(Color);
		result.r = (float)((num >> 24) & 0xFFu) * 0.003921569f;
		result.g = (float)((num >> 16) & 0xFFu) * 0.003921569f;
		result.b = (float)((num >> 8) & 0xFFu) * 0.003921569f;
		result.a = (float)(num & 0xFFu) * 0.003921569f;
		return result;
	}

	private void ReadData(GameObject obj, BinaryReader reader)
	{
		switch ((DataType)reader.ReadByte())
		{
		case DataType.Terrain:
			ReadTerrain(obj, reader);
			break;
		case DataType.PrefabOverrides:
			ReadPrefabOverrides(obj, reader);
			break;
		}
		float @float = INSettings.GetFloat(INFeature.TerrainScale);
		Vector3 position = obj.transform.position;
		Vector3 localScale = obj.transform.localScale;
		obj.transform.position = new Vector3(position.x * @float, position.y * @float, position.z);
		obj.transform.localScale = new Vector3(localScale.x * @float, localScale.y * @float, localScale.z);
	}

	private void ReadPrefabOverrides(GameObject obj, BinaryReader reader)
	{
		int num = reader.ReadInt32();
		byte[] buffer = new byte[num];
		reader.Read(buffer, 0, num);
		using (MemoryStream stream = new MemoryStream(buffer))
		{
			using StreamReader reader2 = new StreamReader(stream, Encoding.UTF8);
			ObjectDeserializer.ObjectReader reader3 = new ObjectDeserializer.ObjectReader(reader2, m_references);
			ObjectDeserializer.ReadFile(obj, reader3);
		}
		obj.SendMessage("OnDataLoaded", SendMessageOptions.DontRequireReceiver);
	}

	private void ReadTerrain(GameObject obj, BinaryReader reader)
	{
		e2dTerrain component = obj.GetComponent<e2dTerrain>();
		component.FillTextureTileOffsetX = reader.ReadSingle();
		component.FillTextureTileOffsetY = reader.ReadSingle();
		GameObject gameObject = obj.transform.Find("_fill").gameObject;
		MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
		Mesh sharedMesh = ReadMesh(readUV: false, fillMesh: true, readColor: false, reader, component);
		Color color = ReadColor(reader);
		int index = reader.ReadInt32();
		component2.sharedMesh = sharedMesh;
		gameObject.GetComponent<Renderer>().sharedMaterial.color = color;
		gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = m_references[index] as Texture2D;
		MeshFilter component3 = obj.transform.Find("_curve").gameObject.GetComponent<MeshFilter>();
		sharedMesh = ReadMesh(readUV: false, fillMesh: false, readColor: true, reader, component);
		component3.sharedMesh = sharedMesh;
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			if (i >= component.CurveTextures.Count)
			{
				component.CurveTextures.Add(new e2dCurveTexture(null));
			}
			int index2 = reader.ReadInt32();
			component.CurveTextures[i].texture = m_references[index2] as Texture;
			component.CurveTextures[i].size = ReadVector2(reader);
			component.CurveTextures[i].fixedAngle = reader.ReadBoolean();
			component.CurveTextures[i].fadeThreshold = reader.ReadSingle();
		}
		if (reader.ReadInt32() > 0)
		{
			int count = reader.ReadInt32();
			byte[] data = reader.ReadBytes(count);
			Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, mipChain: false);
			texture2D.filterMode = FilterMode.Bilinear;
			texture2D.wrapMode = TextureWrapMode.Clamp;
			texture2D.anisoLevel = 1;
			if (!texture2D.LoadImage(data))
			{
				throw new InvalidOperationException("Can't load control texture");
			}
			component.CurveMesh.ControlTextures.Clear();
			component.CurveMesh.ControlTextures.Add(texture2D);
			component.CurveMesh.RebuildMaterial();
		}
		if (reader.ReadBoolean())
		{
			Mesh sharedMesh2 = gameObject.GetComponent<MeshFilter>().sharedMesh;
			CreateCollider(obj, sharedMesh2.vertices);
		}
		else if ((bool)obj.transform.Find(e2dConstants.COLLIDER_MESH_NAME))
		{
			UnityEngine.Object.Destroy(obj.transform.Find(e2dConstants.COLLIDER_MESH_NAME).gameObject);
		}
	}

	private Mesh ReadMesh(bool readUV, bool fillMesh, bool readColor, BinaryReader reader, e2dTerrain terrain)
	{
		Mesh mesh = new Mesh();
		int num = reader.ReadInt32();
		Vector3[] array = new Vector3[num];
		if (fillMesh)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ReadVector2(reader);
			}
			Vector2[] array2 = new Vector2[array.Length];
			float num2 = 1f / terrain.FillTextureTileWidth;
			float num3 = 1f / terrain.FillTextureTileHeight;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].x = (array[j].x - terrain.FillTextureTileOffsetX) * num2;
				array2[j].y = (array[j].y - terrain.FillTextureTileOffsetY) * num3;
			}
			mesh.vertices = array;
			mesh.uv = array2;
		}
		else
		{
			Color[] array3 = new Color[array.Length];
			for (int k = 0; k < array.Length; k++)
			{
				array[k] = ReadVector2(reader);
				array[k].z = -0.01f;
				array3[k].r = (k + 1) % 2;
			}
			Vector2[] array4 = new Vector2[array.Length];
			int num4 = num / 2;
			float num5 = 0f;
			for (int l = 1; l < num4; l++)
			{
				int num6 = l;
				int num7 = 2 * num6;
				float magnitude = (array[num7] - array[num7 - 2]).magnitude;
				num5 += magnitude;
				array4[num7] = (array4[num7 + 1] = new Vector2(num5, num6));
			}
			mesh.vertices = array;
			mesh.colors = array3;
			mesh.uv = array4;
		}
		if (readUV)
		{
			Vector2[] array5 = new Vector2[reader.ReadInt32()];
			for (int m = 0; m < array5.Length; m++)
			{
				array5[m] = ReadVector2(reader);
			}
			mesh.uv = array5;
		}
		int num8 = reader.ReadInt32();
		int[] array6 = new int[num8];
		for (int n = 0; n < num8; n++)
		{
			array6[n] = reader.ReadInt16();
		}
		mesh.triangles = array6;
		return mesh;
	}

	private Vector2 GetPointFillUV(e2dTerrain terrain, Vector2 curvePoint)
	{
		float x = (curvePoint.x - terrain.FillTextureTileOffsetX) / terrain.FillTextureTileWidth;
		float y = (curvePoint.y - terrain.FillTextureTileOffsetY) / terrain.FillTextureTileHeight;
		return new Vector2(x, y);
	}

	public void CreateCollider(GameObject obj, Vector3[] polygon)
	{
		Vector3[] array = new Vector3[2 * polygon.Length];
		int[] array2 = new int[6 * polygon.Length];
		for (int i = 0; i < polygon.Length; i++)
		{
			int num = 2 * i;
			array[num] = new Vector3(polygon[i].x, polygon[i].y, -0.5f * e2dConstants.COLLISION_MESH_Z_DEPTH);
			array[num + 1] = new Vector3(polygon[i].x, polygon[i].y, 0.5f * e2dConstants.COLLISION_MESH_Z_DEPTH);
			int num2 = 6 * i;
			array2[num2] = num % array.Length;
			array2[num2 + 1] = (num + 1) % array.Length;
			array2[num2 + 2] = (num + 2) % array.Length;
			array2[num2 + 3] = (num + 2) % array.Length;
			array2[num2 + 4] = (num + 1) % array.Length;
			array2[num2 + 5] = (num + 3) % array.Length;
		}
		Transform obj2 = obj.transform.Find(e2dConstants.COLLIDER_MESH_NAME);
		MeshCollider component = obj2.GetComponent<MeshCollider>();
		component.sharedMesh = new Mesh
		{
			vertices = array,
			triangles = array2
		};
		obj2.localPosition = Vector3.zero;
		obj2.localRotation = Quaternion.identity;
		obj2.localScale = Vector3.one;
		if (obj.layer != LayerMask.NameToLayer("IceGround"))
		{
			component.gameObject.layer = LayerMask.NameToLayer("Ground");
		}
	}

	public override string ToString()
	{
		return SceneName;
	}
}
