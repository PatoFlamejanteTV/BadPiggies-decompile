using System;

[Serializable]
public class EventProperty
{
	public const string None = "None";

	public string m_eventTypeName = "None";

	public Type EventType
	{
		get
		{
			if (m_eventTypeName != "None")
			{
				return Type.GetType(m_eventTypeName);
			}
			return null;
		}
	}
}
