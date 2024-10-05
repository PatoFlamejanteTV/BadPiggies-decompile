using System.Collections.Generic;
using UnityEngine;

public class CommonAudio : ScriptableObject
{
	[SerializeField]
	private BundleDataObject bundleMusicTheme;

	[SerializeField]
	private BundleDataObject bundleLevelSelectionMusic;

	[SerializeField]
	private BundleDataObject bundleInFlightMusic;

	[SerializeField]
	private BundleDataObject bundleBuildMusic;

	[SerializeField]
	private BundleDataObject bundleFeedingMusic;

	[SerializeField]
	private BundleDataObject bundleAmbientJungle;

	[SerializeField]
	private BundleDataObject bundleAmbientPlateau;

	[SerializeField]
	private BundleDataObject bundleAmbientCave;

	[SerializeField]
	private BundleDataObject bundleAmbientNight;

	[SerializeField]
	private BundleDataObject bundleAmbientMorning;

	[SerializeField]
	private BundleDataObject bundleAmbientHalloween;

	[SerializeField]
	private BundleDataObject bundleAmbientMaya;

	[SerializeField]
	private BundleDataObject bundleCakeRaceTheme;

	[SerializeField]
	private BundleDataObject bundleXmasThemeSong;

	[SerializeField]
	private BundleDataObject bundleMenuClick;

	[HideInInspector]
	public AudioSource menuClick;

	[SerializeField]
	private BundleDataObject bundleGadgetButtonClick;

	[HideInInspector]
	public AudioSource gadgetButtonClick;

	[SerializeField]
	private BundleDataObject bundleMenuHover;

	[HideInInspector]
	public AudioSource menuHover;

	[SerializeField]
	private BundleDataObject bundleGoalBoxCollected;

	[HideInInspector]
	public AudioSource goalBoxCollected;

	[SerializeField]
	private BundleDataObject bundleBonusBoxCollected;

	[HideInInspector]
	public AudioSource bonusBoxCollected;

	[SerializeField]
	private BundleDataObject bundleDessertCollected;

	[HideInInspector]
	public AudioSource dessertCollected;

	[SerializeField]
	private BundleDataObject bundlePlacePart;

	[HideInInspector]
	public AudioSource placePart;

	[SerializeField]
	private BundleDataObject bundleRotatePart;

	[HideInInspector]
	public AudioSource rotatePart;

	[SerializeField]
	private BundleDataObject bundleDragPart;

	[HideInInspector]
	public AudioSource dragPart;

	[SerializeField]
	private BundleDataObject bundleRemovePart;

	[HideInInspector]
	public AudioSource removePart;

	[SerializeField]
	private BundleDataObject bundleClearBuildGrid;

	[HideInInspector]
	public AudioSource clearBuildGrid;

	[SerializeField]
	private BundleDataObject bundleBuildContraption;

	[HideInInspector]
	public AudioSource buildContraption;

	[SerializeField]
	private BundleDataObject bundleVictory;

	[HideInInspector]
	public AudioSource victory;

	[SerializeField]
	private BundleDataObject[] bundleStarEffects;

	[HideInInspector]
	public AudioSource[] starEffects;

	[SerializeField]
	private BundleDataObject[] bundleStarLoops;

	[HideInInspector]
	public AudioSource[] starLoops;

	[SerializeField]
	private BundleDataObject bundleBalloonPop;

	[HideInInspector]
	public AudioSource balloonPop;

	[SerializeField]
	private BundleDataObject bundleFan;

	[HideInInspector]
	public AudioSource fan;

	[SerializeField]
	private BundleDataObject bundlePropeller;

	[HideInInspector]
	public AudioSource propeller;

	[SerializeField]
	private BundleDataObject bundleMotorWheelLoop;

	[HideInInspector]
	public AudioSource motorWheelLoop;

	[SerializeField]
	private BundleDataObject bundleNormalWheelLoop;

	[HideInInspector]
	public AudioSource normalWheelLoop;

	[SerializeField]
	private BundleDataObject bundleSmallWheelLoop;

	[HideInInspector]
	public AudioSource smallWheelLoop;

	[SerializeField]
	private BundleDataObject bundleWoodenWheelLoop;

	[HideInInspector]
	public AudioSource woodenWheelLoop;

	[SerializeField]
	private BundleDataObject bundleStickyWheelLoop;

	[HideInInspector]
	public AudioSource stickyWheelLoop;

	[SerializeField]
	private BundleDataObject bundleRotorLoop;

	[HideInInspector]
	public AudioSource rotorLoop;

	[SerializeField]
	private BundleDataObject bundleBellowsPuff;

