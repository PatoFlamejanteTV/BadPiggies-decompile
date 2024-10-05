using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LevelRigidbody : WPFMonoBehaviour
{
	protected class HingeJointValues
	{
		public Rigidbody connectedBody;

		public Vector3 anchor;

		public Vector3 axis;

		public Vector3 connectedAnchor;

		public bool autoConfigureConnectedAnchor;

		public bool useMotor;

		public JointMotor motor;

		public bool useSpring;

		public JointSpring spring;

		public bool useLimits;

		public JointLimits limits;

		public float breakForce;

		public float breakTorque;

		public bool enableCollision;

		public bool enablePreprocessing;
	}

	public float breakForce = float.PositiveInfinity;

	public bool breakOnlyByGoldenPig;

	public bool breakOnlyByBird;

	public bool chainReactionBreaking;

	public ParticleSystem breakEffect;

	public bool lockPosition;

	public bool freezeOnEnd = true;

	public AudioManager.AudioMaterial audioMaterial;

	public bool isRock;

	private bool m_originalIsKinematic;

	protected Vector3 m_originalPosition = Vector3.zero;

	protected Quaternion m_originalRotation = Quaternion.identity;

	private float m_lastTimePlayedSFX;

	private FixedJoint m_fixedJoint;

	protected HingeJointValues m_originalHingeJointValues;

	protected HingeJoint m_hingeJoint;

	protected Transform m_transform;

	private List<Collider> m_normalColliders;

	private List<Transform> m_iceColliders;

	private int m_icePartLayer;

	private PhysicMaterial m_iceMaterial;

	[SerializeField]
	private bool m_broken;

	[SerializeField]
	private bool m_createIceColliders;

	private LevelManager.GameState lastTrackedState;

	private bool isDataLoaded;

	private void Awake()
	{
		m_transform = base.transform;
		m_hingeJoint = GetComponent<HingeJoint>();
		m_fixedJoint = GetComponent<FixedJoint>();
		m_iceMaterial = Resources.Load<PhysicMaterial>("Ground_PhysMat_Ice");
		m_icePartLayer = LayerMask.NameToLayer("IcePart");
		EventManager.Connect<GameStateChanged>(OnGameStateChanged);
	}

	private void OnDataLoaded()
	{
		if (!isDataLoaded)
		{
			isDataLoaded = true;
			if (lockPosition)
			{
				base.gameObject.layer = LayerMask.NameToLayer("FixedRigidbody");
			}
			SaveState();
			LoadState();
			base.rigidbody.isKinematic = freezeOnEnd;
			float @float = INSettings.GetFloat(INFeature.TerrainScale);
			Vector3 vector = m_originalPosition * @float;
			if (base.transform.parent != null)
			{
				vector += base.transform.parent.position * (@float - 1f);
			}
			m_originalPosition = new Vector3(vector.x, vector.y, m_originalPosition.z);
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
	}

	private void OnGameStateChanged(GameStateChanged newState)
	{
		if (newState.state == LevelManager.GameState.Running && lastTrackedState == LevelManager.GameState.Building)
		{
			ResetRigidbody();
			if (WPFMonoBehaviour.levelManager.HasGroundIce)
			{
				UpdateIceCollider();
			}
		}
		else if ((newState.state == LevelManager.GameState.Building || newState.state == LevelManager.GameState.ShowingUnlockedParts) && (lastTrackedState == LevelManager.GameState.Running || lastTrackedState == LevelManager.GameState.PausedWhileRunning))
		{
			EndLevel();
		}
		lastTrackedState = newState.state;
	}

	private void SaveState()
	{
		m_originalPosition = base.transform.localPosition;
		m_originalRotation = base.transform.localRotation;
		m_originalIsKinematic = base.rigidbody.isKinematic;
		if ((bool)m_hingeJoint)
		{
			if (m_originalHingeJointValues == null)
			{
				m_originalHingeJointValues = new HingeJointValues();
			}
			m_originalHingeJointValues.connectedBody = m_hingeJoint.connectedBody;
			m_originalHingeJointValues.anchor = m_hingeJoint.anchor;
			m_originalHingeJointValues.axis = m_hingeJoint.axis;
			m_originalHingeJointValues.connectedAnchor = m_hingeJoint.connectedAnchor;
			m_originalHingeJointValues.autoConfigureConnectedAnchor = m_hingeJoint.autoConfigureConnectedAnchor;
			m_originalHingeJointValues.useMotor = m_hingeJoint.useMotor;
			m_originalHingeJointValues.motor = m_hingeJoint.motor;
			m_originalHingeJointValues.useSpring = m_hingeJoint.useSpring;
			m_originalHingeJointValues.spring = m_hingeJoint.spring;
			m_originalHingeJointValues.useLimits = m_hingeJoint.useLimits;
			m_originalHingeJointValues.limits = m_hingeJoint.limits;
			m_originalHingeJointValues.breakForce = m_hingeJoint.breakForce;
			m_originalHingeJointValues.breakTorque = m_hingeJoint.breakTorque;
			m_originalHingeJointValues.enableCollision = m_hingeJoint.enableCollision;
			m_originalHingeJointValues.enablePreprocessing = m_hingeJoint.enablePreprocessing;
		}
	}

	private void LoadState()
	{
		if (base.gameObject != null && base.gameObject.activeInHierarchy)
		{
			StartCoroutine(LoadStateRoutine());
		}
	}

	private IEnumerator LoadStateRoutine()
	{
		ResetRigidbody();
		ResetTransform();
		yield return new WaitForFixedUpdate();
		ResetJoints();
		ResetEffects();
	}

	private void EndLevel()
	{
		LoadState();
		base.rigidbody.isKinematic = freezeOnEnd;
	}

	private void ResetRigidbody()
	{
		if (!(this == null))
		{
			base.rigidbody.Sleep();
			base.rigidbody.isKinematic = m_originalIsKinematic;
			if (!m_originalIsKinematic)
			{
				base.rigidbody.WakeUp();
			}
			m_broken = false;
		}
	}

	private void ResetTransform()
	{
		m_transform = m_transform ?? base.transform;
		bool isKinematic = base.rigidbody.isKinematic;
		base.rigidbody.isKinematic = true;
		m_transform.localPosition = m_originalPosition;
		m_transform.localRotation = m_originalRotation;
		base.rigidbody.isKinematic = isKinematic;
	}

	private void ResetJoints()
	{
		if (lockPosition)
		{
			if (m_fixedJoint == null)
			{
				m_fixedJoint = base.gameObject.AddComponent<FixedJoint>();
			}
			m_fixedJoint.breakForce = breakForce;
			m_fixedJoint.enablePreprocessing = false;
		}
		if (m_hingeJoint == null && m_originalHingeJointValues != null)
		{
			m_hingeJoint = base.gameObject.AddComponent<HingeJoint>();
		}
		if (m_originalHingeJointValues != null && !(m_hingeJoint == null))
		{
			m_hingeJoint.autoConfigureConnectedAnchor = m_originalHingeJointValues.autoConfigureConnectedAnchor;
			m_hingeJoint.connectedBody = m_originalHingeJointValues.connectedBody;
			m_hingeJoint.anchor = m_originalHingeJointValues.anchor;
			m_hingeJoint.axis = m_originalHingeJointValues.axis;
			m_hingeJoint.connectedAnchor = m_originalHingeJointValues.connectedAnchor;
			m_hingeJoint.motor = m_originalHingeJointValues.motor;
			m_hingeJoint.useMotor = m_originalHingeJointValues.useMotor;
			m_hingeJoint.spring = m_originalHingeJointValues.spring;
			m_hingeJoint.useSpring = m_originalHingeJointValues.useSpring;
			m_hingeJoint.limits = m_originalHingeJointValues.limits;
			m_hingeJoint.useLimits = m_originalHingeJointValues.useLimits;
			m_hingeJoint.breakForce = m_originalHingeJointValues.breakForce;
			m_hingeJoint.breakTorque = m_originalHingeJointValues.breakTorque;
			m_hingeJoint.enableCollision = m_originalHingeJointValues.enableCollision;
			m_hingeJoint.enablePreprocessing = m_originalHingeJointValues.enablePreprocessing;
		}
	}

	private void ResetEffects()
	{
		if (!(breakEffect == null))
		{
			breakEffect.transform.parent = base.transform;
			breakEffect.transform.localPosition = Vector3.zero;
			breakEffect.Stop();
		}
	}

	private void UpdateIceCollider()
	{
		if (!m_createIceColliders)
		{
			return;
		}
		if (m_iceColliders == null)
		{
			m_normalColliders = new List<Collider>(base.gameObject.GetComponentsInChildren<Collider>(includeInactive: true));
			m_iceColliders = new List<Transform>();
			for (int i = 0; i < m_normalColliders.Count; i++)
			{
				if (m_normalColliders[i].gameObject.layer == LayerMask.NameToLayer("Light") || m_normalColliders[i].gameObject.layer == LayerMask.NameToLayer("Mouth") || m_normalColliders[i].isTrigger)
				{
					m_normalColliders.RemoveAt(i--);
					continue;
				}
				GameObject obj = new GameObject("IceCollider_" + i);
				obj.transform.parent = m_normalColliders[i].transform;
				obj.transform.localScale = Vector3.one;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localRotation = Quaternion.identity;
				obj.layer = m_icePartLayer;
				Collider collider = obj.AddComponent(m_normalColliders[i].GetType()) as Collider;
				if (collider != null)
				{
					m_iceColliders.Add(collider.transform);
				}
			}
		}
		bool flag = false;
		foreach (Transform iceCollider in m_iceColliders)
		{
			if (iceCollider.GetComponent<Collider>() != null)
			{
				Object.Destroy(iceCollider.GetComponent<Collider>());
				flag = true;
			}
		}
		for (int j = 0; j < m_iceColliders.Count; j++)
		{
			if (m_normalColliders[j] is SphereCollider)
			{
				SphereCollider sphereCollider = m_iceColliders[j].GetComponent<Collider>() as SphereCollider;
				if (sphereCollider == null || flag)
				{
					sphereCollider = m_iceColliders[j].gameObject.AddComponent<SphereCollider>();
					sphereCollider.radius = ((!(m_normalColliders[j] as SphereCollider == null)) ? (m_normalColliders[j] as SphereCollider).radius : 0.5f);
					sphereCollider.center = ((!(m_normalColliders[j] as SphereCollider == null)) ? (m_normalColliders[j] as SphereCollider).center : Vector3.zero);
				}
				if (sphereCollider != null)
				{
					sphereCollider.sharedMaterial = m_iceMaterial;
				}
			}
			else if (m_normalColliders[j] is BoxCollider)
			{
				BoxCollider boxCollider = m_iceColliders[j].GetComponent<Collider>() as BoxCollider;
				if (boxCollider == null || flag)
				{
					boxCollider = m_iceColliders[j].gameObject.AddComponent<BoxCollider>();
					boxCollider.size = (m_normalColliders[j] as BoxCollider).size;
					boxCollider.center = (m_normalColliders[j] as BoxCollider).center;
				}
				if (boxCollider != null)
				{
					boxCollider.sharedMaterial = m_iceMaterial;
				}
			}
			else if (m_normalColliders[j] is CapsuleCollider)
			{
				CapsuleCollider capsuleCollider = m_iceColliders[j].GetComponent<Collider>() as CapsuleCollider;
				if (capsuleCollider == null || flag)
				{
					capsuleCollider = m_iceColliders[j].gameObject.AddComponent<CapsuleCollider>();
					capsuleCollider.center = (m_normalColliders[j] as CapsuleCollider).center;
					capsuleCollider.radius = (m_normalColliders[j] as CapsuleCollider).radius;
					capsuleCollider.height = (m_normalColliders[j] as CapsuleCollider).height;
					capsuleCollider.direction = (m_normalColliders[j] as CapsuleCollider).direction;
				}
				if (capsuleCollider != null)
				{
					capsuleCollider.sharedMaterial = m_iceMaterial;
				}
			}
		}
	}

	protected void OnCollisionEnter(Collision c)
	{
		float num = 0f;
		ContactPoint[] contacts = c.contacts;
		foreach (ContactPoint contactPoint in contacts)
		{
			float num2 = Vector3.Dot(c.relativeVelocity, contactPoint.normal);
			if (num2 > num)
			{
				num = num2;
			}
		}
		if (!breakOnlyByGoldenPig && !breakOnlyByBird)
		{
			Break(num);
		}
		if (!lockPosition && Time.time - m_lastTimePlayedSFX > 0.25f)
		{
			PlayCollisionSFX(c);
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		GoldenPig component = other.GetComponent<GoldenPig>();
		Bird component2 = other.GetComponent<Bird>();
		if ((breakOnlyByGoldenPig && (bool)component) || (breakOnlyByBird && (bool)component2))
		{
			Break(1f);
		}
	}

	public void Break(float collisionSpeed)
	{
		if ((m_broken || lockPosition || !(collisionSpeed > breakForce)) && (!lockPosition || !(m_fixedJoint == null)))
		{
			return;
		}
		m_broken = true;
		if (breakEffect != null)
		{
			breakEffect.transform.parent = null;
			breakEffect.Play();
		}
		PlayBreakSFX();
		base.rigidbody.isKinematic = true;
		if (chainReactionBreaking)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, 2f);
			for (int i = 0; i < array.Length; i++)
			{
				LevelRigidbody component = array[i].GetComponent<LevelRigidbody>();
				if ((bool)component)
				{
					component.Break(collisionSpeed);
				}
			}
		}
		base.transform.position = -Vector3.up * 1000f;
	}

	protected void PlayBreakSFX()
	{
		AudioSource[] array = null;
		if (isRock)
		{
			array = WPFMonoBehaviour.gameData.commonAudioCollection.collisionRockBreak;
		}
		if (array != null)
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(array, base.transform.position);
		}
	}

	protected void PlayCollisionSFX(Collision collisionData)
	{
		AudioSource[] array = null;
		AudioSource[] array2 = null;
		switch (audioMaterial)
		{
		default:
			array = null;
			array2 = null;
			break;
		case AudioManager.AudioMaterial.Wood:
			array = WPFMonoBehaviour.gameData.commonAudioCollection.collisionWoodHit;
			array2 = WPFMonoBehaviour.gameData.commonAudioCollection.collisionWoodDamage;
			break;
		case AudioManager.AudioMaterial.Metal:
			array = WPFMonoBehaviour.gameData.commonAudioCollection.collisionMetalHit;
			array2 = WPFMonoBehaviour.gameData.commonAudioCollection.collisionMetalDamage;
			break;
		}
		if (array == null || array2 == null)
		{
			return;
		}
		float num = 0f;
		Vector3 soundPosition = Vector3.zero;
		foreach (ContactPoint collisionDatum in collisionData)
		{
			float num2 = Vector3.Dot(collisionData.relativeVelocity, collisionDatum.normal);
			if (num2 > num)
			{
				num = num2;
				soundPosition = collisionDatum.point;
			}
		}
		if (num > 8f)
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(array2, soundPosition);
			m_lastTimePlayedSFX = Time.time;
		}
		else if (num > 2.5f)
		{
			AudioSource audioSource = Singleton<AudioManager>.Instance.SpawnOneShotEffect(array, soundPosition);
			m_lastTimePlayedSFX = Time.time;
			if ((bool)audioSource)
			{
				audioSource.volume = (num - 2f) / 8f;
			}
		}
	}
}
