using UnityEngine;

public class CheatsToggle : MonoBehaviour
{
	[SerializeField]
	private bool startState;

	[SerializeField]
	private GameObject[] cheats;

	private bool toggleState;

	private void Start()
	{
		toggleState = startState;
		for (int i = 0; i < cheats.Length; i++)
		{
			cheats[i].SetActive(startState);
		}
	}

	public void Toggle()
	{
		toggleState = !toggleState;
		for (int i = 0; i < cheats.Length; i++)
		{
			cheats[i].SetActive(toggleState);
		}
	}
}
