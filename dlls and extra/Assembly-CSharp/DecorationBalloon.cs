using UnityEngine;

public class DecorationBalloon : WPFMonoBehaviour
{
	[SerializeField]
	protected Material m_stringMaterial;

	[SerializeField]
	protected Transform m_anchor;

	protected RopeVisualization m_rope;

	private void Start()
	{
		m_rope = base.gameObject.AddComponent<RopeVisualization>();
		m_rope.m_stringMaterial = m_stringMaterial;
		m_rope.m_pos1Anchor = Vector3.down * 0.5f + 1.1f * Vector3.forward;
		if (m_anchor == null)
		{
			GameObject obj = new GameObject("Anchor");
			obj.transform.parent = base.transform;
			obj.transform.localPosition = Vector3.down * 2.5f;
			m_anchor = obj.transform;
		}
		m_rope.m_pos2Transform = m_anchor;
		Vector3 pos2Anchor = m_anchor.transform.InverseTransformPoint(base.transform.position);
		pos2Anchor.y = 0f;
		m_rope.m_pos2Anchor = pos2Anchor;
	}

	private void Update()
	{
		if (WPFMonoBehaviour.mainCamera == null)
		{
			return;
		}
		Vector3 pos = Vector3.zero;
		bool flag = false;
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began)
			{
				pos = touch.position;
				flag = true;
			}
		}
		if (flag)
		{
			Ray ray = WPFMonoBehaviour.mainCamera.ScreenPointToRay(pos);
			int layerMask = 1 << LayerMask.NameToLayer("Default");
			if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, layerMask) && hitInfo.transform == base.transform)
			{
				Pop();
			}
		}
	}

	[ContextMenu("Pop")]
	public void Pop()
	{
		AudioSource balloonPop = WPFMonoBehaviour.gameData.commonAudioCollection.balloonPop;
		Singleton<AudioManager>.Instance.SpawnOneShotEffect(balloonPop, WPFMonoBehaviour.hudCamera.transform);
		ParticleSystem component = Object.Instantiate(WPFMonoBehaviour.gameData.m_ballonParticles).GetComponent<ParticleSystem>();
		component.transform.position = base.transform.position;
		LayerHelper.SetLayer(component.gameObject, base.gameObject.layer, children: true);
		component.Play();
		Object.Destroy(base.gameObject);
	}
}
