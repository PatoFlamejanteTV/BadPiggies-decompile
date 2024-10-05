using System;
using System.Collections.Generic;
using UnityEngine;

public class PartListingScrollbar : MonoBehaviour
{
	[SerializeField]
	private GameObject newBtnPrefab;

	[SerializeField]
	private Transform scrollButtonTf;

	[SerializeField]
	private PartListing partListing;

	[SerializeField]
	private PartListingScrollbutton scrollButton;

	[SerializeField]
	private float moveArea;

	private Dictionary<int, Tuple<GameObject, float>> newButtons;

	private bool draggingButton;

	private bool waitingPartList;

	private float lastPartListMovement;

	private void OnEnable()
	{
		PartListing obj = partListing;
		obj.OnPartListingMoved = (Action<float>)Delegate.Combine(obj.OnPartListingMoved, new Action<float>(OnPartListMoved));
		PartListing obj2 = partListing;
		obj2.OnPartListDragBegin = (Action)Delegate.Combine(obj2.OnPartListDragBegin, new Action(OnPartListDragBegin));
		PartListingScrollbutton partListingScrollbutton = scrollButton;
		partListingScrollbutton.OnDragBegin = (Action)Delegate.Combine(partListingScrollbutton.OnDragBegin, new Action(OnButtonDragBegin));
		PartListingScrollbutton partListingScrollbutton2 = scrollButton;
		partListingScrollbutton2.OnDrag = (Action<float>)Delegate.Combine(partListingScrollbutton2.OnDrag, new Action<float>(OnButtonDrag));
		PartListingScrollbutton partListingScrollbutton3 = scrollButton;
		partListingScrollbutton3.OnDragEnd = (Action)Delegate.Combine(partListingScrollbutton3.OnDragEnd, new Action(OnButtonDragEnd));
	}

	private void OnDisable()
	{
		PartListing obj = partListing;
		obj.OnPartListingMoved = (Action<float>)Delegate.Remove(obj.OnPartListingMoved, new Action<float>(OnPartListMoved));
		PartListing obj2 = partListing;
		obj2.OnPartListDragBegin = (Action)Delegate.Remove(obj2.OnPartListDragBegin, new Action(OnPartListDragBegin));
		PartListingScrollbutton partListingScrollbutton = scrollButton;
		partListingScrollbutton.OnDragBegin = (Action)Delegate.Remove(partListingScrollbutton.OnDragBegin, new Action(OnButtonDragBegin));
		PartListingScrollbutton partListingScrollbutton2 = scrollButton;
		partListingScrollbutton2.OnDrag = (Action<float>)Delegate.Remove(partListingScrollbutton2.OnDrag, new Action<float>(OnButtonDrag));
		PartListingScrollbutton partListingScrollbutton3 = scrollButton;
		partListingScrollbutton3.OnDragEnd = (Action)Delegate.Remove(partListingScrollbutton3.OnDragEnd, new Action(OnButtonDragEnd));
	}

	private void OnPartListMoved(float relativePosition)
	{
		if (waitingPartList)
		{
			waitingPartList = Mathf.Abs(relativePosition - lastPartListMovement) > float.Epsilon;
			lastPartListMovement = relativePosition;
		}
		else if (!draggingButton)
		{
			float x = (relativePosition - 0.5f) * moveArea;
			scrollButtonTf.localPosition = new Vector3(x, 0f, -1f);
		}
	}

	private void OnButtonDragBegin()
	{
		draggingButton = true;
	}

	private void OnButtonDrag(float x)
	{
		scrollButtonTf.localPosition = new Vector3(Mathf.Clamp(x, (0f - moveArea) / 2f, moveArea / 2f), 0f, -1f);
		partListing.SetRelativePosition(Mathf.Clamp01((x + moveArea / 2f) / moveArea));
	}

	private void OnButtonDragEnd()
	{
		draggingButton = false;
		waitingPartList = true;
	}

	private void OnPartListDragBegin()
	{
		waitingPartList = false;
		draggingButton = false;
	}

	public GameObject SetNewPartButton(float relativePosition)
	{
		if (newButtons == null)
		{
			newButtons = new Dictionary<int, Tuple<GameObject, float>>();
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(newBtnPrefab);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = new Vector3((relativePosition - 0.5f) * moveArea, 0f, -0.5f);
		gameObject.GetComponent<Button>().MethodToCall.SetMethod(this, "OnNewPartButtonPressed", gameObject.GetInstanceID());
		newButtons.Add(gameObject.GetInstanceID(), new Tuple<GameObject, float>(gameObject, relativePosition));
		return gameObject;
	}

	public void ClearNewPartButtons()
	{
		if (newButtons == null)
		{
			return;
		}
		foreach (KeyValuePair<int, Tuple<GameObject, float>> newButton in newButtons)
		{
			UnityEngine.Object.Destroy(newButtons[newButton.Key].Item1);
		}
		newButtons.Clear();
	}

	public void OnNewPartButtonPressed(int id)
	{
		if (newButtons != null && newButtons.ContainsKey(id))
		{
			partListing.SetRelativePosition(newButtons[id].Item2);
		}
	}
}
