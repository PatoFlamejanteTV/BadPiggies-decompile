using System.Collections.Generic;

public class DisjointSet
{
	private int m_count;

	private int[] m_parents;

	public int Count => m_count;

	public DisjointSet(int count)
	{
		m_count = count;
		m_parents = new int[count];
		for (int i = 0; i < count; i++)
		{
			m_parents[i] = i;
		}
	}

	public int FindSet(int x)
	{
		int num = m_parents[x];
		if (x == num)
		{
			return num;
		}
		return m_parents[x] = FindSet(num);
	}

	public void Union(int x, int y)
	{
		m_parents[FindSet(x)] = FindSet(y);
	}
}
public class DisjointSet<T>
{
	private Dictionary<T, T> m_parents;

	public DisjointSet()
	{
		m_parents = new Dictionary<T, T>();
	}

	public DisjointSet(int count)
	{
		m_parents = new Dictionary<T, T>(count);
	}

	public T FindSet(T x)
	{
		if (m_parents.TryGetValue(x, out var value))
		{
			if (object.Equals(x, value))
			{
				return value;
			}
			value = FindSet(value);
			m_parents[x] = value;
			return value;
		}
		m_parents[x] = x;
		return x;
	}

	public void Union(T x, T y)
	{
		m_parents[FindSet(x)] = FindSet(y);
	}
}
