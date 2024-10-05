using System.Collections.Generic;
using UnityEngine;

public class Egg : BasePart
{
	private bool m_enabled;

	private List<Collider> m_colliders;

	public override bool CanBeEnclosed()
	{
		return true;
	}

	protected override void OnTouch()
	{
		if (INSettings.GetBool(INFeature.SpecialEggs))
		{
			int num = customPartIndex;
			if (num == 2 || num == 3 || num == 4)
			{
				SetEnabled(!m_enabled);
			}
		}
	}

	public override void SetEnabled(bool enabled)
	{
		if (!INSettings.GetBool(INFeature.SpecialEggs) || m_enabled == enabled)
		{
			return;
		}
		m_enabled = enabled;
		if (customPartIndex == 3)
		{
			if (!m_enabled)
			{
				foreach (Collider collider2 in m_colliders)
				{
					if (collider2 != null)
					{
						collider2.enabled = true;
					}
				}
				return;
			}
			m_colliders = new List<Collider>();
			Collider[] components = INContraption.Instance.GetComponents<Collider>();
			foreach (Collider collider in components)
			{
				if (collider.attachedRigidbody != null)
				{
					BasePart component = collider.attachedRigidbody.GetComponent<BasePart>();
					if (component != null && component.ConnectedComponent == base.ConnectedComponent)
					{
						collider.enabled = false;
						m_colliders.Add(collider);
					}
				}
			}
		}
		else
		{
			if (customPartIndex != 4)
			{
				return;
			}
			if (!m_enabled)
			{
				foreach (Collider collider3 in m_colliders)
				{
					if (collider3 != null)
					{
						collider3.enabled = true;
					}
				}
				return;
			}
			m_colliders = new List<Collider>();
		}
	}

	private void FixedUpdate()
	{
		if (!INSettings.GetBool(INFeature.SpecialEggs) || !base.contraption || !base.contraption.IsRunning || !m_enabled || customPartIndex != 4)
		{
			return;
		}
		Vector3 position = base.transform.position;
		foreach (Collider collider2 in m_colliders)
		{
			if (collider2 != null)
			{
				Vector3 position2 = collider2.transform.position;
				float num = position2.x - position.x;
				float num2 = position2.y - position.y;
				if (num * num + num2 * num2 >= 64f)
				{
					collider2.enabled = true;
				}
			}
		}
		m_colliders.Clear();
		Collider[] components = INContraption.Instance.GetComponents<Collider>();
		foreach (Collider collider in components)
		{
			Vector3 position3 = collider.transform.position;
			float num3 = position3.x - position.x;
			float num4 = position3.y - position.y;
			if (num3 * num3 + num4 * num4 < 64f)
			{
				collider.enabled = false;
				m_colliders.Add(collider);
			}
		}
	}

	public override void PostInitialize()
	{
		if (INSettings.GetBool(INFeature.SpecialEggs))
		{
			int num = customPartIndex;
			if (num == 2 || num == 3 || num == 4)
			{
				base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), 1);
				OnTouch();
			}
		}
	}

	public override bool IsEnabled()
	{
		if (INSettings.GetBool(INFeature.SpecialEggs))
		{
			return m_enabled;
		}
		return false;
	}
}
