using System;

namespace PlayFab;

public class PlayFabException : Exception
{
	public readonly PlayFabExceptionCode Code;

	public PlayFabException(PlayFabExceptionCode code, string message)
		: base(message)
	{
		Code = code;
	}
}
