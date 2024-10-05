using System;
using System.Collections;
using UnityEngine;

public class LeaderboardCupInfo : MonoBehaviour
{
	private Transform[] cups;

	private LeaderboardDialog leaderboardDialog;

	private bool initialized;

	private void OnEnable()
	{
		Init();
		if (!(leaderboardDialog != null) || !leaderboardDialog.ShowingCupAnimation)
		{
			int currentLeaderboardCup = (int)CakeRaceMenu.GetCurrentLeaderboardCup();
			for (int i = 1; i <= cups.Length; i++)
			{
				EnableCup((PlayFabLeaderboard.Leaderboard)i, i == currentLeaderboardCup);
			}
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < cups.Length; i++)
		{
			GameObject gameObject = cups[i].Find("CupIcon").gameObject;
			Animation animation = ((!gameObject) ? null : gameObject.GetComponent<Animation>());
			if ((bool)animation)
			{
				animation.Stop();
			}
		}
	}

	public void Init(LeaderboardDialog newDialog = null)
	{
		if (initialized)
		{
			return;
		}
		if (newDialog != null)
		{
			leaderboardDialog = newDialog;
		}
		if (cups == null)
		{
			cups = new Transform[6];
			for (int i = 1; i <= 6; i++)
			{
				string n = $"CupGrid/Cup{i}";
				cups[i - 1] = base.transform.Find(n);
			}
		}
		initialized = true;
	}

	private IEnumerator AnimEnableCup(PlayFabLeaderboard.Leaderboard cup, bool enable)
	{
		Init();
		if (cups == null)
		{
			yield break;
		}
		int cupIndex = (int)cup;
		cupIndex--;
		if (cupIndex < 0 || cupIndex >= cups.Length || !(cups[cupIndex] != null))
		{
			yield break;
		}
		Transform currentCupGraphics = cups[cupIndex].Find("BarYou");
		Transform shine = cups[cupIndex].Find("Shine");
		if (currentCupGraphics.gameObject.activeSelf == enable)
		{
			yield break;
		}
		if (!enable && (bool)shine)
		{
			shine.gameObject.SetActive(value: true);
			shine.transform.localScale = Vector3.one;
			yield return StartCoroutine(CoroutineRunner.DeltaAction(0.5f, realTime: false, delegate(float t)
			{
				shine.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
			}));
			shine.gameObject.SetActive(value: false);
		}
		else if (enable && (bool)shine)
		{
			shine.gameObject.SetActive(value: true);
			shine.transform.localScale = Vector3.zero;
			yield return StartCoroutine(CoroutineRunner.DeltaAction(0.5f, realTime: false, delegate(float t)
			{
				shine.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
			}));
			GameObject gameObject = cups[cupIndex].Find("CupIcon").gameObject;
			((!gameObject) ? null : gameObject.GetComponent<Animation>()).Play();
		}
		if (currentCupGraphics != null)
		{
			currentCupGraphics.gameObject.SetActive(enable);
		}
	}

	private void EnableCup(PlayFabLeaderboard.Leaderboard cup, bool enable)
	{
		Init();
		if (cups == null)
		{
			return;
		}
		int num = (int)(cup - 1);
		if (num >= 0 && num < cups.Length && cups[num] != null)
		{
			Transform transform = cups[num].Find("BarYou");
			Transform transform2 = cups[num].Find("Shine");
			if ((bool)transform)
			{
				transform.gameObject.SetActive(enable);
			}
			if ((bool)transform2)
			{
				transform2.gameObject.SetActive(enable);
			}
		}
	}

	public void ShowCupAnimation(int cupIndex, Action callback)
	{
		StartCoroutine(PlayCupAdvanceAnimation((PlayFabLeaderboard.Leaderboard)cupIndex, callback));
	}

	private IEnumerator PlayCupAdvanceAnimation(PlayFabLeaderboard.Leaderboard newCup, Action callback)
	{
		int oldCupIndex = (int)(newCup - 1);
		for (int i = 0; i <= cups.Length; i++)
		{
			EnableCup((PlayFabLeaderboard.Leaderboard)i, enable: false);
		}
		EnableCup((PlayFabLeaderboard.Leaderboard)oldCupIndex, enable: true);
		yield return new WaitForSeconds(0.75f);
		yield return StartCoroutine(AnimEnableCup((PlayFabLeaderboard.Leaderboard)oldCupIndex, enable: false));
		yield return StartCoroutine(AnimEnableCup(newCup, enable: true));
		callback?.Invoke();
	}
}
