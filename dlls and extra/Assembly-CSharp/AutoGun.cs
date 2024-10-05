using System;
using UnityEngine;

public class AutoGun : ExplodingGrapplingHook
{
	private bool m_activated;

	public override bool CanBeEnabled()
	{
		return true;
	}

	public override bool IsEnabled()
	{
		return m_activated;
	}

	protected override void OnTouch()
	{
		m_activated = !m_activated;
	}

	private void FixedUpdate()
	{
		if (!base.contraption || !base.contraption.IsRunning || !IsEnabled() || this.IsSinglePart())
		{
			return;
		}
		float @float = INSettings.GetFloat(INFeature.AutoGunAngle);
		float float2 = INSettings.GetFloat(INFeature.AutoGunMaxDistance);
		float float3 = INSettings.GetFloat(INFeature.AutoGunRandomProbability);
		float num = Mathf.Cos(@float * 0.5f * ((float)Math.PI / 180f));
		bool flag = false;
		Vector3 position = base.transform.position;
		Vector3 right = base.transform.right;
		Vector3 vector = base.rigidbody.velocity + right * float2;
		float magnitude = vector.magnitude;
		float num2 = float.PositiveInfinity;
		foreach (BasePart part in base.contraption.Parts)
		{
			if (MarkerManager.IsInSameTeamStatic(this, part))
			{
				continue;
			}
			Vector3 position2 = part.transform.position;
			if (part.HasMultipleRigidbodies())
			{
				position2 = part.Position;
			}
			Vector3 velocity = part.rigidbody.velocity;
			float num3 = position2.x - position.x;
			float num4 = position2.y - position.y;
			float num5 = Mathf.Sqrt(num3 * num3 + num4 * num4);
			if (!((right.x * num3 + right.y * num4) / num5 > num) || !(num5 < float2))
			{
				continue;
			}
			float num6 = velocity.x - vector.x;
			float num7 = velocity.y - vector.y;
			float num8 = Mathf.Sqrt(num6 * num6 + num7 * num7);
			float num9 = (0f - (num6 * num3 + num7 * num4)) / num8;
			if (!(num8 > 1E-05f) || !(num9 > 0f))
			{
				continue;
			}
			float num10 = num9 / num8;
			float num11 = (0f - (num6 * num4 - num7 * num3)) / num8;
			float num12 = 2f + num10 * 8f;
			num12 = ((num12 > 8f) ? 8f : num12);
			if (0f - num12 < num11 && num11 < num12)
			{
				float num13 = num10 * magnitude;
				if (num13 < num2)
				{
					num2 = num13;
				}
				flag = true;
			}
		}
		int num14 = LayerMask.NameToLayer("Ground");
		int num15 = LayerMask.NameToLayer("IceGround");
		RaycastHit[] array = Physics.RaycastAll(position, right, num2);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit = array[i];
			Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
			int layer = raycastHit.collider.gameObject.layer;
			if (layer == num14 || layer == num15)
			{
				flag = false;
				break;
			}
			if (attachedRigidbody != null)
			{
				BasePart component = attachedRigidbody.GetComponent<BasePart>();
				if (component != null && component.ConnectedComponent != base.ConnectedComponent && MarkerManager.IsInSameTeamStatic(this, component))
				{
					flag = false;
					break;
				}
			}
		}
		if (flag && UnityEngine.Random.Range(0f, 1f) < float3)
		{
			Shoot();
		}
	}
}
