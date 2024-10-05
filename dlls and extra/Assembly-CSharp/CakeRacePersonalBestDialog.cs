using CakeRace;
using UnityEngine;

public class CakeRacePersonalBestDialog : WPFMonoBehaviour
{
	[SerializeField]
	private GameObject[] cakes;

	[SerializeField]
	private TextMesh score;

	[SerializeField]
	private GameObject kingsFavorite;

	private void Start()
	{
		if (!(WPFMonoBehaviour.levelManager.CurrentGameMode is CakeRaceMode))
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		if (WPFMonoBehaviour.levelManager.CurrentGameMode is CakeRaceMode)
		{
			SetScore((WPFMonoBehaviour.levelManager.CurrentGameMode as CakeRaceMode).PersonalBest());
		}
	}

	private void SetScore(CakeRaceReplay replay)
	{
		if (replay == null)
		{
			for (int i = 0; i < cakes.Length; i++)
			{
				cakes[i].SetActive(value: false);
			}
			score.text = "-";
			kingsFavorite.SetActive(value: false);
		}
		else
		{
			for (int j = 0; j < cakes.Length; j++)
			{
				cakes[j].SetActive(replay.CakesCollected > j);
			}
			score.text = $"{CakeRaceReplay.TotalScore(replay):N0}";
			kingsFavorite.SetActive(replay.HasKingsFavoritePart);
		}
	}

	private void OnUIEvent(UIEvent data)
	{
		if (WPFMonoBehaviour.levelManager.CurrentGameMode is CakeRaceMode)
		{
			switch (data.type)
			{
			case UIEvent.Type.Building:
			case UIEvent.Type.Play:
			case UIEvent.Type.Pause:
				base.gameObject.SetActive(value: false);
				break;
			case UIEvent.Type.Preview:
				SetScore((WPFMonoBehaviour.levelManager.CurrentGameMode as CakeRaceMode).PersonalBest());
				base.gameObject.SetActive(value: true);
				break;
			}
		}
	}
}
