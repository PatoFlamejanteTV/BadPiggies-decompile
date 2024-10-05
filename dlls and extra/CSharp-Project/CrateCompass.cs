using UnityEngine;

public class CrateCompass : MonoBehaviour
{
	[SerializeField]
	private CrateCompassIndicator m_indicatorPrefab;

	private LootCrate[] m_crates;

	private CrateCompassIndicator[] m_indicators;

	private void OnEnable()
	{
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(base.transform.GetChild(num).gameObject);
		}
		m_crates = Object.FindObjectsOfType<LootCrate>();
		m_indicators = new CrateCompassIndicator[m_crates.Length];
		for (int i = 0; i < m_crates.Length; i++)
		{
			float z = -98f + 0.1f * (float)i;
			CrateCompassIndicator crateCompassIndicator = Object.Instantiate(m_indicatorPrefab, new Vector3(0f, 15f, z), new Quaternion(0f, 0f, 0f, 0f));
			crateCompassIndicator.AttachToCrate(m_crates[i]);
			crateCompassIndicator.transform.parent = base.transform;
			m_indicators[i] = crateCompassIndicator;
		}
	}

	private void Update()
	{
		CrateCompassIndicator[] indicators = m_indicators;
		foreach (CrateCompassIndicator crateCompassIndicator in indicators)
		{
			if (!crateCompassIndicator.Done)
			{
				if (crateCompassIndicator.CheckCrateOnScreen())
				{
					crateCompassIndicator.Hide();
				}
				else
				{
					crateCompassIndicator.Show();
				}
			}
		}
	}
}
