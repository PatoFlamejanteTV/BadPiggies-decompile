using UnityEngine;

[ExecuteInEditMode]
public class RenderQueueTool : MonoBehaviour
{
	public int renderQueue;

	public int newValue = -1;

	public bool setNewValue;

	private Renderer mr;

	private void Awake()
	{
		mr = GetComponent<Renderer>();
	}

	private void Update()
	{
		if (mr == null)
		{
			Awake();
			return;
		}
		renderQueue = mr.sharedMaterial.renderQueue;
		if (setNewValue)
		{
			mr.sharedMaterial.renderQueue = newValue;
			setNewValue = false;
		}
	}
}
