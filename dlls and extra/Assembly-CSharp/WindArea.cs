using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WindArea : MonoBehaviour
{
	[SerializeField]
	public Vector3 windDirectionHandle = Vector3.up;

	public float m_windPowerFactor = 1f;

	public bool m_calculateParticleValues = true;

	private List<BasePart> affectedParts = new List<BasePart>();

	private bool windEnabled;

	private void Awake()
	{
		EventManager.Connect<GameStateChanged>(ReceiveGameStateChangeEvent);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(ReceiveGameStateChangeEvent);
	}

	private void Start()
	{
	}

	private void ReceiveGameStateChangeEvent(GameStateChanged newState)
	{
		if (newState.state == LevelManager.GameState.Running)
		{
			windEnabled = true;
		}
		else
		{
			windEnabled = false;
		}
		if (newState.state == LevelManager.GameState.Building)
		{
			ResetToInitialState();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		BasePart component = other.GetComponent<BasePart>();
		if ((bool)component)
		{
			affectedParts.Add(component);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		BasePart component = other.GetComponent<BasePart>();
		if ((bool)component)
		{
			affectedParts.Remove(component);
		}
	}

	private void FixedUpdate()
	{
		if (!windEnabled)
		{
			return;
		}
		Vector3 vector = WindDirection();
		foreach (BasePart affectedPart in affectedParts)
		{
			if ((bool)affectedPart)
			{
				Rigidbody rigidbody = affectedPart.rigidbody;
				if ((bool)rigidbody)
				{
					rigidbody.AddForce(vector * m_windPowerFactor);
				}
				affectedPart.WindVelocity = 4f * vector.normalized * m_windPowerFactor;
			}
		}
	}

	private void ResetToInitialState()
	{
		affectedParts.Clear();
	}

	private Vector3 WindDirection()
	{
		return windDirectionHandle - base.transform.position;
	}

	private void OnDrawGizmos()
	{
		Bounds bounds = GetComponent<BoxCollider>().bounds;
		for (float num = bounds.min.y; num <= bounds.max.y; num += 8f)
		{
			for (float num2 = bounds.min.x; num2 <= bounds.max.x; num2 += 8f)
			{
				GizmoUtils.DrawArrow(new Vector3(num2, num, base.transform.position.z), WindDirection());
			}
		}
	}
}
