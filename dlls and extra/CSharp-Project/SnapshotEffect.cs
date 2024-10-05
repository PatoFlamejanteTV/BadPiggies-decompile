using System;
using UnityEngine;

internal class SnapshotEffect : MonoBehaviour
{
	[SerializeField]
	private Shader snapshotShader;

	[SerializeField]
	private AudioSource snapshotSound;

	[SerializeField]
	private float snapshotFadeTime;

	private Material m_snapshotMaterial;

	private float m_snapshotTimer;

	private Action<RenderTexture, RenderTexture> m_renderFunc;

	public event Action SnapshotFinished;

	private void Awake()
	{
		m_snapshotMaterial = new Material(snapshotShader);
		m_snapshotMaterial.hideFlags = HideFlags.HideAndDontSave;
		base.enabled = false;
	}

	private void OnEnable()
	{
		m_renderFunc = SnapshotFunc;
		this.SnapshotFinished = null;
	}

	private void OnDisable()
	{
		if (this.SnapshotFinished != null)
		{
			this.SnapshotFinished();
		}
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(m_snapshotMaterial);
	}

	private void SaveSnapshot()
	{
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		m_renderFunc(source, destination);
	}

	private void SnapshotFunc(RenderTexture source, RenderTexture destination)
	{
		SaveSnapshot();
		if (snapshotSound != null)
		{
			Singleton<AudioManager>.Instance.Play2dEffect(snapshotSound);
		}
		Graphics.Blit(source, destination);
		m_snapshotTimer = snapshotFadeTime;
		m_renderFunc = FadeFunc;
	}

	private void FadeFunc(RenderTexture source, RenderTexture destination)
	{
		float num = m_snapshotTimer / snapshotFadeTime;
		m_snapshotMaterial.SetFloat("_Saturation", 1f - num);
		m_snapshotMaterial.SetFloat("_RampOffset", num);
		Graphics.Blit(source, destination, m_snapshotMaterial);
		m_snapshotTimer -= GameTime.RealTimeDelta;
		base.enabled = m_snapshotTimer > 0f;
	}
}
