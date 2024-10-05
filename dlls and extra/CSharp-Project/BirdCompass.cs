using UnityEngine;

public class BirdCompass : WPFMonoBehaviour
{
	[SerializeField]
	private BirdCompassIndicator m_indicatorPrefab;

	private Bird[] m_birds;

	private BirdCompassIndicator[] m_indicators;

	private void OnEnable()
	{
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(base.transform.GetChild(num).gameObject);
		}
		m_birds = Object.FindObjectsOfType<Bird>();
		m_indicators = new BirdCompassIndicator[m_birds.Length];
		for (int i = 0; i < m_birds.Length; i++)
		{
			float z = -98f + 0.1f * (float)i;
			BirdCompassIndicator birdCompassIndicator = Object.Instantiate(m_indicatorPrefab, new Vector3(0f, 15f, z), new Quaternion(0f, 0f, 0f, 0f));
			birdCompassIndicator.AttachToBird(m_birds[i]);
			birdCompassIndicator.transform.parent = base.transform;
			m_indicators[i] = birdCompassIndicator;
		}
	}

	private void Update()
	{
		BirdCompassIndicator[] indicators = m_indicators;
		foreach (BirdCompassIndicator birdCompassIndicator in indicators)
		{
			if (!birdCompassIndicator.Done)
			{
				if (birdCompassIndicator.CheckBirdOnScreen())
				{
					birdCompassIndicator.Hide();
				}
				else
				{
					birdCompassIndicator.Show();
				}
			}
		}
	}
}
