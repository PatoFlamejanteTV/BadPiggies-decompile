using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

public class AchievementData : Singleton<AchievementData>
{
	public struct AchievementDataHolder
	{
		public double progress;

		public bool completed;

		public bool synced;
	}

	[Serializable]
	public class AchievementDescriptor
	{
		public string id;

		public string iconFileName;

		public double limit;

		public double debugLimit;
	}

	private bool m_limitsInitialized;

	private string m_fileName;

	private bool m_useEncryption;

	private CryptoUtility m_crypto;

	private Dictionary<string, AchievementDataHolder> m_achievementData = new Dictionary<string, AchievementDataHolder>();

	[SerializeField]
	private List<AchievementDescriptor> m_achievementList = new List<AchievementDescriptor>();

	private Dictionary<string, AchievementDescriptor> m_achievementLimits = new Dictionary<string, AchievementDescriptor>();

	public string FileName
	{
		get
		{
			return m_fileName;
		}
		set
		{
			m_fileName = value;
		}
	}

	public Dictionary<string, AchievementDescriptor> AchievementsLimits => m_achievementLimits;

	public Dictionary<string, double> AchievementsProgress
	{
		get
		{
			Dictionary<string, double> dictionary = new Dictionary<string, double>();
			foreach (KeyValuePair<string, AchievementDataHolder> achievementDatum in m_achievementData)
			{
				dictionary.Add(achievementDatum.Key, achievementDatum.Value.progress);
			}
			return dictionary;
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			Save();
		}
	}

	public void Save()
	{
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.Indent = true;
		xmlWriterSettings.IndentChars = "  ";
		xmlWriterSettings.NewLineChars = "\r\n";
		xmlWriterSettings.NewLineHandling = NewLineHandling.Replace;
		xmlWriterSettings.Encoding = Encoding.UTF8;
		MemoryStream memoryStream = new MemoryStream();
		XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
		xmlWriter.WriteStartDocument();
		xmlWriter.WriteStartElement("data");
		foreach (KeyValuePair<string, AchievementDataHolder> achievementDatum in m_achievementData)
		{
			xmlWriter.WriteStartElement("Achievement");
			xmlWriter.WriteAttributeString("id", achievementDatum.Key);
			xmlWriter.WriteAttributeString("progress", achievementDatum.Value.progress.ToString());
			xmlWriter.WriteAttributeString("completed", achievementDatum.Value.completed.ToString());
			xmlWriter.WriteAttributeString("synced", achievementDatum.Value.synced.ToString());
			xmlWriter.WriteEndElement();
		}
		xmlWriter.WriteEndElement();
		xmlWriter.WriteEndDocument();
		xmlWriter.Close();
		byte[] array = memoryStream.ToArray();
		if (!m_useEncryption)
		{
			FileStream fileStream = new FileStream(m_fileName, FileMode.Create);
			fileStream.Write(array, 0, array.Length);
			fileStream.Close();
			return;
		}
		byte[] array2 = m_crypto.Encrypt(array);
		byte[] array3 = CryptoUtility.ComputeHash(array2);
		FileStream fileStream2 = new FileStream(m_fileName, FileMode.Create);
		fileStream2.Write(array3, 0, array3.Length);
		fileStream2.Write(array2, 0, array2.Length);
		fileStream2.Close();
	}

	public bool Load()
	{
		if (!File.Exists(m_fileName))
		{
			return false;
		}
		try
		{
			FileStream fileStream = new FileStream(m_fileName, FileMode.Open);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
			fileStream.Close();
			byte[] buffer;
			if (m_useEncryption)
			{
				if (array.Length < 20)
				{
					throw new IOException("Corrupted data file: could not read hash");
				}
				byte[] array2 = m_crypto.ComputeHash(array, 20, array.Length - 20);
				for (int i = 0; i < 20; i++)
				{
					if (array2[i] != array[i])
					{
						throw new IOException("Corrupted data file");
					}
				}
				buffer = m_crypto.Decrypt(array, 20);
			}
			else
			{
				buffer = array;
			}
			MemoryStream stream = new MemoryStream(buffer);
			LoadXml(stream);
			return true;
		}
		catch
		{
		}
		return false;
	}

	private void LoadXml(Stream stream)
	{
		XDocument xDocument = XDocument.Load(stream);
		m_achievementData.Clear();
		AchievementDataHolder adh = default(AchievementDataHolder);
		foreach (XElement item in xDocument.Element("data").Elements("Achievement"))
		{
			XAttribute xAttribute = item.Attribute("id");
			XAttribute xAttribute2 = item.Attribute("progress");
			XAttribute xAttribute3 = item.Attribute("completed");
			XAttribute xAttribute4 = item.Attribute("synced");
			if (xAttribute2 != null)
			{
				double.TryParse(xAttribute2.Value, out adh.progress);
			}
			else
			{
				adh.progress = 0.0;
			}
			if (xAttribute3 != null)
			{
				bool.TryParse(xAttribute3.Value, out adh.completed);
			}
			else
			{
				adh.completed = false;
			}
			if (xAttribute4 != null)
			{
				bool.TryParse(xAttribute4.Value, out adh.synced);
			}
			else
			{
				adh.synced = false;
			}
			SetAchievement(xAttribute.Value, adh);
		}
	}

	public void SetAchievement(string id, AchievementDataHolder adh)
	{
		m_achievementData[id] = adh;
		Save();
	}

	public AchievementDataHolder GetAchievement(string id)
	{
		if (m_achievementData.ContainsKey(id))
		{
			return m_achievementData[id];
		}
		return default(AchievementDataHolder);
	}

	private void InitializeAchievementLimits()
	{
		foreach (AchievementDescriptor achievement in m_achievementList)
		{
			m_achievementLimits.Add(achievement.id, achievement);
		}
		m_limitsInitialized = true;
	}

	public int GetAchievementLimit(string id)
	{
		if (!m_limitsInitialized)
		{
			InitializeAchievementLimits();
		}
		if (m_achievementLimits.TryGetValue(id, out var value))
		{
			return (int)value.limit;
		}
		throw new KeyNotFoundException("Non-existant achievement: " + id);
	}

	private void Awake()
	{
		SetAsPersistant();
		m_fileName = Application.persistentDataPath + "/Achievements.xml";
		m_useEncryption = true;
		m_crypto = new CryptoUtility("fHHg5#%3RRfnJi78&%lP?65");
		InitializeAchievementLimits();
		if (Load())
		{
			return;
		}
		AchievementDataHolder value = default(AchievementDataHolder);
		foreach (KeyValuePair<string, AchievementDescriptor> achievementLimit in m_achievementLimits)
		{
			value.progress = 0.0;
			value.completed = false;
			value.synced = false;
			m_achievementData.Add(achievementLimit.Key, value);
		}
		Save();
	}
}
