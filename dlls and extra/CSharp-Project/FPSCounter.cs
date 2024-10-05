using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
	private Text m_fps;

	private float updateInterval = 0.5f;

	private float frames;

	private float timeleft;

	private Stopwatch m_stopwatch = new Stopwatch();

	private void Start()
	{
		base.enabled = false;
	}

	private void Update()
	{
		timeleft -= Time.unscaledDeltaTime;
		frames += 1f;
		if (timeleft <= 0f)
		{
			float num = (float)m_stopwatch.ElapsedMilliseconds / 1000f;
			float num2 = frames / num;
			m_fps.text = num2.ToString("f2");
			timeleft = updateInterval;
			frames = 0f;
			m_stopwatch.Reset();
			m_stopwatch.Start();
		}
	}
}
