using System;
using System.Collections;
using UnityEngine;

public class SecureJsonManager
{
	protected string fileName;

	public SecureJsonManager(string newFileName)
	{
		fileName = newFileName;
	}

	public void Initialize(Action<string> onDataLoaded)
	{
		Hashtable hashtable = MiniJSON.jsonDecode(Resources.Load<TextAsset>("rawAppConfig").text) as Hashtable;
		string key = fileName;
		if (hashtable.ContainsKey(key))
		{
			onDataLoaded?.Invoke(MiniJSON.jsonEncode(hashtable[key]));
		}
	}
}
