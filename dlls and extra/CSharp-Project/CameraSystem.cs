using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
	public List<GameObject> m_cameraPrefabs;

	private void Awake()
	{
		Vector3 position = base.transform.position;
		position.z += -100f;
		foreach (GameObject cameraPrefab in m_cameraPrefabs)
		{
			GameObject obj = Object.Instantiate(cameraPrefab);
			obj.name = cameraPrefab.name;
			obj.transform.parent = base.transform;
			obj.transform.position = position;
		}
	}
}
