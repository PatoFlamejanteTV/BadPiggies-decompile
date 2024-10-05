using UnityEngine;

public class LevelButton : MonoBehaviour
{
	[SerializeField]
	private string levelName = string.Empty;

	public string OnLevelButtonClicked()
	{
		return levelName;
	}

	private void Start()
	{
		if (levelName == string.Empty)
		{
			GetComponent<Renderer>().material.SetColor("_TintColor", Color.black);
			GetComponent<Animation>().enabled = false;
		}
	}
}
