using UnityEngine;

[ExecuteInEditMode]
public class RenderQueue : WPFMonoBehaviour
{
	public int renderQueue;

	public bool setNow;

	private void Awake()
	{
		if ((bool)base.renderer)
		{
			renderQueue = base.renderer.sharedMaterial.renderQueue;
		}
	}

	private void Update()
	{
		if ((bool)base.renderer && setNow)
		{
			base.renderer.sharedMaterial.renderQueue = renderQueue;
			setNow = false;
		}
	}
}
