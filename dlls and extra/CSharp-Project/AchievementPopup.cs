using System.Collections;
using UnityEngine;

public class AchievementPopup : MonoBehaviour
{
	private GameObject m_popup;

	private Renderer m_icon;

	private TextMesh m_text;

	private TextMeshLocale m_localeText;

	private SpriteRenderer m_sprite;

	private UnityEngine.Sprite[] sprites;

	private void Start()
	{
		m_popup = base.transform.Find("Popup").gameObject;
		m_text = base.transform.Find("Popup/Text").GetComponent<TextMesh>();
		m_icon = base.transform.Find("Popup/Icon").GetComponent<Renderer>();
		m_sprite = base.transform.Find("Popup/SpriteIcon").GetComponent<SpriteRenderer>();
		m_localeText = base.transform.Find("Popup/Text").GetComponent<TextMeshLocale>();
		m_popup.transform.position = Vector3.up * 13f;
		Object.DontDestroyOnLoad(this);
	}

	public void Show(string achievementId)
	{
		if (sprites == null)
		{
			sprites = Resources.LoadAll<UnityEngine.Sprite>("Achievements/Achievements_Sheet");
		}
		string iconFileName = Singleton<AchievementData>.Instance.AchievementsLimits[achievementId].iconFileName;
		UnityEngine.Sprite[] array = sprites;
		foreach (UnityEngine.Sprite sprite in array)
		{
			if (sprite.name == iconFileName)
			{
				m_sprite.sprite = sprite;
				break;
			}
		}
		if (m_sprite.sprite == null)
		{
			string path = "Achievements/" + Singleton<AchievementData>.Instance.AchievementsLimits[achievementId].iconFileName;
			m_icon.material.mainTexture = Resources.Load(path, typeof(Texture2D)) as Texture2D;
			m_sprite.enabled = false;
			m_icon.enabled = true;
		}
		else
		{
			m_sprite.enabled = true;
			m_icon.enabled = false;
		}
		m_text.text = achievementId;
		m_localeText.RefreshTranslation(Singleton<Localizer>.Instance.CurrentLocale);
		m_popup.GetComponent<Animation>().Play("AchievementPopupEnter");
		StartCoroutine(UnloadIconAfterAnimation());
	}

	private IEnumerator UnloadIconAfterAnimation()
	{
		while (m_popup.GetComponent<Animation>().isPlaying)
		{
			yield return new WaitForSeconds(0.5f);
		}
		Object mainTexture = m_icon.material.mainTexture;
		m_icon.material.mainTexture = null;
		Resources.UnloadAsset(mainTexture);
	}

	private IEnumerator Test()
	{
		int i = 0;
		foreach (string key in Singleton<AchievementData>.Instance.AchievementsLimits.Keys)
		{
			i++;
			Show(key);
			yield return new WaitForSeconds(3f);
		}
	}
}
