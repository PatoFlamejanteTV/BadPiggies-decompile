using System;
using System.Text;

namespace Spine;

public class AnimationState
{
	public delegate void StartEndDelegate(AnimationState state, int trackIndex);

	public delegate void EventDelegate(AnimationState state, int trackIndex, Event e);

	public delegate void CompleteDelegate(AnimationState state, int trackIndex, int loopCount);

	private AnimationStateData data;

	private ExposedList<TrackEntry> tracks = new ExposedList<TrackEntry>();

	private ExposedList<Event> events = new ExposedList<Event>();

	private float timeScale = 1f;

	public AnimationStateData Data => data;

	public ExposedList<TrackEntry> Tracks => tracks;

	public float TimeScale
	{
		get
		{
			return timeScale;
		}
		set
		{
			timeScale = value;
		}
	}

	public event StartEndDelegate Start;

	public event StartEndDelegate End;

	public event EventDelegate Event;

	public event CompleteDelegate Complete;

	public AnimationState(AnimationStateData data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data", "data cannot be null.");
		}
		this.data = data;
	}

	public void Update(float delta)
	{
		delta *= timeScale;
		for (int i = 0; i < tracks.Count; i++)
		{
			TrackEntry trackEntry = tracks.Items[i];
			if (trackEntry == null)
			{
				continue;
			}
			float num = delta * trackEntry.timeScale;
			float num2 = trackEntry.time + num;
			float endTime = trackEntry.endTime;
			trackEntry.time = num2;
			if (trackEntry.previous != null)
			{
				trackEntry.previous.time += num;
				trackEntry.mixTime += num;
			}
			bool num3;
			if (trackEntry.loop)
			{
				num3 = trackEntry.lastTime % endTime > num2 % endTime;
			}
			else
			{
				if (!(trackEntry.lastTime < endTime))
				{
					goto IL_00c7;
				}
				num3 = num2 >= endTime;
			}
			if (num3)
			{
				int loopCount = (int)(num2 / endTime);
				trackEntry.OnComplete(this, i, loopCount);
				if (this.Complete != null)
				{
					this.Complete(this, i, loopCount);
				}
			}
			goto IL_00c7;
			IL_00c7:
			TrackEntry next = trackEntry.next;
			if (next != null)
			{
				next.time = trackEntry.lastTime - next.delay;
				if (next.time >= 0f)
				{
					SetCurrent(i, next);
				}
			}
			else if (!trackEntry.loop && trackEntry.lastTime >= trackEntry.endTime)
			{
				ClearTrack(i);
			}
		}
	}

	public void Apply(Skeleton skeleton)
	{
		ExposedList<Event> exposedList = events;
		for (int i = 0; i < tracks.Count; i++)
		{
			TrackEntry trackEntry = tracks.Items[i];
			if (trackEntry == null)
			{
				continue;
			}
			exposedList.Clear();
			float num = trackEntry.time;
			bool loop = trackEntry.loop;
			if (!loop && num > trackEntry.endTime)
			{
				num = trackEntry.endTime;
			}
			TrackEntry previous = trackEntry.previous;
			if (previous == null)
			{
				if (trackEntry.mix == 1f)
				{
					trackEntry.animation.Apply(skeleton, trackEntry.lastTime, num, loop, exposedList);
				}
				else
				{
					trackEntry.animation.Mix(skeleton, trackEntry.lastTime, num, loop, exposedList, trackEntry.mix);
				}
			}
			else
			{
				float num2 = previous.time;
				if (!previous.loop && num2 > previous.endTime)
				{
					num2 = previous.endTime;
				}
				previous.animation.Apply(skeleton, previous.lastTime, num2, previous.loop, null);
				previous.lastTime = num2;
				float num3 = trackEntry.mixTime / trackEntry.mixDuration * trackEntry.mix;
				if (num3 >= 1f)
				{
					num3 = 1f;
					trackEntry.previous = null;
				}
				trackEntry.animation.Mix(skeleton, trackEntry.lastTime, num, loop, exposedList, num3);
			}
			int j = 0;
			for (int count = exposedList.Count; j < count; j++)
			{
				Event e = exposedList.Items[j];
				trackEntry.OnEvent(this, i, e);
				if (this.Event != null)
				{
					this.Event(this, i, e);
				}
			}
			trackEntry.lastTime = trackEntry.time;
		}
	}

	public void ClearTracks()
	{
		int i = 0;
		for (int count = tracks.Count; i < count; i++)
		{
			ClearTrack(i);
		}
		tracks.Clear();
	}

	public void ClearTrack(int trackIndex)
	{
		if (trackIndex >= tracks.Count)
		{
			return;
		}
		TrackEntry trackEntry = tracks.Items[trackIndex];
		if (trackEntry != null)
		{
			trackEntry.OnEnd(this, trackIndex);
			if (this.End != null)
			{
				this.End(this, trackIndex);
			}
			tracks.Items[trackIndex] = null;
		}
	}

	private TrackEntry ExpandToIndex(int index)
	{
		if (index < tracks.Count)
		{
			return tracks.Items[index];
		}
		while (index >= tracks.Count)
		{
			tracks.Add(null);
		}
		return null;
	}

	private void SetCurrent(int index, TrackEntry entry)
	{
		TrackEntry trackEntry = ExpandToIndex(index);
		if (trackEntry != null)
		{
			TrackEntry previous = trackEntry.previous;
			trackEntry.previous = null;
			trackEntry.OnEnd(this, index);
			if (this.End != null)
			{
				this.End(this, index);
			}
			entry.mixDuration = data.GetMix(trackEntry.animation, entry.animation);
			if (entry.mixDuration > 0f)
			{
				entry.mixTime = 0f;
				if (previous != null && trackEntry.mixTime / trackEntry.mixDuration < 0.5f)
				{
					entry.previous = previous;
				}
				else
				{
					entry.previous = trackEntry;
				}
			}
		}
		tracks.Items[index] = entry;
		entry.OnStart(this, index);
		if (this.Start != null)
		{
			this.Start(this, index);
		}
	}

	public TrackEntry SetAnimation(int trackIndex, string animationName, bool loop)
	{
		Animation animation = data.skeletonData.FindAnimation(animationName);
		if (animation == null)
		{
			throw new ArgumentException("Animation not found: " + animationName, "animationName");
		}
		return SetAnimation(trackIndex, animation, loop);
	}

	public TrackEntry SetAnimation(int trackIndex, Animation animation, bool loop)
	{
		if (animation == null)
		{
			throw new ArgumentNullException("animation", "animation cannot be null.");
		}
		TrackEntry trackEntry = new TrackEntry();
		trackEntry.animation = animation;
		trackEntry.loop = loop;
		trackEntry.time = 0f;
		trackEntry.endTime = animation.Duration;
		SetCurrent(trackIndex, trackEntry);
		return trackEntry;
	}

	public TrackEntry AddAnimation(int trackIndex, string animationName, bool loop, float delay)
	{
		Animation animation = data.skeletonData.FindAnimation(animationName);
		if (animation == null)
		{
			throw new ArgumentException("Animation not found: " + animationName, "animationName");
		}
		return AddAnimation(trackIndex, animation, loop, delay);
	}

	public TrackEntry AddAnimation(int trackIndex, Animation animation, bool loop, float delay)
	{
		if (animation == null)
		{
			throw new ArgumentNullException("animation", "animation cannot be null.");
		}
		TrackEntry trackEntry = new TrackEntry();
		trackEntry.animation = animation;
		trackEntry.loop = loop;
		trackEntry.time = 0f;
		trackEntry.endTime = animation.Duration;
		TrackEntry trackEntry2 = ExpandToIndex(trackIndex);
		if (trackEntry2 != null)
		{
			while (trackEntry2.next != null)
			{
				trackEntry2 = trackEntry2.next;
			}
			trackEntry2.next = trackEntry;
		}
		else
		{
			tracks.Items[trackIndex] = trackEntry;
		}
		if (delay <= 0f)
		{
			delay = ((trackEntry2 == null) ? 0f : (delay + (trackEntry2.endTime - data.GetMix(trackEntry2.animation, animation))));
		}
		trackEntry.delay = delay;
		return trackEntry;
	}

	public TrackEntry GetCurrent(int trackIndex)
	{
		if (trackIndex >= tracks.Count)
		{
			return null;
		}
		return tracks.Items[trackIndex];
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int i = 0;
		for (int count = tracks.Count; i < count; i++)
		{
			TrackEntry trackEntry = tracks.Items[i];
			if (trackEntry != null)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(trackEntry.ToString());
			}
		}
		if (stringBuilder.Length == 0)
		{
			return "<none>";
		}
		return stringBuilder.ToString();
	}
}
