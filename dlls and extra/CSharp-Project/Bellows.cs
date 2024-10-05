using UnityEngine;

public class Bellows : BasePropulsion
{
	public Vector3 m_direction = Vector3.up;

	public bool m_enabled;

	[SerializeField]
	private bool m_alienBellow;

	public float m_boostForce = 10f;

	public const float BOOST_DURATION = 0.5f;

	public const float WAIT_DURATION = 0.3f;

	public const float INFLATE_DURATION = 0.3f;

	public const float ALIEN_INFLATE_DURATION = 0.15f;

	public const float COMPRESSED_SCALE = 0.3f;

	public ParticleSystem smokeEmitter;

	public Transform scaleTarget;

	protected float m_timeBoostStarted;

	protected float m_currentScale;

	private bool m_isConnected;

	private static int m_alienBellowCount;

	private float InflateDuration
	{
		get
		{
			if (m_alienBellow)
			{
				return 0.15f;
			}
			return 0.3f;
		}
	}

	public static bool HasAlienBellows => m_alienBellowCount > 0;

	public override bool CanBeEnabled()
	{
		return m_isConnected;
	}

	public override bool HasOnOffToggle()
	{
		return false;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	public override Direction EffectDirection()
	{
		return BasePart.Rotate(Direction.Right, m_gridRotation);
	}

	public override void InitializeEngine()
	{
		m_isConnected = base.contraption.ComponentPartCount(base.ConnectedComponent) > 1;
	}

	private void Start()
	{
		m_timeBoostStarted = -1000f;
		if (m_alienBellow)
		{
			m_alienBellowCount++;
		}
	}

	private void OnDestroy()
	{
		if (m_alienBellow)
		{
			m_alienBellowCount--;
		}
	}

	public void FixedUpdate()
	{
		float num = Time.time - m_timeBoostStarted;
		if (num > 0.8f + InflateDuration)
		{
			m_enabled = false;
			return;
		}
		if (num < 0.5f)
		{
			m_enabled = true;
			float num2 = 1f - num / 0.5f;
			num2 = 1f - num2 * num2;
			float num3 = num2 * m_boostForce;
			Vector3 vector = base.transform.TransformDirection(m_direction);
			Vector3 position = base.transform.position + vector * 0.5f;
			Vector3 vector2 = vector;
			base.rigidbody.AddForceAtPosition(num3 * vector2, position, ForceMode.Force);
		}
		Vector3 one = Vector3.one;
		one.y *= CompressionScale(num);
		if (scaleTarget != null)
		{
			scaleTarget.localScale = one;
		}
		else
		{
			base.transform.localScale = one;
		}
	}

	protected override void OnTouch()
	{
		if (m_isConnected && !(Time.time - m_timeBoostStarted < 0.8f + InflateDuration))
		{
			m_timeBoostStarted = Time.time;
			if (HasTag("Fart"))
			{
				Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.bellowsFarts, base.transform.position);
			}
			if (HasTag("Alien_part"))
			{
				Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.alienBellows, base.transform.position);
			}
			else
			{
				Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.bellowsPuff, base.transform.position);
			}
			smokeEmitter.Emit(Random.Range(1, 2));
		}
	}

	public static float CompressionScale(float time)
	{
		float t = 0f;
		if (time < 0.5f)
		{
			t = time / 0.5f;
		}
		else if (time < 0.8f)
		{
			t = 1f;
		}
		else if (time < 0.8f + ((!HasAlienBellows) ? 0.3f : 0.15f))
		{
			t = 1f - (time - 0.5f - 0.3f) / ((!HasAlienBellows) ? 0.3f : 0.15f);
		}
		return Mathf.Lerp(1f, 0.3f, t);
	}
}
