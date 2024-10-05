using System;
using UnityEngine;

public class EntityLight : MonoBehaviour
{
	public struct LightData
	{
		public Vector2 PrePosition;

		public Vector2 SucPosition;

		public Vector2 PreDirection;

		public Vector2 SucDirection;

		public void Set(EntityLight light)
		{
			Vector3 position = light.Transform.position;
			Quaternion rotation = light.Transform.rotation;
			PrePosition = SucPosition;
			PreDirection = SucDirection;
			SucPosition.x = position.x;
			SucPosition.y = position.y;
			SucDirection.x = 1f - (rotation.y * rotation.y * 2f + rotation.z * rotation.z * 2f);
			SucDirection.y = rotation.x * rotation.y * 2f + rotation.w * rotation.z * 2f;
		}

		public void SetPosition(Vector2 position, Vector2 prePosition)
		{
			PrePosition = prePosition;
			SucPosition = position;
		}
	}

	[SerializeField]
	private int m_type;

	[SerializeField]
	private float m_angle;

	[SerializeField]
	private float m_width;

	[SerializeField]
	private float m_length;

	private bool m_enabled;

	private bool m_colored;

	private int m_index;

	private int m_sides;

	private float m_cos;

	public float m_electricity;

	public float m_coefficient;

	private Color m_color;

	private BasePart m_part;

	private Transform m_transform;

	private MeshRenderer m_meshRenderer;

	public MeshFilter m_meshFilter;

	private Collider m_collider;

	public LightData m_lightData;

	public bool Enabled
	{
		get
		{
			return m_enabled;
		}
		set
		{
			m_enabled = value;
			if (IsLightPillar)
			{
				m_collider.enabled = value;
			}
		}
	}

	public int Type
	{
		get
		{
			return m_type;
		}
		set
		{
			m_type = value;
		}
	}

	public float Length
	{
		get
		{
			return m_length;
		}
		set
		{
			m_length = value;
		}
	}

	public float Width
	{
		get
		{
			return m_width;
		}
		set
		{
			m_width = value;
		}
	}

	public float Angle
	{
		get
		{
			return m_angle;
		}
		set
		{
			m_angle = value;
		}
	}

	public float Cos => m_cos;

	public bool IsLightPillar
	{
		get
		{
			if (m_type != 0)
			{
				return m_type == 1;
			}
			return true;
		}
	}

	public bool IsLightShield
	{
		get
		{
			if (m_type != 2)
			{
				return m_type == 4;
			}
			return true;
		}
	}

	public bool IsLightBox => m_type == 3;

	public int Index
	{
		get
		{
			return m_index;
		}
		set
		{
			m_index = value;
		}
	}

	public GameObject Light => m_transform.gameObject;

	public Transform Transform => m_transform;

	public BasePart Part => m_part;

	public int ComponentIndex => m_part.ConnectedComponent;

	private void Awake()
	{
		m_part = GetComponent<BasePart>();
		m_transform = base.transform.Find("INLight");
		if (m_transform == null)
		{
			GameObject gameObject = new GameObject("INLight");
			m_transform = gameObject.transform;
			m_transform.parent = base.transform;
			gameObject.AddComponent<MeshRenderer>();
			gameObject.AddComponent<MeshFilter>();
		}
		m_transform.localPosition = new Vector3(0f, 0.5f, -0.5f);
		m_transform.localRotation = new Quaternion(0f, 0f, 0.70710677f, 0.70710677f);
		m_meshRenderer = m_transform.GetComponent<MeshRenderer>();
		m_meshRenderer.sharedMaterial = new Material(INUnity.TextShader);
		m_meshRenderer.material.color = Color.clear;
		m_meshFilter = m_transform.GetComponent<MeshFilter>();
	}

