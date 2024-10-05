using UnityEngine;

public class AudioButton : MonoBehaviour
{
	private GameObject audioOffButton;

	private GameObject audioOnButton;

	private void OnEnable()
	{
		AudioManager.onAudioMuted += HandleAudioManageronAudioMuted;
		audioOffButton = base.transform.Find("AudioOffButton").gameObject;
		audioOnButton = base.transform.Find("AudioOnButton").gameObject;
		audioOnButton.SetActive(value: false);
		audioOffButton.SetActive(value: false);
		RefreshAudioButtonState();
	}

	private void OnDisable()
	{
		AudioManager.onAudioMuted -= HandleAudioManageronAudioMuted;
	}

	private void HandleAudioManageronAudioMuted(bool muted)
	{
		RefreshAudioButtonState();
	}

	private void RefreshAudioButtonState()
	{
		if (Singleton<AudioManager>.IsInstantiated() && Singleton<AudioManager>.Instance.AudioMuted)
		{
			audioOnButton.SetActive(value: false);
			audioOffButton.SetActive(value: true);
		}
		else
		{
			audioOffButton.SetActive(value: false);
			audioOnButton.SetActive(value: true);
		}
	}

	public void ToggleAudio()
	{
		Singleton<AudioManager>.Instance.ToggleMute();
		RefreshAudioButtonState();
	}
}
