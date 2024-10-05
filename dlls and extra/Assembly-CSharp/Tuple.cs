public class Tuple<T1, T2>
{
	private T1 item1;

	private T2 item2;

	public T1 Item1
	{
		get
		{
			return item1;
		}
		set
		{
			item1 = value;
		}
	}

	public T2 Item2
	{
		get
		{
			return item2;
		}
		set
		{
			item2 = value;
		}
	}

	public Tuple()
	{
		item1 = default(T1);
		item2 = default(T2);
	}

	public Tuple(T1 item1, T2 item2)
	{
		this.item1 = item1;
		this.item2 = item2;
	}
}
