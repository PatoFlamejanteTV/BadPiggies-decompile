public class PartManager : INBehaviour
{
	public static T Create<T>() where T : PartManager, new()
	{
		T val = new T();
		val.Initialize();
		return val;
	}

	protected virtual void Initialize()
	{
		INContraption.Instance.AddBehaviour(this);
	}
}
