using System;

public class DisjointSetFull
{
	private int m_count;

	private int[] m_parents;

	private int[] m_size;

	public int Count
	{
		get
		{
			return m_count;
		}
		set
		{
			m_count = value;
		}
	}

	public int Capacity
	{
		get
		{
			return m_parents.Length;
		}
		set
		{
			int[] array = new int[value];
			int[] array2 = new int[value];
			Array.Copy(m_parents, 0, array, 0, m_count);
			Array.Copy(m_size, 0, array2, 0, m_count);
			m_parents = array;
			m_size = array2;
			MakeSet(m_count, value - 1);
		}
	}

	public DisjointSetFull(int count)
	{
		m_count = count;
		if (count == 0)
		{
			m_parents = Array.Empty<int>();
			m_size = Array.Empty<int>();
		}
		else
		{
			m_parents = new int[count];
			m_size = new int[count];
			MakeSet(0, count - 1);
		}
	}

	public void MakeSet(int start, int end)
	{
		for (int i = start; i <= end; i++)
		{
			m_parents[i] = i;
			m_size[i] = 1;
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
		int num = FindSet(x);
		int num2 = FindSet(y);
		if (num != num2)
		{
			if (m_size[num] > m_size[num2])
			{
				m_size[num] += m_size[num2];
				m_parents[num2] = num;
			}
			else
			{
				m_size[num2] += m_size[num];
				m_parents[num] = num2;
			}
		}
	}

	public int GetSize(int x)
	{
		return m_size[x];
	}

	public void Clear()
	{
		m_count = 0;
		Array.Clear(m_parents, 0, m_parents.Length);
		Array.Clear(m_size, 0, m_size.Length);
	}

	public int[] ToArray()
	{
		int[] array = new int[m_count];
		for (int i = 0; i < m_count; i++)
		{
			array[i] = FindSet(i);
		}
		return array;
	}

	public int[][] ToSets()
	{
		int num = 0;
		int[] array = new int[m_count];
		int[] array2 = new int[m_count];
		int[] array3 = new int[m_count];
		for (int i = 0; i < m_count; i++)
		{
			array[FindSet(i)]++;
		}
		for (int j = 0; j < m_count; j++)
		{
			if (array[j] != 0)
			{
				array2[num] = j;
				array3[j] = num;
				num++;
			}
		}
		int[][] array4 = new int[num][];
		int[] array5 = new int[num];
		for (int k = 0; k < num; k++)
		{
			array4[k] = new int[array[array2[k]]];
		}
		for (int l = 0; l < m_count; l++)
		{
			int num2 = array3[FindSet(l)];
			ref int reference = ref array5[num2];
			array4[num2][reference] = l;
			reference++;
		}
		return array4;
	}
}
