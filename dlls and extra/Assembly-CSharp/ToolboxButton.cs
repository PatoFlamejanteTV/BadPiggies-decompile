using System.Collections.Generic;
using UnityEngine;

public class ToolboxButton : SliderButton
{
	public List<GameObject> m_toggleList = new List<GameObject>();

	public GameObject m_button;

	public GameObject m_scrollLeftButton;

	private bool isButtonOut;

	private bool lastIsPlaying;

	private const string AnimName = "ToolBoxButtonSlide";

	private const string ToolBoxAnimName = "ToolBoxButton";

	private bool m_powerupTutorialShown;

	private bool m_openList;

	public bool ToolboxOpen => isButtonOut;

	private void OnEnable()
	{
		InitAnimationStates(isButtonOut, GetComponent<Animation>()["ToolBoxButtonSlide"], m_button.GetComponent<Animation>()["ToolBoxButton"]);
		m_button.transform.Find("Gear").transform.rotation = Quaternion.identity;
		EnableRendererRecursively(base.gameObject, isButtonOut);
		ActivateToggleList(!isButtonOut);
		m_powerupTutorialShown = GameProgress.GetBool("PowerupTutorialShown");
	}

	private void Reset()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			Vector3 localPosition = child.localPosition;
			localPosition.x = (localPosition.y = 0f);
			child.localPosition = localPosition;
			child.localRotation = Quaternion.identity;
			child.localScale = Vector3.one * 0.2f;
		}
		isButtonOut = false;
	}

	public void OnPressed()
	{
		if (!GetComponent<Animation>().isPlaying)
		{
			EnableRendererRecursively(base.gameObject, enable: true);
			bool reverse = isButtonOut;
			InitAnimationStates(reverse, GetComponent<Animation>()["ToolBoxButtonSlide"], m_button.GetComponent<Animation>()["ToolBoxButton"]);
			GetComponent<Animation>().Play();
			m_button.GetComponent<Animation>().Play();
			isButtonOut = !isButtonOut;
			if (isButtonOut)
			{
				ActivateToggleList(state: false);
			}
			if (!m_powerupTutorialShown && !WPFMonoBehaviour.levelManager.m_showPowerupTutorial)
			{
				WPFMonoBehaviour.levelManager.m_showPowerupTutorial = true;
				EventManager.Send(new UIEvent(UIEvent.Type.OpenTutorial));
				m_openList = true;
			}
		}
	}

	private void Update()
	{
		if (!GetComponent<Animation>().isPlaying && lastIsPlaying && !isButtonOut)
		{
			ActivateToggleList(state: true);
		}
		if (m_openList)
		{
			Reset();
			OnPressed();
			m_openList = false;
		}
		lastIsPlaying = GetComponent<Animation>().isPlaying;
	}

	private void ActivateToggleList(bool state)
	{
		foreach (GameObject toggle in m_toggleList)
		{
			toggle.SetActive(state);
		}
	}

	private void InitAnimationStates(bool reverse, params AnimationState[] states)
	{
		foreach (AnimationState animationState in states)
		{
			animationState.speed = ((!reverse) ? 1 : (-1));
			animationState.time = ((!reverse) ? 0f : animationState.length);
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

	public void OpenMenu()
	{
		ActivateToggleList(state: true);
	}
}
