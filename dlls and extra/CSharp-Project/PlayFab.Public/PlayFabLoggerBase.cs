using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using PlayFab.Internal;
using UnityEngine;

namespace PlayFab.Public;

public abstract class PlayFabLoggerBase : IPlayFabLogger
{
	private static readonly StringBuilder Sb = new StringBuilder();

	private readonly Queue<string> LogMessageQueue = new Queue<string>();

	private const int LOG_CACHE_INTERVAL_MS = 10000;

	private Thread _writeLogThread;

	private readonly object _threadLock = new object();

	private static readonly TimeSpan _threadKillTimeout = TimeSpan.FromSeconds(60.0);

	private DateTime _threadKillTime = DateTime.UtcNow + _threadKillTimeout;

	private bool _isApplicationPlaying = true;

	private int _pendingLogsCount;

	public IPAddress ip { get; set; }

	public int port { get; set; }

	public string url { get; set; }

	protected PlayFabLoggerBase()
	{
		string item = new PlayFabDataGatherer().GenerateReport();
		lock (LogMessageQueue)
		{
			LogMessageQueue.Enqueue(item);
		}
	}

	public virtual void OnEnable()
	{
		SingletonMonoBehaviour<PlayFabHttp>.instance.StartCoroutine(RegisterLogger());
	}

	private IEnumerator RegisterLogger()
	{
		yield return new WaitForEndOfFrame();
		if (!string.IsNullOrEmpty(PlayFabSettings.LoggerHost))
		{
			Application.logMessageReceivedThreaded += HandleUnityLog;
		}
	}

	public virtual void OnDisable()
	{
		if (!string.IsNullOrEmpty(PlayFabSettings.LoggerHost))
		{
			Application.logMessageReceivedThreaded -= HandleUnityLog;
		}
	}

	public virtual void OnDestroy()
	{
		_isApplicationPlaying = false;
	}

	protected abstract void BeginUploadLog();

	protected abstract void UploadLog(string message);

	protected abstract void EndUploadLog();

	private void HandleUnityLog(string message, string stacktrace, LogType type)
	{
		if (!PlayFabSettings.EnableRealTimeLogging)
		{
			return;
		}
		Sb.Length = 0;
		switch (type)
		{
		case LogType.Warning:
		case LogType.Log:
			Sb.Append(type).Append(": ").Append(message);
			message = Sb.ToString();
			lock (LogMessageQueue)
			{
				LogMessageQueue.Enqueue(message);
			}
			break;
		case LogType.Error:
		case LogType.Exception:
			Sb.Append(type).Append(": ").Append(message)
				.Append("\n")
				.Append(stacktrace)
				.Append(StackTraceUtility.ExtractStackTrace());
			message = Sb.ToString();
			lock (LogMessageQueue)
			{
				LogMessageQueue.Enqueue(message);
			}
			break;
		}
		ActivateThreadWorker();
	}

	private void ActivateThreadWorker()
	{
		lock (_threadLock)
		{
			if (_writeLogThread == null)
			{
				_writeLogThread = new Thread(WriteLogThreadWorker);
				_writeLogThread.Start();
			}
		}
	}

	private void WriteLogThreadWorker()
	{
		try
		{
			lock (_threadLock)
			{
				_threadKillTime = DateTime.UtcNow + _threadKillTimeout;
			}
			Queue<string> queue = new Queue<string>();
			bool flag;
			do
			{
				lock (LogMessageQueue)
				{
					_pendingLogsCount = LogMessageQueue.Count;
					while (LogMessageQueue.Count > 0)
					{
						queue.Enqueue(LogMessageQueue.Dequeue());
					}
				}
				BeginUploadLog();
				while (queue.Count > 0)
				{
					UploadLog(queue.Dequeue());
				}
				EndUploadLog();
				lock (_threadLock)
				{
					DateTime utcNow = DateTime.UtcNow;
					if (_pendingLogsCount > 0 && _isApplicationPlaying)
					{
						_threadKillTime = utcNow + _threadKillTimeout;
					}
					flag = utcNow <= _threadKillTime;
					if (!flag)
					{
						_writeLogThread = null;
					}
				}
				Thread.Sleep(10000);
			}
			while (flag);
		}
		catch
		{
			_writeLogThread = null;
		}
	}
}
