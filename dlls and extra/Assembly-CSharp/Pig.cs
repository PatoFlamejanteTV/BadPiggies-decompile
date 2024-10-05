using System;
using System.Collections;
using UnityEngine;

public class Pig : BasePart
{
	public enum Expressions
	{
		Normal,
		Laugh,
		Grin,
		Fear,
		Fear2,
		Hit,
		Blink,
		FearfulGrin,
		Chew,
		WaitForFood,
		Burp,
		Snooze,
		Panting,
		MAX
	}

	[Serializable]
	public class Expression
	{
		public Expressions type;

		public Texture texture;

		public AudioSource[] sound;
	}

	public class PigOutOfBounds : EventManager.Event
	{
	}

	public Expression[] m_expressions;

	public float fallFearThreshold;

	public float speedFunThreshold;

	public float speedFearThreshold;

	[HideInInspector]
	public Expressions m_currentExpression;

	public float m_expressionSetTime;

	private AudioSource m_currentSound;

	private float m_blinkTimer;

	private bool m_isPlayingAnimation;

	private Transform m_visualizationPart;

	private SpriteAnimation m_faceAnimation;

	private Transform m_pupilMover;

	public ParticleSystem collisionStars;

	public ParticleSystem sweatLoop;

	public ParticleSystem starsLoop;

	public bool m_isSilent;

	[Range(0f, 2f)]
	public float m_pitch = 1f;

	private float m_starsTimer;

	private float m_rolledDistance;

	private float m_traveledDistance;

	private FaceRotation m_faceRotation;

	private Vector2 m_lookDirection;

	private Vector2 m_lookTargetDirection;

	private float m_lookDirectionChangeTime;

	private bool m_playerIsDraggingPart;

	private Vector3 m_draggingPartPosition;

	private bool m_lookAtDraggedPart;

	private float m_lookAtDraggedPartDistance;

	private float m_lookAtDraggedPartTimer;

	private Vector3 m_partPlacementPosition;

	private float m_partPlacementTime;

	private bool m_partPlaced;

	private PartType m_placedPartType;

	private bool m_partRemoved;

	private PartType m_removedPartType;

	private float m_funLevel;

	private float m_fearLevel;

	private bool m_randomBehaviorActive;

	private float m_randomLaughTime;

	private Vector3 m_stopTestPosition;

	private float m_stopTestTimer;

	private bool m_replayPulseDone;

	private bool m_faceZFlattened;

	private float m_workingTime = 0.5f;

	private bool m_detached;

	private bool m_checkCameraLimits = true;

	private AudioChorusFilter m_chorusFilter;

	private AudioDistortionFilter m_distortionFilter;

	private AudioEchoFilter m_echoFilter;

	private AudioHighPassFilter m_hpFilter;

	private AudioLowPassFilter m_lpFilter;

	private AudioReverbFilter m_reverbFilter;

	private float m_currentMagnitude;

	private float m_previousMagnitude;

	public bool CheckCameraLimits
	{
		get
		{
			return m_checkCameraLimits;
		}
		set
		{
			m_checkCameraLimits = value;
		}
	}

	public float rolledDistance => m_rolledDistance;

	public float traveledDistance => m_traveledDistance;

	public override bool IsIntegralPart()
	{
		return base.enclosedInto;
	}

	public override bool CanBeEnclosed()
	{
		return true;
	}

	public override void OnDetach()
	{
		base.rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
		m_detached = true;
		base.OnDetach();
	}

	public override void PostInitialize()
	{
		base.rigidbody.collisionDetectionMode = ((base.enclosedInto == null) ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete);
		base.PostInitialize();
	}

	public override void Awake()
	{
		base.Awake();
		m_visualizationPart = base.transform.Find("PigVisualization").transform;
		m_faceRotation = m_visualizationPart.Find("Face").GetComponent<FaceRotation>();
		m_faceAnimation = GetComponentInChildren<SpriteAnimation>();
		m_pupilMover = m_visualizationPart.transform.Find("Face").Find("PupilMover");
		m_lookDirectionChangeTime = Time.time + UnityEngine.Random.Range(2f, 4f);
		m_lookAtDraggedPartDistance = UnityEngine.Random.Range(0f, 10f);
		m_stopTestTimer = 0f;
		m_replayPulseDone = false;
		m_chorusFilter = GetComponent<AudioChorusFilter>();
		m_distortionFilter = GetComponent<AudioDistortionFilter>();
		m_echoFilter = GetComponent<AudioEchoFilter>();
		m_hpFilter = GetComponent<AudioHighPassFilter>();
		m_lpFilter = GetComponent<AudioLowPassFilter>();
		m_reverbFilter = GetComponent<AudioReverbFilter>();
	}

