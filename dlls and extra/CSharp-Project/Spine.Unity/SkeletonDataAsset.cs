using System;
using System.IO;
using UnityEngine;

namespace Spine.Unity;

public class SkeletonDataAsset : ScriptableObject
{
	public AtlasAsset[] atlasAssets;

	public float scale = 0.01f;

	public TextAsset skeletonJSON;

	public string[] fromAnimation;

	public string[] toAnimation;

	public float[] duration;

	public float defaultMix;

	public RuntimeAnimatorController controller;

	private SkeletonData skeletonData;

	private AnimationStateData stateData;

	private void OnEnable()
	{
		if (atlasAssets == null)
		{
			atlasAssets = new AtlasAsset[0];
		}
	}

	public void Reset()
	{
		skeletonData = null;
		stateData = null;
	}

	public SkeletonData GetSkeletonData(bool quiet)
	{
		if (atlasAssets == null)
		{
			atlasAssets = new AtlasAsset[0];
			Reset();
			return null;
		}
		if (skeletonJSON == null)
		{
			Reset();
			return null;
		}
		if (atlasAssets.Length == 0)
		{
			Reset();
			return null;
		}
		Atlas[] array = new Atlas[atlasAssets.Length];
		for (int i = 0; i < atlasAssets.Length; i++)
		{
			if (atlasAssets[i] == null)
			{
				Reset();
				return null;
			}
			array[i] = atlasAssets[i].GetAtlas();
			if (array[i] == null)
			{
				Reset();
				return null;
			}
		}
		if (skeletonData != null)
		{
			return skeletonData;
		}
		AttachmentLoader attachmentLoader = new AtlasAttachmentLoader(array);
		float num = scale;
		try
		{
			if (skeletonJSON.name.ToLower().Contains(".skel"))
			{
				MemoryStream input = new MemoryStream(skeletonJSON.bytes);
				skeletonData = new SkeletonBinary(attachmentLoader)
				{
					Scale = num
				}.ReadSkeletonData(input);
			}
			else
			{
				StringReader reader = new StringReader(skeletonJSON.text);
				skeletonData = new SkeletonJson(attachmentLoader)
				{
					Scale = num
				}.ReadSkeletonData(reader);
			}
		}
		catch (Exception)
		{
			return null;
		}
		stateData = new AnimationStateData(skeletonData);
		FillStateData();
		return skeletonData;
	}

	public void FillStateData()
	{
		if (stateData == null)
		{
			return;
		}
		stateData.DefaultMix = defaultMix;
		if (fromAnimation == null || toAnimation == null)
		{
			return;
		}
		int i = 0;
		for (int num = fromAnimation.Length; i < num; i++)
		{
			if (fromAnimation[i].Length != 0 && toAnimation[i].Length != 0)
			{
				stateData.SetMix(fromAnimation[i], toAnimation[i], duration[i]);
			}
		}
	}

	public AnimationStateData GetAnimationStateData()
	{
		if (stateData != null)
		{
			return stateData;
		}
		GetSkeletonData(quiet: false);
		return stateData;
	}
}
