using System;
using UnityEngine;

public class Levitation : WPFMonoBehaviour
{
	[SerializeField]
	private float magnitude;

	[SerializeField]
	private float speed = 1f;

	private Vector3 original;

	private Transform tf;

	private float t;

	private float max;

	private void Awake()
	{
		tf = base.transform;
		original = tf.localPosition;
		max = (float)Math.PI * 2f;
	}

	private void Update()
	{
		t += Time.unscaledDeltaTime * speed;
		t %= max;
		tf.localPosition = original + new Vector3(0f, Mathf.Sin(t) * magnitude);
	}
}
