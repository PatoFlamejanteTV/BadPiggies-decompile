using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class PointLightSource : MonoBehaviour
{
	public PointLightMask.LightType lightType;

	public float size = 1f;

	public float borderWidth = 0.3f;

	[HideInInspector]
	public float beamAngle = 45f;

	[HideInInspector]
	public float beamCut = 0.5f;

	public int vertexCount = 100;

	[HideInInspector]
	public float colliderSize;

	[HideInInspector]
	public Vector3 beamArcCenter;

	public Transform lightTransform;

	[HideInInspector]
	public Transform baseLightTransform;

	[HideInInspector]
	public float baseLightSize = 0.5f;

	public AnimationCurve turnOnCurve = AnimationCurve.Linear(0f, 0f, 0.3f, 1f);

	public AnimationCurve turnOffCurve = AnimationCurve.Linear(0f, 1f, 0.3f, 0f);

	public Action onLightTurnOff;

	public Action onLightTurnOn;

	[HideInInspector]
	[SerializeField]
	private bool _isEnabled;

	[SerializeField]
	private float minColliderSize = 0.2f;

	public bool canLitObjects;

	public bool canCollide;

	public bool canBeLit;

	public bool usesCurves = true;

	private SphereCollider sphereCollider;

	private SphereCollider sphereCollider2;

	private SphereCollider baseLightCollider;

	private LightTrigger lt;

	public bool isEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			_isEnabled = value;
			if (_isEnabled)
			{
				ResetLayer();
			}
			Toggle();
		}
	}

	public void Init()
	{
		lightTransform.localPosition = ((!isEnabled) ? new Vector3(lightTransform.localPosition.x, lightTransform.localPosition.y, -100f) : new Vector3(lightTransform.localPosition.x, lightTransform.localPosition.y, 0f));
		if ((bool)baseLightTransform)
		{
			baseLightTransform.localPosition = ((!isEnabled) ? new Vector3(baseLightTransform.localPosition.x, baseLightTransform.localPosition.y, -100f) : new Vector3(baseLightTransform.localPosition.x, baseLightTransform.localPosition.y, 0f));
		}
		if (!canCollide)
		{
			return;
		}
		lt = GetComponentInChildren<LightTrigger>();
		if (lt == null)
		{
			GameObject gameObject = new GameObject(base.transform.name + "_LightCollider");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.layer = LayerMask.NameToLayer("Light");
			if (gameObject.GetComponent<Rigidbody>() == null && base.gameObject.isStatic && canCollide)
			{
				gameObject.AddComponent<Rigidbody>();
				gameObject.GetComponent<Rigidbody>().isKinematic = true;
				gameObject.GetComponent<Rigidbody>().useGravity = false;
			}
			if (lightType == PointLightMask.LightType.PointLight)
			{
				sphereCollider = gameObject.AddComponent<SphereCollider>();
				sphereCollider.radius = ((!isEnabled) ? minColliderSize : colliderSize);
				sphereCollider.isTrigger = true;
			}
			else if (lightType == PointLightMask.LightType.BeamLight)
			{
				sphereCollider = gameObject.AddComponent<SphereCollider>();
				sphereCollider.radius = ((!isEnabled) ? minColliderSize : colliderSize);
				sphereCollider.isTrigger = true;
				sphereCollider.center = Vector3.up * beamArcCenter.y;
				sphereCollider2 = gameObject.AddComponent<SphereCollider>();
				sphereCollider2.radius = ((!isEnabled) ? minColliderSize : (size / 2f));
				sphereCollider2.isTrigger = true;
				sphereCollider2.center = Vector3.up * (size / 2f);
				baseLightCollider = gameObject.AddComponent<SphereCollider>();
				baseLightCollider.radius = ((!isEnabled) ? minColliderSize : (baseLightSize + 0.5f));
				baseLightCollider.isTrigger = true;
				baseLightCollider.center = Vector3.zero;
			}
			lt = gameObject.AddComponent<LightTrigger>();
			lt.Init(this);
		}
		else
		{
			if (lightType == PointLightMask.LightType.PointLight)
			{
				sphereCollider = lt.gameObject.GetComponent<SphereCollider>();
				sphereCollider.radius = ((!isEnabled) ? minColliderSize : colliderSize);
				sphereCollider.isTrigger = true;
				sphereCollider.gameObject.layer = LayerMask.NameToLayer("Light");
			}
			else if (lightType == PointLightMask.LightType.BeamLight)
			{
				SphereCollider[] components = lt.gameObject.GetComponents<SphereCollider>();
				sphereCollider = components[0];
				sphereCollider2 = components[1];
				baseLightCollider = components[2];
				sphereCollider.radius = ((!isEnabled) ? minColliderSize : colliderSize);
				sphereCollider.isTrigger = true;
				sphereCollider.center = beamArcCenter;
				sphereCollider2.radius = ((!isEnabled) ? minColliderSize : (size / 2f));
				sphereCollider2.isTrigger = true;
				sphereCollider2.center = Vector3.up * (size / 2f);
				baseLightCollider.radius = ((!isEnabled) ? minColliderSize : (baseLightSize + 0.5f));
				baseLightCollider.isTrigger = true;
				baseLightCollider.center = Vector3.zero;
			}
			lt.Init(this);
		}
		RefreshColliderSizes(isEnabled);
	}

	private void RefreshColliderSizes(bool _enabled)
	{
		if (!canCollide)
		{
			return;
		}
		if (lightType == PointLightMask.LightType.PointLight)
		{
			if (sphereCollider != null)
			{
				sphereCollider.radius = ((!_enabled) ? minColliderSize : colliderSize);
			}
		}
		else
		{
			if (lightType != PointLightMask.LightType.BeamLight)
			{
				return;
			}
			if (sphereCollider != null)
			{
				sphereCollider.radius = ((!_enabled) ? minColliderSize : colliderSize);
			}
			if (sphereCollider2 != null)
			{
				sphereCollider2.radius = ((!_enabled) ? minColliderSize : (size / 2f));
			}
			if (baseLightCollider != null)
			{
				baseLightCollider.radius = ((!_enabled) ? minColliderSize : (baseLightSize + 0.5f));
			}
			if (!canBeLit)
			{
				if (sphereCollider != null)
				{
					sphereCollider.enabled = _enabled;
				}
				if (sphereCollider2 != null)
				{
					sphereCollider2.enabled = _enabled;
				}
				if (baseLightCollider != null)
				{
					baseLightCollider.enabled = _enabled;
				}
			}
		}
	}

	private void ResetLayer()
	{
		if (sphereCollider != null)
		{
			sphereCollider.gameObject.layer = LayerMask.NameToLayer("Light");
		}
	}

	private void Toggle()
	{
		if (lightTransform != null)
		{
			StartCoroutine(LitSequence());
		}
	}

	private IEnumerator LitSequence()
	{
		float duration;
		if (isEnabled)
		{
			duration = turnOnCurve[turnOnCurve.length - 1].time;
			lightTransform.localPosition = new Vector3(lightTransform.localPosition.x, lightTransform.localPosition.y, 0f);
			if ((bool)baseLightTransform)
			{
				baseLightTransform.localPosition = new Vector3(baseLightTransform.localPosition.x, baseLightTransform.localPosition.y, 0f);
			}
		}
		else
		{
			duration = turnOffCurve[turnOffCurve.length - 1].time;
		}
		RefreshColliderSizes(!isEnabled);
		float timer = 0f;
		if (canCollide && sphereCollider == null)
		{
			Debug.LogWarning(base.transform.name + " collider is NULL");
		}
		while (usesCurves && timer < duration)
		{
			float num = ((!_isEnabled) ? turnOffCurve.Evaluate(timer) : turnOnCurve.Evaluate(timer));
			if (lightType == PointLightMask.LightType.PointLight)
			{
				lightTransform.localScale = Vector3.up * num * size + Vector3.right * num * size;
				if (sphereCollider != null)
				{
					sphereCollider.radius = num * colliderSize;
				}
			}
			else
			{
				lightTransform.localScale = Vector3.up * num + Vector3.right * num;
				baseLightTransform.localScale = Vector3.up * num * baseLightSize + Vector3.right * num * baseLightSize;
				if (sphereCollider != null)
				{
					sphereCollider.radius = num * colliderSize;
				}
				if (sphereCollider2 != null)
				{
					sphereCollider2.radius = num * (size / 2f);
				}
				if (baseLightCollider != null)
				{
					baseLightCollider.radius = num * (baseLightSize + 0.5f);
				}
			}
			if ((bool)lt)
			{
				lt.transform.position += Vector3.zero;
			}
			timer += Time.deltaTime;
			yield return null;
		}
		if (lightType == PointLightMask.LightType.PointLight)
		{
			lightTransform.localScale = Vector3.up * size + Vector3.right * size;
		}
		else
		{
			lightTransform.localScale = Vector3.up + Vector3.right;
			baseLightTransform.localScale = Vector3.up * baseLightSize + Vector3.right * baseLightSize;
		}
		RefreshColliderSizes(isEnabled);
		if (!isEnabled)
		{
			lightTransform.localPosition = new Vector3(lightTransform.localPosition.x, lightTransform.localPosition.y, -100f);
			if ((bool)baseLightTransform)
			{
				baseLightTransform.localPosition = new Vector3(baseLightTransform.localPosition.x, baseLightTransform.localPosition.y, 100f);
			}
			if (onLightTurnOff != null)
			{
				onLightTurnOff();
			}
		}
		else if (isEnabled && onLightTurnOn != null)
		{
			onLightTurnOn();
		}
	}

	private void OnDrawGizmos()
	{
		if (lightType == PointLightMask.LightType.PointLight)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.position, size + borderWidth);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, minColliderSize);
		}
		else if (lightType == PointLightMask.LightType.BeamLight)
		{
			Gizmos.color = Color.white;
			Vector3 vector = Quaternion.AngleAxis(0f - beamAngle / 2f, base.transform.forward) * base.transform.up;
			Vector3 vector2 = Quaternion.AngleAxis(beamAngle - beamAngle / 2f, base.transform.forward) * base.transform.up;
			Gizmos.DrawLine(base.transform.position, base.transform.position + vector * size);
			Gizmos.DrawLine(base.transform.position, base.transform.position + vector2 * size);
			Gizmos.color = Color.green;
			float num = beamAngle * 0.6f;
			vector = Quaternion.AngleAxis(0f - num, base.transform.forward) * base.transform.up;
			vector2 = Quaternion.AngleAxis(num * 2f - num, base.transform.forward) * base.transform.up;
			Gizmos.DrawLine(base.transform.position, base.transform.position + vector * size);
			Gizmos.DrawLine(base.transform.position, base.transform.position + vector2 * size);
		}
	}
}
