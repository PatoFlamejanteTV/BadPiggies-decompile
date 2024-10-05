using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Pathfinding.Serialization.JsonFx;

public class JsonReader
{
	internal static readonly string LiteralFalse = "false";

	internal static readonly string LiteralTrue = "true";

	internal static readonly string LiteralNull = "null";

	internal static readonly string LiteralUndefined = "undefined";

	internal static readonly string LiteralNotANumber = "NaN";

	internal static readonly string LiteralPositiveInfinity = "Infinity";

	internal static readonly string LiteralNegativeInfinity = "-Infinity";

	internal static readonly string TypeGenericIDictionary = "System.Collections.Generic.IDictionary`2";

	private readonly JsonReaderSettings Settings = new JsonReaderSettings();

	private readonly string Source;

	private readonly int SourceLength;

	private int index;

	private int depth;

	private readonly List<object> previouslyDeserialized = new List<object>();

	private readonly Stack<List<object>> jsArrays = new Stack<List<object>>();

	private static StringBuilder builder = new StringBuilder();

	public JsonReader(string input)
		: this(input, new JsonReaderSettings())
	{
	}

	public JsonReader(string input, JsonReaderSettings settings)
	{
		Settings = settings;
		Source = input;
		SourceLength = Source.Length;
	}

	public object Deserialize(int start, Type type)
	{
		index = start;
		depth = -1;
		return Read(type, typeIsHint: false);
	}

	public object Read(Type expectedType, bool typeIsHint)
	{
		depth++;
		if (object.Equals(expectedType, typeof(object)))
		{
			expectedType = null;
		}
		JsonToken jsonToken = Tokenize();
		if (!object.Equals(expectedType, null) && !expectedType.IsPrimitive)
		{
			JsonConverter converter = Settings.GetConverter(expectedType);
			if (converter != null && (depth > 0 || converter.convertAtDepthZero))
			{
				try
				{
					if (!(Read(typeof(Dictionary<string, object>), typeIsHint: false) is Dictionary<string, object> value))
					{
						return null;
					}
					object result = converter.Read(this, expectedType, value);
					depth--;
					return result;
				}
				catch (JsonTypeCoercionException)
				{
				}
				depth--;
				return null;
			}
			if (typeof(IJsonSerializable).IsAssignableFrom(expectedType))
			{
				IJsonSerializable obj = Settings.Coercion.InstantiateObject(expectedType) as IJsonSerializable;
				obj.ReadJson(this);
				depth--;
				return obj;
			}
		}
		switch (jsonToken)
		{
		case JsonToken.Undefined:
			index += LiteralUndefined.Length;
			depth--;
			return null;
		case JsonToken.Null:
			index += LiteralNull.Length;
			depth--;
			return null;
		case JsonToken.False:
			index += LiteralFalse.Length;
			depth--;
			return false;
		case JsonToken.True:
			index += LiteralTrue.Length;
			depth--;
			return true;
		case JsonToken.NaN:
			index += LiteralNotANumber.Length;
			depth--;
			return double.NaN;
		case JsonToken.PositiveInfinity:
			index += LiteralPositiveInfinity.Length;
			depth--;
			return double.PositiveInfinity;
		case JsonToken.NegativeInfinity:
			index += LiteralNegativeInfinity.Length;
			depth--;
			return double.NegativeInfinity;
		case JsonToken.Number:
		{
			object result3 = ReadNumber((!typeIsHint) ? expectedType : null);
			depth--;
			return result3;
		}
		case JsonToken.String:
		{
			object result5 = ReadString((!typeIsHint) ? expectedType : null);
			depth--;
			return result5;
		}
		case JsonToken.ArrayStart:
		{
			IEnumerable result4 = ReadArray((!typeIsHint) ? expectedType : null);
			depth--;
			return result4;
		}
		case JsonToken.ObjectStart:
		{
			object result2 = ReadObject((!typeIsHint) ? expectedType : null);
			depth--;
			return result2;
		}
		default:
			depth--;
			return null;
		}
	}

