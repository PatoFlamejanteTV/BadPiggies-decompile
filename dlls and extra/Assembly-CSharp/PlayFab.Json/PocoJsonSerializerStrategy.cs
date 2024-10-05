using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace PlayFab.Json;

[GeneratedCode("simple-json", "1.0.0")]
public class PocoJsonSerializerStrategy : IJsonSerializerStrategy
{
	internal IDictionary<Type, ReflectionUtils.ConstructorDelegate> ConstructorCache;

	internal IDictionary<Type, IDictionary<MemberInfo, ReflectionUtils.GetDelegate>> GetCache;

	internal IDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>> SetCache;

	internal static readonly Type[] EmptyTypes = new Type[0];

	internal static readonly Type[] ArrayConstructorParameterTypes = new Type[1] { typeof(int) };

	private static readonly string[] Iso8601Format = new string[3] { "yyyy-MM-dd\\THH:mm:ss.FFFFFFF\\Z", "yyyy-MM-dd\\THH:mm:ss\\Z", "yyyy-MM-dd\\THH:mm:ssK" };

	public PocoJsonSerializerStrategy()
	{
		ConstructorCache = new ReflectionUtils.ThreadSafeDictionary<Type, ReflectionUtils.ConstructorDelegate>(ContructorDelegateFactory);
		GetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<MemberInfo, ReflectionUtils.GetDelegate>>(GetterValueFactory);
		SetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(SetterValueFactory);
	}

	protected virtual string MapClrMemberNameToJsonFieldName(MemberInfo memberInfo)
	{
		object[] customAttributes = memberInfo.GetCustomAttributes(typeof(JsonProperty), inherit: true);
		for (int i = 0; i < customAttributes.Length; i++)
		{
			JsonProperty jsonProperty = (JsonProperty)customAttributes[i];
			if (!string.IsNullOrEmpty(jsonProperty.PropertyName))
			{
				return jsonProperty.PropertyName;
			}
		}
		return memberInfo.Name;
	}

	protected virtual void MapClrMemberNameToJsonFieldName(MemberInfo memberInfo, out string jsonName, out JsonProperty jsonProp)
	{
		jsonName = memberInfo.Name;
		jsonProp = null;
		object[] customAttributes = memberInfo.GetCustomAttributes(typeof(JsonProperty), inherit: true);
		for (int i = 0; i < customAttributes.Length; i++)
		{
			JsonProperty jsonProperty = (jsonProp = (JsonProperty)customAttributes[i]);
			if (!string.IsNullOrEmpty(jsonProperty.PropertyName))
			{
				jsonName = jsonProperty.PropertyName;
			}
		}
	}

	internal virtual ReflectionUtils.ConstructorDelegate ContructorDelegateFactory(Type key)
	{
		return ReflectionUtils.GetContructor(key, (!key.IsArray) ? EmptyTypes : ArrayConstructorParameterTypes);
	}

	internal virtual IDictionary<MemberInfo, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
	{
		IDictionary<MemberInfo, ReflectionUtils.GetDelegate> dictionary = new Dictionary<MemberInfo, ReflectionUtils.GetDelegate>();
		foreach (PropertyInfo property in ReflectionUtils.GetProperties(type))
		{
			if (property.CanRead)
			{
				MethodInfo getterMethodInfo = ReflectionUtils.GetGetterMethodInfo(property);
				if (!getterMethodInfo.IsStatic && getterMethodInfo.IsPublic)
				{
					dictionary[property] = ReflectionUtils.GetGetMethod(property);
				}
			}
		}
		foreach (FieldInfo field in ReflectionUtils.GetFields(type))
		{
			if (!field.IsStatic && field.IsPublic)
			{
				dictionary[field] = ReflectionUtils.GetGetMethod(field);
			}
		}
		return dictionary;
	}

