using System.Collections;
using UnityEngine;

public class KingPig : BasePart
{
	public ParticleSystem collisionStars;

	public ParticleSystem collisionSweat;

	public ParticleSystem sweatLoop;

	public ParticleSystem starsLoop;

	public float speedFearThreshold;

	public float fallFearThreshold;

	public bool followMouse;

	private float m_starsTimer;

	private float m_sweatTimer;

	private FaceRotation m_faceRotation;

	private Transform m_pupilMover;

	private float m_phase;

	public AudioSource[] FearAudio;

	public AudioSource[] HitAudio;

	[HideInInspector]
	public Pig.Expressions m_currentExpression;

	public float m_expressionSetTime;

	private SpriteAnimation m_faceAnimation;

	private float m_blinkTimer;

	private bool m_isPlayingAnimation;

	private AudioSource m_currentSound;

	private bool m_faceZFlattened;

	public override void Awake()
	{
		base.Awake();
		m_currentExpression = Pig.Expressions.Normal;
		m_faceRotation = base.transform.Find("KingPig").Find("Face").GetComponent<FaceRotation>();
		m_pupilMover = m_faceRotation.transform.Find("PupilMover");
		m_faceAnimation = m_faceRotation.transform.Find("Face").GetComponent<SpriteAnimation>();
	}

	private void OnEnable()
	{
		EventManager.Connect<ObjectiveAchieved>(ReceiveObjectiveAchieved);
	}

	private void OnDisable()
	{
		EventManager.Disconnect<ObjectiveAchieved>(ReceiveObjectiveAchieved);
	}

	private void ReceiveObjectiveAchieved(ObjectiveAchieved data)
	{
		StartCoroutine(PlayAnimation(Pig.Expressions.Laugh, 2.5f));
	}

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override void Initialize()
	{
		base.rigidbody.drag = 0.5f;
		base.rigidbody.angularDrag = 1f;
	}

