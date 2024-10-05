using System.Collections.Generic;
using UnityEngine;

public abstract class OneTimeCollectable : WPFMonoBehaviour
{
	[SerializeField]
	protected ParticleSystem collectedEffect;

	[SerializeField]
	private GameObject xpParticles;

	public bool collected;

	private Vector3 startPosition = Vector3.zero;

	private bool startPositionInited;

	protected bool disabled;

	protected bool isDynamic;

	protected bool isBox;

	protected bool isGhost;

	protected Component[] components;

	private string cachedNameKey = string.Empty;

	public bool Disabled => disabled;

	public Vector3 StartPosition => startPosition;

	public string NameKey
	{
		get
		{
			if (string.IsNullOrEmpty(cachedNameKey))
			{
				cachedNameKey = GetNameKey();
			}
			return cachedNameKey;
		}
	}

	protected virtual void Start()
	{
		startPosition = base.transform.position;
		startPositionInited = true;
		isDynamic = GetComponent<LevelRigidbody>() != null;
		isBox = this is PartBox || this is StarBox;
		if (isBox)
		{
			CheckIfGhost();
		}
		components = GetComponents<Component>();
		EventManager.Connect<UIEvent>(OnUIEvent);
		EventManager.Connect<GameStateChanged>(OnGameStateChanged);
		if (INSettings.GetBool(INFeature.AutoGetCollections))
		{
			Collect();
		}
	}

	protected virtual void OnDestroy()
	{
		EventManager.Disconnect<UIEvent>(OnUIEvent);
		EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
	}

	public void ResetToStartPosition()
	{
		if (startPositionInited && !collected)
		{
			base.transform.position = startPosition;
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (disabled || WPFMonoBehaviour.levelManager.gameState == LevelManager.GameState.Completed)
		{
			return;
		}
		BasePart basePart = FindPart(col);
		if (!basePart || !WPFMonoBehaviour.levelManager || !WPFMonoBehaviour.levelManager.ContraptionRunning)
		{
			return;
		}
		WPFMonoBehaviour.levelManager.ContraptionRunning.FinishConnectedComponentSearch();
		BasePart basePart2 = WPFMonoBehaviour.levelManager.ContraptionRunning.FindPig();
		if (WPFMonoBehaviour.levelManager.ContraptionRunning.IsConnectedToPig(basePart, col))
		{
			Collect();
			return;
		}
		if (basePart.m_partType == BasePart.PartType.GoldenPig)
		{
			Collect();
			return;
		}
		List<BasePart> parts = WPFMonoBehaviour.levelManager.ContraptionRunning.Parts;
		for (int i = 0; i < parts.Count; i++)
		{
			BasePart basePart3 = parts[i];
			if (basePart3 != null && basePart3.ConnectedComponent == basePart.ConnectedComponent && Vector3.Distance(basePart3.transform.position, basePart2.transform.position) < 2.5f)
			{
				Collect();
				break;
			}
		}
	}

	private BasePart FindPart(Collider collider)
	{
		Transform parent = collider.transform;
		while (parent != null)
		{
			BasePart component = parent.GetComponent<BasePart>();
			if ((bool)component)
			{
				return component;
			}
			parent = parent.parent;
		}
		return null;
	}

	protected virtual void DisableGoal(bool disable = true)
	{
		HideChildren(base.transform, disable);
		disabled = disable;
		collected = disable;
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i] is MonoBehaviour)
			{
				(components[i] as MonoBehaviour).enabled = !disable;
			}
			else if (components[i] is Renderer)
			{
				(components[i] as Renderer).enabled = !disable;
			}
			else if (components[i] is Collider)
			{
				(components[i] as Collider).enabled = !disable;
			}
			else if (components[i] is Rigidbody)
			{
				Debug.LogWarning("DisableGoal for " + base.transform.name + ", isKinematic will be " + (!isDynamic || disable));
				(components[i] as Rigidbody).isKinematic = !isDynamic || disable;
				(components[i] as Rigidbody).detectCollisions = !disable;
			}
		}
	}

	public virtual bool IsDisabled()
	{
		return disabled;
	}

	private void HideChildren(Transform parent, bool hide = true)
	{
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			Renderer component = child.GetComponent<Renderer>();
			if (component != null)
			{
				component.enabled = !hide;
			}
			child.gameObject.SetActive(!hide);
			HideChildren(child, hide);
		}
	}

	public virtual void Collect()
	{
		if (!collected)
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

	public virtual void OnCollected()
	{
	}

	protected virtual void OnUIEvent(UIEvent data)
	{
		if (!isBox)
		{
			return;
		}
		if (disabled && data.type == UIEvent.Type.Building)
		{
			CheckIfGhost();
			if (base.rigidbody != null)
			{
				base.rigidbody.isKinematic = true;
			}
		}
		if (!disabled && isDynamic && data.type == UIEvent.Type.Play)
		{
			base.rigidbody.isKinematic = false;
		}
	}

	protected virtual void OnGameStateChanged(GameStateChanged data)
	{
		if (data.state == LevelManager.GameState.Building)
		{
			ResetToStartPosition();
		}
	}

	private void CheckIfGhost()
	{
		if (this is StarBox || this is PartBox)
		{
			if (!string.IsNullOrEmpty(NameKey))
			{
				isGhost = GameProgress.HasSandboxStar(Singleton<GameManager>.Instance.CurrentSceneName, NameKey);
			}
			if (isGhost && this is StarBox)
			{
				MakeGhostBox();
			}
			else if (isGhost && this is PartBox)
			{
				MakeGhostBox();
			}
		}
	}

	private void MakeGhostBox()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (!(component == null))
		{
			int index = (component.sharedMaterial.name.StartsWith("IngameAtlas2") ? 1 : 0);
			component.sharedMaterial = AtlasMaterials.Instance.DimmedRenderQueueMaterials[index];
			Transform transform = base.transform.Find("Glow");
			if (transform != null)
			{
				transform.gameObject.SetActive(value: false);
			}
			if (disabled)
			{
				DisableGoal(disable: false);
			}
		}
	}

	protected void ShowXPParticles()
	{
		if ((bool)xpParticles)
		{
			Object.Instantiate(xpParticles, base.transform.position, base.transform.rotation);
		}
	}

	protected abstract string GetNameKey();
}
