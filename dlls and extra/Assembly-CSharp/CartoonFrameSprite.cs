using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CartoonFrameSprite : MonoBehaviour
{
	[HideInInspector]
	public int m_textureWidth;

	[HideInInspector]
	public int m_textureHeight;

	public int m_spriteWidth;

	public int m_spriteHeight;

	public int m_UVx;

	public int m_UVy;

	public int m_width = 16;

	public int m_height = 16;

	public int m_subdivisionsX;

	public int m_subdivisionsY;

	[HideInInspector]
	public Rect m_uvRect;

	public const float DefaultCameraSize = 10f;

	public const float DefaultCameraHeight = 20f;

	public const float DefaultScreenHeight = 768f;

	protected Mesh m_spriteMesh;

	public Vector2 Size => new Vector2(base.transform.localScale.x * (float)m_spriteWidth / 768f * 20f, base.transform.localScale.y * (float)m_spriteHeight / 768f * 20f);

	public Mesh SpriteMesh => m_spriteMesh;

	private void Awake()
	{
		MeshFilter meshFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
		if (!meshFilter.sharedMesh)
		{
			CreatePlane(meshFilter, m_UVx, m_UVy, m_spriteWidth, m_spriteHeight);
			MapUVToTexture(m_UVx, m_UVy, m_width, m_height);
		}
	}

	public void RebuildMesh()
	{
		MeshFilter mf = GetComponent(typeof(MeshFilter)) as MeshFilter;
		CreatePlane(mf, m_UVx, m_UVy, m_spriteWidth, m_spriteHeight);
		MapUVToTexture(m_UVx, m_UVy, m_width, m_height);
	}

	public void ResetSize()
	{
		m_textureWidth = m_width * GetComponent<Renderer>().sharedMaterial.mainTexture.width / m_subdivisionsX;
		m_textureHeight = m_height * GetComponent<Renderer>().sharedMaterial.mainTexture.height / m_subdivisionsY;
		m_spriteWidth = m_textureWidth;
		m_spriteHeight = m_textureHeight;
		RebuildMesh();
	}

	public void MapUVToTexture(int UVx, int UVy, int width, int height)
	{
		if (!(m_spriteMesh == null))
		{
			m_UVx = UVx;
			m_UVy = UVy;
			m_width = width;
			m_height = height;
			Vector3[] array = new Vector3[4]
			{
				new Vector3(-1f, -1f, 0f),
				new Vector3(-1f, 1f, 0f),
				new Vector3(1f, 1f, 0f),
				new Vector3(1f, -1f, 0f)
			};
			Vector2[] array2 = new Vector2[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				float num = 0.5f * (1f / (float)m_subdivisionsX / (float)(1024 / m_subdivisionsX)) * array[i].x;
				float num2 = 0.5f * (1f / (float)m_subdivisionsY / (float)(1024 / m_subdivisionsY)) * array[i].y;
				float x = Mathf.Clamp(array[i].x, 0f, 1f) * (1f / (float)m_subdivisionsX) * (float)m_width + (float)m_UVx * (1f / (float)m_subdivisionsX) - num;
				float y = Mathf.Clamp(array[i].y, 0f, 1f) * (1f / (float)m_subdivisionsY) * (float)m_height + (float)m_UVy * (1f / (float)m_subdivisionsY) - num2;
				array2[i] = new Vector2(x, y);
			}
			m_uvRect = new Rect(array2[0].x, array2[0].y, array2[2].x - array2[0].x, array2[2].y - array2[0].y);
			if ((bool)m_spriteMesh)
			{
				m_spriteMesh.uv = array2;
			}
		}
	}

	public void SetUVs(Vector2[] newUVs)
	{
		m_spriteMesh.uv = newUVs;
	}

	public void CreatePlane(MeshFilter mf, int x, int y, int width, int height)
	{
		float num = 2f * (float)width * 10f / 768f;
		float num2 = 2f * (float)height * 10f / 768f;
		float num3 = (float)(2 * x - m_subdivisionsX) * 10f / 768f;
		float num4 = (float)(2 * y - m_subdivisionsY) * 10f / 768f;
		Mesh mesh = new Mesh();
		mesh.name = "GeneratedMesh_" + width + "x" + height;
		Vector3[] array = new Vector3[4];
		int[] triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
		array[0] = new Vector3(num3, num4, 0f);
		array[1] = new Vector3(num3, num4 + num2, 0f);
		array[2] = new Vector3(num3 + num, num4 + num2, 0f);
		array[3] = new Vector3(num3 + num, num4, 0f);
		mesh.vertices = array;
		mesh.triangles = triangles;
		mf.sharedMesh = mesh;
		m_spriteMesh = mf.sharedMesh;
		mf.sharedMesh.RecalculateNormals();
		mf.sharedMesh.RecalculateBounds();
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			return;
		}
		MeshFilter component = base.transform.GetComponent<MeshFilter>();
		if ((bool)component && (bool)GetComponent<Renderer>().sharedMaterial && (bool)GetComponent<Renderer>().sharedMaterial.mainTexture)
		{
			m_textureWidth = m_width * GetComponent<Renderer>().sharedMaterial.mainTexture.width / m_subdivisionsX;
			m_textureHeight = m_height * GetComponent<Renderer>().sharedMaterial.mainTexture.height / m_subdivisionsY;
			if (m_spriteWidth == 0)
			{
				m_spriteWidth = m_textureWidth;
			}
			if (m_spriteHeight == 0)
			{
				m_spriteHeight = m_textureHeight;
			}
			if (!component.sharedMesh)
			{
				CreatePlane(component, m_UVx, m_UVy, m_spriteWidth, m_spriteHeight);
				MapUVToTexture(m_UVx, m_UVy, m_width, m_height);
			}
		}
	}
}
