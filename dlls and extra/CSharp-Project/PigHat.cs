using System;
using System.Collections;
using UnityEngine;

public class PigHat : MonoBehaviour
{
	private enum State
	{
		Inactive,
		Moving,
		Detaching
	}

	public float m_speedThreshold = 1f;

	public GameObject[] m_hats;

	private State? m_state = State.Inactive;

	private Vector3 m_lastPos;

	private float m_lastTime;

	private Transform m_transform;

	private GameObject m_hat;

	private void Start()
	{
		m_transform = base.transform;
		if (m_transform.childCount > 0)
		{
			m_hat = m_transform.GetChild(0).gameObject;
		}
		if (m_hat == null && m_hats.Length != 0 && Singleton<GameManager>.IsInstantiated() && Singleton<GameManager>.Instance.GetGameState() == GameManager.GameState.Level && Singleton<GameManager>.Instance.CurrentEpisodeLabel.Equals("6"))
		{
			int num = UnityEngine.Random.Range(0, m_hats.Length);
			m_hat = UnityEngine.Object.Instantiate(m_hats[num], m_transform.position, m_transform.rotation);
			m_hat.transform.parent = m_transform;
			m_hat.transform.localPosition = m_hats[num].transform.position;
			m_state = State.Inactive;
		}
		if ((Singleton<GameManager>.IsInstantiated() && !Singleton<GameManager>.Instance.CurrentEpisodeLabel.Equals("6")) || m_hats.Length == 0)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		State? state = m_state;
		if (!state.HasValue)
		{
			return;
		}
		State? state2 = m_state;
		if (state2.HasValue)
		{
			switch (state2.Value)
			{
			case State.Detaching:
				Detaching();
				break;
			case State.Moving:
				Moving();
				break;
			case State.Inactive:
				Inactive();
				break;
			}
		}
	}

	private void Inactive()
	{
		if (!(WPFMonoBehaviour.levelManager == null) && WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Running && !(WPFMonoBehaviour.levelManager.TimeElapsed <= 0f))
		{
			m_lastPos = m_transform.position;
			m_lastTime = Time.time;
			m_state = State.Moving;
		}
	}

	private void Moving()
	{
		float time = Time.time;
		float num = time - m_lastTime;
		if (Vector3.Distance(m_transform.position, m_lastPos) / num > m_speedThreshold)
		{
			m_state = State.Detaching;
			return;
		}
		m_lastPos = m_transform.position;
		m_lastTime = time;
	}

	private void Detaching()
	{
		Transform obj = m_hat.transform.Find("Visualization");
		Quaternion rotation = obj.rotation;
		m_transform.parent = null;
		m_transform.localRotation = Quaternion.identity;
		obj.rotation = rotation;
		m_hat.GetComponent<Animation>().Play();
		m_state = null;
		StartCoroutine(DelayedAction(delegate
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}, 1.2f));
	}

	public void Show(bool show)
	{
		if (!(m_hat == null))
		{
			Renderer[] componentsInChildren = m_hat.GetComponentsInChildren<Renderer>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = show;
			}
		}
	}

	private IEnumerator DelayedAction(Action action, float delay)
	{
		yield return new WaitForSeconds(delay);
		action();
	}
}