	public override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		if (collision.relativeVelocity.magnitude >= 6f)
		{
			StartCoroutine(PlayAnimation(Pig.Expressions.Hit, 1f));
			starsLoop.Play();
			m_starsTimer = 4f;
		}
		if (collision.relativeVelocity.magnitude > 4f)
		{
			collisionStars.Play();
		}
		if (m_currentExpression == Pig.Expressions.Fear)
		{
			collisionSweat.Play();
		}
	}

	public IEnumerator PlayAnimation(Pig.Expressions exp, float time)
	{
		m_isPlayingAnimation = true;
		SetExpression(exp);
		yield return new WaitForSeconds(time);
		m_isPlayingAnimation = false;
	}

	public IEnumerator PlayAnimation(Pig.Expressions exp)
	{
		string value = ExpressionToAnimationName(exp);
		if (!string.IsNullOrEmpty(value))
		{
			SpriteAnimation.Animation animation = m_faceAnimation.GetAnimation(value);
			float endTime = animation.frames[animation.frames.Count - 1].endTime;
			m_isPlayingAnimation = true;
			SetExpression(exp);
			yield return new WaitForSeconds(endTime);
			m_isPlayingAnimation = false;
		}
	}

	private void Update()
	{
		if ((bool)m_faceRotation)
		{
			Vector3 zero = Vector3.zero;
			if (followMouse)
			{
				Vector3 vector = ((!GameObject.FindGameObjectWithTag("MainCamera")) ? GameObject.FindGameObjectWithTag("HUDCamera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition) : GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition));
				zero = vector - base.transform.position;
				zero.z = 0f;
			}
			else
			{
				float num = Mathf.Pow(Mathf.Clamp(Mathf.PerlinNoise(0.6f * Time.time, -1.5234f), 0f, 1f), 1.5f);
				m_phase += num * Time.deltaTime;
				float num2 = Mathf.PerlinNoise(m_phase, 0.123f);
				float num3 = Mathf.PerlinNoise(m_phase, 5.9123f);
				zero = new Vector3(2f * num2 - 1f, 2f * num3 - 1f, 0f);
				zero = Mathf.Pow(zero.magnitude, 1.2f) * zero.normalized;
			}
			m_faceRotation.SetTargetDirection(zero);
		}
		if (!m_isPlayingAnimation)
		{
			m_blinkTimer -= Time.deltaTime;
			bool flag = false;
			if (m_blinkTimer <= 0f && m_currentExpression == Pig.Expressions.Normal)
			{
				m_faceAnimation.Play("Blink");
				m_pupilMover.transform.localPosition = 0.05f * Random.insideUnitCircle;
				m_blinkTimer = Random.Range(2.5f, 5f);
				flag = true;
			}
			if (!base.contraption || !base.contraption.IsRunning)
			{
				return;
			}
			Pig.Expressions expression = SelectExpression();
			if (!flag)
			{
				SetExpression(expression);
			}
		}
		if (starsLoop.isPlaying)
		{
			if (m_starsTimer > 0f)
			{
				float starsTimer = m_starsTimer;
				if (m_starsTimer > 2f)
				{
					starsTimer *= 2f;
				}
				m_starsTimer -= Time.deltaTime;
			}
			else
			{
				starsLoop.Stop();
			}
		}
		if (!m_faceZFlattened && !GetComponent<Joint>())
		{
			m_faceRotation.ScaleFaceZ(0.01f);
			m_faceZFlattened = true;
		}
	}

	private Pig.Expressions SelectExpression()
	{
		Vector3 velocity = base.rigidbody.velocity;
		Pig.Expressions result = Pig.Expressions.Normal;
		if (Time.time > m_expressionSetTime + 1f)
		{
			if (velocity.magnitude + 0.3f * Mathf.Abs(velocity.y) > speedFearThreshold)
			{
				result = Pig.Expressions.Fear;
			}
			if (0f - velocity.y > fallFearThreshold)
			{
				result = Pig.Expressions.Fear;
			}
		}
		else
		{
			result = m_currentExpression;
		}
		return result;
	}

	private string ExpressionToAnimationName(Pig.Expressions exp)
	{
		return exp switch
		{
			Pig.Expressions.Normal => "Normal", 
			Pig.Expressions.Laugh => "Laugh", 
			Pig.Expressions.Fear => "Fear_1", 
			Pig.Expressions.Hit => "Hit", 
			Pig.Expressions.Blink => "Blink", 
			Pig.Expressions.Chew => "Chew", 
			Pig.Expressions.WaitForFood => "WaitForFood", 
			Pig.Expressions.Burp => "Burp", 
			Pig.Expressions.Snooze => "Snooze", 
			Pig.Expressions.Panting => "Panting", 
			_ => null, 
		};
	}

	public void SetExpression(Pig.Expressions exp)
	{
		if (m_currentExpression == exp)
		{
			return;
		}
		AudioSource[] array = null;
		switch (exp)
		{
		case Pig.Expressions.Fear:
			array = FearAudio;
			break;
		case Pig.Expressions.Hit:
			array = HitAudio;
			break;
		}
		string value = ExpressionToAnimationName(exp);
		if (!string.IsNullOrEmpty(value))
		{
			m_faceAnimation.Play(value);
		}
		if (array != null && array.Length != 0)
		{
			if ((bool)m_currentSound)
			{
				m_currentSound.Stop();
			}
			m_currentSound = Singleton<AudioManager>.Instance.SpawnOneShotEffect(array, base.transform);
		}
		m_expressionSetTime = Time.time;
		m_currentExpression = exp;
	}

	private IEnumerator PlayEatingAnimationCoroutine(Pig.Expressions exp, float time, Pig.Expressions normalExpression)
	{
		m_isPlayingAnimation = true;
		SetExpression(exp);
		yield return new WaitForSeconds(time);
		m_isPlayingAnimation = false;
		SetExpression(normalExpression);
	}

	public void PlayEatingAnimation(Pig.Expressions expr, float time, Pig.Expressions normalExpression)
	{
		StartCoroutine(PlayEatingAnimationCoroutine(expr, 1f, normalExpression));
	}
}
