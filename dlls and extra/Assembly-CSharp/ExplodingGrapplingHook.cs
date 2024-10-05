using System;
using UnityEngine;

public class ExplodingGrapplingHook : BasePart
{
	[SerializeField]
	private GameObject m_projectilePrefab;

	[SerializeField]
	private ParticleSystem m_particleEffect;

	private bool m_enabled;

	private GameObject m_leftAttachment;

	private GameObject m_rightAttachment;

	private GameObject m_topAttachment;

	private GameObject m_bottomAttachment;

	private GameObject m_bottomLeftAttachment;

	private GameObject m_bottomRightAttachment;

	private GameObject m_topLeftAttachment;

	private GameObject m_topRightAttachment;

	private ExplodingGrapplingHookProjectile currentProjectile;

	private float m_shootTime;

	protected float CoolingTime { get; set; }

	protected float ForcedCoolingTime { get; set; }

	public override bool CanBeEnabled()
	{
		return true;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	public override Direction EffectDirection()
	{
		return BasePart.RotateWithEightDirections(Direction.Right, m_gridRotation);
	}

	public override void Awake()
	{
		base.Awake();
		m_leftAttachment = base.transform.Find("LeftAttachment").gameObject;
		m_rightAttachment = base.transform.Find("RightAttachment").gameObject;
		m_topAttachment = base.transform.Find("TopAttachment").gameObject;
		m_bottomAttachment = base.transform.Find("BottomAttachment").gameObject;
		m_bottomLeftAttachment = base.transform.Find("BottomLeftAttachment").gameObject;
		m_bottomRightAttachment = base.transform.Find("BottomRightAttachment").gameObject;
		m_topLeftAttachment = base.transform.Find("TopLeftAttachment").gameObject;
		m_topRightAttachment = base.transform.Find("TopRightAttachment").gameObject;
		m_leftAttachment.SetActive(value: false);
		m_rightAttachment.SetActive(value: false);
		m_topAttachment.SetActive(value: false);
		m_bottomAttachment.SetActive(value: false);
		m_bottomLeftAttachment.SetActive(value: false);
		m_bottomRightAttachment.SetActive(value: false);
		m_topLeftAttachment.SetActive(value: false);
		m_topRightAttachment.SetActive(value: false);
		m_eightWay = true;
	}

	public override void ChangeVisualConnections()
	{
		if (INSettings.GetBool(INFeature.EnclosableParts) && m_enclosedInto != null)
		{
			m_leftAttachment.SetActive(value: false);
			m_rightAttachment.SetActive(value: false);
			m_topAttachment.SetActive(value: false);
			m_bottomAttachment.SetActive(value: false);
			m_bottomLeftAttachment.SetActive(value: false);
			m_bottomRightAttachment.SetActive(value: false);
			m_topLeftAttachment.SetActive(value: false);
			m_topRightAttachment.SetActive(value: false);
			return;
		}
		bool flag = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Up, m_gridRotation));
		bool flag2 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Down, m_gridRotation));
		bool flag3 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Left, m_gridRotation));
		bool flag4 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.Right, m_gridRotation));
		bool flag5 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.UpLeft, m_gridRotation));
		bool flag6 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.DownLeft, m_gridRotation));
		bool flag7 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.UpRight, m_gridRotation));
		bool flag8 = base.contraption.CanConnectTo(this, BasePart.Rotate(Direction.DownRight, m_gridRotation));
		bool flag9 = m_gridRotation == GridRotation.Deg_135 || m_gridRotation == GridRotation.Deg_45 || m_gridRotation == GridRotation.Deg_225 || m_gridRotation == GridRotation.Deg_315;
		m_leftAttachment.SetActive(flag3 && !flag9);
		m_rightAttachment.SetActive(flag4 && !flag9);
		m_topAttachment.SetActive(flag && !flag9);
		m_bottomAttachment.SetActive((flag2 && !flag9) || (!flag && !flag3 && !flag4 && !flag9));
		m_bottomLeftAttachment.SetActive(flag6 && flag9);
		m_bottomRightAttachment.SetActive(flag8 && flag9);
		m_topLeftAttachment.SetActive(flag5 && flag9);
		m_topRightAttachment.SetActive(flag7 && flag9);
		if (!flag && !flag6 && !flag3 && !flag4 && !flag5 && !flag6 && !flag7 && !flag8)
		{
			m_bottomAttachment.SetActive(value: true);
		}
	}

	protected override void OnTouch()
	{
		Shoot();
	}

	protected void Shoot()
	{
		if (!m_enabled && !(Time.time - m_shootTime < INSettings.GetFloat(INFeature.GunProjectileForcedCoolingTime)))
		{
			m_shootTime = Time.time;
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.alienLaserFire, base.transform);
			m_enabled = true;
			currentProjectile = UnityEngine.Object.Instantiate(m_projectilePrefab).GetComponent<ExplodingGrapplingHookProjectile>();
			m_particleEffect.Play();
			currentProjectile.transform.parent = base.transform;
			currentProjectile.transform.localPosition = Vector3.forward * 0.1f;
			currentProjectile.transform.rotation = base.transform.rotation;
			if (INSettings.GetBool(INFeature.InertialGunProjectile))
			{
				Vector3 right = base.transform.right;
				currentProjectile.transform.position = base.rigidbody.position + right;
				currentProjectile.rigidbody.position = base.rigidbody.position + right;
				currentProjectile.rigidbody.velocity = base.rigidbody.velocity;
				currentProjectile.collider.material.dynamicFriction = 0f;
			}
			currentProjectile.rigidbody.drag = INSettings.GetFloat(INFeature.GunProjectileDrag);
			Physics.IgnoreCollision(currentProjectile.GetComponentInChildren<Collider>(), base.gameObject.GetComponentInChildren<Collider>());
			currentProjectile.OnExplosion = (Action)Delegate.Combine(currentProjectile.OnExplosion, (Action)delegate
			{
				m_enabled = false;
			});
		}
	}
}
