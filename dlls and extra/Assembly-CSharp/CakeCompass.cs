using CakeRace;
using UnityEngine;

public class CakeCompass : WPFMonoBehaviour
{
	[SerializeField]
	private CakeCompassIndicator m_indicatorPrefab;

	private Cake[] m_cakes;

	private CakeCompassIndicator[] m_indicators;

	private void OnEnable()
	{
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(base.transform.GetChild(num).gameObject);
		}
		m_cakes = Object.FindObjectsOfType<Cake>();
		m_indicators = new CakeCompassIndicator[m_cakes.Length];
		for (int i = 0; i < m_cakes.Length; i++)
		{
			float z = -98f + 0.1f * (float)i;
			CakeCompassIndicator cakeCompassIndicator = Object.Instantiate(m_indicatorPrefab, new Vector3(0f, 15f, z), new Quaternion(0f, 0f, 0f, 0f));
			cakeCompassIndicator.AttachToCake(m_cakes[i]);
			cakeCompassIndicator.transform.parent = base.transform;
			m_indicators[i] = cakeCompassIndicator;
		}
	}

	private void Update()
	{
		CakeCompassIndicator[] indicators = m_indicators;
		foreach (CakeCompassIndicator cakeCompassIndicator in indicators)
		{
			if (!cakeCompassIndicator.Done)
			{
				if (cakeCompassIndicator.CheckCakeOnScreen())
				{
					cakeCompassIndicator.Hide();
				}
				else
				{
					cakeCompassIndicator.Show();
				}
			}
		}
	}
}
