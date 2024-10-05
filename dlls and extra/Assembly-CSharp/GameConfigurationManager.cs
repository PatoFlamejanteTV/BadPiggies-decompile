using System;
using System.Collections;
using System.Collections.Generic;

public class GameConfigurationManager : Singleton<GameConfigurationManager>
{
	public Action OnHasData;

	private Dictionary<string, ConfigData> config;

	private SecureJsonManager secureJson;

	private bool hasData;

	public bool HasData => hasData;

	private void Awake()
	{
		SetAsPersistant();
		hasData = false;
		secureJson = new SecureJsonManager("gameconfiguration");
		secureJson.Initialize(OnDataLoaded);
	}

	private void OnDestroy()
	{
		secureJson = null;
		config = null;
	}

	private void OnDataLoaded(string rawData)
	{
		config = new Dictionary<string, ConfigData>();
		foreach (DictionaryEntry item in MiniJSON.jsonDecode(rawData) as Hashtable)
		{
			try
			{
				config.Add((string)item.Key, new ConfigData((string)item.Key, (Hashtable)item.Value));
			}
			catch
			{
			}
		}
		hasData = true;
		if (OnHasData != null)
		{
			OnHasData();
		}
	}

	public Hashtable GetValues(string itemKey)
	{
		if (config != null && config.ContainsKey(itemKey))
		{
			return config[itemKey].ToHashtable();
		}
		return null;
	}

	public T GetValue<T>(string itemKey, string valueKey)
	{
		if (config != null && config.ContainsKey(itemKey) && config[itemKey].HasKey(valueKey))
		{
			Type typeFromHandle = typeof(T);
			string text = config[itemKey][valueKey];
			if (typeFromHandle == typeof(int))
			{
				if (int.TryParse(text, out var result))
				{
					return (T)Convert.ChangeType(result, typeof(T));
				}
			}
			else
			{
				if (typeFromHandle == typeof(string))
				{
					return (T)Convert.ChangeType(text, typeof(T));
				}
				if (typeFromHandle == typeof(float))
				{
					if (float.TryParse(text, out var result2))
					{
						return (T)Convert.ChangeType(result2, typeof(T));
					}
				}
				else if (typeFromHandle == typeof(bool))
				{
					if (text.ToLower() == "true")
					{
						return (T)Convert.ChangeType(true, typeof(T));
					}
					return default(T);
				}
			}
		}
		return default(T);
	}

	public ConfigData GetConfig(string itemKey)
	{
		if (config != null && config.TryGetValue(itemKey, out var value))
		{
			return value;
		}
		return null;
	}

	public bool HasValue(string itemKey, string valueKey)
	{
		if (config != null && config.ContainsKey(itemKey))
		{
			return config[itemKey].HasKey(valueKey);
		}
		return false;
	}

	public bool HasConfig(string itemKey)
	{
		if (config != null)
		{
			return config.ContainsKey(itemKey);
		}
		return false;
	}
}
