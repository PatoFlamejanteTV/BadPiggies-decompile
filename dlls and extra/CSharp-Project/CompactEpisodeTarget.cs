using UnityEngine;

public class CompactEpisodeTarget : Widget
{
	[SerializeField]
	private int elementIndex;

	[SerializeField]
	private bool isSandboxSelection;

	public CompactEpisodeSelector episodeSelector;

	protected override void OnActivate()
	{
		if (episodeSelector != null)
		{
			episodeSelector.MoveToTargetIndex(elementIndex, isSandboxSelection);
		}
	}

	public void SetAsLastTarget()
	{
		UserSettings.SetInt(CompactEpisodeSelector.CurrentSandboxEpisodeKey, elementIndex);
	}
}
