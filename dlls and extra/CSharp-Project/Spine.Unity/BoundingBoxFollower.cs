using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity;

[ExecuteInEditMode]
public class BoundingBoxFollower : MonoBehaviour
{
	public SkeletonRenderer skeletonRenderer;

	[SpineSlot("", "skeletonRenderer", true)]
	public string slotName;

	public bool isTrigger;

	private Slot slot;

	private BoundingBoxAttachment currentAttachment;

	private string currentAttachmentName;

	private PolygonCollider2D currentCollider;

	private bool valid;

	private bool hasReset;

	public readonly Dictionary<BoundingBoxAttachment, PolygonCollider2D> colliderTable = new Dictionary<BoundingBoxAttachment, PolygonCollider2D>();

	public readonly Dictionary<BoundingBoxAttachment, string> attachmentNameTable = new Dictionary<BoundingBoxAttachment, string>();

	public Slot Slot => slot;

	public BoundingBoxAttachment CurrentAttachment => currentAttachment;

	public string CurrentAttachmentName => currentAttachmentName;

	public PolygonCollider2D CurrentCollider => currentCollider;

	public bool IsTrigger => isTrigger;

	private void OnEnable()
	{
		ClearColliders();
		if (skeletonRenderer == null)
		{
			skeletonRenderer = GetComponentInParent<SkeletonRenderer>();
		}
		if (skeletonRenderer != null)
		{
			SkeletonRenderer obj = skeletonRenderer;
			obj.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Remove(obj.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleRebuild));
			SkeletonRenderer obj2 = skeletonRenderer;
			obj2.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Combine(obj2.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleRebuild));
			if (hasReset)
			{
				HandleRebuild(skeletonRenderer);
			}
		}
	}

	private void OnDisable()
	{
		SkeletonRenderer obj = skeletonRenderer;
		obj.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Remove(obj.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleRebuild));
	}

	private void Start()
	{
		if (!hasReset && skeletonRenderer != null)
		{
			HandleRebuild(skeletonRenderer);
		}
	}

	public void HandleRebuild(SkeletonRenderer renderer)
	{
		if (string.IsNullOrEmpty(slotName))
		{
			return;
		}
		hasReset = true;
		ClearColliders();
		colliderTable.Clear();
		if (skeletonRenderer.skeleton == null)
		{
			SkeletonRenderer obj = skeletonRenderer;
			obj.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Remove(obj.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleRebuild));
			skeletonRenderer.Initialize(overwrite: false);
			SkeletonRenderer obj2 = skeletonRenderer;
			obj2.OnRebuild = (SkeletonRenderer.SkeletonRendererDelegate)Delegate.Combine(obj2.OnRebuild, new SkeletonRenderer.SkeletonRendererDelegate(HandleRebuild));
		}
		Skeleton skeleton = skeletonRenderer.skeleton;
		slot = skeleton.FindSlot(slotName);
		int slotIndex = skeleton.FindSlotIndex(slotName);
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		foreach (Skin skin in skeleton.Data.Skins)
		{
			List<string> list = new List<string>();
			skin.FindNamesForSlot(slotIndex, list);
			foreach (string item in list)
			{
				if (skin.GetAttachment(slotIndex, item) is BoundingBoxAttachment boundingBoxAttachment)
				{
					PolygonCollider2D polygonCollider2D = SkeletonUtility.AddBoundingBoxAsComponent(boundingBoxAttachment, base.gameObject);
					polygonCollider2D.enabled = false;
					polygonCollider2D.hideFlags = HideFlags.NotEditable;
					polygonCollider2D.isTrigger = IsTrigger;
					colliderTable.Add(boundingBoxAttachment, polygonCollider2D);
					attachmentNameTable.Add(boundingBoxAttachment, item);
				}
			}
		}
	}

	private void ClearColliders()
	{
		PolygonCollider2D[] components = GetComponents<PolygonCollider2D>();
		if (components.Length == 0)
		{
			return;
		}
		PolygonCollider2D[] array = components;
		foreach (PolygonCollider2D polygonCollider2D in array)
		{
			if (polygonCollider2D != null)
			{
				UnityEngine.Object.Destroy(polygonCollider2D);
			}
		}
		colliderTable.Clear();
		attachmentNameTable.Clear();
	}

	private void LateUpdate()
	{
		if (skeletonRenderer.valid && slot != null && slot.Attachment != currentAttachment)
		{
			MatchAttachment(slot.Attachment);
		}
	}

	private void MatchAttachment(Attachment attachment)
	{
		BoundingBoxAttachment boundingBoxAttachment = attachment as BoundingBoxAttachment;
		if (currentCollider != null)
		{
			currentCollider.enabled = false;
		}
		if (boundingBoxAttachment == null)
		{
			currentCollider = null;
		}
		else
		{
			currentCollider = colliderTable[boundingBoxAttachment];
			currentCollider.enabled = true;
		}
		currentAttachment = boundingBoxAttachment;
		currentAttachmentName = ((currentAttachment != null) ? attachmentNameTable[boundingBoxAttachment] : null);
	}
}