	[HideInInspector]
	public AudioSource bellowsPuff;

	[SerializeField]
	private BundleDataObject bundleElectricEngine;

	[HideInInspector]
	public AudioSource electricEngine;

	[SerializeField]
	private BundleDataObject bundleEngine;

	[HideInInspector]
	public AudioSource engine;

	[SerializeField]
	private BundleDataObject bundleV8Engine;

	[HideInInspector]
	public AudioSource V8Engine;

	[SerializeField]
	private BundleDataObject bundleBottleCork;

	[HideInInspector]
	public AudioSource bottleCork;

	[SerializeField]
	private BundleDataObject bundleUmbrellaOpen;

	[HideInInspector]
	public AudioSource umbrellaOpen;

	[SerializeField]
	private BundleDataObject bundleUmbrellaClose;

	[HideInInspector]
	public AudioSource umbrellaClose;

	[SerializeField]
	private BundleDataObject bundleTntExplosion;

	[HideInInspector]
	public AudioSource tntExplosion;

	[SerializeField]
	private BundleDataObject bundleSandbagCollision;

	[HideInInspector]
	public AudioSource sandbagCollision;

	[SerializeField]
	private BundleDataObject bundleSpringBoxingGloveShoot;

	[HideInInspector]
	public AudioSource springBoxingGloveShoot;

	[SerializeField]
	private BundleDataObject bundleSpringBoxingGloveWinding;

	[HideInInspector]
	public AudioSource springBoxingGloveWinding;

	[SerializeField]
	private BundleDataObject bundleGrapplingHookAttach;

	[HideInInspector]
	public AudioSource grapplingHookAttach;

	[SerializeField]
	private BundleDataObject bundleGrapplingHookDetach;

	[HideInInspector]
	public AudioSource grapplingHookDetach;

	[SerializeField]
	private BundleDataObject bundleGrapplingHookLaunch;

	[HideInInspector]
	public AudioSource grapplingHookLaunch;

	[SerializeField]
	private BundleDataObject bundleGrapplingHookMiss;

	[HideInInspector]
	public AudioSource grapplingHookMiss;

	[SerializeField]
	private BundleDataObject bundleGrapplingHookReeling;

	[HideInInspector]
	public AudioSource grapplingHookReeling;

	[SerializeField]
	private BundleDataObject bundleGrapplingHookReset;

	[HideInInspector]
	public AudioSource grapplingHookReset;

	[SerializeField]
	private BundleDataObject bundleKickerDetach;

	[HideInInspector]
	public AudioSource kickerDetach;

	[SerializeField]
	private BundleDataObject bundleSuperGlueApplied;

	[HideInInspector]
	public AudioSource SuperGlueApplied;

	[SerializeField]
	private BundleDataObject bundleSuperMagnetApplied;

	[HideInInspector]
	public AudioSource SuperMagnetApplied;

	[SerializeField]
	private BundleDataObject bundleTurboChargeApplied;

	[HideInInspector]
	public AudioSource TurboChargeApplied;

	[SerializeField]
	private BundleDataObject[] bundleCollisionMetalHit;

	[HideInInspector]
	public AudioSource[] collisionMetalHit;

	[SerializeField]
	private BundleDataObject[] bundleCollisionMetalDamage;

	[HideInInspector]
	public AudioSource[] collisionMetalDamage;

	[SerializeField]
	private BundleDataObject[] bundleCollisionMetalBreak;

	[HideInInspector]
	public AudioSource[] collisionMetalBreak;

	[SerializeField]
	private BundleDataObject[] bundleCollisionWoodHit;

	[HideInInspector]
	public AudioSource[] collisionWoodHit;

	[SerializeField]
	private BundleDataObject[] bundleCollisionWoodDamage;

	[HideInInspector]
	public AudioSource[] collisionWoodDamage;

	[SerializeField]
	private BundleDataObject[] bundleCollisionWoodDestroy;

	[HideInInspector]
	public AudioSource[] collisionWoodDestroy;

	[SerializeField]
	private BundleDataObject bundleTutorialIn;

	[HideInInspector]
	public AudioSource tutorialIn;

	[SerializeField]
	private BundleDataObject bundleTutorialOut;

	[HideInInspector]
	public AudioSource tutorialOut;

	[SerializeField]
	private BundleDataObject bundleTutorialFlip;

	[HideInInspector]
	public AudioSource tutorialFlip;

	[SerializeField]
	private BundleDataObject bundleCameraZoomIn;

	[HideInInspector]
	public AudioSource cameraZoomIn;

	[SerializeField]
	private BundleDataObject bundleCameraZoomOut;

	[HideInInspector]
	public AudioSource cameraZoomOut;

