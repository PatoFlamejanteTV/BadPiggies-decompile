using UnityEngine;

public class GoldenPig : BasePart
{
	private AudioSource loopingRollingSound;

	private float targetVolume;

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override void Initialize()
	{
		base.rigidbody.drag = 0.5f;
		base.rigidbody.angularDrag = 1f;
		if (!loopingRollingSound)
		{
			loopingRollingSound = Singleton<AudioManager>.Instance.SpawnLoopingEffect(WPFMonoBehaviour.gameData.commonAudioCollection.goldenPigRoll, base.transform).GetComponent<AudioSource>();
		}
		loopingRollingSound.volume = 0f;
		loopingRollingSound.Play();
	}

	private void Update()
	{
		UpdateSoundEffect();
	}

	protected override void UpdateSoundEffect()
	{
		if ((bool)loopingRollingSound)
		{
			targetVolume = ((!m_isOnGround) ? 0f : (Mathf.Clamp01(base.rigidbody.velocity.magnitude) / 1f));
			loopingRollingSound.volume = Mathf.Lerp(loopingRollingSound.volume, targetVolume, Time.deltaTime * 5f);
		}
	}
}
