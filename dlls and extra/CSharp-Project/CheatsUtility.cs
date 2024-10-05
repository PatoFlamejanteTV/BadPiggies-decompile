using System;
using System.Collections.Generic;
using UnityEngine;

public class CheatsUtility : WPFMonoBehaviour
{
	[SerializeField]
	private TextMesh textMesh;

	private float m_buttonHeight;

	private float m_buttonWidth;

	private bool m_isRegisteringDevice;

	private GUISkin cheatSkin;

	private bool skinInitialized;

	private Vector2 scrollbarPosition = Vector2.zero;

	private int rowItems = 3;

	public static string versionStatusCheat = "cheatMimicOlderVersion";

	private static List<string> gameModeNames = new List<string> { "None", "Cake Race\nPreview Mode" };

	private int currentButtonIndex;

	private void Start()
	{
		float dpi = Screen.dpi;
		if (dpi < 1f)
		{
			m_buttonHeight = (float)Screen.height * (1f / 6f);
			m_buttonWidth = (float)Screen.width * 0.3125f;
		}
		else
		{
			float num = (float)Screen.width / dpi * 0.5f;
			rowItems = Mathf.Clamp((int)num, 3, 10);
			m_buttonWidth = (float)Screen.width * (1f / (1.1f * (float)rowItems));
			float value = (float)Screen.height / dpi;
			m_buttonHeight = (float)Screen.height * (1f / Mathf.Clamp(value, 3f, 10f));
		}
		string text = "sir";
		if (!string.IsNullOrEmpty(SystemInfo.deviceName))
		{
			text = SystemInfo.deviceName;
		}
		if (text.Length > 10)
		{
			text = "\n" + text;
		}
		textMesh.text = "Jolly good day, " + text;
	}

	private void DrawButton(string label, Action onClick)
	{
		if (currentButtonIndex == 0 || currentButtonIndex % rowItems == 0)
		{
			if (currentButtonIndex != 0)
			{
				GUILayout.EndHorizontal();
			}
			GUILayout.BeginHorizontal();
		}
		if (GUILayout.Button(label, GUILayout.Width(m_buttonWidth), GUILayout.Height(m_buttonHeight)))
		{
			onClick?.Invoke();
		}
		currentButtonIndex++;
	}

	private void BeginGrid()
	{
		currentButtonIndex = 0;
	}

	private void EndGrid()
	{
		GUILayout.EndHorizontal();
	}

