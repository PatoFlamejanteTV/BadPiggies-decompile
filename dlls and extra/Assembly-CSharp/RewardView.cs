using UnityEngine;

public class RewardView : WPFMonoBehaviour
{
	public GameData m_gameData;

	public GameObject m_partIconBackground;

	private GameObject m_locked;

	private GameObject m_open;

	private GameObject m_animationNode;

	private GameObject m_plusNode;

	private GameObject m_particleNode;

	private bool m_animationTimerStarted;

	private float m_animationTimer;

	private bool m_particleTimerStarted;

	private float m_particleTimer;

	private Animation m_animationPlaying;

	private float m_animationTime;

	private void Awake()
	{
		if ((bool)base.transform.Find("Locked"))
		{
			m_locked = base.transform.Find("Locked").gameObject;
			EnableRendererRecursively(m_locked, enable: false);
		}
		m_open = base.transform.Find("Open").gameObject;
		EnableRendererRecursively(m_open, enable: false);
	}

	private void OnEnable()
	{
		if ((bool)m_animationPlaying)
		{
			m_animationPlaying.Play();
			m_animationPlaying[m_animationPlaying.clip.name].time = m_animationTime;
		}
	}

	private void Update()
	{
		if (m_animationTimerStarted)
		{
			m_animationTimer -= Time.deltaTime;
			if (m_animationTimer <= 0f)
			{
				m_animationTimerStarted = false;
				if ((bool)m_animationNode)
				{
					Singleton<AudioManager>.Instance.Play2dEffect(WPFMonoBehaviour.gameData.commonAudioCollection.sandboxLevelUnlocked);
					m_animationNode.GetComponent<Animation>().Play();
					m_animationPlaying = m_animationNode.GetComponent<Animation>();
				}
				if ((bool)m_plusNode)
				{
					m_plusNode.GetComponent<Renderer>().enabled = false;
				}
			}
		}
		if (m_particleTimerStarted)
		{
			m_particleTimer -= Time.deltaTime;
			if (m_particleTimer <= 0f)
			{
				m_particleTimerStarted = false;
				if ((bool)m_particleNode)
				{
					m_particleNode.GetComponent<ParticleSystem>().Play();
				}
			}
		}
		if ((bool)m_animationPlaying)
		{
			if (m_animationPlaying.isPlaying)
			{
				m_animationTime = m_animationPlaying[m_animationPlaying.clip.name].time;
			}
			else
			{
				m_animationPlaying = null;
			}
		}
	}

	public void SetPart(BasePart.PartType type)
	{
		m_animationNode = base.transform.Find("Open").transform.Find("PartOffset").Find("AnimationNode").gameObject;
		m_plusNode = base.transform.Find("Open").transform.Find("PartOffset").transform.Find("Plus").gameObject;
		m_particleNode = base.transform.Find("Open").transform.Find("PartOffset").transform.Find("StarBurstEffect").gameObject;
		for (int i = 0; i < 3; i++)
		{
			GameObject obj = Object.Instantiate(m_partIconBackground);
			obj.transform.parent = m_animationNode.transform;
			int index = (obj.GetComponent<Renderer>().sharedMaterial.name.StartsWith("IngameAtlas2") ? 1 : 0);
			Material material = AtlasMaterials.Instance.PartQueueZMaterials[index];
			obj.GetComponent<Renderer>().material = material;
			obj.transform.localPosition = new Vector3(0f, 0f, 0.1f);
			obj.transform.localScale = 3f * Vector3.one;
		}
		GameObject obj2 = Object.Instantiate(m_gameData.GetPart(type).GetComponent<BasePart>().m_constructionIconSprite.gameObject);
		int index2 = (obj2.GetComponent<Renderer>().sharedMaterial.name.StartsWith("IngameAtlas2") ? 1 : 0);
		Material material2 = AtlasMaterials.Instance.PartQueueZMaterials[index2];
		obj2.GetComponent<Renderer>().material = material2;
		obj2.transform.parent = m_animationNode.transform;
		obj2.transform.localPosition = Vector3.zero;
		obj2.transform.localScale = 2.75f * Vector3.one;
		m_animationTimerStarted = true;
		m_animationTimer = 2.8f;
		m_particleTimerStarted = true;
		m_particleTimer = 3.8f;
	}

	public bool HasLocked()
	{
		return m_locked != null;
	}

	public void ShowLocked()
	{
		EnableRendererRecursively(m_open, enable: false);
		EnableRendererRecursively(m_locked, enable: true);
	}

	public void ShowOpen()
	{
		if ((bool)m_locked)
		{
			EnableRendererRecursively(m_locked, enable: false);
		}
		EnableRendererRecursively(m_open, enable: true);
	}

	public void Hide()
	{
		EnableRendererRecursively(m_open, enable: false);
		if ((bool)m_locked)
		{
			EnableRendererRecursively(m_locked, enable: false);
		}
	}

	private void EnableRendererRecursively(GameObject obj, bool enable)
	{
		if ((bool)obj.GetComponent<Renderer>())
		{
			obj.GetComponent<Renderer>().enabled = enable;
		}
		if ((bool)obj.GetComponent<Collider>())
		{
			obj.GetComponent<Collider>().enabled = enable;
		}
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			EnableRendererRecursively(obj.transform.GetChild(i).gameObject, enable);
		}
	}
}
