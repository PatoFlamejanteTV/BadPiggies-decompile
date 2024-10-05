using System.Collections;
using UnityEngine;

public class UnlockSandboxSequence : MonoBehaviour
{
	public CompactEpisodeSelector episodeSelector;

	private SandboxSkullLevelButton sandboxSkullLevelButton;

	private ButtonLock buttonLock;

	private string buttonUnlockKey = string.Empty;

	private void Awake()
	{
		episodeSelector = CompactEpisodeSelector.Instance;
		if (buttonLock == null)
		{
			buttonLock = GetComponentInChildren<ButtonLock>();
		}
		SandboxLevelButton component = GetComponent<SandboxLevelButton>();
		if (component != null)
		{
			if (!GameProgress.GetSandboxUnlocked(component.m_sandboxIdentifier))
			{
				GameProgress.SetSandboxUnlocked(component.m_sandboxIdentifier, unlocked: true);
			}
			buttonUnlockKey = $"SandboxLevelButton_{component.m_sandboxIdentifier}";
			if (GameProgress.GetFullVersionUnlocked() || GameProgress.GetSandboxUnlocked(component.m_sandboxIdentifier) || (component.m_sandboxIdentifier.Equals("S-F") && GameProgress.GetSandboxUnlocked("S-F")))
			{
				if (GameProgress.GetButtonUnlockState(buttonUnlockKey) == GameProgress.ButtonUnlockState.Locked)
				{
					episodeSelector.StartCoroutine(UnlockSequence());
				}
				return;
			}
			GameProgress.SetButtonUnlockState(buttonUnlockKey, GameProgress.ButtonUnlockState.Locked);
			Transform transform = base.transform.Find("StarSet");
			if (transform != null)
			{
				transform.gameObject.SetActive(value: false);
			}
			return;
		}
		SandboxSkullLevelButton component2 = GetComponent<SandboxSkullLevelButton>();
		if (!(component2 != null))
		{
			return;
		}
		if (!GameProgress.GetSandboxUnlocked(component2.m_sandboxIdentifier))
		{
			GameProgress.SetSandboxUnlocked(component2.m_sandboxIdentifier, unlocked: true);
		}
		buttonUnlockKey = $"SandboxLevelButton_{component2.m_sandboxIdentifier}";
		if (!GameProgress.GetFullVersionUnlocked() && !GameProgress.GetSandboxUnlocked(component2.m_sandboxIdentifier))
		{
			return;
		}
		if (GameProgress.GetButtonUnlockState(buttonUnlockKey) == GameProgress.ButtonUnlockState.Locked)
		{
			episodeSelector.StartCoroutine(UnlockSequence());
			return;
		}
		GameProgress.SetButtonUnlockState(buttonUnlockKey, GameProgress.ButtonUnlockState.Locked);
		Transform transform2 = base.transform.Find("StarSet");
		if (transform2 != null)
		{
			transform2.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator UnlockSequence()
	{
		while (!episodeSelector.IsRotated)
		{
			yield return null;
		}
		if ((bool)GetComponent<EpisodeButton>())
		{
			episodeSelector.MoveToTarget(base.transform);
		}
		else
		{
			episodeSelector.MoveToTarget(base.transform.parent);
		}
		yield return new WaitForSeconds(0.5f);
		while (!episodeSelector.IsRotated)
		{
			yield return null;
		}
		GameProgress.UnlockButton(buttonUnlockKey);
		if (buttonLock != null)
		{
			buttonLock.NotifyUnlocked();
		}
	}
}