	private void OnGUI()
	{
		if (!skinInitialized)
		{
			cheatSkin = GUI.skin;
			float dpi = Screen.dpi;
			if (dpi > 1f)
			{
				cheatSkin.label.fontSize = Mathf.FloorToInt(0.1f * dpi + 2f);
				cheatSkin.button.fontSize = Mathf.FloorToInt(0.1f * dpi + 2f);
			}
			else
			{
				cheatSkin.label.fontSize = Mathf.FloorToInt(15f);
				cheatSkin.button.fontSize = Mathf.FloorToInt(15f);
			}
			cheatSkin.button.wordWrap = true;
			GUI.skin.verticalScrollbar.fixedWidth = (float)Screen.width * 0.05f;
			GUI.skin.verticalScrollbarThumb.fixedWidth = (float)Screen.width * 0.05f;
			skinInitialized = true;
		}
		scrollbarPosition = GUILayout.BeginScrollView(scrollbarPosition, GUILayout.Width(Screen.width), GUILayout.Height((float)Screen.height - (float)Screen.height * 0.1f));
		BeginGrid();
		int gameModeIndex = UserSettings.GetInt("game_mode");
		if (gameModeIndex >= 0 && gameModeIndex < gameModeNames.Count)
		{
			DrawButton("Toggle game mode:\n" + gameModeNames[gameModeIndex], delegate
			{
				gameModeIndex++;
				if (gameModeIndex >= gameModeNames.Count)
				{
					gameModeIndex = 0;
				}
				if (gameModeIndex == 0)
				{
					UserSettings.DeleteKey("game_mode");
				}
				else
				{
					UserSettings.SetInt("game_mode", gameModeIndex);
				}
			});
		}
		DrawButton("Reset progress", delegate
		{
			GameProgress.DeleteAll();
			GameProgress.InitializeGameProgressData();
			GameProgress.Save();
			UserSettings.DeleteAll();
			UserSettings.Save();
			if (Singleton<DailyChallenge>.IsInstantiated() && Singleton<DailyChallenge>.Instance.Initialized)
			{
				Singleton<DailyChallenge>.Instance.ForceNewChallenge();
			}
		});
		DrawButton("1-star all levels", delegate
		{
			foreach (Episode episodeLevel in WPFMonoBehaviour.gameData.m_episodeLevels)
			{
				for (int num5 = 0; num5 < episodeLevel.LevelInfos.Count; num5++)
				{
					SetStarsCompletion(episodeLevel.LevelInfos[num5], 1);
				}
			}
		});
		DrawButton("3-stars all but one", delegate
		{
			foreach (Episode episodeLevel2 in WPFMonoBehaviour.gameData.m_episodeLevels)
			{
				int num3 = UnityEngine.Random.Range(0, episodeLevel2.LevelInfos.Count - 3);
				for (int num4 = 0; num4 < episodeLevel2.LevelInfos.Count - 2; num4++)
				{
					if (num4 != num3)
					{
						SetStarsCompletion(episodeLevel2.LevelInfos[num4], 3);
					}
				}
			}
		});
		DrawButton("3-stars all", delegate
		{
			foreach (Episode episodeLevel3 in WPFMonoBehaviour.gameData.m_episodeLevels)
			{
				for (int n = 0; n < episodeLevel3.LevelInfos.Count; n++)
				{
					SetStarsCompletion(episodeLevel3.LevelInfos[n], 3);
				}
			}
		});
		DrawButton("Sandbox all starboxes", delegate
		{
			foreach (SandboxLevels.LevelData level in WPFMonoBehaviour.gameData.m_sandboxLevels.Levels)
			{
				for (int m = 0; m < level.m_starBoxCount; m++)
				{
					if (m < 10)
					{
						GameProgress.AddSandboxStar(level.SceneName, "StarBox0" + m);
					}
					else
					{
						GameProgress.AddSandboxStar(level.SceneName, "StarBox" + m);
					}
				}
			}
		});
		DrawButton("Unlimited Sandbox Parts", delegate
		{
			foreach (BasePart.PartType value2 in Enum.GetValues(typeof(BasePart.PartType)))
			{
				if (value2 != 0 && value2 != BasePart.PartType.ObsoleteWheel && value2 != BasePart.PartType.JetEngine)
				{
					int sandboxPartCount = GameProgress.GetSandboxPartCount(value2);
					GameProgress.AddSandboxParts(value2, 99 - sandboxPartCount, showUnlockAnimations: false);
				}
			}
		});
		if (Application.targetFrameRate == 60)
		{
			DrawButton("Set low target FPS", delegate
			{
				Application.targetFrameRate = 25;
			});
		}
		else
		{
			DrawButton("Set default target FPS", delegate
			{
				Application.targetFrameRate = 60;
			});
		}
		DrawButton("Unlock all levels", delegate
		{
			GameProgress.SetBool("UnlockAllLevels", value: true);
		});
		DrawButton("Restore IAPs", delegate
		{
			Singleton<IapManager>.Instance.RestorePurchasedItems();
		});
		DrawButton("Reset IAPs", delegate
		{
			GameProgress.SetBool("ResetIAPs", value: true);
		});
		DrawButton("Add 10 Autobuilds", delegate
		{
			GameProgress.AddBluePrints(10);
		});
		DrawButton("Add Wooden crate", delegate
		{
			GameProgress.AddLootcrate(LootCrateType.Wood);
		});
		DrawButton("Add Metal crate", delegate
		{
			GameProgress.AddLootcrate(LootCrateType.Metal);
		});
		DrawButton("Add Golden crate", delegate
		{
			GameProgress.AddLootcrate(LootCrateType.Gold);
		});
		DrawButton("Add 10 Glue, Magnet, Turbo and NightVision", delegate
		{
			GameProgress.AddSuperGlue(10);
			GameProgress.AddSuperMagnet(10);
			GameProgress.AddTurboCharge(10);
			GameProgress.AddNightVision(10);
		});
		DrawButton("Unlock all sandboxes", delegate
		{
			GameProgress.UnlockButton("EpisodeButtonSandbox");
			foreach (SandboxLevels.LevelData level2 in WPFMonoBehaviour.gameData.m_sandboxLevels.Levels)
			{
				if (!(level2.m_identifier == "S-F") && !(level2.m_identifier == "S-M"))
				{
					GameProgress.SetSandboxUnlocked(level2.m_identifier, unlocked: true);
				}
			}
		});
		DrawButton("Unlock Field of Dreams", delegate
		{
			GameProgress.SetSandboxUnlocked("S-F", unlocked: true);
		});
		DrawButton("Unlock Little Pig Adventure", delegate
		{
			GameProgress.SetSandboxUnlocked("S-M", unlocked: true);
		});
		DrawButton("Unlock iOS Full version", delegate
		{
			GameProgress.SetFullVersionUnlocked(unlock: true);
		});
		DrawButton("Mimic 1.8.0 install version. Game needs to be restarted.", delegate
		{
			GameProgress.SetString("InstallVersion", "1.8.0");
			GameProgress.DeleteKey("LastKnownVersion");
		});
		DrawButton("Unlock & 3-star Race Levels except for last", delegate
		{
			List<RaceLevels.LevelData> levels = WPFMonoBehaviour.gameData.m_raceLevels.Levels;
			for (int l = 0; l < levels.Count - 1; l++)
			{
				GameProgress.SetInt(levels[l].m_identifier + "_stars", 3);
				GameProgress.SetBestTime(levels[l].SceneName, 10f);
				GameProgress.SetRaceLevelUnlocked(levels[l].m_identifier, unlocked: true);
			}
		});
		DrawButton("Add some desserts", delegate
		{
			int count = WPFMonoBehaviour.gameData.m_desserts.Count;
			int num2 = UnityEngine.Random.Range(1, count);
			for (int k = 0; k < num2; k++)
			{
				GameProgress.AddDesserts(WPFMonoBehaviour.gameData.m_desserts[UnityEngine.Random.Range(0, count)].name, UnityEngine.Random.Range(1, 6));
			}
		});
		DrawButton("Enable basic mechanic", delegate
		{
			GameProgress.SetBool("PermanentBlueprint", value: true);
		});
		DrawButton("Test Force Update. You must relaunch the app manually", delegate
		{
			GameProgress.SetInt(versionStatusCheat, 3);
		});
		DrawButton("Test Optional Update. You must relaunch the app manually", delegate
		{
			GameProgress.SetInt(versionStatusCheat, 1);
		});
		DrawButton("Unlock All Free Levels", delegate
		{
			GameProgress.SetBool("UnlockAllFreeLevels", value: true);
		});
		if (Singleton<RewardSystem>.IsInstantiated())
		{
			string text = "Reward Timer Toggle\nReward time / Reset time\n";
			switch (Singleton<RewardSystem>.Instance.GetTimerMode())
			{
			case 0:
				text += "24h / 48h";
				break;
			case 1:
				text += "15m / 30m";
				break;
			case 2:
				text += "5m / 15m";
				break;
			case 3:
				text += "1m / 1m 15s";
				break;
			case 4:
				text += "5s / 10s";
				break;
			}
			DrawButton(text, delegate
			{
				Singleton<RewardSystem>.Instance.ChangeTimerMode();
			});
		}
		DrawButton("Reset snout intro", delegate
		{
			GameProgress.SetInt("show_count_snout_intro", 0);
		});
		DrawButton("Add 1000 snout coins", delegate
		{
			GameProgress.AddSnoutCoins(1000);
		});
		DrawButton("Add 100 scrap", delegate
		{
			GameProgress.AddScrap(100);
		});
		DrawButton("Unlock all custom parts", delegate
		{
			UnlockParts(BasePart.PartTier.Common);
			UnlockParts(BasePart.PartTier.Rare);
			UnlockParts(BasePart.PartTier.Epic);
			UnlockParts(BasePart.PartTier.Legendary);
		});
		DrawButton("Unlock all Common parts", delegate
		{
			UnlockParts(BasePart.PartTier.Common);
		});
		DrawButton("Unlock all Rare parts", delegate
		{
			UnlockParts(BasePart.PartTier.Rare);
		});
		DrawButton("Unlock all Epic parts", delegate
		{
			UnlockParts(BasePart.PartTier.Epic);
		});
		DrawButton("Unlock all Legendary parts", delegate
		{
			UnlockParts(BasePart.PartTier.Legendary);
		});
		DrawButton("Unlock all craftable items", delegate
		{
			UnlockParts(BasePart.PartTier.Common, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable);
			UnlockParts(BasePart.PartTier.Rare, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable);
			UnlockParts(BasePart.PartTier.Epic, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable);
			UnlockParts(BasePart.PartTier.Legendary, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable);
		});
		DrawButton("Reset Workshop Tutorial", delegate
		{
			GameProgress.DeleteKey("Workshop_Tutorial");
		});
		DrawButton("Reset Crate Craze popup", delegate
		{
			GameProgress.SetBool("CrateCrazeSale_shown", value: false);
		});
		bool processPurchases = GameProgress.GetBool("Process_purchases", defaultValue: true);
		DrawButton((!processPurchases) ? "Don't process purchases" : "Do process purchases", delegate
		{
			GameProgress.SetBool("Process_purchases", !processPurchases);
		});
		DrawButton("CRASH CLIENT", delegate
		{
			((object)null).ToString();
		});
		DrawButton("Force Garbage Collection", delegate
		{
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
		});
		bool fastNotifications = GameProgress.GetBool("fast_notifications");
		DrawButton((!fastNotifications) ? "Use faster notification schedule" : "Use realtime notification schedule", delegate
		{
			GameProgress.SetBool("fast_notifications", !fastNotifications);
		});
		if (GameProgress.HasKey("notification_clicked"))
		{
			DrawButton("Clicked notification:\n" + GameProgress.GetString("notification_clicked", "null"), null);
		}
		DrawButton("Clear PlayerPrefs", delegate
		{
			PlayerPrefs.DeleteAll();
		});
		DrawButton("Get all but one customizations", delegate
		{
			for (int i = 1; i < 4; i++)
			{
				List<BasePart> allTierParts = CustomizationManager.GetAllTierParts((BasePart.PartTier)i, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable | CustomizationManager.PartFlags.Rewardable);
				for (int j = 0; j < allTierParts.Count - 1; j++)
				{
					CustomizationManager.UnlockPart(allTierParts[j], "Cheat");
				}
			}
		});
		DrawButton("Reset level to 1", delegate
		{
			GameProgress.SetInt("player_level", 1);
			GameProgress.SetInt("player_experience", 0);
			GameProgress.SetInt("player_pending_experience", -1);
		});
		DrawButton("Set level to 5", delegate
		{
			GameProgress.SetInt("player_level", 5);
			GameProgress.SetInt("player_experience", 0);
			GameProgress.SetInt("player_pending_experience", -1);
		});
		DrawButton("Force local game configuration '" + GameProgress.GetBool("ForceLocalGameConfiguration") + "' (requires restart)", delegate
		{
		});
		if (GameProgress.HasKey("cup_advance_cheat"))
		{
			DrawButton("Current cup:\n" + CakeRaceMenu.GetCurrentLeaderboardCup().ToString() + "\nNext cup:\n" + (PlayFabLeaderboard.Leaderboard)GameProgress.GetInt("cup_advance_cheat", 1), delegate
			{
			});
			int rankCheat = GameProgress.GetInt("cup_rank_cheat");
			DrawButton((!GameProgress.HasKey("cup_rank_cheat")) ? "Enable rank cheat" : ("Current Rank:\n" + rankCheat + "\nLower rank?"), delegate
			{
				switch (rankCheat)
				{
				case 3:
					rankCheat = 5;
					break;
				default:
					if (rankCheat == 50)
					{
						rankCheat = 100;
						break;
					}
					if (rankCheat == 100)
					{
						rankCheat = 250;
						break;
					}
					if (rankCheat == 250)
					{
						rankCheat = 500;
						break;
					}
					if (rankCheat != 500)
					{
						rankCheat = 1;
						break;
					}
					goto case 1;
				case 5:
					rankCheat = 10;
					break;
				case 10:
					rankCheat = 50;
					break;
				case 1:
				case 2:
					rankCheat++;
					break;
				}
				GameProgress.SetInt("cup_rank_cheat", rankCheat);
			});
		}
		else
		{
			DrawButton("Current cup:\n" + CakeRaceMenu.GetCurrentLeaderboardCup().ToString() + "\nAdvance to next Cup", delegate
			{
				int value = (int)(1 + CakeRaceMenu.GetCurrentLeaderboardCup());
				value = Mathf.Clamp(value, (int)PlayFabLeaderboard.LowestCup(), (int)PlayFabLeaderboard.HighestCup());
				GameProgress.SetInt("cup_advance_cheat", value);
				if (GameProgress.HasKey("cup_rank_cheat"))
				{
					GameProgress.DeleteKey("cup_rank_cheat");
				}
			});
		}
		DrawButton("Reset cup cheats", delegate
		{
			if (GameProgress.HasKey("cup_advance_cheat"))
			{
				GameProgress.DeleteKey("cup_advance_cheat");
			}
			if (GameProgress.HasKey("cup_rank_cheat"))
			{
				GameProgress.DeleteKey("cup_rank_cheat");
			}
			GameProgress.SetInt("cake_race_current_cup", CakeRaceMenu.GetCupIndexFromPlayerLevel());
		});
		int leaderboardTestAmount = GameProgress.GetInt("cheat_leaderboard_test", -1);
		if (leaderboardTestAmount < 0)
		{
			DrawButton("Generate test leaderboard", delegate
			{
				GameProgress.SetInt("cheat_leaderboard_test", 0);
			});
		}
		else
		{
			DrawButton("Test leaderboard amount:\n" + leaderboardTestAmount, delegate
			{
				switch (leaderboardTestAmount)
				{
				case 5:
					leaderboardTestAmount = 8;
					break;
				default:
				{
					int num = leaderboardTestAmount;
					if ((uint)(num - 500) > 2u)
					{
						if (leaderboardTestAmount == 25)
						{
							leaderboardTestAmount = 50;
						}
						else if (leaderboardTestAmount == 50)
						{
							leaderboardTestAmount = 75;
						}
						else if (leaderboardTestAmount == 75)
						{
							leaderboardTestAmount = 100;
						}
						else if (leaderboardTestAmount == 100)
						{
							leaderboardTestAmount = 175;
						}
						else if (leaderboardTestAmount == 175)
						{
							leaderboardTestAmount = 250;
						}
						else if (leaderboardTestAmount == 250)
						{
							leaderboardTestAmount = 425;
						}
						else if (leaderboardTestAmount != 425)
						{
							leaderboardTestAmount = 0;
						}
						else
						{
							leaderboardTestAmount = 500;
						}
						break;
					}
					goto case 0;
				}
				case 8:
					leaderboardTestAmount = 10;
					break;
				case 10:
					leaderboardTestAmount = 25;
					break;
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
					leaderboardTestAmount++;
					break;
				}
				GameProgress.SetInt("cheat_leaderboard_test", leaderboardTestAmount);
			});
			int leaderboardRankCheat = GameProgress.GetInt("cheat_leaderboard_test_local_rank", -1);
			DrawButton("Cheat player rank: " + (leaderboardRankCheat + 1), delegate
			{
				switch (leaderboardRankCheat)
				{
				case -1:
					leaderboardRankCheat = 0;
					break;
				case 0:
					leaderboardRankCheat = 1;
					break;
				case 1:
					leaderboardRankCheat = 2;
					break;
				case 2:
					leaderboardRankCheat = 3;
					break;
				case 3:
					leaderboardRankCheat = 4;
					break;
				case 4:
					leaderboardRankCheat = 9;
					break;
				default:
					if (leaderboardRankCheat != 24)
					{
						if (leaderboardRankCheat != 49)
						{
							if (leaderboardRankCheat != 149)
							{
								if (leaderboardRankCheat != 499)
								{
									if (leaderboardRankCheat != 999)
									{
										if (leaderboardRankCheat != 9999)
										{
											if (leaderboardRankCheat == 99999)
											{
												leaderboardRankCheat = -1;
											}
										}
										else
										{
											leaderboardRankCheat = 99999;
										}
									}
									else
									{
										leaderboardRankCheat = 9999;
									}
								}
								else
								{
									leaderboardRankCheat = 999;
								}
							}
							else
							{
								leaderboardRankCheat = 499;
							}
						}
						else
						{
							leaderboardRankCheat = 149;
						}
					}
					else
					{
						leaderboardRankCheat = 49;
					}
					break;
				case 9:
					leaderboardRankCheat = 24;
					break;
				}
				GameProgress.SetInt("cheat_leaderboard_test_local_rank", leaderboardRankCheat);
			});
			DrawButton("Clear test leaderboard", delegate
			{
				GameProgress.DeleteKey("cheat_leaderboard_test");
				GameProgress.DeleteKey("cheat_leaderboard_test_local_rank");
			});
		}
		DrawButton("Reset alien machine", delegate
		{
			if (GameProgress.HasKey("AlienCraftingMachineShown"))
			{
				GameProgress.DeleteKey("AlienCraftingMachineShown");
			}
		});
		DrawButton("Reset Cake Race unlock", delegate
		{
			if (GameProgress.HasKey("CakeRaceUnlockShown"))
			{
				GameProgress.SetBool("CakeRaceUnlockShown", value: false);
			}
			if (GameProgress.HasKey("UnlockShown_CakeRaceButton"))
			{
				GameProgress.SetBool("UnlockShown_CakeRaceButton", value: false);
			}
		});
		DrawButton("Skip cake race tutorial", delegate
		{
			if (GameProgress.GetInt("cake_race_total_wins") < 7)
			{
				GameProgress.SetInt("cake_race_total_wins", 7);
			}
		});
		EndGrid();
		GUILayout.EndScrollView();
		GUI.Label(new Rect((float)Screen.width * 0.9f, (float)Screen.height * 0.93f, (float)Screen.width * 0.1f, (float)Screen.height * 0.1f), "Debug \n(v" + Singleton<BuildCustomizationLoader>.Instance.ApplicationVersion + " - " + Singleton<BuildCustomizationLoader>.Instance.SVNRevisionNumber + ")");
		if (GUI.Button(new Rect((float)Screen.width * 0.2f, (float)Screen.height * 0.92f, (float)Screen.width * 0.6f, (float)Screen.height * 0.08f), "Back to Main Menu"))
		{
			Singleton<GameManager>.Instance.LoadMainMenu(showLoadingScreen: false);
		}
		GUI.skin = null;
	}

