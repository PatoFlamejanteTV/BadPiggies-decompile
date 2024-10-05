using UnityEngine;

public class Frame : BasePart
{
	public enum FrameMaterial
	{
		Wood,
		Metal
	}

	public FrameMaterial m_material;

	public Texture2D[] m_brokenTextures;

	private bool m_colored;

	private MeshRenderer[] m_renderers;

	public override bool CanEncloseParts()
	{
		return true;
	}

	public override bool IsPartOfChassis()
	{
		return true;
	}

	public override void Initialize()
	{
		if ((bool)m_enclosedPart && m_enclosedPart.m_partType != PartType.Rope)
		{
			FixedJoint fixedJoint = m_enclosedPart.gameObject.AddComponent<FixedJoint>();
			fixedJoint.connectedBody = base.rigidbody;
			float breakForce = base.contraption.GetJointConnectionStrength(GetJointConnectionStrength()) + base.contraption.GetJointConnectionStrength(m_enclosedPart.GetJointConnectionStrength());
			fixedJoint.breakForce = breakForce;
			fixedJoint.enablePreprocessing = false;
			base.contraption.AddJointToMap(this, m_enclosedPart, fixedJoint);
			IgnoreCollisionRecursive(base.collider, m_enclosedPart.gameObject);
		}
	}

	private void IgnoreCollisionRecursive(Collider collider, GameObject part)
	{
		if (part.activeInHierarchy && (bool)part.GetComponent<Collider>())
		{
			Physics.IgnoreCollision(collider, part.GetComponent<Collider>());
		}
		for (int i = 0; i < part.transform.childCount; i++)
		{
			IgnoreCollisionRecursive(collider, part.transform.GetChild(i).gameObject);
		}
	}

	public override void OnBreak()
	{
	}

	private new void Awake()
	{
		base.Awake();
		m_renderers = GetComponentsInChildren<MeshRenderer>();
	}

	private void FixedUpdate()
	{
		if (base.contraption == null || !INSettings.GetBool(INFeature.ColoredFrame) || ((!INSettings.GetBool(INFeature.CanColorSpecialFrames) || (!this.IsAlienMetalFrame() && !this.IsLightFrame())) && !INSettings.GetBool(INFeature.CanColorAllFrames)))
		{
			return;
		}
		int num = 1;
		int num2 = 0;
		Color clear = Color.clear;
		float num3 = 0f;
		for (int i = 0; i < 4; i++)
		{
			BasePart basePart = base.contraption.FindPartAt(m_coordX + num, m_coordY + num2);
			if (basePart != null && basePart.IsColoredrame())
			{
				ColoredFrame coloredFrame = basePart as ColoredFrame;
				clear += coloredFrame.Color * coloredFrame.Color.a;
				num3 += coloredFrame.Color.a;
			}
			int num4 = num;
			num = -num2;
			num2 = num4;
		}
		if (num3 > 0f)
		{
			clear /= num3;
			MeshRenderer[] renderers = m_renderers;
			foreach (MeshRenderer meshRenderer in renderers)
			{
				if (!m_colored)
				{
					meshRenderer.material.shader = INUnity.UnlitColorTransparentGrayOverlayShader;
				}
				meshRenderer.material.color = clear;
			}
			m_colored = true;
		}
		else if (m_colored)
		{
			MeshRenderer[] renderers = m_renderers;
			foreach (MeshRenderer obj in renderers)
			{
				obj.material.shader = INUnity.UnlitColorTransparentShader;
				obj.material.color = Color.white;
			}
			m_colored = false;
		}
	}
}
