using UnityEngine;

public class GoalBox : Goal
{
	protected bool collected;

	protected bool disabled;

	protected Material m_flagVisualization;

	[SerializeField]
	protected GameObject m_flagObject;

	[SerializeField]
	protected GameObject m_goalAchievement;

	[SerializeField]
	protected ParticleSystem m_goalParticles;

	protected Vector3 origGoalPosition;

	protected PointLightSource m_pls;

	public GameObject GoalAchivement
	{
		set
		{
			m_goalAchievement = value;
		}
	}

	public bool Collected => collected;

	public bool Disabled => disabled;

	protected override void Start()
	{
		base.Start();
		m_flagVisualization = m_flagObject.GetComponent<Renderer>().material;
		if ((bool)m_goalAchievement)
		{
			origGoalPosition = m_goalAchievement.transform.position;
		}
		m_pls = GetComponent<PointLightSource>();
	}

	protected void Update()
	{
		m_flagVisualization.mainTextureOffset -= Vector2.up * Time.deltaTime * 0.25f;
		if (m_flagVisualization.mainTextureOffset.y < -1f)
		{
			m_flagVisualization.mainTextureOffset = new Vector2(m_flagVisualization.mainTextureOffset.x, m_flagVisualization.mainTextureOffset.y + 1f);
		}
		if ((bool)m_goalAchievement)
		{
			m_goalAchievement.transform.position = origGoalPosition + Vector3.up * Mathf.Sin(Time.time * 3f) * 0.25f;
		}
	}

	protected void DisableGoal()
	{
		disabled = true;
		GetComponent<Collider>().enabled = false;
		if (m_pls != null)
		{
			m_pls.isEnabled = false;
		}
	}

	protected void HideChildren(Transform parent)
	{
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			if ((bool)child.GetComponent<Renderer>())
			{
				child.GetComponent<Renderer>().enabled = false;
			}
			child.gameObject.SetActive(value: false);
			HideChildren(child);
		}
	}

	protected override void OnGoalEnter(BasePart part)
	{
		if (collected)
		{
			return;
		}
		WPFMonoBehaviour.levelManager.NotifyGoalReachedByPart(part.m_partType);
		if (WPFMonoBehaviour.levelManager.PlayerHasRequiredObjects())
		{
			m_flagObject.GetComponent<Animation>().Play();
			if ((bool)m_goalAchievement)
			{
				m_goalAchievement.GetComponent<Animation>().Play();
			}
			if (m_goalParticles != null)
			{
				m_goalParticles.Stop();
			}
			WPFMonoBehaviour.levelManager.NotifyGoalReached();
			collected = true;
			EventManager.Send(default(ObjectiveAchieved));
			DisableGoal();
		}
	}

	protected override void OnReset()
	{
	}
}
