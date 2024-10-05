using System;
using System.Collections;
using UnityEngine;

public class MaterialAnimation : MonoBehaviour
{
	[Serializable]
	public class AnimationNode
	{
		[SerializeField]
		private Color color = Color.white;

		[SerializeField]
		private float time = 0.1f;

		public Color Color => color;

		public float Time => time;
	}

	[SerializeField]
	private AnimationNode[] animationNodes;

	[SerializeField]
	private bool smoothTransition;

	[SerializeField]
	private bool autoStart;

	[SerializeField]
	private int autoStartLoopCount;

	private Material animationMaterial;

	private Material originalMaterial;

	private int loopsLeft = -1;

	private bool isLooping;

	private Renderer renderer;

	private void Awake()
	{
		renderer = GetComponent<Renderer>();
		if (!(renderer == null) && !(renderer.sharedMaterial == null))
		{
			originalMaterial = renderer.sharedMaterial;
			animationMaterial = new Material(renderer.sharedMaterial.shader);
			animationMaterial.CopyPropertiesFromMaterial(renderer.sharedMaterial);
			AtlasMaterials.Instance.AddMaterialInstance(animationMaterial);
		}
	}

	private void OnDestroy()
	{
		if (animationMaterial != null && AtlasMaterials.IsInstantiated)
		{
			AtlasMaterials.Instance.AddMaterialInstance(animationMaterial);
		}
	}

	private void Start()
	{
		if (autoStart)
		{
			PlayAnimation(loop: true, (autoStartLoopCount >= 0) ? autoStartLoopCount : int.MaxValue);
		}
	}

	private IEnumerator Loop()
	{
		if (isLooping || animationNodes == null || animationNodes.Length == 0 || animationMaterial == null)
		{
			yield break;
		}
		renderer.sharedMaterial = animationMaterial;
		isLooping = true;
		Color prevColor = animationNodes[0].Color;
		do
		{
			for (int i = 0; i < animationNodes.Length; i++)
			{
				if (!smoothTransition)
				{
					animationMaterial.color = animationNodes[i].Color;
				}
				float time = animationNodes[i].Time;
				if (Mathf.Approximately(time, 0f) || time < 0f)
				{
					time = 0.01f;
				}
				float waitTime = time;
				while (waitTime > 0f)
				{
					if (smoothTransition)
					{
						animationMaterial.color = Color.Lerp(prevColor, animationNodes[i].Color, waitTime / time);
					}
					waitTime -= GameTime.RealTimeDelta;
					yield return null;
				}
				prevColor = animationNodes[i].Color;
			}
			loopsLeft--;
		}
		while (loopsLeft >= 0);
		isLooping = false;
		renderer.sharedMaterial = originalMaterial;
	}

	public void PlayAnimation(bool loop = false, int loopCount = 0)
	{
		loopsLeft = loopCount;
		StartCoroutine(Loop());
	}
}
