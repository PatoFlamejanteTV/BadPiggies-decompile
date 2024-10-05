using UnityEngine;

public class INSerializedSprite : MonoBehaviour
{
	public string m_name;

	private void Awake()
	{
		CreateMesh();
	}

	public void CreateMesh()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (component != null)
		{
			MeshFilter component2 = GetComponent<MeshFilter>();
			Texture mainTexture = component.sharedMaterial.mainTexture;
			component2.sharedMesh = Singleton<INSpriteManager>.Instance.GetAtlasData(mainTexture.name)[m_name].CreateMesh();
		}
	}
}
