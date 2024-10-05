using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBarVisibilityConditions : MonoBehaviour
{
	[Serializable]
	private class ResourceBarItemCondition : IComplexVisibilityObject
	{
		public Action<bool> OnStateChangedEvent;

		[SerializeField]
		private List<ComplexVisibilityManager.Condition> showConditions;

		[SerializeField]
		private List<ComplexVisibilityManager.Condition> hideConditions;

		public List<ComplexVisibilityManager.Condition> ShowConditions => showConditions;

		public List<ComplexVisibilityManager.Condition> HideConditions => hideConditions;

		public void OnStateChanged(bool newState)
		{
			if (OnStateChangedEvent != null)
			{
				OnStateChangedEvent(newState);
			}
		}
	}

	[SerializeField]
	private ResourceBarItemCondition visibilityConditions;

	[SerializeField]
	private ResourceBarItemCondition enableConditions;

	[SerializeField]
	private ResourceBar.Item item;

	private void Start()
	{
		ResourceBarItemCondition resourceBarItemCondition = visibilityConditions;
		resourceBarItemCondition.OnStateChangedEvent = (Action<bool>)Delegate.Combine(resourceBarItemCondition.OnStateChangedEvent, new Action<bool>(OnVisibilityChanged));
		ResourceBarItemCondition resourceBarItemCondition2 = enableConditions;
		resourceBarItemCondition2.OnStateChangedEvent = (Action<bool>)Delegate.Combine(resourceBarItemCondition2.OnStateChangedEvent, new Action<bool>(OnEnableChanged));
		Singleton<ComplexVisibilityManager>.Instance.Subscribe(visibilityConditions, ResourceBar.Instance.IsItemActive(item));
		Singleton<ComplexVisibilityManager>.Instance.Subscribe(enableConditions, ResourceBar.Instance.IsItemEnabled(item));
	}

	private void OnDestroy()
	{
		if (Singleton<ComplexVisibilityManager>.IsInstantiated())
		{
			Singleton<ComplexVisibilityManager>.Instance.Unsubscribe(visibilityConditions);
			Singleton<ComplexVisibilityManager>.Instance.Unsubscribe(enableConditions);
		}
	}

	private void OnVisibilityChanged(bool newState)
	{
		ResourceBar.Instance.ShowItem(item, newState, ResourceBar.Instance.IsItemEnabled(item));
	}

	private void OnEnableChanged(bool newState)
	{
		ResourceBar.Instance.ShowItem(item, ResourceBar.Instance.IsItemActive(item), newState);
	}

	private void OnHideButton(GameObject sender)
	{
		Singleton<ComplexVisibilityManager>.Instance.StateChanged(visibilityConditions, newState: false);
	}

	private void OnShowButton(GameObject sender)
	{
		Singleton<ComplexVisibilityManager>.Instance.StateChanged(visibilityConditions, newState: true);
	}

	private void OnEnableButton(GameObject sender)
	{
		Singleton<ComplexVisibilityManager>.Instance.StateChanged(enableConditions, newState: true);
	}

	private void OnDisableButton(GameObject sender)
	{
		Singleton<ComplexVisibilityManager>.Instance.StateChanged(enableConditions, newState: false);
	}
}
