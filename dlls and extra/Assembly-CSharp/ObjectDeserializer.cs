using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public class ObjectDeserializer
{
	public class PropertyData
	{
		public string type;

		public string name;

		public string value;

		public int IntegerValue => int.Parse(value);

		public float FloatValue => float.Parse(value, CultureInfo.InvariantCulture);

		public string StringValue => value.Substring(1, value.Length - 2);

		public bool BoolValue => value == "True";

		public PropertyData(string type, string name)
		{
			this.type = type;
			this.name = name;
		}

		public PropertyData(string type, string name, string value)
		{
			this.type = type;
			this.name = name;
			this.value = value;
		}
	}

	public class ObjectReader
	{
		private StreamReader m_reader;

		private int m_indentation;

		private bool m_indentationRead;

		private List<UnityEngine.Object> m_references;

		private readonly char[] Delimiters = new char[1] { ' ' };

		public ObjectReader(StreamReader reader, List<UnityEngine.Object> references)
		{
			m_reader = reader;
			m_references = references;
		}

		public UnityEngine.Object GetReferencedObject(int index)
		{
			if (m_references != null)
			{
				return m_references[index];
			}
			return null;
		}

		private string ReadLine()
		{
			m_indentationRead = false;
			return m_reader.ReadLine();
		}

		public PropertyData ReadProperty()
		{
			string[] array = ReadLine().Split(' ');
			if (array.Length == 2)
			{
				return new PropertyData(array[0], array[1]);
			}
			if (array.Length == 4 && array[2] == "=")
			{
				return new PropertyData(array[0], array[1], array[3]);
			}
			if (array.Length > 4)
			{
				string text = string.Empty;
				for (int i = 1; i < array.Length && array[i] != "="; i++)
				{
					if (i > 1)
					{
						text += " ";
					}
					text += array[i];
				}
				return new PropertyData(array[0], text, array[^1]);
			}
			return null;
		}

		public PropertyData ReadTypeAndName()
		{
			string[] array = ReadLine().Split(Delimiters, 2);
			if (array.Length == 2)
			{
				return new PropertyData(array[0], array[1]);
			}
			return null;
		}

		public int GetIndentation()
		{
			if (m_indentationRead)
			{
				return m_indentation;
			}
			int num = 0;
			while (m_reader.Peek() == 9)
			{
				num++;
				m_reader.Read();
			}
			m_indentation = num;
			m_indentationRead = true;
			return num;
		}
	}

	private const string GameObjectType = "GameObject";

	private const string ComponentType = "Component";

	private const string IntegerType = "Integer";

	private const string FloatType = "Float";

	private const string StringType = "String";

	private const string BooleanType = "Boolean";

	private const string EnumType = "Enum";

	private const string Vector2Type = "Vector2";

	private const string Vector3Type = "Vector3";

	private const string QuaternionType = "Quaternion";

	private const string AnimationCurveType = "AnimationCurve";

	private const string ColorType = "Color";

	private const string RectType = "Rect";

	private const string BoundsType = "Bounds";

	private const string ObjectReferenceType = "ObjectReference";

	private const string GenericType = "Generic";

	private const string ArrayType = "Array";

	private const string ArrayElementType = "Element";

	private const string KeyframeType = "Keyframe";

	public static void ReadFile(GameObject obj, ObjectReader reader)
	{
		PropertyData propertyData = reader.ReadTypeAndName();
		if (propertyData.type == "GameObject" && propertyData.name == obj.name)
		{
			ReadObject(obj, 1, reader);
		}
	}

	private static void ReadObject(GameObject obj, int depth, ObjectReader reader)
	{
		while (reader.GetIndentation() == depth)
		{
			PropertyData propertyData = reader.ReadTypeAndName();
			if (propertyData.type == "Component")
			{
				string text = ((!propertyData.name.Contains(".")) ? propertyData.name : propertyData.name.Substring(propertyData.name.LastIndexOf(".") + 1));
				Component component = obj.GetComponent(text);
				if (component == null)
				{
					Type componentTypeByName = ComponentHelper.GetComponentTypeByName(text);
					component = ((componentTypeByName == null) ? null : obj.AddComponent(componentTypeByName));
				}
				if (component != null)
				{
					if (component is ParticleSystem)
					{
						ReadParticleSystem((ParticleSystem)component, depth + 1, reader);
					}
					else
					{
						ReadComponent(component, depth + 1, reader);
					}
				}
			}
			else if (propertyData.type == "GameObject")
			{
				GameObject gameObject = obj.transform.Find(propertyData.name).gameObject;
				if ((bool)gameObject)
				{
					ReadObject(gameObject, depth + 1, reader);
				}
			}
		}
	}

	private static void ReadComponent(object component, int depth, ObjectReader reader)
	{
		while (reader.GetIndentation() == depth)
		{
			PropertyData propertyData = reader.ReadProperty();
			if (propertyData.type == "Integer")
			{
				SetProperty(component, propertyData.name, propertyData.IntegerValue);
			}
			else if (propertyData.type == "Float")
			{
				SetProperty(component, propertyData.name, propertyData.FloatValue);
			}
			else if (propertyData.type == "String")
			{
				SetProperty(component, propertyData.name, propertyData.StringValue);
				reader.ReadProperty();
			}
			else if (propertyData.type == "Boolean")
			{
				SetProperty(component, propertyData.name, propertyData.BoolValue);
			}
			else if (propertyData.type == "Enum")
			{
				SetProperty(component, propertyData.name, propertyData.IntegerValue);
			}
			else if (propertyData.type == "Bounds")
			{
				SetProperty(component, propertyData.name, ReadBounds(depth + 1, reader));
			}
			else if (propertyData.type == "ObjectReference")
			{
				UnityEngine.Object referencedObject = reader.GetReferencedObject(propertyData.IntegerValue);
				if ((bool)referencedObject)
				{
					SetProperty(component, propertyData.name, referencedObject);
				}
				else
				{
					SetProperty(component, propertyData.name, null);
				}
				if (reader.GetIndentation() == depth + 1)
				{
					reader.ReadProperty();
				}
				if (reader.GetIndentation() == depth + 1)
				{
					reader.ReadProperty();
				}
			}
			else if (propertyData.type == "Array")
			{
				ReadArray(component, propertyData.name, depth + 1, reader);
			}
			else if (propertyData.type == "AnimationCurve")
			{
				ReadAnimationCurve(component, propertyData.name, depth + 1, reader);
			}
			else
			{
				if (!(propertyData.type == "Generic") && !(propertyData.type == "Color") && !(propertyData.type == "Vector2") && !(propertyData.type == "Vector3") && !(propertyData.type == "Rect") && !(propertyData.type == "16") && !(propertyData.type == "Quaternion"))
				{
					continue;
				}
				FieldInfo field = component.GetType().GetField(propertyData.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field != null)
				{
					object value = field.GetValue(component);
					ReadGeneric(value, depth + 1, reader);
					if (field.FieldType.IsValueType)
					{
						field.SetValue(component, value);
					}
				}
				else if (component is Camera && propertyData.name == "m_BackGroundColor")
				{
					object obj = default(Color);
					ReadGeneric(obj, depth + 1, reader);
					((Camera)component).backgroundColor = (Color)obj;
				}
				else if (component is BoxCollider && propertyData.name == "m_Center")
				{
					Vector3 center = ((BoxCollider)component).center;
					object obj2 = new Vector3(center.x, center.y, center.z);
					ReadGeneric(obj2, depth + 1, reader);
					((BoxCollider)component).center = (Vector3)obj2;
				}
				else if (component is BoxCollider && propertyData.name == "m_Size")
				{
					Vector3 size = ((BoxCollider)component).size;
					object obj3 = new Vector3(size.x, size.y, size.z);
					ReadGeneric(obj3, depth + 1, reader);
					((BoxCollider)component).size = (Vector3)obj3;
				}
				else if (component is Transform && propertyData.name == "m_LocalRotation")
				{
					Quaternion localRotation = ((Transform)component).localRotation;
					object obj4 = new Quaternion(localRotation.x, localRotation.y, localRotation.z, localRotation.w);
					ReadGeneric(obj4, depth + 1, reader);
					((Transform)component).localRotation = (Quaternion)obj4;
				}
				else if (component is Transform && propertyData.name == "m_LocalPosition")
				{
					Vector3 localPosition = ((Transform)component).localPosition;
					object obj5 = new Vector3(localPosition.x, localPosition.y, localPosition.z);
					ReadGeneric(obj5, depth + 1, reader);
					((Transform)component).localPosition = (Vector3)obj5;
				}
				else if (component is Transform && propertyData.name == "m_LocalScale")
				{
					Vector3 localScale = ((Transform)component).localScale;
					object obj6 = new Vector3(localScale.x, localScale.y, localScale.z);
					ReadGeneric(obj6, depth + 1, reader);
					((Transform)component).localScale = (Vector3)obj6;
				}
				else
				{
					_ = component is HingeJoint;
				}
			}
		}
	}

	private static void ReadParticleSystemModule(ParticleSystem particleSystem, string module, int depth, ObjectReader reader)
	{
		while (reader.GetIndentation() == depth)
		{
			PropertyData propertyData = reader.ReadProperty();
			switch (module)
			{
			case "InitialModule":
				if (propertyData.type == "Generic" && propertyData.name == "startLifetime")
				{
					if (reader.GetIndentation() == depth + 1)
					{
						propertyData = reader.ReadProperty();
						particleSystem.startLifetime = propertyData.FloatValue;
					}
				}
				else if (propertyData.type == "Generic" && propertyData.name == "startSpeed" && reader.GetIndentation() == depth + 1)
				{
					propertyData = reader.ReadProperty();
					particleSystem.startSpeed = propertyData.FloatValue;
				}
				break;
			case "EmissionModule":
				if (propertyData.type == "Generic" && propertyData.name == "rate" && reader.GetIndentation() == depth + 1)
				{
					propertyData = reader.ReadProperty();
				}
				break;
			case "ShapeModule":
				if (!(propertyData.type != "Boolean") && !(propertyData.name != "enabled"))
				{
					_ = propertyData.BoolValue;
				}
				break;
			}
		}
	}

	private static void ReadParticleSystem(ParticleSystem particleSystem, int depth, ObjectReader reader)
	{
		while (reader.GetIndentation() == depth)
		{
			PropertyData propertyData = reader.ReadProperty();
			if (propertyData.type == "Generic" && (propertyData.name == "InitialModule" || propertyData.name == "EmissionModule" || propertyData.name == "ShapeModule"))
			{
				ReadParticleSystemModule(particleSystem, propertyData.name, depth + 1, reader);
			}
		}
	}

	private static void SetProperty(object obj, string name, object value)
	{
		if (obj is Keyframe && name == "time")
		{
			name = "m_Time";
		}
		FieldInfo field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			field.SetValue(obj, value);
			return;
		}
		if (obj is Rigidbody && name == "m_IsKinematic")
		{
			name = "isKinematic";
		}
		PropertyInfo property = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property != null)
		{
			property.SetValue(obj, value, null);
			return;
		}
		if (obj is Behaviour && name == "m_Enabled")
		{
			((Behaviour)obj).enabled = (bool)value;
			return;
		}
		if (obj is Camera && name == "orthographic size")
		{
			((Camera)obj).orthographicSize = (float)value;
			return;
		}
		if (obj is Keyframe && name == "inSlope")
		{
			field = obj.GetType().GetField("m_InTangent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			field.SetValue(obj, value);
			return;
		}
		if (obj is Keyframe && name == "outSlope")
		{
			field = obj.GetType().GetField("m_OutTangent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			field.SetValue(obj, value);
			return;
		}
		switch (name)
		{
		case "m_EditorHideFlags":
			return;
		case "m_Name":
			return;
		case "m_PrefabParentObject":
			return;
		case "m_PrefabInternal":
			return;
		case "m_GameObject":
			return;
		case "m_EditorClassIdentifier":
			return;
		case "m_Script":
			return;
		case "m_Mesh":
			return;
		case "m_ConnectedBody":
			return;
		case "m_RootOrder":
			return;
		}
		_ = name == "m_Mass";
	}

	private static Bounds ReadBounds(int depth, ObjectReader reader)
	{
		Bounds result = default(Bounds);
		reader.GetIndentation();
		reader.ReadProperty();
		result.center = ReadVector3(depth + 1, reader);
		reader.GetIndentation();
		reader.ReadProperty();
		result.extents = ReadVector3(depth + 1, reader);
		return result;
	}

	private static Vector3 ReadVector3(int depth, ObjectReader reader)
	{
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			if (reader.GetIndentation() == depth)
			{
				PropertyData propertyData = reader.ReadProperty();
				if (propertyData.name == "x")
				{
					zero.x = propertyData.FloatValue;
				}
				else if (propertyData.name == "y")
				{
					zero.y = propertyData.FloatValue;
				}
				else if (propertyData.name == "z")
				{
					zero.z = propertyData.FloatValue;
				}
			}
		}
		return zero;
	}

	private static object GetDefaultValue(Type type)
	{
		object result = null;
		if (type == typeof(int))
		{
			result = 0;
		}
		else if (type == typeof(float))
		{
			result = 0f;
		}
		else if (type == typeof(string))
		{
			result = string.Empty;
		}
		else if (type == typeof(bool))
		{
			result = false;
		}
		else if (type == typeof(Bounds))
		{
			result = default(Bounds);
		}
		else if (type == typeof(Color))
		{
			result = default(Color);
		}
		else if (type == typeof(Vector2))
		{
			result = default(Vector2);
		}
		else if (type == typeof(Vector3))
		{
			result = default(Vector3);
		}
		else if (type == typeof(Quaternion))
		{
			result = default(Quaternion);
		}
		else if (type == typeof(Rect))
		{
			result = default(Rect);
		}
		else if (type == typeof(Keyframe))
		{
			result = default(Keyframe);
		}
		return result;
	}

	private static object ReadValueType(int depth, ObjectReader reader)
	{
		while (reader.GetIndentation() == depth)
		{
			PropertyData propertyData = reader.ReadProperty();
			if (propertyData.type == "Integer")
			{
				return propertyData.IntegerValue;
			}
			if (propertyData.type == "Float")
			{
				return propertyData.FloatValue;
			}
			if (propertyData.type == "String")
			{
				return propertyData.StringValue;
			}
			if (propertyData.type == "Boolean")
			{
				return propertyData.BoolValue;
			}
			if (propertyData.type == "Enum")
			{
				return propertyData.IntegerValue;
			}
			if (propertyData.type == "Bounds")
			{
				return ReadBounds(depth + 1, reader);
			}
			if (propertyData.type == "Color")
			{
				object obj = default(Color);
				ReadGeneric(obj, depth, reader);
				return obj;
			}
			if (propertyData.type == "Vector2")
			{
				object obj2 = default(Vector2);
				ReadGeneric(obj2, depth, reader);
				return obj2;
			}
			if (propertyData.type == "Vector3")
			{
				object obj3 = default(Vector3);
				ReadGeneric(obj3, depth, reader);
				return obj3;
			}
			if (propertyData.type == "Quaternion")
			{
				object obj4 = default(Quaternion);
				ReadGeneric(obj4, depth, reader);
				return obj4;
			}
			if (propertyData.type == "Rect")
			{
				object obj5 = default(Rect);
				ReadGeneric(obj5, depth, reader);
				return obj5;
			}
			if (propertyData.type == "Keyframe")
			{
				object obj6 = default(Keyframe);
				ReadGeneric(obj6, depth, reader);
				return obj6;
			}
		}
		return null;
	}

	private static void ReadGeneric(object obj, int depth, ObjectReader reader)
	{
		while (reader.GetIndentation() == depth)
		{
			ReadComponent(obj, depth, reader);
		}
	}

	private static Type FindListInterface(Type listType)
	{
		Type[] interfaces = listType.GetInterfaces();
		foreach (Type type in interfaces)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
			{
				return type;
			}
		}
		return null;
	}

	private static void ReadAnimationCurve(object component, string fieldName, int depth, ObjectReader reader)
	{
		FieldInfo field = component.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field == null)
		{
			return;
		}
		object obj = field.GetValue(component);
		if (obj == null)
		{
			obj = Activator.CreateInstance(field.FieldType);
			field.SetValue(component, obj);
		}
		while (reader.GetIndentation() == depth)
		{
			PropertyData propertyData = reader.ReadProperty();
			PropertyInfo property = obj.GetType().GetProperty("keys", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			object value = property.GetValue(obj, null);
			int length = 0;
			depth++;
			if (reader.GetIndentation() == depth)
			{
				propertyData = reader.ReadProperty();
				length = propertyData.IntegerValue;
			}
			IEnumerable obj2 = (IEnumerable)value;
			Type type = null;
			Array array = null;
			int i = 0;
			foreach (object item in obj2)
			{
				if (array == null)
				{
					type = item.GetType();
					array = Array.CreateInstance(type, length);
				}
				if (array.GetLength(0) <= i)
				{
					break;
				}
				array.SetValue(item, i);
				i++;
			}
			for (; array.GetLength(0) > i; i++)
			{
				if (array.GetValue(i) == null)
				{
					array.SetValue(Activator.CreateInstance(type), i);
				}
			}
			while (reader.GetIndentation() == depth)
			{
				propertyData = reader.ReadProperty();
				if (!(propertyData.type == "Element"))
				{
					continue;
				}
				i = int.Parse(propertyData.name);
				while (reader.GetIndentation() == depth + 1)
				{
					propertyData = reader.ReadProperty();
					if (propertyData.type == "Generic" || propertyData.type == "Keyframe")
					{
						object value2 = array.GetValue(i);
						ReadGeneric(value2, depth + 1, reader);
						array.SetValue(value2, i);
					}
				}
			}
			depth--;
			property.SetValue(obj, array, null);
			field.SetValue(component, obj);
		}
	}

	private static void ReadArray(object component, string fieldName, int depth, ObjectReader reader)
	{
		FieldInfo field = component.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field == null)
		{
			return;
		}
		object obj = field.GetValue(component);
		if (obj == null)
		{
			obj = Activator.CreateInstance(field.FieldType);
			field.SetValue(component, obj);
		}
		int num = 0;
		if (reader.GetIndentation() == depth)
		{
			num = reader.ReadProperty().IntegerValue;
		}
		IList list = (IList)obj;
		while (list.Count > num)
		{
			list.RemoveAt(list.Count - 1);
		}
		int num2 = 0;
		while (reader.GetIndentation() == depth)
		{
			PropertyData propertyData = reader.ReadProperty();
			if (propertyData.type == "Element")
			{
				int num3 = int.Parse(propertyData.name);
				Type type = list.GetType().GetGenericArguments()[0];
				if (type.IsValueType)
				{
					if (num3 > list.Count)
					{
						list.Add(GetDefaultValue(type));
						num3 -= num3 - list.Count;
					}
					if (num3 < list.Count)
					{
						list[num3] = ReadValueType(depth + 1, reader);
					}
					else
					{
						list.Insert(num3, ReadValueType(depth + 1, reader));
					}
				}
				else if (type.IsSubclassOf(typeof(UnityEngine.Object)))
				{
					reader.GetIndentation();
					propertyData = reader.ReadProperty();
					UnityEngine.Object referencedObject = reader.GetReferencedObject(propertyData.IntegerValue);
					if (referencedObject != null)
					{
						if (list.Count <= num3)
						{
							list.Insert(num3, referencedObject);
						}
						else
						{
							list[num3] = referencedObject;
						}
					}
				}
				else
				{
					reader.GetIndentation();
					propertyData = reader.ReadProperty();
					if (list.Count <= num3)
					{
						list.Insert(num3, Activator.CreateInstance(type));
					}
					ReadGeneric(list[num3], depth + 1, reader);
				}
			}
			num2++;
		}
	}

	private static void PrintObjectInfo(object obj)
	{
		StringBuilder stringBuilder = new StringBuilder();
		PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (properties.Length < 1)
		{
			stringBuilder.Append("Object " + obj.ToString() + " doesn't have any properties");
		}
		else
		{
			stringBuilder.Append("Object " + obj.ToString() + " has properties:");
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				stringBuilder.Append("\n\tObject property: " + propertyInfo.ToString());
			}
		}
		stringBuilder.Remove(0, stringBuilder.Length);
		FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (fields.Length < 1)
		{
			stringBuilder.Append("Object " + obj.ToString() + " doesn't have any fields");
		}
		else
		{
			stringBuilder.Append("Object " + obj.ToString() + " has fields:");
			FieldInfo[] array2 = fields;
			foreach (FieldInfo fieldInfo in array2)
			{
				stringBuilder.Append("\n\tObject field: " + fieldInfo.ToString());
			}
		}
		stringBuilder.Remove(0, stringBuilder.Length);
		MemberInfo[] members = obj.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (members.Length < 1)
		{
			stringBuilder.Append("Object " + obj.ToString() + " doesn't have any members");
		}
		else
		{
			stringBuilder.Append("Object " + obj.ToString() + " has members:");
			MemberInfo[] array3 = members;
			foreach (MemberInfo memberInfo in array3)
			{
				stringBuilder.Append("\n\tObject member: " + memberInfo.ToString());
			}
		}
		stringBuilder.Remove(0, stringBuilder.Length);
		MethodInfo[] methods = obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (methods.Length < 1)
		{
			stringBuilder.Append("Object " + obj.ToString() + " doesn't have any methods");
			return;
		}
		stringBuilder.Append("Object " + obj.ToString() + " has methods");
		MethodInfo[] array4 = methods;
		foreach (MethodInfo methodInfo in array4)
		{
			stringBuilder.Append("\n\tObject method: " + methodInfo.ToString());
		}
	}
}