	private void Start()
	{
		m_transform.gameObject.layer = LayerMask.NameToLayer("Ground");
		if (m_type == 0 || m_type == 1)
		{
			m_meshFilter.CreateBoxMesh(m_length, m_width);
		}
		else
		{
			m_cos = Mathf.Cos(m_angle * ((float)Math.PI / 360f));
			m_meshFilter.CreateSphereMesh(m_length, m_width, m_angle, 150);
		}
		if (!Contraption.Instance.IsRunning)
		{
			return;
		}
		if (IsLightPillar)
		{
			m_collider = m_transform.GetComponent<BoxCollider>();
			if (m_collider == null)
			{
				m_collider = m_transform.gameObject.AddComponent<BoxCollider>();
			}
			BoxCollider obj = m_collider as BoxCollider;
			obj.size = new Vector3(m_length, m_width * 2f, 1f);
			obj.center = new Vector3(m_length * 0.5f, 0f, 0.5f);
			obj.isTrigger = true;
		}
		bool flag = !Contraption.Instance.HasTurboCharge;
		BasePart enclosedInto = m_part.m_enclosedInto;
		if (INSettings.GetBool(INFeature.ColoredFrame) && enclosedInto != null && enclosedInto is ColoredFrame coloredFrame)
		{
			m_colored = true;
			m_color = coloredFrame.Color;
			m_color.a *= (flag ? 0.5f : 0.7f);
			return;
		}
		m_colored = false;
		if (!flag)
		{
			if (m_type == 0)
			{
				m_color = new Color(0.55f, 0.5f, 1f);
			}
			else if (m_type == 1)
			{
				m_color = new Color(0.65f, 0.5f, 1f);
			}
			else if (m_type == 2 || m_type == 4)
			{
				m_color = new Color(0.6f, 0.5f, 1f);
			}
			else if (m_type == 3)
			{
				m_color = new Color(0.7f, 0.5f, 1f);
			}
		}
		else if (m_type == 0)
		{
			m_color = new Color(0.5f, 0.75f, 1f);
		}
		else if (m_type == 1)
		{
			m_color = new Color(0.5f, 0.65f, 1f);
		}
		else if (m_type == 2 || m_type == 4)
		{
			m_color = new Color(0.5f, 0.7f, 1f);
		}
		else if (m_type == 3)
		{
			m_color = new Color(0.5f, 0.6f, 1f);
		}
		m_color.a = (flag ? 0.5f : 0.7f);
	}

	public void UpdateSelf()
	{
		if (m_type != 0 && m_type != 1)
		{
			return;
		}
		m_sides = 0;
		if (Contraption.Instance.ConnectedToGearbox(m_part))
		{
			Gearbox gearbox = Contraption.Instance.GetGearbox(m_part);
			if (gearbox.m_partTier != 0)
			{
				BasePart.GridRotation gridRotation = m_part.m_gridRotation;
				bool flag = gearbox.IsEnabled() ^ (gridRotation == BasePart.GridRotation.Deg_0 || gridRotation == BasePart.GridRotation.Deg_45 || gridRotation == BasePart.GridRotation.Deg_90 || gridRotation == BasePart.GridRotation.Deg_135);
				m_sides = (flag ? 1 : 2);
			}
		}
	}

