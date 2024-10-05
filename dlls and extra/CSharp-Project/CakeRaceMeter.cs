using System.Collections;
using Spine.Unity;
using UnityEngine;

public class CakeRaceMeter : MonoBehaviour
{
	[SerializeField]
	private string introAnimationName;

	[SerializeField]
	private string idleAnimationName;

	[SerializeField]
	private string eatAnimationName;

	[SerializeField]
	private TextMesh[] scoreLabel;

	[SerializeField]
	private TextMesh[] nameLabel;

	[SerializeField]
	private TextMesh[] levelLabel;

	private SkeletonAnimation[] cakes;

	private int currentCake;

	public void EatCake()
	{
		if (currentCake >= 0 && currentCake < cakes.Length)
		{
			cakes[currentCake].state.SetAnimation(0, eatAnimationName, loop: false);
			currentCake++;
		}
	}

	public void ResetCakes()
	{
		currentCake = 0;
		if (cakes == null)
		{
			StartCoroutine(Init());
			return;
		}
		for (int num = 4; num >= 0; num--)
		{
			if (cakes[num] != null)
			{
				StartCoroutine(DelayAnimation(cakes[num], 1f + 0.2f * (float)num));
			}
		}
	}

	private IEnumerator DelayAnimation(SkeletonAnimation anim, float seconds)
	{
		Renderer rend = anim.GetComponent<Renderer>();
		rend.enabled = false;
		yield return new WaitForSeconds(seconds);
		anim.state.SetAnimation(0, introAnimationName, loop: false);
		anim.state.AddAnimation(0, idleAnimationName, loop: true, 0f);
		yield return null;
		rend.enabled = true;
	}

	private IEnumerator Init()
	{
		yield return null;
		cakes = new SkeletonAnimation[5];
		for (int i = 0; i < 5; i++)
		{
			Transform transform = base.transform.Find($"Cakes/Cake{i}");
			if (transform != null)
			{
				cakes[i] = transform.GetComponent<SkeletonAnimation>();
			}
		}
		ResetCakes();
	}

	public void SetScoreLabel(string text)
	{
		TextMeshHelper.UpdateTextMeshes(scoreLabel, text);
	}

	public void SetPlayerInfo(string name, int level, bool refreshTranslation = false)
	{
		if (string.IsNullOrEmpty(name))
		{
			name = "anonpig";
		}
		TextMeshHelper.UpdateTextMeshes(nameLabel, name, refreshTranslation);
		TextMeshHelper.UpdateTextMeshes(levelLabel, Mathf.Clamp(level, 0, 999).ToString(), refreshTranslation);
	}
}
