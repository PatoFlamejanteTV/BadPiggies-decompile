using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class LevelBalloon : WPFMonoBehaviour
{
	public bool popOnCollect;

	public bool useTargetAltitude;

	public float targetAltitude;

	public float upForce;

	public float breakForce;

	public OneTimeCollectable box;

	private SpringJoint rope;

	private RopeVisualization ropeVisual;

	private bool collected;

	private PartBox partBox;

	private StarBox starBox;

	private SpringJointValues springJointValues;

	private Vector3 origPosition;

	private Quaternion origRotation;

	private void Start()
	{
		if (!useTargetAltitude)
		{
			targetAltitude = base.transform.position.y;
		}
		ropeVisual = GetComponent<RopeVisualization>();
		collected = false;
		rope = GetComponent<SpringJoint>();
		if (box.IsDisabled())
		{
			Object.Destroy(base.transform.parent.gameObject);
			return;
		}
		partBox = box as PartBox;
		starBox = box as StarBox;
		if (partBox != null)
		{
			partBox.onCollect += PartBoxCollected;
		}
		else if (starBox != null)
		{
			starBox.onCollect += StarBoxCollected;
		}
		SaveState();
		EventManager.Connect<GameStateChanged>(OnGameStateChanged);
	}

	private void OnDestroy()
	{
		if (partBox != null)
		{
			partBox.onCollect -= PartBoxCollected;
		}
		else if (starBox != null)
		{
			starBox.onCollect -= StarBoxCollected;
		}
		EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
	}

	private void FixedUpdate()
	{
		if (collected || base.transform.position.y < targetAltitude)
		{
			base.rigidbody.AddForce(Vector3.up * upForce);
			return;
		}
		float num = Mathf.Abs(base.transform.position.y - targetAltitude - 1f) / 1f;
		base.rigidbody.AddForce(Vector3.up * upForce * num);
	}

	private void PartBoxCollected(PartBox partbox)
	{
		collected = true;
		if (popOnCollect)
		{
			Pop();
		}
		else
		{
			Detach();
		}
	}

	private void StarBoxCollected()
	{
		PartBoxCollected(null);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > breakForce)
		{
			Detach();
			Pop();
		}
	}

	private void OnGameStateChanged(GameStateChanged newState)
	{
		LevelManager.GameState state = newState.state;
		if (state == LevelManager.GameState.Running || state == LevelManager.GameState.ShowingUnlockedParts || state == LevelManager.GameState.Building)
		{
			LoadState();
		}
	}

	[ContextMenu("Pop")]
	private void Pop()
	{
		Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.balloonPop, base.transform.position);
		WPFMonoBehaviour.effectManager.CreateParticles(WPFMonoBehaviour.gameData.m_ballonParticles, base.transform.position);
		base.gameObject.SetActive(value: false);
	}

	private void Detach()
	{
		Object.Destroy(rope);
		ropeVisual.enabled = false;
	}

	private void SaveState()
	{
		origPosition = base.transform.position;
		origRotation = base.transform.rotation;
		SaveSpringJointValues();
	}

	private void LoadState()
	{
		base.transform.position = origPosition;
		base.transform.rotation = origRotation;
		base.gameObject.SetActive(value: true);
		ropeVisual.enabled = true;
		collected = false;
		LoadSpringJointValues();
	}

	private void SaveSpringJointValues()
	{
		springJointValues = new SpringJointValues();
		springJointValues.autoConfigureConnectedAnchor = rope.autoConfigureConnectedAnchor;
		springJointValues.anchor = rope.anchor;
		springJointValues.breakForce = rope.breakForce;
		springJointValues.breakTorque = rope.breakTorque;
		springJointValues.connectedAnchor = rope.connectedAnchor;
		springJointValues.connectedRigidbody = rope.connectedBody;
		springJointValues.enableCollision = rope.enableCollision;
		springJointValues.maxDistance = rope.maxDistance;
		springJointValues.minDistance = rope.minDistance;
		springJointValues.damper = rope.damper;
	}

	private void LoadSpringJointValues()
	{
		if (rope == null)
		{
			rope = base.gameObject.AddComponent<SpringJoint>();
		}
		rope.autoConfigureConnectedAnchor = springJointValues.autoConfigureConnectedAnchor;
		rope.anchor = springJointValues.anchor;
		rope.breakForce = springJointValues.breakForce;
		rope.breakTorque = springJointValues.breakTorque;
		rope.connectedAnchor = springJointValues.connectedAnchor;
		rope.connectedBody = springJointValues.connectedRigidbody;
		rope.enableCollision = springJointValues.enableCollision;
		rope.maxDistance = springJointValues.maxDistance;
		rope.minDistance = springJointValues.minDistance;
		rope.damper = springJointValues.damper;
	}
}