	public void CCDPillar(ref EntityLightManager.CCDData data)
	{
		LightData lightData = m_lightData;
		float length = m_length;
		float width = m_width;
		float num = data.PrePosition.x - lightData.PrePosition.x;
		float num2 = data.PrePosition.y - lightData.PrePosition.y;
		float num3 = data.SucPosition.x - lightData.SucPosition.x;
		float num4 = data.SucPosition.y - lightData.SucPosition.y;
		float x = lightData.PreDirection.x;
		float y = lightData.PreDirection.y;
		float x2 = lightData.SucDirection.x;
		float y2 = lightData.SucDirection.y;
		float x3 = data.SucDirection.x;
		float y3 = data.SucDirection.y;
		INBounds bounds = data.Bounds;
		bool isRect = bounds.IsRect;
		float num5 = x * num + y * num2;
		float num6 = x * num2 - y * num;
		float num7 = x2 * num3 + y2 * num4;
		float num8 = x2 * num4 - y2 * num3;
		float num9 = x2 * x3 + y2 * y3;
		float num10 = x2 * y3 - y2 * x3;
		float halfProjection = bounds.GetHalfProjection(0f - num10, num9);
		float num11 = num6 - halfProjection - width;
		float num12 = num8 - halfProjection - width;
		float num13 = num6 + halfProjection + width;
		float num14 = num8 + halfProjection + width;
		float num15 = 0.25f;
		bool flag = num11 > 0f - num15 && num12 < 0f && num12 < num11;
		bool flag2 = num13 < num15 && num14 > 0f && num14 > num13;
		if (m_sides != 0)
		{
			flag &= (m_sides & 1) == 0;
			flag2 &= (m_sides & 2) == 0;
		}
		if (!(flag2 || flag) || data.UsePathPrediction)
		{
			return;
		}
		float num16 = (flag ? 1f : (-1f));
		float num17 = (flag ? num11 : num13);
		float num18 = (flag ? num12 : num14);
		float num19 = (0f - num18) / (num18 - num17);
		float num20 = num7 + num19 * (num7 - num5);
		float halfProjection2 = bounds.GetHalfProjection(num9, num10);
		if (0f - halfProjection2 < num20 && num20 < length + halfProjection2)
		{
			float num21 = ((num10 < 0f) ? num9 : (0f - num9)) * bounds.A + ((num9 > 0f) ? num10 : (0f - num10)) * bounds.B;
			num21 *= num16;
			float num22 = halfProjection * (0f - num16);
			num21 = (isRect ? num21 : 0f);
			float num23 = 0.01f;
			bool flag3 = ((num9 > 0f - num23 && num9 < num23) || (num10 > 0f - num23 && num10 < num23)) && isRect;
			float z = data.Rigidbody.angularVelocity.z;
			float num24 = (num7 - num5) / data.DeltaTime;
			float num25 = (num18 - num17) / data.DeltaTime;
			float num26 = num24 - z * num22;
			float num27 = num25 + z * num21;
			float num28 = num25 - num25 / (((num25 > 0f) ? num25 : (0f - num25)) * 0.04f + 10f);
			float electricity = 0.5f * data.Rigidbody.mass * (num25 * num25 - num28 * num28 * 2f);
			Vector2 material = INContraption.GetMaterial(data.Rigidbody);
			float num29 = (0f - num18) * (material.x + 1f);
			float num30 = (0f - num25) * (material.x + 1f);
			Vector2 vector = new Vector2((0f - y2) * num29, x2 * num29);
			data.PrePosition = (data.PrePosition * num18 - data.SucPosition * num17) / (num18 - num17);
			data.SucPosition += vector;
			data.DeltaTime *= num18 / (num18 - num17);
			float num31 = ((num26 > 0f) ? (-1f) : 1f) * material.y * ((num27 > 0f) ? num27 : (0f - num27));
			if ((num26 + num31 > 0f) ^ (num26 > 0f))
			{
				num31 = 0f - num26;
			}
			float num32 = (0f - num21) * 0.5f * ((!flag3) ? num27 : (2f * z * num21)) * (material.x + 1f);
			float num33 = (0f - num31) * num22;
			EntityLightManager.Instance.m_changeData[m_index].Add(new EntityLightManager.ChangeData(this, data.Rigidbody, data.PrePosition, vector, new Vector2((0f - y2) * num30 + x2 * num31, x2 * num30 + y2 * num31), num32 + num33, electricity));
			BasePart component = data.Rigidbody.GetComponent<BasePart>();
			if (component != null)
			{
				component.OnLightEnter(data.SucPosition + new Vector2((0f - y2) * num22, x2 * num22));
			}
		}
	}

