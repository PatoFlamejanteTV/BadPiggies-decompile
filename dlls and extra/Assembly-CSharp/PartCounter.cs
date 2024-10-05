using UnityEngine;

public class PartCounter : MonoBehaviour
{
	public BasePart.PartType m_partType;

	private TextMesh m_textMesh;

	private void Awake()
	{
		m_textMesh = GetComponent<TextMesh>();
		if (((Singleton<BuildCustomizationLoader>.Instance.IsHDVersion && Singleton<BuildCustomizationLoader>.Instance.CustomerID.Equals("amazon")) || (Singleton<BuildCustomizationLoader>.Instance.IsHDVersion && Singleton<BuildCustomizationLoader>.Instance.CustomerID.Equals("nook"))) && Screen.width > 1024)
		{
			m_textMesh.GetComponent<Renderer>().material.shader = Shader.Find("_Custom/Unlit_Alpha8Bit_Color");
		}
		EventManager.Connect<PartCountChanged>(ReceivePartCountChangedEvent);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<PartCountChanged>(ReceivePartCountChangedEvent);
	}

	private void ReceivePartCountChangedEvent(PartCountChanged data)
	{
		if (data.partType != m_partType)
		{
			return;
		}
		m_textMesh.text = data.count.ToString();
		GameObject icon = base.transform.parent.GetComponent<DraggableButton>().Icon;
		if (!icon)
		{
			return;
		}
		Renderer[] componentsInChildren = icon.GetComponentsInChildren<Renderer>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].sharedMaterial = GetAtlasMaterial(componentsInChildren[i].sharedMaterial, data.count);
			}
		}
	}

	private Material GetAtlasMaterial(Material currentAtlasMaterial, int index)
	{
		if (INSettings.GetBool(INFeature.PartCounter))
		{
			return currentAtlasMaterial;
		}
		int num = 0;
		if (currentAtlasMaterial != null)
		{
			if (currentAtlasMaterial.name.StartsWith("IngameAtlas2"))
			{
				num = 1;
			}
			else if (currentAtlasMaterial.name.StartsWith("IngameAtlas3"))
			{
				num = 2;
			}
		}
		if (index == 0)
		{
			if (num == 0)
			{
				return base.transform.parent.GetComponent<Renderer>().sharedMaterials[1];
			}
			return AtlasMaterials.Instance.GrayMaterials[num];
		}
		if (num == 0)
		{
			return base.transform.parent.GetComponent<Renderer>().sharedMaterials[0];
		}
		return AtlasMaterials.Instance.RenderQueueMaterials[num];
	}
}