	[SerializeField]
	private BundleDataObject bundleJokerLevelUnlocked;

	[HideInInspector]
	public AudioSource jokerLevelUnlocked;

	[SerializeField]
	private BundleDataObject bundleSandboxLevelUnlocked;

	[HideInInspector]
	public AudioSource sandboxLevelUnlocked;

	[SerializeField]
	private BundleDataObject bundleBirdWakeUp;

	[HideInInspector]
	public AudioSource birdWakeUp;

	[SerializeField]
	private BundleDataObject bundleBirdShot;

	[HideInInspector]
	public AudioSource birdShot;

	[SerializeField]
	private BundleDataObject bundleSlingshotStretched;

	[HideInInspector]
	public AudioSource slingshotStretched;

	[SerializeField]
	private BundleDataObject[] bundlePartSlideOnIceLoop;

	[HideInInspector]
	public AudioSource[] partSlideOnIceLoop;

	[SerializeField]
	private BundleDataObject bundleMotorWheelOnIceStart;

	[HideInInspector]
	public AudioSource motorWheelOnIceStart;

	[SerializeField]
	private BundleDataObject bundleMotorWheelOnIceLoop;

	[HideInInspector]
	public AudioSource motorWheelOnIceLoop;

	[SerializeField]
	private BundleDataObject bundleGearboxSwitch;

	[HideInInspector]
	public AudioSource gearboxSwitch;

	[SerializeField]
	private BundleDataObject bundleEnvironmentFanStart;

	[HideInInspector]
	public AudioSource environmentFanStart;

	[SerializeField]
	private BundleDataObject bundleEnvironmentFanLoop;

	[HideInInspector]
	public AudioSource environmentFanLoop;

	[SerializeField]
	private BundleDataObject bundleToggleNightVision;

	[HideInInspector]
	public AudioSource toggleNightVision;

	[SerializeField]
	private BundleDataObject bundleToggleLamp;

	[HideInInspector]
	public AudioSource toggleLamp;

	[SerializeField]
	private BundleDataObject bundleAncientPigDisappear;

	[HideInInspector]
	public AudioSource ancientPigDisappear;

	[SerializeField]
	private BundleDataObject[] bundleGoldenPigHit;

	[HideInInspector]
	public AudioSource[] goldenPigHit;

	[SerializeField]
	private BundleDataObject bundleGoldenPigRoll;

	[HideInInspector]
	public AudioSource goldenPigRoll;

	[SerializeField]
	private BundleDataObject bundleSecretStatueFound;

	[HideInInspector]
	public AudioSource secretStatueFound;

	[SerializeField]
	private BundleDataObject bundlePigFall;

	[HideInInspector]
	public AudioSource pigFall;

	[SerializeField]
	private BundleDataObject bundleBridgeBreak;

	[HideInInspector]
	public AudioSource bridgeBreak;

	[SerializeField]
	private BundleDataObject bundlePressureButtonClick;

	[HideInInspector]
	public AudioSource pressureButtonClick;

	[SerializeField]
	private BundleDataObject bundlePressureButtonRelease;

	[HideInInspector]
	public AudioSource pressureButtonRelease;

	[SerializeField]
	private BundleDataObject[] bundleCollisionRockBreak;

	[HideInInspector]
	public AudioSource[] collisionRockBreak;

	[SerializeField]
	private BundleDataObject[] bundleSnoutCoinHit;

	[HideInInspector]
	public AudioSource[] snoutCoinHit;

	[SerializeField]
	private BundleDataObject[] bundleSnoutCoinFly;

	[HideInInspector]
	public AudioSource[] snoutCoinFly;

	[SerializeField]
	private BundleDataObject bundleSnoutCoinPurchase;

	[HideInInspector]
	public AudioSource snoutCoinPurchase;

	[SerializeField]
	private BundleDataObject bundleSnoutCoinIntro;

	[HideInInspector]
	public AudioSource snoutCoinIntro;

	[SerializeField]
	private BundleDataObject bundleSnoutCoinUse;

	[HideInInspector]
	public AudioSource snoutCoinUse;

	[SerializeField]
	private BundleDataObject bundleSnoutCoinUnlock;

	[HideInInspector]
	public AudioSource snoutCoinUnlock;

	[SerializeField]
	private BundleDataObject bundleInactiveButton;

	[HideInInspector]
	public AudioSource inactiveButton;

	[SerializeField]
	private BundleDataObject bundleToggleEpisode;

	[HideInInspector]
	public AudioSource toggleEpisode;

	[SerializeField]
	private BundleDataObject bundleSnoutCoinCounterLoop;

