using System;
using UnityEngine;

public class InGameMechanicGift : WPFMonoBehaviour
{
	private UnmanagedSprite m_sprite;

	private Texture2D m_texture;

	private System.Random m_random;

	public GameObject superGlueButton;

	public GameObject turboChargeButton;

	public GameObject superMagnetButton;

	private void Awake()
	{
		if (Singleton<Localizer>.Instance.CurrentLocale == "ja-JP")
		{
			base.transform.FindChildRecursively("FreeGiftFrom").GetComponent<TextMesh>().characterSize = 0.8f;
		}
		m_random = new System.Random();
	}

	private void SetTexture(Texture2D texture)
	{
		m_texture = texture;
		m_sprite = GetComponentInChildren<UnmanagedSprite>();
		m_sprite.GetComponent<Renderer>().sharedMaterial.mainTexture = m_texture;
	}

	private void SendClose()
	{
		EventManager.Send(new UIEvent(UIEvent.Type.CloseMechanicInfo));
	}

	private void OnEnable()
	{
		Singleton<KeyListener>.Instance.GrabFocus(this);
		KeyListener.keyReleased += HandleKeyListenerkeyReleased;
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.MechanicGiftScreen)
		{
			superGlueButton.SetActive(value: false);
			turboChargeButton.SetActive(value: false);
			superMagnetButton.SetActive(value: false);
			Texture2D rewardNativeTexture = AdvertisementHandler.GetRewardNativeTexture();
			if (!WPFMonoBehaviour.levelManager.m_SuperGlueAllowed && !WPFMonoBehaviour.levelManager.m_SuperMagnetAllowed && !WPFMonoBehaviour.levelManager.m_TurboChargeAllowed)
			{
				SendClose();
			}
			else if (rewardNativeTexture != null)
			{
				GiveGift();
				SetTexture(rewardNativeTexture);
			}
			else
			{
				SendClose();
			}
		}
	}

	private void OnDestroy()
	{
		m_texture = null;
		Resources.UnloadUnusedAssets();
	}

	private void OnDisable()
	{
		KeyListener.keyReleased -= HandleKeyListenerkeyReleased;
		Singleton<KeyListener>.Instance.ReleaseFocus(this);
	}

	private void HandleKeyListenerkeyReleased(KeyCode obj)
	{
		if (obj == KeyCode.Escape)
		{
			SendClose();
		}
	}

	private void GiveGift()
	{
		int num = m_random.Next(0, 3);
		string customTypeOfGain = "Branded reward";
		switch (num)
		{
		default:
			if (!WPFMonoBehaviour.levelManager.m_SuperGlueAllowed)
			{
				GiveGift();
				break;
			}
			superGlueButton.SetActive(value: true);
			GameProgress.AddSuperGlue(1);
			if (Singleton<IapManager>.Instance != null)
			{
				Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.SuperGlueSingle, 1, customTypeOfGain);
			}
			break;
		case 1:
			if (!WPFMonoBehaviour.levelManager.m_TurboChargeAllowed)
			{
				GiveGift();
				break;
			}
			turboChargeButton.SetActive(value: true);
			GameProgress.AddTurboCharge(1);
			if (Singleton<IapManager>.Instance != null)
			{
				Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.TurboChargeSingle, 1, customTypeOfGain);
			}
			break;
		case 2:
			if (!WPFMonoBehaviour.levelManager.m_SuperMagnetAllowed)
			{
				GiveGift();
				break;
			}
			superMagnetButton.SetActive(value: true);
			GameProgress.AddSuperMagnet(1);
			if (Singleton<IapManager>.Instance != null)
			{
				Singleton<IapManager>.Instance.SendFlurryInventoryGainEvent(IapManager.InAppPurchaseItemType.SuperMagnetSingle, 1, customTypeOfGain);
			}
			break;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		m_sprite.ResetSize();
		Vector3 zero = Vector3.zero;
		float num = 20f * WPFMonoBehaviour.hudCamera.aspect;
		float num2 = 20f;
		zero.x = 11.5f / num * (float)Screen.width;
		zero.y = 6.75f / num2 * (float)Screen.height;
		float num3 = Mathf.Max(zero.x / (float)m_texture.width, zero.y / (float)m_texture.height);
		num3 *= 768f / (float)Screen.height;
		m_sprite.transform.localScale = new Vector3(num3, num3, 1f);
	}
}
