using System;
using System.Collections.Generic;
using UnityEngine;

public class FaceRotation : MonoBehaviour
{
	[Serializable]
	public class TargetInfo
	{
		public bool ignoreX;

		public bool ignoreY;

		public GameObject m_target;

		public float m_zOffset = 1f;

		public Vector2 m_scaleFactor = new Vector2(1f, 1f);

		[HideInInspector]
		public Vector3 m_targetPosition;
	}

	public bool m_followMouse;

	public float m_maxMove = 0.2f;

	public float m_zOffset = -0.5f;

	public List<TargetInfo> m_targets;

	private Vector3 m_normalizedDirection;

	private Vector3 m_targetDirection;

	private bool m_targetDirectionSet;

	private Vector3 m_target2Position;

	[SerializeField]
	[HideInInspector]
	private bool m_positionsSet;

	public void SetTargetDirection(Vector3 direction)
	{
		if (!m_followMouse)
		{
			m_targetDirectionSet = true;
			m_targetDirection = direction;
		}
	}

	public void ScaleFaceZ(float scale)
	{
		foreach (TargetInfo target in m_targets)
		{
			if (target != null)
			{
				TargetInfo targetInfo = target;
				targetInfo.m_targetPosition.z = targetInfo.m_targetPosition.z * scale;
			}
		}
	}

	private void Start()
	{
		if (!m_positionsSet)
		{
			m_positionsSet = true;
			foreach (TargetInfo target in m_targets)
			{
				if (target != null)
				{
					target.m_targetPosition = target.m_target.transform.localPosition;
				}
			}
		}
		m_targetDirectionSet = false;
	}

	private void Update()
	{
		if (m_followMouse)
		{
			Vector3 vector = ((!GameObject.FindGameObjectWithTag("MainCamera")) ? GameObject.FindGameObjectWithTag("HUDCamera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition) : GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition));
			Vector3 vector2 = vector - base.transform.position;
			vector2.z = 0f;
			m_normalizedDirection = 0.3f * vector2;
		}
		if (m_targetDirectionSet)
		{
			m_normalizedDirection = m_targetDirection;
		}
		m_normalizedDirection = Quaternion.Inverse(base.transform.rotation) * m_normalizedDirection;
		if (m_normalizedDirection.sqrMagnitude > 1f)
		{
			m_normalizedDirection.Normalize();
		}
		Vector3 vector3 = m_maxMove * m_normalizedDirection;
		Vector3 normalized = new Vector3(vector3.x, vector3.y, m_zOffset).normalized;
		Quaternion rotation = Quaternion.FromToRotation(new Vector3(0f, 0f, m_zOffset), normalized);
		for (int i = 0; i < m_targets.Count; i++)
		{
			RotateTarget(m_targets[i], rotation);
		}
	}

	private void RotateTarget(TargetInfo info, Quaternion rotation)
	{
		if (info != null)
		{
			Vector3 targetPosition = info.m_targetPosition;
			targetPosition.z = info.m_zOffset;
			Vector3 vector = rotation * targetPosition;
			Quaternion quaternion = Quaternion.FromToRotation(targetPosition, vector);
			Vector3 vector2 = quaternion * Vector3.right;
			Vector3 vector3 = quaternion * Vector3.up;
			Vector3 one = Vector3.one;
			if (info.ignoreX)
			{
				vector.x = info.m_targetPosition.x;
			}
			else
			{
				one.x = vector2.x;
			}
			if (info.ignoreY)
			{
				vector.y = info.m_targetPosition.y;
			}
			else
			{
				one.y = vector3.y;
			}
			vector.z = info.m_targetPosition.z;
			info.m_target.transform.localPosition = vector;
			info.m_target.transform.localScale = new Vector3(info.m_scaleFactor.x * one.x + (1f - info.m_scaleFactor.x), info.m_scaleFactor.y * one.y + (1f - info.m_scaleFactor.y), 1f);
		}
	}
}
