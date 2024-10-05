using System.Collections.Generic;
using UnityEngine;

public class GadgetButtonList : MonoBehaviour
{
	public class NameComparer : IComparer<GameObject>
	{
		public int Compare(GameObject obj1, GameObject obj2)
		{
			return string.Compare(obj1.name, obj2.name);
		}
	}

	[SerializeField]
	private float m_spacing = 1f;

	[SerializeField]
	private List<GadgetButton> m_buttons;

	private Vector2 m_spriteSize;

	private float m_screenWidth;

	private float m_screenHeight;

	public List<GadgetButton> Buttons => m_buttons;

	private void Start()
	{
		if (m_buttons.Count > 0)
		{
			m_spriteSize = m_buttons[0].GetComponent<Sprite>().Size;
		}
	}

	public void Sort(IComparer<GadgetButton> comparer)
	{
		m_buttons.Sort(comparer);
		PlaceButtons(Screen.width, Screen.height);
	}

	private void Update()
	{
		if (INSettings.GetBool(INFeature.InputSettings))
		{
			KeyCode keyCode = KeyCode.Alpha1;
			foreach (GadgetButton button in m_buttons)
			{
				if (button.Enabled)
				{
					if (Input.GetKeyDown(keyCode))
					{
						button.Activate();
					}
					keyCode = keyCode switch
					{
						KeyCode.Alpha9 => KeyCode.Alpha0, 
						KeyCode.Alpha0 => KeyCode.A, 
						_ => keyCode + 1, 
					};
				}
			}
		}
		PlaceButtons(Screen.width, Screen.height);
	}

	private void FixedUpdate()
	{
	}

	private void PlaceButtons(float screenWidth, float screenHeight)
	{
		int num = 0;
		for (int i = 0; i < m_buttons.Count; i++)
		{
			if (m_buttons[i].Enabled)
			{
				num++;
			}
		}
		float num2 = m_spriteSize.x + m_spacing;
		Vector3 vector = base.transform.position - 0.5f * ((float)(num - 1) * num2) * Vector3.right;
		float num3 = 1f;
		float num4 = vector.x - base.transform.position.x;
		float num5 = 10f * screenWidth / screenHeight - 0.5f * num2;
		if (num4 < 0f - num5)
		{
			num2 = m_spriteSize.x + 0.25f * m_spacing;
			num4 = (base.transform.position - 0.5f * ((float)(num - 1) * num2) * Vector3.right).x - base.transform.position.x;
			num5 = 10f * screenWidth / screenHeight - 0.5f * num2;
		}
		if (num4 < 0f - num5)
		{
			num3 = (0f - num5) / num4;
			base.transform.localScale = new Vector3(num3, 1f, 1f);
		}
		else
		{
			base.transform.localScale = Vector3.one;
		}
		vector = base.transform.position - num3 * (0.5f * ((float)(num - 1) * num2) * Vector3.right);
		num2 *= num3;
		for (int j = 0; j < m_buttons.Count; j++)
		{
			if (m_buttons[j].Enabled)
			{
				m_buttons[j].transform.position = vector;
				vector += num2 * Vector3.right;
			}
		}
	}
}
