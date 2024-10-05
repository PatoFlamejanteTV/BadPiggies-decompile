using System.Collections.Generic;
using UnityEngine;

public class AncestorPig : WPFMonoBehaviour
{
	public GameObject childPrefab;

	public GameObject childObject;

	[SerializeField]
	private GameObject childPos;

	private int[] blinkTimes = new int[41]
	{
		2, 2, 0, 1, 2, 0, 1, 2, 1, 0,
		2, 1, 2, 0, 2, 2, 2, 3, 2, 1,
		2, 0, 1, 2, 1, 0, 2, 2, 2, 0,
		2, 1, 2, 0, 2, 1, 2, 0, 1, 1,
		3
	};

	private int currentBlinkTime;

	private GameObject graphics;

	private GameObject eyes;

	private GameObject pupilMover;

	private GameObject pig;

	private ParticleSystem disappearEffect;

	private bool isSeen;

	private SpriteAnimation eyeAnimation;

	private float blinkTimer;

	private List<Vector3> collisionPoints;

	private Transform cachedTransform;

	private void Start()
	{
		cachedTransform = base.transform;
		disappearEffect = base.transform.Find("DisappearEffect").GetComponent<ParticleSystem>();
		childPos = base.transform.Find("ChildPos").gameObject;
		graphics = base.transform.Find("Graphics").gameObject;
		eyes = graphics.transform.Find("Eyes").gameObject;
		pupilMover = graphics.transform.Find("PupilMover").gameObject;
		eyeAnimation = eyes.GetComponent<SpriteAnimation>();
		blinkTimer = Random.Range(3f, 6f);
		EventManager.Connect<GameStateChanged>(OnGameStateChanged);
		if ((bool)GetComponent<Collider>())
		{
			float x = GetComponent<Collider>().bounds.extents.x;
			float y = GetComponent<Collider>().bounds.extents.y;
			collisionPoints = new List<Vector3>();
			collisionPoints.Add(base.transform.position);
			collisionPoints.Add(base.transform.TransformPoint(Vector3.right * x));
			collisionPoints.Add(base.transform.TransformPoint(Vector3.right * (0f - x)));
			collisionPoints.Add(base.transform.TransformPoint(Vector3.up * y));
			collisionPoints.Add(base.transform.TransformPoint(Vector3.up * (0f - y)));
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
	}

	private void Update()
	{
		if (isSeen)
		{
			return;
		}
		cachedTransform.position += Vector3.zero;
		graphics.SetActive(value: true);
		GetComponent<Collider>().enabled = true;
		if (!eyeAnimation)
		{
			return;
		}
		blinkTimer -= Time.deltaTime;
		if (blinkTimer <= 0f)
		{
			switch (blinkTimes[currentBlinkTime])
			{
			case 1:
				eyeAnimation.Play("FastBlink");
				blinkTimer = 0.4f;
				break;
			case 2:
				eyeAnimation.Play("SlowBlink");
				blinkTimer = 0.9f;
				break;
			default:
				eyeAnimation.Play("Normal");
				blinkTimer = Random.Range(3f, 6f);
				break;
			}
			currentBlinkTime++;
			if (currentBlinkTime == blinkTimes.Length)
			{
				currentBlinkTime = 0;
			}
		}
		else
		{
			eyeAnimation.Play("Normal");
		}
		if (pig != null)
		{
			Vector3 vector = pig.transform.position - base.transform.position;
			if (vector.sqrMagnitude > 1f)
			{
				vector.Normalize();
			}
			pupilMover.transform.localPosition = 0.035f * vector;
		}
	}

	private void OnGameStateChanged(GameStateChanged newState)
	{
		if (newState.state == LevelManager.GameState.Building)
		{
			Recreate();
		}
		if (newState.state == LevelManager.GameState.Running)
		{
			Recreate();
			pig = WPFMonoBehaviour.levelManager.ContraptionRunning.FindPart(BasePart.PartType.Pig).gameObject;
		}
	}

	private void Recreate()
	{
		CreateChild();
		graphics.SetActive(value: false);
		GetComponent<Collider>().enabled = false;
		if (disappearEffect != null)
		{
			disappearEffect.Stop();
		}
		isSeen = false;
		blinkTimer = Random.Range(3f, 6f);
	}

	private void OnTriggerEnter(Collider c)
	{
		CheckIfSeen(c);
	}

	private void OnTriggerStay(Collider c)
	{
		CheckIfSeen(c);
	}

	private void CheckIfSeen(Collider c)
	{
		LightTrigger component = c.GetComponent<LightTrigger>();
		if (!component)
		{
			return;
		}
		PointLightSource lightSource = component.LightSource;
		if (!lightSource.isEnabled)
		{
			return;
		}
		if ((bool)lightSource && lightSource.lightType == PointLightMask.LightType.PointLight)
		{
			Disappear();
		}
		else
		{
			if (lightSource.lightType != PointLightMask.LightType.BeamLight)
			{
				return;
			}
			if (Vector3.Distance(base.transform.position, lightSource.beamArcCenter) < lightSource.colliderSize)
			{
				Disappear();
				return;
			}
			float beamAngle = lightSource.beamAngle;
			Vector3 vector = Vector3.up * c.transform.position.y + Vector3.right * c.transform.position.x;
			if (collisionPoints != null && collisionPoints.Count > 0)
			{
				foreach (Vector3 collisionPoint in collisionPoints)
				{
					float num = Vector3.Angle(collisionPoint - vector, lightSource.transform.up);
					if (Vector3.Distance(collisionPoint, vector) <= lightSource.baseLightSize + lightSource.borderWidth || num < beamAngle * 0.5f)
					{
						Disappear();
					}
				}
				return;
			}
			Vector3 vector2 = Vector3.up * base.transform.position.y + Vector3.right * base.transform.position.x;
			float num2 = Vector3.Angle(vector2 - vector, lightSource.transform.up);
			if (Vector3.Distance(vector2, vector) <= lightSource.baseLightSize + lightSource.borderWidth || num2 < beamAngle * 0.5f)
			{
				Disappear();
			}
		}
	}

	private void Disappear()
	{
		if ((bool)childObject && (bool)childObject.GetComponent<Rigidbody>())
		{
			childObject.GetComponent<Rigidbody>().isKinematic = false;
			childObject.GetComponent<Rigidbody>().WakeUp();
		}
		if (graphics.activeInHierarchy)
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.ancientPigDisappear, base.transform.position);
		}
		graphics.SetActive(value: false);
		GetComponent<Collider>().enabled = false;
		disappearEffect.Play();
		isSeen = true;
		CheckForAncestorPigAchievement();
	}

	public void DestroyChildren()
	{
		if ((bool)childObject && (bool)childObject.GetComponent<Goal>())
		{
			return;
		}
		if (childObject != null)
		{
			childObject.transform.parent = null;
			Object.Destroy(childObject);
		}
		if (childPos == null)
		{
			childPos = base.transform.Find("ChildPos").gameObject;
		}
		if (childPos.transform.childCount > 0)
		{
			for (int i = 0; i < childPos.transform.childCount; i++)
			{
				Transform child = childPos.transform.GetChild(i);
				child.parent = null;
				Object.Destroy(child.gameObject);
			}
		}
	}

	private void CreateChild()
	{
		DestroyChildren();
		if (childPrefab == null)
		{
			float radius = ((SphereCollider)GetComponent<Collider>()).radius;
			LayerMask layerMask = 1 << LayerMask.NameToLayer("Light");
			if (Physics.SphereCast(base.transform.position - Vector3.forward * 2f, radius, Vector3.forward * 3f, out var hitInfo, float.MaxValue, ~layerMask.value))
			{
				Challenge componentInParent = hitInfo.collider.GetComponentInParent<Challenge>();
				if ((bool)componentInParent)
				{
					childObject = componentInParent.gameObject;
					if ((bool)childObject.GetComponent<Rigidbody>())
					{
						childObject.GetComponent<Rigidbody>().isKinematic = true;
					}
					childObject.transform.rotation = Quaternion.identity;
				}
			}
		}
		else
		{
			if (childPos == null)
			{
				childPos = base.transform.Find("ChildPos").gameObject;
			}
			childObject = Object.Instantiate(childPrefab);
			childObject.name = childPrefab.name;
			childObject.transform.parent = childPos.transform;
			childObject.transform.localScale = Vector3.one;
			childObject.transform.localRotation = Quaternion.identity;
			if ((bool)childObject.GetComponent<Rigidbody>())
			{
				childObject.GetComponent<Rigidbody>().isKinematic = true;
			}
		}
		if ((bool)childObject)
		{
			childObject.transform.position = base.transform.position + Vector3.up * (0f - childObject.GetComponent<Collider>().bounds.extents.y);
		}
	}

	public void CheckForAncestorPigAchievement()
	{
		if (Singleton<SocialGameManager>.IsInstantiated() && Singleton<GameManager>.Instance.IsInGame())
		{
			int revealedAncestorPigs = GameProgress.GetInt("Revealed_Ancestor_Pigs") + 1;
			GameProgress.SetInt("Revealed_Ancestor_Pigs", revealedAncestorPigs);
			Singleton<SocialGameManager>.Instance.TryReportAchievementProgress("grp.SOMEONES_THERE", 100.0, (int limit) => revealedAncestorPigs > limit);
		}
	}

	private void OnDrawGizmos()
	{
	}
}
