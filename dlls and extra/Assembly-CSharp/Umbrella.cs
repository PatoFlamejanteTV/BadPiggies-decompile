using System;
using UnityEngine;

public class Umbrella : BasePart
{
	public float m_force;

	public bool m_enabled;

	private float m_maximumForce;

	private GameObject m_visualization;

	private GameObject m_openSprite;

	private bool m_open = true;

	private float m_timer;

	private float m_moveTime = 0.25f;

	private Vector3 m_closedPosition;

	private Vector3 m_openPosition;

	private bool m_isConnected;

	private float MinScale = 0.4f;

	private float MaxScale = 1f;

	private float m_angle;

	private Vector3 m_position;

	private Vector3 m_direction;

	public override bool CanBeEnabled()
	{
		return m_isConnected;
	}

	public override bool HasOnOffToggle()
	{
		return false;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	public override bool IsPowered()
	{
		return false;
	}

	public override Direction EffectDirection()
	{
		return BasePart.Rotate(Direction.Up, m_gridRotation);
	}

	public override void Awake()
	{
		base.Awake();
		m_visualization = base.transform.Find("UmbrellaVisualization").gameObject;
		m_openSprite = m_visualization.transform.Find("OpenSprite").gameObject;
		m_closedPosition = -0.05f * Vector3.up;
		m_closedPosition.z = m_visualization.transform.localPosition.z;
		m_openPosition = 0.25f * Vector3.up;
		m_openPosition.z = m_visualization.transform.localPosition.z;
	}

	public override void InitializeEngine()
	{
		m_isConnected = base.contraption.ComponentPartCount(base.ConnectedComponent) > 1;
		float num = 0.25f;
		m_maximumForce = m_force * num;
	}

	public override void Initialize()
	{
		base.Initialize();
		m_openPosition = Vector3.zero;
		m_openPosition.z = m_visualization.transform.localPosition.z;
		m_moveTime = 0.25f;
		m_visualization.transform.localPosition = m_openPosition;
		Vector3 localScale = m_visualization.transform.localScale;
		localScale.x = MaxScale;
		localScale.y = 1f;
		m_visualization.transform.localScale = localScale;
		if (INSettings.GetBool(INFeature.SpecialUmbrellas))
		{
			Vector3 up = base.transform.up;
			m_angle = Mathf.Atan2(up.x, up.y);
			m_position = base.transform.position;
			m_direction = up;
			base.rigidbody.mass = 0.5f;
		}
		m_openSprite.transform.localPosition = Vector3.zero;
	}

	private void FixedUpdate()
	{
		if (!base.contraption || !base.contraption.IsRunning)
		{
			return;
		}
		if (m_open)
		{
			bool flag = INSettings.GetBool(INFeature.SpecialUmbrellas) && !this.IsSinglePart();
			if (flag && m_partTier == PartTier.Common)
			{
				float num = (float)Math.PI;
				float num2 = 150f;
				Vector3 up = base.transform.up;
				float num3 = Mathf.Atan2(up.x, up.y);
				float z = base.rigidbody.angularVelocity.z;
				float num4 = num3 - m_angle;
				num4 = ((num4 < 0f - num) ? (num4 + 2f * num) : ((num4 > num) ? (num4 - 2f * num) : num4));
				float num5 = -16f * z;
				float angle = num3;
				if (customPartIndex == 6)
				{
					num5 = -32f * z + 400f * num4;
					angle = m_angle + num4 * Mathf.Clamp01(10f * Mathf.Abs(num4) - 0.2f);
					angle = ((angle < 0f - num) ? (angle + 2f * num) : ((angle > num) ? (angle - 2f * num) : angle));
				}
				num5 = ((num5 < 0f - num2) ? (0f - num2) : ((num5 > num2) ? num2 : num5));
				m_angle = angle;
				base.rigidbody.AddForce(new Vector3((0f - up.y) * num5, up.x * num5), ForceMode.Force);
				base.rigidbody.AddTorque(new Vector3(0f, 0f, -64f * z + 1024f * num4), ForceMode.Acceleration);
			}
			else if (flag && m_partTier == PartTier.Rare)
			{
				Vector3 velocity = base.rigidbody.velocity;
				float magnitude = velocity.magnitude;
				float num6 = ((magnitude > 4f) ? (4f / magnitude) : 1f);
				base.rigidbody.AddForce(new Vector3((0f - velocity.x) * num6, (0f - velocity.y) * num6), ForceMode.VelocityChange);
				base.rigidbody.AddTorque(new Vector3(0f, 0f, -64f * base.rigidbody.angularVelocity.z), ForceMode.Acceleration);
				m_position = m_position * num6 + base.rigidbody.position * (1f - num6);
				Vector3 force = (m_position - base.rigidbody.position) / Time.deltaTime;
				force.z = 0f;
				float magnitude2 = force.magnitude;
				if (magnitude2 > 4f)
				{
					force *= 4f / magnitude2;
				}
				base.rigidbody.AddForce(force, ForceMode.VelocityChange);
			}
			else if (flag && m_partTier == PartTier.Epic)
			{
				float num7 = (float)Math.PI;
				float num8 = 150f;
				float num9 = 10000f;
				Vector3 up2 = base.transform.up;
				Vector3 velocity2 = base.rigidbody.velocity;
				float z2 = base.rigidbody.angularVelocity.z;
				float num10 = Mathf.Atan2(up2.x, up2.y) - m_angle;
				num10 = ((num10 < 0f - num7) ? (num10 + 2f * num7) : ((num10 > num7) ? (num10 - 2f * num7) : num10));
				float num11 = -32f * z2 + 400f * num10;
				num11 *= ((customPartIndex == 3) ? 1f : 1.5f);
				Vector3 vector = ((customPartIndex == 3) ? up2 : m_direction);
				vector = new Vector3(0f - vector.y, vector.x);
				float num12 = (vector.x * velocity2.x + vector.y * velocity2.y) * num11;
				num11 *= ((num12 > num9) ? (num9 / num12) : 1f);
				num11 = ((num11 < 0f - num8) ? (0f - num8) : ((num11 > num8) ? num8 : num11));
				base.rigidbody.AddForce(new Vector3(vector.x * num11, vector.y * num11), ForceMode.Force);
				base.rigidbody.AddTorque(new Vector3(0f, 0f, -64f * z2 + 1024f * num10), ForceMode.Acceleration);
			}
			else
			{
				Vector3 position = base.transform.position + 0.2f * base.transform.up;
				Vector3 vector2 = base.rigidbody.velocity - base.WindVelocity;
				base.WindVelocity = Vector3.zero;
				float num13 = Vector3.Dot(base.transform.up, vector2);
				float num14 = Mathf.Sign(Vector3.Cross(vector2, base.transform.right).z);
				float num15 = num14 * num13 * num13;
				if (Mathf.Abs(num15) > 60f)
				{
					num15 = 60f * Mathf.Sign(num15);
				}
				Vector3 up3 = base.transform.up;
				base.rigidbody.AddForceAtPosition(num15 * up3, position, ForceMode.Force);
				num13 = Vector3.Dot(base.transform.right, vector2);
				num14 = Mathf.Sign(Vector3.Cross(base.transform.up, vector2).z);
				num15 = 0.5f * num14 * num13 * num13;
				if (Mathf.Abs(num15) > 60f)
				{
					num15 = 60f * Mathf.Sign(num15);
				}
				up3 = base.transform.right;
				base.rigidbody.AddForceAtPosition(num15 * up3, position, ForceMode.Force);
			}
		}
		if (!m_enabled)
		{
			return;
		}
		m_timer += Time.deltaTime;
		if (m_open)
		{
			float a = m_timer / m_moveTime;
			a = Mathf.Min(a, 1f);
			Vector3 localPosition = Vector3.Lerp(m_openPosition, m_closedPosition, a);
			Vector3 localScale = m_visualization.transform.localScale;
			a = Mathf.Pow(a, 3f);
			localScale.x = a * MinScale + (1f - a) * MaxScale;
			localScale.y = a * 2f + (1f - a) * 1f;
			localPosition.y -= 0.45f * (localScale.y - 1f);
			m_visualization.transform.localScale = localScale;
			m_visualization.transform.localPosition = localPosition;
			float maximumForce = m_maximumForce;
			base.rigidbody.AddForce(maximumForce * base.transform.up, ForceMode.Force);
		}
		else
		{
			float a2 = m_timer / m_moveTime;
			a2 = Mathf.Min(a2, 1f);
			Vector3 localPosition2 = Vector3.Lerp(m_closedPosition, m_openPosition, a2);
			Vector3 localScale2 = m_visualization.transform.localScale;
			a2 = Mathf.Pow(a2, 6f);
			localScale2.x = a2 * MaxScale + (1f - a2) * MinScale;
			localScale2.y = a2 * 1f + (1f - a2) * 2f;
			localPosition2.y -= 0.45f * (localScale2.y - 1f);
			m_visualization.transform.localScale = localScale2;
			m_visualization.transform.localPosition = localPosition2;
			float num16 = -0.2f * m_maximumForce;
			base.rigidbody.AddForce(num16 * base.transform.up, ForceMode.Force);
		}
		if (!(m_timer > m_moveTime))
		{
			return;
		}
		m_timer = 0f;
		m_open = !m_open;
		m_enabled = false;
		if (m_open)
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.umbrellaOpen, base.transform.position);
			if (INSettings.GetBool(INFeature.SpecialUmbrellas) && m_partTier != PartTier.Epic)
			{
				Vector3 up4 = base.transform.up;
				m_angle = Mathf.Atan2(up4.x, up4.y);
				m_position = base.transform.position;
			}
		}
		else
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.umbrellaClose, base.transform.position);
		}
	}

	protected override void OnTouch()
	{
		if (m_isConnected || m_enabled)
		{
			SetEnabled(!m_enabled);
		}
	}

	public override void SetEnabled(bool enabled)
	{
		m_enabled = enabled;
	}

	private void Open(bool open)
	{
		if (m_open != open)
		{
			m_open = open;
			if (open)
			{
				m_openSprite.GetComponent<Renderer>().enabled = true;
				m_openSprite.GetComponent<Collider>().enabled = true;
				m_openSprite.GetComponent<Collider>().isTrigger = false;
				m_visualization.transform.localPosition = m_openPosition;
			}
			else
			{
				m_openSprite.GetComponent<Renderer>().enabled = false;
				m_openSprite.GetComponent<Collider>().enabled = false;
				m_openSprite.GetComponent<Collider>().isTrigger = true;
				m_visualization.transform.localPosition = m_closedPosition;
			}
		}
	}
}
