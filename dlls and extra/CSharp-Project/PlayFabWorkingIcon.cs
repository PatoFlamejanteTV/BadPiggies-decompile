using UnityEngine;

public class PlayFabWorkingIcon : MonoBehaviour
{
	[SerializeField]
	private Transform spinner;

	[SerializeField]
	private AnimationCurve spinningSpeed;

	[SerializeField]
	private float speedMultiplier = 720f;

	private bool spinning;

	private float spinTime;

	private int networkActionCount;

	private MeshRenderer[] renderers;

	private void Awake()
	{
		EventManager.Connect<PlayFabEvent>(OnPlayFabEvent);
		renderers = GetComponentsInChildren<MeshRenderer>();
		Show(show: false);
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<PlayFabEvent>(OnPlayFabEvent);
	}

	private void OnPlayFabEvent(PlayFabEvent data)
	{
		switch (data.type)
		{
		case PlayFabEvent.Type.UserDataUploadStarted:
		case PlayFabEvent.Type.UserDeltaChangeUploadStarted:
			if (networkActionCount <= 0)
			{
				Show();
			}
			networkActionCount++;
			spinning = true;
			break;
		case PlayFabEvent.Type.UserDataUploadEnded:
		case PlayFabEvent.Type.UserDeltaChangeUploadEnded:
			networkActionCount--;
			if (networkActionCount <= 0)
			{
				Show(show: false);
			}
			spinning = false;
			break;
		}
	}

	private void Show(bool show = true)
	{
		if (renderers == null || renderers.Length == 0)
		{
			return;
		}
		for (int i = 0; i < renderers.Length; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = show;
			}
		}
	}

	private void Update()
	{
		if (spinning && spinner != null)
		{
			spinTime += GameTime.RealTimeDelta;
			if (spinTime > 1f)
			{
				spinTime -= 1f;
			}
			spinner.Rotate(Vector3.back, GameTime.RealTimeDelta * spinningSpeed.Evaluate(spinTime) * speedMultiplier, Space.Self);
		}
	}
}
