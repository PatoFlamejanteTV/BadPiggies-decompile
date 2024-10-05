using System.Collections;

public class TextureLoaderManager : Singleton<TextureLoaderManager>
{
	private const string TEXTURE_LOADER_PREFAB = "Prefabs/TextureLoader";

	public BundleSelector m_textureBundle;

	private void Start()
	{
		SetAsPersistant();
	}

	private IEnumerator Unloader()
	{
		yield return null;
	}
}
