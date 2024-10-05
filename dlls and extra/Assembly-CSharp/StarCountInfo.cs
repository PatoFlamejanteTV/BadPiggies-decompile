using UnityEngine;

public class StarCountInfo : MonoBehaviour
{
	private TextMesh label;

	private void OnShowButton()
	{
		if (label == null)
		{
			label = base.transform.GetComponentInChildren<TextMesh>();
		}
		UpdateCount();
	}

	public void UpdateCount()
	{
		if (!(label == null))
		{
			int num = 0;
			if (GameProgress.Initialized)
			{
				num = GameProgress.GetAllStars();
			}
			label.text = $"{num}";
		}
	}
}
