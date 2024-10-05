using UnityEngine;

public class SpriteManager : MonoBehaviour
{
	public int m_UVx;

	public int m_UVy;

	public int m_width = 1;

	public int m_height = 1;

	public int m_atlasGridSubdivisions = 8;

	public int m_zOrder;

	public bool m_meshCreated;

	protected Mesh m_spriteMesh;

	public Mesh SpriteMesh => m_spriteMesh;

	private void Awake()
	{
		MeshFilter mf = GetComponent(typeof(MeshFilter)) as MeshFilter;
		CreatePlane(mf);
		MapUVToTexture(m_UVx, m_UVy, m_width, m_height);
	}

	public void MapUVToTexture(int UVx, int UVy, int width, int height)
	{
		if (!(m_spriteMesh == null))
		{
			m_UVx = UVx;
			m_UVy = UVy;
			m_width = width;
			m_height = height;
			Vector3[] vertices = m_spriteMesh.vertices;
			Vector2[] array = new Vector2[vertices.Length];
			for (int i = 0; i < array.Length; i++)
			{
				float num = 0.5f * (1f / (float)m_atlasGridSubdivisions / (float)(1024 / m_atlasGridSubdivisions)) * vertices[i].x;
				float num2 = 0.5f * (1f / (float)m_atlasGridSubdivisions / (float)(1024 / m_atlasGridSubdivisions)) * vertices[i].y;
				float x = Mathf.Clamp(vertices[i].x, 0f, 1f) * (1f / (float)m_atlasGridSubdivisions) * (float)m_width + (float)m_UVx * (1f / (float)m_atlasGridSubdivisions) - num;
				float y = Mathf.Clamp(vertices[i].y, 0f, 1f) * (1f / (float)m_atlasGridSubdivisions) * (float)m_height + (float)m_UVy * (1f / (float)m_atlasGridSubdivisions) - num2;
				array[i] = new Vector2(x, y);
			}
			m_spriteMesh.uv = array;
		}
	}

	public void SetUVs(Vector2[] newUVs)
	{
		m_spriteMesh.uv = newUVs;
	}

	public void CreatePlane(MeshFilter mf)
	{
		if (!m_meshCreated)
		{
			Mesh mesh = new Mesh();
			mesh.name = "PreviewMesh_1x1";
			Vector3[] array = new Vector3[4];
			int[] triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
			array[0] = new Vector3(-1f, -1f, 0f);
			array[1] = new Vector3(-1f, 1f, 0f);
			array[2] = new Vector3(1f, 1f, 0f);
			array[3] = new Vector3(1f, -1f, 0f);
			mesh.vertices = array;
			mesh.triangles = triangles;
			mf.sharedMesh = mesh;
			m_spriteMesh = mf.sharedMesh;
			mf.sharedMesh.RecalculateNormals();
			mf.sharedMesh.RecalculateBounds();
			m_meshCreated = true;
		}
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			MeshFilter component = base.transform.GetComponent<MeshFilter>();
			if ((bool)component)
			{
				CreatePlane(component);
				MapUVToTexture(m_UVx, m_UVy, m_width, m_height);
			}
		}
	}
}
