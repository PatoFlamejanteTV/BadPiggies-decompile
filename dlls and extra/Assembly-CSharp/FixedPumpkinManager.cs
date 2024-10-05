using System.Collections.Generic;
using UnityEngine;

public class FixedPumpkinManager : PartManager
{
	private readonly struct RigidbodyData
	{
		public readonly Rigidbody Rigidbody;

		public readonly RigidbodyConstraints Constraints;

		public RigidbodyData(Rigidbody rigidbody, RigidbodyConstraints constraints)
		{
			Rigidbody = rigidbody;
			Constraints = constraints;
		}
	}

	private bool m_needsUpdate;

	private Dictionary<BasePart, RigidbodyData[]> m_data;

	public static FixedPumpkinManager Instance { get; private set; }

	public bool NeedsUpdate
	{
		get
		{
			return m_needsUpdate;
		}
		set
		{
			m_needsUpdate = value;
		}
	}

	protected override void Initialize()
	{
		base.Initialize();
		m_status = StatusCode.Running;
		m_data = new Dictionary<BasePart, RigidbodyData[]>();
		Contraption.Instance.ConnectedComponentsChangedEvent += UpdatePumpkins;
		Instance = this;
	}

	public override void Start()
	{
		UpdatePumpkins();
	}

	public override void FixedUpdate()
	{
		if (m_needsUpdate)
		{
			UpdatePumpkins();
			m_needsUpdate = false;
		}
	}

	public override void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	private void UpdatePumpkins()
	{
		Contraption instance = Contraption.Instance;
		List<BasePart> parts = instance.Parts;
		bool[] array = new bool[instance.StrictConnectedComponentCount];
		bool[] array2 = new bool[instance.StrictConnectedComponentCount];
		int layer = LayerMask.NameToLayer("Ground");
		int layer2 = LayerMask.NameToLayer("Contraption");
		foreach (BasePart item in parts)
		{
			if (item.m_partType == BasePart.PartType.Pumpkin && item.IsEnabled())
			{
				if (item.customPartIndex == 0)
				{
					array[item.StrictConnectedComponent] = true;
				}
				else
				{
					array2[item.StrictConnectedComponent] = true;
				}
			}
		}
		foreach (BasePart item2 in parts)
		{
			bool flag = array[item2.StrictConnectedComponent];
			bool flag2 = array2[item2.StrictConnectedComponent];
			if (flag || flag2)
			{
				if (m_data.ContainsKey(item2))
				{
					continue;
				}
				Rigidbody[] componentsInChildren = item2.GetComponentsInChildren<Rigidbody>();
				RigidbodyData[] array3 = new RigidbodyData[componentsInChildren.Length];
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Rigidbody rigidbody = componentsInChildren[i];
					if (!rigidbody.IsFixed())
					{
						array3[i] = new RigidbodyData(rigidbody, rigidbody.constraints);
						if (flag)
						{
							rigidbody.isKinematic = true;
						}
						rigidbody.constraints = RigidbodyConstraints.FreezeAll;
						rigidbody.gameObject.layer = layer;
						rigidbody.Sleep();
					}
				}
				m_data.Add(item2, array3);
			}
			else
			{
				if (!m_data.TryGetValue(item2, out var value))
				{
					continue;
				}
				RigidbodyData[] array4 = value;
				for (int j = 0; j < array4.Length; j++)
				{
					RigidbodyData rigidbodyData = array4[j];
					Rigidbody rigidbody2 = rigidbodyData.Rigidbody;
					if (rigidbody2 != null)
					{
						rigidbody2.isKinematic = false;
						rigidbody2.constraints = rigidbodyData.Constraints;
						rigidbody2.gameObject.layer = layer2;
						rigidbody2.WakeUp();
					}
				}
				m_data.Remove(item2);
			}
		}
	}
}
