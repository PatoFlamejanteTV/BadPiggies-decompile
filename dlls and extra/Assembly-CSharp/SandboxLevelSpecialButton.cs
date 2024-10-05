using UnityEngine;

public class SandboxLevelSpecialButton : MonoBehaviour
{
	[SerializeField]
	private string m_sandboxIdentifier;

	[SerializeField]
	private SandboxSelector m_sandboxSelector;

	[SerializeField]
	private TextMesh m_starsText;

	private Transform starSet;

	private void Start()
	{
		if (base.transform.parent != null && base.transform.parent.parent != null)
		{
			m_sandboxSelector = base.transform.parent.parent.GetComponent<SandboxSelector>();
		}
		starSet = base.transform.Find("StarSet");
		if (starSet != null)
		{
			starSet.parent = base.transform.parent;
		}
		if (IsUnlocked())
		{
			ShowUnlocked();
		}
		else
		{
			ShowLocked();
		}
	}

	private void OnEnable()
	{
		IapManager.onPurchaseSucceeded += HandleIapManageronPurchaseSucceeded;
	}

	private void OnDisable()
	{
		IapManager.onPurchaseSucceeded -= HandleIapManageronPurchaseSucceeded;
	}

	private bool IsUnlocked()
	{
		if (Singleton<BuildCustomizationLoader>.Instance.IAPEnabled && !Singleton<BuildCustomizationLoader>.Instance.IsOdyssey)
		{
			return GameProgress.GetSandboxUnlocked("S-F");
		}
		return true;
	}

	private void ShowLocked()
	{
		GetComponent<Button>().MethodToCall.SetMethod(m_sandboxSelector, "OpenIAPPopup");
	}

	private void ShowUnlocked()
	{
		GetComponent<Button>().MethodToCall.SetMethod(m_sandboxSelector.gameObject.GetComponent<SandboxSelector>(), "LoadSandboxLevel", m_sandboxIdentifier);
		base.transform.Find("Finger").gameObject.SetActive(value: false);
		string text = WPFMonoBehaviour.gameData.m_sandboxLevels.GetLevelData(m_sandboxIdentifier).m_starBoxCount.ToString();
		m_starsText.text = GameProgress.SandboxStarCount(m_sandboxSelector.FindLevelFile(m_sandboxIdentifier)) + "/" + text;
	}

	private void HandleIapManageronPurchaseSucceeded(IapManager.InAppPurchaseItemType type)
	{
		ShowUnlocked();
	}
}
