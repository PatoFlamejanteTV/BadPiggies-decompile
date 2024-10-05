using System.Collections;
using UnityEngine;

public class MainMenuPromoPopup : MonoBehaviour
{
	private UnmanagedSprite sprite;

	private Sprite closeSprite;

	private GameObject objectToShowOnClose;

	private float showTime;

	private Camera m_hudCamera;

	private void Awake()
	{
		Debug.LogWarning("MainMenuPromoPopup::Awake");
		sprite = GetComponentInChildren<UnmanagedSprite>();
		closeSprite = base.transform.Find("Close").gameObject.GetComponent<Sprite>();
		if (AdvertisementHandler.MainMenuPromoRenderable != null)
		{
			if (AdvertisementHandler.GetMainMenuPopupTexture() != null)
			{
				Debug.LogWarning("MainMenuPromoPopup::Awake::OnRenderableReady");
				OnRenderableReady(isReady: true);
			}
		}
		else
		{
			Debug.LogWarning("MainMenuPromoPopup::Awake::MainMenuPromoRenderable is NULL");
		}
		Dialog component = GetComponent<Dialog>();
		if (component != null)
		{
			component.onOpen += HideObjects;
			GetComponent<Dialog>().onClose += HandleClose;
		}
		EventManager.Connect<LevelLoadedEvent>(OnLevelLoaded);
		Object.DontDestroyOnLoad(this);
	}

	private void OnLevelLoaded(LevelLoadedEvent data)
	{
		GameManager.GameState gameState = Singleton<GameManager>.Instance.GetGameState();
		if (base.gameObject.activeSelf && (gameState == GameManager.GameState.Level || gameState == GameManager.GameState.Cutscene))
		{
			GetComponent<Dialog>().Close();
		}
		else if (!base.gameObject.activeSelf && AdvertisementHandler.GetMainMenuPopupTexture() != null)
		{
			OnRenderableReady(isReady: true);
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<LevelLoadedEvent>(OnLevelLoaded);
		_ = AdvertisementHandler.MainMenuPromoRenderable;
	}

	public void OnPressed()
	{
		if (Time.time - showTime > 0.5f)
		{
			base.gameObject.SetActive(value: false);
			GetComponent<Dialog>().Close();
		}
	}

	private void Update()
	{
		if (m_hudCamera == null)
		{
			GameObject gameObject = GameObject.FindWithTag("HUDCamera");
			if (gameObject != null)
			{
				m_hudCamera = gameObject.GetComponent<Camera>();
				Layout(m_hudCamera);
			}
		}
		HideObjects();
	}

	private void HideObjects()
	{
		GameManager.GameState gameState = Singleton<GameManager>.Instance.GetGameState();
		if (objectToShowOnClose == null)
		{
			string text = null;
			switch (gameState)
			{
			case GameManager.GameState.MainMenu:
				text = "MainMenuPage";
				break;
			case GameManager.GameState.EpisodeSelection:
				text = "EpisodeSelector";
				break;
			case GameManager.GameState.LevelSelection:
			case GameManager.GameState.SandboxLevelSelection:
			case GameManager.GameState.RaceLevelSelection:
				objectToShowOnClose = GameObject.Find("LevelSelector");
				if (objectToShowOnClose == null)
				{
					objectToShowOnClose = GameObject.Find("RaceLevelSelector");
					if (objectToShowOnClose == null)
					{
						objectToShowOnClose = GameObject.Find("SandboxSelector");
					}
				}
				break;
			}
			if (objectToShowOnClose == null && text != null)
			{
				objectToShowOnClose = GameObject.Find(text);
			}
		}
		if (objectToShowOnClose != null && objectToShowOnClose.activeSelf)
		{
			objectToShowOnClose.SetActive(value: false);
		}
	}

	private void HandleClose()
	{
		if ((bool)objectToShowOnClose)
		{
			objectToShowOnClose.SetActive(value: true);
		}
		Texture mainTexture = base.transform.Find("Popup").GetComponent<Renderer>().sharedMaterial.mainTexture;
		if ((bool)mainTexture)
		{
			Object.Destroy(mainTexture);
			Resources.UnloadUnusedAssets();
		}
		OnRenderableHide();
	}

	private bool IsAllowedToShow()
	{
		GameManager.GameState gameState = Singleton<GameManager>.Instance.GetGameState();
		MainMenu mainMenu = Object.FindObjectOfType(typeof(MainMenu)) as MainMenu;
		if ((gameState != GameManager.GameState.MainMenu || !(mainMenu != null) || !mainMenu.IsUserInMainMenu()) && gameState != GameManager.GameState.RaceLevelSelection && gameState != GameManager.GameState.SandboxLevelSelection)
		{
			return gameState == GameManager.GameState.LevelSelection;
		}
		return true;
	}

	private void Layout(Camera hudCamera)
	{
		Texture mainTexture = sprite.GetComponent<Renderer>().sharedMaterial.mainTexture;
		if (mainTexture != null)
		{
			sprite.ResetSize();
			float num = 1f;
			if (mainTexture.width > Screen.width || mainTexture.height > Screen.height)
			{
				num = Mathf.Min(Screen.width / mainTexture.width, Screen.height / mainTexture.height);
			}
			num *= 768f / (float)Screen.height;
			sprite.transform.localScale = new Vector3(num, num, 1f);
			Rect rect = new Rect((float)(Screen.width - mainTexture.width) / 2f, (float)(Screen.height - mainTexture.height) / 2f, mainTexture.width, mainTexture.height);
			float max = (float)Screen.width / (float)Screen.height * 10f - 1.4f;
			float max2 = 8.6f;
			Vector2 vector = new Vector2((float)mainTexture.width + rect.x, (float)mainTexture.height + rect.y);
			vector = hudCamera.ScreenToWorldPoint(vector);
			vector.x = Mathf.Clamp(vector.x, 0f, max);
			vector.y = Mathf.Clamp(vector.y, 0f, max2);
			closeSprite.transform.position = new Vector3(vector.x, vector.y, -80f);
		}
	}

	private void OnRenderableReady(bool isReady)
	{
		if (Application.isPlaying && AdvertisementHandler.MainMenuPromoRenderable != null && isReady)
		{
			Debug.LogWarning("MainMenuPromoPopup::OnRenderableReady texture set");
			sprite.GetComponent<Renderer>().sharedMaterial.mainTexture = AdvertisementHandler.GetMainMenuPopupTexture();
			if (IsAllowedToShow())
			{
				CoroutineRunner.Instance.StartCoroutine(DelayShow());
			}
		}
	}

	private IEnumerator DelayShow()
	{
		Debug.LogWarning("MainMenuPromoPopup::DelayShow(): BEFORE");
		yield return new WaitForEndOfFrame();
		Debug.LogWarning("MainMenuPromoPopup::DelayShow(): AFTER");
		OnRenderableShow();
	}

	public void OnRenderableShow()
	{
		Debug.LogWarning("OnRenderableShow: MainMenuPromoPopup");
		if (IsAllowedToShow())
		{
			Debug.LogWarning("SHOW: MainMenuPromoPopup");
			showTime = Time.time;
			GetComponent<Dialog>().Open();
			EventManager.Send(new UIEvent(UIEvent.Type.OpenedMainMenuPromo));
		}
	}

	public void OnRenderableHide()
	{
		Singleton<GameManager>.Instance.GetGameState();
		base.gameObject.SetActive(value: false);
		EventManager.Send(new UIEvent(UIEvent.Type.ClosedMainMenuPromo));
	}
}
