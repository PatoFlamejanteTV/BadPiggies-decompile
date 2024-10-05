using System.Collections;
using UnityEngine;

public class SkullPopup : MonoBehaviour
{
	[SerializeField]
	private CollectableType type;

	[SerializeField]
	private int maxCount = 45;

	[SerializeField]
	private GameObject particlesXP;

	private Transform m_skull;

	private Material m_bgColor;

	private void Start()
	{
		maxCount = GameProgress.MaxSkullCount();
		m_skull = base.transform.Find("Skull");
		m_skull.localPosition = -Vector3.up * 17f;
		m_bgColor = base.transform.Find("BackgroundBox").GetComponent<Renderer>().material;
		int num = 0;
		switch (type)
		{
		case CollectableType.Statue:
			num = GameProgress.SecretStatueCount();
			break;
		case CollectableType.Skull:
			num = GameProgress.SecretSkullCount();
			break;
		}
		base.transform.Find("Skull/SkullText").GetComponent<TextMesh>().text = ((num >= 10) ? (num + "/" + maxCount) : ("0" + num + "/" + maxCount));
		StartCoroutine(PlayAnimation());
	}

	private IEnumerator PlayAnimation()
	{
		float alpha = 0f;
		while (alpha < 1f)
		{
			alpha += Time.deltaTime * 4f;
			m_bgColor.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
			yield return new WaitForEndOfFrame();
		}
		while (m_skull.localPosition.y < -0.01f)
		{
			m_skull.localPosition -= Vector3.up * m_skull.localPosition.y * Time.deltaTime * 4f;
			yield return new WaitForEndOfFrame();
		}
		m_skull.localPosition = Vector3.up * 0.05f;
		Object.Instantiate(particlesXP, m_skull.position + Vector3.back, Quaternion.identity).layer = m_skull.gameObject.layer;
		while (m_skull.localPosition.y < 17f)
		{
			m_skull.localPosition += Vector3.up * m_skull.localPosition.y * Time.deltaTime * 12f;
			yield return new WaitForEndOfFrame();
		}
		while (alpha > 0f)
		{
			alpha -= Time.deltaTime * 4f;
			m_bgColor.SetColor("_Color", new Color(1f, 1f, 1f, alpha));
			yield return new WaitForEndOfFrame();
		}
		base.gameObject.SetActive(value: false);
	}
}
