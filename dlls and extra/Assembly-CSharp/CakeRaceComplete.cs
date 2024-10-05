using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;

public class CakeRaceComplete : WPFMonoBehaviour
{
	private const string REFEREE_PIG_FALL = "refereePig_fall";

	private const string REFEREE_PIG_IMPACT = "refereePig_impact";

	private const string REFEREE_PIG_FALL_IMPACT = "refereePig_fall_impact";

	private const string REFEREE_PIG_WHISTLE = "refereePig_whistle";

	private const string REFEREE_PIG_FLAG = "refereePig_flag";

	private const string SAFE_DROP = "safe_drop";

	private const string WIN_TRUMPET = "win_trumpet";

	private const string PILLOW_APPEAR = "you_win_pillow_appear";

	private const string CONFETTI1 = "confetti1";

	private const string CONFETTI2 = "confetti2";

	private const string SCORE_ANTICIPATION = "Anticipation";

	private const string SCORE = "Score";

	[SerializeField]
	private SkeletonAnimation bannerAnimation;

	private MeshRenderer bannerRenderer;

	[SerializeField]
	private string bannerAnimationPlayerWins;

	[SerializeField]
	private string bannerAnimationOpponentWins;

	[SerializeField]
	private TextMesh[] bannerLabel;

	[SerializeField]
	private string bannerWinLocalizationKey;

	[SerializeField]
	private string bannerLoseLocalizationKey;

	[SerializeField]
	private SkeletonAnimation judgeAnimation;

	private MeshRenderer judgeRenderer;

	[SerializeField]
	private string judgeAnimationIntro;

	[SerializeField]
	private string judgeAnimationIdle;

	[SerializeField]
	private string judgeAnimationPlayerWins;

	[SerializeField]
	private string judgeAnimationOpponentWins;

	[SerializeField]
	private string judgeAnimationPlayerWinsIdle;

	[SerializeField]
	private string judgeAnimationOpponentWinsIdle;

	[SerializeField]
	private SkeletonAnimation scoreAnimation;

	private MeshRenderer scoreRenderer;

	[SerializeField]
	private string scoreAnimationPlayerWins;

	[SerializeField]
	private string scoreAnimationOpponentWins;

	[SerializeField]
	private SkeletonAnimation[] hornAnimations;

	[SerializeField]
	private string hornAnimationPlayerWins;

	[SerializeField]
	private GameObject confettiParticlesPrefab;

	[SerializeField]
	private TextMesh[] leftScore;

	[SerializeField]
	private TextMesh[] leftName;

	[SerializeField]
	private TextMesh[] rightScore;

	[SerializeField]
	private TextMesh[] rightName;

	[SerializeField]
	private Transform controlsPanel;

	[SerializeField]
	private GameObject nextOpponentButton;

	[SerializeField]
	private GameObject retryButton;

	[SerializeField]
	private Transform cakeHolder;

	[SerializeField]
	private GameObject pillowSprite;

	private MeshRenderer pillowRenderer;

	[SerializeField]
	private Transform minScore;

	[SerializeField]
	private Transform maxScore;

	[SerializeField]
	private Transform scoreSliderOverride;

	private CakeRaceMode cakeRaceMode;

	private Vector3 originalControlsPanelPosition = Vector3.zero;

	private void Awake()
	{
		originalControlsPanelPosition = controlsPanel.localPosition;
		controlsPanel.localPosition = originalControlsPanelPosition - Vector3.up * 10f;
	}

	private void Start()
	{
		bannerAnimation.state.Event += OnAnimationEvent;
		judgeAnimation.state.Event += OnAnimationEvent;
		scoreAnimation.state.Event += OnAnimationEvent;
	}

	private void OnEnable()
	{
		cakeRaceMode = WPFMonoBehaviour.levelManager.CurrentGameMode as CakeRaceMode;
		if (cakeRaceMode != null)
		{
			bannerRenderer = bannerAnimation.transform.GetComponent<MeshRenderer>();
			judgeRenderer = judgeAnimation.transform.GetComponent<MeshRenderer>();
			scoreRenderer = scoreAnimation.transform.GetComponent<MeshRenderer>();
			pillowRenderer = pillowSprite.GetComponent<MeshRenderer>();
			bannerRenderer.enabled = false;
			judgeRenderer.enabled = false;
			scoreRenderer.enabled = false;
			pillowRenderer.enabled = false;
			if ((bool)retryButton)
			{
				retryButton.SetActive(value: false);
			}
			for (int i = 0; i < hornAnimations.Length; i++)
			{
				hornAnimations[i].gameObject.SetActive(value: false);
			}
			StartCoroutine(CakeRaceEndScreenSequence());
		}
	}

