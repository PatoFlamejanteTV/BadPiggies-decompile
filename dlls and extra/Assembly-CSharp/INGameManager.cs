using UnityEngine;

public class INGameManager : MonoBehaviour
{
	private Vector2Int m_resolution;

	public INGameManager Instance { get; private set; }

	public void SetFullScreen()
	{
		if (Screen.fullScreen)
		{
			Screen.SetResolution(m_resolution.x, m_resolution.y, fullscreen: false);
			return;
		}
		Resolution currentResolution = Screen.currentResolution;
		m_resolution = new Vector2Int(Screen.width, Screen.height);
		Screen.SetResolution(currentResolution.width, currentResolution.height, fullscreen: true);
	}

	private void Awake()
	{
		Instance = this;
		Object.DontDestroyOnLoad(this);
		Resolution currentResolution = Screen.currentResolution;
		m_resolution = new Vector2Int(currentResolution.width, currentResolution.height);
	}

	private void Update()
	{
		if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.F))
		{
			SetFullScreen();
		}
	}
}
