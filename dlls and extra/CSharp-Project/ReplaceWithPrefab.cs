using UnityEngine;

public class ReplaceWithPrefab : MonoBehaviour
{
	public GameObject prefab;

	private void Awake()
	{
		if (!(prefab == null))
		{
			GameObject obj = Object.Instantiate(prefab);
			obj.transform.parent = base.transform.parent;
			obj.transform.localPosition = base.transform.localPosition;
			obj.transform.localRotation = base.transform.localRotation;
			obj.transform.localScale = base.transform.localScale;
			Object.Destroy(base.gameObject);
		}
	}
}
