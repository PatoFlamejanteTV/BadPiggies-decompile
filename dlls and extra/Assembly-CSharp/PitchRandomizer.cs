using UnityEngine;

public class PitchRandomizer : AudioFX
{
	public float m_pitchMin;

	public float m_pitchMax;

	protected override void ProcessAudio()
	{
		base.ProcessAudio();
		Mathf.Clamp(m_pitchMin, -3f, m_pitchMax);
		Mathf.Clamp(m_pitchMax, m_pitchMin, 3f);
		GetComponent<AudioSource>().pitch = Random.Range(m_pitchMin, m_pitchMax);
	}
}
