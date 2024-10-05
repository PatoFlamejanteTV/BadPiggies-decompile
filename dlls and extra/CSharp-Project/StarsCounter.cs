using UnityEngine;

public class StarsCounter : WPFMonoBehaviour
{
	private TextMesh starsCounterTextShadow;

	private TextMesh starsCounterText;

	private string maxStars;

	private void Awake()
	{
		StarBox.onCollected += BoxCollected;
		PartBox.onCollected += BoxCollected;
		starsCounterText = base.transform.Find("AnimationNode/StarsCounter").GetComponent<TextMesh>();
		starsCounterTextShadow = base.transform.Find("AnimationNode/StarsCounter/StarsCounterShadow").GetComponent<TextMesh>();
		if (WPFMonoBehaviour.levelManager.m_sandbox)
		{
			SandboxLevels.LevelData levelData = WPFMonoBehaviour.gameData.m_sandboxLevels.GetLevelData(Singleton<GameManager>.Instance.CurrentLevelIdentifier);
			maxStars = ((levelData == null) ? "20" : levelData.m_starBoxCount.ToString());
		}
		UpdateBoxCount();
	}

	private void OnDestroy()
	{
		StarBox.onCollected -= BoxCollected;
		PartBox.onCollected -= BoxCollected;
	}

	private void UpdateBoxCount()
	{
		int num = GameProgress.SandboxStarCount(Singleton<GameManager>.Instance.CurrentSceneName);
		starsCounterText.text = ((num >= 10) ? (num + "/" + maxStars) : ("0" + num + "/" + maxStars));
		starsCounterTextShadow.text = starsCounterText.text;
	}

	private void BoxCollected()
	{
		UpdateBoxCount();
		base.transform.Find("AnimationNode").GetComponent<Animation>().Play();
		base.transform.Find("StarburstEffect").GetComponent<ParticleSystem>().Play();
	}

	private void OnEnable()
	{
		base.gameObject.SetActive(WPFMonoBehaviour.levelManager.m_sandbox && !INSettings.GetBool(INFeature.HideStarsCounter));
	}
}
