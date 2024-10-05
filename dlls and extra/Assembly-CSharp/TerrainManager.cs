using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
	public bool m_outlineDetailMesh;

	public bool m_innerDetailMesh;

	[HideInInspector]
	public List<Vector3> m_outlineVertexList = new List<Vector3>();

	[HideInInspector]
	public List<Vector3> m_innerMeshPointsList = new List<Vector3>();

	public int m_innerDetailMeshAMount;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		foreach (Vector3 outlineVertex in m_outlineVertexList)
		{
			Gizmos.DrawWireSphere(outlineVertex, 0.01f);
		}
		foreach (Vector3 innerMeshPoints in m_innerMeshPointsList)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(innerMeshPoints, 0.1f);
		}
	}
}
