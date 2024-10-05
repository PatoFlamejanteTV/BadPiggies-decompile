using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CakeRace;

public class CakeRaceReplay
{
	private string uniqueIdentifier;

	private string playerName;

	private int playerLevel;

	private bool kingsFavoriteUsed;

	private bool isValid;

	private Dictionary<int, int> collectTimes;

	public string UniqueIdentifier => uniqueIdentifier;

	public string PlayerName => playerName;

	public int PlayerLevel => playerLevel;

	public bool HasKingsFavoritePart => kingsFavoriteUsed;

	public bool IsValid { get; private set; }

	public int CakesCollected
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<int, int> collectTime in collectTimes)
			{
				if (collectTime.Key >= 0)
				{
					num++;
				}
			}
			return num;
		}
	}

	public CakeRaceReplay(string uniqueIdentifier, string playerName, int playerLevel, bool kingsFavoriteUsed, Dictionary<int, int> collectTimes)
	{
		this.uniqueIdentifier = uniqueIdentifier;
		this.playerName = playerName;
		this.playerLevel = playerLevel;
		this.kingsFavoriteUsed = kingsFavoriteUsed;
		this.collectTimes = collectTimes;
		IsValid = true;
	}

	public CakeRaceReplay(string jsonString)
	{
		try
		{
			bool flag = false;
			if (MiniJSON.jsonDecode(jsonString) is Hashtable hashtable)
			{
				if (hashtable.Contains("uniqueIdentifier"))
				{
					uniqueIdentifier = (string)hashtable["uniqueIdentifier"];
				}
				else
				{
					flag = true;
				}
				if (hashtable.Contains("playerName"))
				{
					playerName = (string)hashtable["playerName"];
				}
				else
				{
					flag = true;
				}
				if (hashtable.Contains("playerLevel"))
				{
					if (int.TryParse((string)hashtable["playerLevel"], out var result))
					{
						playerLevel = result;
					}
				}
				else
				{
					flag = true;
				}
				if (hashtable.Contains("kingsFavorite") && int.TryParse((string)hashtable["kingsFavorite"], out var result2))
				{
					kingsFavoriteUsed = result2 != 0;
				}
				collectTimes = new Dictionary<int, int>();
				if (hashtable.Contains("collectTimes") && hashtable["collectTimes"] is Hashtable)
				{
					foreach (DictionaryEntry item in (Hashtable)hashtable["collectTimes"])
					{
						if (int.TryParse((string)item.Key, out var result3) && int.TryParse((string)item.Value, out var result4))
						{
							collectTimes.Add(result3, result4);
						}
					}
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			IsValid = !flag;
		}
		catch
		{
			IsValid = false;
		}
	}

	public void SetPlayerLevel(int level)
	{
		playerLevel = level;
	}

	public string GetPlayerName()
	{
		string result = "guest";
		if (!string.IsNullOrEmpty(playerName))
		{
			result = ((!playerName.Contains("|")) ? playerName : PlayerName.Split('|')[0]);
		}
		return result;
	}

	public int GetCollectedCakeCount()
	{
		if (collectTimes != null)
		{
			return collectTimes.Count;
		}
		return 0;
	}

	public int GetCakeCollectTime(int index)
	{
		if (collectTimes.ContainsKey(index))
		{
			return collectTimes[index];
		}
		return -1;
	}

	public Dictionary<int, int> GetAllCakeCollectedTimes()
	{
		return collectTimes;
	}

	public void SetPlayerName(string playerName)
	{
		this.playerName = playerName;
	}

	public void SetCollectedCake(int cakeIndex, int collectTime)
	{
		if (collectTimes == null)
		{
			collectTimes = new Dictionary<int, int>();
		}
		if (collectTimes.ContainsKey(cakeIndex))
		{
			collectTimes[cakeIndex] = collectTime;
		}
		else
		{
			collectTimes.Add(cakeIndex, collectTime);
		}
	}

	public void SetKingsFavoritePartUsed(bool newState = true)
	{
		kingsFavoriteUsed = newState;
	}

	public static int TotalScore(CakeRaceReplay replay)
	{
		if (replay == null)
		{
			return 0;
		}
		int num = 0;
		foreach (KeyValuePair<int, int> allCakeCollectedTime in replay.GetAllCakeCollectedTimes())
		{
			num += CalculateCakeScore(allCakeCollectedTime.Key < 0, allCakeCollectedTime.Value, replay.PlayerLevel, replay.HasKingsFavoritePart);
		}
		return num;
	}

	public static int CalculateCakeScore(bool explosion, int collectTime, int playerLevel, bool kingsFavoritePartUsed = false)
	{
		float value = Singleton<GameConfigurationManager>.Instance.GetValue<float>("cake_race", "kings_favorite_bonus");
		value = Mathf.Clamp(value, 1f, float.MaxValue);
		return Mathf.FloorToInt(((!explosion) ? 1f : 0.1f) * (float)collectTime * (float)Mathf.Clamp(playerLevel, 1, 30) * ((!kingsFavoritePartUsed) ? 1f : value));
	}

	public string TrimmedString()
	{
		int count = collectTimes.Count;
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append("\"uniqueIdentifier\":\"" + uniqueIdentifier + "\",");
		stringBuilder.Append("\"playerName\":\"" + playerName + "\",");
		stringBuilder.Append("\"playerLevel\":\"" + playerLevel + "\",");
		stringBuilder.Append("\"kingsFavorite\":\"" + ((!kingsFavoriteUsed) ? "0" : "1") + "\",");
		stringBuilder.Append("\"collectTimes\":{");
		foreach (KeyValuePair<int, int> collectTime in collectTimes)
		{
			stringBuilder.Append("\"" + collectTime.Key + "\":\"" + collectTime.Value + "\"");
			if (num < count - 1)
			{
				stringBuilder.Append(",");
			}
			num++;
		}
		stringBuilder.Append("}}");
		return stringBuilder.ToString();
	}

	public override string ToString()
	{
		int num = ((collectTimes != null) ? collectTimes.Count : 0);
		int num2 = 0;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("{");
		stringBuilder.AppendLine("    \"uniqueIdentifier\":\"" + uniqueIdentifier + "\",");
		stringBuilder.AppendLine("    \"playerName\":\"" + playerName + "\",");
		stringBuilder.AppendLine("    \"playerLevel\":\"" + playerLevel + "\",");
		stringBuilder.AppendLine("    \"kingsFavorite\":\"" + ((!kingsFavoriteUsed) ? "0" : "1") + "\",");
		stringBuilder.AppendLine("    \"collectTimes\": {");
		foreach (KeyValuePair<int, int> collectTime in collectTimes)
		{
			stringBuilder.Append("        \"" + collectTime.Key + "\":\"" + collectTime.Value + "\"");
			if (num2 < num - 1)
			{
				stringBuilder.AppendLine(",");
			}
			num2++;
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("    }");
		stringBuilder.AppendLine("}");
		return stringBuilder.ToString();
	}
}