	public void CCDShield(ref EntityLightManager.CCDData data)
	{
		LightData lightData = m_lightData;
		Vector2 vector = lightData.PrePosition - data.PrePosition;
		Vector2 vector2 = lightData.SucPosition - data.SucPosition;
		float x = data.SucDirection.x;
		float y = data.SucDirection.y;
		float num = x * vector.x + y * vector.y;
		float num2 = x * vector.y - y * vector.x;
		float num3 = x * vector2.x + y * vector2.y;
		float num4 = x * vector2.y - y * vector2.x;
		float num5 = m_length + m_width;
		INBounds bounds = data.Bounds;
		int num6 = 0;
		Vector3 vector3 = default(Vector3);
		Vector3 vector4 = default(Vector3);
		for (int i = 0; i < 2; i++)
		{
			if (num6 >= 2)
			{
				break;
			}
			float num7 = ((i == 0) ? num : num2);
			float num8 = ((i == 0) ? num3 : num4);
			float num9 = ((i == 0) ? (bounds.A + num5) : (bounds.B + num5));
			if (!(num8 - num7 > 1E-05f) && !(num8 - num7 < -1E-05f))
			{
				continue;
			}
			for (int j = 0; j < 2 && num6 < 2; j++)
			{
				float num10 = (((j == 0) ? num9 : (0f - num9)) - num7) / (num8 - num7);
				float num11 = ((i == 0) ? ((1f - num10) * num2 + num10 * num4) : ((1f - num10) * num + num10 * num3));
				bool num12;
				if (i != 0)
				{
					if (!(num11 > 0f - bounds.A))
					{
						continue;
					}
					num12 = num11 < bounds.A;
				}
				else
				{
					if (!(num11 > 0f - bounds.B))
					{
						continue;
					}
					num12 = num11 < bounds.B;
				}
				if (num12)
				{
					if (num6++ == 0)
					{
						vector3 = ((i == 0) ? new Vector3(num10, (j == 0) ? bounds.A : (0f - bounds.A), num11) : new Vector3(num10, num11, (j == 0) ? bounds.B : (0f - bounds.B)));
					}
					else
					{
						vector4 = ((i == 0) ? new Vector3(num10, (j == 0) ? bounds.A : (0f - bounds.A), num11) : new Vector3(num10, num11, (j == 0) ? bounds.B : (0f - bounds.B)));
					}
				}
			}
		}
		float num13 = (num3 - num) * (num3 - num) + (num4 - num2) * (num4 - num2);
		float num14 = num5 * num5 * num13;
		for (int k = 0; k < 4; k++)
		{
			if (num6 >= 2)
			{
				break;
			}
			float num15 = num - (((k & 1) == 0) ? bounds.A : (0f - bounds.A));
			float num16 = num2 - (((k & 2) == 0) ? bounds.B : (0f - bounds.B));
			float num17 = num15 * (num3 - num) + num16 * (num4 - num2);
			float num18 = num15 * (num4 - num2) - num16 * (num3 - num);
			float num19 = num14 - num18 * num18;
			if (!(num19 > 0f))
			{
				continue;
			}
			float num20 = Mathf.Sqrt(num19);
			for (int l = 0; l < 2; l++)
			{
				if (num6 >= 2)
				{
					break;
				}
				float num21 = (0f - num17 + ((l == 0) ? num20 : (0f - num20))) / num13;
				float num22 = (1f - num21) * num + num21 * num3;
				float num23 = (1f - num21) * num2 + num21 * num4;
				if ((((k & 1) == 0) ? num22 : (0f - num22)) > bounds.A && (((k & 2) == 0) ? num23 : (0f - num23)) > bounds.B)
				{
					if (num6++ == 0)
					{
						vector3 = new Vector3(num21, ((k & 1) == 0) ? bounds.A : (0f - bounds.A), ((k & 2) == 0) ? bounds.B : (0f - bounds.B));
					}
					else
					{
						vector4 = new Vector3(num21, ((k & 1) == 0) ? bounds.A : (0f - bounds.A), ((k & 2) == 0) ? bounds.B : (0f - bounds.B));
					}
				}
			}
		}
		if (num6 <= 0)
		{
			return;
		}
		Vector3 vector5 = ((num6 == 1 || vector3.x < vector4.x) ? vector3 : vector4);
		float num24 = (data.UsePathPrediction ? (Time.fixedDeltaTime - data.DeltaTime) : Time.fixedDeltaTime) / data.DeltaTime;
		if (!(vector5.x < num24 + 1f))
		{
			return;
		}
		float num25 = Mathf.Sqrt(num13);
		Vector2 vector6 = data.PrePosition - lightData.PrePosition;
		Vector2 vector7 = data.SucPosition - lightData.SucPosition;
		Vector2 vector8 = (1f - vector5.x) * vector6 + vector5.x * vector7;
		float num26 = x * vector5.y - y * vector5.z;
		float num27 = x * vector5.z + y * vector5.y;
		float num28 = vector8.x + num26;
		float num29 = vector8.y + num27;
		float num30 = Mathf.Sqrt(num28 * num28 + num29 * num29);
		num28 /= num30;
		num29 /= num30;
		if (!(vector5.x * num25 > -1f))
		{
			return;
		}
		float num31 = num28 * vector6.x + num29 * vector6.y - num30;
		float num32 = num28 * vector7.x + num29 * vector7.y - num30;
		float num33 = num28 * x + num29 * y;
		float num34 = num28 * y - num29 * x;
		float num35 = ((num33 > 0f) ? num33 : (0f - num33)) * bounds.A + ((num34 > 0f) ? num34 : (0f - num34)) * bounds.B;
		float num36 = num31 - num35;
		float num37 = num31 + num35;
		float num38 = num32 - num35;
		float num39 = num32 + num35;
		bool flag = num37 > 0f && num38 < 0f;
		bool flag2 = num39 > 0f && num38 + num24 * (num38 - num36) < 0f;
		bool flag3 = flag && !data.UsePathPrediction;
		if (flag3 || flag2)
		{
			float num40 = num36;
			float num41 = num38;
			float num42 = (num41 - num40) / data.DeltaTime;
			float num43 = num42 - num42 / (((num42 > 0f) ? num42 : (0f - num42)) * 0.04f + 10f);
			float electricity = 0.5f * data.Rigidbody.mass * (num42 * num42 - num43 * num43 * 2f);
			float num44 = INContraption.GetMaterial(data.Rigidbody).x + 1f;
			float num45 = (0f - num41) * num44;
			float num46 = (0f - num42) * num44;
			Vector2 vector9 = new Vector2(num28 * num45, num29 * num45);
			if (!data.UsePathPrediction && flag3)
			{
				data.PrePosition = (data.PrePosition * num41 - data.SucPosition * num40) / (num41 - num40);
				data.SucPosition += vector9;
				data.DeltaTime *= num41 / (num41 - num40);
			}
			else if (!data.UsePathPrediction && !flag3)
			{
				Vector2 sucPosition = data.SucPosition;
				data.SucPosition = (data.PrePosition * num41 - data.SucPosition * num40) / (num41 - num40);
				data.PrePosition = sucPosition + vector9;
				data.DeltaTime = Time.fixedDeltaTime * num41 / (num41 - num40);
			}
			else
			{
				data.SucPosition = (data.PrePosition * num41 - data.SucPosition * num40) / (num41 - num40);
				data.PrePosition += vector9;
				data.DeltaTime += (Time.fixedDeltaTime - data.DeltaTime) * num41 / (num41 - num40);
			}
			data.UsePathPrediction = !flag3;
			EntityLightManager.Instance.m_changeData[m_index].Add(new EntityLightManager.ChangeData(this, data.Rigidbody, data.PrePosition, vector9, new Vector2(num28 * num46, num29 * num46), (0f - num46) * (num28 * num27 - num29 * num26), electricity));
		}
	}

