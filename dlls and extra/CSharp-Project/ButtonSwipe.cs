using UnityEngine;

[RequireComponent(typeof(Button))]
public class ButtonSwipe : MonoBehaviour
{
	public bool m_directional = true;

	public Vector3 m_direction;

	public float m_thresholdDistance = 0.3f;

	public bool m_disableButtonRelease;

	private Button m_button;

	private Vector3 m_dragStart;

	private void OnEnable()
	{
		m_button = GetComponent<Button>();
		m_button.SetInputDelegate(OnButtonInput);
		if (m_disableButtonRelease)
		{
			m_button.SetActivateOnRelease(activate: false);
		}
	}

	private void OnButtonInput(InputEvent input)
	{
		if (input.type == InputEvent.EventType.Press)
		{
			m_dragStart = input.position;
		}
		if (input.type == InputEvent.EventType.Drag)
		{
			Vector3 rhs = input.position - m_dragStart;
			rhs /= (float)Screen.height;
			float num = ((!m_directional) ? rhs.magnitude : Vector3.Dot(m_direction.normalized, rhs));
			if (num > m_thresholdDistance)
			{
				m_button.Activate();
			}
		}
	}
}
