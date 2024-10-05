using System;
using System.Reflection;

namespace Pathfinding.Serialization.JsonFx;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class JsonNameAttribute : Attribute
{
	private string jsonName;

	public string Name => jsonName;

	public static string GetJsonName(object value)
	{
		if (value == null)
		{
			return null;
		}
		Type type = value.GetType();
		MemberInfo memberInfo;
		if (TypeCoercionUtility.GetTypeInfo(type).IsEnum)
		{
			string name = Enum.GetName(type, value);
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			memberInfo = TypeCoercionUtility.GetTypeInfo(type).GetField(name);
		}
		else
		{
			memberInfo = value as MemberInfo;
		}
		if (object.Equals(memberInfo, null))
		{
			throw new ArgumentException();
		}
		if (Attribute.GetCustomAttribute(memberInfo, typeof(JsonNameAttribute)) is JsonNameAttribute jsonNameAttribute)
		{
			return jsonNameAttribute.Name;
		}
		return null;
	}
}
