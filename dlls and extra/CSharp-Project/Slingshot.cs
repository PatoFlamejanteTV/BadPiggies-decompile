using UnityEngine;

public class Slingshot : WPFMonoBehaviour
{
	private enum State
	{
		Free,
		InUse
	}

	public Material m_rubberBandMaterial;

	private State m_state;

	private LineRenderer m_line1;

	private LineRenderer m_line2;

	private GameObject m_pad;

	private Vector3 m_restPosition;

	private Vector3 m_drawPosition;

	private void Awake()
	{
		m_pad = base.transform.Find("Pad").gameObject;
		m_restPosition = m_pad.transform.localPosition;
		GameObject gameObject = new GameObject("Line1");
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		GameObject gameObject2 = new GameObject("Line2");
		gameObject2.transform.parent = base.transform;
		gameObject2.transform.localPosition = Vector3.zero;
		m_line1 = gameObject.AddComponent<LineRenderer>();
		m_line1.useWorldSpace = false;
		m_line1.SetWidth(0.25f, 0.25f);
		m_line1.sharedMaterial = m_rubberBandMaterial;
		m_line1.SetVertexCount(2);
		m_line1.SetPosition(0, new Vector3(0.47f, 2.23f, -0.012f));
		m_line1.SetPosition(1, new Vector3(-3f, -1f, -0.012f));
		m_line2 = gameObject2.AddComponent<LineRenderer>();
		m_line2.useWorldSpace = false;
		m_line2.SetWidth(0.25f, 0.25f);
		m_line2.sharedMaterial = m_rubberBandMaterial;
		m_line2.SetVertexCount(2);
		m_line2.SetPosition(0, new Vector3(-0.57f, 2.2f, -0.38f));
		m_line2.SetPosition(1, new Vector3(-3f, -1f, -0.38f));
		SetDrawPosition(Vector3.zero);
	}

	private void Update()
	{
		if (m_state == State.Free && m_drawPosition.sqrMagnitude > 0.0001f)
		{
			m_drawPosition *= 0.95f;
			SetDrawPosition(m_drawPosition);
		}
	}

	public void SetDrawPosition(Vector3 drawPosition)
	{
		m_drawPosition = drawPosition;
		Vector3 localPosition = drawPosition + m_restPosition;
		localPosition.z = m_restPosition.z;
		m_line1.SetPosition(1, new Vector3(localPosition.x, localPosition.y, -0.012f));
		m_line2.SetPosition(1, new Vector3(localPosition.x, localPosition.y, -0.38f));
		m_pad.transform.localPosition = localPosition;
		Vector3 vector = -drawPosition.normalized;
		Quaternion quaternion = Quaternion.AngleAxis(57.29578f * Mathf.Atan2(vector.y, vector.x), Vector3.forward);
		if (m_state == State.Free)
		{
			float num = 2f * drawPosition.sqrMagnitude;
			if (num < 1f)
			{
				quaternion = Quaternion.RotateTowards(quaternion, Quaternion.AngleAxis(0f, Vector3.forward), 180f * (1f - num));
			}
		}
		m_pad.transform.rotation = quaternion;
	}

	public bool IsFree()
	{
		return m_state == State.Free;
	}

	public void StartShot()
	{
		m_state = State.InUse;
		Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.slingshotStretched);
	}

	public void EndShot()
	{
		m_state = State.Free;
	}
}
