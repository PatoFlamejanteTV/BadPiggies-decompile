using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DailyChallengeProgram
{
	private const string SPECIAL_DAYS_JSON = "specialdays.json";

	private const string DEFAULT_PROGRAM_JSON = "defaultdailyprogram.json";

	private SecureJsonManager specialDaysJson;

	private SecureJsonManager defaultProgramJson;

	private Dictionary<DateTime, LootCrateType[]> specialDays;

	private Dictionary<DayOfWeek, LootCrateType[]> defaultProgram;

	private bool specialDaysInitialized;

	private bool defaultProgramInitialized;

	public bool Initialized
	{
		get
		{
			if (specialDaysInitialized)
			{
				return defaultProgramInitialized;
			}
			return false;
		}
	}

	public DailyChallengeProgram()
	{
		specialDays = new Dictionary<DateTime, LootCrateType[]>();
		defaultProgram = new Dictionary<DayOfWeek, LootCrateType[]>();
		specialDaysJson = new SecureJsonManager("specialdays");
		defaultProgramJson = new SecureJsonManager("defaultdailyprogram");
		specialDaysJson.Initialize(OnSpecialDaysLoaded);
		defaultProgramJson.Initialize(OnDefaultProgramLoaded);
	}

	private void OnSpecialDaysLoaded(string data)
	{
		specialDaysInitialized = LoadSpecialDays(data);
	}

	private void OnDefaultProgramLoaded(string data)
	{
		defaultProgramInitialized = LoadDefaultProgram(data);
	}

	private bool LoadSpecialDays(string rawData)
	{
		if (string.IsNullOrEmpty(rawData))
		{
			return true;
		}
		if (DecodeJson(rawData, out var data))
		{
			specialDays = new Dictionary<DateTime, LootCrateType[]>();
			foreach (object key2 in data.Keys)
			{
				DateTime key = new DateTime(long.Parse(key2 as string));
				ArrayList arrayList = data[key2] as ArrayList;
				LootCrateType[] array = new LootCrateType[arrayList.Count];
				for (int i = 0; i < arrayList.Count; i++)
				{
					array[i] = (LootCrateType)Convert.ChangeType(arrayList[i], typeof(int));
				}
				specialDays.Add(key, array);
			}
			return true;
		}
		return false;
	}

	private bool LoadDefaultProgram(string rawData)
	{
		if (DecodeJson(rawData, out var data))
		{
			defaultProgram = new Dictionary<DayOfWeek, LootCrateType[]>();
			foreach (object key2 in data.Keys)
			{
				DayOfWeek key = (DayOfWeek)int.Parse(key2 as string);
				ArrayList arrayList = data[key2] as ArrayList;
				LootCrateType[] array = new LootCrateType[arrayList.Count];
				for (int i = 0; i < arrayList.Count; i++)
				{
					array[i] = (LootCrateType)Convert.ChangeType(arrayList[i], typeof(int));
				}
				defaultProgram.Add(key, array);
			}
			return true;
		}
		return false;
	}

	private bool DecodeJson(string rawData, out Hashtable data)
	{
		try
		{
			data = MiniJSON.jsonDecode(rawData) as Hashtable;
			return true;
		}
		catch
		{
			data = null;
			return false;
		}
	}

	private bool LoadData(string path, out Hashtable data)
	{
		if (!File.Exists(path))
		{
			data = null;
			return false;
		}
		StreamReader streamReader = null;
		try
		{
			streamReader = new StreamReader(path);
			string json = streamReader.ReadToEnd();
			data = MiniJSON.jsonDecode(json) as Hashtable;
			return true;
		}
		catch
		{
			data = null;
			return false;
		}
		finally
		{
			streamReader?.Dispose();
		}
	}

	public LootCrateType GetLootCrateType(DateTime date, int index)
	{
		if (specialDays.ContainsKey(date) && index >= 0 && index < specialDays[date].Length)
		{
			return specialDays[date][index];
		}
		if (defaultProgram.ContainsKey(date.DayOfWeek) && index >= 0 && index < defaultProgram[date.DayOfWeek].Length)
		{
			return defaultProgram[date.DayOfWeek][index];
		}
		return LootCrateType.Wood;
	}
}