	private void OnEnable()
	{
		EventManager.Connect<DraggingPartEvent>(ReceiveDraggingPartEvent);
		EventManager.Connect<DragStartedEvent>(ReceiveDragStartedEvent);
		EventManager.Connect<PartPlacedEvent>(ReceivePartPlacedEvent);
		EventManager.Connect<PartRemovedEvent>(ReceivePartRemovedEvent);
		EventManager.Connect<ObjectiveAchieved>(ReceiveObjectiveAchieved);
		EventManager.Connect<UserInputEvent>(ReceiveUserInputEvent);
		m_randomBehaviorActive = false;
	}

	private void OnDisable()
	{
		EventManager.Disconnect<DraggingPartEvent>(ReceiveDraggingPartEvent);
		EventManager.Disconnect<DragStartedEvent>(ReceiveDragStartedEvent);
		EventManager.Disconnect<PartPlacedEvent>(ReceivePartPlacedEvent);
		EventManager.Disconnect<PartRemovedEvent>(ReceivePartRemovedEvent);
		EventManager.Disconnect<ObjectiveAchieved>(ReceiveObjectiveAchieved);
		EventManager.Disconnect<UserInputEvent>(ReceiveUserInputEvent);
	}

	public override void EnsureRigidbody()
	{
		if (base.rigidbody == null)
		{
			base.rigidbody = base.gameObject.AddComponent<Rigidbody>();
		}
		base.rigidbody.constraints = (RigidbodyConstraints)56;
		base.rigidbody.mass = m_mass;
		base.rigidbody.drag = 0.2f;
		base.rigidbody.angularDrag = 0.05f;
		base.rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		base.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
	}

	private void OnDestroy()
	{
	}

	private void FixedUpdate()
	{
		if ((bool)base.contraption && base.contraption.IsRunning)
		{
			float magnitude = base.rigidbody.velocity.magnitude;
			if (magnitude < 1f)
			{
				base.rigidbody.drag = 0.2f + 2.5f * (1f - magnitude);
				base.rigidbody.angularDrag = 0.2f + 2.5f * (1f - magnitude);
			}
			else
			{
				base.rigidbody.drag = 0.2f;
				base.rigidbody.angularDrag = 0.05f;
			}
		}
	}

	public void PrepareForTNT(Vector3 tntPosition, float force)
	{
	}

	private void Start()
	{
		m_traveledDistance = GameProgress.GetFloat("traveledDistance");
		m_rolledDistance = GameProgress.GetFloat("rolledDistance");
	}

