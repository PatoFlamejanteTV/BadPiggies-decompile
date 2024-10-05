using System;

public class RefVariant : IEquatable<RefVariant>
{
	private Type m_type;

	private object m_objectValue;

	public RefVariant(object value)
	{
		m_type = value.GetType();
		m_objectValue = value;
	}

	public object ToObject()
	{
		return m_objectValue;
	}

	public override string ToString()
	{
		return m_objectValue.ToString();
	}

	public bool Equals(RefVariant other)
	{
		return this == other;
	}

	public static bool operator ==(RefVariant left, RefVariant right)
	{
		if (left.m_type != right.m_type)
		{
			return false;
		}
		return object.Equals(left, right);
	}

	public static bool operator !=(RefVariant left, RefVariant right)
	{
		return !(left == right);
	}
}
