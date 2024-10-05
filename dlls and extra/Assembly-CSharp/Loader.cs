using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : Singleton<Loader>
{
	public static bool isLoadingLevel;

	private Vector3 originalPosition = Vector3.zero;

	private string m_lastLoadedLevel = string.Empty;

	public string LastLoadedString => m_lastLoadedLevel;

	public void LoadLevel(string levelName, GameManager.GameState nextState, bool showLoadingScreen, bool enableGUIAfterLoad = true)
	{
		isLoadingLevel = true;
		m_lastLoadedLevel = levelName;
		if (showLoadingScreen)
		{
			Show();
		}
		else
		{
			base.gameObject.SetActive(value: true);
		}
		GameProgress.Save();
		CoroutineRunner.Instance.StartCoroutine(LoadLevelAsync(levelName, nextState, enableGUIAfterLoad));
	}

	private IEnumerator LoadLevelAsync(string levelName, GameManager.GameState nextState, bool enableGUIAfterLoad = true)
	{
		Singleton<GuiManager>.Instance.IsEnabled = false;
		yield return null;
		if (!levelName.Equals("DailyChallenge") && !levelName.Equals("CakeRaceIntro") && (levelName.Equals("LevelStub") || nextState == GameManager.GameState.Cutscene || nextState == GameManager.GameState.StarLevelCutscene))
		{
			LevelLoader levelLoader = Singleton<GameManager>.instance.CurrentLevelLoader();
			string bundleId = ((!(levelLoader != null)) ? string.Empty : levelLoader.AssetBundleName);
			if (!string.IsNullOrEmpty(bundleId) && Bundle.HasBundle(bundleId))
			{
				Bundle.LoadBundleAsync(bundleId);
				while (!Bundle.IsBundleLoaded(bundleId))
				{
					yield return null;
				}
			}
		}
		CoroutineRunner.Instance.StartCoroutine(DelayLoadLevelEvent(Singleton<GameManager>.Instance.GetGameState(), nextState, levelName));
		Singleton<GameManager>.Instance.SetLoadingLevelGameState(nextState);
		yield return SceneManager.LoadSceneAsync(levelName);
		if (Singleton<GameManager>.Instance.GetGameState() == GameManager.GameState.LevelSelection || Singleton<GameManager>.Instance.GetGameState() == GameManager.GameState.SandboxLevelSelection || Singleton<GameManager>.Instance.GetGameState() == GameManager.GameState.EpisodeSelection)
		{
			GameTime.Pause(pause: false);
		}
		Singleton<GuiManager>.Instance.IsEnabled = enableGUIAfterLoad;
		isLoadingLevel = false;
	}

	private void Awake()
	{
		SetAsPersistant();
		originalPosition = base.transform.position;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void Start()
	{
		Hide();
	}

	private void Show()
	{
		RepositionToNearplane();
		base.gameObject.SetActive(value: true);
	}

	private void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	private void RepositionToNearplane()
	{
		Camera hudCamera = WPFMonoBehaviour.hudCamera;
		if ((bool)hudCamera)
		{
			float z = hudCamera.transform.position.z + hudCamera.nearClipPlane * 2f;
			base.transform.position = new Vector3(originalPosition.x, originalPosition.y - hudCamera.transform.InverseTransformPoint(0f, 0f, 0f).y, z);
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		Singleton<GuiManager>.Instance.IsEnabled = true;
		Hide();
		RepositionToNearplane();
		DisableScreenSleep(Singleton<GameManager>.Instance.IsInGame());
		EventManager.SendOnNextUpdate(CoroutineRunner.Instance, new LevelLoadedEvent(Singleton<GameManager>.Instance.GetGameState()));
	}

	private IEnumerator DelayLoadLevelEvent(GameManager.GameState currentState, GameManager.GameState nextState, string levelName)
	{
		yield return new WaitForEndOfFrame();
		EventManager.Send(new LoadLevelEvent(currentState, nextState, levelName));
	}

	private void DisableScreenSleep(bool disable)
	{
		Screen.sleepTimeout = ((!disable) ? (-2) : (-1));
	}
}
