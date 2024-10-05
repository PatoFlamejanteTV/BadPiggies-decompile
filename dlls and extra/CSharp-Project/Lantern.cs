using System;
using UnityEngine;

public class Lantern : PointLight
{
	[SerializeField]
	private float mainSinSpeed;

	[SerializeField]
	private float subSinSpeed;

	[SerializeField]
	private float flicker;

	private float originalSize;

	private bool enableFlicker;

	private Transform lightTransform;

	private SphereCollider lightCollider;

	public override void Awake()
	{
		base.Awake();
		PointLightSource pointLightSource = lightSource;
		pointLightSource.onLightTurnOff = (Action)Delegate.Combine(pointLightSource.onLightTurnOff, new Action(OnLightOff));
		PointLightSource pointLightSource2 = lightSource;
		pointLightSource2.onLightTurnOn = (Action)Delegate.Combine(pointLightSource2.onLightTurnOn, new Action(OnLightOn));
	}

	private void OnDestroy()
	{
		if (!(lightSource == null))
		{
			PointLightSource pointLightSource = lightSource;
			pointLightSource.onLightTurnOff = (Action)Delegate.Remove(pointLightSource.onLightTurnOff, new Action(OnLightOff));
			PointLightSource pointLightSource2 = lightSource;
			pointLightSource2.onLightTurnOn = (Action)Delegate.Remove(pointLightSource2.onLightTurnOn, new Action(OnLightOn));
		}
	}

	private void OnLightOff()
	{
		enableFlicker = false;
	}

	private void OnLightOn()
	{
		enableFlicker = true;
		originalSize = lightSource.size;
		if (lightTransform == null)
		{
			lightTransform = lightSource.lightTransform;
		}
		if (lightCollider == null)
		{
			lightCollider = GetComponentInChildren<SphereCollider>();
		}
	}

	private void Update()
	{
		if (enableFlicker && !(lightTransform == null))
		{
			float num = Mathf.Sin(Time.realtimeSinceStartup * mainSinSpeed) * Mathf.Sin(Time.realtimeSinceStartup * subSinSpeed) * flicker;
			lightTransform.localScale = new Vector3(originalSize + num, originalSize + num, lightTransform.localScale.z);
			lightCollider.radius = originalSize + num;
		}
	}
}
