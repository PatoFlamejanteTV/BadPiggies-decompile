using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAnimator : MonoBehaviour
{
	[Serializable]
	public class ObjectState
	{
		[SerializeField]
		public Transform obj;

		[SerializeField]
		public Vector2 pos;

		[SerializeField]
		public bool enabled;
	}

	[Serializable]
	public class AnimState
	{
		[SerializeField]
		public List<ObjectState> objectStates;

		[SerializeField]
		public float length;
	}

	public List<AnimState> animStates;

	private bool animating;

	private void Start()
	{
		StartAnimation();
	}

	private void OnEnable()
	{
		StartAnimation();
	}

	private void OnDisable()
	{
		StopAnimation();
	}

	private void StartAnimation()
	{
		if (!animating)
		{
			StartCoroutine(Animate());
		}
	}

	private void StopAnimation()
	{
		animating = false;
	}

	private IEnumerator Animate()
	{
		animating = true;
		while (animating)
		{
			for (int i = 0; i < animStates.Count; i++)
			{
				for (int j = 0; j < animStates[i].objectStates.Count; j++)
				{
					Transform obj = animStates[i].objectStates[j].obj;
					if (!(obj == null))
					{
						if (animStates[i].objectStates[j].enabled)
						{
							obj.gameObject.SetActive(value: true);
							obj.localPosition = new Vector3(animStates[i].objectStates[j].pos.x, animStates[i].objectStates[j].pos.y, obj.localPosition.z);
						}
						else
						{
							obj.gameObject.SetActive(value: false);
						}
					}
				}
				yield return new WaitForSeconds(animStates[i].length);
			}
		}
	}
}
