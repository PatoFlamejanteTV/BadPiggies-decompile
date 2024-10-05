using System;

public class Variant : IEquatable<Variant>
{
	private bool m_isValue;

	private ValueVariant m_value;

	private RefVariant m_reference;

	public bool IsValue => m_isValue;

	public ref readonly ValueVariant Value
	{
		get
		{
			if (m_isValue)
			{
				return ref m_value;
			}
			throw new InvalidCastException("Invalid Variant Type");
		}
	}

	public RefVariant Reference
	{
		get
		{
			if (!m_isValue)
			{
				return m_reference;
			}
			throw new InvalidCastException("Invalid Variant Type");
		}
	}

	public Variant(in ValueVariant value)
	{
		m_isValue = true;
		m_value = value;
	}

	public Variant(RefVariant reference)
	{
		m_isValue = false;
		m_reference = reference;
	}

	public object ToObject()
	{
		if (m_isValue)
		{
			return m_value.ToObject();
		}
		return m_reference.ToObject();
	}

	public override string ToString()
	{
		if (m_isValue)
		{
			return m_value.ToString();
		}
		return m_reference.ToString();
	}

	public bool Equals(Variant other)
	{
		return this == other;
	}

	public static bool operator ==(Variant left, Variant right)
	{
		if (left.m_isValue && right.m_isValue)
		{
			return left.m_value == right.m_value;
		}
		if (!left.m_isValue && !right.m_isValue)
		{
			return left.m_reference == right.m_reference;
		}
		return false;
	}

	public static bool operator !=(Variant left, Variant right)
	{
		return !(left == right);
	}
}
