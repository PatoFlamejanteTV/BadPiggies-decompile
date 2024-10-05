using PlayFab.SharedModels;

namespace PlayFab.Internal;

public class ApiProcessingEventArgs
{
	public string ApiEndpoint;

	public ApiProcessingEventType EventType;

	public PlayFabRequestCommon Request;

	public PlayFabResultCommon Result;

	public TRequest GetRequest<TRequest>() where TRequest : PlayFabRequestCommon
	{
		return Request as TRequest;
	}
}
