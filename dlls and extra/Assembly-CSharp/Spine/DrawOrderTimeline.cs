namespace Spine;

public class DrawOrderTimeline : Timeline
{
	internal float[] frames;

	private int[][] drawOrders;

	public float[] Frames
	{
		get
		{
			return frames;
		}
		set
		{
			frames = value;
		}
	}

	public int[][] DrawOrders
	{
		get
		{
			return drawOrders;
		}
		set
		{
			drawOrders = value;
		}
	}

	public int FrameCount => frames.Length;

	public DrawOrderTimeline(int frameCount)
	{
		frames = new float[frameCount];
		drawOrders = new int[frameCount][];
	}

	public void SetFrame(int frameIndex, float time, int[] drawOrder)
	{
		frames[frameIndex] = time;
		drawOrders[frameIndex] = drawOrder;
	}

	public void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha)
	{
		float[] array = frames;
		if (time < array[0])
		{
			return;
		}
		int num = ((!(time >= array[^1])) ? (Animation.binarySearch(array, time) - 1) : (array.Length - 1));
		ExposedList<Slot> drawOrder = skeleton.drawOrder;
		ExposedList<Slot> slots = skeleton.slots;
		int[] array2 = drawOrders[num];
		if (array2 == null)
		{
			drawOrder.Clear();
			int i = 0;
			for (int count = slots.Count; i < count; i++)
			{
				drawOrder.Add(slots.Items[i]);
			}
			return;
		}
		Slot[] items = drawOrder.Items;
		Slot[] items2 = slots.Items;
		int j = 0;
		for (int num2 = array2.Length; j < num2; j++)
		{
			items[j] = items2[array2[j]];
		}
	}
}
