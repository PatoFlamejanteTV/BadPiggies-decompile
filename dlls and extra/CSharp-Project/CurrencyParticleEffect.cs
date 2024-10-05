using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyParticleEffect : MonoBehaviour
{
	private class CurrencyParticle
	{
		public Transform tf;

		public Vector3 velocity;

		public float totalLifeTime;

		public CurrencyParticle(Transform _tf, Vector3 _velocity)
		{
			tf = _tf;
			velocity = _velocity;
			totalLifeTime = 0f;
		}

		public void Update()
		{
			if ((bool)tf)
			{
				tf.position += velocity * GameTime.RealTimeDelta;
			}
			totalLifeTime += GameTime.RealTimeDelta;
		}

		public void Destroy()
		{
			Object.Destroy(tf.gameObject);
		}
	}

	[SerializeField]
	private GameObject particlePrefab;

	[SerializeField]
	private Transform moveTarget;

	[SerializeField]
	private ParticleSystem collectParticleEffect;

	[SerializeField]
	private AnimationCurve velocityCurve;

	private List<CurrencyParticle> currencyParticles;

	private ICurrencyParticleEffectTarget targetCurrencyButton;

	private Vector3 originalMoveTargetSize = Vector3.one;

	private List<AudioSource> hitAudioSources;

	private List<AudioSource> flyAudioSources;

	private float hitSoundCooldown = 0.2f;

	private float lastPlayedHitSound;

	private float flySoundCooldown = 0.2f;

	private float lastPlayedFlySound;

	private void Awake()
	{
		hitAudioSources = new List<AudioSource>();
		flyAudioSources = new List<AudioSource>();
		originalMoveTargetSize = moveTarget.localScale;
	}

	private void OnDisable()
	{
		if (currencyParticles == null)
		{
			return;
		}
		for (int i = 0; i < currencyParticles.Count; i++)
		{
			if (currencyParticles[i].tf != null)
			{
				currencyParticles[i].Destroy();
			}
		}
		currencyParticles = null;
		targetCurrencyButton.UpdateAmount();
	}

	public void SetTarget(ICurrencyParticleEffectTarget softCurrencyButton)
	{
		targetCurrencyButton = softCurrencyButton;
	}

	public void AddParticle(Vector3 position, Vector3 velocity, float delay = 0f)
	{
		if (delay > 0.01f)
		{
			StartCoroutine(DelayAddParticle(position, velocity, delay));
		}
		else if (!(particlePrefab == null))
		{
			if (currencyParticles == null)
			{
				currencyParticles = new List<CurrencyParticle>();
			}
			position.z = moveTarget.position.z + 0.1f;
			GameObject gameObject = Object.Instantiate(particlePrefab, position, Quaternion.identity);
			currencyParticles.Add(new CurrencyParticle(gameObject.transform, velocity));
			StartCoroutine(DelayFlySound(0.1f));
		}
	}

	private IEnumerator DelayFlySound(float delay)
	{
		yield return WaitForRealSeconds(delay);
		PlayRandomParticleFlySound();
	}

	public void AddParticle(Transform target, Vector3 velocity, float delay = 0f)
	{
		if (delay > 0.01f)
		{
			StartCoroutine(DelayAddParticle(target, velocity, delay));
		}
		else
		{
			AddParticle(target.position, velocity);
		}
	}

	private IEnumerator DelayAddParticle(Vector3 position, Vector3 velocity, float delay)
	{
		yield return WaitForRealSeconds(delay);
		AddParticle(position, velocity);
	}

	private IEnumerator DelayAddParticle(Transform target, Vector3 velocity, float delay)
	{
		yield return WaitForRealSeconds(delay);
		if (target != null)
		{
			AddParticle(target.position, velocity);
		}
	}

	private void Update()
	{
		if (moveTarget == null)
		{
			return;
		}
		moveTarget.localScale = Vector3.Lerp(moveTarget.localScale, originalMoveTargetSize, GameTime.RealTimeDelta * 5f);
		if (currencyParticles == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < currencyParticles.Count; i++)
		{
			if (currencyParticles[i].tf != null)
			{
				Vector3 vector = moveTarget.position - currencyParticles[i].tf.position;
				float num2 = Vector3.Distance(currencyParticles[i].tf.position, moveTarget.position);
				float num3 = velocityCurve.Evaluate(currencyParticles[i].totalLifeTime);
				currencyParticles[i].velocity = Vector3.Lerp(currencyParticles[i].velocity, vector.normalized * num3, GameTime.RealTimeDelta * 4f);
				currencyParticles[i].Update();
				if (num2 < 1f)
				{
					PlayRandomCurrencyHitSound();
					currencyParticles[i].Destroy();
					num++;
					targetCurrencyButton.CurrencyParticleAdded();
					moveTarget.localScale = originalMoveTargetSize * 1.3f;
					if (collectParticleEffect != null)
					{
						collectParticleEffect.Emit(5);
					}
				}
			}
			else
			{
				num++;
			}
		}
		if (num == currencyParticles.Count)
		{
			currencyParticles = null;
			targetCurrencyButton.UpdateAmount();
		}
	}

	private void PlayRandomCurrencyHitSound()
	{
		if (lastPlayedHitSound + hitSoundCooldown > Time.realtimeSinceStartup)
		{
			return;
		}
		lastPlayedHitSound = Time.realtimeSinceStartup;
		int num = 0;
		if (hitAudioSources.Count > 0)
		{
			for (int num2 = hitAudioSources.Count - 1; num2 >= 0; num2--)
			{
				if (hitAudioSources[num2] == null)
				{
					hitAudioSources.RemoveAt(num2);
					num2++;
					break;
				}
				num++;
			}
		}
		if (num < 5)
		{
			hitAudioSources.Add(PlayRandomSound(targetCurrencyButton.GetHitSounds()));
		}
	}

	private void PlayRandomParticleFlySound()
	{
		if (lastPlayedFlySound + flySoundCooldown > Time.realtimeSinceStartup)
		{
			return;
		}
		lastPlayedFlySound = Time.realtimeSinceStartup;
		int num = 0;
		if (flyAudioSources.Count > 0)
		{
			for (int num2 = flyAudioSources.Count - 1; num2 >= 0; num2--)
			{
				if (flyAudioSources[num2] == null)
				{
					flyAudioSources.RemoveAt(num2);
					num2++;
					break;
				}
				num++;
			}
		}
		if (num < 5)
		{
			flyAudioSources.Add(PlayRandomSound(targetCurrencyButton.GetFlySounds()));
		}
	}

	private AudioSource PlayRandomSound(AudioSource[] audioSources)
	{
		if (audioSources != null && audioSources.Length != 0)
		{
			int num = Random.Range(0, audioSources.Length);
			return Singleton<AudioManager>.Instance.Play2dEffect(audioSources[num]);
		}
		return null;
	}

	private void OnDestroy()
	{
		if (currencyParticles == null)
		{
			return;
		}
		for (int i = 0; i < currencyParticles.Count; i++)
		{
			if (currencyParticles[i] != null && currencyParticles[i].tf != null)
			{
				Object.Destroy(currencyParticles[i].tf.gameObject);
			}
		}
		currencyParticles = null;
	}

	private IEnumerator WaitForRealSeconds(float seconds)
	{
		float stopTime = Time.realtimeSinceStartup + seconds;
		while (stopTime > Time.realtimeSinceStartup)
		{
			yield return null;
		}
	}
}
