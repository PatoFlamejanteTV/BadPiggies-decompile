namespace PlayFab.Json;

public static class JsonWrapper
{
	private static ISerializer _instance = new SimpleJsonInstance();

	public static ISerializer Instance
	{
		get
		{
			return _instance;
		}
		set
		{
			_instance = value;
		}
	}

	public static T DeserializeObject<T>(string json)
	{
		return _instance.DeserializeObject<T>(json);
	}

	public static T DeserializeObject<T>(string json, object jsonSerializerStrategy)
	{
		return _instance.DeserializeObject<T>(json, jsonSerializerStrategy);
	}

	public static object DeserializeObject(string json)
	{
		return _instance.DeserializeObject(json);
	}

	public static string SerializeObject(object json)
	{
		return _instance.SerializeObject(json);
	}

	public static string SerializeObject(object json, object jsonSerializerStrategy)
	{
		return _instance.SerializeObject(json, jsonSerializerStrategy);
	}
}
