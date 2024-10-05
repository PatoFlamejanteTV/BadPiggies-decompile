using UnityEngine;

public class VisibilityMultiCondition : MonoBehaviour
{
	[SerializeField]
	private VisibilityCondition.ConditionStruct[] conditions;

	[SerializeField]
	private bool disableGameObject;

	private bool[] lastStates;

	private bool lastState;

	private Renderer m_renderer;

	private Collider m_collider;

	private Transform m_transform;

	private void Awake()
	{
		if (Singleton<VisibilityConditionManager>.IsInstantiated())
		{
			lastStates = new bool[conditions.Length];
			m_renderer = base.gameObject.GetComponent<Renderer>();
			m_collider = base.gameObject.GetComponent<Collider>();
			m_transform = base.gameObject.transform;
			for (int i = 0; i < conditions.Length; i++)
			{
				Singleton<VisibilityConditionManager>.Instance.SubscribeToConditionChange(OnConditionChange, conditions[i].condition);
			}
			UpdateState(force: true);
		}
	}

	private void OnDestroy()
	{
		if (Singleton<VisibilityConditionManager>.IsInstantiated())
		{
			for (int i = 0; i < conditions.Length; i++)
			{
				Singleton<VisibilityConditionManager>.Instance.UnsubscribeToConditionChange(OnConditionChange, conditions[i].condition);
			}
		}
	}

	private void UpdateState(bool force = false)
	{
		bool flag = true;
		for (int i = 0; i < lastStates.Length; i++)
		{
			if (!lastStates[i])
			{
				flag = false;
				break;
			}
		}
		if (flag != lastState || force)
		{
			lastState = flag;
			SetEnabled(flag);
		}
	}

	private void OnConditionChange(VisibilityCondition.Condition condition, bool state)
	{
		if (GetConditionIndex(condition, out var index))
		{
			lastStates[index] = ((!conditions[index].not) ? state : (!state));
			UpdateState();
		}
	}

	private bool GetConditionIndex(VisibilityCondition.Condition condition, out int index)
	{
		index = 0;
		for (int i = 0; i < conditions.Length; i++)
		{
			if (conditions[i].condition == condition)
			{
				index = i;
				return true;
			}
		}
		return false;
	}

	private void SetEnabled(bool enabled)
	{
		bool flag = false;
		if ((bool)m_renderer && m_renderer.enabled != enabled)
		{
			flag = true;
			m_renderer.enabled = enabled;
		}
		if ((bool)m_collider && m_collider.enabled != enabled)
		{
			flag = true;
			m_collider.enabled = enabled;
		}
		if (flag || !m_renderer)
		{
			int childCount = m_transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Renderer component = m_transform.GetChild(i).GetComponent<Renderer>();
				if ((bool)component && component.enabled != enabled)
				{
					component.enabled = enabled;
				}
			}
		}
		if (disableGameObject)
		{
			base.gameObject.SetActive(enabled);
		}
	}
}
