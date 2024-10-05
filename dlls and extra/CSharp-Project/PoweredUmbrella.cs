using UnityEngine;

public class PoweredUmbrella : BasePart
{
	public float m_force;

	public bool m_enabled;

	private float m_maximumForce;

	private GameObject m_visualization;

	private bool m_open = true;

	private float m_timer;

	private float m_moveTime = 0.25f;

	private Vector3 m_closedPosition;

	private Vector3 m_openPosition;

	private bool m_hasPower;

	private const float MinScale = 0.4f;

	private float MaxScale = 1f;

	public override bool CanBeEnabled()
	{
		return m_hasPower;
	}

	public override bool HasOnOffToggle()
	{
		return true;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	public override bool IsPowered()
	{
		return true;
	}

	public override Direction EffectDirection()
	{
		return BasePart.Rotate(Direction.Up, m_gridRotation);
	}

	public override void Awake()
	{
		base.Awake();
		m_visualization = base.transform.Find("UmbrellaVisualization").gameObject;
		m_closedPosition = -0.05f * Vector3.up;
		m_closedPosition.z = m_visualization.transform.localPosition.z;
		m_openPosition = 0.25f * Vector3.up;
		m_openPosition.z = m_visualization.transform.localPosition.z;
	}

	public override void InitializeEngine()
	{
		float num = base.contraption.GetEnginePowerFactor(this);
		m_hasPower = num > 0f;
		if (num > 1f)
		{
			num = Mathf.Pow(num, 0.75f);
		}
		if (base.contraption.HasComponentEngine(base.ConnectedComponent))
		{
			m_openPosition = 0.25f * Vector3.up;
			m_openPosition.z = m_visualization.transform.localPosition.z;
			float num2 = 7f * num;
			m_moveTime = 1f / num2;
			MaxScale = 1.1f;
		}
		else
		{
			m_openPosition = 0.25f * Vector3.up;
			m_openPosition.z = m_visualization.transform.localPosition.z;
			float num3 = 7f * num;
			m_moveTime = 1f / num3;
			MaxScale = 1.1f;
			num *= 0.5f;
		}
		m_maximumForce = m_force * num;
	}

	public override void Initialize()
	{
		base.Initialize();
		if (m_open)
		{
			m_visualization.transform.localPosition = m_openPosition;
			Vector3 localScale = m_visualization.transform.localScale;
			localScale.x = MaxScale;
			localScale.y = 1f;
			m_visualization.transform.localScale = localScale;
			m_timer = 0f;
			float a = m_timer / m_moveTime;
			a = Mathf.Min(a, 1f);
			a = Mathf.Min(a, 1f);
			Vector3 localPosition = Vector3.Lerp(m_openPosition, m_closedPosition, a);
			localScale = m_visualization.transform.localScale;
			a = Mathf.Pow(a, 3f);
			localScale.x = a * 0.4f + (1f - a) * MaxScale;
			localScale.y = a * 2f + (1f - a) * 1f;
			localPosition.y -= 0.45f * (localScale.y - 1f);
			m_visualization.transform.localScale = localScale;
			m_visualization.transform.localPosition = localPosition;
		}
		else
		{
			m_visualization.transform.localPosition = m_closedPosition;
		}
	}

	private void FixedUpdate()
	{
		if (!base.contraption || !base.contraption.IsRunning)
		{
			return;
		}
		if (m_open)
		{
			Vector3 position = base.transform.position + 0.2f * base.transform.up;
			Vector3 vector = base.rigidbody.velocity - base.WindVelocity;
			base.WindVelocity = Vector3.zero;
			float magnitude = (Vector3.Dot(base.transform.up, vector.normalized) * vector).magnitude;
			float num = Mathf.Sign(Vector3.Cross(vector, base.transform.right).z) * 1f * magnitude * magnitude;
			if (Mathf.Abs(num) > 60f)
			{
				num = 60f * Mathf.Sign(num);
			}
			Vector3 up = base.transform.up;
			base.rigidbody.AddForceAtPosition(num * up, position, ForceMode.Force);
			magnitude = (Vector3.Dot(base.transform.right, vector.normalized) * vector).magnitude;
			num = Mathf.Sign(Vector3.Cross(base.transform.up, vector).z) * 0.5f * magnitude * magnitude;
			if (Mathf.Abs(num) > 60f)
			{
				num = 60f * Mathf.Sign(num);
			}
			up = base.transform.right;
			base.rigidbody.AddForceAtPosition(num * up, position, ForceMode.Force);
		}
		if (m_enabled)
		{
			m_timer += Time.deltaTime;
			if (m_open)
			{
				float a = m_timer / m_moveTime;
				a = Mathf.Min(a, 1f);
				Vector3 localPosition = Vector3.Lerp(m_openPosition, m_closedPosition, a);
				Vector3 localScale = m_visualization.transform.localScale;
				a = Mathf.Pow(a, 3f);
				localScale.x = a * 0.4f + (1f - a) * MaxScale;
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
				localScale2.x = a2 * MaxScale + (1f - a2) * 0.4f;
				localScale2.y = a2 * 1f + (1f - a2) * 2f;
				localPosition2.y -= 0.45f * (localScale2.y - 1f);
				m_visualization.transform.localScale = localScale2;
				m_visualization.transform.localPosition = localPosition2;
				float num2 = -0.2f * m_maximumForce;
				base.rigidbody.AddForce(num2 * base.transform.up, ForceMode.Force);
			}
			if (m_timer > m_moveTime)
			{
				m_timer = 0f;
				m_open = !m_open;
			}
		}
	}

	protected override void OnTouch()
	{
		if (m_hasPower || m_enabled)
		{
			SetEnabled(!m_enabled);
		}
	}

	public override void SetEnabled(bool enabled)
	{
		m_enabled = enabled;
		base.contraption.UpdateEngineStates(base.ConnectedComponent);
	}
}