	private void OnDisabled()
	{
		if ((bool)bannerRenderer)
		{
			bannerRenderer.enabled = false;
		}
		hornAnimations[0].state.Event -= OnAnimationEvent;
	}

	private void OnDestroy()
	{
		bannerAnimation.state.Event -= OnAnimationEvent;
		judgeAnimation.state.Event -= OnAnimationEvent;
		scoreAnimation.state.Event -= OnAnimationEvent;
	}

	private void OnAnimationEvent(Spine.AnimationState state, int trackIndex, Spine.Event e)
	{
		AudioSource audioSource = null;
		switch (e.Data.Name)
		{
		case "refereePig_fall":
			audioSource = WPFMonoBehaviour.gameData.commonAudioCollection.refereePigFall;
			break;
		case "refereePig_flag":
			audioSource = WPFMonoBehaviour.gameData.commonAudioCollection.refereePigFlag;
			break;
		case "refereePig_impact":
			audioSource = WPFMonoBehaviour.gameData.commonAudioCollection.refereePigImpact;
			break;
		case "refereePig_whistle":
			audioSource = WPFMonoBehaviour.gameData.commonAudioCollection.refereePigWhistle;
			break;
		case "safe_drop":
			audioSource = WPFMonoBehaviour.gameData.commonAudioCollection.safeDrop;
			break;
		case "you_win_pillow_appear":
			audioSource = WPFMonoBehaviour.gameData.commonAudioCollection.youWinPillowAppear;
			break;
		case "refereePig_fall_impact":
			audioSource = WPFMonoBehaviour.gameData.commonAudioCollection.refereePigFallImpact;
			break;
		case "Anticipation":
			audioSource = WPFMonoBehaviour.gameData.commonAudioCollection.scoreAnticipation[0];
			break;
		case "win_trumpet":
		{
			AudioSource[] winTrumpet = WPFMonoBehaviour.gameData.commonAudioCollection.winTrumpet;
			audioSource = winTrumpet[Random.Range(0, winTrumpet.Length)];
			break;
		}
		case "confetti1":
		{
			GameObject obj2 = Object.Instantiate(confettiParticlesPrefab);
			LayerHelper.SetSortingLayer(obj2, "Popup", children: true);
			LayerHelper.SetLayer(obj2, base.gameObject.layer, children: true);
			obj2.transform.position = WPFMonoBehaviour.hudCamera.ViewportToWorldPoint(new Vector3(1f / 3f, 0f));
			break;
		}
		case "confetti2":
		{
			GameObject obj = Object.Instantiate(confettiParticlesPrefab);
			LayerHelper.SetSortingLayer(obj, "Popup", children: true);
			LayerHelper.SetLayer(obj, base.gameObject.layer, children: true);
			obj.transform.position = WPFMonoBehaviour.hudCamera.ViewportToWorldPoint(new Vector3(2f / 3f, 0f));
			break;
		}
		default:
			audioSource = null;
			break;
		}
		if (audioSource != null)
		{
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(audioSource);
		}
	}

