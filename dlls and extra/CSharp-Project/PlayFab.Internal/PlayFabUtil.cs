using System;
using System.Globalization;
using System.IO;
using System.Text;
using PlayFab.Json;

namespace PlayFab.Internal;

internal static class PlayFabUtil
{
	public static readonly string[] _defaultDateTimeFormats = new string[15]
	{
		"yyyy-MM-ddTHH:mm:ss.FFFFFFZ", "yyyy-MM-ddTHH:mm:ss.FFFFZ", "yyyy-MM-ddTHH:mm:ss.FFFZ", "yyyy-MM-ddTHH:mm:ss.FFZ", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-dd HH:mm:ssZ", "yyyy-MM-dd HH:mm:ss.FFFFFF", "yyyy-MM-dd HH:mm:ss.FFFF", "yyyy-MM-dd HH:mm:ss.FFF", "yyyy-MM-dd HH:mm:ss.FF",
		"yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm.ss.FFFF", "yyyy-MM-dd HH:mm.ss.FFF", "yyyy-MM-dd HH:mm.ss.FF", "yyyy-MM-dd HH:mm.ss"
	};

	public const int DEFAULT_UTC_OUTPUT_INDEX = 2;

	public const int DEFAULT_LOCAL_OUTPUT_INDEX = 9;

	public static DateTimeStyles DateTimeStyles = DateTimeStyles.RoundtripKind;

	[ThreadStatic]
	private static StringBuilder _sb;

	[Obsolete("This field has moved to SimpleJsonInstance.ApiSerializerStrategy", false)]
	public static SimpleJsonInstance.PlayFabSimpleJsonCuztomization ApiSerializerStrategy => SimpleJsonInstance.ApiSerializerStrategy;

	public static string timeStamp => DateTime.Now.ToString(_defaultDateTimeFormats[9]);

	public static string utcTimeStamp => DateTime.UtcNow.ToString(_defaultDateTimeFormats[2]);

	public static string Format(string text, params object[] args)
	{
		if (args.Length != 0)
		{
			return string.Format(text, args);
		}
		return text;
	}

	public static string ReadAllFileText(string filename)
	{
		if (_sb == null)
		{
			_sb = new StringBuilder();
		}
		_sb.Length = 0;
		BinaryReader binaryReader = new BinaryReader(new FileStream(filename, FileMode.Open));
		while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
		{
			_sb.Append(binaryReader.ReadChar());
		}
		return _sb.ToString();
	}
}
