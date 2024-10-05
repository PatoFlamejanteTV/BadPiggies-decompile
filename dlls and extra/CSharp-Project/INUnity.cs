using System;
using System.Collections.Generic;
using UnityEngine;

public static class INUnity
{
	public enum VersionType
	{
		Original,
		ModeO,
		ModeA,
		ModeB
	}

	private static Version s_version;

	private static string s_versionText;

	private static string s_dataPath;

	private static string s_settingsPath;

	private static SystemLanguage s_language;

	public static Font ArialFont;

	public static Mesh QuadMesh;

	public static Shader TextShader;

	public static Shader UnlitColorShader;

	public static Shader UnlitColorTransparentShader;

	public static Shader UnlitColorTransparentGrayShader;

	public static Shader UnlitColorTransparentGrayOverlayShader;

	public static Shader UnlitSolidColorShader;

	public static Shader UnlitColorTransparentTextureShader;

	private static Dictionary<string, UnityEngine.Object> s_resources;

	public static bool Enabled => true;

	public static Version Version => s_version;

	public static string VersionText => s_versionText;

	public static string DataPath => s_dataPath;

	public static string SettingsPath => s_settingsPath;

	public static SystemLanguage Language => s_language;

	static INUnity()
	{
		s_version = Version.Parse(Application.version);
		s_versionText = Application.version;
		s_dataPath = Application.persistentDataPath;
		s_settingsPath = Application.persistentDataPath + "/Settings";
		SystemLanguage systemLanguage = Application.systemLanguage;
		if (systemLanguage == SystemLanguage.Chinese || systemLanguage == SystemLanguage.ChineseSimplified || systemLanguage == SystemLanguage.ChineseTraditional)
		{
			s_language = SystemLanguage.Chinese;
		}
		else
		{
			s_language = SystemLanguage.English;
		}
		ArialFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
		QuadMesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
		TextShader = Shader.Find("GUI/Text Shader");
		UnlitColorShader = Shader.Find("Unlit/Color");
		UnlitColorTransparentShader = Shader.Find("_Custom/PreAlpha_Unlit_ColorTransparent_Geometry");
		UnlitColorTransparentGrayShader = Shader.Find("_Custom/PreAlpha_Unlit_ColorTransparent_Geometry_Gray");
		s_resources = new Dictionary<string, UnityEngine.Object>();
	}

	public static void Initialize(INGameData data)
	{
		LoadResources(data.Fonts);
		LoadResources(data.Prefabs);
		LoadResources(data.Materials);
		LoadResources(data.Shaders);
		LoadResources(data.TextAssets);
		UnlitColorTransparentGrayOverlayShader = LoadShader("_Custom/Unlit_ColorTransparent_Geometry_GrayOverlay");
		UnlitSolidColorShader = LoadShader("_Custom/Unlit_ColorTransparent_Geometry_SolidColor");
		UnlitColorTransparentTextureShader = LoadShader("_Custom/Unlit_ColorTransparent_Texture");
	}

	private static void LoadResources<T>(List<T> data) where T : UnityEngine.Object
	{
		foreach (T datum in data)
		{
			if (datum != null)
			{
				s_resources[datum.name] = datum;
			}
		}
	}

	public static Font LoadFont(string name)
	{
		return (Font)s_resources[name];
	}

	public static GameObject LoadGameObject(string name)
	{
		return (GameObject)s_resources[name];
	}

	public static Material LoadMaterial(string name)
	{
		return (Material)s_resources[name];
	}

	public static Shader LoadShader(string name)
	{
		return (Shader)s_resources[name];
	}

	public static TextAsset LoadTextAsset(string name)
	{
		return (TextAsset)s_resources[name];
	}
}