	public void CCDBox(ref EntityLightManager.CCDData data)
	{
		INBounds bounds = data.Bounds;
		Vector2 prePosition = m_lightData.PrePosition;
		float x = m_lightData.PreDirection.x;
		float y = m_lightData.PreDirection.y;
		Vector2 prePosition2 = data.PrePosition;
		float num = prePosition2.x - prePosition.x;
		float num2 = prePosition2.y - prePosition.y;
		if (!(num * x + num2 * y < m_cos * Mathf.Sqrt(num * num + num2 * num2)))
		{
			float x2 = data.SucDirection.x;
			float y2 = data.SucDirection.y;
			num += bounds.X * x2 - bounds.Y * y2;
			num2 += bounds.X * y2 + bounds.Y * x2;
			float num3 = (((num3 = num * x2 + num2 * y2) > 0f) ? num3 : (0f - num3));
			float num4 = (((num4 = num * y2 - num2 * x2) > 0f) ? num4 : (0f - num4));
			float num5 = num3 + bounds.A;
			float num6 = num4 + bounds.B;
			float num7 = (((num7 = num3 - bounds.A) > 0f) ? num7 : 0f);
			float num8 = (((num8 = num4 - bounds.B) > 0f) ? num8 : 0f);
			Vector2 vector = new Vector2(Mathf.Sqrt(num5 * num5 + num6 * num6), Mathf.Sqrt(num7 * num7 + num8 * num8));
			Vector2 sucPosition = m_lightData.SucPosition;
			Rigidbody rigidbody = data.Rigidbody;
			float x3 = data.SucDirection.x;
			float y3 = data.SucDirection.y;
			float num9 = data.SucPosition.x - sucPosition.x + bounds.X * x3 - bounds.Y * y3;
			float num10 = data.SucPosition.y - sucPosition.y + bounds.X * y3 + bounds.Y * x3;
			float num11 = (((num11 = num9 * x3 + num10 * y3) > 0f) ? num11 : (0f - num11));
			float num12 = (((num12 = num9 * y3 - num10 * x3) > 0f) ? num12 : (0f - num12));
			float num13 = num11 + bounds.A;
			float num14 = num12 + bounds.B;
			float num15 = (((num15 = num11 - bounds.A) > 0f) ? num15 : 0f);
			float num16 = (((num16 = num12 - bounds.B) > 0f) ? num16 : 0f);
			float num17 = Mathf.Sqrt(num13 * num13 + num14 * num14);
			float num18 = Mathf.Sqrt(num15 * num15 + num16 * num16);
			float num19 = Mathf.Sqrt(num9 * num9 + num10 * num10);
			if (num19 > 1E-05f && ((num17 > m_length - m_width && vector.y < m_length - m_width) || (num17 * 2f - vector.x > m_length - m_width && num19 < m_length - m_width)) && num17 > vector.x)
			{
				float num20 = (num17 - vector.x) / data.DeltaTime;
				float num21 = num20 - num20 / (((num20 > 0f) ? num20 : (0f - num20)) * data.DeltaTime + 10f);
				float electricity = 0.5f * rigidbody.mass * (num20 * num20 - num21 * num21 * 2f);
				float num22 = (0f - num20) * (INContraption.GetMaterial(rigidbody).x + 1f) / num19;
				float num23 = (0f - num17 + m_length - m_width) / num19;
				data.SucPosition += new Vector2(num9 * num23, num10 * num23);
				EntityLightManager.Instance.m_changeData[m_index].Add(new EntityLightManager.ChangeData(this, rigidbody, data.PrePosition, new Vector2(num9 * num23, num10 * num23), new Vector2(num9 * num22, num10 * num22), 0f, electricity));
			}
			else if (num19 > 1E-05f && num18 < m_length + m_width && vector.x > m_length + m_width && num18 < vector.y)
			{
				float num24 = (num18 - vector.y) / data.DeltaTime;
				float num25 = (0f - num24) / (((num24 > 0f) ? num24 : (0f - num24)) * data.DeltaTime + 5f);
				float electricity2 = -0.5f * rigidbody.mass * ((num24 + num25) * (num24 + num25) - num24 * num24);
				float num26 = num25 / num19;
				EntityLightManager.Instance.m_changeData[m_index].Add(new EntityLightManager.ChangeData(this, rigidbody, data.PrePosition, new Vector2(0f, 0f), new Vector2(num9 * num26, num10 * num26), 0f, electricity2));
			}
		}
	}

