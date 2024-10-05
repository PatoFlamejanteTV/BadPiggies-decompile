using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class MethodCaller
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

		public void SetValue(object value)
		{
			if (value.GetType() == type)
			{
				if (type.IsSubclassOf(typeof(Enum)))
				{
					stringValue = value.ToString();
				}
				else if (type == typeof(int))
				{
					intValue = (int)value;
				}
				else if (type == typeof(bool))
				{
					boolValue = (bool)value;
				}
				else if (type == typeof(string))
				{
					stringValue = (string)value;
				}
			}
		}
	}

	[SerializeField]
	private GameObject m_targetObject;

	[SerializeField]
	private string m_targetComponent;

	[SerializeField]
	private string m_methodToInvoke;

	[SerializeField]
	private List<Parameter> m_parameters;

	private MethodInfo m_methodInfo;

	private Component m_component;

	private object[] m_callParameters;

	public GameObject TargetObject => m_targetObject;

	public string TargetComponent => m_targetComponent;

	public string MethodToInvoke => m_methodToInvoke;

	public void Reset()
	{
		m_targetObject = null;
		m_targetComponent = null;
		m_methodToInvoke = null;
		m_parameters = null;
		m_methodInfo = null;
	}

	public void ResetTargetObject(GameObject obj)
	{
		m_targetObject = obj;
		m_targetComponent = null;
		m_methodToInvoke = null;
		m_parameters = null;
		m_methodInfo = null;
	}

	public void ResetComponent(string component)
	{
		m_targetComponent = component;
		m_methodToInvoke = null;
		m_parameters = null;
		m_methodInfo = null;
	}

	public void SetMethod(Component targetComponent, string method)
	{
		m_targetObject = targetComponent.gameObject;
		m_targetComponent = targetComponent.GetType().Name;
		m_methodToInvoke = method;
		PrepareCall();
	}

	public void SetMethod(Component targetComponent, string method, object parameter)
	{
		SetMethod(targetComponent, method);
		SetParameter(parameter);
	}

	public void SetMethod(Component targetComponent, string method, object[] parameters)
	{
		SetMethod(targetComponent, method);
		SetParameters(parameters);
	}

	public void SetParameter(object parameter)
	{
		GetParameters()[0].SetValue(parameter);
		PrepareCall();
	}

	public void SetParameters(object[] parameters)
	{
		List<Parameter> parameters2 = GetParameters();
		for (int i = 0; i < parameters.Length; i++)
		{
			parameters2[i].SetValue(parameters[i]);
		}
		PrepareCall();
	}

	public T GetParameter<T>(int index)
	{
		List<Parameter> parameters = GetParameters();
		if (index >= parameters.Count)
		{
			throw new Exception("MethodCaller: invalid parameter index " + index + " for " + m_methodToInvoke);
		}
		Parameter parameter = parameters[index];
		if (parameter.type != typeof(T))
		{
			throw new Exception("MethodCaller: invalid parameter index " + index + " for " + m_methodToInvoke);
		}
		if (parameter.type.IsSubclassOf(typeof(Enum)))
		{
			return (T)Enum.Parse(parameter.type, parameter.stringValue);
		}
		if (parameter.type == typeof(int))
		{
			return (T)(object)parameter.intValue;
		}
		if (parameter.type == typeof(bool))
		{
			return (T)(object)parameter.boolValue;
		}
		if (parameter.type == typeof(string))
		{
			return (T)(object)parameter.stringValue;
		}
		throw new Exception("MethodCaller.GetParameter: type not implemented");
	}

	public List<Parameter> GetParametersForInspector()
	{
		return GetParameters();
	}

	private List<Parameter> GetParameters()
	{
		List<Parameter> list = new List<Parameter>();
		MethodInfo method = m_targetObject.GetComponent(m_targetComponent).GetType().GetMethod(m_methodToInvoke);
		if (method != null)
		{
			ParameterInfo[] parameters = method.GetParameters();
			foreach (ParameterInfo parameterInfo in parameters)
			{
				list.Add(new Parameter(parameterInfo.ParameterType, parameterInfo.ParameterType.FullName, parameterInfo.Name));
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

	public void PrepareCall()
	{
		if (!m_targetObject || !(m_targetComponent != string.Empty) || !(m_methodToInvoke != string.Empty))
		{
			return;
		}
		m_component = m_targetObject.GetComponent(m_targetComponent);
		if ((bool)m_component)
		{
			m_methodInfo = m_component.GetType().GetMethod(m_methodToInvoke);
			if (m_methodInfo != null)
			{
				m_callParameters = CreateParameterObjects();
			}
		}
	}

	public void Call()
	{
		if (m_methodInfo == null)
		{
			if (!m_targetObject)
			{
				return;
			}
			PrepareCall();
		}
		m_methodInfo.Invoke(m_component, m_callParameters);
	}
}
