using UnityEngine;

public class UnscaledParticles : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particles;

	private void Awake()
	{
		if (particles == null)
		{
			particles = GetComponent<ParticleSystem>();
		}
	}

	private void Update()
	{
		if (Time.timeScale != 1f)
		{
			particles.Simulate(Time.unscaledDeltaTime, withChildren: true, restart: false);
		}
	}
}
