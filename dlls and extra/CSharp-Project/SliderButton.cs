using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class SliderButton : MonoBehaviour
{
	private struct OriginalTransform
	{
		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;
	}

	[SerializeField]
	private List<string> ButtonOrder = new List<string>();

	private Dictionary<string, OriginalTransform> dicOriginalButtonsTransforms = new Dictionary<string, OriginalTransform>();

	protected virtual void Start()
	{
		SetInitialValues();
	}

	public void SetInitialValues()
	{
		int num = 0;
		OriginalTransform value = default(OriginalTransform);
		foreach (string item in ButtonOrder)
		{
			Transform transform = base.transform.FindChildRecursively(item);
			if (!(transform == null) && (!(transform.GetComponent<Collider>() != null) || transform.GetComponent<Collider>().enabled))
			{
				if (dicOriginalButtonsTransforms.ContainsKey(item))
				{
					transform.localPosition = dicOriginalButtonsTransforms[item].position;
					transform.localRotation = dicOriginalButtonsTransforms[item].rotation;
					transform.localScale = dicOriginalButtonsTransforms[item].scale;
				}
				else
				{
					value.position = transform.localPosition;
					value.rotation = transform.localRotation;
					value.scale = transform.localScale;
					dicOriginalButtonsTransforms.Add(item, value);
				}
				float num2 = Mathf.Sign(transform.transform.position.x);
				transform.transform.localRotation *= Quaternion.AngleAxis(num2 * -180f, Vector3.back);
				transform.transform.localScale = Vector3.one * 0.1f;
				num++;
			}
		}
	}
}
