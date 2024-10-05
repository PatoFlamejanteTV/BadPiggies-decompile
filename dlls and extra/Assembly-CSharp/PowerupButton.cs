using UnityEngine;

public class PowerupButton : MonoBehaviour
{
	[SerializeField]
	private GameObject disabledIcon;

	[SerializeField]
	private SpriteAnimation spriteAnimation;

	private TextMesh[] textMeshes;

	private bool initialized;

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		if (!initialized)
		{
			textMeshes = GetComponentsInChildren<TextMesh>();
			initialized = true;
		}
	}

	public void SetText(string text)
	{
		Initialize();
		for (int i = 0; i < textMeshes.Length; i++)
		{
			textMeshes[i].text = text;
		}
	}

	public void SetAvailable(bool available)
	{
		disabledIcon.SetActive(!available);
	}

	public void SetUsedState(bool used)
	{
		spriteAnimation.Play((!used) ? "Default" : "Used");
	}
}
