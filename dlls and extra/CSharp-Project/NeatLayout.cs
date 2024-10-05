using UnityEngine;

[ExecuteInEditMode]
public class NeatLayout : MonoBehaviour
{
	public enum Align
	{
		Left,
		Right,
		Middle
	}

	[SerializeField]
	private Align align;

	[SerializeField]
	private float horizontalGap;

	[SerializeField]
	private float verticalGap;

	[SerializeField]
	private int rowItems = 4;

	[SerializeField]
	private bool evenDistribution;

	private int TotalRows => ActiveChildCount / (RowMax + 1) + 1;

	private int RowMax
	{
		get
		{
			if (!evenDistribution)
			{
				return rowItems;
			}
			if (ActiveChildCount < 2 * rowItems - 2)
			{
				return Mathf.CeilToInt((float)ActiveChildCount / 2f);
			}
			return rowItems;
		}
	}

	private int ActiveChildCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				if (!base.transform.GetChild(i).gameObject.activeSelf)
				{
					continue;
				}
				MeshRenderer[] componentsInChildren = base.transform.GetChild(i).GetComponentsInChildren<MeshRenderer>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					if (componentsInChildren[j].enabled)
					{
						num++;
						break;
					}
				}
			}
			return num;
		}
	}

	private int ObjectsInRow(int row)
	{
		return Mathf.Clamp(ActiveChildCount - row * RowMax, 0, RowMax);
	}

	private int RowIndex(int index)
	{
		return index % RowMax;
	}

	private bool IsActive(Transform child)
	{
		MeshRenderer[] componentsInChildren = child.GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].enabled)
			{
				return true;
			}
		}
		return false;
	}

	private void Start()
	{
		OrganizeChildren();
	}

	private void OnEnable()
	{
		OrganizeChildren();
	}

	private void OrganizeChildren()
	{
		int num = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (IsActive(child) && child.gameObject.activeInHierarchy)
			{
				Vector2 vector = new Vector2(horizontalGap, verticalGap);
				int num2 = num / RowMax;
				switch (align)
				{
				case Align.Left:
					vector.y = verticalGap * (float)num2;
					vector.x = horizontalGap * (float)num;
					break;
				case Align.Right:
					vector.y = verticalGap * (float)num2;
					vector.x = (0f - horizontalGap) * (float)num;
					break;
				case Align.Middle:
					vector.y *= 0f - ((float)num2 - (float)TotalRows / 2f);
					vector.x *= (float)RowIndex(num) - (float)ObjectsInRow(num2) / 2f;
					vector.y -= verticalGap / 2f;
					vector.x += horizontalGap / 2f;
					break;
				}
				child.localPosition = vector;
				num++;
			}
		}
	}
}
