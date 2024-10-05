using System;
using System.Collections.Generic;

namespace Pathfinding.Serialization.JsonFx;

public class JsonReaderSettings
{
	internal readonly TypeCoercionUtility Coercion = new TypeCoercionUtility();

	private bool allowUnquotedObjectKeys;

	private string typeHintName;

	protected List<JsonConverter> converters = new List<JsonConverter>();

	public bool HandleCyclicReferences { get; }

	public bool AllowUnquotedObjectKeys => allowUnquotedObjectKeys;

	internal bool IsTypeHintName(string name)
	{
		if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(typeHintName))
		{
			return StringComparer.Ordinal.Equals(typeHintName, name);
		}
		return false;
	}

	public virtual JsonConverter GetConverter(Type type)
	{
		for (int i = 0; i < converters.Count; i++)
		{
			if (converters[i].CanConvert(type))
			{
				return converters[i];
			}
		}
		return null;
	}
}
