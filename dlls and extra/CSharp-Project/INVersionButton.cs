using UnityEngine;
using UnityEngine.UI;

public class INVersionButton : UnityEngine.UI.Button
{
	[SerializeField]
	private int m_type;

	private INVersionSelector m_versionSelector;

	public int Type => m_type;

	public bool IsEnabled
	{
		get
		{
			SelectionState selectionState = base.currentSelectionState;
			if (selectionState != SelectionState.Pressed)
			{
				return selectionState == SelectionState.Selected;
			}
			return true;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		ColorBlock colorBlock = base.colors;
		colorBlock.normalColor = new Color(1f, 1f, 1f, 0.4f);
		colorBlock.highlightedColor = new Color(1f, 1f, 1f, 0.8f);
		colorBlock.pressedColor = new Color(1f, 1f, 1f, 0.6f);
		colorBlock.selectedColor = new Color(1f, 1f, 1f, 1f);
		base.colors = colorBlock;
		base.onClick.AddListener(OnClick);
		m_versionSelector = base.transform.root.GetComponent<INVersionSelector>();
	}

	private void OnClick()
	{
		if (m_type == -1)
		{
			m_versionSelector.EnterVersion();
		}
		else
		{
			m_versionSelector.SelectVersion(m_type);
		}
	}
}
