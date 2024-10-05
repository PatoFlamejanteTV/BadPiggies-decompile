using System.Collections.Generic;
using UnityEngine;

public class Tail : BasePart
{
	public float liftConstant;

	private ResponseCurve liftCoefficients = new ResponseCurve();

	private bool m_enabled;

	private int m_rotationIndex;

	public override bool ValidatePart()
	{
		if (!WPFMonoBehaviour.levelManager.RequireConnectedContraption)
		{
			return true;
		}
		List<BasePart> list = base.contraption.FindNeighbours(m_coordX, m_coordY);
		int num = 0;
		foreach (BasePart item in list)
		{
			if (item.IsPartOfChassis())
			{
				num++;
			}
		}
		return num >= 1;
	}

	private void Start()
	{
		liftCoefficients.AddPoint(-180f, 0f);
		liftCoefficients.AddPoint(-135f, -1.5f);
		liftCoefficients.AddPoint(-90f, 0f);
		liftCoefficients.AddPoint(-45f, -1.5f);
		liftCoefficients.AddPoint(-10f, 0f);
		liftCoefficients.AddPoint(10f, 1f);
		liftCoefficients.AddPoint(45f, 1.5f);
		liftCoefficients.AddPoint(90f, 0f);
		liftCoefficients.AddPoint(135f, -1.5f);
		liftCoefficients.AddPoint(180f, 0f);
	}

	public override void EnsureRigidbody()
	{
		if (base.rigidbody == null)
		{
			base.rigidbody = base.gameObject.AddComponent<Rigidbody>();
		}
		base.rigidbody.constraints = (RigidbodyConstraints)56;
		base.rigidbody.mass = m_mass;
		base.rigidbody.drag = 1f;
		base.rigidbody.angularDrag = 0.2f;
		base.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
	}

	public void FixedUpdate()
	{
		if ((bool)base.contraption && base.contraption.IsRunning && (!INSettings.GetBool(INFeature.SwitchableTail) || m_enabled))
		{
			Vector3 vector = base.rigidbody.velocity - base.WindVelocity;
			base.WindVelocity = Vector3.zero;
			Vector3 right = base.transform.right;
			float num = ((!IsFlipped()) ? 1f : (-1f));
			float num2 = Vector3.Angle(new Vector3(num, 0f, 0f), right);
			num2 = Mathf.Sign(Vector3.Cross(new Vector3(1f, 0f, 0f), right).z) * num2;
			right = Quaternion.AngleAxis(0.4f * (num2 - 30f), base.transform.forward) * right;
			float x = num * Mathf.Sign(Vector3.Cross(vector, right).z) * Vector3.Angle(vector, right);
			float num3 = liftCoefficients.Get(x);
			Vector3 vector2 = Vector3.Cross(base.transform.forward, vector.normalized);
			Vector3 vector3 = liftConstant * vector.sqrMagnitude * num3 * vector2;
			vector3 = Vector3.ClampMagnitude(vector3, 100f);
			base.rigidbody.AddForce(vector3, ForceMode.Force);
		}
	}

	public override void PrePlaced()
	{
		base.PrePlaced();
		if (INSettings.GetBool(INFeature.RotatableWing))
		{
			m_autoAlign = (AutoAlignType)(-1);
		}
	}

	public override void SetRotation(GridRotation rotation)
	{
		SetCustomRotation((int)rotation * 2 + (m_flipped ? 1 : 0));
	}

	public override void SetFlipped(bool flipped)
	{
		SetCustomRotation((int)m_gridRotation * 2 + (flipped ? 1 : 0));
	}

	public override int GetCustomRotation()
	{
		return m_rotationIndex;
	}

	public override void SetCustomRotation(int rotation)
	{
		int num = rotation % 8;
		int num2 = num / 2;
		bool flag = num % 2 == 1;
		if (m_flipped != flag)
		{
			OnFlipped();
		}
		m_rotationIndex = num;
		m_gridRotation = (GridRotation)num2;
		m_flipped = flag;
		int num3 = ((flag && (num2 == 1 || num2 == 3)) ? 180 : 0);
		int num4 = ((flag && (num2 == 0 || num2 == 2)) ? 180 : 0);
		int num5 = 90 * num2;
		base.transform.localRotation = Quaternion.Euler(num3, num4, num5);
	}

	protected override void OnTouch()
	{
		if (INSettings.GetBool(INFeature.SwitchableTail))
		{
			m_enabled = !m_enabled;
		}
	}

	public override void Initialize()
	{
		if (INSettings.GetBool(INFeature.SwitchableTail))
		{
			base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), 1);
			OnTouch();
		}
	}

	public override bool IsEnabled()
	{
		if (INSettings.GetBool(INFeature.SwitchableTail))
		{
			return m_enabled;
		}
		return false;
	}
}
