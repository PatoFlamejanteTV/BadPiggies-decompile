using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Challenge : WPFMonoBehaviour
{
	public enum ChallengeType
	{
		DontUseParts,
		Time,
		PerfectFlight,
		Transport,
		Box,
		Max
	}

	public class ChallengeOrder : IComparer<Challenge>
	{
		public int Compare(Challenge obj1, Challenge obj2)
		{
			return string.Compare(obj1.name, obj2.name);
		}
	}

	[Serializable]
	public class IconPlacement
	{
		public Vector3 position;

		public float scale = 1f;

		public GameObject icon;
	}

	public List<IconPlacement> m_icons;

	public GameObject m_tutorialBookPage;

	private static List<Challenge> s_challenges = new List<Challenge>();

	[SerializeField]
	private int m_challengeNumber;

	public static List<Challenge> Challenges => s_challenges;

	public virtual ChallengeType Type => ChallengeType.DontUseParts;

	public int ChallengeNumber
	{
		get
		{
			return m_challengeNumber;
		}
		set
		{
			m_challengeNumber = value;
		}
	}

	public List<IconPlacement> Icons => m_icons;

	protected virtual void Awake()
	{
		s_challenges.Add(this);
		Refresh();
	}

	protected virtual void OnDestroy()
	{
		s_challenges.Remove(this);
		Refresh();
	}

	private void Refresh()
	{
		s_challenges.Sort(new ChallengeOrder());
		for (int i = 0; i < s_challenges.Count; i++)
		{
			s_challenges[i].m_challengeNumber = i + 1;
		}
	}

	public abstract bool IsCompleted();

	public virtual float TimeLimit()
	{
		return 0f;
	}
}
