using System;
using UnityEngine;

public static class INConvert
{
	private static (BasePart.PartType[], SortedPartType[]) s_partTypes;

	static INConvert()
	{
		s_partTypes.Item1 = new BasePart.PartType[46];
		s_partTypes.Item2 = new SortedPartType[49];
		for (int i = 0; i < 46; i++)
		{
			SortedPartType sortedPartType = (SortedPartType)i;
			string value = sortedPartType.ToString();
			ref BasePart.PartType reference = ref s_partTypes.Item1[i];
			if (!Enum.TryParse<BasePart.PartType>(value, out reference))
			{
				reference = BasePart.PartType.Unknown;
			}
		}
		for (int j = 0; j < 49; j++)
		{
			BasePart.PartType partType = (BasePart.PartType)j;
			string value2 = partType.ToString();
			ref SortedPartType reference2 = ref s_partTypes.Item2[j];
			if (!Enum.TryParse<SortedPartType>(value2, out reference2))
			{
				reference2 = SortedPartType.Unknown;
			}
		}
	}

	public static string ValueToString<T>(this T value)
	{
		if (value is Vector2 vector)
		{
			return vector.Vector2ToString();
		}
		if (value is Vector3 vector2)
		{
			return vector2.Vector3ToString();
		}
		return value.ToString();
	}

	public static string ArrayToString<T>(this T[] array)
	{
		string text = "[";
		for (int i = 0; i < array.Length; i++)
		{
			text += array[i].ToString();
			if (i != array.Length - 1)
			{
				text += "; ";
			}
		}
		return text + "]";
	}

	public static string Vector2ToString(this Vector2 vector)
	{
		return vector.Vector2ToString(null);
	}

	public static string Vector2ToString(this Vector2 vector, string format)
	{
		return "(" + vector.x.ToString(format) + ", " + vector.y.ToString(format) + ")";
	}

	public static string Vector2ToString(this Vector3 vector)
	{
		return vector.Vector2ToString(null);
	}

	public static string Vector2ToString(this Vector3 vector, string format)
	{
		return "(" + vector.x.ToString(format) + ", " + vector.y.ToString(format) + ")";
	}

	public static string Vector3ToString(this Vector3 vector)
	{
		return vector.Vector3ToString(null);
	}

	public static string Vector3ToString(this Vector3 vector, string format)
	{
		return "(" + vector.x.ToString(format) + ", " + vector.y.ToString(format) + ", " + vector.z.ToString(format) + ")";
	}

	public static string Color32ToString(this Color32 color)
	{
		return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
	}

	public static T ToValue<T>(this string str) where T : struct
	{
		return str.ToValue<T>(ignoreCase: false);
	}

