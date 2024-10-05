using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartGeneratorManager : PartManager
{
	private class PartGenerationSystem
	{
		private bool m_resize;

		private DisjointSetFull m_disjointSet;

		private ComponentData[] m_components;

		private RuntimeContraption m_runtimeContraption;

		public bool IsEmpty => m_runtimeContraption.Parts.Count == 0;

		public RuntimeContraption RuntimeContraption => m_runtimeContraption;

		public PartGenerationSystem()
		{
			m_components = Array.Empty<ComponentData>();
			m_runtimeContraption = new RuntimeContraption();
			m_disjointSet = new DisjointSetFull(0);
		}

		public void InitializePart(BasePart template, GrapplingHook generator, int x, int y)
		{
			int enclosedPartIndex = -1;
			RuntimeContraption runtimeContraption = m_runtimeContraption;
			int key = generator.m_coordX + x + (generator.m_coordY + y << 16);
			int num = runtimeContraption.FindPartIndexAt(generator.m_coordX + x, generator.m_coordY + y);
			BasePart basePart;
			if (num != -1)
			{
				BasePart part = runtimeContraption.Parts[num].Part;
				if (part.CanBeEnclosed() && template.CanEncloseParts())
				{
					if (part.enclosedInto != null)
					{
						return;
					}
					basePart = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.GetCustomPart(template.m_partType, template.customPartIndex));
					runtimeContraption.PartIndexMap[key] = runtimeContraption.Parts.Count;
					basePart.enclosedPart = part;
					enclosedPartIndex = num;
				}
				else
				{
					if (!template.CanBeEnclosed() || !part.CanEncloseParts() || part.enclosedPart != null)
					{
						return;
					}
					basePart = (part.enclosedPart = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.GetCustomPart(template.m_partType, template.customPartIndex)));
					PartData value = runtimeContraption.Parts[num];
					value.EnclosedPartIndex = runtimeContraption.Parts.Count;
					runtimeContraption.Parts[num] = value;
				}
			}
			else
			{
				basePart = UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.GetCustomPart(template.m_partType, template.customPartIndex));
				runtimeContraption.PartIndexMap[key] = runtimeContraption.Parts.Count;
			}
			basePart.gameObject.SetActive(value: true);
			basePart.transform.position = template.transform.position;
			basePart.transform.rotation = template.transform.rotation;
			basePart.transform.parent = template.transform.parent;
			basePart.m_coordX = generator.m_coordX + x;
			basePart.m_coordY = generator.m_coordY + y;
			basePart.PrePlaced();
			basePart.SetRotation(template.m_gridRotation);
			if (template.m_flipped)
			{
				basePart.SetFlipped(template.m_flipped);
			}
			basePart.GenerationLevel = template.GenerationLevel + 1;
			basePart.contraption = template.contraption;
			basePart.gameObject.SetActive(value: false);
			runtimeContraption.Parts.Add(new PartData(basePart, template, generator, Mathf.Sqrt(x * x + y * y), Time.time, enclosedPartIndex, generator.GetRendererArray()));
			m_resize = true;
		}

		public void UpdateParts()
		{
			RuntimeContraption runtimeContraption = m_runtimeContraption;
			List<PartData> parts = runtimeContraption.Parts;
			int count = parts.Count;
			if (m_resize)
			{
				int count2 = m_disjointSet.Count;
				int capacity = m_disjointSet.Capacity;
				if (capacity < count)
				{
					m_disjointSet.Capacity = ((capacity * 2 > count) ? (capacity * 2) : count);
				}
				m_disjointSet.MakeSet(m_disjointSet.Count, count - 1);
				m_disjointSet.Count = count;
				Connect(count2, count - 1);
				int num = 0;
				int[] array = new int[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = -1;
				}
				for (int j = 0; j < count; j++)
				{
					int num2 = m_disjointSet.FindSet(j);
					int num3 = array[num2];
					PartData value = parts[j];
					if (num3 == -1)
					{
						num3 = num++;
					}
					array[num2] = num3;
					value.ComponentIndex = num3;
					parts[j] = value;
				}
				m_components = new ComponentData[num];
				for (int k = 0; k < count; k++)
				{
					PartData partData = parts[k];
					ref ComponentData reference = ref m_components[partData.ComponentIndex];
					if (partData.Time > reference.Time)
					{
						reference.Time = partData.Time;
					}
					int size = m_disjointSet.GetSize(m_disjointSet.FindSet(k));
					reference.GenerateTime = Mathf.Sqrt((size - 1) * 6 + 1) * 0.2f;
				}
				m_resize = false;
			}
			Contraption instance = Contraption.Instance;
			Dictionary<int, BasePart> dictionary = new Dictionary<int, BasePart>(m_runtimeContraption.PartIndexMap.Count);
			foreach (KeyValuePair<int, int> item in m_runtimeContraption.PartIndexMap)
			{
				PartData partData2 = m_runtimeContraption.Parts[item.Value];
				ComponentData componentData = m_components[partData2.ComponentIndex];
				if (componentData.Time + componentData.GenerateTime - Time.time < 0f)
				{
					dictionary.Add(item.Key, partData2.Part);
				}
			}
			instance.RuntimePartMap = dictionary;
			foreach (PartData part3 in m_runtimeContraption.Parts)
			{
				ComponentData componentData2 = m_components[part3.ComponentIndex];
				float num4 = componentData2.Time + componentData2.GenerateTime - Time.time;
				if (num4 < 0f)
				{
					if (part3.Original == null)
					{
						UnityEngine.Object.Destroy(part3.Part);
						continue;
					}
					part3.Generator.OnPartGenerated();
					BasePart part = part3.Part;
					Vector3 position = part3.Generator.rigidbody.position + part3.Generator.transform.right * part3.Distance;
					position.z = part.transform.position.z;
					part.gameObject.SetActive(value: true);
					part.enabled = true;
					part.gameObject.tag = "Contraption";
					for (int l = 0; l < part.transform.childCount; l++)
					{
						part.transform.GetChild(l).gameObject.tag = "Contraption";
					}
					part.ChangeVisualConnections();
					part.EnsureRigidbody();
					part.transform.position = position;
					part.transform.rotation = part3.Original.transform.rotation;
					part.rigidbody.position = position;
					part.rigidbody.velocity = part3.Generator.rigidbody.velocity;
					instance.AddRuntimePart(part);
					continue;
				}
				(Renderer, Color)[] renderers = part3.Renderers;
				for (int m = 0; m < renderers.Length; m++)
				{
					(Renderer, Color) tuple = renderers[m];
					(MeshRenderer, Color) tuple2 = ((MeshRenderer)tuple.Item1, tuple.Item2);
					if (tuple2.Item1 != null)
					{
						Color color = Color.Lerp(new Color(0.5f, 0.75f, 1f, 0.1f), tuple2.Item2, 1f / (num4 + 1f));
						tuple2.Item1.material.color = color;
					}
				}
			}
			List<BasePart> list = new List<BasePart>();
			CreateJoints();
			foreach (PartData item2 in parts)
			{
				ComponentData componentData3 = m_components[item2.ComponentIndex];
				if (componentData3.Time + componentData3.GenerateTime - Time.time < 0f)
				{
					BasePart part2 = item2.Part;
					part2.ConnectedComponent = item2.ComponentIndex;
					list.Add(part2);
					part2.Initialize();
					part2.PostInitialize();
				}
			}
			instance.RuntimePartMap = null;
			if (INSettings.GetBool(INFeature.FrameJoint))
			{
				FrameJointManager.Instance.AddJoints(list);
			}
			foreach (BasePart item3 in list)
			{
				item3.ConnectedComponent = -1;
			}
			int count3 = parts.Count;
			int[] array2 = new int[count3];
			int num5 = 0;
			int num6 = 0;
			while (num5 < parts.Count && num6 < count3)
			{
				PartData partData3 = parts[num5];
				ComponentData componentData4 = m_components[partData3.ComponentIndex];
				if (componentData4.Time + componentData4.GenerateTime - Time.time < 0f)
				{
					array2[num6] = -1;
					parts.RemoveAt(num5);
				}
				else
				{
					array2[num6] = num5;
					num5++;
				}
				num6++;
			}
			bool flag = false;
			foreach (KeyValuePair<int, int> item4 in runtimeContraption.PartIndexMap.ToList())
			{
				int num7 = array2[item4.Value];
				if (num7 == -1)
				{
					flag = true;
					runtimeContraption.PartIndexMap.Remove(item4.Key);
				}
				else
				{
					runtimeContraption.PartIndexMap[item4.Key] = num7;
				}
			}
			for (int n = 0; n < parts.Count; n++)
			{
				PartData value2 = parts[n];
				if (value2.EnclosedPartIndex != -1)
				{
					value2.EnclosedPartIndex = array2[value2.EnclosedPartIndex];
					parts[n] = value2;
				}
			}
			if (flag)
			{
				m_disjointSet.Clear();
				m_disjointSet.MakeSet(0, parts.Count - 1);
				m_disjointSet.Count = parts.Count;
				Connect(0, parts.Count - 1);
				Contraption.Instance.UpdateConnectedComponents();
			}
		}

		private IEnumerable<PartData> GetGeneratedParts()
		{
			foreach (PartData part in m_runtimeContraption.Parts)
			{
				ComponentData componentData = m_components[part.ComponentIndex];
				if (Time.time > componentData.Time + componentData.GenerateTime)
				{
					yield return part;
				}
			}
		}

		private void Connect(int start, int end)
		{
			Contraption instance = Contraption.Instance;
			RuntimeContraption runtimeContraption = m_runtimeContraption;
			for (int i = start; i <= end; i++)
			{
				PartData partData = runtimeContraption.Parts[i];
				BasePart part = partData.Part;
				if (partData.EnclosedPartIndex != -1)
				{
					m_disjointSet.Union(i, partData.EnclosedPartIndex);
				}
				int coordX = part.m_coordX;
				int coordY = part.m_coordY;
				if (part is Balloon)
				{
					int j = 1;
					int x = 0;
					int y = 1;
					int @int = INSettings.GetInt(INFeature.BalloonConnectionDistance);
					BasePart basePart = null;
					int num = -1;
					if (INSettings.GetBool(INFeature.RotatableBalloon))
					{
						BasePart.GetDirection((BasePart.GridRotation)((int)(part.m_gridRotation + 1) % 4), out x, out y);
					}
					for (; j < @int + 1; j++)
					{
						if (!(basePart == null))
						{
							break;
						}
						num = runtimeContraption.FindPartIndexAt(part.m_coordX - j * x, part.m_coordY - j * y);
						basePart = runtimeContraption.GetPart(num);
						if (basePart != null && !basePart.IsPartOfChassis() && basePart.m_partType != BasePart.PartType.Pig && basePart.m_partType != BasePart.PartType.Kicker)
						{
							basePart = null;
						}
					}
					if (num != -1)
					{
						m_disjointSet.Union(i, num);
					}
					continue;
				}
				if (part is Rope)
				{
					int num2;
					int num3;
					if (part.GetCustomJointConnectionDirection() == BasePart.JointConnectionDirection.LeftAndRight)
					{
						num2 = runtimeContraption.FindPartIndexAt(coordX - 1, coordY);
						num3 = runtimeContraption.FindPartIndexAt(coordX + 1, coordY);
					}
					else
					{
						num2 = runtimeContraption.FindPartIndexAt(coordX, coordY + 1);
						num3 = runtimeContraption.FindPartIndexAt(coordX, coordY - 1);
					}
					BasePart basePart2 = runtimeContraption.GetPart(num2);
					BasePart basePart3 = runtimeContraption.GetPart(num3);
					if ((bool)basePart2 && basePart2 is Rope && basePart2.m_gridRotation != part.m_gridRotation)
					{
						basePart2 = null;
					}
					if ((bool)basePart2 && !(basePart2 is Rope) && !(basePart2 is Frame) && !(basePart2 is Kicker))
					{
						basePart2 = null;
					}
					if ((bool)basePart3 && basePart3 is Rope && basePart3.m_gridRotation != part.m_gridRotation)
					{
						basePart3 = null;
					}
					if ((bool)basePart3 && !(basePart3 is Rope) && !(basePart3 is Frame) && !(basePart3 is Kicker))
					{
						basePart3 = null;
					}
					if (basePart2 != null)
					{
						m_disjointSet.Union(i, num2);
					}
					if (basePart3 != null)
					{
						m_disjointSet.Union(i, num3);
					}
					continue;
				}
				if (part is HingePlate hingePlate)
				{
					(int, int)[] directions = HingePlate.Directions;
					for (int k = 0; k < directions.Length; k++)
					{
						(int, int) tuple = directions[k];
						int num4 = runtimeContraption.FindPartIndexAt(coordX + tuple.Item1, coordY + tuple.Item2);
						if (num4 != -1)
						{
							BasePart part2 = runtimeContraption.GetPart(num4);
							if (hingePlate.CanConnectTo(part2))
							{
								m_disjointSet.Union(i, num4);
							}
						}
					}
					continue;
				}
				int num5 = 1;
				int num6 = 0;
				for (int l = 0; l < 4; l++)
				{
					int num7 = runtimeContraption.FindPartIndexAt(coordX + num5, coordY + num6);
					if (num7 != -1)
					{
						BasePart part3 = runtimeContraption.Parts[num7].Part;
						if (instance.CanConnectTo(part, part3, (BasePart.Direction)l))
						{
							m_disjointSet.Union(i, num7);
						}
					}
					int num8 = num5;
					num5 = -num6;
					num6 = num8;
				}
			}
		}

		private void CreateJoints()
		{
			Contraption instance = Contraption.Instance;
			RuntimeContraption runtimeContraption = m_runtimeContraption;
			foreach (PartData generatedPart in GetGeneratedParts())
			{
				BasePart part = generatedPart.Part;
				int coordX = part.m_coordX;
				int coordY = part.m_coordY;
				BasePart.JointConnectionDirection customJointConnectionDirection = part.GetCustomJointConnectionDirection();
				if (part.m_jointConnectionType != 0)
				{
					BasePart basePart = runtimeContraption.FindPartAt(coordX + 1, coordY);
					BasePart basePart2 = runtimeContraption.FindPartAt(coordX, coordY - 1);
					if (instance.CanConnectTo(part, basePart, BasePart.Direction.Right))
					{
						BasePart.JointConnectionDirection customJointConnectionDirection2 = basePart.GetCustomJointConnectionDirection();
						if (customJointConnectionDirection == BasePart.JointConnectionDirection.Right || customJointConnectionDirection == BasePart.JointConnectionDirection.LeftAndRight)
						{
							instance.AddCustomConnectionBetweenParts(part, basePart);
						}
						else if (customJointConnectionDirection2 == BasePart.JointConnectionDirection.Left || customJointConnectionDirection2 == BasePart.JointConnectionDirection.LeftAndRight)
						{
							instance.AddCustomConnectionBetweenParts(basePart, part);
						}
						else
						{
							instance.AddFixedJoint(part, basePart);
						}
					}
					if (instance.CanConnectTo(part, basePart2, BasePart.Direction.Down))
					{
						BasePart.JointConnectionDirection customJointConnectionDirection3 = basePart2.GetCustomJointConnectionDirection();
						if (customJointConnectionDirection == BasePart.JointConnectionDirection.Down || customJointConnectionDirection == BasePart.JointConnectionDirection.UpAndDown)
						{
							instance.AddCustomConnectionBetweenParts(part, basePart2);
						}
						else if (customJointConnectionDirection3 == BasePart.JointConnectionDirection.Up || customJointConnectionDirection3 == BasePart.JointConnectionDirection.UpAndDown)
						{
							instance.AddCustomConnectionBetweenParts(basePart2, part);
						}
						else
						{
							instance.AddFixedJoint(part, basePart2);
						}
					}
					if (INSettings.GetBool(INFeature.AllDirectionsConnection))
					{
						BasePart basePart3 = runtimeContraption.FindPartAt(coordX - 1, coordY);
						BasePart basePart4 = runtimeContraption.FindPartAt(coordX, coordY + 1);
						if (instance.CanConnectTo(part, basePart3, BasePart.Direction.Left))
						{
							BasePart.JointConnectionDirection customJointConnectionDirection4 = basePart3.GetCustomJointConnectionDirection();
							if (customJointConnectionDirection == BasePart.JointConnectionDirection.Left || customJointConnectionDirection == BasePart.JointConnectionDirection.LeftAndRight)
							{
								instance.AddCustomConnectionBetweenParts(part, basePart3);
							}
							else if (customJointConnectionDirection4 == BasePart.JointConnectionDirection.Right || customJointConnectionDirection4 == BasePart.JointConnectionDirection.LeftAndRight)
							{
								instance.AddCustomConnectionBetweenParts(basePart3, part);
							}
							else
							{
								instance.AddFixedJoint(part, basePart3);
							}
						}
						if (instance.CanConnectTo(part, basePart4, BasePart.Direction.Up))
						{
							BasePart.JointConnectionDirection customJointConnectionDirection5 = basePart4.GetCustomJointConnectionDirection();
							if (customJointConnectionDirection == BasePart.JointConnectionDirection.Up || customJointConnectionDirection == BasePart.JointConnectionDirection.UpAndDown)
							{
								instance.AddCustomConnectionBetweenParts(part, basePart4);
							}
							else if (customJointConnectionDirection5 == BasePart.JointConnectionDirection.Down || customJointConnectionDirection5 == BasePart.JointConnectionDirection.UpAndDown)
							{
								instance.AddCustomConnectionBetweenParts(basePart4, part);
							}
							else
							{
								instance.AddFixedJoint(part, basePart4);
							}
						}
					}
					if (part.m_partType == BasePart.PartType.Rope && part is Rope)
					{
						Rope obj = (Rope)part;
						BasePart basePart5;
						BasePart basePart6;
						if (customJointConnectionDirection == BasePart.JointConnectionDirection.LeftAndRight)
						{
							basePart5 = runtimeContraption.FindPartAt(coordX - 1, coordY);
							basePart6 = runtimeContraption.FindPartAt(coordX + 1, coordY);
						}
						else
						{
							basePart5 = runtimeContraption.FindPartAt(coordX, coordY + 1);
							basePart6 = runtimeContraption.FindPartAt(coordX, coordY - 1);
						}
						if ((bool)basePart5 && basePart5 is Rope && basePart5.m_gridRotation != part.m_gridRotation)
						{
							basePart5 = null;
						}
						if ((bool)basePart5 && !(basePart5 is Rope) && !(basePart5 is Frame) && !(basePart5 is Kicker))
						{
							basePart5 = null;
						}
						if ((bool)basePart6 && basePart6 is Rope && basePart6.m_gridRotation != part.m_gridRotation)
						{
							basePart6 = null;
						}
						if ((bool)basePart6 && !(basePart6 is Rope) && !(basePart6 is Frame) && !(basePart6 is Kicker))
						{
							basePart6 = null;
						}
						obj.Create(basePart5, basePart6);
					}
				}
				if (part.m_partType == BasePart.PartType.Spring && part.m_enclosedInto == null)
				{
					BasePart.Direction direction = BasePart.ConvertDirection(part.GetCustomJointConnectionDirection());
					BasePart partAt = runtimeContraption.GetPartAt(part, direction);
					if (!partAt || !instance.CanConnectTo(part, partAt, direction))
					{
						(part as Spring).CreateSpringBody(direction);
					}
				}
				if (part.m_partType == BasePart.PartType.Pig && (bool)WPFMonoBehaviour.levelManager && WPFMonoBehaviour.levelManager.m_disablePigCollisions && part.m_enclosedInto != null)
				{
					part.gameObject.layer = LayerMask.NameToLayer("NonCollidingPart");
				}
				if (part.m_partType != BasePart.PartType.KingPig && part.m_partType != BasePart.PartType.GoldenPig)
				{
					continue;
				}
				for (int i = 0; i <= 1; i++)
				{
					for (int j = -2; j <= 2; j += 4)
					{
						BasePart basePart7 = runtimeContraption.FindPartAt(coordX + j, coordY + i);
						if (basePart7 != null && (basePart7.m_partType == BasePart.PartType.Wings || basePart7.m_partType == BasePart.PartType.MetalWing) && basePart7.collider != null)
						{
							Physics.IgnoreCollision(part.collider, basePart7.collider);
						}
					}
					for (int k = -1; k <= 1; k++)
					{
						if (k == 0 && i == 0)
						{
							continue;
						}
						BasePart basePart8 = runtimeContraption.FindPartAt(coordX + k, coordY + i);
						if (!(basePart8 != null))
						{
							continue;
						}
						if (basePart8.m_partType == BasePart.PartType.WoodenFrame || basePart8.m_partType == BasePart.PartType.MetalFrame)
						{
							if (i == 0)
							{
								instance.AddFixedJoint(part, basePart8);
							}
							else
							{
								Physics.IgnoreCollision(part.collider, basePart8.collider);
							}
						}
						else if (basePart8.m_partType == BasePart.PartType.Spring && basePart8.collider != null)
						{
							Physics.IgnoreCollision(part.collider, basePart8.collider);
						}
					}
				}
			}
		}
	}

	private struct DiscretePartData
	{
		public BasePart Part;

		public BasePart Generator;

		public int EnclosedPart;

		public float Distance;

		public float Time;

		public DiscretePartData(BasePart part, BasePart generator, float distance, float time, int enclosedPart)
		{
			Part = part;
			Generator = generator;
			Distance = distance;
			Time = time;
			EnclosedPart = enclosedPart;
		}
	}

	private struct PartInfo
	{
		public BasePart.PartType PartType;

		public int CustomPartIndex;

		public int CoordX;

		public int CoordY;

		public BasePart.GridRotation Rotation;

		public bool Flipped;
	}

	private struct PartData
	{
		public BasePart Part;

		public BasePart Original;

		public GrapplingHook Generator;

		public int EnclosedPartIndex;

		public float Distance;

		public float Time;

		public int ComponentIndex;

		public (Renderer, Color)[] Renderers;

		public PartData(BasePart part, BasePart original, GrapplingHook generator, float distance, float time, int enclosedPartIndex, Renderer[] renderers)
		{
			Part = part;
			Original = original;
			Generator = generator;
			Distance = distance;
			Time = time;
			ComponentIndex = 0;
			EnclosedPartIndex = enclosedPartIndex;
			Renderers = new(Renderer, Color)[renderers.Length];
			for (int i = 0; i < renderers.Length; i++)
			{
				Renderers[i] = (renderers[i], renderers[i].material.color);
			}
		}
	}

	private struct ComponentData
	{
		public float GenerateTime;

		public float Time;
	}

	private class RuntimeContraption
	{
		private List<PartData> m_parts;

		private Dictionary<int, int> m_partIndexMap;

		private Dictionary<int, BasePart> m_partMap;

		public List<PartData> Parts => m_parts;

		public Dictionary<int, int> PartIndexMap => m_partIndexMap;

		public RuntimeContraption()
		{
			m_parts = new List<PartData>();
			m_partIndexMap = new Dictionary<int, int>();
		}

		public BasePart GetPart(int partIndex)
		{
			if (partIndex == -1)
			{
				return null;
			}
			return m_parts[partIndex].Part;
		}

		public BasePart FindPartAt(int x, int y)
		{
			int key = x + (y << 16);
			if (m_partIndexMap.TryGetValue(key, out var value))
			{
				return m_parts[value].Part;
			}
			return null;
		}

		public int FindPartIndexAt(int x, int y)
		{
			int key = x + (y << 16);
			if (m_partIndexMap.TryGetValue(key, out var value))
			{
				return value;
			}
			return -1;
		}

		public BasePart GetPartAt(BasePart part, BasePart.Direction direction)
		{
			int coordX = part.m_coordX;
			int coordY = part.m_coordY;
			return direction switch
			{
				BasePart.Direction.Right => FindPartAt(coordX + 1, coordY), 
				BasePart.Direction.Up => FindPartAt(coordX, coordY + 1), 
				BasePart.Direction.Left => FindPartAt(coordX - 1, coordY), 
				BasePart.Direction.Down => FindPartAt(coordX, coordY - 1), 
				_ => null, 
			};
		}

		public bool CanConnectTo(BasePart part, BasePart.Direction direction)
		{
			int coordX = part.m_coordX;
			int coordY = part.m_coordY;
			switch (direction)
			{
			case BasePart.Direction.Right:
			{
				BasePart part5 = FindPartAt(coordX + 1, coordY);
				return Contraption.Instance.CanConnectTo(part, part5, direction);
			}
			case BasePart.Direction.Up:
			{
				BasePart part4 = FindPartAt(coordX, coordY + 1);
				return Contraption.Instance.CanConnectTo(part, part4, direction);
			}
			case BasePart.Direction.Left:
			{
				BasePart part3 = FindPartAt(coordX - 1, coordY);
				return Contraption.Instance.CanConnectTo(part, part3, direction);
			}
			case BasePart.Direction.Down:
			{
				BasePart part2 = FindPartAt(coordX, coordY - 1);
				return Contraption.Instance.CanConnectTo(part, part2, direction);
			}
			default:
				return false;
			}
		}
	}

	private List<(BasePart, BasePart, int, int)> m_cachedParts;

	private List<PartGenerationSystem> m_systems;

	private PartGenerationSystem[] m_systemMap;

	public static PartGeneratorManager Instance { get; private set; }

	protected override void Initialize()
	{
		base.Initialize();
		m_status = StatusCode.Running;
		Instance = this;
	}

	public override void Start()
	{
		m_cachedParts = new List<(BasePart, BasePart, int, int)>();
		m_systems = new List<PartGenerationSystem>();
		m_systemMap = Array.Empty<PartGenerationSystem>();
	}

	public override void FixedUpdate()
	{
		int generalConnectedComponentCount = Contraption.Instance.GeneralConnectedComponentCount;
		if (m_systemMap.Length >= generalConnectedComponentCount)
		{
			Array.Clear(m_systemMap, 0, m_systemMap.Length);
		}
		else
		{
			m_systemMap = new PartGenerationSystem[generalConnectedComponentCount];
		}
		foreach (PartGenerationSystem system in m_systems)
		{
			int count = system.RuntimeContraption.Parts.Count;
			foreach (PartData part in system.RuntimeContraption.Parts)
			{
				int generalConnectedComponent = part.Generator.GeneralConnectedComponent;
				if (m_systemMap[generalConnectedComponent] == null || count > m_systemMap[generalConnectedComponent].RuntimeContraption.Parts.Count)
				{
					m_systemMap[generalConnectedComponent] = system;
				}
			}
		}
		foreach (var cachedPart in m_cachedParts)
		{
			(BasePart, GrapplingHook, int, int) tuple = (cachedPart.Item1, (GrapplingHook)cachedPart.Item2, cachedPart.Item3, cachedPart.Item4);
			BasePart item = tuple.Item1;
			int generalConnectedComponent2 = item.GeneralConnectedComponent;
			PartGenerationSystem partGenerationSystem = m_systemMap[generalConnectedComponent2];
			if (partGenerationSystem == null)
			{
				partGenerationSystem = new PartGenerationSystem();
				m_systems.Add(partGenerationSystem);
				m_systemMap[generalConnectedComponent2] = partGenerationSystem;
			}
			partGenerationSystem.InitializePart(item, tuple.Item2, tuple.Item3, tuple.Item4);
		}
		m_cachedParts.Clear();
		foreach (PartGenerationSystem system2 in m_systems)
		{
			system2.UpdateParts();
		}
		m_systems.RemoveAll((PartGenerationSystem system) => system.IsEmpty);
	}

	public override void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public void GeneratePart(BasePart template, BasePart generator, int x, int y, bool discrete)
	{
		if (!(WPFMonoBehaviour.levelManager == null))
		{
			m_cachedParts.Add((template, generator, x, y));
		}
	}
}