	private void Update()
	{
		if (!Contraption.Instance.IsRunning)
		{
			if (m_type == 4)
			{
				m_meshRenderer.material.color = Color.clear;
				return;
			}
			if (m_part.m_enclosedInto != null)
			{
				m_meshRenderer.material.color = new Color(1f, 1f, 1f, 0.1f);
				return;
			}
			Contraption instance = Contraption.Instance;
			int num = 1;
			int num2 = 0;
			for (int i = 0; i < 4; i++)
			{
				BasePart part = instance.FindPartAt(m_part.m_coordX + num, m_part.m_coordY + num2);
				if (instance.CanConnectTo(m_part, part, (BasePart.Direction)i))
				{
					m_meshRenderer.material.color = new Color(1f, 1f, 1f, 0.1f);
					return;
				}
				int num3 = num;
				num = -num2;
				num2 = num3;
			}
			m_meshRenderer.material.color = Color.clear;
		}
		else if (!m_enabled)
		{
			m_meshRenderer.material.color = Color.clear;
		}
		else
		{
			Color color = m_color;
			Color a = (m_colored ? m_color : new Color(1f, 0.25f, 0.25f));
			a.a = 0.1f;
			if (m_type == 4)
			{
				color.a /= m_coefficient;
				a.a /= m_coefficient;
			}
			if (!EntityLightManager.Instance.m_consume)
			{
				m_meshRenderer.material.color = color;
				return;
			}
			float num4 = EntityLightManager.Instance.m_electricities[m_index] / EntityLightManager.Instance.m_capacities[m_index];
			m_meshRenderer.material.color = Color.Lerp(a, color, (num4 > 0f) ? Mathf.Sqrt(num4) : 0f);
		}
	}

