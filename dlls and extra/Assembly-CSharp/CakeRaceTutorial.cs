using System.Collections.Generic;
using UnityEngine;

public class CakeRaceTutorial : MonoBehaviour
{
	[SerializeField]
	private GameObject pointerPrefab;

	[SerializeField]
	private GameObject clickPrefab;

	[SerializeField]
	private Transform findOpponentButton;

	[SerializeField]
	private Transform lootCrateSlots;

	private Tutorial.Pointer pointer;

	private Tutorial.PointerTimeLine startTutorial;

	private Tutorial.PointerTimeLine lootCrateTutorial;

	private Tutorial.PointerTimeLine beginOpeningTutorial;

	private bool active;

	private Tutorial.PointerTimeLine currentTutorial;

	private bool StartTutorialShown
	{
		get
		{
			return GameProgress.GetBool("CakeRaceStartTutorialShown");
		}
		set
		{
			GameProgress.SetBool("CakeRaceStartTutorialShown", value);
		}
	}

	private bool LootCrateTutorialShown
	{
		get
		{
			return GameProgress.GetBool("LootCrateTutorialShown");
		}
		set
		{
			GameProgress.SetBool("LootCrateTutorialShown", value);
		}
	}

	private bool BeginOpeningTutorialShown
	{
		get
		{
			return GameProgress.GetBool("BeginOpeninTutorialShown");
		}
		set
		{
			GameProgress.SetBool("BeginOpeninTutorialShown", value);
		}
	}

	private void Awake()
	{
		if (StartTutorialShown && LootCrateTutorialShown && BeginOpeningTutorialShown)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GameObject go = Object.Instantiate(pointerPrefab);
		LayerHelper.SetSortingLayer(go, "Popup", children: true);
		LayerHelper.SetOrderInLayer(go, 0, children: true);
		GameObject gameObject = Object.Instantiate(clickPrefab);
		LayerHelper.SetSortingLayer(gameObject, "Popup", children: true);
		LayerHelper.SetOrderInLayer(gameObject, 0, children: true);
		gameObject.SetActive(value: false);
		pointer = new Tutorial.Pointer(go, gameObject);
		SetActive(active: false);
	}

	private void Start()
	{
		if (!StartTutorialShown)
		{
			StartTutorial();
			StartTutorialShown = true;
		}
		else if (!LootCrateTutorialShown && CakeRaceMenu.WinCount > 0)
		{
			LootCrateTutorial();
			LootCrateTutorialShown = true;
		}
	}

	private void Update()
	{
		if (active && currentTutorial != null)
		{
			currentTutorial.Update();
			if (currentTutorial.IsFinished())
			{
				currentTutorial.Start();
			}
		}
	}

	private void StartTutorial()
	{
		if (!StartTutorialShown)
		{
			currentTutorial = CreateStartTutorial();
			currentTutorial.Start();
			SetActive(active: true);
		}
	}

	private void LootCrateTutorial()
	{
		if (!LootCrateTutorialShown)
		{
			currentTutorial = CreateLootCrateTutorial(lootCrateSlots.FindChildRecursively("Slot1"));
			currentTutorial.Start();
			SetActive(active: true);
		}
	}

	public void OpenCrateTutorial(Transform buttonTf)
	{
		if (!BeginOpeningTutorialShown)
		{
			currentTutorial = CreateOpeningTutorial(buttonTf);
			currentTutorial.Start();
			SetActive(active: true);
		}
	}

	public void SetActive(bool active)
	{
		this.active = active;
		pointer.Show(active);
	}

	private Tutorial.PointerTimeLine CreateStartTutorial()
	{
		Tutorial.PointerTimeLine pointerTimeLine = new Tutorial.PointerTimeLine(pointer);
		List<Vector3> positions = new List<Vector3>
		{
			findOpponentButton.position + 15f * Vector3.down + Vector3.back,
			findOpponentButton.position + Vector3.back
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

	private Tutorial.PointerTimeLine CreateLootCrateTutorial(Transform slotTf)
	{
		Tutorial.PointerTimeLine pointerTimeLine = new Tutorial.PointerTimeLine(pointer);
		List<Vector3> positions = new List<Vector3>
		{
			slotTf.position + 10f * Vector3.down + Vector3.back,
			slotTf.position + Vector3.back
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

	private Tutorial.PointerTimeLine CreateOpeningTutorial(Transform buttonTf)
	{
		Tutorial.PointerTimeLine pointerTimeLine = new Tutorial.PointerTimeLine(pointer);
		List<Vector3> positions = new List<Vector3>
		{
			buttonTf.position + 15f * Vector3.down + Vector3.back,
			buttonTf.position + Vector3.back
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
}