	[HideInInspector]
	public AudioSource snoutCoinCounterLoop;

	[SerializeField]
	private BundleDataObject bundleCraftAmbience;

	[HideInInspector]
	public AudioSource craftAmbience;

	[SerializeField]
	private BundleDataObject bundleCraftEmpty;

	[HideInInspector]
	public AudioSource craftEmpty;

	[SerializeField]
	private BundleDataObject bundleCraftLeverCrank;

	[HideInInspector]
	public AudioSource craftLeverCrank;

	[SerializeField]
	private BundleDataObject[] bundleMachineIdles;

	[HideInInspector]
	public AudioSource[] machineIdles;

	[SerializeField]
	private BundleDataObject bundleMachineIntro;

	[HideInInspector]
	public AudioSource machineIntro;

	[SerializeField]
	private BundleDataObject bundleCraftPart;

	[HideInInspector]
	public AudioSource craftPart;

	[SerializeField]
	private BundleDataObject bundleEjectScrap;

	[HideInInspector]
	public AudioSource ejectScrap;

	[SerializeField]
	private BundleDataObject bundleEjectScrapButton;

	[HideInInspector]
	public AudioSource ejectScrapButton;

	[SerializeField]
	private BundleDataObject bundleInsertScrap;

	[HideInInspector]
	public AudioSource insertScrap;

	[SerializeField]
	private BundleDataObject bundleLightBulb;

	[HideInInspector]
	public AudioSource lightBulb;

	[SerializeField]
	private BundleDataObject bundlePullChain;

	[HideInInspector]
	public AudioSource pullChain;

	[SerializeField]
	private BundleDataObject bundleReleaseChain;

	[HideInInspector]
	public AudioSource releaseChain;

	[SerializeField]
	private BundleDataObject[] bundleGoldenCrateHits;

	[HideInInspector]
	public AudioSource[] goldenCrateHits;

	[SerializeField]
	private BundleDataObject[] bundleMetalCrateHits;

	[HideInInspector]
	public AudioSource[] metalCrateHits;

	[SerializeField]
	private BundleDataObject[] bundleWoodenCrateHits;

	[HideInInspector]
	public AudioSource[] woodenCrateHits;

	[SerializeField]
	private BundleDataObject bundleSalvagePart;

	[HideInInspector]
	public AudioSource salvagePart;

	[SerializeField]
	private BundleDataObject[] bundleBellowsFarts;

	[HideInInspector]
	public AudioSource[] bellowsFarts;

	[SerializeField]
	private BundleDataObject bundleAlienEngineLoop;

	[HideInInspector]
	public AudioSource alienEngineLoop;

	[SerializeField]
	private BundleDataObject bundleGiftTNTExplosion;

	[HideInInspector]
	public AudioSource giftTNTExplosion;

	[SerializeField]
	private BundleDataObject[] bundleCratePillowHit;

	[HideInInspector]
	public AudioSource[] cratePillowHit;

	[SerializeField]
	private BundleDataObject[] bundlePillowDrops;

	[HideInInspector]
	public AudioSource[] pillowDrops;

	[SerializeField]
	private BundleDataObject[] bundleNutFly;

	[HideInInspector]
	public AudioSource[] nutFly;

	[SerializeField]
	private BundleDataObject[] bundleNutHit;

	[HideInInspector]
	public AudioSource[] nutHit;

	[SerializeField]
	private BundleDataObject bundleLootcrateEpicWow;

	[HideInInspector]
	public AudioSource lootcrateEpicWow;

	[SerializeField]
	private BundleDataObject bundlePartCraftedJingle;

	[HideInInspector]
	public AudioSource partCraftedJingle;

	[SerializeField]
	private BundleDataObject[] bundleRewardBounce;

	[HideInInspector]
	public AudioSource[] rewardBounce;

	[SerializeField]
	private BundleDataObject[] bundleLootWheelTickSounds;

	[HideInInspector]
	public AudioSource[] lootWheelTickSounds;

	[SerializeField]
	private BundleDataObject bundleLootWheelJackPot;

	[HideInInspector]
	public AudioSource lootWheelJackPot;

	[SerializeField]
	private BundleDataObject bundleLootWheelPrizeFly;

	[HideInInspector]
	public AudioSource lootWheelPrizeFly;

	[SerializeField]
	private BundleDataObject bundleLootWheelStarExplosion;

	[HideInInspector]
	public AudioSource lootWheelStarExplosion;

	[SerializeField]
	private BundleDataObject bundleConfetti;

	[HideInInspector]
	public AudioSource confetti;

	[SerializeField]
	private BundleDataObject bundleRefereePigFall;

	[HideInInspector]
	public AudioSource refereePigFall;

