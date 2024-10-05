using System;
using System.Collections.Generic;
using UnityEngine;

public class AdvertisementHandler
{
	public class RenderableHandler
	{
		public string m_placementName;

		public Texture2D m_texture;

		public Action<bool> onRenderableReady;

		public RenderableHandler(string placement)
		{
			m_placementName = placement;
			m_texture = null;
		}

		public bool OnRenderableReady(string placement, string contentType, List<byte> content)
		{
			if (!placement.Equals(m_placementName) || content == null)
			{
				if (onRenderableReady != null)
				{
					onRenderableReady(obj: false);
				}
				return false;
			}
			if (contentType.StartsWith("image/"))
			{
				Texture2D texture2D = new Texture2D(1, 1);
				if (texture2D.LoadImage(content.ToArray()))
				{
					m_texture = texture2D;
					if (onRenderableReady != null)
					{
						onRenderableReady(obj: true);
					}
					return true;
				}
			}
			if (onRenderableReady != null)
			{
				onRenderableReady(obj: false);
			}
			return false;
		}
	}

	private static RenderableHandler rewardNativeRenderable;

	private static RenderableHandler mainMenuPromoRenderable;

	private static RenderableHandler crossPromoMainRenderable;

	private static RenderableHandler crossPromoEpisodeRenderable;

	private static string dailyChallengeRevealPlacement = "RewardVideo.DailyChallengeReveal";

	private static string levelRewardVideoPlacement = "RewardVideo.LevelUnlock";

	private static string snoutCoinRewardVideoPlacement = "RewardVideo.SnoutReward";

	private static string timeRewardVideoPlacement = "RewardVideo";

	private static string rewardNativePlacement = "RewardNative";

	private static string mainMenuPopupPlacement = "MainMenuPopup";

	private static string interstitialPlacement = "LevelStartInterstitial";

	private static string doubleRewardPlacement = "RewardVideo.DoubleReward";

	private static string extraCoinsRewardPlacement = "RewardVideo.ExtraCoins";

	private static string pauseMenuPromoPlacement = "NewsFeed.pause";

	private static string freeLootCratePlacement = "RewardVideo.FreeLootCrate";

	private static string crossPromoMainPlacement = "InGameNative.MainMenu";

	private static string crossPromoEpisodePlacement = "InGameNative.EpisodeMenu";

	public static string LevelRewardVideoPlacement => levelRewardVideoPlacement;

	public static string SnoutCoinRewardVideoPlacement => snoutCoinRewardVideoPlacement;

	public static string DoubleRewardPlacement => doubleRewardPlacement;

	public static string ExtraCoinsRewardPlacement => extraCoinsRewardPlacement;

	public static string DailyChallengeRevealPlacement => dailyChallengeRevealPlacement;

	public static string FreeLootCratePlacement => freeLootCratePlacement;

	public static RenderableHandler MainMenuPromoRenderable => mainMenuPromoRenderable;

	public static RenderableHandler CrossPromoMainRenderable => crossPromoMainRenderable;

	public static RenderableHandler CrossPromoEpisodeRenderable => crossPromoEpisodeRenderable;

	public static Texture2D GetRewardNativeTexture()
	{
		if (rewardNativeRenderable != null)
		{
			return rewardNativeRenderable.m_texture;
		}
		return null;
	}

	public static Texture2D GetMainMenuPopupTexture()
	{
		if (mainMenuPromoRenderable != null)
		{
			return mainMenuPromoRenderable.m_texture;
		}
		return null;
	}

	public static Texture2D GetCrossPromoMainTexture()
	{
		if (crossPromoMainRenderable != null)
		{
			return crossPromoMainRenderable.m_texture;
		}
		return null;
	}

	public static Texture2D GetCrossPromoEpisodeTexture()
	{
		if (crossPromoEpisodeRenderable != null)
		{
			return crossPromoEpisodeRenderable.m_texture;
		}
		return null;
	}

	public static bool IsAdvertisementReady(string placement)
	{
		return false;
	}
}
