using UnityEngine;

[ExecuteInEditMode]
public class CustomSpritePanel : MonoBehaviour
{
	private enum Pieces
	{
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		BottomLeft,
		Left,
		Center
	}

	public Transform[] pieces;

	public Vector3[] pieceOffsets;

	public Transform[] masks;

	public float width = 2f;

	public float height = 2f;

	private bool hasChanged = true;

	private float Width => Mathf.Clamp(width, 3f, 100f) - 1f;

	private float Height => Mathf.Clamp(height, 3f, 100f) - 1f;

	private float HalfWidth => Width * 0.5f;

	private float HalfHeight => Height * 0.5f;

	private void Enable()
	{
		UpdatePieces();
	}

	private void Update()
	{
		UpdatePieces();
	}

	private void UpdatePieces()
	{
		if (!hasChanged || pieces == null || pieceOffsets == null)
		{
			return;
		}
		float num = ((width >= 10f) ? ((width - 10f) * 0.15f) : 0f);
		float num2 = ((height >= 10f) ? ((height - 10f) * 0.15f) : 0f);
		if (pieces.Length != 0 && pieces[0] != null && pieceOffsets.Length != 0)
		{
			pieces[0].localPosition = -Vector3.right * HalfWidth + Vector3.up * HalfHeight + pieceOffsets[0];
		}
		if (pieces.Length > 1 && pieces[1] != null && pieceOffsets.Length > 1)
		{
			pieces[1].localPosition = Vector3.up * HalfHeight + pieceOffsets[1];
		}
		if (pieces.Length > 1 && pieces[1] != null)
		{
			pieces[1].localScale = Vector3.up + Vector3.right * (HalfWidth + num);
		}
		if (pieces.Length > 2 && pieces[2] != null && pieceOffsets.Length > 2)
		{
			pieces[2].localPosition = Vector3.right * HalfWidth + Vector3.up * HalfHeight + pieceOffsets[2];
		}
		if (pieces.Length > 3 && pieces[3] != null && pieceOffsets.Length > 3)
		{
			pieces[3].localPosition = Vector3.right * HalfWidth + pieceOffsets[3];
		}
		if (pieces.Length > 3 && pieces[3] != null)
		{
			pieces[3].localScale = Vector3.up + Vector3.right * (HalfHeight + num2);
		}
		if (pieces.Length > 4 && pieces[4] != null && pieceOffsets.Length > 4)
		{
			pieces[4].localPosition = Vector3.right * HalfWidth - Vector3.up * HalfHeight + pieceOffsets[4];
		}
		if (pieces.Length > 5 && pieces[5] != null && pieceOffsets.Length > 5)
		{
			pieces[5].localPosition = -Vector3.up * HalfHeight + pieceOffsets[5];
		}
		if (pieces.Length > 5 && pieces[5] != null)
		{
			pieces[5].localScale = Vector3.up + Vector3.right * (HalfWidth + num);
		}
		if (pieces.Length > 6 && pieces[6] != null && pieceOffsets.Length > 6)
		{
			pieces[6].localPosition = -Vector3.right * HalfWidth - Vector3.up * HalfHeight + pieceOffsets[6];
		}
		if (pieces.Length > 7 && pieces[7] != null && pieceOffsets.Length > 7)
		{
			pieces[7].localPosition = -Vector3.right * HalfWidth + pieceOffsets[7];
		}
		if (pieces.Length > 7 && pieces[7] != null)
		{
			pieces[7].localScale = Vector3.up + Vector3.right * (HalfHeight + num2);
		}
		if (pieces.Length > 8 && pieces[8] != null && pieceOffsets.Length > 8)
		{
			pieces[8].localPosition = Vector3.zero + pieceOffsets[8];
		}
		if (pieces.Length > 8 && pieces[8] != null)
		{
			pieces[8].localScale = Vector3.right * (HalfWidth + num + 0.2f) + Vector3.up * (HalfHeight + num2 + 0.2f);
		}
		if (masks != null)
		{
			if (masks.Length != 0 && masks[0] != null)
			{
				masks[0].localPosition = Vector3.right * (HalfWidth + 8.2f) - Vector3.forward * 5f;
			}
			if (masks.Length > 1 && masks[1] != null)
			{
				masks[1].localPosition = -Vector3.right * (HalfWidth + 8.2f) - Vector3.forward * 5f;
			}
		}
	}
}
