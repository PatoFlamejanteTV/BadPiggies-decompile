using System;
using System.Collections;
using System.Collections.Generic;

public class ConfigData
{
	private string configId;

	private List<string> keys;

	private List<string> values;

	public string ConfigID
	{
		get
		{
			return configId;
		}
		set
		{
			configId = value;
		}
	}

	public int Count => keys.Count;

	public string[] Keys => keys.ToArray();

	public string[] Values => values.ToArray();

	public string this[string key]
	{
		get
		{
			if (keys.Contains(key))
			{
				return values[keys.IndexOf(key)];
			}
			throw new KeyNotFoundException("Key not found");
		}
		set
		{
			if (keys.Contains(key))
			{
				values[keys.IndexOf(key)] = value;
				return;
			}
			throw new KeyNotFoundException("Key not found");
		}
	}

	public ConfigData(string configId)
	{
		this.configId = configId;
		keys = new List<string>();
		values = new List<string>();
	}

	public ConfigData(string configId, Hashtable data)
	{
		this.configId = configId;
		keys = new List<string>();
		values = new List<string>();
		foreach (DictionaryEntry datum in data)
		{
			keys.Add((string)datum.Key);
			values.Add((string)datum.Value);
		}
	}

	public bool HasKey(string key)
	{
		if (keys != null)
		{
			return keys.Contains(key);
		}
		return false;
	}

	public void AddValue(string key, string value)
	{
		if (keys.Contains(key))
		{
			throw new ArgumentException("Key already exists");
		}
		keys.Add(key);
		values.Add(value);
	}

	public void RemoveValue(string key)
	{
		if (!keys.Contains(key))
		{
			throw new KeyNotFoundException("Key not found");
		}
		int index = keys.IndexOf(key);
		keys.RemoveAt(index);
		values.RemoveAt(index);
	}

	public void ReplaceKey(string oldKey, string newKey)
	{
		if (keys.Contains(oldKey))
		{
			keys[keys.IndexOf(oldKey)] = newKey;
		}
	}

	public Hashtable ToHashtable()
	{
		Hashtable hashtable = new Hashtable();
		for (int i = 0; i < keys.Count; i++)
		{
			hashtable.Add(keys[i], values[i]);
		}
		return hashtable;
	}
}