	public static T ToValue<T>(this string str, bool ignoreCase) where T : struct
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle.IsPrimitive)
		{
			return str.ToPrimitive<T>();
		}
		if (typeFromHandle.IsEnum)
		{
			return str.ToEnum<T>(ignoreCase);
		}
		throw new FormatException();
	}

	public static bool TryToValue<T>(this string str, out T value) where T : struct
	{
		return str.TryToValue<T>(ignoreCase: false, out value);
	}

	public static bool TryToValue<T>(this string str, bool ignoreCase, out T value) where T : struct
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle.IsPrimitive)
		{
			return str.TryToPrimitive<T>(out value);
		}
		if (typeFromHandle.IsEnum)
		{
			return str.TryToEnum<T>(ignoreCase, out value);
		}
		value = default(T);
		return false;
	}

	public static T[] ToArray<T>(this string str) where T : struct
	{
		string[] array = str.Substring(1, str.Length - 2).Split(';');
		T[] array2 = new T[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].ToValue<T>();
		}
		return array2;
	}

	public static bool TryToArray<T>(this string str, out T[] result) where T : struct
	{
		if (str.Length >= 2)
		{
			string[] array = str.Substring(1, str.Length - 2).Split(',');
			T[] array2 = new T[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].TryToValue<T>(out array2[i]))
				{
					result = null;
					return false;
				}
			}
			result = array2;
			return true;
		}
		result = null;
		return false;
	}

	public static T ToPrimitive<T>(this string str) where T : struct
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle == typeof(bool))
		{
			return (T)(object)bool.Parse(str);
		}
		if (typeFromHandle == typeof(char))
		{
			return (T)(object)char.Parse(str);
		}
		if (typeFromHandle == typeof(sbyte))
		{
			return (T)(object)sbyte.Parse(str);
		}
		if (typeFromHandle == typeof(byte))
		{
			return (T)(object)byte.Parse(str);
		}
		if (typeFromHandle == typeof(short))
		{
			return (T)(object)short.Parse(str);
		}
		if (typeFromHandle == typeof(ushort))
		{
			return (T)(object)ushort.Parse(str);
		}
		if (typeFromHandle == typeof(int))
		{
			return (T)(object)int.Parse(str);
		}
		if (typeFromHandle == typeof(uint))
		{
			return (T)(object)uint.Parse(str);
		}
		if (typeFromHandle == typeof(long))
		{
			return (T)(object)long.Parse(str);
		}
		if (typeFromHandle == typeof(ulong))
		{
			return (T)(object)ulong.Parse(str);
		}
		if (typeFromHandle == typeof(float))
		{
			return (T)(object)float.Parse(str);
		}
		if (typeFromHandle == typeof(double))
		{
			return (T)(object)double.Parse(str);
		}
		throw new FormatException();
	}

	public static bool TryToPrimitive<T>(this string str, out T value) where T : struct
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle == typeof(bool))
		{
			bool result;
			bool result2 = bool.TryParse(str, out result);
			value = (T)(object)result;
			return result2;
		}
		if (typeFromHandle == typeof(char))
		{
			char result3;
			bool result4 = char.TryParse(str, out result3);
			value = (T)(object)result3;
			return result4;
		}
		if (typeFromHandle == typeof(sbyte))
		{
			sbyte result5;
			bool result6 = sbyte.TryParse(str, out result5);
			value = (T)(object)result5;
			return result6;
		}
		if (typeFromHandle == typeof(byte))
		{
			byte result7;
			bool result8 = byte.TryParse(str, out result7);
			value = (T)(object)result7;
			return result8;
		}
		if (typeFromHandle == typeof(short))
		{
			short result9;
			bool result10 = short.TryParse(str, out result9);
			value = (T)(object)result9;
			return result10;
		}
		if (typeFromHandle == typeof(ushort))
		{
			ushort result11;
			bool result12 = ushort.TryParse(str, out result11);
			value = (T)(object)result11;
			return result12;
		}
		if (typeFromHandle == typeof(int))
		{
			int result13;
			bool result14 = int.TryParse(str, out result13);
			value = (T)(object)result13;
			return result14;
		}
		if (typeFromHandle == typeof(uint))
		{
			uint result15;
			bool result16 = uint.TryParse(str, out result15);
			value = (T)(object)result15;
			return result16;
		}
		if (typeFromHandle == typeof(long))
		{
			long result17;
			bool result18 = long.TryParse(str, out result17);
			value = (T)(object)result17;
			return result18;
		}
		if (typeFromHandle == typeof(ulong))
		{
			ulong result19;
			bool result20 = ulong.TryParse(str, out result19);
			value = (T)(object)result19;
			return result20;
		}
		if (typeFromHandle == typeof(float))
		{
			float result21;
			bool result22 = float.TryParse(str, out result21);
			value = (T)(object)result21;
			return result22;
		}
		if (typeFromHandle == typeof(double))
		{
			double result23;
			bool result24 = double.TryParse(str, out result23);
			value = (T)(object)result23;
			return result24;
		}
		value = default(T);
		return false;
	}

	public static T ToEnum<T>(this string str) where T : struct
	{
		return (T)Enum.Parse(typeof(T), str);
	}

	public static bool TryToEnum<T>(this string str, out T value) where T : struct
	{
		T result;
		bool result2 = Enum.TryParse<T>(str, out result);
		value = result;
		return result2;
	}

	public static T ToEnum<T>(this string str, bool ignoreCase) where T : struct
	{
		return (T)Enum.Parse(typeof(T), str, ignoreCase);
	}

	public static bool TryToEnum<T>(this string str, bool ignoreCase, out T value) where T : struct
	{
		T result;
		bool result2 = Enum.TryParse<T>(str, ignoreCase, out result);
		value = result;
		return result2;
	}

	public static Vector2 ToVector2(this string str)
	{
		string[] array = str.Substring(1, str.Length - 2).Split(',');
		float x = float.Parse(array[0]);
		float y = float.Parse(array[1]);
		return new Vector2(x, y);
	}

	public static bool TryToVector2(this string str, out Vector2 vector)
	{
		if (str.Length >= 2)
		{
			string[] array = str.Substring(1, str.Length - 2).Split(',');
			if (array.Length >= 2 && float.TryParse(array[0], out var result) && float.TryParse(array[1], out var result2))
			{
				vector = new Vector2(result, result2);
				return true;
			}
		}
		vector = default(Vector2);
		return false;
	}

	public static Vector3 ToVector3(this string str)
	{
		string[] array = str.Substring(1, str.Length - 2).Split(',');
		float x = float.Parse(array[0]);
		float y = float.Parse(array[1]);
		float z = float.Parse(array[2]);
		return new Vector3(x, y, z);
	}

	public static Color32 ToColor32(this string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			throw new ArgumentException();
		}
		if (str[0] != '#')
		{
			throw new ArgumentException();
		}
		byte r = (byte)Convert.ToInt32(str.Substring(1, 2), 16);
		byte g = (byte)Convert.ToInt32(str.Substring(3, 2), 16);
		byte b = (byte)Convert.ToInt32(str.Substring(5, 2), 16);
		if (str.Length <= 7)
		{
			return new Color32(r, g, b, byte.MaxValue);
		}
		byte a = (byte)Convert.ToInt32(str.Substring(7, 2), 16);
		return new Color32(r, g, b, a);
	}

	public static BasePart.PartType ToPartType(this SortedPartType partType)
	{
		if (partType < SortedPartType.Unknown || partType >= SortedPartType.MAX)
		{
			return (BasePart.PartType)partType;
		}
		return s_partTypes.Item1[(int)partType];
	}

	public static SortedPartType ToSortedPartType(this BasePart.PartType partType)
	{
		if (partType < BasePart.PartType.Unknown || partType >= BasePart.PartType.MAX)
		{
			return (SortedPartType)partType;
		}
		return s_partTypes.Item2[(int)partType];
	}
}