	private object ReadObject(Type objectType)
	{
		Type genericDictionaryType = null;
		Dictionary<string, MemberInfo> memberMap = null;
		object result;
		if (!object.Equals(objectType, null))
		{
			result = Settings.Coercion.InstantiateObject(objectType, out memberMap);
			previouslyDeserialized.Add(result);
			if (memberMap == null)
			{
				genericDictionaryType = GetGenericDictionaryType(objectType);
			}
		}
		else
		{
			result = new Dictionary<string, object>();
		}
		object obj = result;
		PopulateObject(ref result, objectType, memberMap, genericDictionaryType);
		if (obj != result && !object.Equals(objectType, null))
		{
			previouslyDeserialized.RemoveAt(previouslyDeserialized.Count - 1);
		}
		return result;
	}

	private Type GetGenericDictionaryType(Type objectType)
	{
		Type @interface = TypeCoercionUtility.GetTypeInfo(objectType).GetInterface(TypeGenericIDictionary);
		if (@interface != null)
		{
			Type[] genericArguments = @interface.GetGenericArguments();
			if (genericArguments.Length == 2)
			{
				if (!object.Equals(genericArguments[0], typeof(string)))
				{
					throw new JsonDeserializationException($"Types which implement Generic IDictionary<TKey, TValue> need to have string keys to be deserialized. ({objectType})", index);
				}
				if (!object.Equals(genericArguments[1], typeof(object)))
				{
					return genericArguments[1];
				}
			}
		}
		return null;
	}

	private void PopulateObject(ref object result, Type objectType, Dictionary<string, MemberInfo> memberMap, Type genericDictionaryType)
	{
		if (Source[index] != '{')
		{
			throw new JsonDeserializationException("Expected JSON object.", index);
		}
		IDictionary dictionary = result as IDictionary;
		if (dictionary == null && !object.Equals(TypeCoercionUtility.GetTypeInfo(objectType).GetInterface(TypeGenericIDictionary), null))
		{
			throw new JsonDeserializationException($"Types which implement Generic IDictionary<TKey, TValue> also need to implement IDictionary to be deserialized. ({objectType})", index);
		}
		JsonToken jsonToken;
		do
		{
			index++;
			if (index < SourceLength)
			{
				jsonToken = Tokenize(Settings.AllowUnquotedObjectKeys);
				switch (jsonToken)
				{
				case JsonToken.String:
				case JsonToken.UnquotedName:
				{
					string text = ((jsonToken != JsonToken.String) ? ReadUnquotedKey() : ((string)ReadString(null)));
					Type type;
					MemberInfo memberInfo;
					if (object.Equals(genericDictionaryType, null) && memberMap != null)
					{
						type = TypeCoercionUtility.GetMemberInfo(memberMap, text, out memberInfo);
					}
					else
					{
						type = genericDictionaryType;
						memberInfo = null;
					}
					jsonToken = Tokenize();
					if (jsonToken == JsonToken.NameDelim)
					{
						index++;
						if (index < SourceLength)
						{
							if (Settings.HandleCyclicReferences && text == "@ref")
							{
								int num = (int)Read(typeof(int), typeIsHint: false);
								result = previouslyDeserialized[num];
								jsonToken = Tokenize();
								continue;
							}
							object obj = Read(type, typeIsHint: false);
							if (dictionary != null)
							{
								if (object.Equals(objectType, null) && Settings.IsTypeHintName(text))
								{
									result = Settings.Coercion.ProcessTypeHint(dictionary, obj as string, out objectType, out memberMap);
								}
								else
								{
									dictionary[text] = obj;
								}
							}
							else if (Settings.IsTypeHintName(text))
							{
								result = Settings.Coercion.ProcessTypeHint(result, obj as string, out objectType, out memberMap);
							}
							else
							{
								Settings.Coercion.SetMemberValue(result, type, memberInfo, obj);
							}
							jsonToken = Tokenize();
							continue;
						}
						throw new JsonDeserializationException("Unterminated JSON object.", index);
					}
					throw new JsonDeserializationException("Expected JSON object property name delimiter.", index);
				}
				default:
					throw new JsonDeserializationException("Expected JSON object property name.", index);
				case JsonToken.ObjectEnd:
					break;
				}
				break;
			}
			throw new JsonDeserializationException("Unterminated JSON object.", index);
		}
		while (jsonToken == JsonToken.ValueDelim);
		if (jsonToken != JsonToken.ObjectEnd)
		{
			throw new JsonDeserializationException("Unterminated JSON object.", index);
		}
		index++;
	}

