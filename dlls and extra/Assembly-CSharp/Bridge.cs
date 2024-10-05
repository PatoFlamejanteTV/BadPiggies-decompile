using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Bridge : ExportAction
{
	private const string achievementId = "grp.LPA_BRIDGE_BREAK";

	private static bool achievementSent;

	public GameObject stepPrefab;

	public GameObject stepRopePrefab;

	public float stepLength = 1f;

	public float stepGap = 0.2f;

	private Transform stepParent;

	private Transform endPoint;

	public List<Transform> steps;

	public List<GameObject> stepRopes;

	private List<JointBreakEvent> jbEvents;

	[SerializeField]
	private List<float> stepBreakForces;

	private bool isBroken;

	private void Awake()
	{
		EventManager.Connect<GameStateChanged>(OnGameStateChanged);
		if (!LevelLoader.IsLoadingLevel())
		{
			OnDataLoaded();
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
	}

	public void OnDataLoaded()
	{
		stepParent = base.transform.Find("StepParent");
		endPoint = base.transform.Find("EndPoint");
		if (stepPrefab == null || endPoint == null)
		{
			return;
		}
		Vector3 vector = endPoint.position - base.transform.position;
		float f = vector.magnitude / (stepLength + stepGap);
		steps = new List<Transform>();
		for (int i = 0; i < stepParent.childCount; i++)
		{
			Transform child = stepParent.GetChild(i);
			steps.Add(child);
		}
		int num = steps.Count - Mathf.FloorToInt(f);
		if (num > 0)
		{
			for (int j = 0; j < num; j++)
			{
				UnityEngine.Object.DestroyImmediate(steps[steps.Count - 1].gameObject);
				steps.RemoveAt(steps.Count - 1);
			}
		}
		int num2 = 0;
		while (steps.Count < Mathf.FloorToInt(f) && num2 <= 100)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(stepPrefab);
			gameObject.transform.parent = stepParent;
			gameObject.name = $"Step{steps.Count:000}";
			steps.Add(gameObject.transform);
			num2++;
		}
		HingeJoint hingeJoint = null;
		for (int k = 0; k < steps.Count; k++)
		{
			steps[k].localPosition = Vector3.right * ((float)k * (stepLength + stepGap) + (stepLength + stepGap) * 0.5f);
			steps[k].localRotation = Quaternion.identity;
			HingeJoint component = steps[k].GetComponent<HingeJoint>();
			if (hingeJoint != null)
			{
				component.connectedBody = hingeJoint.GetComponent<Rigidbody>();
			}
			component.autoConfigureConnectedAnchor = true;
			hingeJoint = component;
		}
		Transform transform = base.transform.Find("StepRopeParent");
		if (transform != null)
		{
			UnityEngine.Object.DestroyImmediate(transform.gameObject);
		}
		if (transform == null)
		{
			transform = new GameObject("StepRopeParent").transform;
			transform.parent = base.transform;
		}
		transform.localPosition = Vector3.zero;
		if (stepRopes == null)
		{
			stepRopes = new List<GameObject>();
		}
		stepRopes.Clear();
		for (int l = 0; l < steps.Count + 1; l++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(stepRopePrefab);
			gameObject2.name = $"StepRope{l:000}";
			gameObject2.transform.parent = transform;
			if (l < steps.Count)
			{
				gameObject2.transform.position = steps[l].position;
			}
			else
			{
				gameObject2.transform.position = endPoint.position;
			}
			stepRopes.Add(gameObject2);
		}
		for (int m = 0; m < stepParent.childCount; m++)
		{
			HingeJoint component2 = stepParent.GetChild(m).GetComponent<HingeJoint>();
			if ((bool)component2 && stepBreakForces.Count > m)
			{
				component2.breakForce = stepBreakForces[m] * INSettings.GetFloat(INFeature.TerrainScale);
				if (stepBreakForces[m] < 0.1f)
				{
					isBroken = true;
				}
			}
		}
		Transform transform2 = steps[steps.Count - 1];
		HingeJoint component3 = endPoint.GetComponent<HingeJoint>();
		component3.connectedBody = transform2.GetComponent<Rigidbody>();
		component3.breakForce *= INSettings.GetFloat(INFeature.TerrainScale);
		float num3 = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		stepParent.localEulerAngles = Vector3.forward * num3;
		endPoint.position = transform2.position + transform2.right * (stepLength * 0.5f + stepGap);
		base.transform.rotation = Quaternion.identity;
		if (!Application.isPlaying)
		{
			return;
		}
		foreach (Transform step in steps)
		{
			BridgeStep bridgeStep = step.GetComponent<BridgeStep>();
			LevelRigidbody component4 = step.GetComponent<LevelRigidbody>();
			if ((bool)component4)
			{
				UnityEngine.Object.DestroyImmediate(component4);
			}
			if (!bridgeStep)
			{
				bridgeStep = step.gameObject.AddComponent<BridgeStep>();
			}
			bridgeStep.Init(isKinematic: false);
		}
		LevelRigidbody component5 = endPoint.GetComponent<LevelRigidbody>();
		if ((bool)component5)
		{
			UnityEngine.Object.DestroyImmediate(component5);
		}
		BridgeStep bridgeStep2 = endPoint.GetComponent<BridgeStep>();
		if (!bridgeStep2)
		{
			bridgeStep2 = endPoint.gameObject.AddComponent<BridgeStep>();
		}
		bridgeStep2.Init(isKinematic: true);
	}

	public void ClearAllSteps()
	{
		stepBreakForces = new List<float>();
		for (int i = 0; i < stepParent.childCount; i++)
		{
			HingeJoint component = stepParent.GetChild(i).GetComponent<HingeJoint>();
			if ((bool)component)
			{
				stepBreakForces.Add(component.breakForce);
			}
		}
		for (int num = stepParent.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.DestroyImmediate(stepParent.GetChild(num).gameObject);
		}
		Transform transform = base.transform.Find("StepRopeParent");
		if (transform != null)
		{
			UnityEngine.Object.DestroyImmediate(transform.gameObject);
		}
	}

	public override void StartActions()
	{
		if (stepParent == null)
		{
			stepParent = base.transform.Find("StepParent");
		}
		stepParent.rotation = Quaternion.identity;
		ClearAllSteps();
	}

	public override void EndActions()
	{
		OnDataLoaded();
	}

	private void Update()
	{
		if (steps == null || stepRopes == null || endPoint == null)
		{
			return;
		}
		for (int i = 0; i < stepRopes.Count; i++)
		{
			if (stepRopes[i] == null)
			{
				continue;
			}
			Vector3 position = base.transform.position;
			Vector3 position2 = endPoint.position;
			Vector3 vector = steps[(i < steps.Count) ? i : 0].right * stepLength * 0.5f;
			if (i == 0)
			{
				position = base.transform.position;
				position2 = steps[i].position - vector;
			}
			else
			{
				Vector3 vector2 = steps[i - 1].right * stepLength * 0.5f;
				if (i == steps.Count)
				{
					position = steps[i - 1].position + vector2;
					position2 = endPoint.position;
				}
				else
				{
					position = steps[i - 1].position + vector2;
					position2 = steps[i].position - vector;
				}
			}
			Vector3 vector3 = position2 - position;
			float num = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
			stepRopes[i].transform.position = position + Vector3.forward * 0.01f;
			stepRopes[i].transform.eulerAngles = Vector3.forward * num;
			stepRopes[i].transform.localScale = ((vector3.magnitude >= 1f) ? (Vector3.one * 0.01f) : (Vector3.one - Vector3.right * vector3.magnitude));
		}
	}

	private void OnGameStateChanged(GameStateChanged newState)
	{
		if (newState.state == LevelManager.GameState.PreviewMoving && newState.prevState == LevelManager.GameState.Preview)
		{
			LoadStepStates();
		}
		if ((newState.state == LevelManager.GameState.Building || newState.state == LevelManager.GameState.ShowingUnlockedParts) && (newState.prevState == LevelManager.GameState.Running || newState.prevState == LevelManager.GameState.PausedWhileRunning) && !isBroken)
		{
			LoadStepStates();
		}
	}

	private void LoadStepStates()
	{
		List<BridgeStep> list = new List<BridgeStep>();
		int num = 0;
		for (int i = 0; i < steps.Count; i++)
		{
			if ((bool)steps[i].GetComponent<HingeJoint>())
			{
				num++;
			}
			BridgeStep component = steps[i].GetComponent<BridgeStep>();
			if ((bool)component)
			{
				list.Add(component);
			}
		}
		if ((bool)endPoint.GetComponent<HingeJoint>())
		{
			num++;
		}
		if (num != steps.Count + 1)
		{
			for (int j = 0; j < list.Count; j++)
			{
				list[j].LoadState();
			}
			BridgeStep component2 = endPoint.GetComponent<BridgeStep>();
			if ((bool)component2)
			{
				component2.LoadState();
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (endPoint == null)
		{
			return;
		}
		Gizmos.color = Color.green;
		for (int i = 0; i < steps.Count; i++)
		{
			Gizmos.color = ((i % 2 != 0) ? Color.red : Color.green);
			Vector3 vector = steps[i].right * stepLength * 0.5f;
			if (i == 0)
			{
				Gizmos.DrawLine(base.transform.position, steps[i].position - vector);
				continue;
			}
			Vector3 vector2 = steps[i - 1].right * stepLength * 0.5f;
			Gizmos.DrawLine(steps[i - 1].position + vector2, steps[i].position - vector);
			if (i == steps.Count - 1)
			{
				Gizmos.color = (((i + 1) % 2 != 0) ? Color.red : Color.green);
				Gizmos.DrawLine(steps[i].position + vector, endPoint.position);
			}
		}
	}

	private void OnBridgeBroken(object sender)
	{
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Running || WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.PreviewWhileRunning)
		{
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(Singleton<GameManager>.Instance.gameData.commonAudioCollection.bridgeBreak, base.transform.position);
		}
		foreach (JointBreakEvent jbEvent in jbEvents)
		{
			jbEvent.onJointBreak = (JointBreakEvent.JointBreak)Delegate.Remove(jbEvent.onJointBreak, new JointBreakEvent.JointBreak(OnBridgeBroken));
		}
	}
}
