using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class WPFMonoBehaviour : MonoBehaviour
{
	private Animation cachedAnimation;

	private Collider cachedCollider;

	private Renderer cachedRenderer;

	private Rigidbody cachedRigidbody;

	protected static IngameCamera s_ingameCamera;

	protected static Camera s_hudCamera;

	protected static Camera s_mainCamera;

	protected static LevelManager s_levelManager;

	protected static GameData s_gameData;

	protected static EffectManager s_effectManager;

	protected static LevelSelector s_levelSelector;

	public Animation animation
	{
		get
		{
			if (cachedAnimation == null)
			{
				cachedAnimation = GetComponent<Animation>();
			}
			return cachedAnimation;
		}
		set
		{
			cachedAnimation = value;
		}
	}

	public Collider collider
	{
		get
		{
			if (cachedCollider == null)
			{
				cachedCollider = GetComponent<Collider>();
			}
			return cachedCollider;
		}
		set
		{
			cachedCollider = value;
		}
	}

	public Renderer renderer
	{
		get
		{
			if (cachedRenderer == null)
			{
				cachedRenderer = GetComponent<MeshRenderer>();
			}
			return cachedRenderer;
		}
		set
		{
			cachedRenderer = value;
		}
	}

	public Rigidbody rigidbody
	{
		get
		{
			if (cachedRigidbody == null)
			{
				cachedRigidbody = GetComponent<Rigidbody>();
			}
			return cachedRigidbody;
		}
		set
		{
			cachedRigidbody = value;
		}
	}

	public static IngameCamera ingameCamera
	{
		get
		{
			if ((bool)s_ingameCamera)
			{
				return s_ingameCamera;
			}
			IngameCamera[] array = Object.FindObjectsOfType<IngameCamera>();
			if (array.Length != 0)
			{
				s_ingameCamera = array[0];
			}
			return s_ingameCamera;
		}
	}

	public static LevelSelector levelSelector
	{
		get
		{
			if ((bool)s_levelSelector)
			{
				return s_levelSelector;
			}
			s_levelSelector = Object.FindObjectOfType<LevelSelector>();
			return s_levelSelector;
		}
	}

	public static Camera hudCamera
	{
		get
		{
			if ((bool)s_hudCamera)
			{
				return s_hudCamera;
			}
			GameObject gameObject = GameObject.FindGameObjectWithTag("HUDCamera");
			if ((bool)gameObject)
			{
				s_hudCamera = gameObject.GetComponent<Camera>();
			}
			return s_hudCamera;
		}
	}

	public static Camera mainCamera
	{
		get
		{
			if ((bool)s_mainCamera)
			{
				return s_mainCamera;
			}
			s_mainCamera = Camera.main;
			return s_mainCamera;
		}
	}

	public static LevelManager levelManager
	{
		get
		{
			if ((bool)s_levelManager)
			{
				return s_levelManager;
			}
			if (!Singleton<GameManager>.Instance.IsInGame())
			{
				return null;
			}
			LevelManager[] array = Object.FindObjectsOfType<LevelManager>();
			if (array.Length != 0)
			{
				s_levelManager = array[0];
			}
			return s_levelManager;
		}
	}

	public static EffectManager effectManager
	{
		get
		{
			if ((bool)s_effectManager)
			{
				return s_effectManager;
			}
			EffectManager[] array = Object.FindObjectsOfType<EffectManager>();
			if (array.Length != 0)
			{
				s_effectManager = array[0];
			}
			return s_effectManager;
		}
	}

	public static GameData gameData
	{
		get
		{
			if (INSettings.GetBool(INFeature.RuntimeGameData) && INRuntimeGameData.IsInitialized)
			{
				return Singleton<INRuntimeGameData>.Instance.GameData;
			}
			if ((bool)s_gameData)
			{
				return s_gameData;
			}
			s_gameData = Singleton<GameManager>.Instance.gameData;
			return s_gameData;
		}
	}

	public static Vector3 ScreenToZ0(Vector3 pos)
	{
		if ((bool)ingameCamera && ingameCamera.GetComponent<Camera>().orthographic)
		{
			Camera camera = mainCamera;
			pos.z = camera.farClipPlane;
			Vector3 result = camera.ScreenToWorldPoint(pos);
			result.z = 0f;
			return result;
		}
		Camera camera2 = mainCamera;
		pos.z = camera2.farClipPlane;
		Vector3 result2 = camera2.ScreenToWorldPoint(pos);
		result2.z = 0f;
		return result2;
	}

	public static T FindSceneObjectOfType<T>() where T : Object
	{
		T[] array = Object.FindObjectsOfType<T>();
		if (array.Length != 0)
		{
			return array[0];
		}
		return null;
	}

	public static int GetNumberOfHighestBit(int val)
	{
		for (int num = 30; num >= 0; num--)
		{
			if ((val & (1 << num)) != 0)
			{
				return num;
			}
		}
		return -1;
	}

	public static Vector3 ClipAgainstViewport(Vector3 pos1, Vector3 pos2)
	{
		Camera camera = mainCamera;
		Vector3 vector = camera.WorldToViewportPoint(pos1);
		Vector3 vector2 = camera.WorldToViewportPoint(pos2) - vector;
		float num = 1f;
		if (vector2.x < 0f)
		{
			float num2 = vector.x / (0f - vector2.x);
			if (num2 < num)
			{
				num = num2;
			}
		}
		if (vector2.y < 0f)
		{
			float num3 = vector.y / (0f - vector2.y);
			if (num3 < num)
			{
				num = num3;
			}
		}
		if (vector2.x > 0f)
		{
			float num4 = (1f - vector.x) / vector2.x;
			if (num4 < num)
			{
				num = num4;
			}
		}
		if (vector2.y > 0f)
		{
			float num5 = (1f - vector.y) / vector2.y;
			if (num5 < num)
			{
				num = num5;
			}
		}
		return camera.ViewportToWorldPoint(vector + vector2 * num);
	}

	public T[] GetComponentsOnlyInChildren<T>() where T : Component
	{
		List<T> list = new List<T>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			list.AddRange(base.transform.GetChild(i).GetComponentsInChildren<T>());
		}
		return list.ToArray();
	}

	public List<T> GetActiveComponents<T>() where T : Component
	{
		List<T> list = new List<T>(GetComponentsInChildren<T>(includeInactive: true));
		for (int i = 0; i < list.Count; i++)
		{
			PropertyInfo property = list[i].GetType().GetProperty("enabled");
			bool flag = true;
			if (property != null && property.PropertyType == typeof(bool))
			{
				flag = (bool)property.GetValue(list[i], null);
			}
			if (!flag || !list[i].gameObject.activeSelf)
			{
				list.RemoveAt(i--);
			}
		}
		return list;
	}
}
