using UnityEngine;

public class DeviceInfo
{
	public enum DeviceFamily
	{
		Ios,
		Android,
		Pc,
		Osx,
		BB10,
		WP8
	}

	public static bool UsesTouchInput
	{
		get
		{
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.IPhonePlayer)
			{
				return platform == RuntimePlatform.Android;
			}
			return true;
		}
	}

	public static DeviceFamily ActiveDeviceFamily => DeviceFamily.Android;

	public static bool IsDesktop
	{
		get
		{
			RuntimePlatform platform = Application.platform;
			if (platform != 0 && platform != RuntimePlatform.OSXPlayer && platform != RuntimePlatform.WindowsPlayer)
			{
				return platform == RuntimePlatform.WindowsEditor;
			}
			return true;
		}
	}
}
