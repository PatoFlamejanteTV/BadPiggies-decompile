using UnityEngine;

[ExecuteInEditMode]
public class TerrainMaterialOverride : MonoBehaviour
{
	[SerializeField]
	private Material fillMaterial;

	[SerializeField]
	private Material curveMaterial;

	private MeshRenderer fillRenderer;

	private MeshRenderer curveRenderer;

	private void Awake()
	{
		Transform transform = base.transform.Find("_fill");
		Transform transform2 = base.transform.Find("_curve");
		if (transform != null)
		{
			fillRenderer = transform.GetComponent<MeshRenderer>();
		}
		if (transform2 != null)
		{
			curveRenderer = transform2.GetComponent<MeshRenderer>();
		}
		if (fillRenderer != null)
		{
			fillRenderer.enabled = false;
		}
		if (curveRenderer != null)
		{
			curveRenderer.enabled = false;
		}
	}

	private void Start()
	{
		if (fillRenderer != null && fillMaterial != null)
		{
			fillRenderer.sharedMaterial = fillMaterial;
			fillRenderer.enabled = true;
		}
		if (curveRenderer != null && curveMaterial != null)
		{
			curveRenderer.sharedMaterial = curveMaterial;
			curveRenderer.enabled = true;
		}
	}
}
