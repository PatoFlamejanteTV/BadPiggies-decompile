using UnityEngine;

public class CreditsCluster : MonoBehaviour
{
	[SerializeField]
	private string clusterLabelId = "localization_id_not_defined";

	[SerializeField]
	private string[] names = new string[0];

	[SerializeField]
	private GameObject creditsTextPrefab;

	[SerializeField]
	private Material dropShadowMaterial;

	[SerializeField]
	private float characterSize = 0.1f;

	private readonly Vector3 shadowOffset = new Vector3(0.1f, -0.1f, 0f);

	public float CreateCreditsCluster(Vector3 offset)
	{
		CreateCreditsLine(clusterLabelId, offset, localize: true);
		float num = offset.y;
		string[] array = names;
		foreach (string textContent in array)
		{
			num -= 1f;
			CreateCreditsLine(textContent, new Vector3(0f, num, 0f), localize: false);
		}
		return num - 0.5f;
	}

	private void Start()
	{
	}

	private void CreateCreditsLine(string textContent, Vector3 offset, bool localize)
	{
		GameObject gameObject = Object.Instantiate(creditsTextPrefab);
		gameObject.transform.parent = base.transform;
		gameObject.transform.Translate(offset);
		gameObject.GetComponent<TextMesh>().text = textContent;
		GameObject gameObject2 = Object.Instantiate(creditsTextPrefab);
		gameObject2.transform.parent = base.transform;
		gameObject2.GetComponent<TextMesh>().text = textContent;
		gameObject2.transform.Translate(offset + shadowOffset);
		gameObject2.GetComponent<Renderer>().material = dropShadowMaterial;
		gameObject2.GetComponent<TextMesh>().offsetZ = 0.1f;
		if (localize)
		{
			gameObject.gameObject.AddComponent<TextMeshLocale>();
			gameObject.GetComponent<TextMesh>().characterSize = characterSize;
			gameObject2.GetComponent<TextMesh>().characterSize = characterSize;
			gameObject2.gameObject.AddComponent<TextMeshLocale>();
		}
		if (textContent.Length > 10)
		{
			gameObject.name = textContent.Substring(0, 9);
			gameObject2.name = textContent.Substring(0, 9) + "(shadow)";
		}
		else
		{
			gameObject.name = textContent;
			gameObject2.name = textContent + "(shadow)";
		}
	}
}
