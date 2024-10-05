using System;
using UnityEngine;

[ExecuteInEditMode]
public class AnimateColor : MonoBehaviour
{
	[SerializeField]
	private Material originalMaterial;

	public string colorName = "_Color";

	public Color color = Color.white;

	private MeshRenderer m_renderer;

	[NonSerialized]
	private Material materialInstance;

	private void Awake()
	{
		Reset();
	}

	private void Reset()
	{
		m_renderer = GetComponent<MeshRenderer>();
		if (m_renderer != null && originalMaterial != null)
		{
			materialInstance = new Material(originalMaterial);
			AtlasMaterials.Instance.AddMaterialInstance(materialInstance);
			m_renderer.material = materialInstance;
		}
	}

	private void OnDestroy()
	{
		if (AtlasMaterials.IsInstantiated)
		{
			AtlasMaterials.Instance.RemoveMaterialInstance(materialInstance);
		}
	}

	private void Update()
	{
		if (!(materialInstance == null))
		{
			materialInstance.SetColor(colorName, color);
		}
	}
}
