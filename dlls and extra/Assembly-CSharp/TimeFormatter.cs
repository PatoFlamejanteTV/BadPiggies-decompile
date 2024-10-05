using System;
using System.Text;

public static class TimeFormatter
{
	private static StringBuilder sb = new StringBuilder();

	private static string hour = Singleton<Localizer>.Instance.Resolve("COMMON_HOUR").translation;

	private static string hours = Singleton<Localizer>.Instance.Resolve("COMMON_HOURS").translation;

	private static string minute = Singleton<Localizer>.Instance.Resolve("COMMON_MINUTE").translation;

	private static string minutes = Singleton<Localizer>.Instance.Resolve("COMMON_MINUTES").translation;

	private static string second = Singleton<Localizer>.Instance.Resolve("COMMON_SECOND").translation;

	private static string seconds = Singleton<Localizer>.Instance.Resolve("COMMON_SECONDS").translation;

	public static string Format2Minutes(TimeSpan time)
	{
		sb.Remove(0, sb.Length);
		if (time.Hours > 1)
		{
			sb.Append(time.Hours);
			sb.Append(' ');
			sb.Append(hours);
			sb.Append(' ');
		}
		else if (time.Hours > 0)
		{
			sb.Append(time.Hours);
			sb.Append(' ');
			sb.Append(hour);
			sb.Append(' ');
		}
		if (time.Minutes > 1)
		{
			sb.Append(time.Minutes);
			sb.Append(' ');
			sb.Append(minutes);
		}
		else if (time.Minutes > 0)
		{
			sb.Append(time.Minutes);
			sb.Append(' ');
			sb.Append(minute);
		}
		return sb.ToString();
	}

	public static string Format2Seconds(TimeSpan time)
	{
		sb.Remove(0, sb.Length);
		if (time.Hours > 1)
		{
			sb.Append(time.Hours);
			sb.Append(' ');
			sb.Append(hours);
			sb.Append(' ');
		}
		else if (time.Hours > 0)
		{
			sb.Append(time.Hours);
			sb.Append(' ');
			sb.Append(hour);
			sb.Append(' ');
		}
		if (time.Minutes > 1)
		{
			sb.Append(time.Minutes);
			sb.Append(' ');
			sb.Append(minutes);
			sb.Append(' ');
		}
		else if (time.Minutes > 0)
		{
			sb.Append(time.Minutes);
			sb.Append(' ');
			sb.Append(minute);
			sb.Append(' ');
		}
		if (time.Seconds > 1)
		{
			sb.Append(time.Seconds);
			sb.Append(' ');
			sb.Append(seconds);
		}
		else
		{
			sb.Append(time.Seconds);
			sb.Append(' ');
			sb.Append(second);
		}
		return sb.ToString();
	}
}
