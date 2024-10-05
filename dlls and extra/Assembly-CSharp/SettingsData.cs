using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

public class SettingsData
{
	private string m_fileName;

	private bool m_useEncryption;

	private Dictionary<string, object> m_data = new Dictionary<string, object>();

	private bool m_changedSinceSave;

	private CryptoUtility m_crypto;

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

	public bool ChangedSinceLastSave => m_changedSinceSave;

	public SettingsData(string fileName, bool useEncryption, string key)
	{
		m_fileName = fileName;
		m_useEncryption = useEncryption;
		m_crypto = new CryptoUtility(key);
	}

	public T Get<T>(string key, T defaultValue)
	{
		if (m_data.TryGetValue(key, out var value))
		{
			return (T)value;
		}
		return defaultValue;
	}

	public void SetInt(string key, int value)
	{
		SetValue(key, value);
	}

	public int GetInt(string key, int defaultValue)
	{
		return Get(key, defaultValue);
	}

	public int AddToInt(string key, int delta, int minValue = int.MinValue, int maxValue = int.MaxValue)
	{
		if (m_data.TryGetValue(key, out var value))
		{
			int num = Mathf.Clamp((int)value + delta, minValue, maxValue);
			SetValue(key, num);
			return num;
		}
		int num2 = Mathf.Clamp(delta, minValue, maxValue);
		SetValue(key, num2);
		return num2;
	}

	public void SetFloat(string key, float value)
	{
		SetValue(key, value);
	}

	public float GetFloat(string key, float defaultValue)
	{
		return Get(key, defaultValue);
	}

	public void SetString(string key, string value)
	{
		SetValue(key, value);
	}

	public string GetString(string key, string defaultValue)
	{
		return Get(key, defaultValue);
	}

	public void SetBool(string key, bool value)
	{
		SetValue(key, value);
	}

	public bool GetBool(string key, bool defaultValue)
	{
		return Get(key, defaultValue);
	}

	public bool HasKey(string key)
	{
		return m_data.ContainsKey(key);
	}

	public void DeleteKey(string key)
	{
		Change(delegate
		{
			m_data.Remove(key);
		});
	}

	public void DeleteAll()
	{
		Change(delegate
		{
			m_data.Clear();
		});
	}

	public bool Save()
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
		foreach (KeyValuePair<string, object> datum in m_data)
		{
			xmlWriter.WriteStartElement(datum.Value.GetType().Name);
			xmlWriter.WriteAttributeString("key", datum.Key);
			xmlWriter.WriteAttributeString("value", datum.Value.ToString());
			xmlWriter.WriteEndElement();
		}
		xmlWriter.WriteEndElement();
		xmlWriter.WriteEndDocument();
		xmlWriter.Close();
		if (m_useEncryption)
		{
			byte[] array = m_crypto.Encrypt(memoryStream.ToArray());
			byte[] array2 = CryptoUtility.ComputeHash(array);
			return TransactionalFileWrite(m_fileName, array2, array);
		}
		return TransactionalFileWrite(m_fileName, memoryStream.ToArray());
	}

	public void Load()
	{
		try
		{
			if (File.Exists(m_fileName))
			{
				Load(m_fileName);
				return;
			}
		}
		catch
		{
		}
		try
		{
			if (File.Exists(m_fileName + ".bak"))
			{
				Load(m_fileName + ".bak");
				return;
			}
		}
		catch
		{
		}
		try
		{
			if (File.Exists(m_fileName + ".bak2"))
			{
				Load(m_fileName + ".bak2");
			}
		}
		catch
		{
		}
	}

	public void BackupData()
	{
		try
		{
			if (File.Exists(m_fileName) && new FileInfo(m_fileName).Length > 0)
			{
				File.Copy(m_fileName, m_fileName + ".bak2", overwrite: true);
			}
		}
		catch
		{
		}
	}

	private void Load(string fileName)
	{
		FileStream fileStream = new FileStream(fileName, FileMode.Open);
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
	}

	public void LoadXml(Stream stream)
	{
		XDocument xDocument = XDocument.Load(stream);
		DeleteAll();
		foreach (XElement item in xDocument.Element("data").Elements())
		{
			XAttribute xAttribute = item.Attribute("key");
			XAttribute xAttribute2 = item.Attribute("value");
			if (item.Name == "Int32")
			{
				if (int.TryParse(xAttribute2.Value, out var result))
				{
					SetInt(xAttribute.Value, result);
				}
			}
			else if (item.Name == "Single")
			{
				if (float.TryParse(xAttribute2.Value, out var result2))
				{
					SetFloat(xAttribute.Value, result2);
				}
			}
			else if (item.Name == "Boolean")
			{
				if (bool.TryParse(xAttribute2.Value, out var result3))
				{
					SetBool(xAttribute.Value, result3);
				}
			}
			else if (item.Name == "String")
			{
				SetString(xAttribute.Value, xAttribute2.Value);
			}
		}
	}

	public void Set<T>(string key, T value)
	{
		SetValue(key, value);
	}

	private void SetValue(string key, object value)
	{
		Change(delegate
		{
			m_data[key] = value;
		});
	}

	private void Change(Action ChangeData)
	{
		ChangeData();
		m_changedSinceSave = true;
	}

	private bool TransactionalFileWrite(string filename, params byte[][] args)
	{
		string text = filename + ".tmp";
		try
		{
			using (FileStream fileStream = new FileStream(text, FileMode.Create))
			{
				foreach (byte[] array in args)
				{
					fileStream.Write(array, 0, array.Length);
				}
			}
			if (File.Exists(filename))
			{
				File.Replace(text, filename, filename + ".bak");
			}
			else
			{
				File.Move(text, filename);
			}
			m_changedSinceSave = false;
		}
		catch
		{
			return false;
		}
		return true;
	}
}
