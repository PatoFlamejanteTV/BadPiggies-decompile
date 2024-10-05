using UnityEngine;

public class KingPigMouth : WPFMonoBehaviour
{
	private Collider m_collider;

	private void Awake()
	{
		m_collider = GetComponent<Collider>();
		if (Singleton<GameManager>.Instance.IsInGame() && m_collider != null)
		{
			m_collider.enabled = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		DessertFood component = other.gameObject.GetComponent<DessertFood>();
		if (component != null)
		{
			component.OnMouthTriggerEnter(m_collider);
		}
	}
}