	internal virtual IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
	{
		IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> dictionary = new Dictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>();
		foreach (PropertyInfo property in ReflectionUtils.GetProperties(type))
		{
			if (property.CanWrite)
			{
				MethodInfo setterMethodInfo = ReflectionUtils.GetSetterMethodInfo(property);
				if (!setterMethodInfo.IsStatic && setterMethodInfo.IsPublic)
				{
					dictionary[MapClrMemberNameToJsonFieldName(property)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(property.PropertyType, ReflectionUtils.GetSetMethod(property));
				}
			}
		}
		foreach (FieldInfo field in ReflectionUtils.GetFields(type))
		{
			if (!field.IsInitOnly && !field.IsStatic && field.IsPublic)
			{
				dictionary[MapClrMemberNameToJsonFieldName(field)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(field.FieldType, ReflectionUtils.GetSetMethod(field));
			}
		}
		return dictionary;
	}

	public virtual bool TrySerializeNonPrimitiveObject(object input, out object output)
	{
		if (!TrySerializeKnownTypes(input, out output))
		{
			return TrySerializeUnknownTypes(input, out output);
		}
		return true;
	}

	public virtual object DeserializeObject(object value, Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (value != null && type.IsInstanceOfType(value))
		{
			return value;
		}
		string text = value as string;
		if (type == typeof(Guid) && string.IsNullOrEmpty(text))
		{
			return default(Guid);
		}
		if (value == null)
		{
			return null;
		}
		object result = null;
		if (text != null)
		{
			if (text.Length != 0)
			{
				if (type == typeof(DateTime) || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTime)))
				{
					return DateTime.ParseExact(text, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
				}
				if (type == typeof(DateTimeOffset) || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTimeOffset)))
				{
					return DateTimeOffset.ParseExact(text, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
				}
				if (type == typeof(Guid) || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid)))
				{
					return new Guid(text);
				}
				if (type == typeof(Uri))
				{
					if (Uri.IsWellFormedUriString(text, UriKind.RelativeOrAbsolute) && Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out var result2))
					{
						return result2;
					}
					return null;
				}
				if (type == typeof(string))
				{
					return text;
				}
				return Convert.ChangeType(text, type, CultureInfo.InvariantCulture);
			}
			result = ((type == typeof(Guid)) ? ((object)default(Guid)) : ((!ReflectionUtils.IsNullableType(type) || !(Nullable.GetUnderlyingType(type) == typeof(Guid))) ? text : null));
			if (!ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
			{
				return text;
			}
		}
		else if (value is bool)
		{
			return value;
		}
		bool flag = value is long;
		bool flag2 = value is ulong;
		bool flag3 = value is double;
		Type underlyingType = Nullable.GetUnderlyingType(type);
		if (underlyingType != null && PlayFabSimpleJson.NumberTypes.IndexOf(underlyingType) != -1)
		{
			type = underlyingType;
		}
		bool flag4 = PlayFabSimpleJson.NumberTypes.IndexOf(type) != -1;
		bool isEnum = type.IsEnum;
		if ((flag && type == typeof(long)) || (flag2 && type == typeof(ulong)) || (flag3 && type == typeof(double)))
		{
			return value;
		}
		if ((flag || flag2 || flag3) && isEnum)
		{
			return Enum.ToObject(type, Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture));
		}
		if ((flag || flag2 || flag3) && flag4)
		{
			return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
		}
		if (!(value is IDictionary<string, object> dictionary))
		{
			if (value is IList<object> list)
			{
				IList<object> list2 = list;
				IList list3 = null;
				if (type.IsArray)
				{
					list3 = (IList)ConstructorCache[type](list2.Count);
					int num = 0;
					foreach (object item in list2)
					{
						list3[num++] = DeserializeObject(item, type.GetElementType());
					}
				}
				else if (ReflectionUtils.IsTypeGenericeCollectionInterface(type) || ReflectionUtils.IsAssignableFrom(typeof(IList), type) || type == typeof(object))
				{
					Type genericListElementType = ReflectionUtils.GetGenericListElementType(type);
					ReflectionUtils.ConstructorDelegate constructorDelegate = null;
					if (type != typeof(object))
					{
						constructorDelegate = ConstructorCache[type];
					}
					if (constructorDelegate == null)
					{
						constructorDelegate = ConstructorCache[typeof(List<>).MakeGenericType(genericListElementType)];
					}
					list3 = (IList)constructorDelegate();
					foreach (object item2 in list2)
					{
						list3.Add(DeserializeObject(item2, genericListElementType));
					}
				}
				result = list3;
			}
			return result;
		}
		IDictionary<string, object> dictionary2 = dictionary;
		if (ReflectionUtils.IsTypeDictionary(type))
		{
			Type[] genericTypeArguments = ReflectionUtils.GetGenericTypeArguments(type);
			Type type2 = genericTypeArguments[0];
			Type type3 = genericTypeArguments[1];
			Type key = typeof(Dictionary<, >).MakeGenericType(type2, type3);
			IDictionary dictionary3 = (IDictionary)ConstructorCache[key]();
			foreach (KeyValuePair<string, object> item3 in dictionary2)
			{
				dictionary3.Add(item3.Key, DeserializeObject(item3.Value, type3));
			}
			result = dictionary3;
		}
		else if (type == typeof(object))
		{
			result = value;
		}
		else
		{
			result = ConstructorCache[type]();
			foreach (KeyValuePair<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> item4 in SetCache[type])
			{
				if (dictionary2.TryGetValue(item4.Key, out var value2))
				{
					value2 = DeserializeObject(value2, item4.Value.Key);
					item4.Value.Value(result, value2);
				}
			}
		}
		if (ReflectionUtils.IsNullableType(type))
		{
			return ReflectionUtils.ToNullableType(result, type);
		}
		return result;
	}

	protected virtual object SerializeEnum(Enum p)
	{
		return Convert.ToDouble(p, CultureInfo.InvariantCulture);
	}

	protected virtual bool TrySerializeKnownTypes(object input, out object output)
	{
		bool result = true;
		if (input is DateTime)
		{
			output = ((DateTime)input).ToUniversalTime().ToString(Iso8601Format[0], CultureInfo.InvariantCulture);
		}
		else if (input is DateTimeOffset)
		{
			output = ((DateTimeOffset)input).ToUniversalTime().ToString(Iso8601Format[0], CultureInfo.InvariantCulture);
		}
		else if (input is Guid)
		{
			output = ((Guid)input).ToString("D");
		}
		else if (input is Uri)
		{
			output = input.ToString();
		}
		else if (input is Enum p)
		{
			output = SerializeEnum(p);
		}
		else
		{
			result = false;
			output = null;
		}
		return result;
	}

	protected virtual bool TrySerializeUnknownTypes(object input, out object output)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		output = null;
		Type type = input.GetType();
		if (type.FullName == null)
		{
			return false;
		}
		IDictionary<string, object> dictionary = new JsonObject();
		foreach (KeyValuePair<MemberInfo, ReflectionUtils.GetDelegate> item in GetCache[type])
		{
			if (item.Value != null)
			{
				MapClrMemberNameToJsonFieldName(item.Key, out var jsonName, out var jsonProp);
				if (dictionary.ContainsKey(jsonName))
				{
					throw new Exception("The given key is defined multiple times in the same type: " + input.GetType().Name + "." + jsonName);
				}
				object obj = item.Value(input);
				if (jsonProp == null || jsonProp.NullValueHandling == NullValueHandling.Include || obj != null)
				{
					dictionary.Add(jsonName, obj);
				}
			}
		}
		output = dictionary;
		return true;
	}
}
