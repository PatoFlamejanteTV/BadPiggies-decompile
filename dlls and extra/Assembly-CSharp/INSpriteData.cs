using UnityEngine;

public struct INSpriteData
{
	public Rect Rect;

	public Vector2 Scale;

	public int ScreenHeight;

	public Vector2Int TextureSize;

	private const float CameraSize = 10f;

	public INSpriteData(Rect rect, Vector2 scale, int screenHeight, Vector2Int textureSize)
	{
		Rect = rect;
		Scale = scale;
		ScreenHeight = screenHeight;
		TextureSize = textureSize;
	}

	public Mesh CreateMesh()
	{
		float num = Rect.width * Scale.x * 10f / (float)ScreenHeight;
		float num2 = Rect.height * Scale.y * 10f / (float)ScreenHeight;
		float num3 = Rect.x / (float)TextureSize.x;
		float num4 = Rect.y / (float)TextureSize.y;
		float num5 = Rect.width / (float)TextureSize.x;
		float num6 = Rect.height / (float)TextureSize.y;
		Mesh mesh = new Mesh();
		mesh.name = string.Concat(new string[4]
		{
			"Mesh_",
			Rect.width.ToString(),
			"x",
			Rect.height.ToString()
		});
		mesh.vertices = new Vector3[4]
		{
			new Vector3(0f - num, 0f - num2, 0f),
			new Vector3(0f - num, num2, 0f),
			new Vector3(num, num2, 0f),
			new Vector3(num, 0f - num2, 0f)
		};
		mesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
		mesh.uv = new Vector2[4]
		{
			new Vector2(num3, 1f - (num4 + num6)),
			new Vector2(num3, 1f - num4),
			new Vector2(num3 + num5, 1f - num4),
			new Vector2(num3 + num5, 1f - (num4 + num6))
		};
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		return mesh;
	}
}
