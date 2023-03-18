using System;
using System.Runtime.CompilerServices;
using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared.CCVar
{
	// Token: 0x02000611 RID: 1553
	[NullableContext(1)]
	[Nullable(0)]
	[CVarDefs]
	public sealed class CCVars : CVars
	{
		// Token: 0x040011C1 RID: 4545
		public static readonly CVarDef<string> ServerId = CVarDef.Create<string>("server.id", "unknown_server_id", 10, null);

		// Token: 0x040011C2 RID: 4546
		public static readonly CVarDef<string> RulesFile = CVarDef.Create<string>("server.rules_file", "Rules.txt", 10, null);

		// Token: 0x040011C3 RID: 4547
		public static readonly CVarDef<string> RulesHeader = CVarDef.Create<string>("server.rules_header", "ui-rules-header", 10, null);

		// Token: 0x040011C4 RID: 4548
		public static readonly CVarDef<bool> AmbienceBasicEnabled = CVarDef.Create<bool>("ambiance.basic_enabled", true, 144, null);

		// Token: 0x040011C5 RID: 4549
		public static readonly CVarDef<float> AmbientCooldown = CVarDef.Create<float>("ambience.cooldown", 0.1f, 144, null);

		// Token: 0x040011C6 RID: 4550
		public static readonly CVarDef<float> AmbientRange = CVarDef.Create<float>("ambience.range", 8f, 10, null);

		// Token: 0x040011C7 RID: 4551
		public static readonly CVarDef<int> MaxAmbientSources = CVarDef.Create<int>("ambience.max_sounds", 16, 144, null);

		// Token: 0x040011C8 RID: 4552
		public static readonly CVarDef<int> MinMaxAmbientSourcesConfigured = CVarDef.Create<int>("ambience.min_max_sounds_configured", 16, 11, null);

		// Token: 0x040011C9 RID: 4553
		public static readonly CVarDef<int> MaxMaxAmbientSourcesConfigured = CVarDef.Create<int>("ambience.max_max_sounds_configured", 64, 11, null);

		// Token: 0x040011CA RID: 4554
		public static readonly CVarDef<float> AmbienceVolume = CVarDef.Create<float>("ambience.volume", 0f, 144, null);

		// Token: 0x040011CB RID: 4555
		public static readonly CVarDef<float> UIVolume = CVarDef.Create<float>("ui.volume", 0f, 144, null);

		// Token: 0x040011CC RID: 4556
		public static readonly CVarDef<float> LobbyMusicVolume = CVarDef.Create<float>("ambience.lobby_music_volume", 0f, 144, null);

		// Token: 0x040011CD RID: 4557
		public static readonly CVarDef<bool> AmbienceMusicEnabled = CVarDef.Create<bool>("ambience.music", true, 144, null);

		// Token: 0x040011CE RID: 4558
		public static readonly CVarDef<float> AmbienceMusicVolume = CVarDef.Create<float>("ambience.music_volume", 0f, 144, null);

		// Token: 0x040011CF RID: 4559
		public static readonly CVarDef<string> StatusMoMMIUrl = CVarDef.Create<string>("status.mommiurl", "", 64, null);

		// Token: 0x040011D0 RID: 4560
		public static readonly CVarDef<string> StatusMoMMIPassword = CVarDef.Create<string>("status.mommipassword", "", 320, null);

		// Token: 0x040011D1 RID: 4561
		public static readonly CVarDef<bool> EventsEnabled = CVarDef.Create<bool>("events.enabled", true, 80, null);

		// Token: 0x040011D2 RID: 4562
		public static readonly CVarDef<float> EventsRampingAverageEndTime = CVarDef.Create<float>("events.ramping_average_end_time", 40f, 80, null);

		// Token: 0x040011D3 RID: 4563
		public static readonly CVarDef<float> EventsRampingAverageChaos = CVarDef.Create<float>("events.ramping_average_chaos", 6f, 80, null);

		// Token: 0x040011D4 RID: 4564
		public static readonly CVarDef<bool> GameDummyTicker = CVarDef.Create<bool>("game.dummyticker", false, 80, null);

		// Token: 0x040011D5 RID: 4565
		public static readonly CVarDef<bool> GameLobbyEnabled = CVarDef.Create<bool>("game.lobbyenabled", true, 16, null);

		// Token: 0x040011D6 RID: 4566
		public static readonly CVarDef<int> GameLobbyDuration = CVarDef.Create<int>("game.lobbyduration", 150, 16, null);

		// Token: 0x040011D7 RID: 4567
		public static readonly CVarDef<bool> GameDisallowLateJoins = CVarDef.Create<bool>("game.disallowlatejoins", false, 80, null);

		// Token: 0x040011D8 RID: 4568
		public static readonly CVarDef<string> GameLobbyDefaultPreset = CVarDef.Create<string>("game.defaultpreset", "secret", 16, null);

		// Token: 0x040011D9 RID: 4569
		public static readonly CVarDef<bool> GameLobbyFallbackEnabled = CVarDef.Create<bool>("game.fallbackenabled", true, 16, null);

		// Token: 0x040011DA RID: 4570
		public static readonly CVarDef<string> GameLobbyFallbackPreset = CVarDef.Create<string>("game.fallbackpreset", "extended", 16, null);

		// Token: 0x040011DB RID: 4571
		public static readonly CVarDef<bool> GameLobbyEnableWin = CVarDef.Create<bool>("game.enablewin", true, 16, null);

		// Token: 0x040011DC RID: 4572
		public static readonly CVarDef<int> GameMaxCharacterSlots = CVarDef.Create<int>("game.maxcharacterslots", 3, 80, null);

		// Token: 0x040011DD RID: 4573
		public static readonly CVarDef<string> GameMap = CVarDef.Create<string>("game.map", string.Empty, 64, null);

		// Token: 0x040011DE RID: 4574
		public static readonly CVarDef<string> GameMapPool = CVarDef.Create<string>("game.map_pool", "WhiteMapBox", 64, null);

		// Token: 0x040011DF RID: 4575
		public static readonly CVarDef<int> GameMapMemoryDepth = CVarDef.Create<int>("game.map_memory_depth", 16, 64, null);

		// Token: 0x040011E0 RID: 4576
		public static readonly CVarDef<bool> GameMapRotation = CVarDef.Create<bool>("game.map_rotation", true, 64, null);

		// Token: 0x040011E1 RID: 4577
		public static readonly CVarDef<bool> GameRoleTimers = CVarDef.Create<bool>("game.role_timers", true, 10, null);

		// Token: 0x040011E2 RID: 4578
		public static readonly CVarDef<bool> StationOffset = CVarDef.Create<bool>("game.station_offset", true, 0, null);

		// Token: 0x040011E3 RID: 4579
		public static readonly CVarDef<float> MaxStationOffset = CVarDef.Create<float>("game.maxstationoffset", 1000f, 0, null);

		// Token: 0x040011E4 RID: 4580
		public static readonly CVarDef<bool> StationRotation = CVarDef.Create<bool>("game.station_rotation", true, 0, null);

		// Token: 0x040011E5 RID: 4581
		public static readonly CVarDef<bool> GamePersistGuests = CVarDef.Create<bool>("game.persistguests", true, 80, null);

		// Token: 0x040011E6 RID: 4582
		public static readonly CVarDef<bool> GameDiagonalMovement = CVarDef.Create<bool>("game.diagonalmovement", true, 16, null);

		// Token: 0x040011E7 RID: 4583
		public static readonly CVarDef<int> SoftMaxPlayers = CVarDef.Create<int>("game.soft_max_players", 30, 80, null);

		// Token: 0x040011E8 RID: 4584
		public static readonly CVarDef<bool> PanicBunkerEnabled = CVarDef.Create<bool>("game.panic_bunker.enabled", false, 40, null);

		// Token: 0x040011E9 RID: 4585
		public static readonly CVarDef<bool> PanicBunkerShowReason = CVarDef.Create<bool>("game.panic_bunker.show_reason", false, 64, null);

		// Token: 0x040011EA RID: 4586
		public static readonly CVarDef<int> PanicBunkerMinAccountAge = CVarDef.Create<int>("game.panic_bunker.min_account_age", 1440, 64, null);

		// Token: 0x040011EB RID: 4587
		public static readonly CVarDef<int> PanicBunkerMinOverallHours = CVarDef.Create<int>("game.panic_bunker.min_overall_hours", 10, 64, null);

		// Token: 0x040011EC RID: 4588
		public static readonly CVarDef<bool> GameTableBonk = CVarDef.Create<bool>("game.table_bonk", false, 64, null);

		// Token: 0x040011ED RID: 4589
		public static readonly CVarDef<int> RoundStartFailShutdownCount = CVarDef.Create<int>("game.round_start_fail_shutdown_count", 5, 66, null);

		// Token: 0x040011EE RID: 4590
		public static readonly CVarDef<string> DiscordAHelpWebhook = CVarDef.Create<string>("discord.ahelp_webhook", string.Empty, 64, null);

		// Token: 0x040011EF RID: 4591
		public static readonly CVarDef<string> DiscordAHelpFooterIcon = CVarDef.Create<string>("discord.ahelp_footer_icon", string.Empty, 64, null);

		// Token: 0x040011F0 RID: 4592
		public static readonly CVarDef<string> DiscordAHelpAvatar = CVarDef.Create<string>("discord.ahelp_avatar", string.Empty, 64, null);

		// Token: 0x040011F1 RID: 4593
		public static readonly CVarDef<int> SuspicionMinPlayers = CVarDef.Create<int>("suspicion.min_players", 5, 0, null);

		// Token: 0x040011F2 RID: 4594
		public static readonly CVarDef<int> SuspicionMinTraitors = CVarDef.Create<int>("suspicion.min_traitors", 2, 0, null);

		// Token: 0x040011F3 RID: 4595
		public static readonly CVarDef<int> SuspicionPlayersPerTraitor = CVarDef.Create<int>("suspicion.players_per_traitor", 6, 0, null);

		// Token: 0x040011F4 RID: 4596
		public static readonly CVarDef<int> SuspicionStartingBalance = CVarDef.Create<int>("suspicion.starting_balance", 20, 0, null);

		// Token: 0x040011F5 RID: 4597
		public static readonly CVarDef<int> SuspicionMaxTimeSeconds = CVarDef.Create<int>("suspicion.max_time_seconds", 300, 0, null);

		// Token: 0x040011F6 RID: 4598
		public static readonly CVarDef<int> TraitorMaxTraitors = CVarDef.Create<int>("traitor.max_traitors", 12, 0, null);

		// Token: 0x040011F7 RID: 4599
		public static readonly CVarDef<int> TraitorPlayersPerTraitor = CVarDef.Create<int>("traitor.players_per_traitor", 10, 0, null);

		// Token: 0x040011F8 RID: 4600
		public static readonly CVarDef<int> TraitorCodewordCount = CVarDef.Create<int>("traitor.codeword_count", 4, 0, null);

		// Token: 0x040011F9 RID: 4601
		public static readonly CVarDef<int> TraitorStartingBalance = CVarDef.Create<int>("traitor.starting_balance", 20, 0, null);

		// Token: 0x040011FA RID: 4602
		public static readonly CVarDef<int> TraitorMaxDifficulty = CVarDef.Create<int>("traitor.max_difficulty", 5, 0, null);

		// Token: 0x040011FB RID: 4603
		public static readonly CVarDef<int> TraitorMaxPicks = CVarDef.Create<int>("traitor.max_picks", 20, 0, null);

		// Token: 0x040011FC RID: 4604
		public static readonly CVarDef<float> TraitorStartDelay = CVarDef.Create<float>("traitor.start_delay", 240f, 0, null);

		// Token: 0x040011FD RID: 4605
		public static readonly CVarDef<float> TraitorStartDelayVariance = CVarDef.Create<float>("traitor.start_delay_variance", 180f, 0, null);

		// Token: 0x040011FE RID: 4606
		public static readonly CVarDef<int> TraitorDeathMatchStartingBalance = CVarDef.Create<int>("traitordm.starting_balance", 20, 0, null);

		// Token: 0x040011FF RID: 4607
		public static readonly CVarDef<int> ZombieMaxInitialInfected = CVarDef.Create<int>("zombie.max_initial_infected", 6, 0, null);

		// Token: 0x04001200 RID: 4608
		public static readonly CVarDef<int> ZombiePlayersPerInfected = CVarDef.Create<int>("zombie.players_per_infected", 10, 0, null);

		// Token: 0x04001201 RID: 4609
		public static readonly CVarDef<int> PiratesMinPlayers = CVarDef.Create<int>("pirates.min_players", 25, 0, null);

		// Token: 0x04001202 RID: 4610
		public static readonly CVarDef<int> PiratesMaxOps = CVarDef.Create<int>("pirates.max_pirates", 6, 0, null);

		// Token: 0x04001203 RID: 4611
		public static readonly CVarDef<int> PiratesPlayersPerOp = CVarDef.Create<int>("pirates.players_per_pirate", 5, 0, null);

		// Token: 0x04001204 RID: 4612
		public static readonly CVarDef<bool> TipsEnabled = CVarDef.Create<bool>("tips.enabled", false, 0, null);

		// Token: 0x04001205 RID: 4613
		public static readonly CVarDef<string> TipsDataset = CVarDef.Create<string>("tips.dataset", "Tips", 0, null);

		// Token: 0x04001206 RID: 4614
		public static readonly CVarDef<float> TipFrequencyOutOfRound = CVarDef.Create<float>("tips.out_of_game_frequency", 90f, 0, null);

		// Token: 0x04001207 RID: 4615
		public static readonly CVarDef<float> TipFrequencyInRound = CVarDef.Create<float>("tips.in_game_frequency", 3600f, 0, null);

		// Token: 0x04001208 RID: 4616
		public static readonly CVarDef<bool> ConsoleLoginLocal = CVarDef.Create<bool>("console.loginlocal", true, 80, null);

		// Token: 0x04001209 RID: 4617
		public static readonly CVarDef<string> DatabaseEngine = CVarDef.Create<string>("database.engine", "sqlite", 64, null);

		// Token: 0x0400120A RID: 4618
		public static readonly CVarDef<string> DatabaseSqliteDbPath = CVarDef.Create<string>("database.sqlite_dbpath", "preferences.db", 64, null);

		// Token: 0x0400120B RID: 4619
		public static readonly CVarDef<int> DatabaseSqliteDelay = CVarDef.Create<int>("database.sqlite_delay", 0, 64, null);

		// Token: 0x0400120C RID: 4620
		private const int DefaultSqliteDelay = 0;

		// Token: 0x0400120D RID: 4621
		public static readonly CVarDef<string> DatabasePgHost = CVarDef.Create<string>("database.pg_host", "localhost", 64, null);

		// Token: 0x0400120E RID: 4622
		public static readonly CVarDef<int> DatabasePgPort = CVarDef.Create<int>("database.pg_port", 5432, 64, null);

		// Token: 0x0400120F RID: 4623
		public static readonly CVarDef<string> DatabasePgDatabase = CVarDef.Create<string>("database.pg_database", "ss14", 64, null);

		// Token: 0x04001210 RID: 4624
		public static readonly CVarDef<string> DatabasePgUsername = CVarDef.Create<string>("database.pg_username", "postgres", 64, null);

		// Token: 0x04001211 RID: 4625
		public static readonly CVarDef<string> DatabasePgPassword = CVarDef.Create<string>("database.pg_password", "", 320, null);

		// Token: 0x04001212 RID: 4626
		public static readonly CVarDef<bool> DatabaseSynchronous = CVarDef.Create<bool>("database.sync", false, 64, null);

		// Token: 0x04001213 RID: 4627
		public static readonly CVarDef<bool> OutlineEnabled = CVarDef.Create<bool>("outline.enabled", true, 128, null);

		// Token: 0x04001214 RID: 4628
		public static readonly CVarDef<bool> ParallaxEnabled = CVarDef.Create<bool>("parallax.enabled", true, 128, null);

		// Token: 0x04001215 RID: 4629
		public static readonly CVarDef<bool> ParallaxDebug = CVarDef.Create<bool>("parallax.debug", false, 128, null);

		// Token: 0x04001216 RID: 4630
		public static readonly CVarDef<bool> ParallaxLowQuality = CVarDef.Create<bool>("parallax.low_quality", false, 144, null);

		// Token: 0x04001217 RID: 4631
		public static readonly CVarDef<bool> RelativeMovement = CVarDef.Create<bool>("physics.relative_movement", true, 24, null);

		// Token: 0x04001218 RID: 4632
		public static readonly CVarDef<float> TileFrictionModifier = CVarDef.Create<float>("physics.tile_friction", 40f, 24, null);

		// Token: 0x04001219 RID: 4633
		public static readonly CVarDef<float> StopSpeed = CVarDef.Create<float>("physics.stop_speed", 0.1f, 24, null);

		// Token: 0x0400121A RID: 4634
		public static readonly CVarDef<bool> MobPushing = CVarDef.Create<bool>("physics.mob_pushing", false, 8, null);

		// Token: 0x0400121B RID: 4635
		public static readonly CVarDef<bool> LobbyMusicEnabled = CVarDef.Create<bool>("ambience.lobby_music_enabled", true, 144, null);

		// Token: 0x0400121C RID: 4636
		public static readonly CVarDef<bool> EventMusicEnabled = CVarDef.Create<bool>("ambience.event_music_enabled", true, 144, null);

		// Token: 0x0400121D RID: 4637
		public static readonly CVarDef<bool> RestartSoundsEnabled = CVarDef.Create<bool>("ambience.restart_sounds_enabled", true, 144, null);

		// Token: 0x0400121E RID: 4638
		public static readonly CVarDef<bool> AdminSoundsEnabled = CVarDef.Create<bool>("audio.admin_sounds_enabled", true, 144, null);

		// Token: 0x0400121F RID: 4639
		public static readonly CVarDef<string> AdminChatSoundPath = CVarDef.Create<string>("audio.admin_chat_sound_path", "/Audio/Items/pop.ogg", 536, null);

		// Token: 0x04001220 RID: 4640
		public static readonly CVarDef<float> AdminChatSoundVolume = CVarDef.Create<float>("audio.admin_chat_sound_volume", -5f, 536, null);

		// Token: 0x04001221 RID: 4641
		public static readonly CVarDef<int> HudTheme = CVarDef.Create<int>("hud.theme", 0, 144, null);

		// Token: 0x04001222 RID: 4642
		public static readonly CVarDef<bool> HudHeldItemShow = CVarDef.Create<bool>("hud.held_item_show", true, 144, null);

		// Token: 0x04001223 RID: 4643
		public static readonly CVarDef<float> HudHeldItemOffset = CVarDef.Create<float>("hud.held_item_offset", 28f, 144, null);

		// Token: 0x04001224 RID: 4644
		public static readonly CVarDef<bool> HudFpsCounterVisible = CVarDef.Create<bool>("hud.fps_counter_visible", false, 144, null);

		// Token: 0x04001225 RID: 4645
		public static readonly CVarDef<int> NPCMaxUpdates = CVarDef.Create<int>("npc.max_updates", 128, 0, null);

		// Token: 0x04001226 RID: 4646
		public static readonly CVarDef<bool> NPCEnabled = CVarDef.Create<bool>("npc.enabled", true, 0, null);

		// Token: 0x04001227 RID: 4647
		public static readonly CVarDef<bool> NPCPathfinding = CVarDef.Create<bool>("npc.pathfinding", true, 0, null);

		// Token: 0x04001228 RID: 4648
		public static readonly CVarDef<float> NetAtmosDebugOverlayTickRate = CVarDef.Create<float>("net.atmosdbgoverlaytickrate", 3f, 0, null);

		// Token: 0x04001229 RID: 4649
		public static readonly CVarDef<float> NetGasOverlayTickRate = CVarDef.Create<float>("net.gasoverlaytickrate", 3f, 0, null);

		// Token: 0x0400122A RID: 4650
		public static readonly CVarDef<int> GasOverlayThresholds = CVarDef.Create<int>("net.gasoverlaythresholds", 20, 0, null);

		// Token: 0x0400122B RID: 4651
		public static readonly CVarDef<bool> AdminAnnounceLogin = CVarDef.Create<bool>("admin.announce_login", true, 64, null);

		// Token: 0x0400122C RID: 4652
		public static readonly CVarDef<bool> AdminAnnounceLogout = CVarDef.Create<bool>("admin.announce_logout", true, 64, null);

		// Token: 0x0400122D RID: 4653
		public static readonly CVarDef<int> ExplosionTilesPerTick = CVarDef.Create<int>("explosion.tiles_per_tick", 100, 64, null);

		// Token: 0x0400122E RID: 4654
		public static readonly CVarDef<int> ExplosionThrowLimit = CVarDef.Create<int>("explosion.throw_limit", 400, 64, null);

		// Token: 0x0400122F RID: 4655
		public static readonly CVarDef<bool> ExplosionSleepNodeSys = CVarDef.Create<bool>("explosion.node_sleep", true, 64, null);

		// Token: 0x04001230 RID: 4656
		public static readonly CVarDef<int> ExplosionMaxArea = CVarDef.Create<int>("explosion.max_area", 196608, 64, null);

		// Token: 0x04001231 RID: 4657
		public static readonly CVarDef<int> ExplosionMaxIterations = CVarDef.Create<int>("explosion.max_iterations", 500, 64, null);

		// Token: 0x04001232 RID: 4658
		public static readonly CVarDef<float> ExplosionMaxProcessingTime = CVarDef.Create<float>("explosion.max_tick_time", 7f, 64, null);

		// Token: 0x04001233 RID: 4659
		public static readonly CVarDef<bool> ExplosionIncrementalTileBreaking = CVarDef.Create<bool>("explosion.incremental_tile", false, 64, null);

		// Token: 0x04001234 RID: 4660
		public static readonly CVarDef<float> ExplosionPersistence = CVarDef.Create<float>("explosion.persistence", 0.3f, 64, null);

		// Token: 0x04001235 RID: 4661
		public static readonly CVarDef<int> ExplosionSingleTickAreaLimit = CVarDef.Create<int>("explosion.single_tick_area_limit", 400, 64, null);

		// Token: 0x04001236 RID: 4662
		public static readonly CVarDef<float> RadiationMinIntensity = CVarDef.Create<float>("radiation.min_intensity", 0.1f, 64, null);

		// Token: 0x04001237 RID: 4663
		public static readonly CVarDef<float> RadiationGridcastUpdateRate = CVarDef.Create<float>("radiation.gridcast.update_rate", 1f, 64, null);

		// Token: 0x04001238 RID: 4664
		public static readonly CVarDef<bool> RadiationGridcastSimplifiedSameGrid = CVarDef.Create<bool>("radiation.gridcast.simplified_same_grid", true, 64, null);

		// Token: 0x04001239 RID: 4665
		public static readonly CVarDef<float> RadiationGridcastMaxDistance = CVarDef.Create<float>("radiation.gridcast.max_distance", 50f, 64, null);

		// Token: 0x0400123A RID: 4666
		public static readonly CVarDef<bool> AdminLogsEnabled = CVarDef.Create<bool>("adminlogs.enabled", true, 64, null);

		// Token: 0x0400123B RID: 4667
		public static readonly CVarDef<float> AdminLogsQueueSendDelay = CVarDef.Create<float>("adminlogs.queue_send_delay_seconds", 5f, 64, null);

		// Token: 0x0400123C RID: 4668
		public static readonly CVarDef<int> AdminLogsQueueMax = CVarDef.Create<int>("adminlogs.queue_max", 5000, 64, null);

		// Token: 0x0400123D RID: 4669
		public static readonly CVarDef<int> AdminLogsPreRoundQueueMax = CVarDef.Create<int>("adminlogs.pre_round_queue_max", 5000, 64, null);

		// Token: 0x0400123E RID: 4670
		public static readonly CVarDef<int> AdminLogsClientBatchSize = CVarDef.Create<int>("adminlogs.client_batch_size", 1000, 64, null);

		// Token: 0x0400123F RID: 4671
		public static readonly CVarDef<string> AdminLogsServerName = CVarDef.Create<string>("adminlogs.server_name", "unknown", 64, null);

		// Token: 0x04001240 RID: 4672
		public static readonly CVarDef<bool> SpaceWind = CVarDef.Create<bool>("atmos.space_wind", true, 64, null);

		// Token: 0x04001241 RID: 4673
		public static readonly CVarDef<float> SpaceWindPressureForceDivisorThrow = CVarDef.Create<float>("atmos.space_wind_pressure_force_divisor_throw", 30f, 64, null);

		// Token: 0x04001242 RID: 4674
		public static readonly CVarDef<float> SpaceWindPressureForceDivisorPush = CVarDef.Create<float>("atmos.space_wind_pressure_force_divisor_push", 3000f, 64, null);

		// Token: 0x04001243 RID: 4675
		public static readonly CVarDef<float> SpaceWindMaxVelocity = CVarDef.Create<float>("atmos.space_wind_max_velocity", 40f, 64, null);

		// Token: 0x04001244 RID: 4676
		public static readonly CVarDef<float> SpaceWindMaxPushForce = CVarDef.Create<float>("atmos.space_wind_max_push_force", 20f, 64, null);

		// Token: 0x04001245 RID: 4677
		public static readonly CVarDef<bool> MonstermosEqualization = CVarDef.Create<bool>("atmos.monstermos_equalization", true, 64, null);

		// Token: 0x04001246 RID: 4678
		public static readonly CVarDef<bool> MonstermosDepressurization = CVarDef.Create<bool>("atmos.monstermos_depressurization", true, 64, null);

		// Token: 0x04001247 RID: 4679
		public static readonly CVarDef<bool> MonstermosRipTiles = CVarDef.Create<bool>("atmos.monstermos_rip_tiles", true, 64, null);

		// Token: 0x04001248 RID: 4680
		public static readonly CVarDef<bool> AtmosGridImpulse = CVarDef.Create<bool>("atmos.grid_impulse", false, 64, null);

		// Token: 0x04001249 RID: 4681
		public static readonly CVarDef<bool> Superconduction = CVarDef.Create<bool>("atmos.superconduction", false, 64, null);

		// Token: 0x0400124A RID: 4682
		public static readonly CVarDef<bool> ExcitedGroups = CVarDef.Create<bool>("atmos.excited_groups", true, 64, null);

		// Token: 0x0400124B RID: 4683
		public static readonly CVarDef<bool> ExcitedGroupsSpaceIsAllConsuming = CVarDef.Create<bool>("atmos.excited_groups_space_is_all_consuming", false, 64, null);

		// Token: 0x0400124C RID: 4684
		public static readonly CVarDef<float> AtmosMaxProcessTime = CVarDef.Create<float>("atmos.max_process_time", 3f, 64, null);

		// Token: 0x0400124D RID: 4685
		public static readonly CVarDef<float> AtmosTickRate = CVarDef.Create<float>("atmos.tickrate", 15f, 64, null);

		// Token: 0x0400124E RID: 4686
		public static readonly CVarDef<int> MaxMidiEventsPerSecond = CVarDef.Create<int>("midi.max_events_per_second", 1000, 10, null);

		// Token: 0x0400124F RID: 4687
		public static readonly CVarDef<int> MaxMidiEventsPerBatch = CVarDef.Create<int>("midi.max_events_per_batch", 60, 10, null);

		// Token: 0x04001250 RID: 4688
		public static readonly CVarDef<int> MaxMidiBatchesDropped = CVarDef.Create<int>("midi.max_batches_dropped", 1, 64, null);

		// Token: 0x04001251 RID: 4689
		public static readonly CVarDef<int> MaxMidiLaggedBatches = CVarDef.Create<int>("midi.max_lagged_batches", 8, 64, null);

		// Token: 0x04001252 RID: 4690
		public static readonly CVarDef<bool> HolidaysEnabled = CVarDef.Create<bool>("holidays.enabled", true, 64, null);

		// Token: 0x04001253 RID: 4691
		public static readonly CVarDef<bool> BrandingSteam = CVarDef.Create<bool>("branding.steam", false, 128, null);

		// Token: 0x04001254 RID: 4692
		public static readonly CVarDef<int> CooldownAllMessage = CVarDef.Create<int>("white.chat.cooldown_time_all_message", 1, 64, null);

		// Token: 0x04001255 RID: 4693
		public static readonly CVarDef<bool> OocEnabled = CVarDef.Create<bool>("ooc.enabled", true, 40, null);

		// Token: 0x04001256 RID: 4694
		public static readonly CVarDef<bool> AdminOocEnabled = CVarDef.Create<bool>("ooc.enabled_admin", true, 32, null);

		// Token: 0x04001257 RID: 4695
		public static readonly CVarDef<bool> DisableHookedOOC = CVarDef.Create<bool>("ooc.disabling_ooc_disables_relay", false, 64, null);

		// Token: 0x04001258 RID: 4696
		public static readonly CVarDef<bool> OocEnableDuringRound = CVarDef.Create<bool>("ooc.enable_during_round", false, 42, null);

		// Token: 0x04001259 RID: 4697
		public static readonly CVarDef<bool> LoocEnabled = CVarDef.Create<bool>("looc.enabled", true, 40, null);

		// Token: 0x0400125A RID: 4698
		public static readonly CVarDef<bool> AdminLoocEnabled = CVarDef.Create<bool>("looc.enabled_admin", true, 32, null);

		// Token: 0x0400125B RID: 4699
		public static readonly CVarDef<int> CooldownLOOCMessage = CVarDef.Create<int>("white.looc.cooldown_time", 90, 64, null);

		// Token: 0x0400125C RID: 4700
		public static readonly CVarDef<bool> DeadLoocEnabled = CVarDef.Create<bool>("looc.enabled_dead", false, 40, null);

		// Token: 0x0400125D RID: 4701
		public static readonly CVarDef<int> EntityMenuGroupingType = CVarDef.Create<int>("entity_menu", 0, 128, null);

		// Token: 0x0400125E RID: 4702
		public static readonly CVarDef<bool> WhitelistEnabled = CVarDef.Create<bool>("whitelist.enabled", false, 64, null);

		// Token: 0x0400125F RID: 4703
		public static readonly CVarDef<string> WhitelistReason = CVarDef.Create<string>("whitelist.reason", "whitelist-not-whitelisted", 64, null);

		// Token: 0x04001260 RID: 4704
		public static readonly CVarDef<int> WhitelistMinPlayers = CVarDef.Create<int>("whitelist.min_players", 0, 64, null);

		// Token: 0x04001261 RID: 4705
		public static readonly CVarDef<int> WhitelistMaxPlayers = CVarDef.Create<int>("whitelist.max_players", int.MaxValue, 64, null);

		// Token: 0x04001262 RID: 4706
		public static readonly CVarDef<bool> VoteEnabled = CVarDef.Create<bool>("vote.enabled", true, 64, null);

		// Token: 0x04001263 RID: 4707
		public static readonly CVarDef<bool> VoteRestartEnabled = CVarDef.Create<bool>("vote.restart_enabled", true, 64, null);

		// Token: 0x04001264 RID: 4708
		public static readonly CVarDef<bool> VotePresetEnabled = CVarDef.Create<bool>("vote.preset_enabled", true, 64, null);

		// Token: 0x04001265 RID: 4709
		public static readonly CVarDef<bool> VoteMapEnabled = CVarDef.Create<bool>("vote.map_enabled", false, 64, null);

		// Token: 0x04001266 RID: 4710
		public static readonly CVarDef<float> VoteRestartRequiredRatio = CVarDef.Create<float>("vote.restart_required_ratio", 0.85f, 64, null);

		// Token: 0x04001267 RID: 4711
		public static readonly CVarDef<bool> VoteRestartNotAllowedWhenAdminOnline = CVarDef.Create<bool>("vote.restart_not_allowed_when_admin_online", true, 64, null);

		// Token: 0x04001268 RID: 4712
		public static readonly CVarDef<float> VoteSameTypeTimeout = CVarDef.Create<float>("vote.same_type_timeout", 240f, 64, null);

		// Token: 0x04001269 RID: 4713
		public static readonly CVarDef<int> VoteTimerMap = CVarDef.Create<int>("vote.timermap", 90, 64, null);

		// Token: 0x0400126A RID: 4714
		public static readonly CVarDef<int> VoteTimerRestart = CVarDef.Create<int>("vote.timerrestart", 60, 64, null);

		// Token: 0x0400126B RID: 4715
		public static readonly CVarDef<int> VoteTimerPreset = CVarDef.Create<int>("vote.timerpreset", 30, 64, null);

		// Token: 0x0400126C RID: 4716
		public static readonly CVarDef<int> VoteTimerAlone = CVarDef.Create<int>("vote.timeralone", 10, 64, null);

		// Token: 0x0400126D RID: 4717
		public static readonly CVarDef<bool> BanHardwareIds = CVarDef.Create<bool>("ban.hardware_ids", true, 64, null);

		// Token: 0x0400126E RID: 4718
		public static readonly CVarDef<bool> CameraRotationLocked = CVarDef.Create<bool>("shuttle.camera_rotation_locked", false, 8, null);

		// Token: 0x0400126F RID: 4719
		public static readonly CVarDef<bool> CargoShuttles = CVarDef.Create<bool>("shuttle.cargo", true, 64, null);

		// Token: 0x04001270 RID: 4720
		public static readonly CVarDef<bool> EmergencyEarlyLaunchAllowed = CVarDef.Create<bool>("shuttle.emergency_early_launch_allowed", false, 64, null);

		// Token: 0x04001271 RID: 4721
		public static readonly CVarDef<float> EmergencyShuttleDockTime = CVarDef.Create<float>("shuttle.emergency_dock_time", 180f, 64, null);

		// Token: 0x04001272 RID: 4722
		public static readonly CVarDef<float> EmergencyShuttleAuthorizeTime = CVarDef.Create<float>("shuttle.emergency_authorize_time", 10f, 64, null);

		// Token: 0x04001273 RID: 4723
		public static readonly CVarDef<float> EmergencyShuttleTransitTime = CVarDef.Create<float>("shuttle.emergency_transit_time", 60f, 64, null);

		// Token: 0x04001274 RID: 4724
		public static readonly CVarDef<bool> EmergencyShuttleEnabled = CVarDef.Create<bool>("shuttle.emergency_enabled", true, 64, null);

		// Token: 0x04001275 RID: 4725
		public static readonly CVarDef<float> EmergencyRecallTurningPoint = CVarDef.Create<float>("shuttle.recall_turning_point", 0.5f, 64, null);

		// Token: 0x04001276 RID: 4726
		public static readonly CVarDef<int> EmergencyShuttleAutoCallTime = CVarDef.Create<int>("shuttle.auto_call_time", 0, 64, null);

		// Token: 0x04001277 RID: 4727
		public static readonly CVarDef<int> EmergencyShuttleAutoCallExtensionTime = CVarDef.Create<int>("shuttle.auto_call_extension_time", 45, 64, null);

		// Token: 0x04001278 RID: 4728
		public static readonly CVarDef<string> CentcommMap = CVarDef.Create<string>("shuttle.centcomm_map", "/Maps/centcomm.yml", 64, null);

		// Token: 0x04001279 RID: 4729
		public static readonly CVarDef<bool> EmergencyShuttleCallEnabled = CVarDef.Create<bool>("shuttle.emergency_shuttle_call", true, 26, null);

		// Token: 0x0400127A RID: 4730
		public static readonly CVarDef<bool> CrewManifestWithoutEntity = CVarDef.Create<bool>("crewmanifest.no_entity", true, 8, null);

		// Token: 0x0400127B RID: 4731
		public static readonly CVarDef<bool> CrewManifestUnsecure = CVarDef.Create<bool>("crewmanifest.unsecure", true, 8, null);

		// Token: 0x0400127C RID: 4732
		public static readonly CVarDef<string> CrewManifestOrdering = CVarDef.Create<string>("crewmanifest.ordering", "Command,Security,Science,Medical,Engineering,Cargo,Civilian,Unknown", 8, null);

		// Token: 0x0400127D RID: 4733
		public static readonly CVarDef<bool> BiomassEasyMode = CVarDef.Create<bool>("biomass.easy_mode", true, 64, null);

		// Token: 0x0400127E RID: 4734
		public static readonly CVarDef<float> AnomalyGenerationGridBoundsScale = CVarDef.Create<float>("anomaly.generation_grid_bounds_scale", 0.6f, 64, null);

		// Token: 0x0400127F RID: 4735
		public static readonly CVarDef<bool> ViewportStretch = CVarDef.Create<bool>("viewport.stretch", true, 144, null);

		// Token: 0x04001280 RID: 4736
		public static readonly CVarDef<int> ViewportFixedScaleFactor = CVarDef.Create<int>("viewport.fixed_scale_factor", 2, 144, null);

		// Token: 0x04001281 RID: 4737
		public static readonly CVarDef<int> ViewportSnapToleranceMargin = CVarDef.Create<int>("viewport.snap_tolerance_margin", 64, 144, null);

		// Token: 0x04001282 RID: 4738
		public static readonly CVarDef<int> ViewportSnapToleranceClip = CVarDef.Create<int>("viewport.snap_tolerance_clip", 32, 144, null);

		// Token: 0x04001283 RID: 4739
		public static readonly CVarDef<bool> ViewportScaleRender = CVarDef.Create<bool>("viewport.scale_render", true, 144, null);

		// Token: 0x04001284 RID: 4740
		public static readonly CVarDef<int> ViewportMinimumWidth = CVarDef.Create<int>("viewport.minimum_width", 15, 8, null);

		// Token: 0x04001285 RID: 4741
		public static readonly CVarDef<int> ViewportMaximumWidth = CVarDef.Create<int>("viewport.maximum_width", 21, 8, null);

		// Token: 0x04001286 RID: 4742
		public static readonly CVarDef<int> ViewportWidth = CVarDef.Create<int>("viewport.width", 21, 144, null);

		// Token: 0x04001287 RID: 4743
		public static readonly CVarDef<string> UILayout = CVarDef.Create<string>("ui.layout", "Separated", 144, null);

		// Token: 0x04001288 RID: 4744
		public static readonly CVarDef<int> ChatMaxMessageLength = CVarDef.Create<int>("chat.max_message_length", 1000, 10, null);

		// Token: 0x04001289 RID: 4745
		public static readonly CVarDef<bool> ChatSanitizerEnabled = CVarDef.Create<bool>("chat.chat_sanitizer_enabled", true, 64, null);

		// Token: 0x0400128A RID: 4746
		public static readonly CVarDef<bool> ChatShowTypingIndicator = CVarDef.Create<bool>("chat.show_typing_indicator", true, 128, null);

		// Token: 0x0400128B RID: 4747
		public static readonly CVarDef<float> AfkTime = CVarDef.Create<float>("afk.time", 60f, 64, null);

		// Token: 0x0400128C RID: 4748
		public static readonly CVarDef<bool> RestrictedNames = CVarDef.Create<bool>("ic.restricted_names", true, 10, null);

		// Token: 0x0400128D RID: 4749
		public static readonly CVarDef<bool> FlavorText = CVarDef.Create<bool>("ic.flavor_text", false, 10, null);

		// Token: 0x0400128E RID: 4750
		public static readonly CVarDef<bool> ChatPunctuation = CVarDef.Create<bool>("ic.punctuation", false, 2, null);

		// Token: 0x0400128F RID: 4751
		public static readonly CVarDef<bool> ChatSlangFilter = CVarDef.Create<bool>("ic.slang_filter", true, 26, null);

		// Token: 0x04001290 RID: 4752
		public static readonly CVarDef<bool> ICNameCase = CVarDef.Create<bool>("ic.name_case", true, 10, null);

		// Token: 0x04001291 RID: 4753
		public static readonly CVarDef<string> SalvageForced = CVarDef.Create<string>("salvage.forced", "", 64, null);

		// Token: 0x04001292 RID: 4754
		public static readonly CVarDef<int> FlavorLimit = CVarDef.Create<int>("flavor.limit", 10, 64, null);

		// Token: 0x04001293 RID: 4755
		public static readonly CVarDef<bool> AutosaveEnabled = CVarDef.Create<bool>("mapping.autosave", true, 64, null);

		// Token: 0x04001294 RID: 4756
		public static readonly CVarDef<float> AutosaveInterval = CVarDef.Create<float>("mapping.autosave_interval", 600f, 64, null);

		// Token: 0x04001295 RID: 4757
		public static readonly CVarDef<string> AutosaveDirectory = CVarDef.Create<string>("mapping.autosave_dir", "Autosaves", 64, null);

		// Token: 0x04001296 RID: 4758
		public static readonly CVarDef<float> RulesWaitTime = CVarDef.Create<float>("rules.time", 45f, 10, null);

		// Token: 0x04001297 RID: 4759
		public static readonly CVarDef<bool> RulesExemptLocal = CVarDef.Create<bool>("rules.exempt_local", true, 64, null);

		// Token: 0x04001298 RID: 4760
		public static readonly CVarDef<string> DestinationFile = CVarDef.Create<string>("autogen.destination_file", "", 66, null);

		// Token: 0x04001299 RID: 4761
		public static readonly CVarDef<bool> ResourceUploadingEnabled = CVarDef.Create<bool>("netres.enabled", true, 10, null);

		// Token: 0x0400129A RID: 4762
		public static readonly CVarDef<float> ResourceUploadingLimitMb = CVarDef.Create<float>("netres.limit", 3f, 10, null);

		// Token: 0x0400129B RID: 4763
		public static readonly CVarDef<bool> ResourceUploadingStoreEnabled = CVarDef.Create<bool>("netres.store_enabled", true, 66, null);

		// Token: 0x0400129C RID: 4764
		public static readonly CVarDef<int> ResourceUploadingStoreDeletionDays = CVarDef.Create<int>("netres.store_deletion_days", 30, 66, null);

		// Token: 0x0400129D RID: 4765
		public static readonly CVarDef<float> DragDropDeadZone = CVarDef.Create<float>("control.drag_dead_zone", 12f, 144, null);

		// Token: 0x0400129E RID: 4766
		public static readonly CVarDef<float> UpdateRestartDelay = CVarDef.Create<float>("update.restart_delay", 20f, 64, null);

		// Token: 0x0400129F RID: 4767
		public static readonly CVarDef<float> GhostRoleTime = CVarDef.Create<float>("ghost.role_time", 3f, 8, null);

		// Token: 0x040012A0 RID: 4768
		public static readonly CVarDef<float> GhostRespawnTime = CVarDef.Create<float>("ghost.respawn_time", 30f, 64, null);

		// Token: 0x040012A1 RID: 4769
		public static readonly CVarDef<int> GhostRespawnMaxPlayers = CVarDef.Create<int>("ghost.respawn_max_players", 40, 64, null);

		// Token: 0x040012A2 RID: 4770
		public static readonly CVarDef<bool> FireAlarmAllAccess = CVarDef.Create<bool>("firealarm.allaccess", true, 64, null);

		// Token: 0x040012A3 RID: 4771
		public static readonly CVarDef<float> PlayTimeSaveInterval = CVarDef.Create<float>("playtime.save_interval", 900f, 64, null);

		// Token: 0x040012A4 RID: 4772
		public static readonly CVarDef<string> InfoLinksDiscord = CVarDef.Create<string>("infolinks.discord", "https://discord.gg/2WAsvv5B5v", 10, null);

		// Token: 0x040012A5 RID: 4773
		public static readonly CVarDef<string> InfoLinksForum = CVarDef.Create<string>("infolinks.forum", "", 10, null);

		// Token: 0x040012A6 RID: 4774
		public static readonly CVarDef<string> InfoLinksGithub = CVarDef.Create<string>("infolinks.github", "https://github.com/frosty-dev/ss14", 10, null);

		// Token: 0x040012A7 RID: 4775
		public static readonly CVarDef<string> InfoLinksWebsite = CVarDef.Create<string>("infolinks.website", "https://ss14.station13.ru/", 10, null);

		// Token: 0x040012A8 RID: 4776
		public static readonly CVarDef<string> InfoLinksWiki = CVarDef.Create<string>("infolinks.wiki", "https://wiki.ss14.station13.ru/", 10, null);

		// Token: 0x040012A9 RID: 4777
		public static readonly CVarDef<string> InfoLinksPatreon = CVarDef.Create<string>("infolinks.patreon", "https://boosty.to/whitedream", 10, null);

		// Token: 0x040012AA RID: 4778
		public static readonly CVarDef<string> InfoLinksBugReport = CVarDef.Create<string>("infolinks.bug_report", "", 10, null);

		// Token: 0x040012AB RID: 4779
		public static readonly CVarDef<string> SponsorsApiUrl = CVarDef.Create<string>("sponsor.api_url", "", 64, null);

		// Token: 0x040012AC RID: 4780
		public static readonly CVarDef<bool> QueueEnabled = CVarDef.Create<bool>("queue.enabled", false, 64, null);

		// Token: 0x040012AD RID: 4781
		public static readonly CVarDef<string> DiscordRoundWebhook = CVarDef.Create<string>("discord.round_webhook", string.Empty, 64, null);

		// Token: 0x040012AE RID: 4782
		public static readonly CVarDef<string> DiscordRoundRoleId = CVarDef.Create<string>("discord.round_roleid", string.Empty, 64, null);

		// Token: 0x040012AF RID: 4783
		public static readonly CVarDef<bool> DiscordRoundStartOnly = CVarDef.Create<bool>("discord.round_start_only", false, 64, null);

		// Token: 0x040012B0 RID: 4784
		public static readonly CVarDef<bool> FilmGrain = CVarDef.Create<bool>("graphics.film_grain", true, 144, null);

		// Token: 0x040012B1 RID: 4785
		public static readonly CVarDef<bool> TTSEnabled = CVarDef.Create<bool>("tts.enabled", true, 64, null);

		// Token: 0x040012B2 RID: 4786
		public static readonly CVarDef<string> TTSApiUrl = CVarDef.Create<string>("tts.api_url", "", 64, null);

		// Token: 0x040012B3 RID: 4787
		public static readonly CVarDef<float> TtsVolume = CVarDef.Create<float>("tts.volume", 0f, 144, null);

		// Token: 0x040012B4 RID: 4788
		public static readonly CVarDef<int> TTSMaxCacheSize = CVarDef.Create<int>("tts.max_cash_size", 200, 80, null);

		// Token: 0x040012B5 RID: 4789
		public static readonly CVarDef<string> ConfigPresets = CVarDef.Create<string>("config.presets", "", 64, null);

		// Token: 0x040012B6 RID: 4790
		public static readonly CVarDef<bool> ConfigPresetDevelopment = CVarDef.Create<bool>("config.preset_development", true, 64, null);

		// Token: 0x040012B7 RID: 4791
		public static readonly CVarDef<bool> ConfigPresetDebug = CVarDef.Create<bool>("config.preset_debug", true, 64, null);

		// Token: 0x040012B8 RID: 4792
		public static readonly CVarDef<string> UtkaSocketKey = CVarDef.Create<string>("utka.socket_key", "ass", 320, null);

		// Token: 0x040012B9 RID: 4793
		public static readonly CVarDef<string> StalinSalt = CVarDef.Create<string>("stalin.salt", string.Empty, 336, null);

		// Token: 0x040012BA RID: 4794
		public static readonly CVarDef<string> StalinApiUrl = CVarDef.Create<string>("stalin.api_url", string.Empty, 336, null);

		// Token: 0x040012BB RID: 4795
		public static readonly CVarDef<string> StalinAuthUrl = CVarDef.Create<string>("stalin.auth_url", string.Empty, 336, null);

		// Token: 0x040012BC RID: 4796
		public static readonly CVarDef<bool> StalinEnabled = CVarDef.Create<bool>("stalin.enabled", false, 80, null);

		// Token: 0x040012BD RID: 4797
		public static readonly CVarDef<float> StalinDiscordMinimumAge = CVarDef.Create<float>("stalin.minimal_discord_age_minutes", 30f, 80, null);

		// Token: 0x040012BE RID: 4798
		public static readonly CVarDef<bool> NonPeacefulRoundEndEnabled = CVarDef.Create<bool>("white.non_peaceful_round_end_enabled", true, 80, null);

		// Token: 0x040012BF RID: 4799
		public static readonly CVarDef<bool> MeatyOrePanelEnabled = CVarDef.Create<bool>("white.meatyore_panel_enabled", true, 26, null);

		// Token: 0x040012C0 RID: 4800
		public static readonly CVarDef<int> MeatyOreDefaultBalance = CVarDef.Create<int>("white.meatyore_default_balance", 20, 18, null);

		// Token: 0x040012C1 RID: 4801
		public static readonly CVarDef<float> BwoinkVolume = CVarDef.Create<float>("white.admin.bwoinkVolume", 0f, 144, null);

		// Token: 0x040012C2 RID: 4802
		public static readonly CVarDef<bool> FanaticXenophobiaEnabled = CVarDef.Create<bool>("white.fanatic_xenophobia", true, 80, null);
	}
}
