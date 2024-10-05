using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkshopTutorial : WPFMonoBehaviour
{
	private enum Mode
	{
		None,
		WatchPopup,
		ScrapInsert,
		StartMachine,
		Finished
	}

	private const string FINISHED_TUTORIAL = "Workshop_Tutorial";

	[SerializeField]
	public GameObject pointerPrefab;

	[SerializeField]
	public GameObject clickPrefab;

	private Tutorial.Pointer pointer;

	private Tutorial.PointerTimeLine scrapInsertTutorial;

	private Tutorial.PointerTimeLine startMachineTutorial;

	private Tutorial.PointerTimeLine timeLine;

	private Mode mode;

	private bool playing;

	private bool initialized;

	private bool stop;

	private bool popupWatched;

	private bool machineReady;

	private void Awake()
	{
		stop = false;
		GameObject gameObject = UnityEngine.Object.Instantiate(pointerPrefab);
		GameObject gameObject2 = UnityEngine.Object.Instantiate(clickPrefab);
		gameObject2.SetActive(value: false);
		EventManager.Connect<WorkshopMenu.CraftingMachineEvent>(OnCraftingMachineEvent);
		WorkshopIntro.OnPressedOk = (Action)Delegate.Combine(WorkshopIntro.OnPressedOk, new Action(OnPopupWatched));
		pointer = new Tutorial.Pointer(gameObject, gameObject2);
		SetupTutorials();
	}

	private void OnDestroy()
	{
		WorkshopIntro.OnPressedOk = (Action)Delegate.Remove(WorkshopIntro.OnPressedOk, new Action(OnPopupWatched));
		EventManager.Disconnect<WorkshopMenu.CraftingMachineEvent>(OnCraftingMachineEvent);
	}

	private void OnCraftingMachineEvent(WorkshopMenu.CraftingMachineEvent data)
	{
		switch (data.action)
		{
		case WorkshopMenu.CraftingMachineAction.Idle:
			machineReady = true;
			break;
		case WorkshopMenu.CraftingMachineAction.ResetScrap:
			if (mode == Mode.StartMachine)
			{
				GameProgress.SetInt("Workshop_Tutorial", 1);
				SwitchMode(Mode.ScrapInsert);
			}
			break;
		case WorkshopMenu.CraftingMachineAction.AddScrap:
			if (mode == Mode.ScrapInsert)
			{
				GameProgress.SetInt("Workshop_Tutorial", 2);
				SwitchMode(Mode.StartMachine);
			}
			break;
		case WorkshopMenu.CraftingMachineAction.CraftPart:
			if (mode == Mode.StartMachine)
			{
				GameProgress.SetInt("Workshop_Tutorial", 3);
				SwitchMode(Mode.Finished);
			}
			break;
		case WorkshopMenu.CraftingMachineAction.RemoveScrap:
			break;
		}
	}

	private bool CanBegin()
	{
		if (machineReady)
		{
			return popupWatched;
		}
		return false;
	}

	private void OnPopupWatched()
	{
		GameProgress.SetBool("Workshop_Visited", value: true);
		GameProgress.SetInt("Workshop_Tutorial", 1);
		popupWatched = true;
	}

	private void SetupTutorials()
	{
		if (initialized)
		{
			return;
		}
		mode = (Mode)GameProgress.GetInt("Workshop_Tutorial");
		switch (mode)
		{
		case Mode.None:
			scrapInsertTutorial = ScrapInsertTutorial();
			startMachineTutorial = StartMachineTutorial();
			pointer.Show(show: false);
			if (!GameProgress.GetBool("Workshop_Visited"))
			{
				UnityEngine.Object.Instantiate(WPFMonoBehaviour.gameData.m_workshopIntroduction).transform.position = new Vector3(0f, 0f, -5f);
			}
			else
			{
				OnPopupWatched();
			}
			StartCoroutine(WaitFor(() => machineReady && popupWatched, delegate
			{
				SwitchMode(Mode.ScrapInsert);
			}));
			break;
		case Mode.WatchPopup:
			scrapInsertTutorial = ScrapInsertTutorial();
			startMachineTutorial = StartMachineTutorial();
			pointer.Show(show: false);
			StartCoroutine(WaitFor(() => machineReady, delegate
			{
				SwitchMode(Mode.ScrapInsert);
			}));
			break;
		case Mode.ScrapInsert:
			scrapInsertTutorial = ScrapInsertTutorial();
			startMachineTutorial = StartMachineTutorial();
			pointer.Show(show: false);
			StartCoroutine(WaitFor(() => machineReady, delegate
			{
				SwitchMode(Mode.StartMachine);
			}));
			break;
		case Mode.StartMachine:
			UnityEngine.Object.Destroy(base.gameObject);
			break;
		}
		initialized = true;
	}

	private Tutorial.PointerTimeLine ScrapInsertTutorial()
	{
		Tutorial.PointerTimeLine pointerTimeLine = new Tutorial.PointerTimeLine(pointer);
		GameObject gameObject = GameObject.Find("AddScrap");
		List<Vector3> positions = new List<Vector3>
		{
			gameObject.transform.position + 21f * Vector3.down,
			gameObject.transform.position
		};
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Wait(0.1f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Move(positions, 2.5f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Press());
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Wait(0.5f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Release());
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Wait(0.75f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Hide());
		return pointerTimeLine;
	}

	private Tutorial.PointerTimeLine StartMachineTutorial()
	{
		Tutorial.PointerTimeLine pointerTimeLine = new Tutorial.PointerTimeLine(pointer);
		GameObject gameObject = GameObject.Find("CrankLever");
		List<Vector3> positions = new List<Vector3>
		{
			gameObject.transform.position + 12f * Vector3.down,
			gameObject.transform.position
		};
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Wait(0.1f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Move(positions, 2.5f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Press());
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Wait(0.5f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Release());
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Wait(0.75f));
		pointerTimeLine.AddEvent(new Tutorial.PointerTimeLine.Hide());
		return pointerTimeLine;
	}

	private void Update()
	{
		if (playing)
		{
			timeLine.Update();
		}
		if (stop && timeLine.IsFinished())
		{
			playing = false;
		}
		if (playing && timeLine.IsFinished())
		{
			timeLine.Start();
		}
		if (!playing)
		{
			pointer.Show(show: false);
		}
	}

	private void SwitchMode(Mode mode)
	{
		if (this.mode != mode)
		{
			this.mode = mode;
			switch (mode)
			{
			case Mode.Finished:
				playing = false;
				break;
			case Mode.StartMachine:
				timeLine = startMachineTutorial;
				timeLine.Start();
				playing = true;
				break;
			case Mode.ScrapInsert:
				timeLine = scrapInsertTutorial;
				timeLine.Start();
				playing = true;
				break;
			}
		}
	}

	private IEnumerator WaitFor(Func<bool> wait, Action exec)
	{
		while (!wait())
		{
			yield return null;
		}
		exec?.Invoke();
	}
}