	[SerializeField]
	private BundleDataObject bundleRefereePigFallImpact;

	[HideInInspector]
	public AudioSource refereePigFallImpact;

	[SerializeField]
	private BundleDataObject bundleRefereePigFlag;

	[HideInInspector]
	public AudioSource refereePigFlag;

	[SerializeField]
	private BundleDataObject bundleRefereePigImpact;

	[HideInInspector]
	public AudioSource refereePigImpact;

	[SerializeField]
	private BundleDataObject bundleRefereePigWhistle;

	[HideInInspector]
	public AudioSource refereePigWhistle;

	[SerializeField]
	private BundleDataObject bundleSafeDrop;

	[HideInInspector]
	public AudioSource safeDrop;

	[SerializeField]
	private BundleDataObject[] bundleScoreCountDown;

	[HideInInspector]
	public AudioSource[] scoreCountDown;

	[SerializeField]
	private BundleDataObject[] bundleScoreCountUp;

	[HideInInspector]
	public AudioSource[] scoreCountUp;

	[SerializeField]
	private BundleDataObject[] bundleWinTrumpet;

	[HideInInspector]
	public AudioSource[] winTrumpet;

	[SerializeField]
	private BundleDataObject bundleYouWinPillowAppear;

	[HideInInspector]
	public AudioSource youWinPillowAppear;

	[SerializeField]
	private BundleDataObject[] bundleXPGain;

	[HideInInspector]
	public AudioSource[] xpGain;

	[SerializeField]
	private BundleDataObject[] bundleGhostBalloonPop;

	[HideInInspector]
	public AudioSource[] ghostBalloonPop;

	[SerializeField]
	private BundleDataObject[] bundleGhostBalloonWail;

	[HideInInspector]
	public AudioSource[] ghostBalloonWail;

	[SerializeField]
	private BundleDataObject[] bundleMarbleCrateHits;

	[HideInInspector]
	public AudioSource[] marbleCrateHits;

	[SerializeField]
	private BundleDataObject[] bundleGlassCrateHits;

	[HideInInspector]
	public AudioSource[] glassCrateHits;

	[SerializeField]
	private BundleDataObject[] bundleTimeBombAlarm;

	[HideInInspector]
	public AudioSource[] timeBombAlarm;

	[SerializeField]
	private BundleDataObject[] bundleAlienLaserFire;

	[HideInInspector]
	public AudioSource[] alienLaserFire;

	[SerializeField]
	private BundleDataObject bundleAlienFan;

	[HideInInspector]
	public AudioSource alienFan;

	[SerializeField]
	private BundleDataObject bundleAlienRotor;

	[HideInInspector]
	public AudioSource alienRotor;

	[SerializeField]
	private BundleDataObject bundleJingleBell;

	[HideInInspector]
	public AudioSource jingleBell;

	[SerializeField]
	private BundleDataObject bundleAlienSodaBottleStart;

	[HideInInspector]
	public AudioSource alienSodaBottleStart;

	[SerializeField]
	private BundleDataObject bundleAlienSodaBottleLoop;

	[HideInInspector]
	public AudioSource alienSodaBottleLoop;

	[SerializeField]
	private BundleDataObject bundleAlienBeepBoop;

	[HideInInspector]
	public AudioSource alienBeepBoop;

	[SerializeField]
	private BundleDataObject[] bundleAlienLanguage;

	[HideInInspector]
	public AudioSource[] alienLanguage;

	[SerializeField]
	private BundleDataObject bundleAlienMachineReveal;

	[HideInInspector]
	public AudioSource alienMachineReveal;

	[SerializeField]
	private BundleDataObject bundleCraftingSlime;

	[HideInInspector]
	public AudioSource craftingSlime;

	[SerializeField]
	private BundleDataObject[] bundleAlienBellows;

	[HideInInspector]
	public AudioSource[] alienBellows;

	[SerializeField]
	private BundleDataObject bundleAlienPunch;

	[HideInInspector]
	public AudioSource alienPunch;

	[SerializeField]
	private BundleDataObject[] bundleScoreAnticipation;

	[HideInInspector]
	public AudioSource[] scoreAnticipation;

	[SerializeField]
	private BundleDataObject bundleScoreImpact;

	[HideInInspector]
	public AudioSource scoreImpact;

	private Dictionary<string, AudioSource> loadedSources;

	public GameObject MusicTheme => bundleMusicTheme.LoadValue<GameObject>();

	public GameObject LevelSelectionMusic => bundleLevelSelectionMusic.LoadValue<GameObject>();

	public GameObject InFlightMusic => bundleInFlightMusic.LoadValue<GameObject>();

