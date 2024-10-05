using System;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class INSettings
{
	private enum SettingScope
	{
		None,
		Level,
		Sandbox,
		Global
	}

	private enum SettingTypeCode
	{
		Empty,
		Boolean,
		Int32,
		Single,
		String,
		Array
	}

	private class SettingType
	{
		private SettingTypeCode m_mainType;

		private SettingTypeCode[] m_genericArguments;

		public SettingTypeCode MainType => m_mainType;

		public SettingTypeCode[] GenericArguments => m_genericArguments;

		public bool IsGeneric => m_genericArguments != null;

		public SettingType(SettingTypeCode mainType, SettingTypeCode[] genericArguments)
		{
			m_mainType = mainType;
			m_genericArguments = genericArguments;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(m_mainType.ToString());
			if (m_genericArguments != null && m_genericArguments.Length != 0)
			{
				stringBuilder.Append('<');
				stringBuilder.Append(m_genericArguments[0].ToString());
				for (int i = 1; i < m_genericArguments.Length; i++)
				{
					stringBuilder.Append(", ");
					stringBuilder.Append(m_genericArguments[i].ToString());
				}
				stringBuilder.Append('>');
			}
			return stringBuilder.ToString();
		}

		public static SettingType Parse(string s)
		{
			SettingTypeCode[] array = null;
			int num = s.IndexOf('<');
			int num2 = s.IndexOf('>');
			SettingTypeCode mainType;
			if (num != -1 && num2 != -1)
			{
				mainType = s.Substring(0, num).ToEnum<SettingTypeCode>();
				string[] array2 = s.Substring(num + 1, num2 - num - 1).Split(',');
				array = new SettingTypeCode[array2.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array[i] = array2[i].ToEnum<SettingTypeCode>();
				}
			}
			else
			{
				mainType = s.ToEnum<SettingTypeCode>();
			}
			return new SettingType(mainType, array);
		}
	}

	private class SettingDeclaration
	{
		public SettingType Type { get; private set; }

		public Variant InitialValue { get; private set; }

		public SettingDeclaration(SettingType type, Variant initialValue)
		{
			Type = type;
			InitialValue = initialValue;
		}
	}

	private class SettingDeclarationContainer
	{
		private SettingDeclaration[] m_items;

		public int Count => m_items.Length;

		public SettingDeclarationContainer(int count)
		{
			m_items = new SettingDeclaration[count];
		}

		public SettingDeclaration GetDeclaration(INFeature name)
		{
			return m_items[(int)name];
		}

		public void SetDeclaration(INFeature name, SettingDeclaration data)
		{
			m_items[(int)name] = data;
		}
	}

	private class SettingData
	{
		public SettingScope Scope { get; set; }

		public Variant Value { get; set; }

		public SettingData()
		{
			Scope = SettingScope.None;
			Value = null;
		}

		public SettingData(SettingScope scope, Variant value)
		{
			Scope = scope;
			Value = value;
		}
	}

	private class SettingDataContainer
	{
		private SettingData[] m_items;

		public int Count => m_items.Length;

		public SettingDataContainer(int count)
		{
			m_items = new SettingData[count];
		}

		public SettingDataContainer(SettingDataContainer settings)
		{
			int num = settings.m_items.Length;
			m_items = new SettingData[num];
			Array.Copy(settings.m_items, m_items, num);
		}

		public SettingData GetData(INFeature name)
		{
			return m_items[(int)name];
		}

		public void SetData(INFeature name, SettingData data)
		{
			m_items[(int)name] = data;
		}
	}

	public class WrappedSettingData
	{
		private readonly SettingDeclaration m_declaration;

		private readonly SettingData m_data;

		private WrappedSettingData(SettingDeclaration declaration, SettingData data)
		{
			m_declaration = declaration;
			m_data = data;
		}

		public static WrappedSettingData Create(INFeature name)
		{
			return new WrappedSettingData(s_declarations.GetDeclaration(name), s_runtimeSettings.GetData(name));
		}

		private Variant GetVariant()
		{
			if (!IsEnabled(m_data.Scope))
			{
				return m_declaration.InitialValue;
			}
			return m_data.Value;
		}

		public bool GetBool()
		{
			return (bool)GetVariant().Value;
		}

		public int GetInt()
		{
			return (int)GetVariant().Value;
		}

		public float GetFloat()
		{
			return (float)GetVariant().Value;
		}

		public string GetString()
		{
			return (string)GetVariant().Reference.ToObject();
		}

		public object GetObject()
		{
			return GetVariant().Reference.ToObject();
		}
	}

	[Serializable]
	private class SerializedDeclaration
	{
		public string name;

		public string type;

		public object value;

		public SerializedDeclaration(string name, string type, object value)
		{
			this.name = name;
			this.type = type;
			this.value = value;
		}
	}

	[Serializable]
	private class SerializedDeclarations
	{
		public SerializedDeclaration[] items;

		public SerializedDeclarations(SerializedDeclaration[] items)
		{
			this.items = items;
		}

		public SettingDeclarationContainer Convert()
		{
			SettingDeclarationContainer settingDeclarationContainer = new SettingDeclarationContainer(s_count);
			SerializedDeclaration[] array = items;
			foreach (SerializedDeclaration serializedDeclaration in array)
			{
				INFeature name = serializedDeclaration.name.ToEnum<INFeature>();
				SettingType type = SettingType.Parse(serializedDeclaration.type);
				Variant initialValue = ToVariant(type, serializedDeclaration.value);
				settingDeclarationContainer.SetDeclaration(name, new SettingDeclaration(type, initialValue));
			}
			return settingDeclarationContainer;
		}
	}

	[Serializable]
	private class SerializedSetting
	{
		public string name;

		public string scope;

		public object value;

		public SerializedSetting(string name, string scope, object value)
		{
			this.name = name;
			this.scope = scope;
			this.value = value;
		}
	}

	[Serializable]
	private class SerializedSettings
	{
		public SerializedSetting[] items;

		public SerializedSettings(SerializedSetting[] items)
		{
			this.items = items;
		}

		public SettingDataContainer Convert(SettingDeclarationContainer declarations)
		{
			int s_count = INSettings.s_count;
			SettingDataContainer settingDataContainer = new SettingDataContainer(s_count);
			for (int i = 0; i < s_count; i++)
			{
				INFeature name = (INFeature)i;
				settingDataContainer.SetData(name, new SettingData(SettingScope.Global, declarations.GetDeclaration(name).InitialValue));
			}
			SerializedSetting[] array = items;
			foreach (SerializedSetting serializedSetting in array)
			{
				INFeature name2 = serializedSetting.name.ToEnum<INFeature>();
				SettingScope scope = serializedSetting.scope.ToEnum<SettingScope>();
				Variant value = ToVariant(declarations.GetDeclaration(name2).Type, serializedSetting.value);
				settingDataContainer.SetData(name2, new SettingData(scope, value));
			}
			return settingDataContainer;
		}
	}

	private static readonly int s_count = Enum.GetNames(typeof(INFeature)).Length;

	private static bool s_versionSelected;

	private static int s_versionType;

	private static SettingDeclarationContainer s_declarations;

	private static SettingDataContainer s_defaultSettings;

	private static SettingDataContainer s_runtimeSettings;

	private static Action[] s_settingEditedEvents = new Action[s_count];

	public static bool VersionSelected => s_versionSelected;

	public static int VersionType => s_versionType;

	public static void Initialize(int version)
	{
		s_versionSelected = true;
		Load(version);
		InitializeSettings();
	}

	private static void Load(int version)
	{
		string text = version switch
		{
			2 => "A", 
			1 => "O", 
			0 => string.Empty, 
			_ => "B", 
		};
		s_versionType = version;
		s_declarations = INJsonSerializer.Deserialize<SerializedDeclarations>(INUnity.LoadTextAsset("INDeclarationSettings").text).Convert();
		if (version != 0)
		{
			s_defaultSettings = INJsonSerializer.Deserialize<SerializedSettings>(INUnity.LoadTextAsset("INSettings" + text).text).Convert(s_declarations);
		}
		else
		{
			s_defaultSettings = new SettingDataContainer(s_count);
			for (int i = 0; i < s_count; i++)
			{
				INFeature name = (INFeature)i;
				s_defaultSettings.SetData(name, new SettingData());
			}
		}
		s_runtimeSettings = new SettingDataContainer(s_defaultSettings);
	}

	public static bool IsEnabled(INFeature name)
	{
		return IsEnabled(GetScope(name));
	}

	private static bool IsEnabled(SettingScope scope)
	{
		return scope switch
		{
			SettingScope.None => false, 
			SettingScope.Level => !IsSandboxMode(), 
			SettingScope.Sandbox => IsSandboxMode(), 
			SettingScope.Global => true, 
			_ => false, 
		};
	}

	public static bool GetBool(INFeature name)
	{
		return (bool)GetVariant(name).Value;
	}

	public static int GetInt(INFeature name)
	{
		return (int)GetVariant(name).Value;
	}

	public static float GetFloat(INFeature name)
	{
		return (float)GetVariant(name).Value;
	}

	public static string GetString(INFeature name)
	{
		return (string)GetVariant(name).Reference.ToObject();
	}

	public static object GetObject(INFeature name)
	{
		return GetVariant(name).Reference.ToObject();
	}

	public static bool GetInitialBool(INFeature name)
	{
		return (bool)GetInitialVariant(name).Value;
	}

	public static int GetInitialInt(INFeature name)
	{
		return (int)GetInitialVariant(name).Value;
	}

	public static float GetInitialFloat(INFeature name)
	{
		return (float)GetInitialVariant(name).Value;
	}

	public static string GetInitialString(INFeature name)
	{
		return (string)GetInitialVariant(name).Reference.ToObject();
	}

	public static object GetInitialObject(INFeature name)
	{
		return GetInitialVariant(name).Reference.ToObject();
	}

	public static WrappedSettingData GetSettingsData(INFeature name)
	{
		return WrappedSettingData.Create(name);
	}

	private static SettingScope GetScope(INFeature name)
	{
		return s_runtimeSettings.GetData(name).Scope;
	}

	private static Variant GetVariant(INFeature name)
	{
		if (!IsEnabled(name))
		{
			return GetInitialVariant(name);
		}
		return s_runtimeSettings.GetData(name).Value;
	}

	private static Variant GetInitialVariant(INFeature name)
	{
		return s_declarations.GetDeclaration(name).InitialValue;
	}

	private static void SetValue(INFeature name, Variant value)
	{
		Variant variant = GetVariant(name);
		s_runtimeSettings.GetData(name).Value = value;
		Action listener = GetListener(name);
		if (listener != null && variant != GetVariant(name))
		{
			listener();
		}
	}

	private static void SetScope(INFeature name, SettingScope scope)
	{
		Variant variant = GetVariant(name);
		s_runtimeSettings.GetData(name).Scope = scope;
		Action listener = GetListener(name);
		if (listener != null && variant != GetVariant(name))
		{
			listener();
		}
	}

	private static bool IsSandboxMode()
	{
		GameManager instance = Singleton<GameManager>.Instance;
		LevelManager levelManager = WPFMonoBehaviour.levelManager;
		if (instance != null && levelManager != null && instance.CurrentEpisodeType == GameManager.EpisodeType.Sandbox)
		{
			return levelManager.CurrentGameMode is BaseGameMode;
		}
		return false;
	}

	public static void AddListener(INFeature name, Action action)
	{
		ref Action reference = ref s_settingEditedEvents[(int)name];
		reference = (Action)Delegate.Combine(reference, action);
	}

	public static void AddListenerAndInvoke(INFeature name, Action action)
	{
		action();
		ref Action reference = ref s_settingEditedEvents[(int)name];
		reference = (Action)Delegate.Combine(reference, action);
	}

	public static void RemoveListener(INFeature name, Action action)
	{
		ref Action reference = ref s_settingEditedEvents[(int)name];
		reference = (Action)Delegate.Remove(reference, action);
	}

	private static Action GetListener(INFeature name)
	{
		return s_settingEditedEvents[(int)name];
	}

	private static void InitializeSettings()
	{
		UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object.Instantiate(INUnity.LoadGameObject("EventSystem")));
		AddListenerAndInvoke(INFeature.TimeScale, SetTimeScale);
		AddListenerAndInvoke(INFeature.NewContraptionData, INContraptionDataManager.SetContraptionData);
	}

	private static void SetTimeScale()
	{
		Time.timeScale = GetFloat(INFeature.TimeScale);
	}

	private static Variant ToVariant(SettingType type, object obj)
	{
		if (type.MainType.IsValueType() && obj is IConvertible value)
		{
			ValueVariant value2 = new ValueVariant(type.MainType.ToTypeCode(), value);
			return new Variant(in value2);
		}
		if (type.MainType == SettingTypeCode.Array && obj is JArray jArray)
		{
			obj = jArray.ToObject(type.GenericArguments[0] switch
			{
				SettingTypeCode.Boolean => typeof(bool[]), 
				SettingTypeCode.Int32 => typeof(int[]), 
				SettingTypeCode.Single => typeof(float[]), 
				SettingTypeCode.String => typeof(string[]), 
				_ => typeof(object[]), 
			});
		}
		return new Variant(new RefVariant(obj));
	}

	private static bool IsValueType(this SettingTypeCode settingsType)
	{
		if (settingsType != SettingTypeCode.Boolean && settingsType != SettingTypeCode.Int32)
		{
			return settingsType == SettingTypeCode.Single;
		}
		return true;
	}

	private static TypeCode ToTypeCode(this SettingTypeCode settingsType)
	{
		return settingsType switch
		{
			SettingTypeCode.Boolean => TypeCode.Boolean, 
			SettingTypeCode.Int32 => TypeCode.Int32, 
			SettingTypeCode.Single => TypeCode.Single, 
			SettingTypeCode.String => TypeCode.String, 
			_ => TypeCode.Empty, 
		};
	}
}