	public static bool RaycastWithLights(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
	{
		RaycastHit[] array = Physics.RaycastAll(origin, direction, maxDistance, layerMask);
		int num = -1;
		float num2 = float.PositiveInfinity;
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			if (raycastHit.distance < num2 && CheckRaycast(in raycastHit))
			{
				num2 = raycastHit.distance;
				num = i;
			}
		}
		if (num == -1)
		{
			hitInfo = default(RaycastHit);
			return false;
		}
		hitInfo = array[num];
		return true;
	}

	public static bool CheckRaycast(in RaycastHit raycastHit)
	{
		Rigidbody rigidbody = raycastHit.rigidbody;
		if (rigidbody == null)
		{
			return true;
		}
		EntityLight component = rigidbody.GetComponent<EntityLight>();
		if (component == null || !component.IsLightPillar || !Contraption.Instance.ConnectedToGearbox(component.m_part))
		{
			return true;
		}
		Gearbox gearbox = Contraption.Instance.GetGearbox(component.m_part);
		if (gearbox.m_partTier == BasePart.PartTier.Regular)
		{
			return true;
		}
		Vector2 sucDirection = component.m_lightData.SucDirection;
		Vector2 sucPosition = component.m_lightData.SucPosition;
		Vector3 point = raycastHit.point;
		bool num = sucDirection.x * (point.y - sucPosition.y) - sucDirection.y * (point.x - sucPosition.x) > 0f;
		BasePart.GridRotation gridRotation = component.m_part.m_gridRotation;
		return num ^ gearbox.IsEnabled() ^ (gridRotation == BasePart.GridRotation.Deg_0 || gridRotation == BasePart.GridRotation.Deg_45 || gridRotation == BasePart.GridRotation.Deg_90 || gridRotation == BasePart.GridRotation.Deg_135);
	}
}
