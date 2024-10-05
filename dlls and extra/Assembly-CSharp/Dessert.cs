using System;
using UnityEngine;

public class Dessert : OneTimeCollectable
{
	public struct DessertCollectedEvent : EventManager.Event
	{
		public Dessert dessert;

		public DessertCollectedEvent(Dessert dessert)
		{
			this.dessert = dessert;
		}
	}

	public GameObject prefabGlow;

	public GameObject prefabIcon;

	public string saveId;

	[NonSerialized]
	public DessertPlace place;

	private void Awake()
	{
		if ((bool)prefabGlow && base.transform.childCount == 0)
		{
			UnityEngine.Object.Instantiate(prefabGlow, base.transform.position + prefabGlow.transform.localPosition, prefabGlow.transform.localRotation * base.transform.rotation).transform.parent = base.transform;
		}
	}

	public override void OnCollected()
	{
		base.OnCollected();
		EventManager.Send(new DessertCollectedEvent(this));
	}

	public override void Collect()
	{
		if (!collected)
		{
			if ((bool)collectedEffect)
			{
				UnityEngine.Object.Instantiate(collectedEffect, base.transform.position, base.transform.rotation);
			}
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.dessertCollected);
			collected = true;
			DisableGoal();
			EventManager.Send(default(ObjectiveAchieved));
			OnCollected();
		}
	}

	protected override string GetNameKey()
	{
		return string.Empty;
	}
}