	private IEnumerable ReadArray(Type arrayType)
	{
		if (Source[index] != '[')
		{
			throw new JsonDeserializationException("Expected JSON array.", index);
		}
		bool flag = !object.Equals(arrayType, null);
		bool typeIsHint = !flag;
		Type type = null;
		if (flag)
		{
			if (arrayType.HasElementType)
			{
				type = arrayType.GetElementType();
			}
			else if (TypeCoercionUtility.GetTypeInfo(arrayType).IsGenericType)
			{
				Type[] genericArguments = arrayType.GetGenericArguments();
				if (genericArguments.Length == 1)
				{
					type = genericArguments[0];
				}
			}
		}
		List<object> list = ((jsArrays.Count <= 0) ? new List<object>() : jsArrays.Pop());
		list.Clear();
		JsonToken jsonToken;
		do
		{
			index++;
			if (index < SourceLength)
			{
				jsonToken = Tokenize();
				if (jsonToken == JsonToken.ArrayEnd)
				{
					break;
				}
				object obj = Read(type, typeIsHint);
				list.Add(obj);
				if (obj == null)
				{
					if (!object.Equals(type, null) && TypeCoercionUtility.GetTypeInfo(type).IsValueType)
					{
						type = null;
					}
					flag = true;
				}
				else if (!object.Equals(type, null) && !TypeCoercionUtility.GetTypeInfo(type).IsAssignableFrom(TypeCoercionUtility.GetTypeInfo(obj.GetType())))
				{
					if (TypeCoercionUtility.GetTypeInfo(obj.GetType()).IsAssignableFrom(TypeCoercionUtility.GetTypeInfo(type)))
					{
						type = obj.GetType();
					}
					else
					{
						type = null;
						flag = true;
					}
				}
				else if (!flag)
				{
					type = obj.GetType();
					flag = true;
				}
				jsonToken = Tokenize();
				continue;
			}
			throw new JsonDeserializationException("Unterminated JSON array.", index);
		}
		while (jsonToken == JsonToken.ValueDelim);
		if (jsonToken != JsonToken.ArrayEnd)
		{
			throw new JsonDeserializationException("Unterminated JSON array.", index);
		}
		index++;
		jsArrays.Push(list);
		if (object.Equals(type, null) || object.Equals(type, typeof(object)))
		{
			return list.ToArray();
		}
		if (arrayType != null && arrayType.IsGenericType && object.Equals(arrayType.GetGenericTypeDefinition(), typeof(List<>)))
		{
			IList list2 = Activator.CreateInstance(arrayType, list.Count) as IList;
			for (int i = 0; i < list.Count; i++)
			{
				list2.Add(list[i]);
			}
			return list2;
		}
		Array array = Array.CreateInstance(type, new int[1] { list.Count });
		for (int j = 0; j < list.Count; j++)
		{
			array.SetValue(list[j], new int[1] { j });
		}
		return array;
	}

	private string ReadUnquotedKey()
	{
		int num = index;
		do
		{
			index++;
		}
		while (Tokenize(allowUnquotedString: true) == JsonToken.UnquotedName);
		return Source.Substring(num, index - num);
	}