	public GameObject BuildMusic => bundleBuildMusic.LoadValue<GameObject>();

	public GameObject FeedingMusic => bundleFeedingMusic.LoadValue<GameObject>();

	public GameObject AmbientJungle => bundleAmbientJungle.LoadValue<GameObject>();

	public GameObject AmbientPlateau => bundleAmbientPlateau.LoadValue<GameObject>();

	public GameObject AmbientCave => bundleAmbientCave.LoadValue<GameObject>();

	public GameObject AmbientNight => bundleAmbientNight.LoadValue<GameObject>();

	public GameObject AmbientMorning => bundleAmbientMorning.LoadValue<GameObject>();

	public GameObject AmbientHalloween => bundleAmbientHalloween.LoadValue<GameObject>();

	public GameObject AmbientMaya => bundleAmbientMaya.LoadValue<GameObject>();

	public GameObject CakeRaceTheme => bundleCakeRaceTheme.LoadValue<GameObject>();

	public GameObject XmasThemeSong => bundleXmasThemeSong.LoadValue<GameObject>();

	public void Initialize()
	{
		menuClick = GetAudioSource(bundleMenuClick);
		gadgetButtonClick = GetAudioSource(bundleGadgetButtonClick);
		menuHover = GetAudioSource(bundleMenuHover);
		goalBoxCollected = GetAudioSource(bundleGoalBoxCollected);
		bonusBoxCollected = GetAudioSource(bundleBonusBoxCollected);
		dessertCollected = GetAudioSource(bundleDessertCollected);
		placePart = GetAudioSource(bundlePlacePart);
		rotatePart = GetAudioSource(bundleRotatePart);
		dragPart = GetAudioSource(bundleDragPart);
		removePart = GetAudioSource(bundleRemovePart);
		clearBuildGrid = GetAudioSource(bundleClearBuildGrid);
		buildContraption = GetAudioSource(bundleBuildContraption);
		victory = GetAudioSource(bundleVictory);
		starEffects = GetAudioSource(bundleStarEffects);
		starLoops = GetAudioSource(bundleStarLoops);
		balloonPop = GetAudioSource(bundleBalloonPop);
		fan = GetAudioSource(bundleFan);
		propeller = GetAudioSource(bundlePropeller);
		motorWheelLoop = GetAudioSource(bundleMotorWheelLoop);
		normalWheelLoop = GetAudioSource(bundleNormalWheelLoop);
		smallWheelLoop = GetAudioSource(bundleSmallWheelLoop);
		woodenWheelLoop = GetAudioSource(bundleWoodenWheelLoop);
		stickyWheelLoop = GetAudioSource(bundleStickyWheelLoop);
		rotorLoop = GetAudioSource(bundleRotorLoop);
		bellowsPuff = GetAudioSource(bundleBellowsPuff);
		electricEngine = GetAudioSource(bundleElectricEngine);
		engine = GetAudioSource(bundleEngine);
		V8Engine = GetAudioSource(bundleV8Engine);
		bottleCork = GetAudioSource(bundleBottleCork);
		umbrellaOpen = GetAudioSource(bundleUmbrellaOpen);
		umbrellaClose = GetAudioSource(bundleUmbrellaClose);
		tntExplosion = GetAudioSource(bundleTntExplosion);
		sandbagCollision = GetAudioSource(bundleSandbagCollision);
		springBoxingGloveShoot = GetAudioSource(bundleSpringBoxingGloveShoot);
		springBoxingGloveWinding = GetAudioSource(bundleSpringBoxingGloveWinding);
		grapplingHookAttach = GetAudioSource(bundleGrapplingHookAttach);
		grapplingHookDetach = GetAudioSource(bundleGrapplingHookDetach);
		grapplingHookLaunch = GetAudioSource(bundleGrapplingHookLaunch);
		grapplingHookMiss = GetAudioSource(bundleGrapplingHookMiss);
		grapplingHookReeling = GetAudioSource(bundleGrapplingHookReeling);
		grapplingHookReset = GetAudioSource(bundleGrapplingHookReset);
		kickerDetach = GetAudioSource(bundleKickerDetach);
		SuperGlueApplied = GetAudioSource(bundleSuperGlueApplied);
		SuperMagnetApplied = GetAudioSource(bundleSuperMagnetApplied);
		TurboChargeApplied = GetAudioSource(bundleTurboChargeApplied);
		collisionMetalHit = GetAudioSource(bundleCollisionMetalHit);
		collisionMetalDamage = GetAudioSource(bundleCollisionMetalDamage);
		collisionMetalBreak = GetAudioSource(bundleCollisionMetalBreak);
		collisionWoodHit = GetAudioSource(bundleCollisionWoodHit);
		collisionWoodDamage = GetAudioSource(bundleCollisionWoodDamage);
		collisionWoodDestroy = GetAudioSource(bundleCollisionWoodDestroy);
		tutorialIn = GetAudioSource(bundleTutorialIn);
		tutorialOut = GetAudioSource(bundleTutorialOut);
		tutorialFlip = GetAudioSource(bundleTutorialFlip);
		cameraZoomIn = GetAudioSource(bundleCameraZoomIn);
		cameraZoomOut = GetAudioSource(bundleCameraZoomOut);
		jokerLevelUnlocked = GetAudioSource(bundleJokerLevelUnlocked);
		sandboxLevelUnlocked = GetAudioSource(bundleSandboxLevelUnlocked);
		birdWakeUp = GetAudioSource(bundleBirdWakeUp);
		birdShot = GetAudioSource(bundleBirdShot);
		slingshotStretched = GetAudioSource(bundleSlingshotStretched);
		partSlideOnIceLoop = GetAudioSource(bundlePartSlideOnIceLoop);
		motorWheelOnIceStart = GetAudioSource(bundleMotorWheelOnIceStart);
		motorWheelOnIceLoop = GetAudioSource(bundleMotorWheelOnIceLoop);
		gearboxSwitch = GetAudioSource(bundleGearboxSwitch);
		environmentFanStart = GetAudioSource(bundleEnvironmentFanStart);
		environmentFanLoop = GetAudioSource(bundleEnvironmentFanLoop);
		toggleNightVision = GetAudioSource(bundleToggleNightVision);
		toggleLamp = GetAudioSource(bundleToggleLamp);
		ancientPigDisappear = GetAudioSource(bundleAncientPigDisappear);
		goldenPigHit = GetAudioSource(bundleGoldenPigHit);
		goldenPigRoll = GetAudioSource(bundleGoldenPigRoll);
		secretStatueFound = GetAudioSource(bundleSecretStatueFound);
		pigFall = GetAudioSource(bundlePigFall);
		bridgeBreak = GetAudioSource(bundleBridgeBreak);
		pressureButtonClick = GetAudioSource(bundlePressureButtonClick);
		pressureButtonRelease = GetAudioSource(bundlePressureButtonRelease);
		collisionRockBreak = GetAudioSource(bundleCollisionRockBreak);
		snoutCoinHit = GetAudioSource(bundleSnoutCoinHit);
		snoutCoinFly = GetAudioSource(bundleSnoutCoinFly);
		snoutCoinPurchase = GetAudioSource(bundleSnoutCoinPurchase);
		snoutCoinIntro = GetAudioSource(bundleSnoutCoinIntro);
		snoutCoinUse = GetAudioSource(bundleSnoutCoinUse);
		snoutCoinUnlock = GetAudioSource(bundleSnoutCoinUnlock);
		inactiveButton = GetAudioSource(bundleInactiveButton);
		toggleEpisode = GetAudioSource(bundleToggleEpisode);
		snoutCoinCounterLoop = GetAudioSource(bundleSnoutCoinCounterLoop);
		craftAmbience = GetAudioSource(bundleCraftAmbience);
		craftEmpty = GetAudioSource(bundleCraftEmpty);
		craftLeverCrank = GetAudioSource(bundleCraftLeverCrank);
		machineIdles = GetAudioSource(bundleMachineIdles);
		machineIntro = GetAudioSource(bundleMachineIntro);
		craftPart = GetAudioSource(bundleCraftPart);
		ejectScrap = GetAudioSource(bundleEjectScrap);
		ejectScrapButton = GetAudioSource(bundleEjectScrapButton);
		insertScrap = GetAudioSource(bundleInsertScrap);
		lightBulb = GetAudioSource(bundleLightBulb);
		pullChain = GetAudioSource(bundlePullChain);
		releaseChain = GetAudioSource(bundleReleaseChain);
		goldenCrateHits = GetAudioSource(bundleGoldenCrateHits);
		metalCrateHits = GetAudioSource(bundleMetalCrateHits);
		woodenCrateHits = GetAudioSource(bundleWoodenCrateHits);
		salvagePart = GetAudioSource(bundleSalvagePart);
		bellowsFarts = GetAudioSource(bundleBellowsFarts);
		alienEngineLoop = GetAudioSource(bundleAlienEngineLoop);
		giftTNTExplosion = GetAudioSource(bundleGiftTNTExplosion);
		cratePillowHit = GetAudioSource(bundleCratePillowHit);
		pillowDrops = GetAudioSource(bundlePillowDrops);
		nutFly = GetAudioSource(bundleNutFly);
		nutHit = GetAudioSource(bundleNutHit);
		lootcrateEpicWow = GetAudioSource(bundleLootcrateEpicWow);
		partCraftedJingle = GetAudioSource(bundlePartCraftedJingle);
		rewardBounce = GetAudioSource(bundleRewardBounce);
		lootWheelTickSounds = GetAudioSource(bundleLootWheelTickSounds);
		lootWheelJackPot = GetAudioSource(bundleLootWheelJackPot);
		lootWheelPrizeFly = GetAudioSource(bundleLootWheelPrizeFly);
		lootWheelStarExplosion = GetAudioSource(bundleLootWheelStarExplosion);
		confetti = GetAudioSource(bundleConfetti);
		refereePigFall = GetAudioSource(bundleRefereePigFall);
		refereePigFallImpact = GetAudioSource(bundleRefereePigFallImpact);
		refereePigFlag = GetAudioSource(bundleRefereePigFlag);
		refereePigImpact = GetAudioSource(bundleRefereePigImpact);
		refereePigWhistle = GetAudioSource(bundleRefereePigWhistle);
		safeDrop = GetAudioSource(bundleSafeDrop);
		scoreCountDown = GetAudioSource(bundleScoreCountDown);
		scoreCountUp = GetAudioSource(bundleScoreCountUp);
		winTrumpet = GetAudioSource(bundleWinTrumpet);
		youWinPillowAppear = GetAudioSource(bundleYouWinPillowAppear);
		xpGain = GetAudioSource(bundleXPGain);
		ghostBalloonPop = GetAudioSource(bundleGhostBalloonPop);
		ghostBalloonWail = GetAudioSource(bundleGhostBalloonWail);
		marbleCrateHits = GetAudioSource(bundleMarbleCrateHits);
		glassCrateHits = GetAudioSource(bundleGlassCrateHits);
		timeBombAlarm = GetAudioSource(bundleTimeBombAlarm);
		alienLaserFire = GetAudioSource(bundleAlienLaserFire);
		alienFan = GetAudioSource(bundleAlienFan);
		alienRotor = GetAudioSource(bundleAlienRotor);
		jingleBell = GetAudioSource(bundleJingleBell);
		alienSodaBottleStart = GetAudioSource(bundleAlienSodaBottleStart);
		alienSodaBottleLoop = GetAudioSource(bundleAlienSodaBottleLoop);
		alienBeepBoop = GetAudioSource(bundleAlienBeepBoop);
		alienLanguage = GetAudioSource(bundleAlienLanguage);
		alienMachineReveal = GetAudioSource(bundleAlienMachineReveal);
		craftingSlime = GetAudioSource(bundleCraftingSlime);
		alienBellows = GetAudioSource(bundleAlienBellows);
		alienPunch = GetAudioSource(bundleAlienPunch);
		scoreAnticipation = GetAudioSource(bundleScoreAnticipation);
		scoreImpact = GetAudioSource(bundleScoreImpact);
		Bundle.UnloadAssetBundle(bundleEnvironmentFanLoop.AssetBundle, unloadAllLoadedObjects: false);
	}

