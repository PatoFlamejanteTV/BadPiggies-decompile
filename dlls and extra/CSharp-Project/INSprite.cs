using UnityEngine;

public class INSprite : MonoBehaviour
{
	public Rect m_rect;

	public Vector2 m_scale = new Vector2(1f, 1f);

	public int m_screenHeight = 1080;

	private void Awake()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component != null)
		{
			MeshFilter component2 = GetComponent<MeshFilter>();
			Texture mainTexture = component.sharedMaterial.mainTexture;
			component2.sharedMesh = new INSpriteData(textureSize: new Vector2Int(mainTexture.width, mainTexture.height), rect: m_rect, scale: m_scale, screenHeight: m_screenHeight).CreateMesh();
		}
	}
}
