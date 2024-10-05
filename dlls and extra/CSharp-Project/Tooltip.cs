using System.Collections;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
	private TextMeshLocale locale;

	private TextMesh textMesh;

	private bool closing;

	private string thisLocaleKey = string.Empty;

	private Transform followTarget;

	private Camera hudCamera;

	private Transform background;

	private Transform arrow;

	private Vector3 backgroundOriginalPosition;

	private Vector3 arrowOriginalPosition;

	private void Awake()
	{
		arrow = base.transform.Find("Arrow");
		if (arrow != null)
		{
			arrowOriginalPosition = arrow.localPosition;
		}
		background = base.transform.Find("Popup");
		if (background != null)
		{
			backgroundOriginalPosition = background.localPosition;
			Transform transform = background.Find("InfoText");
			if (transform != null)
			{
				locale = transform.GetComponent<TextMeshLocale>();
				textMesh = transform.GetComponent<TextMesh>();
			}
		}
		base.transform.localScale = Vector3.one * 0.001f;
		hudCamera = Singleton<GuiManager>.Instance.FindCamera();
	}

	private IEnumerator Start()
	{
		yield return null;
		if (!string.IsNullOrEmpty(thisLocaleKey))
		{
			SetText(thisLocaleKey);
			GetComponent<Animation>().Play("TooltipOpen");
		}
	}

	private void Update()
	{
		if (!closing && GuiManager.GetPointer().dragging)
		{
			Close();
		}
		if (!followTarget)
		{
			return;
		}
		if ((bool)hudCamera && (bool)background && !GetComponent<Animation>().isPlaying)
		{
			Vector3 vector = Vector3.right * ((float)Screen.width / (float)Screen.height) * hudCamera.orthographicSize;
			Vector3 vector2 = Vector3.right * (base.transform.position.x + background.GetComponent<Renderer>().bounds.size.x) + Vector3.up * (base.transform.position.y + background.GetComponent<Renderer>().bounds.size.y);
			if (vector.x < vector2.x)
			{
				Vector3 vector3 = Vector3.right * (vector.x - vector2.x);
				background.localPosition = backgroundOriginalPosition + vector3;
				if ((bool)arrow && vector3.x < -7f)
				{
					arrow.localPosition = arrowOriginalPosition + (vector3 + Vector3.right * 7f);
				}
			}
		}
		base.transform.position = followTarget.position;
	}

	private void Close()
	{
		closing = true;
		GetComponent<Animation>().Play("TooltipClose");
		StartCoroutine(DelayClose());
	}

	private IEnumerator DelayClose()
	{
		while (GetComponent<Animation>().isPlaying)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	private void SetText(string localeKey)
	{
		thisLocaleKey = localeKey;
		textMesh.text = thisLocaleKey;
		locale.RefreshTranslation();
		if (TextMeshHelper.UsesKanjiCharacters())
		{
			TextMeshHelper.Wrap(textMesh, 13);
		}
		else
		{
			TextMeshHelper.Wrap(textMesh, 24);
		}
	}

	public void SetLocaleKey(string tooltipLocaleKey)
	{
		thisLocaleKey = tooltipLocaleKey;
	}

	public void SetTarget(Transform newTarget)
	{
		followTarget = newTarget;
	}
}
