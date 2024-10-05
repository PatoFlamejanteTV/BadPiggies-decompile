using UnityEngine;

public class SpriteShineEffect : MonoBehaviour
{
	[SerializeField]
	private Color shineColor = Color.white;

	[SerializeField]
	private float shineSpeed = 1f;

	[SerializeField]
	private bool unscaledDeltaTime = true;

	[SerializeField]
	private bool alwaysShine;

	[SerializeField]
	private float shineInterval = 3f;

	[SerializeField]
	private float offsetLeft;

	[SerializeField]
	private float offsetRight = 1f;

	[SerializeField]
	private float width = 0.1f;

	private Sprite[] sprites;

	private Material[] shineMaterials;

	private Material[] originalMaterials;

	private bool isInit;

	private bool isShining;

	private float shineFade;

	private float shineEnded;

	public void ShineOnce()
	{
		if (!isInit)
		{
			sprites = GetComponentsInChildren<Sprite>();
			if (sprites == null || sprites.Length == 0)
			{
				return;
			}
			originalMaterials = new Material[sprites.Length];
			shineMaterials = new Material[sprites.Length];
			for (int i = 0; i < sprites.Length; i++)
			{
				originalMaterials[i] = sprites[i].renderer.sharedMaterial;
				shineMaterials[i] = AtlasMaterials.Instance.GetCachedMaterialInstance(originalMaterials[i], AtlasMaterials.MaterialType.Shiny);
				if (shineMaterials[i] == null)
				{
					return;
				}
				shineMaterials[i].SetFloat("_Scale", 1f / width);
				shineMaterials[i].SetColor("_Color", shineColor);
			}
			isInit = true;
		}
		for (int j = 0; j < sprites.Length; j++)
		{
			sprites[j].renderer.sharedMaterial = shineMaterials[j];
			SetCenter(shineMaterials[j]);
		}
		shineFade = 0f;
		isShining = true;
	}

	private void SetCenter(Material mat)
	{
		if (!(mat == null))
		{
			mat.SetFloat("_Center", Mathf.Lerp(offsetLeft - width, offsetRight + width, shineFade));
		}
	}

	private void Update()
	{
		if (isShining)
		{
			shineFade += ((!unscaledDeltaTime) ? GameTime.DeltaTime : GameTime.RealTimeDelta) * shineSpeed;
			for (int i = 0; i < sprites.Length; i++)
			{
				SetCenter(shineMaterials[i]);
				if (shineFade >= 1f)
				{
					sprites[i].renderer.sharedMaterial = originalMaterials[i];
				}
			}
			if (shineFade >= 1f)
			{
				isShining = false;
				shineEnded = Time.realtimeSinceStartup;
			}
		}
		else if (alwaysShine && shineEnded + shineInterval < Time.realtimeSinceStartup)
		{
			ShineOnce();
		}
	}

	public static void AddOneTimeShine(GameObject go)
	{
		if (!(go == null))
		{
			go.AddComponent<SpriteShineEffect>().ShineOnce();
		}
	}
}
