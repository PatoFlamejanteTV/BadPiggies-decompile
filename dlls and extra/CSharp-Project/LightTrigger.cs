using UnityEngine;

public class LightTrigger : MonoBehaviour
{
	private PointLightSource pls;

	private Transform cachedTransform;

	public PointLightSource LightSource => pls;

	public void Init(PointLightSource pls)
	{
		this.pls = pls;
	}

	private void Start()
	{
		cachedTransform = base.transform;
	}

	private void Update()
	{
		cachedTransform.position += Vector3.zero;
	}

	private void OnTriggerEnter(Collider c)
	{
		if (!(pls == null) && pls.lightType == PointLightMask.LightType.PointLight && pls.canLitObjects && pls.isEnabled)
		{
			c.SendMessageUpwards("Lit", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerStay(Collider c)
	{
		if (pls == null)
		{
			return;
		}
		if (pls.lightType == PointLightMask.LightType.PointLight && pls.canLitObjects && pls.isEnabled)
		{
			c.SendMessage("Lit", SendMessageOptions.DontRequireReceiver);
		}
		else if (pls.lightType == PointLightMask.LightType.BeamLight && pls.canLitObjects && pls.isEnabled)
		{
			float beamAngle = pls.beamAngle;
			Vector3 vector = Vector3.up * c.transform.position.y + Vector3.right * c.transform.position.x;
			Vector3 vector2 = Vector3.up * base.transform.position.y + Vector3.right * base.transform.position.x;
			float num = Vector3.Angle(vector - vector2, pls.transform.up);
			if (Vector3.Distance(vector, vector2) <= pls.baseLightSize + pls.borderWidth || num < beamAngle * 0.5f)
			{
				c.SendMessageUpwards("Lit", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void Lit()
	{
		if (pls != null && !pls.isEnabled && pls.canBeLit)
		{
			pls.isEnabled = true;
		}
	}
}
