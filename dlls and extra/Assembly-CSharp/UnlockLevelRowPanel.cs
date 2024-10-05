using UnityEngine;

public class UnlockLevelRowPanel : MonoBehaviour
{
	public delegate void OnWatchAdPressed();

	public delegate void OnPayPressed();

	[SerializeField]
	private GameObject unlockDialogPrefab;

	[SerializeField]
	private BoxCollider bgCollider;

	[SerializeField]
	private Panel bgPanel;

	[SerializeField]
	private float buttonOffset;

	private LevelRowUnlockDialog unlockDialog;

	private GameObject openPopupButton;

	private int page;

	public LevelRowUnlockDialog UnlockDialog => unlockDialog;

	public Vector2 BackgroundScale
	{
		set
		{
			bgPanel.gameObject.transform.localScale = value;
			bgCollider.gameObject.transform.localScale = value;
		}
	}

	public int Page
	{
		get
		{
			return page;
		}
		set
		{
			page = value;
		}
	}

	public Vector2 RealSize => new Vector2(bgCollider.size.x * bgCollider.transform.localScale.x, bgCollider.size.y * bgCollider.transform.localScale.y);

	public Vector2 UnscaledSize => new Vector2(bgCollider.size.x, bgCollider.size.y);

	public Vector2 PanelSize => new Vector2(bgPanel.m_width, bgPanel.m_height);

	public event OnWatchAdPressed WatchAd;

	public event OnPayPressed Pay;

	private void Awake()
	{
		openPopupButton = base.transform.Find("OpenPopupButton").gameObject;
		unlockDialog = Object.Instantiate(unlockDialogPrefab).GetComponent<LevelRowUnlockDialog>();
		UnlockDialog.transform.position = new Vector3(0f, 0f, -95f);
		unlockDialog.Close();
	}

	private void Start()
	{
		PositionButton();
	}

	public void SetCost(int cost)
	{
		string text = $"[snout] {cost}";
		unlockDialog.transform.Find("PayUnlockBtn/Text").gameObject.GetComponent<TextMesh>().text = text;
		unlockDialog.transform.Find("PayUnlockBtnDisabled/Text").gameObject.GetComponent<TextMesh>().text = text;
		unlockDialog.ShowConfirmEnabled = () => GameProgress.SnoutCoinCount() >= cost;
	}

	public void AdButtonPressed()
	{
		if (this.WatchAd != null)
		{
			this.WatchAd();
		}
	}

	public void PayButtonPressed()
	{
		if (this.Pay != null)
		{
			this.Pay();
		}
	}

	public void OpenUnlockDialog()
	{
		unlockDialog.Open();
		UserSettings.SetInt(Singleton<GameManager>.Instance.CurrentSceneName + "_active_page", page);
		Transform transform = unlockDialog.transform.Find("PayUnlockBtn");
		if ((bool)transform)
		{
			transform.GetComponent<Button>().MethodToCall.SetMethod(this, "PayButtonPressed");
		}
	}

	public void PulseButton()
	{
		openPopupButton.GetComponent<Animation>().Play();
	}

	private void PositionButton()
	{
		float num = (0f - RealSize.x) / 2f;
		float x = openPopupButton.GetComponent<BoxCollider>().size.x;
		openPopupButton.transform.localPosition = new Vector3(num + x / 2f + buttonOffset, 0f, -5.1f);
	}
}
