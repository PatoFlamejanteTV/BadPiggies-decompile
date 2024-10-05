using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class SpriteText : MonoBehaviour
{
	public enum VerticalAlignment
	{
		Bottom,
		Center,
		Top
	}

	public enum HorizontalAlignment
	{
		Left,
		Center,
		Right
	}

	private const float CameraScale = 5f / 192f;

	public SpriteFont m_font;

	[SerializeField]
	private string text;

	public VerticalAlignment vAlign;

	public HorizontalAlignment hAlign;

	public VerticalAlignment letterVAlign;

	public float letterInterval;

	private RuntimeSpriteDatabase m_spriteDatabase;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			if (value != text)
			{
				text = value;
				BuildMesh();
			}
		}
	}

	private void Awake()
	{
		m_font.Initialize(Singleton<RuntimeSpriteDatabase>.Instance);
		GetComponent<Renderer>().sharedMaterial = m_font.GetComponent<Renderer>().sharedMaterial;
		BuildMesh();
	}

	private Vector2 GetSize()
	{
		int length = text.Length;
		float num = 0f;
		float num2 = 0f;
		float num3 = letterInterval * (5f / 192f);
		for (int i = 0; i < length; i++)
		{
			char c = text[i];
			SpriteFont.SpriteSymbol symbol = m_font.GetSymbol(c);
			SpriteData spriteData = symbol.spriteData;
			float num4 = symbol.spriteScaleX * (float)spriteData.width * (5f / 192f);
			float num5 = symbol.spriteScaleY * (float)spriteData.height * (5f / 192f);
			num2 += num4 + num3;
			if (num5 > num)
			{
				num = num5;
			}
		}
		return new Vector2(num2, num);
	}

	public void BuildMesh()
	{
		int length = text.Length;
		Vector2 size = GetSize();
		Vector3[] array = new Vector3[4 * length];
		Vector2[] array2 = new Vector2[4 * length];
		int[] array3 = new int[6 * length];
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		int num4 = 0;
		float num5 = ((hAlign == HorizontalAlignment.Center) ? ((0f - size.x) / 2f) : ((hAlign != HorizontalAlignment.Right) ? 0f : (0f - size.x)));
		float num6 = ((vAlign == VerticalAlignment.Center) ? ((0f - size.y) / 2f) : ((vAlign != VerticalAlignment.Top) ? 0f : (0f - size.y)));
		float num7 = letterInterval * (5f / 192f);
		for (int i = 0; i < length; i++)
		{
			char c = text[i];
			SpriteFont.SpriteSymbol symbol = m_font.GetSymbol(c);
			SpriteData spriteData = symbol.spriteData;
			float num8 = symbol.spriteScaleX * (float)spriteData.width * (5f / 192f);
			float num9 = symbol.spriteScaleY * (float)spriteData.height * (5f / 192f);
			float num10 = num6 + ((letterVAlign == VerticalAlignment.Center) ? ((size.y - num9) / 2f) : ((letterVAlign != VerticalAlignment.Top) ? 0f : (size.y - num9)));
			array[num3] = new Vector3(num + num5, num2 + num10, 0f);
			array[num3 + 1] = new Vector3(num + num5, num2 + num10 + num9, 0f);
			array[num3 + 2] = new Vector3(num + num5 + num8, num2 + num10 + num9, 0f);
			array[num3 + 3] = new Vector3(num + num5 + num8, num2 + num10, 0f);
			array3[num4] = num3;
			array3[num4 + 1] = num3 + 1;
			array3[num4 + 2] = num3 + 2;
			array3[num4 + 3] = num3 + 2;
			array3[num4 + 4] = num3 + 3;
			array3[num4 + 5] = num3;
			float num11 = 0f;
			float num12 = 0f;
			if (spriteData.opaqueBorderPixels > 0)
			{
				num11 = (float)spriteData.opaqueBorderPixels * spriteData.uv.width / (float)spriteData.width;
				num12 = (float)spriteData.opaqueBorderPixels * spriteData.uv.height / (float)spriteData.height;
			}
			array2[num3] = new Vector2(spriteData.uv.x + num11, spriteData.uv.y + num12);
			array2[num3 + 1] = new Vector2(spriteData.uv.x + num11, spriteData.uv.y + spriteData.uv.height - 1f * num12);
			array2[num3 + 2] = new Vector2(spriteData.uv.x + spriteData.uv.width - 1f * num11, spriteData.uv.y + spriteData.uv.height - 1f * num12);
			array2[num3 + 3] = new Vector2(spriteData.uv.x + spriteData.uv.width - 1f * num11, spriteData.uv.y + num12);
			num3 += 4;
			num4 += 6;
			num += num8 + num7;
		}
		MeshFilter meshFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
		if (!(meshFilter.sharedMesh != null) || !meshFilter.sharedMesh.vertices.SequenceEqual(array) || !meshFilter.sharedMesh.triangles.SequenceEqual(array3) || !meshFilter.sharedMesh.uv.SequenceEqual(array2))
		{
			meshFilter.sharedMesh = new Mesh
			{
				vertices = array,
				triangles = array3,
				uv = array2
			};
			meshFilter.sharedMesh.RecalculateNormals();
			meshFilter.sharedMesh.RecalculateBounds();
		}
	}

	private void OnDrawGizmos()
	{
		RefreshEditorView();
	}

	public void RefreshEditorView()
	{
		if (!Application.isPlaying)
		{
			if (m_spriteDatabase == null)
			{
				m_spriteDatabase = Singleton<RuntimeSpriteDatabase>.Instance;
			}
			m_font.Initialize(m_spriteDatabase);
			GetComponent<Renderer>().sharedMaterial = m_font.GetComponent<Renderer>().sharedMaterial;
			BuildMesh();
		}
	}
}
