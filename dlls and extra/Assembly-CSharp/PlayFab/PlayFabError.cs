using System;
using System.Collections.Generic;
using System.Text;

namespace PlayFab;

public class PlayFabError
{
	public string ApiEndpoint;

	public int HttpCode;

	public string HttpStatus;

	public PlayFabErrorCode Error;

	public string ErrorMessage;

	public Dictionary<string, List<string>> ErrorDetails;

	public object CustomData;

	[ThreadStatic]
	private static StringBuilder _tempSb;

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (ErrorDetails != null)
		{
			foreach (KeyValuePair<string, List<string>> errorDetail in ErrorDetails)
			{
				stringBuilder.Append(errorDetail.Key);
				stringBuilder.Append(": ");
				stringBuilder.Append(string.Join(", ", errorDetail.Value.ToArray()));
				stringBuilder.Append(" | ");
			}
		}
		return $"{ApiEndpoint} PlayFabError({Error}, {ErrorMessage}, {HttpCode} {HttpStatus}" + ((stringBuilder.Length <= 0) ? ")" : (" - Details: " + stringBuilder.ToString() + ")"));
	}

	public string GenerateErrorReport()
	{
		if (_tempSb == null)
		{
			_tempSb = new StringBuilder();
		}
		_tempSb.Length = 0;
		_tempSb.Append(ApiEndpoint).Append(": ").Append(ErrorMessage);
		if (ErrorDetails != null)
		{
			foreach (KeyValuePair<string, List<string>> errorDetail in ErrorDetails)
			{
				foreach (string item in errorDetail.Value)
				{
					_tempSb.Append("\n").Append(errorDetail.Key).Append(": ")
						.Append(item);
				}
			}
		}
		return _tempSb.ToString();
	}
}