	private void UnlockParts(BasePart.PartTier tier)
	{
		List<BasePart> allTierParts = CustomizationManager.GetAllTierParts(tier, CustomizationManager.PartFlags.Locked | CustomizationManager.PartFlags.Craftable | CustomizationManager.PartFlags.Rewardable);
		if (allTierParts != null && allTierParts.Count != 0)
		{
			for (int i = 0; i < allTierParts.Count; i++)
			{
				CustomizationManager.UnlockPart(allTierParts[i], "Cheat");
			}
		}
	}

	private void UnlockParts(BasePart.PartTier tier, CustomizationManager.PartFlags flags)
	{
		List<BasePart> allTierParts = CustomizationManager.GetAllTierParts(tier, flags);
		if (allTierParts != null && allTierParts.Count != 0)
		{
			for (int i = 0; i < allTierParts.Count; i++)
			{
				CustomizationManager.UnlockPart(allTierParts[i], "Cheat");
			}
		}
	}

	private void SetStarsCompletion(EpisodeLevelInfo level, int starCount)
	{
		int num = Mathf.Clamp(starCount, 0, 3);
		GameProgress.SetInt(level.sceneName + "_stars", num);
		GameProgress.SetLevelCompleted(level.sceneName);
		if (num > 0)
		{
			GameProgress.SetChallengeCompleted(level.sceneName, 0, completed: true);
		}
		if (num > 1)
		{
			GameProgress.SetChallengeCompleted(level.sceneName, 1, completed: true);
		}
		if (num > 2)
		{
			GameProgress.SetChallengeCompleted(level.sceneName, 2, completed: true);
		}
	}

	private void OnDeviceRegistered(bool result)
	{
		if (result)
		{
			GameProgress.SetBool("TestDeviceRegistered", value: true);
		}
		m_isRegisteringDevice = false;
	}

	private void OnDeviceUnregistered(bool result)
	{
		if (result)
		{
			GameProgress.SetBool("TestDeviceRegistered", value: false);
		}
		m_isRegisteringDevice = false;
	}
}
