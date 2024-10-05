using UnityEngine;

public class InstantiateOnStart : MonoBehaviour
{
	public GameObject m_Prefab;

	public bool m_MakeItChild = true;

	public bool m_selfRendererEnabled = true;

	private void Start()
	{
		if ((bool)GetComponent<Renderer>())
		{
			GetComponent<Renderer>().enabled = m_selfRendererEnabled;
		}
		if ((bool)m_Prefab)
		{
			GameObject gameObject = Object.Instantiate(m_Prefab, base.transform.position, base.transform.rotation);
			if (m_MakeItChild)
			{
				gameObject.transform.parent = base.transform;
			}
		}
	}
}
