using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace PlayFab.Json;

[GeneratedCode("simple-json", "1.0.0")]
public static class PlayFabSimpleJson
{
	private enum TokenType : byte
	{
		NONE,
		CURLY_OPEN,
		CURLY_CLOSE,
		SQUARED_OPEN,
		SQUARED_CLOSE,
		COLON,
		COMMA,
		STRING,
		NUMBER,
		TRUE,
		FALSE,
		NULL
	}

	private const int BUILDER_INIT = 2000;

	private static readonly char[] EscapeTable;

	private static readonly char[] EscapeCharacters;

	internal static readonly List<Type> NumberTypes;

	[ThreadStatic]
	private static StringBuilder _serializeObjectBuilder;

	[ThreadStatic]
	private static StringBuilder _parseStringBuilder;

	private static IJsonSerializerStrategy _currentJsonSerializerStrategy;

	private static PocoJsonSerializerStrategy _pocoJsonSerializerStrategy;

	public static IJsonSerializerStrategy CurrentJsonSerializerStrategy
	{
		get
		{
			IJsonSerializerStrategy result;
			if ((result = _currentJsonSerializerStrategy) == null)
			{
				result = (_currentJsonSerializerStrategy = PocoJsonSerializerStrategy);
			}
			return result;
		}
		set
		{
			_currentJsonSerializerStrategy = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static PocoJsonSerializerStrategy PocoJsonSerializerStrategy
	{
		get
		{
			PocoJsonSerializerStrategy result;
			if ((result = _pocoJsonSerializerStrategy) == null)
			{
				result = (_pocoJsonSerializerStrategy = new PocoJsonSerializerStrategy());
			}
			return result;
		}
	}

	static PlayFabSimpleJson()
	{
		EscapeCharacters = new char[7] { '"', '\\', '\b', '\f', '\n', '\r', '\t' };
		NumberTypes = new List<Type>
		{
			typeof(bool),
			typeof(byte),
			typeof(ushort),
			typeof(uint),
			typeof(ulong),
			typeof(sbyte),
			typeof(short),
			typeof(int),
			typeof(long),
			typeof(double),
			typeof(float),
			typeof(decimal)
		};
		EscapeTable = new char[93];
		EscapeTable[34] = '"';
		EscapeTable[92] = '\\';
		EscapeTable[8] = 'b';
		EscapeTable[12] = 'f';
		EscapeTable[10] = 'n';
		EscapeTable[13] = 'r';
		EscapeTable[9] = 't';
	}

	public static object DeserializeObject(string json)
	{
		if (TryDeserializeObject(json, out var obj))
		{
			return obj;
		}
		throw new SerializationException("Invalid JSON string");
	}

	public static bool TryDeserializeObject(string json, out object obj)
	{
		bool success = true;
		if (json != null)
		{
			int index = 0;
			obj = ParseValue(json, ref index, ref success);
		}
		else
		{
			obj = null;
		}
		return success;
	}

	public static object DeserializeObject(string json, Type type, IJsonSerializerStrategy jsonSerializerStrategy = null)
	{
		object obj = DeserializeObject(json);
		if (type == null || (obj != null && ReflectionUtils.IsAssignableFrom(obj.GetType(), type)))
		{
			return obj;
		}
		return (jsonSerializerStrategy ?? CurrentJsonSerializerStrategy).DeserializeObject(obj, type);
	}

	public static T DeserializeObject<T>(string json, IJsonSerializerStrategy jsonSerializerStrategy = null)
	{
		return (T)DeserializeObject(json, typeof(T), jsonSerializerStrategy);
	}

	public static string SerializeObject(object json, IJsonSerializerStrategy jsonSerializerStrategy = null)
	{
		if (_serializeObjectBuilder == null)
		{
			_serializeObjectBuilder = new StringBuilder(2000);
		}
		_serializeObjectBuilder.Length = 0;
		if (jsonSerializerStrategy == null)
		{
			jsonSerializerStrategy = CurrentJsonSerializerStrategy;
		}
		if (SerializeValue(jsonSerializerStrategy, json, _serializeObjectBuilder))
		{
			return _serializeObjectBuilder.ToString();
		}
		return null;
	}

	public static string EscapeToJavascriptString(string jsonString)
	{
		if (string.IsNullOrEmpty(jsonString))
		{
			return jsonString;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		while (num < jsonString.Length)
		{
			char c = jsonString[num++];
			if (c == '\\')
			{
				if (jsonString.Length - num >= 2)
				{
					switch (jsonString[num])
					{
					case '\\':
						stringBuilder.Append('\\');
						num++;
						break;
					case '"':
						stringBuilder.Append("\"");
						num++;
						break;
					case 't':
						stringBuilder.Append('\t');
						num++;
						break;
					case 'b':
						stringBuilder.Append('\b');
						num++;
						break;
					case 'n':
						stringBuilder.Append('\n');
						num++;
						break;
					case 'r':
						stringBuilder.Append('\r');
						num++;
						break;
					}
				}
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	private static IDictionary<string, object> ParseObject(string json, ref int index, ref bool success)
	{
		IDictionary<string, object> dictionary = new JsonObject();
		NextToken(json, ref index);
		bool flag = false;
		while (!flag)
		{
			switch (LookAhead(json, index))
			{
			case TokenType.NONE:
				success = false;
				return null;
			case TokenType.COMMA:
				NextToken(json, ref index);
				continue;
			case TokenType.CURLY_CLOSE:
				NextToken(json, ref index);
				return dictionary;
			}
			string key = ParseString(json, ref index, ref success);
			if (!success)
			{
				success = false;
				return null;
			}
			TokenType tokenType = NextToken(json, ref index);
			if (tokenType != TokenType.COLON)
			{
				success = false;
				return null;
			}
			object value = ParseValue(json, ref index, ref success);
			if (!success)
			{
				success = false;
				return null;
			}
			dictionary[key] = value;
		}
		return dictionary;
	}

	private static JsonArray ParseArray(string json, ref int index, ref bool success)
	{
		JsonArray jsonArray = new JsonArray();
		NextToken(json, ref index);
		bool flag = false;
		while (!flag)
		{
			switch (LookAhead(json, index))
			{
			case TokenType.NONE:
				success = false;
				return null;
			case TokenType.COMMA:
				NextToken(json, ref index);
				continue;
			case TokenType.SQUARED_CLOSE:
				break;
			default:
			{
				object item = ParseValue(json, ref index, ref success);
				if (!success)
				{
					return null;
				}
				jsonArray.Add(item);
				continue;
			}
			}
			NextToken(json, ref index);
			break;
		}
		return jsonArray;
	}

	private static object ParseValue(string json, ref int index, ref bool success)
	{
		switch (LookAhead(json, index))
		{
		case TokenType.CURLY_OPEN:
			return ParseObject(json, ref index, ref success);
		case TokenType.SQUARED_OPEN:
			return ParseArray(json, ref index, ref success);
		case TokenType.STRING:
			return ParseString(json, ref index, ref success);
		case TokenType.NUMBER:
			return ParseNumber(json, ref index, ref success);
		case TokenType.TRUE:
			NextToken(json, ref index);
			return true;
		case TokenType.FALSE:
			NextToken(json, ref index);
			return false;
		case TokenType.NULL:
			NextToken(json, ref index);
			return null;
		default:
			success = false;
			return null;
		}
	}

	private static string ParseString(string json, ref int index, ref bool success)
	{
		if (_parseStringBuilder == null)
		{
			_parseStringBuilder = new StringBuilder(2000);
		}
		_parseStringBuilder.Length = 0;
		EatWhitespace(json, ref index);
		char c = json[index++];
		bool flag = false;
		while (!flag && index != json.Length)
		{
			c = json[index++];
			switch (c)
			{
			case '"':
				flag = true;
				break;
			case '\\':
			{
				if (index == json.Length)
				{
					break;
				}
				switch (json[index++])
				{
				case '"':
					_parseStringBuilder.Append('"');
					continue;
				case '\\':
					_parseStringBuilder.Append('\\');
					continue;
				case '/':
					_parseStringBuilder.Append('/');
					continue;
				case 'b':
					_parseStringBuilder.Append('\b');
					continue;
				case 'f':
					_parseStringBuilder.Append('\f');
					continue;
				case 'n':
					_parseStringBuilder.Append('\n');
					continue;
				case 'r':
					_parseStringBuilder.Append('\r');
					continue;
				case 't':
					_parseStringBuilder.Append('\t');
					continue;
				case 'u':
					break;
				default:
					continue;
				}
				if (json.Length - index < 4)
				{
					break;
				}
				if (!(success = uint.TryParse(json.Substring(index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result)))
				{
					return string.Empty;
				}
				if (55296 <= result && result <= 56319)
				{
					index += 4;
					if (json.Length - index < 6 || !(json.Substring(index, 2) == "\\u") || !uint.TryParse(json.Substring(index + 2, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result2) || 56320 > result2 || result2 > 57343)
					{
						success = false;
						return string.Empty;
					}
					_parseStringBuilder.Append((char)result);
					_parseStringBuilder.Append((char)result2);
					index += 6;
				}
				else
				{
					_parseStringBuilder.Append(ConvertFromUtf32((int)result));
					index += 4;
				}
				continue;
			}
			default:
				_parseStringBuilder.Append(c);
				continue;
			}
			break;
		}
		if (!flag)
		{
			success = false;
			return null;
		}
		return _parseStringBuilder.ToString();
	}

	private static string ConvertFromUtf32(int utf32)
	{
		if (utf32 < 0 || utf32 > 1114111)
		{
			throw new ArgumentOutOfRangeException("utf32", "The argument must be from 0 to 0x10FFFF.");
		}
		if (55296 <= utf32 && utf32 <= 57343)
		{
			throw new ArgumentOutOfRangeException("utf32", "The argument must not be in surrogate pair range.");
		}
		if (utf32 < 65536)
		{
			return new string((char)utf32, 1);
		}
		utf32 -= 65536;
		return new string(new char[2]
		{
			(char)((utf32 >> 10) + 55296),
			(char)(utf32 % 1024 + 56320)
		});
	}

	private static object ParseNumber(string json, ref int index, ref bool success)
	{
		EatWhitespace(json, ref index);
		int lastIndexOfNumber = GetLastIndexOfNumber(json, index);
		int length = lastIndexOfNumber - index + 1;
		string text = json.Substring(index, length);
		object result2;
		if (text.IndexOf(".", StringComparison.OrdinalIgnoreCase) != -1 || text.IndexOf("e", StringComparison.OrdinalIgnoreCase) != -1)
		{
			success = double.TryParse(json.Substring(index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out var result);
			result2 = result;
		}
		else if (text.IndexOf("-", StringComparison.OrdinalIgnoreCase) == -1)
		{
			success = ulong.TryParse(json.Substring(index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out var result3);
			result2 = result3;
		}
		else
		{
			success = long.TryParse(json.Substring(index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out var result4);
			result2 = result4;
		}
		index = lastIndexOfNumber + 1;
		return result2;
	}

	private static int GetLastIndexOfNumber(string json, int index)
	{
		int i;
		for (i = index; i < json.Length && "0123456789+-.eE".IndexOf(json[i]) != -1; i++)
		{
		}
		return i - 1;
	}

	private static void EatWhitespace(string json, ref int index)
	{
		while (index < json.Length && " \t\n\r\b\f".IndexOf(json[index]) != -1)
		{
			index++;
		}
	}

	private static TokenType LookAhead(string json, int index)
	{
		int index2 = index;
		return NextToken(json, ref index2);
	}

	private static TokenType NextToken(string json, ref int index)
	{
		EatWhitespace(json, ref index);
		if (index == json.Length)
		{
			return TokenType.NONE;
		}
		char c = json[index];
		index++;
		switch (c)
		{
		case ',':
			return TokenType.COMMA;
		case '-':
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			return TokenType.NUMBER;
		case '[':
			return TokenType.SQUARED_OPEN;
		case '{':
			return TokenType.CURLY_OPEN;
		case '"':
			return TokenType.STRING;
		default:
		{
			index--;
			int num = json.Length - index;
			if (num >= 5 && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
			{
				index += 5;
				return TokenType.FALSE;
			}
			if (num >= 4 && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
			{
				index += 4;
				return TokenType.TRUE;
			}
			if (num >= 4 && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
			{
				index += 4;
				return TokenType.NULL;
			}
			return TokenType.NONE;
		}
		case '}':
			return TokenType.CURLY_CLOSE;
		case ']':
			return TokenType.SQUARED_CLOSE;
		case ':':
			return TokenType.COLON;
		}
	}

	private static bool SerializeValue(IJsonSerializerStrategy jsonSerializerStrategy, object value, StringBuilder builder)
	{
		bool flag = true;
		string text = value as string;
		if (value == null)
		{
			builder.Append("null");
		}
		else if (text != null)
		{
			flag = SerializeString(text, builder);
		}
		else
		{
			IDictionary<string, object> dictionary = value as IDictionary<string, object>;
			Type type = value.GetType();
			Type[] genericTypeArguments = ReflectionUtils.GetGenericTypeArguments(type);
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<, >) && genericTypeArguments[0] == typeof(string))
			{
				IDictionary dictionary2 = value as IDictionary;
				flag = SerializeObject(jsonSerializerStrategy, dictionary2.Keys, dictionary2.Values, builder);
			}
			else if (dictionary != null)
			{
				flag = SerializeObject(jsonSerializerStrategy, dictionary.Keys, dictionary.Values, builder);
			}
			else if (value is IDictionary<string, string> dictionary3)
			{
				flag = SerializeObject(jsonSerializerStrategy, dictionary3.Keys, dictionary3.Values, builder);
			}
			else if (value is IEnumerable anArray)
			{
				flag = SerializeArray(jsonSerializerStrategy, anArray, builder);
			}
			else if (IsNumeric(value))
			{
				flag = SerializeNumber(value, builder);
			}
			else if (value is bool)
			{
				builder.Append((!(bool)value) ? "false" : "true");
			}
			else
			{
				flag = jsonSerializerStrategy.TrySerializeNonPrimitiveObject(value, out var output);
				if (flag)
				{
					SerializeValue(jsonSerializerStrategy, output, builder);
				}
			}
		}
		return flag;
	}

	private static bool SerializeObject(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable keys, IEnumerable values, StringBuilder builder)
	{
		builder.Append("{");
		IEnumerator enumerator = keys.GetEnumerator();
		IEnumerator enumerator2 = values.GetEnumerator();
		bool flag = true;
		while (enumerator.MoveNext() && enumerator2.MoveNext())
		{
			object current = enumerator.Current;
			object current2 = enumerator2.Current;
			if (!flag)
			{
				builder.Append(",");
			}
			if (current is string aString)
			{
				SerializeString(aString, builder);
			}
			else if (!SerializeValue(jsonSerializerStrategy, current2, builder))
			{
				return false;
			}
			builder.Append(":");
			if (!SerializeValue(jsonSerializerStrategy, current2, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append("}");
		return true;
	}

	private static bool SerializeArray(IJsonSerializerStrategy jsonSerializerStrategy, IEnumerable anArray, StringBuilder builder)
	{
		builder.Append("[");
		bool flag = true;
		foreach (object item in anArray)
		{
			if (!flag)
			{
				builder.Append(",");
			}
			if (!SerializeValue(jsonSerializerStrategy, item, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append("]");
		return true;
	}

	private static bool SerializeString(string aString, StringBuilder builder)
	{
		if (aString.IndexOfAny(EscapeCharacters) == -1)
		{
			builder.Append('"');
			builder.Append(aString);
			builder.Append('"');
			return true;
		}
		builder.Append('"');
		int num = 0;
		char[] array = aString.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			if (c >= EscapeTable.Length || EscapeTable[(uint)c] == '\0')
			{
				num++;
				continue;
			}
			if (num > 0)
			{
				builder.Append(array, i - num, num);
				num = 0;
			}
			builder.Append('\\');
			builder.Append(EscapeTable[(uint)c]);
		}
		if (num > 0)
		{
			builder.Append(array, array.Length - num, num);
		}
		builder.Append('"');
		return true;
	}

	private static bool SerializeNumber(object number, StringBuilder builder)
	{
		if (number is decimal)
		{
			builder.Append(((decimal)number).ToString("R", CultureInfo.InvariantCulture));
		}
		else if (number is double)
		{
			builder.Append(((double)number).ToString("R", CultureInfo.InvariantCulture));
		}
		else if (number is float)
		{
			builder.Append(((float)number).ToString("R", CultureInfo.InvariantCulture));
		}
		else if (NumberTypes.IndexOf(number.GetType()) != -1)
		{
			builder.Append(number);
		}
		return true;
	}

	private static bool IsNumeric(object value)
	{
		if (!(value is sbyte) && !(value is byte) && !(value is short) && !(value is ushort) && !(value is int) && !(value is uint) && !(value is long) && !(value is ulong) && !(value is float) && !(value is double))
		{
			return value is decimal;
		}
		return true;
	}
}
