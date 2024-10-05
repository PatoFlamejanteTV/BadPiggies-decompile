using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Fan : WPFMonoBehaviour
{
	public GameObject fanSprite;

	public ParticleSystem particles;

	public float targetForce;

	public AnimationCurve verticalRamp;

	public AnimationCurve horizontalRamp;

	public AnimationCurve spinupRamp;

	public bool alwaysOn;

	public float delayedStart;

	public float startTime;

	public float offTime;

	public float onTime;

	public float hearingDistance = 1000f;

	public float counter;

	private float force;

	private bool running;

	private bool paused;

	private List<Rigidbody> affectedParts;

	private List<BasePart> partsInZone;

	private Vector3 fanRotation;

	private BoxCollider boxCollider;

	private AudioSource fanSound;

	private GameObject loopingSound;

	private AudioManager audioManager;

	private Vector3 fanTop;

	private Vector3 fanUp;

	private bool initialized;

	private float targetVolume;

	private Transform contraptionTf;

	private State state;

	private AnimationCurve spinDown;

	private float angleLeft;

	private const string CONTRAPTION_NAME = "Part_Pig_01_SET(Clone)(Clone)";

	public Vector3 FanTop
	{
		get
		{
			if (initialized)
			{
				return fanTop;
			}
			if (boxCollider == null)
			{
				boxCollider = GetComponent<Collider>() as BoxCollider;
			}
			fanTop = base.transform.position + boxCollider.center.magnitude * FanUp + FanUp * (boxCollider.size.y / 2f);
			return fanTop;
		}
		set
		{
			if (boxCollider == null)
			{
				boxCollider = GetComponent<Collider>() as BoxCollider;
			}
			float num = Vector3.Angle(value - base.transform.position, Vector3.up);
			if (value.x > base.transform.position.x)
			{
				num = 0f - num;
			}
			base.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, num));
			float magnitude = (value - base.transform.position).magnitude;
			boxCollider.size = new Vector3(boxCollider.size.x, magnitude);
			boxCollider.center = base.transform.InverseTransformPoint((value - base.transform.position).normalized * magnitude / 2f + base.transform.position);
			fanTop = base.transform.position + boxCollider.center.magnitude * FanUp + FanUp * (boxCollider.size.y / 2f);
		}
	}

	public Vector3 FanUp
	{
		get
		{
			if (initialized)
			{
				return fanUp;
			}
			fanUp = base.transform.up;
			return fanUp;
		}
	}

	private void Awake()
	{
		boxCollider = GetComponent<Collider>() as BoxCollider;
		affectedParts = new List<Rigidbody>();
		partsInZone = new List<BasePart>();
		fanRotation = new Vector3(0f, 10f);
	}

	private IEnumerator Start()
	{
		while (!Bundle.initialized)
		{
			yield return null;
		}
		audioManager = Singleton<AudioManager>.Instance;
		EventManager.Connect<UIEvent>(ReceiveUIEvent);
		fanSound = WPFMonoBehaviour.gameData.commonAudioCollection.environmentFanLoop;
		targetVolume = fanSound.volume;
		InitValues();
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<UIEvent>(ReceiveUIEvent);
	}

	private void InitValues()
	{
		delayedStart += onTime;
		fanTop = FanTop;
		fanUp = FanUp;
		paused = true;
		spinDown = new AnimationCurve();
		initialized = true;
	}

	private bool ShouldAffect(BasePart part)
	{
		if (part is Frame)
		{
			return true;
		}
		if (part is Wings)
		{
			return true;
		}
		if (part is Tail)
		{
			return true;
		}
		if (part is Balloon)
		{
			return true;
		}
		if (part is Pig)
		{
			return part.enclosedInto == null;
		}
		return !part.contraption.PartIsConnected(part);
	}

	private void OnTriggerEnter(Collider other)
	{
		BasePart component = other.GetComponent<BasePart>();
		Rigidbody rigidbody = ((!(other.tag == "Dynamic")) ? null : other.GetComponent<Rigidbody>());
		if (rigidbody == null && component != null && ShouldAffect(component))
		{
			rigidbody = component.rigidbody;
		}
		if (rigidbody != null && !affectedParts.Contains(rigidbody))
		{
			affectedParts.Add(rigidbody);
		}
		if (component != null)
		{
			partsInZone.Add(component);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		BasePart component = other.GetComponent<BasePart>();
		Rigidbody component2 = other.GetComponent<Rigidbody>();
		component2 = ((!(component != null)) ? component2 : component.rigidbody);
		if (component2 != null && affectedParts.Contains(component2))
		{
			affectedParts.Remove(component2);
		}
		if (component != null)
		{
			partsInZone.Remove(component);
		}
	}

	private void Update()
	{
		if (!paused)
		{
			switch (state)
			{
			case State.Inactive:
				Inactive();
				break;
			case State.DelayedStart:
				DelayedStart();
				break;
			case State.SpinUp:
				SpinUp();
				break;
			case State.SpinDown:
				SpinDown();
				break;
			case State.Spinning:
				Spinning();
				break;
			}
			if (running)
			{
				fanSprite.transform.Rotate(fanRotation * Mathf.Clamp(force, 0f, 10f));
			}
		}
	}

	private void FixedUpdate()
	{
		if (!running)
		{
			return;
		}
		for (int i = 0; i < partsInZone.Count; i++)
		{
			if (partsInZone[i] == null)
			{
				partsInZone.RemoveAt(i);
			}
			else if (affectedParts.Contains(partsInZone[i].rigidbody))
			{
				partsInZone.RemoveAt(i);
			}
			else if (partsInZone[i].contraption.PartIsConnected(partsInZone[i]))
			{
				affectedParts.Add(partsInZone[i].rigidbody);
				partsInZone.RemoveAt(i);
			}
		}
		for (int j = 0; j < affectedParts.Count; j++)
		{
			if (affectedParts[j] == null)
			{
				affectedParts.RemoveAt(j);
				continue;
			}
			Vector3 vector = CalculateForce(affectedParts[j].transform.position);
			affectedParts[j].AddForce(vector);
		}
	}

	private Vector3 CalculateForce(Vector3 position)
	{
		Vector3 vector = NormalizePosition(position);
		float num = verticalRamp.Evaluate(vector.y);
		float num2 = horizontalRamp.Evaluate(vector.x);
		return fanUp * (num * num2) * force;
	}

	private Vector3 NormalizePosition(Vector3 position)
	{
		Vector3 vector = FanTop;
		Vector3 onNormal = FanUp;
		Vector3 result = default(Vector3);
		Vector3 vector2 = Vector3.Project(position - vector, onNormal) + vector;
		result.y = Mathf.Clamp01((vector - vector2).magnitude / boxCollider.size.y);
		result.x = Mathf.Clamp01((position - vector2).magnitude / (boxCollider.size.x / 2f));
		result.x = Mathf.Abs(result.x -= 1f);
		return result;
	}

	private bool ContraptionInProximity()
	{
		if (contraptionTf == null)
		{
			FindContraption();
		}
		if (!(contraptionTf == null))
		{
			return (contraptionTf.position - base.transform.position).sqrMagnitude < hearingDistance;
		}
		return false;
	}

	private void FindContraption()
	{
		if (contraptionTf == null)
		{
			contraptionTf = Camera.main.transform;
		}
		_ = contraptionTf == null;
	}

	public void TurnOn()
	{
		StartCoroutine(AudioFadeIn());
		state = State.SpinUp;
		running = true;
		particles.Play();
		ParticleSystem.EmissionModule emission = particles.emission;
		emission.enabled = true;
		counter = 0f;
	}

	public void TurnOff()
	{
		running = false;
		ParticleSystem.EmissionModule emission = particles.emission;
		emission.enabled = false;
		state = State.SpinDown;
		counter = 0f;
		for (int i = 0; i < spinDown.length; i++)
		{
			spinDown.RemoveKey(i);
		}
		spinDown.AddKey(new Keyframe(0f, Mathf.Clamp(force, 0f, 10f)));
		spinDown.AddKey(new Keyframe(2f, 0f));
		angleLeft = 360f - fanSprite.transform.localRotation.eulerAngles.y;
		angleLeft += ((angleLeft >= 180f) ? 0f : 180f);
		StartCoroutine(AudioFadeOut());
	}

	private void ReceiveUIEvent(UIEvent data)
	{
		switch (data.type)
		{
		case UIEvent.Type.ContinueFromPause:
			paused = false;
			break;
		case UIEvent.Type.Pause:
			paused = true;
			break;
		case UIEvent.Type.Play:
			paused = false;
			if (!alwaysOn && delayedStart > 0f)
			{
				state = State.DelayedStart;
			}
			else
			{
				TurnOn();
			}
			FindContraption();
			break;
		case UIEvent.Type.Building:
			TurnOff();
			state = State.Inactive;
			fanSprite.transform.localRotation = Quaternion.identity;
			paused = true;
			break;
		}
	}

	private void DelayedStart()
	{
		if (counter < delayedStart)
		{
			counter += Time.deltaTime;
		}
		else
		{
			TurnOn();
		}
	}

	private void SpinUp()
	{
		if (counter < startTime)
		{
			counter += Time.deltaTime;
			float num = counter / startTime;
			force = targetForce * Mathf.Clamp01(spinupRamp.Evaluate(num));
			if (loopingSound != null)
			{
				loopingSound.GetComponent<AudioSource>().volume = num;
			}
		}
		else
		{
			state = State.Spinning;
			counter = 0f;
			force = targetForce;
		}
	}

	private void SpinDown()
	{
		if (angleLeft > 0f && !running)
		{
			float num = spinDown.Evaluate(counter);
			fanSprite.transform.Rotate(new Vector3(0f, 1f) * num);
			angleLeft -= num;
			if (angleLeft < 3f || counter > 2f)
			{
				if (Mathf.Abs(fanSprite.transform.localRotation.eulerAngles.y - 180f) < 90f)
				{
					fanSprite.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f));
				}
				else
				{
					fanSprite.transform.localRotation = Quaternion.identity;
				}
				angleLeft = 0f;
			}
			counter += Time.deltaTime;
		}
		else
		{
			state = State.Inactive;
			counter = 0f;
			running = false;
		}
	}

	private void Inactive()
	{
		if (counter < offTime)
		{
			counter += Time.deltaTime;
		}
		else
		{
			TurnOn();
		}
	}

	private void Spinning()
	{
		if (!alwaysOn)
		{
			if (counter < onTime)
			{
				counter += Time.deltaTime;
			}
			else
			{
				TurnOff();
			}
		}
	}

	private IEnumerator AudioFadeIn()
	{
		if (ContraptionInProximity())
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.environmentFanStart, base.transform.position);
		}
		yield return new WaitForSeconds(0.5f);
		if (ContraptionInProximity())
		{
			loopingSound = audioManager.SpawnCombinedLoopingEffect(fanSound, base.gameObject.transform);
		}
		float time = 2f;
		float counter = 0f;
		while (counter < time && loopingSound != null)
		{
			counter += Time.deltaTime;
			loopingSound.GetComponent<AudioSource>().volume = counter / time * targetVolume;
			yield return null;
		}
		if (loopingSound != null)
		{
			loopingSound.GetComponent<AudioSource>().volume = targetVolume;
		}
	}

	private IEnumerator AudioFadeOut()
	{
		float time = 0.75f;
		float counter = time;
		while (counter > 0f && loopingSound != null)
		{
			counter -= Time.deltaTime;
			loopingSound.GetComponent<AudioSource>().volume = counter / time * targetVolume;
			yield return null;
		}
		if (loopingSound != null)
		{
			audioManager.RemoveCombinedLoopingEffect(fanSound, loopingSound);
			loopingSound = null;
		}
	}
}
