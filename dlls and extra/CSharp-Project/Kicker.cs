using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Kicker : BasePart
{
	public bool m_enabled;

	private List<BasePart> m_connectedParts;

	private List<bool> m_isOverDistance;

	private List<(Rigidbody, Joint)> m_connectedJoints;

	private List<(Rigidbody, Joint)> m_originalConnectedJoints;

	private bool m_isTriggered;

	private List<Joint> m_JointsToBreak;

	private GameObject[] m_colorMarks = new GameObject[8];

	private float m_connectTime;

	public override Direction EffectDirection()
	{
		return BasePart.Rotate(Direction.Right, m_gridRotation);
	}

	public override void SetRotation(GridRotation rotation)
	{
		m_gridRotation = rotation;
		SetColorMark();
	}

	private void SetColorMark()
	{
		int num = m_colorMarks.Length;
		while (--num >= 0)
		{
			if ((bool)m_colorMarks[num])
			{
				m_colorMarks[num].GetComponent<Renderer>().enabled = false;
			}
		}
		if ((bool)m_colorMarks[(int)m_gridRotation])
		{
			m_colorMarks[(int)m_gridRotation].GetComponent<Renderer>().enabled = true;
		}
	}

	public override void Awake()
	{
		base.Awake();
		m_colorMarks[0] = base.transform.Find("Deg0").gameObject;
		m_colorMarks[1] = base.transform.Find("Deg90").gameObject;
		m_colorMarks[2] = base.transform.Find("Deg180").gameObject;
		m_colorMarks[3] = base.transform.Find("Deg270").gameObject;
		SetColorMark();
	}

	public override void Initialize()
	{
		if (WPFMonoBehaviour.levelManager.ContraptionRunning == null || (INSettings.GetBool(INFeature.SeparatorConnection) && m_enclosedInto != null))
		{
			return;
		}
		m_JointsToBreak = WPFMonoBehaviour.levelManager.ContraptionRunning.FindPartFixedJoints(this);
		if (INSettings.GetBool(INFeature.AutoConnector) && this.IsAutoConnector())
		{
			m_connectedJoints = new List<(Rigidbody, Joint)>();
			m_originalConnectedJoints = new List<(Rigidbody, Joint)>();
			foreach (Joint item in m_JointsToBreak)
			{
				if (item != null && item.GetComponent<Rigidbody>() == base.rigidbody)
				{
					m_originalConnectedJoints.Add((item.connectedBody, item));
				}
			}
		}
		if (!INSettings.GetBool(INFeature.MarkerSeparator) || !this.IsMarker())
		{
			base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), 1);
		}
	}

	protected override void OnTouch()
	{
		if (INSettings.GetBool(INFeature.MarkerSeparator) && this.IsMarker())
		{
			return;
		}
		base.rigidbody.WakeUp();
		if (INSettings.GetBool(INFeature.SeparatorConnection))
		{
			if (m_enclosedInto != null)
			{
				return;
			}
			if (this.IsAutoConnector() && m_isTriggered)
			{
				m_enabled = !m_enabled;
				if (m_enabled)
				{
					return;
				}
				foreach (var connectedJoint in m_connectedJoints)
				{
					Joint item = connectedJoint.Item2;
					bool flag = false;
					if (item != null)
					{
						UnityEngine.Object.Destroy(item);
						flag = true;
					}
					if (flag)
					{
						Contraption.Instance.UpdateConnectedComponents();
					}
				}
				m_connectedJoints.Clear();
				return;
			}
			if (m_partTier != 0 && m_isTriggered)
			{
				m_enabled = !m_enabled;
				base.contraption.UpdateConnectedComponents();
				return;
			}
			if (m_partTier == PartTier.Regular)
			{
				base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), -1);
			}
			m_isTriggered = true;
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.kickerDetach, base.transform.position);
			m_connectedParts = new List<BasePart>();
			m_isOverDistance = new List<bool>();
			for (int i = 0; i < m_JointsToBreak.Count; i++)
			{
				if ((bool)m_JointsToBreak[i])
				{
					BasePart component = m_JointsToBreak[i].GetComponent<BasePart>();
					BasePart component2 = m_JointsToBreak[i].connectedBody.GetComponent<BasePart>();
					if ((component == null || ((component.m_enclosedInto == null || component.m_partType != PartType.Kicker) && (component.m_enclosedPart == null || component.m_enclosedPart.m_partType != PartType.Kicker))) && (component2 == null || ((component2.m_enclosedInto == null || component2.m_partType != PartType.Kicker) && (component2.m_enclosedPart == null || component2.m_enclosedPart.m_partType != PartType.Kicker))))
					{
						UnityEngine.Object.Destroy(m_JointsToBreak[i]);
						if (component != null)
						{
							component.HandleJointBreak(playEffects: false);
						}
						if (component != null && component.m_enclosedInto == null && component != this)
						{
							m_connectedParts.Add(component);
							m_isOverDistance.Add(IsPartOverDistance(component));
						}
						if (component2 != null && component2.m_enclosedInto == null && component2 != this)
						{
							m_connectedParts.Add(component2);
							m_isOverDistance.Add(IsPartOverDistance(component2));
						}
					}
				}
				m_JointsToBreak[i] = null;
			}
		}
		else
		{
			if (m_isTriggered)
			{
				return;
			}
			m_isTriggered = true;
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.kickerDetach, base.transform.position);
			base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), -1);
			for (int j = 0; j < m_JointsToBreak.Count; j++)
			{
				if ((bool)m_JointsToBreak[j])
				{
					UnityEngine.Object.Destroy(m_JointsToBreak[j]);
					BasePart component3 = m_JointsToBreak[j].gameObject.GetComponent<BasePart>();
					if ((bool)component3)
					{
						component3.HandleJointBreak(playEffects: false);
					}
				}
				m_JointsToBreak[j] = null;
			}
			m_JointsToBreak.Clear();
		}
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	private void FixedUpdate()
	{
		if (INSettings.GetBool(INFeature.AutoConnector) && this.IsAutoConnector())
		{
			float @float = INSettings.GetFloat(INFeature.AutoConnectorCoolingTime);
			if (m_enabled && Time.time > m_connectTime + @float)
			{
				bool flag = false;
				for (int num = m_connectedJoints.Count - 1; num >= 0; num--)
				{
					(Rigidbody, Joint) tuple2 = m_connectedJoints[num];
					if (tuple2.Item1 == null || tuple2.Item2 == null)
					{
						if (tuple2.Item2 != null)
						{
							UnityEngine.Object.Destroy(tuple2.Item2);
							flag = true;
						}
						m_connectedJoints.RemoveAt(num);
					}
				}
				Vector3 position = base.transform.position;
				float num2 = 1.44f;
				float jointConnectionStrength = Contraption.Instance.GetJointConnectionStrength(GetJointConnectionStrength());
				Rigidbody rigidbody2 = base.rigidbody;
				int @int = INSettings.GetInt(INFeature.AutoConnectorMaxConnectionCount);
				Rigidbody[] components = INContraption.Instance.GetComponents<Rigidbody>();
				foreach (Rigidbody rigidbody in components)
				{
					if (m_connectedJoints.Count >= @int)
					{
						break;
					}
					Vector3 position2 = rigidbody.position;
					if ((position2.x - position.x) * (position2.x - position.x) + (position2.y - position.y) * (position2.y - position.y) > num2 || rigidbody == rigidbody2)
					{
						continue;
					}
					Predicate<(Rigidbody, Joint)> match = ((Rigidbody, Joint) tuple) => tuple.Item1 == rigidbody && tuple.Item2 != null;
					if (m_originalConnectedJoints.Exists(match) || m_connectedJoints.Exists(match))
					{
						continue;
					}
					BasePart component = rigidbody.GetComponent<BasePart>();
					if (component == null || component.m_partType != PartType.Rope || !rigidbody.isKinematic)
					{
						flag = true;
						float num3 = ((component != null) ? Contraption.Instance.GetJointConnectionStrength(component.GetJointConnectionStrength()) : 0f);
						Joint joint = base.gameObject.AddComponent<FixedJoint>();
						joint.connectedBody = rigidbody;
						if (Contraption.Instance.HasSuperGlue)
						{
							joint.breakForce = float.PositiveInfinity;
						}
						else
						{
							joint.breakForce = (jointConnectionStrength + num3) * INSettings.GetFloat(INFeature.ConnectionStrength);
						}
						joint.enablePreprocessing = false;
						m_connectedJoints.Add((rigidbody, joint));
						m_connectTime = Time.time;
						break;
					}
				}
				if (flag)
				{
					Contraption.Instance.UpdateConnectedComponents();
				}
			}
		}
		if (!INSettings.GetBool(INFeature.SeparatorConnection) || !this.IsElasticConnector() || !m_enabled)
		{
			return;
		}
		for (int j = 0; j < m_connectedParts.Count; j++)
		{
			BasePart part = m_connectedParts[j];
			bool flag2 = IsPartOverDistance(part);
			if (m_isOverDistance[j] != flag2)
			{
				base.contraption.UpdateConnectedComponents();
			}
			m_isOverDistance[j] = flag2;
		}
		float float2 = INSettings.GetFloat(INFeature.SeparatorConnectionInnerDistance);
		float float3 = INSettings.GetFloat(INFeature.SeparatorConnectionOuterDistance);
		if (customPartIndex == 2)
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			float mass = base.rigidbody.mass;
			Vector3 position3 = base.rigidbody.position;
			Vector3 velocity = base.rigidbody.velocity;
			for (int k = 0; k < m_connectedParts.Count; k++)
			{
				BasePart basePart = m_connectedParts[k];
				if (basePart != null && m_isOverDistance[k])
				{
					Vector3 position4 = basePart.rigidbody.position;
					float num4 = position3.x - position4.x;
					float num5 = position3.y - position4.y;
					float num6 = Vector.Distance(position3, position4);
					if (num6 > 1E-05f)
					{
						Vector3 velocity2 = basePart.rigidbody.velocity;
						float num7 = velocity.x - velocity2.x;
						float num8 = velocity.y - velocity2.y;
						float mass2 = basePart.rigidbody.mass;
						float num9 = mass2 / (mass + mass2);
						float num10 = mass / (mass + mass2);
						float num11 = ((num6 > float2) ? ((float3 - num6) / (float3 - float2)) : 1f);
						float num12 = (num4 * num7 + num5 * num8) / num6;
						num12 *= num11;
						num12 = ((num12 > 20f) ? 20f : ((num12 < -20f) ? (-20f) : num12)) / num6;
						velocity -= new Vector3(num9 * num12 * num4, num9 * num12 * num5);
						basePart.rigidbody.velocity += new Vector3(num10 * num12 * num4, num10 * num12 * num5);
						float num13 = num6 - 1f;
						num13 *= num11;
						num13 = ((num13 > 0.4f) ? 0.4f : ((num13 < -0.4f) ? (-0.4f) : num13)) / num6;
						velocity -= new Vector3(num9 * num13 * num4 / fixedDeltaTime, num9 * num13 * num5 / fixedDeltaTime);
						basePart.rigidbody.velocity += new Vector3(num10 * num13 * num4 / fixedDeltaTime, num10 * num13 * num5 / fixedDeltaTime);
					}
				}
			}
			base.rigidbody.velocity = velocity;
		}
		else
		{
			if (customPartIndex != 4)
			{
				return;
			}
			float deltaTime = Time.deltaTime;
			float mass3 = base.rigidbody.mass;
			Vector3 position5 = base.rigidbody.position;
			Vector3 velocity3 = base.rigidbody.velocity;
			for (int l = 0; l < m_connectedParts.Count; l++)
			{
				BasePart basePart2 = m_connectedParts[l];
				if (basePart2 != null && m_isOverDistance[l])
				{
					Vector3 position6 = basePart2.rigidbody.position;
					Vector3 velocity4 = basePart2.rigidbody.velocity;
					float num14 = Vector.Distance(position5, position6);
					float num15 = ((num14 > float2) ? ((float3 - num14) / (float3 - float2)) : 1f);
					float num16 = 32f * (velocity3.x - velocity4.x) + 128f * (position5.x - position6.x);
					float num17 = 32f * (velocity3.y - velocity4.y) + 128f * (position5.y - position6.y);
					num16 *= num15;
					num17 *= num15;
					float num18 = Mathf.Sqrt(num16 * num16 + num17 * num17);
					float mass4 = basePart2.rigidbody.mass;
					float num19 = ((num18 * mass4 > 250f) ? (deltaTime * 250f / (num18 * mass4)) : deltaTime);
					float num20 = num19 * mass4 / mass3;
					velocity3 -= new Vector3(num20 * num16, num20 * num17);
					basePart2.rigidbody.velocity += new Vector3(num19 * num16, num19 * num17);
				}
			}
			base.rigidbody.velocity = velocity3;
		}
	}

	public IEnumerable<BasePart> GetConnectedParts()
	{
		if (customPartIndex == 2 || customPartIndex == 4 || customPartIndex == 3)
		{
			if (m_connectedParts == null)
			{
				return Enumerable.Empty<BasePart>();
			}
			if (INSettings.GetBool(INFeature.PersistentSeparatorConnection))
			{
				return m_connectedParts;
			}
			if (!m_enabled)
			{
				return Enumerable.Empty<BasePart>();
			}
			return GetConnectedPartsYield();
		}
		if (customPartIndex == 1)
		{
			if (!m_enabled)
			{
				return Enumerable.Empty<BasePart>();
			}
			return GetConnectedPartsYield();
		}
		return Enumerable.Empty<BasePart>();
	}

	private IEnumerable<BasePart> GetConnectedPartsYield()
	{
		if (this.IsElasticConnector())
		{
			for (int i = 0; i < m_connectedParts.Count; i++)
			{
				if (m_isOverDistance[i])
				{
					yield return m_connectedParts[i];
				}
			}
		}
		else
		{
			if (!this.IsAutoConnector())
			{
				yield break;
			}
			foreach (var connectedJoint in m_connectedJoints)
			{
				if (connectedJoint.Item1 != null)
				{
					BasePart component = connectedJoint.Item1.GetComponent<BasePart>();
					if (component != null)
					{
						yield return component;
					}
				}
			}
		}
	}

	private bool IsPartOverDistance(BasePart part)
	{
		float @float = INSettings.GetFloat(INFeature.SeparatorConnectionOuterDistance);
		Vector3 position = base.transform.position;
		Vector3 position2 = part.transform.position;
		return (position.x - position2.x) * (position.x - position2.x) + (position.y - position2.y) * (position.y - position2.y) < @float * @float;
	}
}
