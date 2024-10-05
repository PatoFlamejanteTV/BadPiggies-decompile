using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class RuntimeSpriteDatabase : Singleton<RuntimeSpriteDatabase>
{
	private Dictionary<string, SpriteData> m_data;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		Singleton<RuntimeSpriteDatabase>.instance = this;
	}

	public void SetData(List<SpriteData> data)
	{
		if (m_data != null)
		{
			m_data.Clear();
		}
		else
		{
			m_data = new Dictionary<string, SpriteData>();
		}
		foreach (SpriteData datum in data)
		{
			m_data[datum.id] = datum;
		}
	}

	public SpriteData Find(string id)
	{
		if (m_data == null)
		{
			Load();
		}
		m_data.TryGetValue(id, out var value);
		return value;
	}

	private void Load()
	{
		LoadFast();
	}

	public void Load(List<SpriteData> data)
	{
		data.Clear();
		using (MemoryStream stream = new MemoryStream(((TextAsset)Resources.Load("GUISystem/Sprites", typeof(TextAsset))).bytes, writable: false))
		{
			using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
			string text = streamReader.ReadToEnd();
			char[] separator = new char[1] { '\n' };
			string[] array = text.Split(separator);
			foreach (string text2 in array)
			{
				char[] separator2 = new char[1] { '\t' };
				string[] array2 = text2.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
				if (array2.Length == 14)
				{
					string text3 = array2[1].Substring(1, array2[1].Length - 2);
					string materialId = array2[2];
					SpriteData item = new SpriteData(array2[0], text3, materialId, null, int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), int.Parse(array2[10]), int.Parse(array2[11]), int.Parse(array2[12]), int.Parse(array2[13]), 0, string.Empty, default(Rect));
					data.Add(item);
				}
				else if (array2.Length == 15)
				{
					string text4 = array2[1].Substring(1, array2[1].Length - 2);
					string materialId2 = array2[2];
					SpriteData item2 = new SpriteData(array2[0], text4, materialId2, null, int.Parse(array2[3]), int.Parse(array2[4]), int.Parse(array2[5]), int.Parse(array2[6]), int.Parse(array2[7]), int.Parse(array2[8]), int.Parse(array2[9]), int.Parse(array2[10]), int.Parse(array2[11]), int.Parse(array2[12]), int.Parse(array2[13]), int.Parse(array2[14]), string.Empty, default(Rect));
					data.Add(item2);
				}
			}
		}
		using MemoryStream stream2 = new MemoryStream(((TextAsset)Resources.Load("GUISystem/SpriteMapping", typeof(TextAsset))).bytes, writable: false);
		using StreamReader streamReader2 = new StreamReader(stream2, Encoding.UTF8);
		string text5 = streamReader2.ReadToEnd();
		char[] separator3 = new char[1] { '\n' };
		char[] separator4 = new char[1] { '\t' };
		string[] array = text5.Split(separator3, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array3 = array[i].Split(separator4, StringSplitOptions.RemoveEmptyEntries);
			if (array3.Length != 5)
			{
				continue;
			}
			string text6 = array3[0];
			float x = float.Parse(array3[1], CultureInfo.InvariantCulture);
			float y = float.Parse(array3[2], CultureInfo.InvariantCulture);
			float width = float.Parse(array3[3], CultureInfo.InvariantCulture);
			float height = float.Parse(array3[4], CultureInfo.InvariantCulture);
			Rect uv = new Rect(x, y, width, height);
			for (int j = 0; j < data.Count; j++)
			{
				if (data[j].id == text6)
				{
					data[j].uv = uv;
					break;
				}
			}
		}
	}

	public void LoadFast()
	{
		Dictionary<string, SpriteData> dictionary = new Dictionary<string, SpriteData>();
		TextAsset textAsset = Resources.Load<TextAsset>("GUISystem/Sprites");
		char[] separator = new char[1] { '\t' };
		StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries;
		using (MemoryStream stream = new MemoryStream(textAsset.bytes, writable: false))
		{
			using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
			while (!streamReader.EndOfStream)
			{
				string[] array = streamReader.ReadLine().Split(separator, options);
				string text = array[0];
				if (array.Length == 14 || array.Length == 15)
				{
					string text2 = array[1].Substring(1, array[1].Length - 2);
					int opaqueBorderPixels = ((array.Length != 14) ? int.Parse(array[14]) : 0);
					SpriteData value = new SpriteData(text, text2, array[2], null, int.Parse(array[3]), int.Parse(array[4]), int.Parse(array[5]), int.Parse(array[6]), int.Parse(array[7]), int.Parse(array[8]), int.Parse(array[9]), int.Parse(array[10]), int.Parse(array[11]), int.Parse(array[12]), int.Parse(array[13]), opaqueBorderPixels, string.Empty, default(Rect));
					dictionary.Add(text, value);
				}
			}
		}
		textAsset = Resources.Load<TextAsset>("GUISystem/SpriteMapping");
		using (MemoryStream stream2 = new MemoryStream(textAsset.bytes, writable: false))
		{
			using StreamReader streamReader2 = new StreamReader(stream2, Encoding.UTF8);
			while (!streamReader2.EndOfStream)
			{
				string[] array2 = streamReader2.ReadLine().Split(separator, options);
				if (array2.Length == 5)
				{
					string key = array2[0];
					float x = float.Parse(array2[1], CultureInfo.InvariantCulture);
					float y = float.Parse(array2[2], CultureInfo.InvariantCulture);
					float width = float.Parse(array2[3], CultureInfo.InvariantCulture);
					float height = float.Parse(array2[4], CultureInfo.InvariantCulture);
					Rect uv = new Rect(x, y, width, height);
					if (dictionary.TryGetValue(key, out var value2))
					{
						value2.uv = uv;
					}
				}
			}
		}
		m_data = dictionary;
	}
}
