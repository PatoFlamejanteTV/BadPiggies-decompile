using UnityEngine;

public class BoxingGlove : MonoBehaviour
{
	private static bool suckerPunchReported;

	private void OnCollisionEnter(Collision coll)
	{
		if (!suckerPunchReported && Singleton<SocialGameManager>.IsInstantiated() && (bool)coll.collider.gameObject.GetComponent<KingPig>())
		{
			suckerPunchReported = true;
			Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.SUCKER_PUNCH", 100.0);
		}
	}
}
