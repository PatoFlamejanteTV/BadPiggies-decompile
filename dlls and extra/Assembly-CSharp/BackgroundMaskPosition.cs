using UnityEngine;

public class BackgroundMaskPosition : MonoBehaviour
{
	[SerializeField]
	private string sortingLayerName = "Default";

	private void OnEnable()
	{
		BackgroundMask.Show(show: true, this, sortingLayerName, base.transform, Vector3.zero);
	}

	private void OnDisable()
	{
		BackgroundMask.Show(show: false, this, string.Empty);
	}
}
