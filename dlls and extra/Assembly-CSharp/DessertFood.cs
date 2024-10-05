using UnityEngine;

public class DessertFood : MonoBehaviour
{
	[HideInInspector]
	public Widget m_DessertButton;

	[HideInInspector]
	public DessertSelector m_DessertSelector;

	private void Start()
	{
		Object.Destroy(base.gameObject, 10f);
	}

	public void OnMouthTriggerEnter(Collider other)
	{
		if ((bool)m_DessertSelector && !m_DessertSelector.IsEating())
		{
			m_DessertSelector.EatDessert(m_DessertButton);
			base.gameObject.GetComponent<Collider>().enabled = false;
			Object.Destroy(base.gameObject, 0.02f);
		}
	}

	public void OnCollisionEnter(Collision coll)
	{
		if (coll.collider.tag == "Ground")
		{
			if (coll.relativeVelocity.magnitude > 3f && (bool)m_DessertSelector && !m_DessertSelector.IsEating())
			{
				m_DessertSelector.MissDessert(m_DessertButton);
			}
			Object.Destroy(base.gameObject);
		}
	}
}
