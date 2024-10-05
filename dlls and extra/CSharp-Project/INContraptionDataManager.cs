using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class INContraptionDataManager
{
	private enum SerializationFormat
	{
		CSV = 0,
		JSON = 1,
		ALL = -1
	}

	private class INContraptionDataSettings
	{
		public Version version;

		public bool enabled;

		public SerializationFormat loadFormat;

		public SerializationFormat saveFormat;

		public bool indented;

		public bool loadOriginalDataFirst;

		public bool renameOriginalData;

		public bool saveOriginalData;

		public INContraptionDataSettings()
		{
			version = INUnity.Version;
			enabled = true;
			loadFormat = SerializationFormat.ALL;
			saveFormat = SerializationFormat.CSV;
			indented = true;
			loadOriginalDataFirst = false;
			renameOriginalData = true;
			saveOriginalData = false;
		}
	}

	public class INContraptionDataUnit
	{
		public int type;

		public int index;

		public int x;

		public int y;

		public int rotation;

		public int flipped;
	}

	public class INContraptionData
	{
		public INContraptionDataUnit[] items;

		public INContraptionData()
		{
			items = Array.Empty<INContraptionDataUnit>();
		}

		public INContraptionData(int count)
		{
			items = new INContraptionDataUnit[count];
		}

		public static INContraptionData Create(ContraptionDataset contraptionDataset)
		{
			List<ContraptionDataset.ContraptionDatasetUnit> contraptionDatasetList = contraptionDataset.ContraptionDatasetList;
			int count = contraptionDatasetList.Count;
			INContraptionData iNContraptionData = new INContraptionData(count);
			for (int i = 0; i < count; i++)
			{
				ContraptionDataset.ContraptionDatasetUnit contraptionDatasetUnit = contraptionDatasetList[i];
				INContraptionDataUnit iNContraptionDataUnit = new INContraptionDataUnit();
				iNContraptionDataUnit.x = contraptionDatasetUnit.x;
				iNContraptionDataUnit.y = contraptionDatasetUnit.y;
				iNContraptionDataUnit.type = (int)((BasePart.PartType)contraptionDatasetUnit.partType).ToSortedPartType();
				iNContraptionDataUnit.index = contraptionDatasetUnit.customPartIndex;
				iNContraptionDataUnit.rotation = contraptionDatasetUnit.rot;
				iNContraptionDataUnit.flipped = Convert.ToInt32(contraptionDatasetUnit.flipped);
				iNContraptionData.items[i] = iNContraptionDataUnit;
			}
			return iNContraptionData;
		}

		public ContraptionDataset ConvertTo()
		{
			ContraptionDataset contraptionDataset = new ContraptionDataset();
			INContraptionDataUnit[] array = items;
			foreach (INContraptionDataUnit iNContraptionDataUnit in array)
			{
				contraptionDataset.AddPart(iNContraptionDataUnit.x, iNContraptionDataUnit.y, (int)((SortedPartType)iNContraptionDataUnit.type).ToPartType(), iNContraptionDataUnit.index, ToGridRotation(iNContraptionDataUnit.rotation), Convert.ToBoolean(iNContraptionDataUnit.flipped));
			}
			return contraptionDataset;
		}
	}

	private string m_dataDirectory;

	private INContraptionDataSettings m_settings;

	public static INContraptionDataManager Instance { get; private set; }

	public string DataDirectory => m_dataDirectory;

	public static void Create()
	{
		INContraptionDataManager iNContraptionDataManager = new INContraptionDataManager();
		iNContraptionDataManager.Initialize();
		Instance = iNContraptionDataManager;
	}

	public static void SetContraptionData()
	{
		if (INSettings.GetBool(INFeature.NewContraptionData))
		{
			Create();
		}
		else
		{
			Instance = null;
		}
	}

	public void Initialize()
	{
		int versionType = INSettings.VersionType;
		m_dataDirectory = INUnity.DataPath + "/contraptions" + versionType switch
		{
			2 => "A", 
			1 => "O", 
			0 => "", 
			_ => "B", 
		};
		string settingsPath = INUnity.SettingsPath;
		string path = settingsPath + "/INContraptionDataSettings.json";
		Directory.CreateDirectory(settingsPath);
		Directory.CreateDirectory(m_dataDirectory);
		LoadSettings(path);
		SaveSettings(path);
	}

	private void LoadSettings(string path)
	{
		try
		{
			using StreamReader reader = new StreamReader(path);
			m_settings = INJsonSerializer.Deserialize<INContraptionDataSettings>(reader);
		}
		catch
		{
			m_settings = new INContraptionDataSettings();
		}
	}

	private void SaveSettings(string path)
	{
		using StreamWriter writer = new StreamWriter(path);
		INJsonSerializer.Serialize(m_settings, indented: true, writer);
	}

	public ContraptionDataset LoadContraptionData(string levelName)
	{
		string dataDirectory = m_dataDirectory;
		if (!m_settings.enabled)
		{
			return WPFPrefs.LoadOriginalContraptionDataset(dataDirectory, levelName);
		}
		string text = dataDirectory + "/" + WPFPrefs.ContraptionFileName(levelName);
		string path = dataDirectory + "/" + levelName;
		ContraptionDataset result;
		if (!m_settings.loadOriginalDataFirst)
		{
			if (!File.Exists(path))
			{
				result = ((!File.Exists(text)) ? new ContraptionDataset() : WPFPrefs.LoadOriginalContraptionDataset(dataDirectory, levelName));
			}
			else
			{
				TryLoadAndConvert(path, out result);
			}
		}
		else if (File.Exists(text))
		{
			result = WPFPrefs.LoadOriginalContraptionDataset(dataDirectory, levelName);
		}
		else if (File.Exists(path))
		{
			TryLoadAndConvert(path, out result);
		}
		else
		{
			result = new ContraptionDataset();
		}
		if (m_settings.renameOriginalData && File.Exists(text))
		{
			try
			{
				string text2 = text.Replace(".contraption", ".bak");
				if (File.Exists(text2))
				{
					File.Delete(text2);
				}
				File.Move(text, text2);
			}
			catch
			{
			}
		}
		return result;
	}

	public void SaveContraptionData(string levelName, ContraptionDataset data)
	{
		string dataDirectory = m_dataDirectory;
		if (!m_settings.enabled)
		{
			WPFPrefs.SaveOriginalContraptionDataset(dataDirectory, levelName, data);
		}
		string path = dataDirectory + "/" + levelName;
		Save(path, INContraptionData.Create(data));
		if (m_settings.saveOriginalData)
		{
			WPFPrefs.SaveOriginalContraptionDataset(dataDirectory, levelName, data);
		}
	}

	private bool TryLoadAndConvert(string path, out ContraptionDataset result)
	{
		try
		{
			result = Load(path).ConvertTo();
			return true;
		}
		catch
		{
			result = new ContraptionDataset();
			return false;
		}
	}

	private bool TryLoad(string path, out INContraptionData result)
	{
		try
		{
			result = LoadCSVFile(path);
			return true;
		}
		catch
		{
			result = new INContraptionData();
			return false;
		}
	}

	public INContraptionData Load(string path)
	{
		switch (m_settings.loadFormat)
		{
		case SerializationFormat.ALL:
		{
			if (TryLoadCSVFile(path, out var result))
			{
				return result;
			}
			return LoadJSONFile(path);
		}
		case SerializationFormat.CSV:
			return LoadCSVFile(path);
		case SerializationFormat.JSON:
			return LoadJSONFile(path);
		default:
			return new INContraptionData();
		}
	}

	private INContraptionData LoadCSVFile(string path)
	{
		using StreamReader streamReader = new StreamReader(path);
		string[] array = streamReader.ReadToEnd().Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
		int num = array.Length;
		INContraptionData iNContraptionData = new INContraptionData(num);
		for (int i = 0; i < num; i++)
		{
			string[] array2 = array[i].Split(new char[2] { ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			INContraptionDataUnit iNContraptionDataUnit = new INContraptionDataUnit();
			iNContraptionDataUnit.type = int.Parse(array2[0]);
			iNContraptionDataUnit.index = int.Parse(array2[1]);
			iNContraptionDataUnit.x = int.Parse(array2[2]);
			iNContraptionDataUnit.y = int.Parse(array2[3]);
			iNContraptionDataUnit.rotation = int.Parse(array2[4]);
			iNContraptionDataUnit.flipped = int.Parse(array2[5]);
			iNContraptionData.items[i] = iNContraptionDataUnit;
		}
		return iNContraptionData;
	}

	private bool TryLoadCSVFile(string path, out INContraptionData result)
	{
		result = null;
		using StreamReader streamReader = new StreamReader(path);
		string[] array = streamReader.ReadToEnd().Split(new char[2] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
		int num = array.Length;
		INContraptionData iNContraptionData = new INContraptionData(num);
		for (int i = 0; i < num; i++)
		{
			string[] array2 = array[i].Split(new char[2] { ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length != 6)
			{
				return false;
			}
			INContraptionDataUnit iNContraptionDataUnit = new INContraptionDataUnit();
			if (int.TryParse(array2[0], out iNContraptionDataUnit.type) && int.TryParse(array2[1], out iNContraptionDataUnit.index) && int.TryParse(array2[2], out iNContraptionDataUnit.x) && int.TryParse(array2[3], out iNContraptionDataUnit.y) && int.TryParse(array2[4], out iNContraptionDataUnit.rotation) && int.TryParse(array2[5], out iNContraptionDataUnit.flipped))
			{
				iNContraptionData.items[i] = iNContraptionDataUnit;
				continue;
			}
			return false;
		}
		result = iNContraptionData;
		return true;
	}

	private INContraptionData LoadJSONFile(string path)
	{
		using StreamReader reader = new StreamReader(path);
		return INJsonSerializer.Deserialize<INContraptionData>(reader);
	}

	public void Save(string path, INContraptionData data)
	{
		SerializationFormat saveFormat = m_settings.saveFormat;
		if (saveFormat == SerializationFormat.CSV)
		{
			SaveCSVFile(path, data);
		}
		if (saveFormat == SerializationFormat.JSON)
		{
			SaveJSONFile(path, data);
		}
	}

	private void SaveCSVFile(string path, INContraptionData data)
	{
		using StreamWriter streamWriter = new StreamWriter(path);
		StringBuilder stringBuilder = new StringBuilder();
		INContraptionDataUnit[] items = data.items;
		foreach (INContraptionDataUnit iNContraptionDataUnit in items)
		{
			string value = ",";
			stringBuilder.Append(iNContraptionDataUnit.type.ToString());
			stringBuilder.Append(value);
			stringBuilder.Append(iNContraptionDataUnit.index.ToString());
			stringBuilder.Append(value);
			stringBuilder.Append(iNContraptionDataUnit.x);
			stringBuilder.Append(value);
			stringBuilder.Append(iNContraptionDataUnit.y.ToString());
			stringBuilder.Append(value);
			stringBuilder.Append(iNContraptionDataUnit.rotation.ToString());
			stringBuilder.Append(value);
			stringBuilder.Append(iNContraptionDataUnit.flipped.ToString());
			stringBuilder.AppendLine();
		}
		streamWriter.Write(stringBuilder.ToString());
	}

	private void SaveJSONFile(string path, INContraptionData data)
	{
		using StreamWriter writer = new StreamWriter(path);
		INJsonSerializer.Serialize(data, m_settings.indented, writer);
	}

	private static string IndentString(string s, string separator, int indentation)
	{
		int num = s.Length + separator.Length;
		int count = ((num > indentation * 4) ? ((num % 4 != 0) ? 1 : 0) : (indentation - num / 4));
		return s + separator + new string('\t', count);
	}

	public static BasePart.GridRotation ToGridRotation(int value)
	{
		return (BasePart.GridRotation)value;
	}
}