	private AudioSource GetAudioSource(BundleDataObject bundleDataObject)
	{
		GameObject gameObject = bundleDataObject.LoadValue<GameObject>();
		if (gameObject != null)
		{
			AudioSource component = gameObject.GetComponent<AudioSource>();
			AddLoadedBundle(bundleDataObject, component);
			return component;
		}
		return null;
	}

	private AudioSource[] GetAudioSource(BundleDataObject[] bundleDataObject)
	{
		List<AudioSource> list = new List<AudioSource>();
		for (int i = 0; i < bundleDataObject.Length; i++)
		{
			AudioSource audioSource = GetAudioSource(bundleDataObject[i]);
			if (audioSource != null)
			{
				AddLoadedBundle(bundleDataObject[i], audioSource);
				list.Add(audioSource);
			}
		}
		return list.ToArray();
	}

	private void AddLoadedBundle(BundleDataObject bundleDataObject, AudioSource source)
	{
		if (loadedSources == null)
		{
			loadedSources = new Dictionary<string, AudioSource>();
		}
		if (!loadedSources.ContainsKey(bundleDataObject.AssetName))
		{
			loadedSources.Add(bundleDataObject.AssetName, source);
		}
	}

	public AudioSource GetLoadedAudioSource(BundleDataObject bundleDataObject)
	{
		if (loadedSources == null || !loadedSources.ContainsKey(bundleDataObject.AssetName))
		{
			return null;
		}
		return loadedSources[bundleDataObject.AssetName];
	}
}
