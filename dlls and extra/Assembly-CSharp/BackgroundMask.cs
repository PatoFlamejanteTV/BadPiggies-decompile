using System;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMask : MonoBehaviour
{
	private struct BackgroundData
	{
		public string sortingLayerName;

		public Transform target;

		public Vector3 offset;

		public object owner;

		public Vector3 TargetPosition => ((!(target != null)) ? Vector3.zero : target.position) + offset;

		public BackgroundData(string sortingLayerName, Transform target, Vector3 offset, object owner)
		{
			this.sortingLayerName = sortingLayerName;
			this.target = target;
			this.offset = offset;
			this.owner = owner;
		}
	}

	private class StackList<T>
	{
		public List<T> list;

		public int Count => list.Count;

		private int Head => Count - 1;

		public StackList()
		{
			list = new List<T>();
		}

		public void Push(T element)
		{
			list.Add(element);
		}

		public T Pop()
		{
			if (Count <= 0)
			{
				return default(T);
			}
			T result = list[Head];
			list.RemoveAt(Head);
			return result;
		}

		public T Peek()
		{
			if (Count <= 0)
			{
				return default(T);
			}
			return list[Head];
		}
	}

	private static int depth = 0;

	private static int prevDepth = 0;

	private static StackList<BackgroundData> depthPositions = null;

	private static MeshRenderer instanceRenderer = null;

	private static BackgroundMask instance = null;

	private static float fade = 0f;

	private static bool showing = false;

	private static bool applySmoothFade = false;

	private static Color transparentBlack = new Color(0f, 0f, 0f, 0f);

	private static bool isExiting = false;

	private static BackgroundMask Instance
	{
		get
		{
			if (isExiting)
			{
				return null;
			}
			if (instance == null)
			{
				GameObject gameObject = Resources.Load<GameObject>("UI/BackgroundMask");
				if (gameObject != null)
				{
					GameObject obj = UnityEngine.Object.Instantiate(gameObject);
					obj.name = "BackgroundMask";
					instance = obj.GetComponent<BackgroundMask>();
				}
			}
			if (instance == null)
			{
				instance = new GameObject("BackgroundMask", typeof(BackgroundMask)).GetComponent<BackgroundMask>();
			}
			if (depthPositions == null)
			{
				depthPositions = new StackList<BackgroundData>();
			}
			return instance;
		}
	}

	public static bool Instantiated => instance != null;

	public static void Show(bool show, object owner, string sortingLayerName = "", Transform target = null, Vector3 offset = default(Vector3), bool smoothFade = false)
	{
		if (owner == null)
		{
			throw new ArgumentException("Argument 'owner' cannot be null!");
		}
		if (Instance == null)
		{
			return;
		}
		applySmoothFade = smoothFade;
		if (show)
		{
			BackgroundData backgroundData = new BackgroundData(sortingLayerName, target, offset, owner);
			if (!Add(backgroundData))
			{
				return;
			}
			prevDepth = depth++;
			SetBackground(backgroundData);
			showing = true;
		}
		else if (!show && depthPositions.Count > 0)
		{
			if (!Remove(owner))
			{
				return;
			}
			prevDepth = depth--;
			if (depthPositions.Count > 0)
			{
				SetBackground(depthPositions.Peek());
			}
			showing = depth > 0;
		}
		if (applySmoothFade)
		{
			instance.gameObject.SetActive(value: true);
		}
		else
		{
			instance.gameObject.SetActive(showing);
		}
	}

	private static void SetBackground(BackgroundData data)
	{
		if (instance == null)
		{
			return;
		}
		instance.transform.position = data.TargetPosition;
		if (instanceRenderer == null)
		{
			instanceRenderer = instance.GetComponent<MeshRenderer>();
			if (instanceRenderer != null)
			{
				AtlasMaterials.Instance.AddMaterialInstance(instanceRenderer.material);
			}
		}
		if (instanceRenderer != null)
		{
			instanceRenderer.sortingLayerName = data.sortingLayerName;
			if (depth == 1 && prevDepth == 0)
			{
				instanceRenderer.material.color = ((!applySmoothFade) ? Color.white : transparentBlack);
			}
		}
	}

	private void OnDestroy()
	{
		if (!(instance == null) && AtlasMaterials.IsInstantiated && instanceRenderer == null)
		{
			instanceRenderer = instance.GetComponent<MeshRenderer>();
			if (instanceRenderer != null)
			{
				AtlasMaterials.Instance.RemoveMaterialInstance(instanceRenderer.material);
			}
		}
	}

	public static void SetParent(Transform parent)
	{
		if (!(instance == null))
		{
			instance.transform.parent = parent;
		}
	}

	private void Update()
	{
		if (applySmoothFade && !(instance == null) && !(instanceRenderer == null))
		{
			if (!showing && Mathf.Approximately(fade, 0f))
			{
				instance.gameObject.SetActive(value: false);
			}
			fade += GameTime.RealTimeDelta * ((!showing) ? (-2f) : 2f);
			if (fade < 0f)
			{
				fade = 0f;
			}
			else if (fade > 1f)
			{
				fade = 1f;
			}
			instanceRenderer.material.color = Color.Lerp(transparentBlack, Color.white, fade);
		}
	}

	private void LateUpdate()
	{
		if (depthPositions.Count > 0)
		{
			base.transform.position = depthPositions.Peek().TargetPosition;
		}
	}

	private static bool Remove(object owner)
	{
		for (int i = 0; i < depthPositions.Count; i++)
		{
			if (depthPositions.list[i].owner == owner)
			{
				depthPositions.list.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	private static bool Add(BackgroundData data)
	{
		for (int i = 0; i < depthPositions.Count; i++)
		{
			if (depthPositions.list[i].owner == data.owner)
			{
				return false;
			}
		}
		depthPositions.Push(data);
		return true;
	}

	private void OnApplicationQuit()
	{
		isExiting = true;
	}
}
