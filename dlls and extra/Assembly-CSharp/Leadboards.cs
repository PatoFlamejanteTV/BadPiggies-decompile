using System;
using System.Text;
using PlayFab;
using PlayFab.ClientModels;

public static class Leadboards
{
	public static void GetLeaderboards()
	{
		GetLeaderboardRequest request = new GetLeaderboardRequest
		{
			StartPosition = 0,
			MaxResultsCount = 50,
			StatisticName = "Score"
		};
		Action<GetLeaderboardResult> resultCallback = OnSuccess;
		PlayFabClientAPI.GetLeaderboard(request, resultCallback, OnError);
	}

	private static void OnSuccess(GetLeaderboardResult result)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (PlayerLeaderboardEntry item in result.Leaderboard)
		{
			stringBuilder.Append("********************\n");
			stringBuilder.AppendFormat("Position: {0}\n", item.Position);
			stringBuilder.AppendFormat("Name: {0}\n", item.DisplayName);
			stringBuilder.AppendFormat("Score: {0}\n", item.StatValue);
			stringBuilder.Append("\n");
		}
	}

	private static void OnError(PlayFabError error)
	{
	}
}
