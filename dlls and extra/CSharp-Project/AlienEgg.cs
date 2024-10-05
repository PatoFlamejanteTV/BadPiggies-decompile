using System.Collections.Generic;
using UnityEngine;

public class AlienEgg : Egg
{
	private bool m_alienEnabled;

	private List<BasePart> m_parts;

	private List<KeyValuePair<Joint, float>> m_joints;

	public override void Initialize()
	{
		if (!INSettings.GetBool(INFeature.NewAlienEgg) && !base.contraption.HasRegularGlue)
		{
			base.contraption.ApplySuperGlue(Glue.Type.Alien);
			base.contraption.MakeUnbreakable();
		}
	}

	public override bool IsEnabled()
	{
		return m_alienEnabled;
	}

	protected override void OnTouch()
	{
		if (!INSettings.GetBool(INFeature.NewAlienEgg))
		{
			return;
		}
		m_alienEnabled = !m_alienEnabled;
		if (!m_alienEnabled)
		{
			Glue.RemoveSuperGlue(m_parts);
			foreach (KeyValuePair<Joint, float> joint2 in m_joints)
			{
				Joint key = joint2.Key;
				if (key != null)
				{
					key.breakForce = joint2.Value;
				}
			}
			m_parts.Clear();
			m_joints.Clear();
			return;
		}
		Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.SuperGlueApplied);
		foreach (BasePart part in base.contraption.Parts)
		{
			if ((part.m_partType == PartType.WoodenFrame || part.m_partType == PartType.MetalFrame) && part.ConnectedComponent == base.ConnectedComponent)
			{
				m_parts.Add(part);
			}
		}
		Glue.ShowSuperGlue(base.contraption, Glue.Type.Alien, m_parts);
		Joint[] components = INContraption.Instance.GetComponents<Joint>();
		foreach (Joint joint in components)
		{
			BasePart component = joint.GetComponent<BasePart>();
			if (component != null && component.ConnectedComponent == base.ConnectedComponent)
			{
				m_joints.Add(new KeyValuePair<Joint, float>(joint, joint.breakForce));
				joint.breakForce = float.PositiveInfinity;
			}
		}
	}

	public override void PostInitialize()
	{
		if (INSettings.GetBool(INFeature.NewAlienEgg))
		{
			base.contraption.ChangeOneShotPartAmount(m_partType, EffectDirection(), 1);
			m_parts = new List<BasePart>();
			m_joints = new List<KeyValuePair<Joint, float>>();
			OnTouch();
		}
	}
}
