using UnityEngine;

public class DessertsCounter : WPFMonoBehaviour
{
	private TextMesh textShadow;

	private TextMesh text;

	private bool needToUpdateCounter = true;

	private bool playEffects;

	private void Start()
	{
		text = base.transform.Find("AnimationNode/DessertsCounter").GetComponent<TextMesh>();
		textShadow = base.transform.Find("AnimationNode/DessertsCounter/DessertsCounterShadow").GetComponent<TextMesh>();
		EventManager.Connect<Dessert.DessertCollectedEvent>(ReceiveDessertCollected);
	}

	private void OnEnable()
	{
		base.gameObject.SetActive(WPFMonoBehaviour.levelManager.m_sandbox && (GameProgress.GetBool("ChiefPigExploded") || INSettings.GetBool(INFeature.EnableDesserts)) && !INSettings.GetBool(INFeature.HideDessertsCounter));
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<Dessert.DessertCollectedEvent>(ReceiveDessertCollected);
	}

	private void ReceiveDessertCollected(Dessert.DessertCollectedEvent eventData)
	{
		playEffects = (needToUpdateCounter = true);
	}

	private void LateUpdate()
	{
		if (needToUpdateCounter)
		{
			needToUpdateCounter = false;
			text.text = WPFMonoBehaviour.levelManager.m_CollectedDessertsCount.ToString();
			textShadow.text = WPFMonoBehaviour.levelManager.m_CollectedDessertsCount.ToString();
		}
		if (playEffects)
		{
			playEffects = false;
			base.transform.Find("AnimationNode").GetComponent<Animation>().Play();
			base.transform.Find("StarburstEffect").GetComponent<ParticleSystem>().Play();
		}
	}
}
