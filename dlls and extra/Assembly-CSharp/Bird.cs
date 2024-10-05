using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : WPFMonoBehaviour
{
	public enum BirdType
	{
		Red,
		Yellow,
		Blue
	}

	private enum State
	{
		Sleeping,
		Ready,
		JumpToSlingshot,
		Aim,
		Shoot,
		Fly,
		KnockedOut
	}

	[SerializeField]
	private BirdType m_birdType;

	[SerializeField]
	private float m_relativeSlingForce = 1f;

	[SerializeField]
	private float m_speedBoost = 1f;

	[SerializeField]
	private int m_split;

	private AnimationHandler m_animation;

	private BasePart m_target;

	private State m_state;

	private float m_timer;

	private Vector3 m_lastAim;

	private Vector3 m_startPosition;

	private Slingshot m_slingshot;

	private Vector3 m_slingshotRestPosition;

	private Vector3 m_shootDirection;

	private Vector3 m_previousTargetVelocity;

	private Vector3 m_targetAcceleration;

	private float springConstant = 300f;

	private bool m_waking;

	private float m_disturbance;

	private FaceRotation m_faceRotation;

	private float m_speed;

	private bool m_collisionsRemoved;

	private float m_previousSpeed;

	private bool m_boosted;

	private bool m_splitDone;

	private const float m_zPosition = -0.2f;

	private float m_colliderRadius = 1f;

	private float m_distanceToSlingShot;

	private List<Collider> m_ignoredCollisions = new List<Collider>();

	private bool m_contraptionHit;

	private const float m_wake_up_threshold = 2.5f;

	private bool m_flyingRight;

	private GameObject m_alarm;

	private float m_alarmTimer;

	private bool m_isCollided;

	public float ColliderRadius => m_colliderRadius;

	public BirdType GetBirdType()
	{
		return m_birdType;
	}

	public bool IsSleeping()
	{
		return m_state == State.Sleeping;
	}

	public bool IsDisturbed()
	{
		if (m_state != State.KnockedOut)
		{
			if (m_state == State.Sleeping)
			{
				return m_disturbance >= 1f;
			}
			return true;
		}
		return false;
	}

	public float WakeUpProgress()
	{
		return Mathf.Pow(m_disturbance / 2.5f, 0.75f);
	}

	public bool IsAwake()
	{
		if (m_state != 0)
		{
			return m_state != State.KnockedOut;
		}
		return false;
	}

	public bool IsCollided()
	{
		return m_isCollided;
	}

	public bool IsAttacking()
	{
		if (m_state != State.JumpToSlingshot && m_state != State.Aim && m_state != State.Shoot)
		{
			return m_state == State.Fly;
		}
		return true;
	}

	public void IgnoreCollisions(Collider targetCollider)
	{
		Physics.IgnoreCollision(base.collider, targetCollider);
		m_ignoredCollisions.Add(targetCollider);
	}

	private void OnCollisionEnter(Collision col)
	{
		bool flag = false;
		ContactPoint[] contacts = col.contacts;
		foreach (ContactPoint contactPoint in contacts)
		{
			if (contactPoint.otherCollider.tag == "Contraption")
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			m_contraptionHit = true;
			if (Singleton<SocialGameManager>.IsInstantiated() && !GameProgress.GetBool("bird_hit"))
			{
				GameProgress.SetBool("bird_hit", value: true);
				Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.CANNON_FODDER", 100.0);
			}
		}
	}

	private void Start()
	{
		if (m_state != 0)
		{
			return;
		}
		m_animation = GetComponent<AnimationHandler>();
		m_animation.Play("Sleep");
		GameObject[] array = GameObject.FindGameObjectsWithTag("Slingshot");
		m_slingshot = null;
		float num = float.MaxValue;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			float num2 = Vector3.Distance(gameObject.transform.position, base.transform.position);
			if (num2 < num)
			{
				num = num2;
				m_slingshot = gameObject.GetComponent<Slingshot>();
			}
		}
		if ((bool)m_slingshot)
		{
			m_slingshotRestPosition = m_slingshot.transform.position;
			m_slingshotRestPosition.y += 2.25f;
			m_distanceToSlingShot = num;
		}
		springConstant = m_relativeSlingForce * springConstant * base.rigidbody.mass / 4f;
		base.rigidbody.isKinematic = true;
		Vector3 position = base.transform.position;
		position.z = -0.2f;
		base.transform.position = position;
		m_faceRotation = base.transform.Find("Visualization").Find("Face").GetComponent<FaceRotation>();
		SphereCollider sphereCollider = base.collider as SphereCollider;
		if ((bool)sphereCollider)
		{
			m_colliderRadius = sphereCollider.radius;
		}
		m_alarm = base.transform.Find("Visualization/Alarm").gameObject;
		SetAlarmOn(on: false);
	}

	public void StartAfterSplit(Bird from)
	{
		SetState(State.Fly);
		base.rigidbody.isKinematic = false;
		m_animation = GetComponent<AnimationHandler>();
		m_animation.Play("Normal");
		m_splitDone = true;
		m_colliderRadius = from.ColliderRadius;
	}

	public void SetAlarmOn(bool on)
	{
		if (m_alarm == null)
		{
			m_alarm = base.transform.Find("Visualization/Alarm").gameObject;
		}
		m_alarm.SetActive(on);
		if (on)
		{
			m_alarmTimer = 0.5f;
		}
		else
		{
			m_alarmTimer = 0f;
		}
	}

	private void SetState(State state)
	{
		m_state = state;
		m_timer = 0f;
		switch (m_state)
		{
		case State.Fly:
			m_speed = base.rigidbody.velocity.magnitude;
			break;
		case State.Aim:
			m_previousTargetVelocity = m_target.rigidbody.velocity;
			break;
		case State.JumpToSlingshot:
			JumpToSlingshotStart();
			break;
		}
	}

	private void FixedUpdate()
	{
		if ((bool)WPFMonoBehaviour.levelManager && (bool)WPFMonoBehaviour.levelManager.ContraptionRunning)
		{
			if (!m_target)
			{
				m_target = WPFMonoBehaviour.levelManager.ContraptionRunning.FindPig();
			}
			switch (m_state)
			{
			case State.Sleeping:
				Sleeping();
				break;
			case State.Ready:
				Ready();
				break;
			case State.JumpToSlingshot:
				JumpToSlingshot();
				break;
			case State.Aim:
				Aim();
				break;
			case State.Shoot:
				Shoot();
				break;
			case State.Fly:
				Fly();
				break;
			}
		}
	}

	private void Sleeping()
	{
		float noiseAtPosition = GetNoiseAtPosition(base.transform.position);
		if (((!WPFMonoBehaviour.levelManager) ? 124.56f : WPFMonoBehaviour.levelManager.TimeElapsed) > 1f)
		{
			if (noiseAtPosition > 0.5f)
			{
				m_disturbance += 1.5f * noiseAtPosition * Time.deltaTime;
			}
			else if (noiseAtPosition > 0.3f)
			{
				m_disturbance += 0.5f * noiseAtPosition * Time.deltaTime;
			}
			else
			{
				m_disturbance -= 2f * Time.deltaTime;
				m_disturbance = Mathf.Max(m_disturbance, 0f);
			}
		}
		if (m_disturbance < 1f)
		{
			if (m_waking)
			{
				m_animation.Play("Sleep");
				m_disturbance *= 0.5f;
			}
			m_waking = false;
		}
		else if (m_disturbance < 2.5f)
		{
			if (!m_waking)
			{
				m_animation.Play("Waking");
				EventManager.Send(new BirdWakeUpEvent(this));
			}
			m_waking = true;
		}
		else
		{
			SetState(State.Ready);
			m_animation.Play("Blink");
			SetAlarmOn(on: true);
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.birdWakeUp);
		}
	}

	private void Ready()
	{
		Vector3 vector = m_target.transform.position - m_slingshotRestPosition;
		m_faceRotation.SetTargetDirection(vector.normalized);
		if (m_alarmTimer > 0f)
		{
			m_alarmTimer -= Time.deltaTime;
			if (m_alarmTimer <= 0f)
			{
				SetAlarmOn(on: false);
			}
		}
		if ((bool)m_slingshot && m_slingshot.IsFree())
		{
			int layerMask = 1 << LayerMask.NameToLayer("Ground");
			if (!Physics.Raycast(m_slingshotRestPosition, vector.normalized, out var _, vector.magnitude, layerMask))
			{
				m_timer += Time.deltaTime;
			}
			if (m_timer > 0.4f + 0.03f * m_distanceToSlingShot)
			{
				m_slingshot.StartShot();
				m_faceRotation.SetTargetDirection(Vector3.zero);
				SetState(State.JumpToSlingshot);
				StartCoroutine(PlayAnimation("Angry", 0.6f));
				SetAlarmOn(on: false);
			}
		}
	}

	private void JumpToSlingshotStart()
	{
		m_startPosition = base.transform.position;
	}

	private void JumpToSlingshot()
	{
		m_timer += Time.deltaTime;
		Vector3 vector = new Vector3(-29.46859f, -0.6941621f, 0f);
		Vector3 vector2 = new Vector3(-30.86662f, 2.937689f, 0f);
		AnimationState animationState = GetComponent<Animation>()["JumpToSlingshot"];
		animationState.enabled = true;
		animationState.time = m_timer;
		animationState.weight = 1f;
		GetComponent<Animation>().Sample();
		animationState.enabled = false;
		float num = (base.transform.position.x - vector.x) / (vector2.x - vector.x);
		float num2 = (base.transform.position.y - vector.y) / (vector2.y - vector.y);
		float x = m_startPosition.x + num * m_slingshotRestPosition.x - num * m_startPosition.x;
		float y = m_startPosition.y + num2 * m_slingshotRestPosition.y - num2 * m_startPosition.y;
		base.transform.position = new Vector3(x, y, -0.2f);
		if (m_timer >= animationState.length)
		{
			SetState(State.Aim);
		}
	}

	private float SimulateShot(Vector3 birdStartPosition, Vector3 targetPosition, Vector3 targetVelocity, Vector3 targetAcceleration)
	{
		birdStartPosition = 3.2f * birdStartPosition.normalized;
		Vector3 vector = birdStartPosition;
		Vector3 lhs = -birdStartPosition;
		Vector3 vector2 = Vector3.zero;
		float mass = base.rigidbody.mass;
		for (int i = 0; i < 1000; i++)
		{
			Vector3 vector3 = -vector;
			Vector3 normalized = vector3.normalized;
			float num = Vector3.Dot(lhs, normalized);
			Vector3 vector4 = springConstant * vector3;
			vector4.y += -9.81f * mass;
			vector2 += vector4 / mass * 0.02f;
			vector += vector2 * 0.02f;
			targetVelocity += targetAcceleration * 0.02f;
			targetPosition += targetVelocity * 0.02f;
			if (num <= 0f)
			{
				break;
			}
		}
		float num2 = (vector - targetPosition).sqrMagnitude;
		bool flag = false;
		while (true)
		{
			Vector3 vector5 = new Vector3(0f, -9.81f * mass, 0f);
			Vector3 vector6 = -vector2 * base.rigidbody.drag;
			vector5 += vector6;
			vector2 += vector5 / mass * 0.02f;
			vector += vector2 * 0.02f;
			targetVelocity += targetAcceleration * 0.02f;
			targetPosition += targetVelocity * 0.02f;
			float sqrMagnitude = (vector - targetPosition).sqrMagnitude;
			if (m_speedBoost > 0f && !flag && sqrMagnitude < 0.5f * targetPosition.sqrMagnitude)
			{
				flag = true;
				vector2 = m_speedBoost * vector2;
			}
			if (sqrMagnitude > num2)
			{
				break;
			}
			num2 = sqrMagnitude;
		}
		return Mathf.Sqrt(num2);
	}

	private Vector3 FindShot(Vector3 relativeTargetPosition, Vector3 targetVelocity, Vector3 targetAcceleration)
	{
		Vector3 zero = Vector3.zero;
		Vector3 vector = -relativeTargetPosition.normalized;
		float num = Mathf.Atan2(vector.y, vector.x);
		Vector3 vector2 = 3.2f * new Vector3(Mathf.Cos(num), Mathf.Sin(num), 0f);
		float f = num + 0.04363323f;
		float num2 = num - 0.04363323f;
		Vector3 vector3 = 3.2f * new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f);
		Vector3 vector4 = 3.2f * new Vector3(Mathf.Cos(num2), Mathf.Sin(num2), 0f);
		float num3 = SimulateShot(vector3, relativeTargetPosition, targetVelocity, targetAcceleration);
		float num4 = SimulateShot(vector4, relativeTargetPosition, targetVelocity, targetAcceleration);
		float num5;
		float num6;
		if (num3 < num4)
		{
			num5 = num4;
			zero = vector3;
			num = num2;
			num6 = 1f;
		}
		else
		{
			num5 = num3;
			zero = vector4;
			num6 = -1f;
		}
		float num7 = 0.17453292f;
		for (int i = 0; i < 6; i++)
		{
			float num8 = num;
			num += num6 * num7;
			vector2 = 3.2f * new Vector3(Mathf.Cos(num), Mathf.Sin(num), 0f);
			float num9 = SimulateShot(vector2, relativeTargetPosition, targetVelocity, targetAcceleration);
			if (num9 < num5)
			{
				num5 = num9;
				zero = vector2;
				continue;
			}
			num = num8;
			num7 *= 0.5f;
			if (i < 5)
			{
				vector4 = 3.2f * new Vector3(Mathf.Cos(num + 0.1f * num7 * num6 * ((float)Math.PI / 180f)), Mathf.Sin(num + num6 * num7 * ((float)Math.PI / 180f)), 0f);
				if (SimulateShot(vector4, relativeTargetPosition, targetVelocity, targetAcceleration) > num5)
				{
					num6 = 0f - num6;
				}
			}
		}
		return zero;
	}

	private void Aim()
	{
		m_timer += Time.deltaTime;
		float num = Mathf.Min(Mathf.Pow(m_timer * 17f, 0.5f), 3.5f);
		float num2 = Vector3.Distance(m_target.transform.position, base.transform.position);
		Vector3 position = m_target.transform.position;
		position.z = 0f;
		Vector3 velocity = m_target.rigidbody.velocity;
		Vector3 vector = (velocity - m_previousTargetVelocity) / Time.deltaTime;
		m_targetAcceleration = 0.95f * m_targetAcceleration + 0.05f * vector;
		m_previousTargetVelocity = velocity;
		float num3 = num2 / 35f;
		Vector3 vector2 = m_target.transform.position + num3 * velocity;
		Vector3 normalized = (vector2 - base.transform.position).normalized;
		if (vector2.x > base.transform.position.x && !m_flyingRight)
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x * -1f, base.transform.localScale.y, base.transform.localScale.z);
			m_flyingRight = true;
		}
		normalized = -FindShot(position - m_slingshotRestPosition, velocity, 0.5f * m_targetAcceleration);
		normalized = (m_lastAim = 0.2f * normalized + 0.8f * m_lastAim);
		normalized.Normalize();
		Vector3 position2 = m_slingshotRestPosition - num * normalized;
		position2.z = -0.2f;
		base.transform.position = position2;
		if (num >= 3.499f)
		{
			m_shootDirection = normalized;
			base.rigidbody.isKinematic = false;
			SetState(State.Shoot);
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.birdShot);
		}
		m_slingshot.SetDrawPosition(base.transform.position - m_slingshotRestPosition - 0.45f * normalized);
	}

	private void Shoot()
	{
		Vector3 vector = new Vector3(base.transform.position.x, base.transform.position.y, 0f);
		Vector3 vector2 = m_slingshotRestPosition - vector;
		Vector3 normalized = vector2.normalized;
		float num = Vector3.Dot(m_shootDirection, normalized);
		base.rigidbody.AddForce(springConstant * vector2, ForceMode.Force);
		if (num <= 0f)
		{
			SetState(State.Fly);
			m_slingshot.EndShot();
		}
		m_slingshot.SetDrawPosition(base.transform.position - m_slingshotRestPosition - 0.45f * normalized);
	}

	private void Fly()
	{
		float magnitude = base.rigidbody.velocity.magnitude;
		if (m_previousSpeed - magnitude > 5f)
		{
			m_animation.Play("Hit");
			m_isCollided = true;
		}
		m_previousSpeed = magnitude;
		if (m_speedBoost > 0f && !m_boosted)
		{
			Vector3 position = m_target.transform.position;
			if (Vector3.SqrMagnitude(base.transform.position - position) < 0.5f * Vector3.SqrMagnitude(m_slingshotRestPosition - position))
			{
				m_boosted = true;
				base.rigidbody.velocity = m_speedBoost * base.rigidbody.velocity;
			}
		}
		if (m_split > 0 && !m_splitDone)
		{
			Vector3 position2 = m_target.transform.position;
			if (Vector3.SqrMagnitude(base.transform.position - position2) < 0.5f * Vector3.SqrMagnitude(m_slingshotRestPosition - position2))
			{
				Split();
			}
		}
		SphereCollider sphereCollider = base.collider as SphereCollider;
		float num = 0.02f * base.rigidbody.velocity.magnitude;
		m_speed = 0.8f * m_speed + 0.2f * base.rigidbody.velocity.magnitude;
		if (m_speed < 0.75f && !m_collisionsRemoved)
		{
			base.gameObject.layer = LayerMask.NameToLayer("Bird");
			m_collisionsRemoved = true;
			SetState(State.KnockedOut);
			num = 0f;
			if ((bool)sphereCollider)
			{
				sphereCollider.radius = ColliderRadius;
			}
			foreach (Collider ignoredCollision in m_ignoredCollisions)
			{
				Physics.IgnoreCollision(base.collider, ignoredCollision, ignore: false);
			}
			if (Singleton<SocialGameManager>.IsInstantiated() && !m_contraptionHit && !GameProgress.GetBool("bird_evaded"))
			{
				GameProgress.SetBool("bird_evaded", value: true);
				Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.NER_NER", 100.0);
			}
		}
		if ((bool)sphereCollider && m_state == State.Fly)
		{
			float radius = Mathf.Max(0.75f * num, m_colliderRadius);
			sphereCollider.radius = radius;
		}
	}

	private void Split()
	{
		m_splitDone = true;
		GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject);
		Vector3 velocity = base.rigidbody.velocity;
		velocity = Quaternion.AngleAxis(7.5f, Vector3.forward) * velocity;
		gameObject.GetComponent<Rigidbody>().velocity = velocity;
		IgnoreCollisions(gameObject.GetComponent<Collider>());
		gameObject.GetComponent<Bird>().StartAfterSplit(this);
		gameObject.GetComponent<Bird>().SetAlarmOn(on: false);
		WPFMonoBehaviour.levelManager.AddTemporaryDynamicObject(gameObject);
		GameObject gameObject2 = UnityEngine.Object.Instantiate(base.gameObject);
		velocity = base.rigidbody.velocity;
		velocity = Quaternion.AngleAxis(-7.5f, Vector3.forward) * velocity;
		gameObject2.GetComponent<Rigidbody>().velocity = velocity;
		IgnoreCollisions(gameObject2.GetComponent<Collider>());
		gameObject2.GetComponent<Bird>().StartAfterSplit(this);
		gameObject2.GetComponent<Bird>().SetAlarmOn(on: false);
		WPFMonoBehaviour.levelManager.AddTemporaryDynamicObject(gameObject2);
		gameObject.GetComponent<Bird>().IgnoreCollisions(gameObject2.GetComponent<Collider>());
	}

	private IEnumerator PlayAnimation(string name, float delay)
	{
		yield return new WaitForSeconds(delay);
		m_animation.Play(name);
	}

	public float GetNoiseAtPosition(Vector3 position)
	{
		AudioManager instance = Singleton<AudioManager>.Instance;
		float num = 0f;
		foreach (AudioSource activeLoopingSound in instance.GetActiveLoopingSounds())
		{
			if ((bool)activeLoopingSound)
			{
				num += ComputeNoiseLevel(activeLoopingSound, position);
			}
		}
		foreach (AudioSource active3dOneShotSound in instance.GetActive3dOneShotSounds())
		{
			if ((bool)active3dOneShotSound)
			{
				num += ComputeNoiseLevel(active3dOneShotSound, position);
			}
		}
		return num;
	}

	private float ComputeNoiseLevel(AudioSource source, Vector3 position)
	{
		NoiseLevel component = source.GetComponent<NoiseLevel>();
		if ((bool)component && component.Level > 0f)
		{
			float num = Vector3.Distance(position, source.transform.position);
			float result = component.Level / (num + 0.1f);
			_ = source.transform.position;
			return result;
		}
		return 0f;
	}
}
