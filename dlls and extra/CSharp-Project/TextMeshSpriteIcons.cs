using System.Collections.Generic;
using UnityEngine;

public class TextMeshSpriteIcons : MonoBehaviour
{
	private TextMesh textMesh;

	private string originalText;

	private bool initialized;

	private void Awake()
	{
		Initialize();
	}

	private void Initialize()
	{
		if (!initialized)
		{
			textMesh = GetComponent<TextMesh>();
			originalText = ((!(textMesh == null)) ? textMesh.text : null);
			initialized = true;
		}
	}

	private void TextUpdated()
	{
		UpdateIcons();
	}

	public void UpdateIcons()
	{
		Initialize();
		if (textMesh == null || string.IsNullOrEmpty(originalText) || string.IsNullOrEmpty(originalText = textMesh.text) || !ContainsTag())
		{
			return;
		}
		Renderer component = textMesh.GetComponent<Renderer>();
		textMesh.text = string.Empty;
		bool flag = component.enabled;
		component.enabled = false;
		string text = originalText;
		RuntimeSpriteDatabase instance = Singleton<RuntimeSpriteDatabase>.Instance;
		List<Material> list = new List<Material>();
		foreach (string tag in SpriteIconData.Instance.GetTags())
		{
			if (!originalText.Contains(tag))
			{
				continue;
			}
			SpriteIconData.SpriteIcon spriteIcon = SpriteIconData.Instance.GetSpriteIcon(tag);
			SpriteData spriteData = instance.Find(spriteIcon.spriteId);
			Material atlasReference = spriteIcon.atlasReference;
			if (spriteData == null || !(atlasReference != null))
			{
				continue;
			}
			int num = -1;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == atlasReference)
				{
					num = i + 1;
				}
			}
			if (num < 0)
			{
				list.Add(atlasReference);
				num = list.Count;
			}
			int num2 = Mathf.FloorToInt(spriteIcon.scale * (float)textMesh.font.fontSize);
			spriteIcon.quad = string.Format("<quad material={5} size={0} x={1:0.000} y={2:0.000} width={3:0.000} height={4:0.000} />", num2, spriteData.uv.x, spriteData.uv.y, spriteData.uv.width, spriteData.uv.height, num);
			text = text.Replace(spriteIcon.tag, spriteIcon.quad);
		}
		if (list.Count > 0)
		{
			Material[] array = new Material[list.Count + 1];
			array[0] = component.sharedMaterials[0];
			int num3 = 1;
			foreach (Material item in list)
			{
				array[num3] = item;
				num3++;
			}
			component.sharedMaterials = array;
		}
		component.enabled = flag;
		text = " " + text + " ";
		text = text.Replace("  ", " ");
		textMesh.text = text;
	}

	private bool ContainsTag()
	{
		if (string.IsNullOrEmpty(originalText))
		{
			return false;
		}
		foreach (string tag in SpriteIconData.Instance.GetTags())
		{
			if (originalText.Contains(tag))
			{
				return true;
			}
		}
		return false;
	}

	public static void EnsureSpriteIcon(TextMesh[] target)
	{
		if (target != null && target.Length != 0)
		{
			for (int i = 0; i < target.Length; i++)
			{
				EnsureSpriteIcon(target[i]);
			}
		}
	}

	public static void EnsureSpriteIcon(TextMesh target)
	{
		if (!(target == null))
		{
			TextMeshSpriteIcons textMeshSpriteIcons = target.GetComponent<TextMeshSpriteIcons>();
			if (textMeshSpriteIcons == null)
			{
				textMeshSpriteIcons = target.gameObject.AddComponent<TextMeshSpriteIcons>();
			}
			textMeshSpriteIcons.UpdateIcons();
		}
	}
}