	private IEnumerator CakeRaceEndScreenSequence()
	{
		TextMeshHelper.UpdateTextMeshes(bannerLabel, string.Empty);
		SetScoreLabels(-1, -1);
		SetNameLabels(string.Empty, string.Empty, refreshLocalization: false);
		yield return new WaitForSeconds(0.5f);
		bool winner = cakeRaceMode.LocalPlayerIsWinner;
		judgeAnimation.state.SetAnimation(0, judgeAnimationIntro, loop: false);
		judgeAnimation.state.AddAnimation(0, judgeAnimationIdle, loop: true, 0f);
		yield return null;
		judgeRenderer.enabled = true;
		yield return new WaitForSeconds(1f);
		scoreAnimation.state.SetAnimation(0, (!winner) ? scoreAnimationOpponentWins : scoreAnimationPlayerWins, loop: false);
		scoreAnimation.state.Event += OnScoreAnimationEvent;
		yield return null;
		scoreRenderer.enabled = true;
		yield return new WaitForSeconds(1f);
		judgeAnimation.state.SetAnimation(0, (!winner) ? judgeAnimationOpponentWins : judgeAnimationPlayerWins, loop: false);
		judgeAnimation.state.AddAnimation(0, (!winner) ? judgeAnimationOpponentWinsIdle : judgeAnimationPlayerWinsIdle, loop: true, 0f);
		yield return new WaitForSeconds(2f);
		SetScoreLabels(cakeRaceMode.CurrentScore, cakeRaceMode.OpponentScore);
		SetNameLabels("CAKE_RACE_YOU", CakeRaceMode.OpponentReplay.GetPlayerName(), refreshLocalization: true);
		WPFMonoBehaviour.levelManager.StopAmbient();
		Singleton<AudioManager>.Instance.SpawnLoopingEffect(WPFMonoBehaviour.gameData.commonAudioCollection.starLoops[winner ? 1 : 0], base.transform);
		bannerAnimation.state.SetAnimation(0, (!winner) ? bannerAnimationOpponentWins : bannerAnimationPlayerWins, loop: false);
		yield return null;
		Localizer.LocaleParameters localeParameters = Singleton<Localizer>.Instance.Resolve((!winner) ? bannerLoseLocalizationKey : bannerWinLocalizationKey);
		TextMeshHelper.UpdateTextMeshes(bannerLabel, localeParameters.translation);
		yield return null;
		bannerRenderer.enabled = true;
		if (winner)
		{
			for (int i = 0; i < hornAnimations.Length; i++)
			{
				hornAnimations[i].gameObject.SetActive(value: true);
				hornAnimations[i].state.SetAnimation(0, hornAnimationPlayerWins, loop: false);
			}
			pillowRenderer.enabled = true;
			hornAnimations[0].state.Event += OnAnimationEvent;
			yield return null;
		}
		nextOpponentButton.SetActive(!cakeRaceMode.LocalPlayerIsWinner);
		yield return new WaitForSeconds(0.5f);
		Vector3 fromPosition = controlsPanel.localPosition;
		float fade = 0f;
		while (fade < 1f)
		{
			fade += GameTime.RealTimeDelta * 2f;
			controlsPanel.localPosition = Vector3.Lerp(fromPosition, originalControlsPanelPosition, fade);
			yield return null;
		}
		controlsPanel.localPosition = originalControlsPanelPosition;
		if ((bool)PlayerProgressBar.Instance)
		{
			float burstRate = (float)cakeRaceMode.GainedXP / 0.6f;
			PlayerProgressBar.Instance.AddParticles(scoreAnimation.gameObject, cakeRaceMode.GainedXP, 0f, burstRate);
		}
		SpawnReward();
	}

	private void OnScoreAnimationEvent(Spine.AnimationState state, int trackIndex, Spine.Event e)
	{
		if (!(e.Data.Name != "Score"))
		{
			scoreAnimation.state.Event -= OnScoreAnimationEvent;
			_ = cakeRaceMode.OpponentScore;
			_ = cakeRaceMode.CurrentScore;
			float num = maxScore.position.x - minScore.position.x;
			float num2 = (maxScore.position.x + minScore.position.x) / 2f;
			float num3 = ((cakeRaceMode.OpponentScore <= cakeRaceMode.CurrentScore) ? (1f - (float)cakeRaceMode.OpponentScore / (float)cakeRaceMode.CurrentScore) : (-1f + (float)cakeRaceMode.CurrentScore / (float)cakeRaceMode.OpponentScore));
			Vector3 position = scoreSliderOverride.position;
			position.x = num2 + num / 2f * num3;
			scoreSliderOverride.position = position;
			Singleton<AudioManager>.Instance.Spawn2dOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.scoreImpact);
		}
	}

	private void SpawnReward()
	{
		if (CakeRaceMode.CurrentRewardCrate != LootCrateType.None)
		{
			LayerHelper.SetLayer(LootCrateGraphicSpawner.CreateCrate(CakeRaceMode.CurrentRewardCrate, cakeHolder, new Vector3(0.6f, 0f, -0.2f), Vector3.one * 0.65f, Quaternion.Euler(new Vector3(0f, 0f, -90f))), base.gameObject.layer, children: true);
		}
	}

	private void SetScoreLabels(int score, int opponentScore)
	{
		if (leftScore != null)
		{
			TextMeshHelper.UpdateTextMeshes(leftScore, (score >= 0) ? $"{score:n0}" : string.Empty);
		}
		if (rightScore != null)
		{
			TextMeshHelper.UpdateTextMeshes(rightScore, (opponentScore >= 0) ? $"{opponentScore:n0}" : string.Empty);
		}
	}

	private void SetNameLabels(string left, string right, bool refreshLocalization)
	{
		if (leftName != null)
		{
			TextMeshHelper.UpdateTextMeshes(leftName, left, refreshLocalization);
		}
		if (rightName != null)
		{
			TextMeshHelper.UpdateTextMeshes(rightName, right);
		}
	}
}
