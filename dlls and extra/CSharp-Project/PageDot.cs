using UnityEngine;

public class PageDot : MonoBehaviour
{
	private GameObject m_spriteOn;

	private GameObject m_spriteOff;

	private void Awake()
	{
		m_spriteOn = base.transform.Find("SpriteOn").gameObject;
		m_spriteOff = base.transform.Find("SpriteOff").gameObject;
	}

	public void Enable()
	{
		m_spriteOn.SetActive(value: true);
		m_spriteOff.SetActive(value: false);
	}

	public void Disable()
	{
		m_spriteOn.SetActive(value: false);
		m_spriteOff.SetActive(value: true);
	}
}
