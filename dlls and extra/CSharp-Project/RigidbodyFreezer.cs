using UnityEngine;

public class RigidbodyFreezer : WPFMonoBehaviour
{
	public float sqrDistanceLimit = 500f;

	public bool unfreezeOnce = true;

	private const string CONTRAPTION_NAME = "Part_Pig_01_SET(Clone)(Clone)";

	private Transform contraptionTf;

	private Rigidbody[] rigidbodies;

	private RigidbodyConstraints[] origConstraints;

	private bool frozen;

	private void Start()
	{
		EventManager.Connect<UIEvent>(ReceiveUIEvent);
		frozen = false;
		rigidbodies = GetComponentsInChildren<Rigidbody>(includeInactive: true);
		origConstraints = new RigidbodyConstraints[rigidbodies.Length];
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			origConstraints[i] = rigidbodies[i].constraints;
		}
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<UIEvent>(ReceiveUIEvent);
	}

	private void Update()
	{
		CheckDistance();
	}

	private void ReceiveUIEvent(UIEvent data)
	{
		if (data.type == UIEvent.Type.Play)
		{
			Freeze();
			FindContraption();
		}
	}

	private void FindContraption()
	{
		if (contraptionTf == null)
		{
			BasePart basePart = WPFMonoBehaviour.levelManager.ContraptionRunning.FindPart(BasePart.PartType.Pig);
			if ((bool)basePart)
			{
				contraptionTf = basePart.transform;
			}
		}
		_ = contraptionTf == null;
	}

	private void Freeze()
	{
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			if ((bool)rigidbodies[i])
			{
				rigidbodies[i].constraints = RigidbodyConstraints.FreezeAll;
				rigidbodies[i].Sleep();
			}
		}
		frozen = true;
	}

	private void Unfreeze()
	{
		for (int i = 0; i < rigidbodies.Length; i++)
		{
			rigidbodies[i].constraints = origConstraints[i];
			rigidbodies[i].WakeUp();
		}
		frozen = false;
	}

	private void CheckDistance()
	{
		if (contraptionTf == null || (unfreezeOnce && !frozen))
		{
			return;
		}
		if ((contraptionTf.position - base.transform.position).sqrMagnitude < sqrDistanceLimit)
		{
			if (frozen)
			{
				Unfreeze();
			}
		}
		else if (!frozen)
		{
			Freeze();
		}
	}
}
