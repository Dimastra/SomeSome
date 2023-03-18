using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Station.Systems;
using Content.Shared.Administration;
using Content.Shared.Audio;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Audio
{
	// Token: 0x02000730 RID: 1840
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ServerGlobalSoundSystem : SharedGlobalSoundSystem
	{
		// Token: 0x06002693 RID: 9875 RVA: 0x000CBFB7 File Offset: 0x000CA1B7
		public override void Initialize()
		{
			base.Initialize();
			this._conHost.RegisterCommand("playglobalsound", Loc.GetString("play-global-sound-command-description"), Loc.GetString("play-global-sound-command-help"), new ConCommandCallback(this.PlayGlobalSoundCommand), false);
		}

		// Token: 0x06002694 RID: 9876 RVA: 0x000CBFF0 File Offset: 0x000CA1F0
		public override void Shutdown()
		{
			base.Shutdown();
			this._conHost.UnregisterCommand("playglobalsound");
		}

		// Token: 0x06002695 RID: 9877 RVA: 0x000CC008 File Offset: 0x000CA208
		private void PlayAdminGlobal(Filter playerFilter, string filename, AudioParams? audioParams = null, bool replay = true)
		{
			AdminSoundEvent msg = new AdminSoundEvent(filename, audioParams);
			base.RaiseNetworkEvent(msg, playerFilter, replay);
		}

		// Token: 0x06002696 RID: 9878 RVA: 0x000CC027 File Offset: 0x000CA227
		private Filter GetStationAndPvs(EntityUid source)
		{
			Filter inOwningStation = this._stationSystem.GetInOwningStation(source, 32f);
			inOwningStation.AddPlayersByPvs(source, 2f, this.EntityManager, null, null);
			return inOwningStation;
		}

		// Token: 0x06002697 RID: 9879 RVA: 0x000CC050 File Offset: 0x000CA250
		public void PlayGlobalOnStation(EntityUid source, string filename, AudioParams? audioParams = null)
		{
			GameGlobalSoundEvent msg = new GameGlobalSoundEvent(filename, audioParams);
			Filter filter = this.GetStationAndPvs(source);
			base.RaiseNetworkEvent(msg, filter, true);
		}

		// Token: 0x06002698 RID: 9880 RVA: 0x000CC078 File Offset: 0x000CA278
		public void StopStationEventMusic(EntityUid source, StationEventMusicType type)
		{
			StopStationEventMusic msg = new StopStationEventMusic(type);
			Filter filter = this.GetStationAndPvs(source);
			base.RaiseNetworkEvent(msg, filter, true);
		}

		// Token: 0x06002699 RID: 9881 RVA: 0x000CC0A0 File Offset: 0x000CA2A0
		public void DispatchStationEventMusic(EntityUid source, SoundSpecifier sound, StationEventMusicType type)
		{
			AudioParams audio = AudioParams.Default.WithVolume(-8f);
			StationEventMusicEvent msg = new StationEventMusicEvent(sound.GetSound(null, null), type, new AudioParams?(audio));
			Filter filter = this.GetStationAndPvs(source);
			base.RaiseNetworkEvent(msg, filter, true);
		}

		// Token: 0x0600269A RID: 9882 RVA: 0x000CC0E4 File Offset: 0x000CA2E4
		[AdminCommand(AdminFlags.Fun)]
		public void PlayGlobalSoundCommand(IConsoleShell shell, string argStr, string[] args)
		{
			AudioParams audio = AudioParams.Default;
			bool replay = true;
			int num = args.Length;
			if (num != 0)
			{
				Filter filter;
				if (num != 1)
				{
					int volume;
					if (!int.TryParse(args[1], out volume))
					{
						shell.WriteError(Loc.GetString("play-global-sound-command-volume-parse", new ValueTuple<string, object>[]
						{
							new ValueTuple<string, object>("volume", args[1])
						}));
						return;
					}
					audio = audio.WithVolume((float)volume);
					int volumeOffset = 1;
					if (args.Length == 2)
					{
						filter = Filter.Empty().AddAllPlayers(this._playerManager);
					}
					else
					{
						replay = false;
						filter = Filter.Empty();
						for (int i = 1 + volumeOffset; i < args.Length; i++)
						{
							string username = args[i];
							IPlayerSession session;
							if (!this._playerManager.TryGetSessionByUsername(username, ref session))
							{
								shell.WriteError(Loc.GetString("play-global-sound-command-player-not-found", new ValueTuple<string, object>[]
								{
									new ValueTuple<string, object>("username", username)
								}));
							}
							else
							{
								filter.AddPlayer(session);
							}
						}
					}
				}
				else
				{
					filter = Filter.Empty().AddAllPlayers(this._playerManager);
				}
				audio = audio.AddVolume(-8f);
				this.PlayAdminGlobal(filter, args[0], new AudioParams?(audio), replay);
				return;
			}
			shell.WriteLine(Loc.GetString("play-global-sound-command-help"));
		}

		// Token: 0x040017FC RID: 6140
		[Dependency]
		private readonly IConsoleHost _conHost;

		// Token: 0x040017FD RID: 6141
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040017FE RID: 6142
		[Dependency]
		private readonly StationSystem _stationSystem;
	}
}
