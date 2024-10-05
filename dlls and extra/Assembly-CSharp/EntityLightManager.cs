using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityLightManager : PartManager
{
	public struct RigidbodyData
	{
		public Rigidbody Rigidbody;

		public Vector2 Position;

		public Vector2 Direction;

		public float DeltaTime;

		public bool IsCollided;

		public RigidbodyData(Rigidbody rigidbody, Vector2 position, float deltaTime, bool isCollided)
		{
			Quaternion rotation = rigidbody.rotation;
			Rigidbody = rigidbody;
			Position = position;
			Direction.x = 1f - (rotation.y * rotation.y * 2f + rotation.z * rotation.z * 2f);
			Direction.y = rotation.x * rotation.y * 2f + rotation.w * rotation.z * 2f;
			DeltaTime = deltaTime;
			IsCollided = isCollided;
		}
	}

	private readonly struct ComparisonData : IComparable<ComparisonData>
	{
		public readonly int LightIndex;

		public readonly float Time;

		public ComparisonData(int lightIndex, float time)
		{
			LightIndex = lightIndex;
			Time = time;
		}

		public int CompareTo(ComparisonData value)
		{
			if (Time < value.Time)
			{
				return -1;
			}
			if (Time > value.Time)
			{
				return 1;
			}
			return 0;
		}
	}

	public struct CCDData
	{
		public Rigidbody Rigidbody;

		public Vector2 PrePosition;

		public Vector2 PreDirection;

		public Vector2 SucPosition;

		public Vector2 SucDirection;

		public INBounds Bounds;

		public float DeltaTime;

		public bool UsePathPrediction;

		public CCDData(RigidbodyData data)
		{
			Rigidbody rigidbody = data.Rigidbody;
			Vector3 position = rigidbody.position;
			Quaternion rotation = rigidbody.rotation;
			Rigidbody = rigidbody;
			PrePosition = data.Position;
			PreDirection = data.Direction;
			SucPosition.x = position.x;
			SucPosition.y = position.y;
			SucDirection.x = 1f - (rotation.y * rotation.y * 2f + rotation.z * rotation.z * 2f);
			SucDirection.y = rotation.x * rotation.y * 2f + rotation.w * rotation.z * 2f;
			Bounds = INContraption.GetBounds(rigidbody);
			DeltaTime = Time.fixedDeltaTime;
			UsePathPrediction = false;
		}

		public void Set(RigidbodyData data)
		{
			Rigidbody rigidbody = data.Rigidbody;
			Vector3 position = rigidbody.position;
			Quaternion rotation = rigidbody.rotation;
			INBounds bounds = INContraption.GetBounds(rigidbody);
			Rigidbody = rigidbody;
			PreDirection = data.Direction;
			if (data.IsCollided)
			{
				PrePosition = data.Position;
			}
			else
			{
				PrePosition.x = data.Position.x + PreDirection.x * bounds.X - PreDirection.y * bounds.Y;
				PrePosition.y = data.Position.y + PreDirection.x * bounds.Y + PreDirection.y * bounds.X;
			}
			SucDirection.x = 1f - (rotation.y * rotation.y * 2f + rotation.z * rotation.z * 2f);
			SucDirection.y = rotation.x * rotation.y * 2f + rotation.w * rotation.z * 2f;
			SucPosition.x = position.x + SucDirection.x * bounds.X - SucDirection.y * bounds.Y;
			SucPosition.y = position.y + SucDirection.x * bounds.Y + SucDirection.y * bounds.X;
			Bounds = bounds;
			DeltaTime = data.DeltaTime;
			UsePathPrediction = false;
		}
	}

	public struct ChangeData
	{
		public EntityLight Light;

		public Rigidbody Rigidbody;

		public Vector2 Position;

		public Vector2 PositionChange;

		public Vector2 VelocityChange;

		public float Torque;

		public float Electricity;

		public ChangeData(EntityLight light, Rigidbody rigidbody, Vector2 position, Vector2 positionChange, Vector2 velocityChange, float torque, float electricity)
		{
			Light = light;
			Rigidbody = rigidbody;
			Position = position;
			VelocityChange = velocityChange;
			PositionChange = positionChange;
			Torque = torque;
			Electricity = electricity;
		}
	}

	private static EntityLightManager s_instance;

	public bool m_consume;

	public float[] m_electricities;

	public float[] m_capacities;

	private int[] m_lightCounts;

	private int m_rigidbodyCount;

	private int m_componentCount;

	private int m_alienComponentCount;

	private float m_maxDetectionDistance;

	private RigidbodyData[] m_rigidbodyData;

	private List<EntityLight> m_lights;

	public List<ChangeData>[] m_changeData;

	public static EntityLightManager Instance => s_instance;

	protected override void Initialize()
	{
		base.Initialize();
		m_status = StatusCode.Running;
		s_instance = this;
	}

	public override void Start()
	{
		m_consume = !Contraption.Instance.HasTurboCharge;
		m_rigidbodyCount = 0;
		m_rigidbodyData = Array.Empty<RigidbodyData>();
		m_lights = new List<EntityLight>();
		m_changeData = Array.Empty<List<ChangeData>>();
		m_maxDetectionDistance = 160f;
	}

	public override void FixedUpdate()
	{
		m_lights.Clear();
		foreach (BasePart part in Contraption.Instance.Parts)
		{
			if (part is PointLight pointLight && pointLight.EntityLight != null)
			{
				m_lights.Add(pointLight.EntityLight);
			}
			else if (part is SpotLight spotLight && spotLight.EntityLight != null)
			{
				m_lights.Add(spotLight.EntityLight);
			}
		}
		int count = m_lights.Count;
		List<EntityLight> lights = m_lights;
		UpdateLights(lights);
		UpdateAlienLightShields(lights);
		foreach (EntityLight item in lights)
		{
			if (item.Type != 4)
			{
				item.m_lightData.Set(item);
			}
			item.UpdateSelf();
		}
		CCDData data = default(CCDData);
		ComparisonData[] array = new ComparisonData[count];
		Dictionary<Rigidbody, (Vector2, float)> dictionary = new Dictionary<Rigidbody, (Vector2, float)>();
		bool[] array2 = new bool[m_componentCount];
		for (int i = 0; i < m_rigidbodyCount; i++)
		{
			RigidbodyData data2 = m_rigidbodyData[i];
			if (!(data2.Rigidbody != null) || data2.Rigidbody.IsFixed())
			{
				continue;
			}
			int num = 0;
			data.Set(data2);
			for (int j = 0; j < count; j++)
			{
				EntityLight entityLight = lights[j];
				if (entityLight.Enabled && ComputeTimeOfImpact(entityLight, ref data, out var time))
				{
					array[num++] = new ComparisonData(j, time);
				}
			}
			Array.Sort(array, 0, num);
			Array.Clear(array2, 0, array2.Length);
			for (int k = 0; k < num; k++)
			{
				EntityLight entityLight2 = lights[array[k].LightIndex];
				int type = entityLight2.Type;
				if (type != 4 || !array2[entityLight2.Index / 5])
				{
					switch (type)
					{
					case 0:
					case 1:
						entityLight2.CCDPillar(ref data);
						break;
					case 2:
						entityLight2.CCDShield(ref data);
						break;
					case 3:
						entityLight2.CCDBox(ref data);
						break;
					default:
						entityLight2.CCDShield(ref data);
						break;
					}
					if (type == 4)
					{
						array2[entityLight2.Index / 5] = true;
					}
				}
			}
			if (data.UsePathPrediction)
			{
				dictionary[data.Rigidbody] = (data.SucPosition, Time.fixedDeltaTime - data.DeltaTime);
			}
		}
		for (int l = 0; l < m_changeData.Length; l++)
		{
			float num2 = 0f;
			float num3 = m_electricities[l];
			List<ChangeData> list = m_changeData[l];
			foreach (ChangeData item2 in list)
			{
				num2 -= item2.Electricity;
			}
			float num4 = ((num2 < num3) ? num2 : num3);
			float num5 = ((num2 > 0f && m_consume) ? Mathf.Sqrt(((num4 > 0f) ? num4 : 0f) / num2) : 1f);
			foreach (ChangeData item3 in list)
			{
				EntityLight light = item3.Light;
				Rigidbody rigidbody = item3.Rigidbody;
				float num6 = item3.Electricity * num5 * num5;
				float num7 = num5 * Defend(item3.Light.m_lightData.SucPosition, item3.Position, num6, m_electricities[light.Index]);
				if (light.Type == 4)
				{
					foreach (EntityLight item4 in lights)
					{
						if (item4.Index == light.Index)
						{
							item4.m_electricity += (m_consume ? (num6 / light.m_coefficient) : 0f);
						}
					}
				}
				else
				{
					light.m_electricity += (m_consume ? num6 : 0f);
				}
				Vector3 position = rigidbody.position;
				position.x += item3.PositionChange.x * num7;
				position.y += item3.PositionChange.y * num7;
				rigidbody.position = position;
				Vector3 force = default(Vector3);
				force.x = item3.VelocityChange.x * num7;
				force.y = item3.VelocityChange.y * num7;
				rigidbody.AddForce(force, ForceMode.VelocityChange);
				rigidbody.AddTorque(new Vector3(0f, 0f, item3.Torque * rigidbody.mass), ForceMode.Impulse);
			}
		}
		Rigidbody[] components = INContraption.Instance.GetComponents<Rigidbody>();
		m_rigidbodyCount = components.Length;
		if (m_rigidbodyData.Length < m_rigidbodyCount)
		{
			m_rigidbodyData = new RigidbodyData[m_rigidbodyCount];
		}
		for (int m = 0; m < m_rigidbodyCount; m++)
		{
			Rigidbody rigidbody2 = components[m];
			if (!dictionary.TryGetValue(rigidbody2, out var value))
			{
				m_rigidbodyData[m] = new RigidbodyData(rigidbody2, rigidbody2.position, Time.fixedDeltaTime, isCollided: false);
			}
			else
			{
				m_rigidbodyData[m] = new RigidbodyData(rigidbody2, value.Item1, value.Item2, isCollided: true);
			}
		}
	}

	private bool ComputeTimeOfImpact(EntityLight light, ref CCDData data, out float time)
	{
		if (light.Type == 0 || light.Type == 1)
		{
			return ComputeTimeOfImpactPillar(light, ref data, out time);
		}
		return ComputeTimeOfImpactCircle(light, ref data, out time);
	}

	private bool ComputeTimeOfImpactPillar(EntityLight light, ref CCDData data, out float time)
	{
		time = 0f;
		EntityLight.LightData lightData = light.m_lightData;
		float length = light.Length;
		float num = data.PrePosition.x - lightData.PrePosition.x;
		float num2 = data.PrePosition.y - lightData.PrePosition.y;
		float num3 = data.SucPosition.x - lightData.SucPosition.x;
		float num4 = data.SucPosition.y - lightData.SucPosition.y;
		float x = lightData.PreDirection.x;
		float y = lightData.PreDirection.y;
		float x2 = lightData.SucDirection.x;
		float y2 = lightData.SucDirection.y;
		float num5 = x * num + y * num2;
		float num6 = x * num2 - y * num;
		float num7 = x2 * num3 + y2 * num4;
		float num8 = x2 * num4 - y2 * num3;
		if ((!(0f < num5) || !(num5 < length)) && (!(0f < num7) || !(num7 < length)))
		{
			return false;
		}
		float num9 = light.Width + m_maxDetectionDistance;
		if ((!(0f - num9 < num6) || !(num6 < num9)) && (!(0f - num9 < num8) || !(num8 < num9)))
		{
			return false;
		}
		if (num6 == num8)
		{
			time = 1f;
			return true;
		}
		float num10 = Mathf.Sqrt((num5 - num7) * (num5 - num7) + (num6 - num8) * (num6 - num8));
		float num11 = (0f - num8) / (num8 - num6);
		time = num10 * num11;
		return true;
	}

	private bool ComputeTimeOfImpactCircle(EntityLight light, ref CCDData data, out float time)
	{
		time = 0f;
		EntityLight.LightData lightData = light.m_lightData;
		float length = light.Length;
		float width = light.Width;
		float cos = light.Cos;
		float x = lightData.PreDirection.x;
		float y = lightData.PreDirection.y;
		float num = data.PrePosition.x - lightData.PrePosition.x;
		float num2 = data.PrePosition.y - lightData.PrePosition.y;
		if (num * x + num2 * y < cos * Mathf.Sqrt(num * num + num2 * num2))
		{
			return false;
		}
		Vector2 vector = lightData.PrePosition - data.PrePosition;
		Vector2 vector2 = lightData.SucPosition - data.SucPosition;
		float num3 = vector.x * vector.x + vector.y * vector.y;
		float num4 = vector2.x * vector2.x + vector2.y * vector2.y;
		float num5 = light.Length + m_maxDetectionDistance;
		num5 *= num5;
		if (num3 > num5 && num4 > num5)
		{
			return false;
		}
		float x2 = data.SucDirection.x;
		float y2 = data.SucDirection.y;
		float num6 = x2 * vector.x + y2 * vector.y;
		float num7 = x2 * vector.y - y2 * vector.x;
		float num8 = x2 * vector2.x + y2 * vector2.y;
		float num9 = x2 * vector2.y - y2 * vector2.x;
		float num10 = length + width;
		INBounds bounds = data.Bounds;
		int num11 = 0;
		float num12 = 0f;
		float num13 = 0f;
		for (int i = 0; i < 2; i++)
		{
			if (num11 >= 2)
			{
				break;
			}
			float num14 = ((i == 0) ? num6 : num7);
			float num15 = ((i == 0) ? num8 : num9);
			float num16 = ((i == 0) ? (bounds.A + num10) : (bounds.B + num10));
			if (!(num15 - num14 > 1E-05f) && !(num15 - num14 < -1E-05f))
			{
				continue;
			}
			for (int j = 0; j < 2 && num11 < 2; j++)
			{
				float num17 = (((j == 0) ? num16 : (0f - num16)) - num14) / (num15 - num14);
				float num18 = ((i == 0) ? ((1f - num17) * num7 + num17 * num9) : ((1f - num17) * num6 + num17 * num8));
				bool num19;
				if (i != 0)
				{
					if (!(num18 > 0f - bounds.A))
					{
						continue;
					}
					num19 = num18 < bounds.A;
				}
				else
				{
					if (!(num18 > 0f - bounds.B))
					{
						continue;
					}
					num19 = num18 < bounds.B;
				}
				if (num19)
				{
					if (num11++ == 0)
					{
						num12 = num17;
					}
					else
					{
						num13 = num17;
					}
				}
			}
		}
		float num20 = (num8 - num6) * (num8 - num6) + (num9 - num7) * (num9 - num7);
		float num21 = num10 * num10 * num20;
		for (int k = 0; k < 4; k++)
		{
			if (num11 >= 2)
			{
				break;
			}
			float num22 = num6 - (((k & 1) == 0) ? bounds.A : (0f - bounds.A));
			float num23 = num7 - (((k & 2) == 0) ? bounds.B : (0f - bounds.B));
			float num24 = num22 * (num8 - num6) + num23 * (num9 - num7);
			float num25 = num22 * (num9 - num7) - num23 * (num8 - num6);
			float num26 = num21 - num25 * num25;
			if (!(num26 > 0f))
			{
				continue;
			}
			float num27 = Mathf.Sqrt(num26);
			for (int l = 0; l < 2; l++)
			{
				if (num11 >= 2)
				{
					break;
				}
				float num28 = (0f - num24 + ((l == 0) ? num27 : (0f - num27))) / num20;
				float num29 = (1f - num28) * num6 + num28 * num8;
				float num30 = (1f - num28) * num7 + num28 * num9;
				if ((((k & 1) == 0) ? num29 : (0f - num29)) > bounds.A && (((k & 2) == 0) ? num30 : (0f - num30)) > bounds.B)
				{
					if (num11++ == 0)
					{
						num12 = num28;
					}
					else
					{
						num13 = num28;
					}
				}
			}
		}
		if (num11 == 2)
		{
			time = ((num12 >= num13) ? num13 : num12);
			return true;
		}
		time = ((num11 == 1) ? num12 : 0f);
		return true;
	}

	private void UpdateLights(List<EntityLight> lights)
	{
		int num = 0;
		int num2 = 5;
		List<BasePart> parts = Contraption.Instance.Parts;
		int[] array = new int[Contraption.Instance.ConnectedComponentCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = -1;
		}
		foreach (EntityLight light in lights)
		{
			int componentIndex = light.ComponentIndex;
			light.Enabled = light.Part.IsEnabled() && !light.Part.IsSinglePart();
			if (componentIndex != -1)
			{
				array[componentIndex] = num++;
			}
		}
		m_componentCount = num;
		foreach (EntityLight light2 in lights)
		{
			int componentIndex2 = light2.ComponentIndex;
			if (componentIndex2 != -1)
			{
				light2.Index = array[componentIndex2] * num2 + light2.Type;
			}
		}
		m_electricities = new float[num * num2];
		m_capacities = new float[num * num2];
		m_lightCounts = new int[num * num2];
		m_changeData = new List<ChangeData>[num * num2];
		float[] array2 = new float[num];
		float[] array3 = new float[num * num2];
		float[] array4 = new float[num * num2];
		for (int j = 0; j < num * num2; j++)
		{
			m_changeData[j] = new List<ChangeData>();
		}
		foreach (BasePart item in parts)
		{
			if ((item.m_partType == BasePart.PartType.EngineSmall && item.m_partTier == BasePart.PartTier.Legendary) || item.m_partType == BasePart.PartType.Engine || item.m_partType == BasePart.PartType.EngineBig)
			{
				int connectedComponent = item.ConnectedComponent;
				if (connectedComponent != -1 && array[connectedComponent] != -1)
				{
					array2[array[connectedComponent]] += 20000f;
				}
			}
		}
		foreach (EntityLight light3 in lights)
		{
			m_electricities[light3.Index] += light3.m_electricity;
			m_capacities[light3.Index] += 5000f;
			m_lightCounts[light3.Index]++;
		}
		for (int k = 0; k < num; k++)
		{
			int num3 = 0;
			for (int l = 0; l < num2; l++)
			{
				num3 += m_lightCounts[k * num2 + l];
			}
			if (num3 != 0)
			{
				for (int m = 0; m < num2; m++)
				{
					m_capacities[k * num2 + m] += array2[k] * (float)m_lightCounts[k * num2 + m] / (float)num3;
				}
			}
		}
		for (int n = 0; n < num * num2; n++)
		{
			ref float reference = ref m_electricities[n];
			float num4 = m_capacities[n];
			float num5 = reference + num4;
			float num6 = num5 + Mathf.Sqrt(num4);
			reference = ((num6 < num4) ? num6 : num4);
			array3[n] = (reference - num5) / (float)m_lightCounts[n];
		}
		for (int num7 = 0; num7 < num; num7++)
		{
			int num8 = 0;
			float num9 = 0f;
			for (int num10 = 0; num10 < num2; num10++)
			{
				num9 += m_electricities[num7 * num2 + num10];
			}
			for (int num11 = num2; num11 > 0; num11--)
			{
				int num12 = 0;
				float num13 = float.MaxValue;
				for (int num14 = 0; num14 < num2; num14++)
				{
					if ((num8 & (1 << num14)) == 0)
					{
						float num15 = m_capacities[num7 * num2 + num14];
						if (num15 < num13)
						{
							num12 = num14;
							num13 = num15;
						}
					}
				}
				num8 |= 1 << num12;
				float num16 = ((num13 < num9 / (float)num11) ? num13 : (num9 / (float)num11));
				num9 -= num16;
				int num17 = m_lightCounts[num7 * num2 + num12];
				if (num17 != 0)
				{
					array4[num7 * num2 + num12] = (num16 - num13) / (float)num17;
				}
			}
		}
		foreach (EntityLight light4 in lights)
		{
			light4.m_electricity += array3[light4.Index];
			light4.m_electricity += (array4[light4.Index] - light4.m_electricity) * 0.04f;
		}
	}

	private void UpdateAlienLightShields(List<EntityLight> lights)
	{
		int num = 0;
		int[] array = new int[Contraption.Instance.ConnectedComponentCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = -1;
		}
		foreach (EntityLight light in lights)
		{
			int connectedComponent = light.Part.ConnectedComponent;
			if (light.Type == 4 && connectedComponent != -1)
			{
				array[connectedComponent] = num++;
			}
		}
		m_alienComponentCount = num;
		List<BasePart> parts = Contraption.Instance.Parts;
		List<BasePart>[] array2 = new List<BasePart>[num];
		List<EntityLight>[] array3 = new List<EntityLight>[num];
		for (int j = 0; j < num; j++)
		{
			array2[j] = new List<BasePart>();
			array3[j] = new List<EntityLight>();
		}
		foreach (BasePart item in parts)
		{
			if (item.ConnectedComponent != -1)
			{
				int num2 = array[item.ConnectedComponent];
				if (num2 != -1)
				{
					array2[num2].Add(item);
				}
			}
		}
		foreach (EntityLight light2 in lights)
		{
			int connectedComponent2 = light2.Part.ConnectedComponent;
			if (light2.Type == 4 && connectedComponent2 != -1)
			{
				int num3 = array[connectedComponent2];
				array3[num3].Add(light2);
			}
		}
		float fixedDeltaTime = Time.fixedDeltaTime;
		for (int k = 0; k < num; k++)
		{
			int num4 = 0;
			float num5 = 0f;
			Vector2 position = default(Vector2);
			Vector2 prePosition = default(Vector2);
			List<BasePart> list = array2[k];
			foreach (BasePart item2 in list)
			{
				if (!item2.HasMultipleRigidbodies())
				{
					num4++;
					Vector3 position2 = item2.transform.position;
					Vector3 velocity = item2.rigidbody.velocity;
					position = new Vector2(position.x + position2.x, position.y + position2.y);
					prePosition = new Vector2(prePosition.x + position2.x - velocity.x * fixedDeltaTime, prePosition.y + position2.y - velocity.y * fixedDeltaTime);
				}
			}
			position /= (float)num4;
			prePosition /= (float)num4;
			foreach (BasePart item3 in list)
			{
				if (!item3.HasMultipleRigidbodies())
				{
					Vector3 position3 = item3.transform.position;
					float num6 = (position3.x - position.x) * (position3.x - position.x) + (position3.y - position.y) * (position3.y - position.y);
					num5 = ((num6 > num5) ? num6 : num5);
				}
			}
			int num7 = 0;
			float num8 = 2f * Mathf.Sqrt(list.Count);
			float num9 = Mathf.Sqrt(num5);
			num9 = ((num9 < num8) ? num9 : num8) + 4f;
			foreach (EntityLight item4 in array3[k])
			{
				if (item4.Enabled)
				{
					num7++;
				}
				Vector3 position4 = item4.Transform.position;
				item4.Transform.position = new Vector3(position.x, position.y, position4.z);
				item4.Length = num9;
				item4.Width = (num9 + 8f) / 64f;
				item4.m_meshFilter.CreateSphereMesh(item4.Length, item4.Width, item4.Angle, 150);
				item4.m_lightData.SetPosition(position, prePosition);
			}
			float coefficient = ((num7 <= 1) ? 1 : num7);
			foreach (EntityLight item5 in array3[k])
			{
				item5.m_coefficient = coefficient;
			}
		}
	}

	public float Defend(Vector2 from, Vector2 to, float deltaElectricity, float electricity)
	{
		int num = 0;
		float num2 = 0f;
		EntityLight[] components = INContraption.Instance.GetComponents<EntityLight>();
		List<(int, float)> list = null;
		bool[] array = new bool[m_componentCount * 5];
		float num3 = 0f;
		for (int i = 0; i < components.Length; i++)
		{
			EntityLight entityLight = components[i];
			if (!entityLight.Enabled || entityLight.Type != 4)
			{
				continue;
			}
			float num4 = m_electricities[entityLight.Index] / m_capacities[entityLight.Index];
			if (!(num4 > 0f))
			{
				continue;
			}
			Vector2 sucPosition = entityLight.m_lightData.SucPosition;
			float num5 = sucPosition.x - to.x;
			float num6 = sucPosition.y - to.y;
			float num7 = entityLight.Length - entityLight.Width;
			float num8 = num7 * num7;
			if (!(num5 * num5 + num6 * num6 < num8))
			{
				continue;
			}
			float num9 = sucPosition.x - from.x;
			float num10 = sucPosition.y - from.y;
			if (num9 * num9 + num10 * num10 > num8)
			{
				if (num == 0)
				{
					list = new List<(int, float)>();
				}
				if (!array[entityLight.Index])
				{
					array[entityLight.Index] = true;
					num3 += m_electricities[entityLight.Index];
				}
				num++;
				num2 += ((num4 > 0.2f) ? 0.25f : (num4 * 1.25f));
				list.Add((i, (num4 > 0.2f) ? 1f : (num4 * 5f)));
			}
		}
		num2 = ((num2 < 0.75f) ? num2 : 0.75f) * ((num3 < electricity) ? Mathf.Sqrt(num3 / electricity) : 1f);
		if (m_consume && num > 0 && deltaElectricity < 0f)
		{
			float num11 = deltaElectricity * num2 / (float)num;
			for (int j = 0; j < list.Count; j++)
			{
				(int, float) tuple = list[j];
				components[tuple.Item1].m_electricity += num11 * tuple.Item2;
			}
		}
		return 1f - num2;
	}
}
