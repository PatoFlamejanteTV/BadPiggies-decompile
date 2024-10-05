using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Pathfinding.Serialization.JsonFx;

internal class TypeCoercionUtility
{
	private Dictionary<Type, Dictionary<string, MemberInfo>> memberMapCache;

	private bool allowNullValueTypes = true;

	private Dictionary<Type, Dictionary<string, MemberInfo>> MemberMapCache
	{
		get
		{
			if (memberMapCache == null)
			{
				memberMapCache = new Dictionary<Type, Dictionary<string, MemberInfo>>();
			}
			return memberMapCache;
		}
	}

	public static Type GetTypeInfo(Type tp)
	{
		return tp;
	}

	internal object ProcessTypeHint(IDictionary result, string typeInfo, out Type objectType, out Dictionary<string, MemberInfo> memberMap)
	{
		if (string.IsNullOrEmpty(typeInfo))
		{
			objectType = null;
			memberMap = null;
			return result;
		}
		Type type = Type.GetType(typeInfo, throwOnError: false);
		if (object.Equals(type, null))
		{
			objectType = null;
			memberMap = null;
			return result;
		}
		objectType = type;
		return CoerceType(type, result, out memberMap);
	}

	internal object ProcessTypeHint(object result, string typeInfo, out Type objectType, out Dictionary<string, MemberInfo> memberMap)
	{
		Type type = Type.GetType(typeInfo, throwOnError: false);
		if (object.Equals(type, null))
		{
			objectType = null;
			memberMap = null;
			return result;
		}
		objectType = type;
		return InstantiateObject(objectType, out memberMap);
	}

	internal object InstantiateObject(Type objectType)
	{
		return Activator.CreateInstance(objectType);
	}

	internal object InstantiateObject(Type objectType, out Dictionary<string, MemberInfo> memberMap)
	{
		object result = InstantiateObject(objectType);
		memberMap = GetMemberMap(objectType);
		return result;
	}

	public Dictionary<string, MemberInfo> GetMemberMap(Type objectType)
	{
		if (GetTypeInfo(typeof(IDictionary)).IsAssignableFrom(GetTypeInfo(objectType)))
		{
			return null;
		}
		return CreateMemberMap(objectType);
	}

