using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ComponentHelper
{
	private static Dictionary<string, Type> _knownComponents;

	public static Type GetComponentTypeByName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		if (_knownComponents == null)
		{
			_knownComponents = new Dictionary<string, Type>();
			Type typeFromHandle = typeof(Component);
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type[] types = assemblies[i].GetTypes();
				foreach (Type type in types)
				{
					if (typeFromHandle.IsAssignableFrom(type) && !_knownComponents.ContainsKey(type.Name))
					{
						_knownComponents.Add(type.Name, type);
					}
				}
			}
		}
		if (_knownComponents.ContainsKey(name))
		{
			return _knownComponents[name];
		}
		return null;
	}

	public static Component AddComponent(this GameObject go, string name)
	{
		if (go == null)
		{
			throw new ArgumentNullException("go");
		}
		Type componentTypeByName = GetComponentTypeByName(name);
		if (!(componentTypeByName == null))
		{
			return go.AddComponent(componentTypeByName);
		}
		return null;
	}
}
