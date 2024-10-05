using System.Collections.Generic;
using UnityEngine;

public class ButtonGrid : MonoBehaviour
{
	private enum Action
	{
		Place,
		DrawGizmos
	}

	public GameObject buttonPrefab;

	public int horizontalCount = 5;

	public Vector2 offset = new Vector2(10f, 10f);

	public int count = 20;

	private List<GameObject> buttons = new List<GameObject>();

	public void AddButton(Button button)
	{
		buttons.Add(button.gameObject);
	}

	public void Clear()
	{
		foreach (GameObject button in buttons)
		{
			Object.Destroy(button);
		}
		buttons.Clear();
	}

	private void OnDrawGizmos()
	{
		if ((bool)buttonPrefab)
		{
			PlaceButtons(Action.DrawGizmos);
		}
	}

	private void PlaceButtons(Action action)
	{
		int num = 0;
		Vector3 position = base.transform.position;
		position.x -= 0.5f * ((float)(horizontalCount - 1) * offset.x);
		position.y -= 0.5f * buttonPrefab.GetComponent<Sprite>().Size.y;
		Vector3 vector = position;
		int num2 = count;
		if (action == Action.Place)
		{
			num2 = buttons.Count;
		}
		for (int i = 0; i < num2; i++)
		{
			if (action == Action.Place)
			{
				buttons[i].transform.position = vector;
			}
			else
			{
				Gizmos.DrawWireCube(vector, buttonPrefab.GetComponent<Sprite>().Size);
			}
			vector.x += offset.x;
			num++;
			if (num >= horizontalCount)
			{
				num = 0;
				vector.x = position.x;
				vector.y -= offset.y;
			}
		}
	}
}
