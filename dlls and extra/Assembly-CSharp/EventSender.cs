using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class EventSender
{
	[Serializable]
	public class Parameter
	{
		public Type type;

		public string typeName;

		public string name;

		public string stringValue;

		public int intValue;

		public bool boolValue;

		public Parameter(Type type, string typeName, string name)
		{
			this.type = type;
			this.typeName = typeName;
			this.name = name;
			stringValue = string.Empty;
		}
	}

	[SerializeField]
	private EventProperty m_event = new EventProperty();

	[SerializeField]
	private List<Parameter> m_parameters;

	private MethodInfo m_genericSendMethod;

	private object[] m_sendParameters;

	public string EventName => m_event.m_eventTypeName;

	public bool HasEvent()
	{
		return m_event.EventType != null;
	}

	public void SetEvent(string eventName, List<Parameter> parameters)
	{
		m_event.m_eventTypeName = eventName;
		m_parameters = parameters;
	}

	public List<Parameter> GetParameters()
	{
		List<Parameter> list = new List<Parameter>();
		Type eventType = m_event.EventType;
		if (eventType != null)
		{
			ConstructorInfo[] constructors = eventType.GetConstructors();
			int num = 0;
			if (num < constructors.Length)
			{
				ParameterInfo[] parameters = constructors[num].GetParameters();
				foreach (ParameterInfo parameterInfo in parameters)
				{
					list.Add(new Parameter(parameterInfo.ParameterType, parameterInfo.ParameterType.FullName, parameterInfo.Name));
				}
			}
			if (m_parameters == null)
			{
				m_parameters = list;
			}
			else
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (j < m_parameters.Count)
					{
						m_parameters[j].typeName = list[j].typeName;
						m_parameters[j].type = list[j].type;
						m_parameters[j].name = list[j].name;
					}
					else
					{
						m_parameters.Add(list[j]);
					}
				}
				for (int k = list.Count; k < m_parameters.Count; k++)
				{
					m_parameters[k].type = null;
				}
			}
			return m_parameters;
		}
		return new List<Parameter>();
	}

	public object[] CreateParameterObjects()
	{
		List<Parameter> parameters = GetParameters();
		int num = 0;
		for (int i = 0; i < parameters.Count; i++)
		{
			if (parameters[i].type != null)
			{
				num++;
			}
		}
		object[] array = new object[num];
		for (int j = 0; j < num; j++)
		{
			Parameter parameter = parameters[j];
			if (parameter.type.IsSubclassOf(typeof(Enum)))
			{
				array[j] = Enum.Parse(parameter.type, parameter.stringValue);
			}
			else if (parameter.type == typeof(int))
			{
				array[j] = parameter.intValue;
			}
			else if (parameter.type == typeof(bool))
			{
				array[j] = parameter.boolValue;
			}
			else if (parameter.type == typeof(string))
			{
				array[j] = parameter.stringValue;
			}
		}
		return array;
	}

	public void PrepareSend()
	{
		Type eventType = m_event.EventType;
		if (eventType != null)
		{
			MethodInfo method = typeof(EventManager).GetMethod("Send");
			m_genericSendMethod = method.MakeGenericMethod(eventType);
			EventManager.Event @event = (EventManager.Event)Activator.CreateInstance(eventType, CreateParameterObjects());
			m_sendParameters = new object[1] { @event };
		}
	}

	public void Send()
	{
		if (m_genericSendMethod == null)
		{
			if (m_event.EventType == null)
			{
				return;
			}
			PrepareSend();
		}
		PrepareSend();
		m_genericSendMethod.Invoke(null, m_sendParameters);
	}
}
