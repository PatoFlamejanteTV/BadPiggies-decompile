using UnityEngine;

public class RopeVisualization : MonoBehaviour
{
	public Material m_stringMaterial;

	public Vector3 m_pos1Anchor;

	public Vector3 m_pos2Anchor;

	public Transform m_pos2Transform;

	private LineRenderer lr;

	private void Awake()
	{
		lr = base.gameObject.AddComponent<LineRenderer>();
	}

	public void Start()
	{
		lr.material = m_stringMaterial;
		lr.SetVertexCount(2);
		lr.SetWidth(0.05f, 0.05f);
		lr.SetColors(Color.black, Color.black);
	}

	private void OnDisable()
	{
		if ((bool)lr)
		{
			lr.enabled = false;
		}
	}

	private void OnEnable()
	{
		if ((bool)lr)
		{
			lr.enabled = true;
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(lr);
	}

	public void LateUpdate()
	{
		Vector3 position = base.transform.TransformPoint(m_pos1Anchor);
		Vector3 position2 = m_pos2Transform.TransformPoint(m_pos2Anchor);
		lr.SetPosition(0, position);
		lr.SetPosition(1, position2);
	}
}
