using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
	public static List<Vector3> enabledLightPositions;

	private static LightManager instance;

	[SerializeField]
	private Material maskNormalMaterial;

	[SerializeField]
	private Material maskNVMaterial;

	[SerializeField]
	private Material lightBorderNormalMaterial;

	[SerializeField]
	private Material lightBorderNVMaterial;

	private bool disableNv;

	private bool isInit;

	private bool nvOn;

	private PointLightContainer container;

	private GameObject pointLightPrefab;

	private LevelManager levelManager;

	private PointLightSource startPls;

	private GameObject mask;

	private GameObject nightVisionMask;

	public bool NightVisionOn => nvOn;

	public static LightManager Instance => instance;

	private void Awake()
	{
		instance = this;
		nightVisionMask = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Lights/MaskQuadNightVision"));
		nightVisionMask.transform.parent = WPFMonoBehaviour.ingameCamera.transform;
		nightVisionMask.transform.localPosition = Vector3.forward * 0.5f;
		nightVisionMask.SetActive(value: false);
		EventManager.Connect<GameStateChanged>(OnGameStateChanged);
	}

	private void OnDestroy()
	{
		EventManager.Disconnect<GameStateChanged>(OnGameStateChanged);
	}

	public void Init(LevelManager _levelManager)
	{
		levelManager = _levelManager;
		mask = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Lights/MaskQuad"));
		mask.transform.parent = WPFMonoBehaviour.ingameCamera.transform;
		mask.transform.localPosition = Vector3.forward * 2.5f;
		if (INSettings.GetBool(INFeature.HideDarkMask))
		{
			mask.gameObject.SetActive(value: false);
		}
		pointLightPrefab = Resources.Load<GameObject>("Prefabs/Lights/PointLight");
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Lights/PointLightContainer"));
		if (gameObject != null)
		{
			gameObject.transform.position = WPFMonoBehaviour.ingameCamera.transform.position + Vector3.forward;
			container = gameObject.GetComponent<PointLightContainer>();
			if (container != null)
			{
				GameObject gameObject2 = Object.Instantiate(pointLightPrefab);
				startPls = gameObject2.GetComponent<PointLightSource>();
				if (startPls != null)
				{
					startPls.size = 1f + 0.5f * (float)Mathf.Max(levelManager.GridWidth, levelManager.GridHeight);
					startPls.usesCurves = false;
				}
				if (gameObject2 != null)
				{
					gameObject2.name = "StartPointLight";
					Vector3 startingPosition = levelManager.StartingPosition;
					startingPosition += 0.5f * Vector3.up * levelManager.GridHeight;
					startingPosition -= Vector3.up * 0.5f;
					if (levelManager.GridWidth % 2 == 0)
					{
						startingPosition += Vector3.right * 0.5f;
					}
					gameObject2.transform.position = startingPosition;
				}
			}
		}
		isInit = true;
	}

	[ContextMenu("Toggle Nightvision")]
	public void ToggleNightVision()
	{
		if (isInit)
		{
			nvOn = !nvOn;
			nightVisionMask.SetActive(nvOn);
			mask.GetComponent<Renderer>().sharedMaterial = ((!nvOn) ? maskNormalMaterial : maskNVMaterial);
			container.borderMaterial = ((!nvOn) ? lightBorderNormalMaterial : lightBorderNVMaterial);
			UpdateLights();
			if (Singleton<SocialGameManager>.IsInstantiated())
			{
				Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.LIGHT_UP_DARKNESS", 100.0);
			}
		}
	}

	private void OnGameStateChanged(GameStateChanged newState)
	{
		if (!isInit)
		{
			return;
		}
		if (newState.state == LevelManager.GameState.Building)
		{
			if ((bool)startPls)
			{
				startPls.isEnabled = true;
			}
			if (disableNv && nvOn)
			{
				ToggleNightVision();
				disableNv = false;
			}
		}
		else if (newState.state == LevelManager.GameState.Running)
		{
			if ((bool)startPls)
			{
				startPls.isEnabled = false;
			}
			GameObject gameObject = Object.Instantiate(pointLightPrefab);
			PointLightSource component = gameObject.GetComponent<PointLightSource>();
			if (component != null)
			{
				component.size = 2f;
				component.canCollide = true;
				component.canLitObjects = true;
				component.usesCurves = false;
			}
			if (nvOn)
			{
				disableNv = true;
			}
			if (gameObject != null)
			{
				gameObject.transform.parent = levelManager.ContraptionRunning.FindPig().transform;
				gameObject.transform.localPosition = Vector3.zero;
			}
			BasePart basePart = levelManager.ContraptionRunning.FindPart(BasePart.PartType.GoldenPig);
			if ((bool)basePart)
			{
				Transform transform = basePart.transform;
				GameObject obj = Object.Instantiate(pointLightPrefab);
				PointLightSource component2 = obj.GetComponent<PointLightSource>();
				if (component2 != null)
				{
					component2.size = 2f;
					component2.canCollide = true;
					component2.canLitObjects = true;
					component2.usesCurves = false;
				}
				obj.transform.parent = transform.Find("Graphics");
				obj.transform.localPosition = Vector3.zero;
			}
			List<WPFMonoBehaviour> list = new List<WPFMonoBehaviour>();
			TNT[] array = Object.FindObjectsOfType<TNT>();
			foreach (TNT item in array)
			{
				list.Add(item);
			}
			TNTBox[] array2 = Object.FindObjectsOfType<TNTBox>();
			foreach (TNTBox item2 in array2)
			{
				list.Add(item2);
			}
			DynamicTNTBox[] array3 = Object.FindObjectsOfType<DynamicTNTBox>();
			foreach (DynamicTNTBox item3 in array3)
			{
				list.Add(item3);
			}
			foreach (WPFMonoBehaviour item4 in list)
			{
				GameObject obj2 = Object.Instantiate(pointLightPrefab);
				PointLightSource component3 = obj2.GetComponent<PointLightSource>();
				component3.size = 4f;
				component3.canCollide = true;
				component3.canLitObjects = true;
				component3.isEnabled = false;
				obj2.transform.parent = item4.transform;
				obj2.transform.localPosition = Vector3.zero;
			}
			Rocket[] array4 = Object.FindObjectsOfType<Rocket>();
			foreach (Rocket rocket in array4)
			{
				GameObject obj3 = Object.Instantiate(pointLightPrefab);
				PointLightSource component4 = obj3.GetComponent<PointLightSource>();
				component4.size = 2f;
				component4.canCollide = true;
				component4.canLitObjects = true;
				component4.isEnabled = false;
				obj3.transform.parent = rocket.transform;
				obj3.transform.localPosition = Vector3.zero;
			}
			UpdateLights();
		}
		if (newState.state == LevelManager.GameState.Building && newState.prevState == LevelManager.GameState.Running)
		{
			UpdateLights();
		}
	}

	public void UpdateLights(bool waitOneFrame = true)
	{
		if (waitOneFrame)
		{
			StartCoroutine(WaitAndUpdate());
		}
		else
		{
			container.UpdateMeshes();
		}
	}

	private IEnumerator WaitAndUpdate()
	{
		yield return new WaitForEndOfFrame();
		UpdateLights(waitOneFrame: false);
	}
}
