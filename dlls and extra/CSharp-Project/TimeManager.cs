using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
	private class Timer
	{
		public const string DATE = "date";

		private const float CHECK_INTERVAL = 60f;

		public string id;

		public DateTime date;

		private bool fired;

		private bool remove;

		private bool confirming;

		private bool confirmed;

		private float time;

		private float inited;

		private float lastCheck;

		public bool Fired => fired;

		public bool Remove => remove;

		public float FireTime => time;

		public float TimeLeft => inited + time - (float)Singleton<TimeManager>.Instance.TimeFromStart.TotalSeconds;

		public DateTime Date => date;

		private bool CanCheck => realtimeSinceStartup - (double)lastCheck > 60.0;

		public event OnTimedOut onTimedOut;

		public Timer(string id, DateTime time, OnTimedOut onTimedOut, TimeManager timeMan)
		{
			confirmed = false;
			confirming = false;
			lastCheck = -60f;
			this.id = id;
			this.time = (float)time.Subtract(timeMan.CurrentTime).TotalSeconds;
			inited = (float)timeMan.TimeFromStart.TotalSeconds;
			if (onTimedOut != null)
			{
				this.onTimedOut = (OnTimedOut)Delegate.Combine(this.onTimedOut, onTimedOut);
			}
			date = time;
			fired = false;
		}

		public bool Check()
		{
			if (TimeLeft < 0f && !confirming && !confirmed && CanCheck)
			{
				confirming = true;
				OnServerTimeSuccessfull(0uL);
			}
			if (!fired && this.onTimedOut != null && TimeLeft < 0f)
			{
				return confirmed;
			}
			return false;
		}

		public void Fire()
		{
			if (this.onTimedOut != null)
			{
				this.onTimedOut((int)(time - (float)Singleton<TimeManager>.Instance.TimeFromStart.TotalSeconds));
				fired = true;
			}
		}

		public void SetRemoved()
		{
			remove = true;
		}

		private void OnServerTimeSuccessfull(ulong serverTime)
		{
			DateTime now = DateTime.Now;
			confirmed = now >= date;
			lastCheck = (float)realtimeSinceStartup;
			confirming = false;
		}

		private void OnServerTimeUnsuccessfull(int errorCode, string message)
		{
			lastCheck = (float)realtimeSinceStartup;
			confirming = false;
		}
	}

	private const float MAX_UPDATE_RATE = 5f;

	private double sessionStart;

	private Dictionary<string, Timer> timers;

	private bool timePending;

	private int lastServerTime;

	private int offset;

	private float lastServerTimeCheck;

	private bool initialized;

	private static double m_realtimeSinceStartup;

	private bool HasTime
	{
		get
		{
			if (lastServerTime > 0)
			{
				return lastServerTimeCheck > 0f;
			}
			return false;
		}
	}

	private bool CanUpdate => (float)Singleton<TimeManager>.Instance.TimeFromStart.TotalSeconds - lastServerTimeCheck > 5f;

	public bool Initialized => initialized;

	public TimeSpan TimeFromStart => new TimeSpan(0, 0, (int)(realtimeSinceStartup - sessionStart));

	public DateTime CurrentTime => DateTime.Now;

	public DateTime ServerTime => DateTime.Now;

	public int CurrentEpochTime => (int)DateTimeOffset.Now.ToUnixTimeSeconds();

	public static double realtimeSinceStartup => m_realtimeSinceStartup;

	public event OnInitializeHandler OnInitialize;

	private void Awake()
	{
		SetAsPersistant();
		sessionStart = realtimeSinceStartup;
		timers = new Dictionary<string, Timer>();
		lastServerTime = -1;
		lastServerTimeCheck = -1f;
		EventManager.Connect<PlayerChangedEvent>(OnPlayerChanged);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<PlayerChangedEvent>(OnPlayerChanged);
	}

	private void OnPlayerChanged(PlayerChangedEvent data)
	{
		initialized = false;
		sessionStart = realtimeSinceStartup;
		timers = new Dictionary<string, Timer>();
		lastServerTime = -1;
		lastServerTimeCheck = -1f;
	}

	private void Initialize()
	{
		if (initialized)
		{
			return;
		}
		string[] timerIds = GameProgress.GetTimerIds();
		for (int i = 0; i < timerIds.Length; i++)
		{
			if (!timers.ContainsKey(timerIds[i]))
			{
				timers.Add(timerIds[i], new Timer(timerIds[i], ConvertSeconds2DateTime(GameProgress.GetTimerData<int>(timerIds[i], "date")), null, this));
			}
		}
		initialized = true;
		if (this.OnInitialize != null)
		{
			this.OnInitialize();
		}
	}

	private void Update()
	{
		m_realtimeSinceStartup += Time.unscaledDeltaTime;
		if (!HasTime && !timePending && CanUpdate)
		{
			timePending = true;
			ServerTimeSuccessfullCallback((ulong)lastServerTime);
		}
		else if (HasTime)
		{
			UpdateTimers();
		}
	}

	private void UpdateTimers()
	{
		Action action = null;
		Action action2 = null;
		foreach (string key in timers.Keys)
		{
			Timer timer = timers[key];
			if (timer.Check())
			{
				action = (Action)Delegate.Combine(action, new Action(timer.Fire));
				action2 = (Action)Delegate.Combine(action2, new Action(timer.SetRemoved));
			}
		}
		if (action != null || action2 != null)
		{
			action2();
			SaveTimers();
			action();
		}
	}

	private void SaveTimers()
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (string key in timers.Keys)
		{
			if (timers[key].Remove)
			{
				list.Add(key);
			}
			else
			{
				list2.Add(key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			timers.Remove(list[i]);
			GameProgress.RemoveTimerData(list[i], "date");
		}
		GameProgress.SetTimerIds(list2.ToArray());
		for (int j = 0; j < list2.Count; j++)
		{
			GameProgress.SetTimerData(list2[j], "date", ConvertDateTime2Seconds(timers[list2[j]].Date));
		}
	}

	private void ServerTimeSuccessfullCallback(ulong currentTime)
	{
		lastServerTimeCheck = (float)Singleton<TimeManager>.Instance.TimeFromStart.TotalSeconds;
		lastServerTime = (int)currentTime;
		timePending = false;
		if (!initialized)
		{
			Initialize();
		}
	}

	public void Subscribe(string id, OnTimedOut onTimedOut)
	{
		if (timers.ContainsKey(id))
		{
			timers[id].onTimedOut += onTimedOut;
		}
	}

	public void Unsubscribe(string id, OnTimedOut onTimedOut)
	{
		if (timers.ContainsKey(id))
		{
			timers[id].onTimedOut -= onTimedOut;
		}
	}

	public bool HasTimer(string id)
	{
		return timers.ContainsKey(id);
	}

	public float TimeLeft(string id)
	{
		if (!timers.ContainsKey(id))
		{
			return -1f;
		}
		return timers[id].TimeLeft;
	}

	public void CreateTimer(string id, DateTime time, OnTimedOut onTimedOut)
	{
		timers.Add(id, new Timer(id, time, onTimedOut, this));
		SaveTimers();
	}

	public void RemoveTimer(string id)
	{
		timers[id].SetRemoved();
		SaveTimers();
	}

	public void RemoveAllTimers()
	{
		foreach (string key in timers.Keys)
		{
			timers[key].SetRemoved();
		}
		SaveTimers();
	}

	private int GetTimeOffset()
	{
		if (GameProgress.HasKey("TimeOffset"))
		{
			return GameProgress.GetInt("TimeOffset");
		}
		int num = (int)DateTime.Now.Subtract(ServerTime).TotalSeconds;
		GameProgress.SetInt("TimeOffset", num);
		return num;
	}

	public static DateTime ConvertSeconds2DateTime(int seconds)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
	}

	public static int ConvertDateTime2Seconds(DateTime date)
	{
		return (int)date.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
	}
}
