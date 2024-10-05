using System.Collections;
using UnityEngine;

public class Star : WPFMonoBehaviour
{
	protected bool m_isTriggered;

	private void OnTriggerEnter(Collider col)
	{
		CheckTrigger(col.transform);
	}

	private void CheckTrigger(Transform t)
	{
		if (!(t.tag == "Contraption") || m_isTriggered)
		{
			if ((bool)t.parent)
			{
				CheckTrigger(t.parent);
			}
			return;
		}
		BasePart component = t.GetComponent<BasePart>();
		if ((!(component != null) || (component.m_partType != BasePart.PartType.Balloon && component.m_partType != BasePart.PartType.Balloons2 && component.m_partType != BasePart.PartType.Balloons3)) && !(Vector3.Distance(t.position, WPFMonoBehaviour.levelManager.ContraptionRunning.m_cameraTarget.transform.position) > 2f))
		{
			m_isTriggered = true;
			StartCoroutine(PlayAnimation());
		}
	}

	public IEnumerator PlayAnimation()
	{
		Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.goalBoxCollected);
		float timer = 1f;
		while (timer > 0f)
		{
			base.transform.localScale += Vector3.one * 0.1f * Mathf.Pow(timer, 2f);
			base.transform.position += Vector3.up * 0.1f * Mathf.Pow(timer, 2f);
			timer -= 0.03f;
			yield return new WaitForSeconds(0.03f);
		}
		base.gameObject.GetComponent<Renderer>().enabled = false;
		GameObject particles = Object.Instantiate(WPFMonoBehaviour.gameData.m_particles, base.transform.position, Quaternion.identity);
		yield return new WaitForSeconds(1.5f);
		WPFMonoBehaviour.levelManager.NotifyStarCollected();
		Object.Destroy(particles);
		Object.Destroy(base.gameObject);
	}
}