	private object ReadString(Type expectedType)
	{
		if (Source[index] != '"' && Source[index] != '\'')
		{
			throw new JsonDeserializationException("Expected JSON string.", index);
		}
		char c = Source[index];
		index++;
		if (index >= SourceLength)
		{
			throw new JsonDeserializationException("Unterminated JSON string.", index);
		}
		builder.Length = 0;
		int num = index;
		while (Source[index] != c)
		{
			if (Source[index] == '\\')
			{
				builder.Append(Source, num, index - num);
				index++;
				if (index >= SourceLength)
				{
					throw new JsonDeserializationException("Unterminated JSON string.", index);
				}
				switch (Source[index])
				{
				case 'n':
					builder.Append('\n');
					break;
				default:
					builder.Append(Source[index]);
					break;
				case 'f':
					builder.Append('\f');
					break;
				case 'b':
					builder.Append('\b');
					break;
				case 'r':
					builder.Append('\r');
					break;
				case 't':
					builder.Append('\t');
					break;
				case 'u':
				{
					if (index + 4 < SourceLength && int.TryParse(Source.Substring(index + 1, 4), NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo, out var result))
					{
						builder.Append(char.ConvertFromUtf32(result));
						index += 4;
					}
					else
					{
						builder.Append(Source[index]);
					}
					break;
				}
				case '0':
					break;
				}
				index++;
				if (index >= SourceLength)
				{
					throw new JsonDeserializationException("Unterminated JSON string.", index);
				}
				num = index;
			}
			else
			{
				index++;
				if (index >= SourceLength)
				{
					throw new JsonDeserializationException("Unterminated JSON string.", index);
				}
			}
		}
		builder.Append(Source, num, index - num);
		index++;
		string text = builder.ToString();
		if (!object.Equals(expectedType, null) && !object.Equals(expectedType, typeof(string)))
		{
			return Settings.Coercion.CoerceType(expectedType, text);
		}
		return text;
	}

	private object ReadNumber(Type expectedType)
	{
		bool flag = false;
		bool flag2 = false;
		int num = index;
		int result = 0;
		if (Source[index] == '-')
		{
			index++;
			if (index >= SourceLength || !char.IsDigit(Source[index]))
			{
				throw new JsonDeserializationException("Illegal JSON number.", index);
			}
		}
		while (index < SourceLength && char.IsDigit(Source[index]))
		{
			index++;
		}
		if (index < SourceLength && Source[index] == '.')
		{
			flag = true;
			index++;
			if (index >= SourceLength || !char.IsDigit(Source[index]))
			{
				throw new JsonDeserializationException("Illegal JSON number.", index);
			}
			while (index < SourceLength && char.IsDigit(Source[index]))
			{
				index++;
			}
		}
		int num2 = index - num - (flag ? 1 : 0);
		if (index < SourceLength && (Source[index] == 'e' || Source[index] == 'E'))
		{
			flag2 = true;
			index++;
			if (index >= SourceLength)
			{
				throw new JsonDeserializationException("Illegal JSON number.", index);
			}
			int num3 = index;
			if (Source[index] == '-' || Source[index] == '+')
			{
				index++;
				if (index >= SourceLength || !char.IsDigit(Source[index]))
				{
					throw new JsonDeserializationException("Illegal JSON number.", index);
				}
			}
			else if (!char.IsDigit(Source[index]))
			{
				throw new JsonDeserializationException("Illegal JSON number.", index);
			}
			while (index < SourceLength && char.IsDigit(Source[index]))
			{
				index++;
			}
			int.TryParse(Source.Substring(num3, index - num3), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result);
		}
		string s = Source.Substring(num, index - num);
		if (!flag && !flag2 && num2 < 19)
		{
			decimal num4 = decimal.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
			if (!object.Equals(expectedType, null))
			{
				return Settings.Coercion.CoerceType(expectedType, num4);
			}
			if (num4 >= -2147483648m && num4 <= 2147483647m)
			{
				return (int)num4;
			}
			if (num4 >= -9223372036854775808m && num4 <= 9223372036854775807m)
			{
				return (long)num4;
			}
			return num4;
		}
		if (object.Equals(expectedType, typeof(decimal)))
		{
			return decimal.Parse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
		}
		double num5 = double.Parse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
		if (!object.Equals(expectedType, null))
		{
			return Settings.Coercion.CoerceType(expectedType, num5);
		}
		return num5;
	}

	public static object Deserialize(string value)
	{
		return Deserialize(value, 0, null);
	}

	public static T Deserialize<T>(string value)
	{
		return (T)Deserialize(value, 0, typeof(T));
	}

