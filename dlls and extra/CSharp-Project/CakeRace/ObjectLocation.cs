using System;
using UnityEngine;

namespace CakeRace;

[Serializable]
public struct ObjectLocation
{
	[SerializeField]
	private Vector3 m_position;

	[SerializeField]
	private Quaternion m_rotation;

	[SerializeField]
	private GameObject m_prefab;

	public Vector3 Position => m_position;

	public Quaternion Rotation => m_rotation;

	public GameObject Prefab => m_prefab;
}
