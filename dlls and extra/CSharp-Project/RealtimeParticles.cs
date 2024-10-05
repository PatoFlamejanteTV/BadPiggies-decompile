using UnityEngine;

public class RealtimeParticles : MonoBehaviour
{
	[SerializeField]
	private float emitInterval = 1f;

	[SerializeField]
	private int emitCount;

	private ParticleSystem ps;

	private float lastEmitTime;

	private void Awake()
	{
		ps = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (!(ps == null) && Time.timeScale < 0.01f)
		{
			if (emitCount > 0 && lastEmitTime + emitInterval < Time.realtimeSinceStartup)
			{
				ps.Emit(emitCount);
				lastEmitTime = Time.realtimeSinceStartup;
			}
			if (ps.particleCount > 0)
			{
				ps.Simulate(Time.unscaledDeltaTime, withChildren: true, restart: false);
			}
		}
	}
}
