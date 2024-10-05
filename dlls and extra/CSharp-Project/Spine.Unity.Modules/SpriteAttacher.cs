using UnityEngine;

namespace Spine.Unity.Modules;

public class SpriteAttacher : MonoBehaviour
{
	private const string DefaultPMAShader = "Spine/Skeleton";

	private const string DefaultStraightAlphaShader = "Sprites/Default";

	public bool attachOnStart = true;

	public bool keepLoaderInMemory = true;

	public UnityEngine.Sprite sprite;

	[SpineSlot("", "", false)]
	public string slot;

	private SpriteAttachmentLoader loader;

	private RegionAttachment attachment;

	private bool applyPMA;

	private void Start()
	{
		if (attachOnStart)
		{
			Attach();
		}
	}

	public void Attach()
	{
		ISkeletonComponent component = GetComponent<ISkeletonComponent>();
		SkeletonRenderer skeletonRenderer = component as SkeletonRenderer;
		if (skeletonRenderer != null)
		{
			applyPMA = skeletonRenderer.pmaVertexColors;
		}
		else
		{
			SkeletonGraphic skeletonGraphic = component as SkeletonGraphic;
			if (skeletonGraphic != null)
			{
				applyPMA = skeletonGraphic.SpineMeshGenerator.PremultiplyVertexColors;
			}
		}
		Shader shader = ((!applyPMA) ? Shader.Find("Sprites/Default") : Shader.Find("Spine/Skeleton"));
		loader = loader ?? new SpriteAttachmentLoader(sprite, shader, applyPMA);
		if (attachment == null)
		{
			attachment = loader.NewRegionAttachment(null, sprite.name, string.Empty);
		}
		component.Skeleton.FindSlot(slot).Attachment = attachment;
		if (!keepLoaderInMemory)
		{
			loader = null;
		}
	}
}
