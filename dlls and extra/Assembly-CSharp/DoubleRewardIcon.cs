using System;
using System.Collections;
using UnityEngine;

public class DoubleRewardIcon : MonoBehaviour
{
	[SerializeField]
	private TextMesh[] timeFields;

	[SerializeField]
	private MeshRenderer[] renderers;

	private int prevHours;

	private int prevMinutes;

	private int prevSeconds;

	private static DoubleRewardIcon instance;

	public static DoubleRewardIcon Instance => instance;

	private void Awake()
	{
		if (instance == null || instance == this)
		{
			renderers = GetComponentsInChildren<MeshRenderer>();
			instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnEnable()
	{
		StartCoroutine(UpdateTimeField());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator UpdateTimeField()
	{
		while (base.enabled)
		{
			float doubleRewardTimeRemaining = Singleton<DoubleRewardManager>.Instance.DoubleRewardTimeRemaining;
			if (doubleRewardTimeRemaining <= 0f)
			{
				base.gameObject.SetActive(value: false);
				break;
			}
			TimeSpan time = TimeSpan.FromSeconds(doubleRewardTimeRemaining);
			if (NeedsUpdate(time))
			{
				string text = ((time.Hours > 0) ? $"{time.Hours}h {time.Minutes}m {time.Seconds}s" : ((time.Minutes <= 0) ? $"{time.Seconds}s" : $"{time.Minutes}m {time.Seconds}s"));
				for (int i = 0; i < timeFields.Length; i++)
				{
					timeFields[i].text = text;
				}
			}
			yield return null;
		}
	}

	private bool NeedsUpdate(TimeSpan time)
	{
		bool result = time.Hours != prevHours || time.Minutes != prevMinutes || time.Seconds != prevSeconds;
		prevHours = time.Hours;
		prevMinutes = time.Minutes;
		prevSeconds = time.Seconds;
		return result;
	}

	public void SetSortingLayer(string layer)
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].sortingLayerName = layer;
		}
	}
}
