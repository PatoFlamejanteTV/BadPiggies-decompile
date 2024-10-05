using System.Collections.Generic;
using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
	private class ScrollData
	{
		public float time;

		public Vector3 delta;

		public ScrollData(float time, Vector3 delta)
		{
			this.time = time;
			this.delta = delta;
		}
	}

	public bool m_disableScrolling;

	private bool m_scrolling;

	private Vector3 m_dragStartPosition;

	private Queue<ScrollData> m_scrollHistory = new Queue<ScrollData>();

	private Vector3 m_scrollVelocity;

	private Vector3 m_position;

	private float wrapLimitUp = -1f;

	private const float clusterOffset = -2f;

	private const float WrapLimitDown = 25f;

	private const float AutoScrollSpeed = 1.5f;

	public void InstantiateCredits()
	{
		float y = 0f;
		List<CreditsCluster> list = new List<CreditsCluster>(GetComponentsInChildren<CreditsCluster>());
		list.Sort((CreditsCluster c1, CreditsCluster c2) => c1.gameObject.name.CompareTo(c2.gameObject.name));
		CreditsCluster[] array = list.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			y = array[i].CreateCreditsCluster(new Vector3(0f, y, 0f)) + -2f;
		}
	}

	public void CleanCredits()
	{
		CreditsCluster[] componentsInChildren = GetComponentsInChildren<CreditsCluster>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			TextMesh[] componentsInChildren2 = componentsInChildren[i].GetComponentsInChildren<TextMesh>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				Object.DestroyImmediate(componentsInChildren2[j].gameObject);
			}
		}
	}

	private void Start()
	{
		if (m_disableScrolling)
		{
			return;
		}
		base.transform.position = new Vector3(base.transform.position.x, 25f, base.transform.position.z);
		m_position = base.transform.position;
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (0f - transform.localPosition.y > wrapLimitUp)
			{
				wrapLimitUp = 0f - transform.localPosition.y;
			}
		}
		wrapLimitUp += 12f;
	}

	private void Awake()
	{
		GameObject.Find("PigJumping").GetComponent<Animation>().Play("PigLevelCompleteAnimation");
		GameObject.Find("PigJumping").GetComponent<Animation>().wrapMode = WrapMode.Loop;
		GameObject.Find("Pig_Shadow").GetComponent<Animation>().Play("LevelCompleteShadowAnimation");
		GameObject.Find("Pig_Shadow").GetComponent<Animation>().wrapMode = WrapMode.Loop;
		string text = $"Version {Singleton<BuildCustomizationLoader>.Instance.ApplicationVersion} - {Singleton<BuildCustomizationLoader>.Instance.SVNRevisionNumber}";
		GameObject.Find("VersionID").GetComponent<TextMesh>().text = text;
		GameObject.Find("VersionIDShadow").GetComponent<TextMesh>().text = text;
	}

	private void OnEnable()
	{
		if (!m_disableScrolling)
		{
			m_scrolling = true;
			base.transform.position = (m_position = new Vector3(m_position.x, 25f, m_position.z));
		}
	}

	private void OnDisable()
	{
		if (!m_disableScrolling)
		{
			base.transform.position = new Vector3(base.transform.position.x, wrapLimitUp, base.transform.position.z);
			m_position = base.transform.position;
		}
	}

	private void Update()
	{
		if (!m_disableScrolling)
		{
			Camera component = GameObject.FindGameObjectWithTag("HUDCamera").GetComponent<Camera>();
			GuiManager.Pointer pointer = GuiManager.GetPointer();
			if (pointer.up)
			{
				m_scrolling = true;
			}
			if (pointer.down)
			{
				m_scrolling = false;
				m_dragStartPosition = component.ScreenToWorldPoint(pointer.position);
			}
			else if (pointer.dragging && !LootCrateOpenDialog.DialogOpen)
			{
				Vector3 vector = component.ScreenToWorldPoint(pointer.position);
				Vector3 newDelta = vector - m_dragStartPosition;
				CalculateScrollVelocity(newDelta);
				m_dragStartPosition = vector;
				m_position.y += newDelta.y;
			}
			else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
			{
				m_position.y += -10f * Input.GetAxis("Mouse ScrollWheel");
			}
			else if (m_scrollVelocity.magnitude > 0.01f)
			{
				Vector3 vector2 = Time.deltaTime * m_scrollVelocity;
				m_scrollVelocity *= Mathf.Pow(0.925f, Time.deltaTime / (1f / 60f));
				m_position.y += vector2.y;
			}
			if (m_scrolling || LootCrateOpenDialog.DialogOpen)
			{
				m_position.y += 1.5f * Time.deltaTime;
			}
			if (m_position.y < 25f)
			{
				m_position = new Vector3(m_position.x, wrapLimitUp, m_position.z);
			}
			else if (m_position.y > wrapLimitUp)
			{
				m_position = new Vector3(m_position.x, 25f, m_position.z);
			}
			base.transform.position = m_position;
		}
	}

	private void CalculateScrollVelocity(Vector3 newDelta)
	{
		while (m_scrollHistory.Count > 0 && m_scrollHistory.Peek().time < Time.time - 0.1f)
		{
			m_scrollHistory.Dequeue();
		}
		m_scrollHistory.Enqueue(new ScrollData(Time.time, newDelta));
		Vector3 zero = Vector3.zero;
		float time = m_scrollHistory.Peek().time;
		float num = 0f;
		foreach (ScrollData item in m_scrollHistory)
		{
			zero += item.delta;
			num = item.time - time;
		}
		if (num > 0f)
		{
			zero /= num;
		}
		m_scrollVelocity = zero;
	}

	public void LaunchEula()
	{
		Singleton<URLManager>.Instance.OpenURL(URLManager.LinkType.Eula);
	}

	public void LaunchPrivacyPolicy()
	{
		Singleton<URLManager>.Instance.OpenURL(URLManager.LinkType.Privacy);
	}
}