	private void Update()
	{
		UpdateBuildModeAnimations();
		float x = 0.2f * Mathf.PerlinNoise(0.75f * Time.time, 0f) - 0.1f;
		float y = 0.2f * Mathf.PerlinNoise(0f, 0.75f * Time.time) - 0.1f;
		m_faceRotation.SetTargetDirection(m_lookDirection + new Vector2(x, y));
		m_blinkTimer -= Time.deltaTime;
		bool flag = false;
		if (m_blinkTimer <= 0f && m_currentExpression == Expressions.Normal)
		{
			m_faceAnimation.Play("Blink");
			if (m_playerIsDraggingPart)
			{
				m_pupilMover.transform.localPosition = 0.035f * m_lookTargetDirection;
			}
			else
			{
				m_pupilMover.transform.localPosition = 0.04f * UnityEngine.Random.insideUnitCircle;
			}
			m_blinkTimer = UnityEngine.Random.Range(1.5f, 4f);
			flag = true;
		}
		m_playerIsDraggingPart = false;
		if (!base.contraption || !base.contraption.IsRunning)
		{
			return;
		}
		if (base.enclosedInto == null)
		{
			m_rolledDistance += base.rigidbody.velocity.magnitude * Time.deltaTime;
		}
		else
		{
			m_traveledDistance += base.rigidbody.velocity.magnitude * Time.deltaTime;
		}
		m_currentMagnitude = base.rigidbody.velocity.magnitude;
		if (Mathf.Abs(m_currentMagnitude - m_previousMagnitude) > 5f && !m_isPlayingAnimation)
		{
			PlayAnimation(Expressions.Hit, 1f);
			starsLoop.Play();
			m_starsTimer = 4f;
			m_previousMagnitude = m_currentMagnitude;
			return;
		}
		m_previousMagnitude = m_currentMagnitude;
		if (!m_isPlayingAnimation)
		{
			Expressions expression = SelectExpression();
			if (!flag)
			{
				SetExpression(expression);
			}
		}
		if (!base.contraption.HasComponentEngine(base.ConnectedComponent) && base.contraption.HasPoweredPartsRunning(base.ConnectedComponent))
		{
			PlayWorkingAnimation();
			if (!sweatLoop.isPlaying)
			{
				sweatLoop.Play();
			}
		}
		else
		{
			if (sweatLoop.isPlaying)
			{
				sweatLoop.Stop();
			}
			if (m_visualizationPart.localScale.x > 1.001f || m_visualizationPart.localScale.y > 1.001f || m_visualizationPart.localPosition.y > 0.001f)
			{
				m_visualizationPart.localScale = 0.9f * m_visualizationPart.localScale + 0.1f * Vector3.one;
				m_visualizationPart.localPosition = 0.9f * m_visualizationPart.localPosition + 0.1f * Vector3.zero;
				m_workingTime = 0.5f;
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
		if (!m_replayPulseDone && Singleton<GameManager>.Instance.IsInGame())
		{
			CheckStopped();
		}
		if (!m_faceZFlattened && !GetComponent<Joint>())
		{
			m_faceRotation.ScaleFaceZ(0.01f);
			m_faceZFlattened = true;
		}
	}

	private void CheckStopped()
	{
		if (WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.Running)
		{
			return;
		}
		Vector3 position = base.transform.position;
		if (Vector3.Distance(position, m_stopTestPosition) > 0.1f)
		{
			m_stopTestPosition = position;
			m_stopTestTimer = 0f;
		}
		else
		{
			m_stopTestTimer += Time.deltaTime;
			if (m_stopTestTimer > 5f)
			{
				EventManager.Send(new PulseButtonEvent(UIEvent.Type.Building));
				m_replayPulseDone = true;
			}
		}
		if (!INSettings.GetBool(INFeature.CancelPigBoundsDetection) && m_checkCameraLimits)
		{
			LevelManager.CameraLimits currentCameraLimits = WPFMonoBehaviour.levelManager.CurrentCameraLimits;
			if (position.y < currentCameraLimits.topLeft.y - currentCameraLimits.size.y || position.x > currentCameraLimits.topLeft.x + currentCameraLimits.size.x * 1.1f || position.x < currentCameraLimits.topLeft.x - currentCameraLimits.size.x * 0.1f)
			{
				EventManager.Send(new PigOutOfBounds());
			}
		}
	}

	private Expressions SelectExpression()
	{
		Vector3 velocity = base.rigidbody.velocity;
		Expressions result = Expressions.Normal;
		if (Time.time > m_expressionSetTime + 1f)
		{
			float num = velocity.magnitude + 0.3f * Mathf.Abs(velocity.y);
			if (num > speedFunThreshold)
			{
				result = Expressions.Grin;
			}
			if (num > 0.5f * (speedFunThreshold + speedFearThreshold))
			{
				result = Expressions.FearfulGrin;
			}
			if (num > speedFearThreshold)
			{
				result = Expressions.Fear;
			}
			if (Time.time - base.contraption.GetGroundTouchTime(base.ConnectedComponent) > 0.25f && 0f - velocity.y > fallFearThreshold)
			{
				result = Expressions.Fear2;
			}
		}
		else
		{
			result = m_currentExpression;
		}
		return result;
	}

	private void UpdateBuildModeAnimations()
	{
		if ((!Singleton<GameManager>.Instance.IsInGame() || WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.Building) && Singleton<GameManager>.Instance.GetGameState() != GameManager.GameState.KingPigFeeding)
		{
			m_randomBehaviorActive = false;
			return;
		}
		FollowPartDragging();
		m_funLevel *= Mathf.Pow(0.997f, Time.deltaTime / (1f / 60f));
		m_fearLevel *= Mathf.Pow(0.999f, Time.deltaTime / (1f / 60f));
		if (m_partRemoved)
		{
			m_partRemoved = false;
			GameData.PartReaction partReaction = WPFMonoBehaviour.gameData.GetPartReaction(m_removedPartType);
			if (partReaction != null)
			{
				m_funLevel -= partReaction.fun;
				m_fearLevel -= partReaction.fear;
				m_funLevel = Mathf.Clamp(m_funLevel, 0f, 100f);
				m_fearLevel = Mathf.Clamp(m_fearLevel, 0f, 100f);
			}
		}
		if (m_lookAtDraggedPart)
		{
			m_randomBehaviorActive = false;
			if (!m_partPlaced)
			{
				return;
			}
			m_partPlaced = false;
			GameData.PartReaction partReaction2 = WPFMonoBehaviour.gameData.GetPartReaction(m_placedPartType);
			if (partReaction2 != null)
			{
				m_funLevel += partReaction2.fun;
				m_fearLevel += partReaction2.fear;
				m_funLevel = Mathf.Clamp(m_funLevel, 0f, 100f);
				m_fearLevel = Mathf.Clamp(m_fearLevel, 0f, 100f);
			}
			if (m_fearLevel < 50f || partReaction2.fear == 0f)
			{
				if (UnityEngine.Random.Range(0f, 100f) < m_funLevel && m_expressionSetTime < Time.time - 3f)
				{
					PlayAnimation(Expressions.Laugh, 1.5f);
				}
			}
			else if (UnityEngine.Random.Range(0f, 100f) < m_fearLevel && m_expressionSetTime < Time.time - 3f)
			{
				PlayAnimation(Expressions.Fear, 1.5f);
			}
			return;
		}
		if (!m_randomBehaviorActive)
		{
			m_randomBehaviorActive = true;
			m_randomLaughTime = Time.time + UnityEngine.Random.Range(5f, 9f);
		}
		if (Time.time >= m_randomLaughTime)
		{
			m_randomLaughTime = Time.time + UnityEngine.Random.Range(5f, 9f);
			if (m_expressionSetTime < Time.time - 3f)
			{
				PlayAnimation(Expressions.Laugh, 1.5f);
			}
		}
	}

	private void FollowPartDragging()
	{
		if (!m_playerIsDraggingPart && m_partPlacementTime > Time.time - 1.5f)
		{
			m_playerIsDraggingPart = true;
			m_draggingPartPosition = m_partPlacementPosition;
		}
		if (m_playerIsDraggingPart)
		{
			if (!m_lookAtDraggedPart && m_lookAtDraggedPartTimer < 1f && Vector2.Distance(base.transform.position, m_draggingPartPosition) < m_lookAtDraggedPartDistance)
			{
				m_lookAtDraggedPart = true;
			}
		}
		else
		{
			m_lookAtDraggedPart = false;
		}
		if (m_lookAtDraggedPart)
		{
			m_lookAtDraggedPartTimer += Time.deltaTime;
			if (m_lookAtDraggedPartTimer > 8f)
			{
				m_faceAnimation.Play("Hit");
				m_blinkTimer = 0.2f;
				m_lookAtDraggedPart = false;
			}
			m_lookTargetDirection = m_draggingPartPosition - base.transform.position;
			if (m_lookTargetDirection.sqrMagnitude > 1f)
			{
				m_lookTargetDirection.Normalize();
			}
			Vector2 vector = m_lookTargetDirection - m_lookDirection;
			m_lookDirection += 8f * Time.deltaTime * vector;
		}
		else
		{
			if (m_lookAtDraggedPartTimer > 0f)
			{
				m_lookAtDraggedPartTimer -= Time.deltaTime;
			}
			if (Time.time > m_lookDirectionChangeTime)
			{
				m_lookDirectionChangeTime = Time.time + UnityEngine.Random.Range(2f, 4f);
				Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
				if (Vector2.Distance(m_lookTargetDirection, insideUnitCircle) < 0.3f)
				{
					insideUnitCircle = UnityEngine.Random.insideUnitCircle;
				}
				m_lookTargetDirection = Mathf.Pow(insideUnitCircle.magnitude, 0.2f) * insideUnitCircle.normalized;
			}
			Vector2 vector2 = m_lookTargetDirection - m_lookDirection;
			m_lookDirection += 2f * Time.deltaTime * vector2;
		}
		if (m_playerIsDraggingPart)
		{
			Vector3 vector3 = 0.035f * m_lookTargetDirection;
			if (Vector3.Distance(m_pupilMover.transform.localPosition, vector3) > 0.010500001f)
			{
				Vector3 vector4 = vector3 - m_pupilMover.transform.localPosition;
				m_pupilMover.transform.localPosition += 4f * Time.deltaTime * vector4;
			}
		}
	}

	private void ReceiveUserInputEvent(UserInputEvent data)
	{
		EventManager.Send(new PulseButtonEvent(UIEvent.Type.Building, pulse: false));
		m_stopTestTimer = 0f;
	}

	private void ReceiveDraggingPartEvent(DraggingPartEvent data)
	{
		m_playerIsDraggingPart = true;
		m_draggingPartPosition = data.position;
	}

	private void ReceiveDragStartedEvent(DragStartedEvent data)
	{
		m_lookAtDraggedPartDistance = UnityEngine.Random.Range(0f, 10f);
	}

	private void ReceivePartPlacedEvent(PartPlacedEvent data)
	{
		m_partPlacementTime = Time.time;
		m_partPlacementPosition = data.position;
		m_partPlaced = true;
		m_placedPartType = data.partType;
	}

	private void ReceivePartRemovedEvent(PartRemovedEvent data)
	{
		m_partRemoved = true;
		m_removedPartType = data.partType;
	}

	private void ReceiveObjectiveAchieved(ObjectiveAchieved data)
	{
		PlayAnimation(Expressions.Laugh, 3f);
	}

	public override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		if (collision.relativeVelocity.magnitude > 2f)
		{
			collisionStars.Play();
		}
	}

	public void SetExpression(Expressions exp)
	{
		if (m_currentExpression == exp)
		{
			return;
		}
		switch (exp)
		{
		case Expressions.Normal:
			m_faceAnimation.Play("Normal");
			break;
		case Expressions.Laugh:
			m_faceAnimation.Play("Laugh");
			break;
		case Expressions.Grin:
			m_faceAnimation.Play("Grin");
			break;
		case Expressions.Fear:
			m_faceAnimation.Play("Fear_1");
			break;
		case Expressions.Fear2:
			m_faceAnimation.Play("Fear_2");
			break;
		case Expressions.Hit:
			m_faceAnimation.Play("Hit");
			break;
		case Expressions.Blink:
			m_faceAnimation.Play("Blink");
			break;
		case Expressions.FearfulGrin:
			m_faceAnimation.Play("FearfulGrin");
			break;
		}
		if (Singleton<GameManager>.Instance.IsInGame() && WPFMonoBehaviour.levelManager != null && WPFMonoBehaviour.levelManager.gameState != LevelManager.GameState.Building)
		{
			AudioSource[] sound = m_expressions[(int)exp].sound;
			if (sound != null && sound.Length != 0)
			{
				for (int i = 0; i < sound.Length; i++)
				{
					if (sound[i] != null)
					{
						sound[i].pitch = m_pitch;
					}
				}
				if ((bool)m_currentSound)
				{
					m_currentSound.Stop();
				}
				m_currentSound = ((!m_isSilent) ? Singleton<AudioManager>.Instance.SpawnOneShotEffect(sound, base.transform) : null);
				if ((bool)m_chorusFilter)
				{
					AudioChorusFilter audioChorusFilter = m_currentSound.gameObject.AddComponent<AudioChorusFilter>();
					audioChorusFilter.dryMix = m_chorusFilter.dryMix;
					audioChorusFilter.wetMix1 = m_chorusFilter.wetMix1;
					audioChorusFilter.wetMix2 = m_chorusFilter.wetMix2;
					audioChorusFilter.wetMix3 = m_chorusFilter.wetMix3;
					audioChorusFilter.delay = m_chorusFilter.delay;
					audioChorusFilter.rate = m_chorusFilter.rate;
					audioChorusFilter.depth = m_chorusFilter.depth;
				}
				if ((bool)m_distortionFilter)
				{
					m_currentSound.gameObject.AddComponent<AudioDistortionFilter>().distortionLevel = m_distortionFilter.distortionLevel;
				}
				if ((bool)m_echoFilter)
				{
					AudioEchoFilter audioEchoFilter = m_currentSound.gameObject.AddComponent<AudioEchoFilter>();
					audioEchoFilter.delay = m_echoFilter.delay;
					audioEchoFilter.decayRatio = m_echoFilter.decayRatio;
					audioEchoFilter.wetMix = m_echoFilter.wetMix;
					audioEchoFilter.dryMix = m_echoFilter.dryMix;
				}
				if ((bool)m_hpFilter)
				{
					AudioHighPassFilter audioHighPassFilter = m_currentSound.gameObject.AddComponent<AudioHighPassFilter>();
					audioHighPassFilter.cutoffFrequency = m_hpFilter.cutoffFrequency;
					audioHighPassFilter.highpassResonanceQ = m_hpFilter.highpassResonanceQ;
				}
				if ((bool)m_lpFilter)
				{
					AudioLowPassFilter audioLowPassFilter = m_currentSound.gameObject.AddComponent<AudioLowPassFilter>();
					audioLowPassFilter.cutoffFrequency = m_lpFilter.cutoffFrequency;
					audioLowPassFilter.lowpassResonanceQ = m_lpFilter.lowpassResonanceQ;
				}
				if ((bool)m_reverbFilter)
				{
					AudioReverbFilter audioReverbFilter = m_currentSound.gameObject.AddComponent<AudioReverbFilter>();
					if (m_reverbFilter.reverbPreset == AudioReverbPreset.User)
					{
						audioReverbFilter.dryLevel = m_reverbFilter.dryLevel;
						audioReverbFilter.room = m_reverbFilter.room;
						audioReverbFilter.roomHF = m_reverbFilter.roomHF;
						audioReverbFilter.roomLF = m_reverbFilter.roomLF;
						audioReverbFilter.decayTime = m_reverbFilter.decayTime;
						audioReverbFilter.decayHFRatio = m_reverbFilter.decayHFRatio;
						audioReverbFilter.reflectionsLevel = m_reverbFilter.reflectionsLevel;
						audioReverbFilter.reflectionsDelay = m_reverbFilter.reflectionsDelay;
						audioReverbFilter.hfReference = m_reverbFilter.hfReference;
						audioReverbFilter.lfReference = m_reverbFilter.lfReference;
						audioReverbFilter.diffusion = m_reverbFilter.diffusion;
						audioReverbFilter.density = m_reverbFilter.density;
					}
					else
					{
						audioReverbFilter.reverbPreset = m_reverbFilter.reverbPreset;
					}
				}
			}
		}
		m_expressionSetTime = Time.time;
		m_currentExpression = exp;
	}

	public void PlayAnimation(Expressions exp, float time)
	{
		StartCoroutine(AnimationCoroutine(exp, time));
	}

	private IEnumerator AnimationCoroutine(Expressions exp, float time)
	{
		m_isPlayingAnimation = true;
		SetExpression(exp);
		yield return new WaitForSeconds(time);
		SetExpression(Expressions.Normal);
		m_isPlayingAnimation = false;
	}

	private void PlayWorkingAnimation()
	{
		if (!m_detached)
		{
			m_workingTime += Time.deltaTime;
			m_visualizationPart.localScale = new Vector3(Mathf.PingPong(m_workingTime * 0.2f + 0.1f, 0.1f) + 1f, Mathf.PingPong(m_workingTime * 0.2f, 0.1f) + 0.9f, 1f);
			m_visualizationPart.localPosition = new Vector3(0f, 0f - Mathf.PingPong(m_workingTime * 0.2f + 0.1f, 0.1f), 0f);
		}
	}

	protected override void OnTouch()
	{
		base.contraption.ActivateAllPoweredParts(base.ConnectedComponent);
	}
}
