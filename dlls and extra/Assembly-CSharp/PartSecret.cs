using UnityEngine;

public class PartSecret : MonoBehaviour
{
	public Camera gameCamera;

	public string key = "Secret";

	public int secretTapCount = 3;

	public BasePart partToUnlock;

	public Animation onTapAnimation;

	public ParticleSystem collectEffect;

	private void Awake()
	{
		if (GameProgress.GetBool(key))
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (gameCamera == null)
		{
			gameCamera = WPFMonoBehaviour.mainCamera;
		}
	}

	private void Update()
	{
		if (gameCamera == null)
		{
			return;
		}
		Vector3 pos = Vector3.zero;
		bool flag = false;
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began)
			{
				pos = touch.position;
				flag = true;
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			pos = Input.mousePosition;
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		Ray ray = gameCamera.ScreenPointToRay(pos);
		int layerMask = 1 << LayerMask.NameToLayer("Default");
		if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, layerMask) && hitInfo.transform == base.transform)
		{
			if ((bool)onTapAnimation)
			{
				onTapAnimation.Play();
			}
			secretTapCount--;
			if (secretTapCount == 0)
			{
				SetSecret();
			}
		}
	}

	public void SetSecret()
	{
		GameProgress.SetBool(key, value: true);
		Debug.Log(key + " set");
		Collect();
	}

	private void Collect()
	{
		if (partToUnlock != null)
		{
			CustomizationManager.UnlockPart(partToUnlock, "secret");
		}
		if (collectEffect != null)
		{
			collectEffect.Emit(5);
		}
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
	}
}
