using System;
using UnityEngine;

public class ObjectiveSlot : MonoBehaviour
{
	private GameObject m_succeededImage;

	private bool bShrinking;

	private bool bGrowing;

	private float animTimer;

	private void Awake()
	{
		m_succeededImage = base.transform.Find("Succeeded").gameObject;
		m_succeededImage.GetComponent<Renderer>().enabled = false;
		bShrinking = false;
		bGrowing = false;
	}

	public void SetSucceeded()
	{
		bGrowing = true;
		animTimer = 0.2f;
	}

	public void SetSucceededImmediate()
	{
		GetComponent<Renderer>().enabled = false;
		m_succeededImage.GetComponent<Renderer>().enabled = true;
	}

	public void SetChallenge(Challenge challenge)
	{
		foreach (Challenge.IconPlacement icon in challenge.Icons)
		{
			GameObject obj = UnityEngine.Object.Instantiate(icon.icon);
			obj.transform.parent = base.transform;
			obj.transform.localPosition = icon.position;
			Material sharedMaterial = obj.GetComponent<Renderer>().sharedMaterial;
			obj.GetComponent<Renderer>().sharedMaterial = AtlasMaterials.Instance.GetCachedMaterialInstance(sharedMaterial, AtlasMaterials.MaterialType.PartZ);
			obj.transform.localScale = new Vector3(icon.scale, icon.scale, 1f);
			TimeChallenge timeChallenge = challenge as TimeChallenge;
			if ((bool)timeChallenge)
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(timeChallenge.TimeLimit());
				string text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
				TextMesh[] componentsInChildren = GetComponentsInChildren<TextMesh>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].text = text;
				}
			}
		}
	}

	public void SetChallenge(GoalChallenge challenge)
	{
		GameObject obj = UnityEngine.Object.Instantiate(challenge.Icon.icon);
		obj.transform.parent = base.transform;
		obj.transform.localPosition = challenge.Icon.position;
		obj.transform.localScale = new Vector3(challenge.Icon.scale, challenge.Icon.scale, 1f);
		Material sharedMaterial = obj.GetComponent<Renderer>().sharedMaterial;
		obj.GetComponent<Renderer>().sharedMaterial = AtlasMaterials.Instance.GetCachedMaterialInstance(sharedMaterial, AtlasMaterials.MaterialType.PartZ);
	}

	public void ShowSnoutReward(bool show = true, int count = 0, bool parentToParent = true)
	{
		Transform transform = base.transform.Find("SnoutReward");
		if (transform != null)
		{
			transform.gameObject.SetActive(show);
			if (show)
			{
				for (int i = 0; i < 5; i++)
				{
					Transform transform2 = transform.Find($"SnoutReward{i + 1:00}");
					if ((bool)transform2)
					{
						transform2.gameObject.SetActive(i < count);
					}
				}
			}
		}
		if (parentToParent)
		{
			transform.parent = transform.parent.parent;
		}
	}

	private void Update()
	{
		if (bGrowing)
		{
			if (animTimer > 0f)
			{
				base.transform.localScale += Vector3.one * Time.deltaTime;
				animTimer -= Time.deltaTime;
				return;
			}
			bGrowing = false;
			bShrinking = true;
			animTimer = 0.2f;
			GetComponent<Renderer>().enabled = false;
			m_succeededImage.GetComponent<Renderer>().enabled = true;
		}
		else
		{
			if (!bShrinking)
			{
				return;
			}
			if (animTimer > 0f)
			{
				if (base.transform.localScale.x - Time.deltaTime >= 1f)
				{
					base.transform.localScale -= Vector3.one * Time.deltaTime;
				}
				animTimer -= Time.deltaTime;
			}
			else
			{
				bShrinking = false;
				animTimer = 0f;
			}
		}
	}
}
