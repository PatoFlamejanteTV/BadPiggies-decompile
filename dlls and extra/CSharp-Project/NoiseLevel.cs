using UnityEngine;

public class NoiseLevel : MonoBehaviour
{
	[SerializeField]
	private float noiseLevel;

	private float baseVolume;

	public float Level => noiseLevel * GetComponent<AudioSource>().volume / baseVolume;

	private void Awake()
	{
		baseVolume = GetComponent<AudioSource>().volume;
	}
}