	private Dictionary<string, MemberInfo> CreateMemberMap(Type objectType)
	{
		if (MemberMapCache.TryGetValue(objectType, out var value))
		{
			return value;
		}
		value = new Dictionary<string, MemberInfo>();
		Type type = objectType;
		while (type != null)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanRead && propertyInfo.CanWrite && !JsonIgnoreAttribute.IsJsonIgnore(propertyInfo))
				{
					string jsonName = JsonNameAttribute.GetJsonName(propertyInfo);
					if (string.IsNullOrEmpty(jsonName))
					{
						value[propertyInfo.Name] = propertyInfo;
					}
					else
					{
						value[jsonName] = propertyInfo;
					}
				}
			}
			FieldInfo[] fields = GetTypeInfo(objectType).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				if ((fieldInfo.IsPublic || fieldInfo.GetCustomAttributes(typeof(JsonMemberAttribute), inherit: true).Length != 0) && !JsonIgnoreAttribute.IsJsonIgnore(fieldInfo))
				{
					string jsonName2 = JsonNameAttribute.GetJsonName(fieldInfo);
					if (string.IsNullOrEmpty(jsonName2))
					{
						value[fieldInfo.Name] = fieldInfo;
					}
					else
					{
						value[jsonName2] = fieldInfo;
					}
				}
			}
			type = type.BaseType;
		}
		MemberMapCache[objectType] = value;
		return value;
	}

	internal static Type GetMemberInfo(Dictionary<string, MemberInfo> memberMap, string memberName, out MemberInfo memberInfo)
	{
		if (memberMap != null && memberMap.TryGetValue(memberName, out memberInfo))
		{
			if (memberInfo is PropertyInfo)
			{
				return ((PropertyInfo)memberInfo).PropertyType;
			}
			if (memberInfo is FieldInfo)
			{
				return ((FieldInfo)memberInfo).FieldType;
			}
		}
		memberInfo = null;
		return null;
	}

	internal void SetMemberValue(object result, Type memberType, MemberInfo memberInfo, object value)
	{
		if (memberInfo is PropertyInfo)
		{
			((PropertyInfo)memberInfo).SetValue(result, CoerceType(memberType, value), null);
		}
		else if (memberInfo is FieldInfo)
		{
			((FieldInfo)memberInfo).SetValue(result, CoerceType(memberType, value));
		}
	}

	internal object CoerceType(Type targetType, object value)
	{
		bool flag = IsNullable(targetType);
		if (value == null)
		{
			if (!allowNullValueTypes && GetTypeInfo(targetType).IsValueType && !flag)
			{
				throw new JsonTypeCoercionException($"{targetType.FullName} does not accept null as a value");
			}
			return value;
		}
		if (flag)
		{
			Type[] genericArguments = targetType.GetGenericArguments();
			if (genericArguments.Length == 1)
			{
				targetType = genericArguments[0];
			}
		}
		Type type = value.GetType();
		if (GetTypeInfo(targetType).IsAssignableFrom(GetTypeInfo(type)))
		{
			return value;
		}
		if (GetTypeInfo(targetType).IsEnum)
		{
			if (value is string)
			{
				if (!Enum.IsDefined(targetType, value))
				{
					FieldInfo[] fields = GetTypeInfo(targetType).GetFields();
					foreach (FieldInfo fieldInfo in fields)
					{
						string jsonName = JsonNameAttribute.GetJsonName(fieldInfo);
						if (((string)value).Equals(jsonName))
						{
							value = fieldInfo.Name;
							break;
						}
					}
				}
				return Enum.Parse(targetType, (string)value);
			}
			value = CoerceType(Enum.GetUnderlyingType(targetType), value);
			return Enum.ToObject(targetType, value);
		}
		Dictionary<string, MemberInfo> memberMap;
		if (value is IDictionary)
		{
			return CoerceType(targetType, (IDictionary)value, out memberMap);
		}
		if (GetTypeInfo(typeof(IEnumerable)).IsAssignableFrom(GetTypeInfo(targetType)) && GetTypeInfo(typeof(IEnumerable)).IsAssignableFrom(GetTypeInfo(type)))
		{
			return CoerceList(targetType, type, (IEnumerable)value);
		}
		if (value is string)
		{
			if (object.Equals(targetType, typeof(DateTime)))
			{
				if (DateTime.TryParse((string)value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.RoundtripKind, out var result))
				{
					return result;
				}
			}
			else
			{
				if (object.Equals(targetType, typeof(Guid)))
				{
					return new Guid((string)value);
				}
				if (object.Equals(targetType, typeof(char)))
				{
					if (((string)value).Length == 1)
					{
						return ((string)value)[0];
					}
				}
				else if (object.Equals(targetType, typeof(Uri)))
				{
					if (Uri.TryCreate((string)value, UriKind.RelativeOrAbsolute, out var result2))
					{
						return result2;
					}
				}
				else if (object.Equals(targetType, typeof(Version)))
				{
					return new Version((string)value);
				}
			}
		}
		else if (object.Equals(targetType, typeof(TimeSpan)))
		{
			return new TimeSpan((long)CoerceType(typeof(long), value));
		}
		TypeConverter converter = TypeDescriptor.GetConverter(targetType);
		if (converter.CanConvertFrom(type))
		{
			return converter.ConvertFrom(value);
		}
		converter = TypeDescriptor.GetConverter(type);
		if (converter.CanConvertTo(targetType))
		{
			return converter.ConvertTo(value, targetType);
		}
		try
		{
			return Convert.ChangeType(value, targetType);
		}
		catch (Exception innerException)
		{
			throw new JsonTypeCoercionException(string.Format("Error converting {0} to {1}", new object[2]
			{
				value.GetType().FullName,
				targetType.FullName
			}), innerException);
		}
	}

	private object CoerceType(Type targetType, IDictionary value, out Dictionary<string, MemberInfo> memberMap)
	{
		object result = InstantiateObject(targetType, out memberMap);
		if (memberMap != null)
		{
			foreach (object key in value.Keys)
			{
				MemberInfo memberInfo;
				Type memberInfo2 = GetMemberInfo(memberMap, key as string, out memberInfo);
				SetMemberValue(result, memberInfo2, memberInfo, value[key]);
			}
		}
		return result;
	}

	private object CoerceList(Type targetType, Type arrayType, IEnumerable value)
	{
		if (targetType.IsArray)
		{
			return CoerceArray(targetType.GetElementType(), value);
		}
		ConstructorInfo[] constructors = targetType.GetConstructors();
		ConstructorInfo constructorInfo = null;
		ConstructorInfo[] array = constructors;
		foreach (ConstructorInfo constructorInfo2 in array)
		{
			ParameterInfo[] parameters = constructorInfo2.GetParameters();
			if (parameters.Length == 0)
			{
				constructorInfo = constructorInfo2;
			}
			else if (parameters.Length == 1 && GetTypeInfo(parameters[0].ParameterType).IsAssignableFrom(GetTypeInfo(arrayType)))
			{
				try
				{
					return constructorInfo2.Invoke(new object[1] { value });
				}
				catch
				{
				}
			}
		}
		if (object.Equals(constructorInfo, null))
		{
			throw new JsonTypeCoercionException($"Only objects with default constructors can be deserialized. ({targetType.FullName})");
		}
		object obj2;
		try
		{
			obj2 = constructorInfo.Invoke(null);
		}
		catch (TargetInvocationException ex)
		{
			if (ex.InnerException != null)
			{
				throw new JsonTypeCoercionException(ex.InnerException.Message, ex.InnerException);
			}
			throw new JsonTypeCoercionException("Error instantiating " + targetType.FullName, ex);
		}
		MethodInfo method = GetTypeInfo(targetType).GetMethod("AddRange");
		ParameterInfo[] array2 = ((!object.Equals(method, null)) ? method.GetParameters() : null);
		Type type = ((array2 != null && array2.Length == 1) ? array2[0].ParameterType : null);
		if (!object.Equals(type, null) && GetTypeInfo(type).IsAssignableFrom(GetTypeInfo(arrayType)))
		{
			try
			{
				method.Invoke(obj2, new object[1] { value });
				return obj2;
			}
			catch (TargetInvocationException ex2)
			{
				if (ex2.InnerException != null)
				{
					throw new JsonTypeCoercionException(ex2.InnerException.Message, ex2.InnerException);
				}
				throw new JsonTypeCoercionException("Error calling AddRange on " + targetType.FullName, ex2);
			}
		}
		method = GetTypeInfo(targetType).GetMethod("Add");
		array2 = ((!object.Equals(method, null)) ? method.GetParameters() : null);
		type = ((array2 != null && array2.Length == 1) ? array2[0].ParameterType : null);
		if (!object.Equals(type, null))
		{
			foreach (object item in value)
			{
				try
				{
					method.Invoke(obj2, new object[1] { CoerceType(type, item) });
				}
				catch (TargetInvocationException ex3)
				{
					if (ex3.InnerException != null)
					{
						throw new JsonTypeCoercionException(ex3.InnerException.Message, ex3.InnerException);
					}
					throw new JsonTypeCoercionException("Error calling Add on " + targetType.FullName, ex3);
				}
			}
			return obj2;
		}
		try
		{
			return Convert.ChangeType(value, targetType);
		}
		catch (Exception innerException)
		{
			throw new JsonTypeCoercionException(string.Format("Error converting {0} to {1}", new object[2]
			{
				value.GetType().FullName,
				targetType.FullName
			}), innerException);
		}
	}

	private Array CoerceArray(Type elementType, IEnumerable value)
	{
		int num = 0;
		foreach (object item in value)
		{
			_ = item;
			num++;
		}
		Array array = Array.CreateInstance(elementType, new int[1] { num });
		int num2 = 0;
		foreach (object item2 in value)
		{
			array.SetValue(CoerceType(elementType, item2), new int[1] { num2 });
			num2++;
		}
		return array;
	}

	private static bool IsNullable(Type type)
	{
		if (GetTypeInfo(type).IsGenericType)
		{
			return typeof(Nullable<>).Equals(type.GetGenericTypeDefinition());
		}
		return false;
	}
}