	public static object Deserialize(string value, int start, Type type)
	{
		return new JsonReader(value).Deserialize(start, type);
	}

	private JsonToken Tokenize()
	{
		return Tokenize(allowUnquotedString: false);
	}

	private JsonToken Tokenize(bool allowUnquotedString)
	{
		if (index >= SourceLength)
		{
			return JsonToken.End;
		}
		while (char.IsWhiteSpace(Source[index]))
		{
			index++;
			if (index >= SourceLength)
			{
				return JsonToken.End;
			}
		}
		if (Source[index] == "/*"[0])
		{
			if (index + 1 >= SourceLength)
			{
				throw new JsonDeserializationException("Illegal JSON sequence. (end of stream while parsing possible comment)", index);
			}
			index++;
			bool flag = false;
			if (Source[index] == "/*"[1])
			{
				flag = true;
			}
			else if (Source[index] != "//"[1])
			{
				throw new JsonDeserializationException("Illegal JSON sequence.", index);
			}
			index++;
			if (flag)
			{
				int num = index - 2;
				if (index + 1 >= SourceLength)
				{
					throw new JsonDeserializationException("Unterminated comment block.", num);
				}
				while (Source[index] != "*/"[0] || Source[index + 1] != "*/"[1])
				{
					index++;
					if (index + 1 >= SourceLength)
					{
						throw new JsonDeserializationException("Unterminated comment block.", num);
					}
				}
				index += 2;
				if (index >= SourceLength)
				{
					return JsonToken.End;
				}
			}
			else
			{
				while ("\r\n".IndexOf(Source[index]) < 0)
				{
					index++;
					if (index >= SourceLength)
					{
						return JsonToken.End;
					}
				}
			}
			while (char.IsWhiteSpace(Source[index]))
			{
				index++;
				if (index >= SourceLength)
				{
					return JsonToken.End;
				}
			}
		}
		if (Source[index] == '+')
		{
			index++;
			if (index >= SourceLength)
			{
				return JsonToken.End;
			}
		}
		switch (Source[index])
		{
		case '[':
			return JsonToken.ArrayStart;
		case '{':
			return JsonToken.ObjectStart;
		case '"':
		case '\'':
			return JsonToken.String;
		case ',':
			return JsonToken.ValueDelim;
		case ':':
			return JsonToken.NameDelim;
		default:
		{
			if (char.IsDigit(Source[index]) || (Source[index] == '-' && index + 1 < SourceLength && char.IsDigit(Source[index + 1])))
			{
				return JsonToken.Number;
			}
			if (MatchLiteral(LiteralFalse))
			{
				return JsonToken.False;
			}
			if (MatchLiteral(LiteralTrue))
			{
				return JsonToken.True;
			}
			if (MatchLiteral(LiteralNull))
			{
				return JsonToken.Null;
			}
			if (MatchLiteral(LiteralNotANumber))
			{
				return JsonToken.NaN;
			}
			if (MatchLiteral(LiteralPositiveInfinity))
			{
				return JsonToken.PositiveInfinity;
			}
			if (MatchLiteral(LiteralNegativeInfinity))
			{
				return JsonToken.NegativeInfinity;
			}
			if (MatchLiteral(LiteralUndefined))
			{
				return JsonToken.Undefined;
			}
			if (allowUnquotedString)
			{
				return JsonToken.UnquotedName;
			}
			string text = Source.Substring(Math.Max(0, index - 5), Math.Min(SourceLength - index - 1, 20));
			throw new JsonDeserializationException("Illegal JSON sequence. (when parsing '" + Source[index] + "' " + (int)Source[index] + ") at index " + index + "\nAround: '" + text + "'", index);
		}
		case '}':
			return JsonToken.ObjectEnd;
		case ']':
			return JsonToken.ArrayEnd;
		}
	}

	private bool MatchLiteral(string literal)
	{
		int length = literal.Length;
		if (index + length > SourceLength)
		{
			return false;
		}
		for (int i = 0; i < length; i++)
		{
			if (literal[i] != Source[index + i])
			{
				return false;
			}
		}
		return true;
	}
}
