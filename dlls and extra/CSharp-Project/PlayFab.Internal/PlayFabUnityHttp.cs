using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zlib;
using PlayFab.Json;
using PlayFab.SharedModels;
using UnityEngine.Networking;

namespace PlayFab.Internal;

public class PlayFabUnityHttp : IPlayFabHttp
{
	private readonly int _pendingWwwMessages;

	public bool SessionStarted { get; set; }

	public string AuthKey { get; set; }

	public string EntityToken { get; set; }

	public void InitializeHttp()
	{
	}

	public void Update()
	{
	}

	public void OnDestroy()
	{
	}

	public void SimpleGetCall(string fullUrl, Action<byte[]> successCallback, Action<string> errorCallback)
	{
		SingletonMonoBehaviour<PlayFabHttp>.instance.StartCoroutine(SimpleCallCoroutine(fullUrl, null, successCallback, errorCallback));
	}

	public void SimplePutCall(string fullUrl, byte[] payload, Action successCallback, Action<string> errorCallback)
	{
		SingletonMonoBehaviour<PlayFabHttp>.instance.StartCoroutine(SimpleCallCoroutine(fullUrl, payload, delegate
		{
			successCallback();
		}, errorCallback));
	}

	private static IEnumerator SimpleCallCoroutine(string fullUrl, byte[] payload, Action<byte[]> successCallback, Action<string> errorCallback)
	{
		if (payload == null)
		{
			UnityWebRequest www = new UnityWebRequest(fullUrl)
			{
				downloadHandler = new DownloadHandlerBuffer(),
				method = "POST"
			};
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				errorCallback(www.error);
			}
			else
			{
				successCallback(www.downloadHandler.data);
			}
		}
		else
		{
			UnityWebRequest www = UnityWebRequest.Put(fullUrl, payload);
			www.SendWebRequest();
			while (www.uploadProgress < 1f && www.downloadProgress < 1f)
			{
				yield return 1;
			}
			if (!string.IsNullOrEmpty(www.error))
			{
				errorCallback(www.error);
			}
			else
			{
				successCallback(null);
			}
		}
	}

	public void MakeApiCall(CallRequestContainer reqContainer)
	{
		reqContainer.RequestHeaders["Content-Type"] = "application/json";
		if (PlayFabSettings.CompressApiData)
		{
			reqContainer.RequestHeaders["Content-Encoding"] = "GZIP";
			reqContainer.RequestHeaders["Accept-Encoding"] = "GZIP";
			using MemoryStream memoryStream = new MemoryStream();
			using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestCompression))
			{
				gZipStream.Write(reqContainer.Payload, 0, reqContainer.Payload.Length);
			}
			reqContainer.Payload = memoryStream.ToArray();
		}
		SingletonMonoBehaviour<PlayFabHttp>.instance.StartCoroutine(Post(reqContainer));
	}

	private IEnumerator Post(CallRequestContainer reqContainer)
	{
		UnityWebRequest www = new UnityWebRequest(reqContainer.FullUrl)
		{
			uploadHandler = new UploadHandlerRaw(reqContainer.Payload),
			downloadHandler = new DownloadHandlerBuffer(),
			method = "POST"
		};
		foreach (KeyValuePair<string, string> requestHeader in reqContainer.RequestHeaders)
		{
			www.SetRequestHeader(requestHeader.Key, requestHeader.Value);
		}
		yield return www.SendWebRequest();
		if (!string.IsNullOrEmpty(www.error))
		{
			OnError(www.error, reqContainer);
		}
		else
		{
			try
			{
				byte[] data = www.downloadHandler.data;
				bool num = data != null && data[0] == 31 && data[1] == 139;
				string response = "Unexpected error: cannot decompress GZIP stream.";
				if (!num && data != null)
				{
					response = Encoding.UTF8.GetString(data, 0, data.Length);
				}
				if (num)
				{
					using GZipStream gZipStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress, leaveOpen: false);
					byte[] array = new byte[4096];
					using MemoryStream memoryStream = new MemoryStream();
					int count;
					while ((count = gZipStream.Read(array, 0, array.Length)) > 0)
					{
						memoryStream.Write(array, 0, count);
					}
					memoryStream.Seek(0L, SeekOrigin.Begin);
					string response2 = new StreamReader(memoryStream).ReadToEnd();
					OnResponse(response2, reqContainer);
				}
				else
				{
					OnResponse(response, reqContainer);
				}
			}
			catch (Exception ex)
			{
				OnError("Unhandled error in PlayFabUnityHttp: " + ex, reqContainer);
			}
		}
		www.Dispose();
	}

	public int GetPendingMessages()
	{
		return _pendingWwwMessages;
	}

	public void OnResponse(string response, CallRequestContainer reqContainer)
	{
		try
		{
			HttpResponseObject httpResponseObject = JsonWrapper.DeserializeObject<HttpResponseObject>(response);
			if (httpResponseObject.code == 200)
			{
				reqContainer.JsonResponse = JsonWrapper.SerializeObject(httpResponseObject.data);
				reqContainer.DeserializeResultJson();
				reqContainer.ApiResult.Request = reqContainer.ApiRequest;
				reqContainer.ApiResult.CustomData = reqContainer.CustomData;
				SingletonMonoBehaviour<PlayFabHttp>.instance.OnPlayFabApiResult(reqContainer.ApiResult);
				PlayFabDeviceUtil.OnPlayFabLogin(reqContainer.ApiResult);
				try
				{
					PlayFabHttp.SendEvent(reqContainer.ApiEndpoint, reqContainer.ApiRequest, reqContainer.ApiResult, ApiProcessingEventType.Post);
				}
				catch (Exception)
				{
				}
				try
				{
					reqContainer.InvokeSuccessCallback();
					return;
				}
				catch (Exception)
				{
					return;
				}
			}
			if (reqContainer.ErrorCallback != null)
			{
				reqContainer.Error = PlayFabHttp.GeneratePlayFabError(reqContainer.ApiEndpoint, response, reqContainer.CustomData);
				PlayFabHttp.SendErrorEvent(reqContainer.ApiRequest, reqContainer.Error);
				reqContainer.ErrorCallback(reqContainer.Error);
			}
		}
		catch (Exception)
		{
		}
	}

	public void OnError(string error, CallRequestContainer reqContainer)
	{
		reqContainer.JsonResponse = error;
		if (reqContainer.ErrorCallback != null)
		{
			reqContainer.Error = PlayFabHttp.GeneratePlayFabError(reqContainer.ApiEndpoint, reqContainer.JsonResponse, reqContainer.CustomData);
			PlayFabHttp.SendErrorEvent(reqContainer.ApiRequest, reqContainer.Error);
			reqContainer.ErrorCallback(reqContainer.Error);
		}
	}
}
