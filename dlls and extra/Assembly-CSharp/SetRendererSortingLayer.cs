using UnityEngine;

[ExecuteInEditMode]
public class SetRendererSortingLayer : MonoBehaviour
{
	[SerializeField]
	private string sortingLayerName = string.Empty;

	[SerializeField]
	private int sortingOrder;

	private void OnEnable()
	{
		GetComponent<Renderer>().sortingLayerName = sortingLayerName;
		GetComponent<Renderer>().sortingOrder = sortingOrder;
	}
}
