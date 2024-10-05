namespace Pathfinding.Serialization.JsonFx;

public class JsonDeserializationException : JsonSerializationException
{
	private int index = -1;

	public JsonDeserializationException(string message, int index)
		: base(message)
	{
		this.index = index;
	}
}
