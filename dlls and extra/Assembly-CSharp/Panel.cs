using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(SpriteReference))]
[ExecuteInEditMode]
public class Panel : MonoBehaviour, SpriteMeshGenerator
{
	public int m_width = 5;

	public int m_height = 3;

	public const float DefaultCameraSize = 10f;

	public const float DefaultCameraHeight = 20f;

	public const float DefaultScreenHeight = 768f;

	private int m_spriteWidth;

	private int m_spriteHeight;

	private Rect m_spriteUVRect;

	private RuntimeSpriteDatabase m_spriteDatabase;

	private SpriteData m_spriteData;

	public int width
	{
		get
		{
			return m_width;
		}
		set
		{
			if (value > 0 && value != m_width)
			{
				m_width = value;
				Rebuild(m_spriteData);
			}
		}
	}

	private void Awake()
	{
		if (Application.isPlaying)
		{
			RuntimeSpriteDatabase instance = Singleton<RuntimeSpriteDatabase>.Instance;
			m_spriteData = instance.Find(GetComponent<SpriteReference>().Id);
			Rebuild(m_spriteData);
		}
	}

	private void OnDestroy()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		if (!Application.isPlaying)
		{
			if ((bool)component.sharedMesh)
			{
				Object.DestroyImmediate(component.sharedMesh);
			}
		}
		else
		{
			Object.Destroy(component.sharedMesh);
		}
	}

	private void Rebuild(SpriteData spriteData)
	{
		float scaleX = GetComponent<SpriteReference>().m_scaleX;
		float scaleY = GetComponent<SpriteReference>().m_scaleY;
		int num = (int)(scaleX * (float)spriteData.width / 3f);
		int num2 = (int)(scaleY * (float)spriteData.height / 3f);
		int num3 = num * m_width;
		int num4 = num2 * m_height;
		if (num3 != m_spriteWidth || num4 != m_spriteHeight || m_spriteUVRect != spriteData.uv)
		{
			m_spriteWidth = num3;
			m_spriteHeight = num4;
			m_spriteUVRect = spriteData.uv;
			CreateMesh(GetComponent<MeshFilter>(), num, num2, spriteData, 0, 0);
		}
	}

	public void CreateMesh(MeshFilter mf, int tileWidth, int tileHeight, SpriteData data, int pivotX, int pivotY)
	{
		Vector3[] array = new Vector3[4 * m_width * m_height];
		int[] array2 = new int[6 * m_width * m_height];
		Vector2[] array3 = new Vector2[4 * m_width * m_height];
		Mesh mesh = new Mesh();
		mesh.name = "GeneratedPanelMesh_" + m_width * tileWidth + "x" + m_height * tileHeight;
		if (!Application.isPlaying)
		{
			mesh.hideFlags = HideFlags.DontSave;
		}
		int num = 0;
		for (int i = 0; i < m_height; i++)
		{
			for (int j = 0; j < m_width; j++)
			{
				float num2 = 2f * (float)tileWidth * 10f / 768f;
				float num3 = 2f * (float)tileHeight * 10f / 768f;
				float num4 = -2f * (float)pivotX * 10f / 768f;
				float num5 = -2f * (float)pivotY * 10f / 768f;
				float num6 = num2 * (-0.5f * (float)m_width + (float)j);
				float num7 = num3 * (-0.5f * (float)m_height + (float)i);
				array[num * 4] = new Vector3(num6 + num4, num7 + num5, 0f);
				array[num * 4 + 1] = new Vector3(num6 + num4, num7 + num5 + num3, 0f);
				array[num * 4 + 2] = new Vector3(num6 + num4 + num2, num7 + num5 + num3, 0f);
				array[num * 4 + 3] = new Vector3(num6 + num4 + num2, num7 + num5, 0f);
				array2[num * 6] = num * 4;
				array2[num * 6 + 1] = num * 4 + 1;
				array2[num * 6 + 2] = num * 4 + 2;
				array2[num * 6 + 3] = num * 4 + 2;
				array2[num * 6 + 4] = num * 4 + 3;
				array2[num * 6 + 5] = num * 4;
				Rect uvs = GetUvs(j, i, data);
				array3[num * 4].x = uvs.x;
				array3[num * 4].y = uvs.y;
				array3[num * 4 + 1].x = uvs.x;
				array3[num * 4 + 1].y = uvs.y + uvs.height;
				array3[num * 4 + 2].x = uvs.x + uvs.width;
				array3[num * 4 + 2].y = uvs.y + uvs.height;
				array3[num * 4 + 3].x = uvs.x + uvs.width;
				array3[num * 4 + 3].y = uvs.y;
				num++;
			}
		}
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.uv = array3;
		mf.sharedMesh = mesh;
		mf.sharedMesh.RecalculateNormals();
		mf.sharedMesh.RecalculateBounds();
	}

	private Rect GetUvs(int x, int y, SpriteData data)
	{
		Rect result = new Rect(data.uv.x, data.uv.y, data.uv.width / 3f, data.uv.height / 3f);
		if (x == m_width - 1)
		{
			result.x += 2f * result.width;
		}
		else if (x > 0)
		{
			result.x += result.width;
		}
		if (y == m_height - 1)
		{
			result.y += 2f * result.height;
		}
		else if (y > 0)
		{
			result.y += result.height;
		}
		return result;
	}

	private void OnDrawGizmos()
	{
		RefreshEditorView();
	}

	public void RefreshEditorView()
	{
		if (!Application.isPlaying && (bool)GetComponent<Renderer>().sharedMaterial && (bool)GetComponent<Renderer>().sharedMaterial.mainTexture)
		{
			if (m_spriteDatabase == null)
			{
				m_spriteDatabase = Singleton<RuntimeSpriteDatabase>.Instance;
			}
			SpriteData spriteData = m_spriteDatabase.Find(GetComponent<SpriteReference>().Id);
			if (spriteData != null)
			{
				Rebuild(spriteData);
			}
		}
	}
}
