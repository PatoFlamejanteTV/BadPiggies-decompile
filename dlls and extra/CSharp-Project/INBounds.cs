public struct INBounds
{
	public int Type;

	public float X;

	public float Y;

	public float A;

	public float B;

	public float R;

	public bool IsRect => Type == 0;

	public bool IsSphere => Type == 1;

	public float GetHalfProjection(float dX, float dY)
	{
		if (Type == 0)
		{
			return ((dX > 0f) ? dX : (0f - dX)) * A + ((dY > 0f) ? dY : (0f - dY)) * B;
		}
		return R;
	}
}
