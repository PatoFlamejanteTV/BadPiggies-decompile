using UnityEngine;

public class AudioFX : MonoBehaviour
{
	public enum FXType
	{
		Preprocess,
		Continuous
	}

	[SerializeField]
	protected FXType type;

	public virtual void Awake()
	{
		if (type == FXType.Preprocess)
		{
			ProcessAudio();
		}
	}

	public virtual void Update()
	{
		if (type == FXType.Continuous)
		{
			ProcessAudio();
		}
	}

	protected virtual void ProcessAudio()
	{
		_ = GetComponent<AudioSource>() == null;
	}
}
