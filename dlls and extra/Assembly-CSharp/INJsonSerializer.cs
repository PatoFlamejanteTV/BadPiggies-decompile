using System;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public static class INJsonSerializer
{
	public class Vector2Converter : JsonConverter<Vector2>
	{
		public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return ((string)reader.Value).ToVector2();
		}

		public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
		{
			writer.WriteValue(value.Vector2ToString());
		}
	}

	public class Vector3Converter : JsonConverter<Vector3>
	{
		public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return ((string)reader.Value).ToVector3();
		}

		public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
		{
			writer.WriteValue(value.Vector3ToString());
		}
	}

	public class ColorConverter : JsonConverter<Color>
	{
		public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return ((string)reader.Value).ToColor32();
		}

		public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
		{
			writer.WriteValue(INConvert.Color32ToString(value));
		}
	}

	public class Color32Converter : JsonConverter<Color32>
	{
		public override Color32 ReadJson(JsonReader reader, Type objectType, Color32 existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return ((string)reader.Value).ToColor32();
		}

		public override void WriteJson(JsonWriter writer, Color32 value, JsonSerializer serializer)
		{
			writer.WriteValue(value.Color32ToString());
		}
	}

	public class ConverterContractResolver : DefaultContractResolver
	{
		private static readonly ConverterContractResolver s_instance = new ConverterContractResolver();

		public static ConverterContractResolver Instance => s_instance;

		protected override JsonContract CreateContract(Type objectType)
		{
			JsonContract jsonContract = base.CreateContract(objectType);
			JsonConverter[] s_converters = INJsonSerializer.s_converters;
			foreach (JsonConverter jsonConverter in s_converters)
			{
				if (jsonConverter.CanConvert(objectType))
				{
					jsonContract.Converter = jsonConverter;
					break;
				}
			}
			return jsonContract;
		}
	}

	private static JsonConverter[] s_converters = new JsonConverter[6]
	{
		new StringEnumConverter(),
		new VersionConverter(),
		new Vector2Converter(),
		new Vector3Converter(),
		new ColorConverter(),
		new Color32Converter()
	};

	private static JsonSerializerSettings s_settings = new JsonSerializerSettings
	{
		Formatting = Formatting.Indented,
		ContractResolver = ConverterContractResolver.Instance
	};

	public static string Serialize<T>(T value)
	{
		return Serialize(value, indented: true);
	}

	public static string Serialize<T>(T value, bool indented)
	{
		StringWriter stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
		Serialize(value, indented, stringWriter);
		return stringWriter.ToString();
	}

	public static void Serialize<T>(T value, TextWriter writer)
	{
		Serialize(value, indented: true, writer);
	}

	public static void Serialize<T>(T value, bool indented, TextWriter writer)
	{
		JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(s_settings);
		jsonSerializer.Formatting = (indented ? Formatting.Indented : Formatting.None);
		using JsonTextWriter jsonTextWriter = new JsonTextWriter(writer);
		jsonTextWriter.Indentation = 1;
		jsonTextWriter.IndentChar = '\t';
		jsonTextWriter.Formatting = jsonSerializer.Formatting;
		jsonSerializer.Serialize(jsonTextWriter, value, null);
	}

	public static T Deserialize<T>(string json)
	{
		return Deserialize<T>(new StringReader(json));
	}

	public static T Deserialize<T>(TextReader reader)
	{
		JsonSerializer jsonSerializer = JsonSerializer.CreateDefault(s_settings);
		object obj;
		using (JsonTextReader reader2 = new JsonTextReader(reader))
		{
			obj = jsonSerializer.Deserialize(reader2, typeof(T));
		}
		return (T)obj;
	}
}
