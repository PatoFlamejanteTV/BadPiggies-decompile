using UnityEngine;

namespace CakeRace;

public class Cake : OneTimeCollectable
{
	private static int s_cakeCount;

	private int m_cakeIndex;

	private float m_maxDistance;

	public int CakeIndex => m_cakeIndex;

	public bool CollectedByOtherPlayer { get; private set; }

	public event OnCakeCollectedHandler OnCakeCollected;

	protected override string GetNameKey()
	{
		return $"Cake{m_cakeIndex}";
	}

	private void Awake()
	{
		m_cakeIndex = s_cakeCount++;
		CollectedByOtherPlayer = false;
	}

	protected override void Start()
	{
		base.Start();
		isDynamic = false;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		s_cakeCount--;
	}

	public override void Collect()
	{
		if (WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Running && !collected)
		{
			if ((bool)collectedEffect)
			{
				Object.Instantiate(collectedEffect, base.transform.position, base.transform.rotation);
			}
			Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.bonusBoxCollected);
			collected = true;
			DisableGoal();
			EventManager.Send(default(ObjectiveAchieved));
			OnCollected();
		}
	}

	public override void OnCollected()
	{
		if (this.OnCakeCollected != null)
		{
			this.OnCakeCollected(this);
		}
	}

	public void Reset()
	{
		DisableGoal(disable: false);
	}
}
