using UnityEngine;

public class SliderToggle : Widget
{
	[SerializeField]
	private CompactEpisodeSelector episodeSelector;

	[SerializeField]
	private float clickThreshold = 1f;

	[SerializeField]
	public AudioSource m_toggleSound;

	private Camera hudCamera;

	private Transform leftIcon;

	private Transform rightIcon;

	private Transform slider;

	private bool isDragging;

	private Vector3 lastDragPosition = Vector3.zero;

	private Vector3 sliderLeftPosition = Vector3.zero;

	private Vector3 sliderRightPosition = Vector3.zero;

	private float pressStartTime;

	private bool toggleStateAtPress;

	private int freezeFrames;

	private bool wasToggled;

	private float lastTimeToggleSoundPlayed;

	private bool IsToggled
	{
		get
		{
			if (episodeSelector != null)
			{
				return episodeSelector.IsRotated;
			}
			return false;
		}
	}

	private void Awake()
	{
		hudCamera = Singleton<GuiManager>.Instance.FindCamera();
		slider = base.transform.Find("Slider");
		leftIcon = base.transform.Find("LeftIcon");
		rightIcon = base.transform.Find("RightIcon");
		if (!(slider == null))
		{
			sliderLeftPosition = slider.localPosition;
			sliderRightPosition = Vector3.up * sliderLeftPosition.y + Vector3.right * Mathf.Abs(sliderLeftPosition.x);
		}
	}

	private void Start()
	{
		slider.localPosition = ((!IsToggled) ? sliderLeftPosition : sliderRightPosition);
	}

	private void Update()
	{
		if (leftIcon != null)
		{
			leftIcon.localScale = Vector3.Lerp(leftIcon.localScale, (!IsToggled) ? (Vector3.one * 1.2f) : Vector3.one, Time.deltaTime * 20f);
		}
		if (rightIcon != null)
		{
			rightIcon.localScale = Vector3.Lerp(rightIcon.localScale, (!IsToggled) ? Vector3.one : (Vector3.one * 1.2f), Time.deltaTime * 20f);
		}
		if (isDragging)
		{
			if (wasToggled != IsToggled && (bool)m_toggleSound && Time.realtimeSinceStartup - lastTimeToggleSoundPlayed > m_toggleSound.clip.length * 0.3f)
			{
				Singleton<AudioManager>.Instance.Play2dEffect(m_toggleSound);
				lastTimeToggleSoundPlayed = Time.realtimeSinceStartup;
			}
			GuiManager.Pointer pointer = GuiManager.GetPointer();
			if (pointer.widget != this)
			{
				OnInput(new InputEvent(InputEvent.EventType.Release, pointer.position));
			}
			wasToggled = IsToggled;
		}
		else
		{
			wasToggled = IsToggled;
			if (freezeFrames > 0)
			{
				freezeFrames--;
			}
			else
			{
				slider.localPosition = Vector3.Lerp(slider.localPosition, (!IsToggled) ? sliderLeftPosition : sliderRightPosition, Time.deltaTime * 20f);
			}
		}
	}

	protected override void OnInput(InputEvent input)
	{
		if (!(slider == null) && (!(episodeSelector != null) || !episodeSelector.IsRotating))
		{
			switch (input.type)
			{
			case InputEvent.EventType.Drag:
				OnDrag(input.position);
				break;
			case InputEvent.EventType.Release:
				OnPress(pressed: false);
				break;
			case InputEvent.EventType.Press:
				OnPress(pressed: true, input.position);
				break;
			}
		}
	}

	private void OnPress(bool pressed, Vector3 position = default(Vector3), bool forceToggle = false)
	{
		if (pressed)
		{
			toggleStateAtPress = IsToggled;
			pressStartTime = Time.realtimeSinceStartup;
			isDragging = true;
			lastDragPosition = position;
			if (episodeSelector != null)
			{
				episodeSelector.PrepareRotation();
			}
		}
		else if (isDragging)
		{
			isDragging = false;
			Vector3 vector = hudCamera.WorldToScreenPoint(base.transform.position);
			bool flag = toggleStateAtPress == IsToggled && Time.realtimeSinceStartup - pressStartTime < clickThreshold && ((!toggleStateAtPress && lastDragPosition.x > vector.x) || (toggleStateAtPress && lastDragPosition.x < vector.x));
			if ((bool)m_toggleSound && flag)
			{
				Singleton<AudioManager>.Instance.Play2dEffect(m_toggleSound);
			}
			if (flag || forceToggle)
			{
				freezeFrames = 4;
			}
			if (episodeSelector != null)
			{
				episodeSelector.ReleaseRotation(flag || forceToggle);
			}
		}
	}

	private void OnDrag(Vector3 position)
	{
		if (isDragging)
		{
			slider.position += Vector3.right * ((position - lastDragPosition) * (20f / (float)Screen.height)).x;
			if (slider.localPosition.x < sliderLeftPosition.x)
			{
				slider.localPosition = sliderLeftPosition;
			}
			else if (slider.localPosition.x > sliderRightPosition.x)
			{
				slider.localPosition = sliderRightPosition;
			}
			lastDragPosition = position;
			if (episodeSelector != null)
			{
				episodeSelector.SetRotation(slider.localPosition.x / sliderRightPosition.x);
			}
		}
	}
}
