using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Audio;
using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Audio
{
	// Token: 0x0200042E RID: 1070
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ClientGlobalSoundSystem : SharedGlobalSoundSystem
	{
		// Token: 0x06001A12 RID: 6674 RVA: 0x0009527C File Offset: 0x0009347C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), null, null);
			base.SubscribeNetworkEvent<AdminSoundEvent>(new EntityEventHandler<AdminSoundEvent>(this.PlayAdminSound), null, null);
			this._cfg.OnValueChanged<bool>(CCVars.AdminSoundsEnabled, new Action<bool>(this.ToggleAdminSound), true);
			base.SubscribeNetworkEvent<StationEventMusicEvent>(new EntityEventHandler<StationEventMusicEvent>(this.PlayStationEventMusic), null, null);
			base.SubscribeNetworkEvent<StopStationEventMusic>(new EntityEventHandler<StopStationEventMusic>(this.StopStationEventMusic), null, null);
			this._cfg.OnValueChanged<bool>(CCVars.EventMusicEnabled, new Action<bool>(this.ToggleStationEventMusic), true);
			base.SubscribeNetworkEvent<GameGlobalSoundEvent>(new EntityEventHandler<GameGlobalSoundEvent>(this.PlayGameSound), null, null);
		}

		// Token: 0x06001A13 RID: 6675 RVA: 0x0009532D File Offset: 0x0009352D
		private void OnRoundRestart(RoundRestartCleanupEvent ev)
		{
			this.ClearAudio();
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x00095335 File Offset: 0x00093535
		public override void Shutdown()
		{
			base.Shutdown();
			this.ClearAudio();
		}

		// Token: 0x06001A15 RID: 6677 RVA: 0x00095344 File Offset: 0x00093544
		private void ClearAudio()
		{
			foreach (IPlayingAudioStream playingAudioStream in this._adminAudio)
			{
				if (playingAudioStream != null)
				{
					playingAudioStream.Stop();
				}
			}
			this._adminAudio.Clear();
			foreach (KeyValuePair<StationEventMusicType, IPlayingAudioStream> keyValuePair in this._eventAudio)
			{
				StationEventMusicType stationEventMusicType;
				IPlayingAudioStream playingAudioStream2;
				keyValuePair.Deconstruct(out stationEventMusicType, out playingAudioStream2);
				IPlayingAudioStream playingAudioStream3 = playingAudioStream2;
				if (playingAudioStream3 != null)
				{
					playingAudioStream3.Stop();
				}
			}
			this._eventAudio.Clear();
		}

		// Token: 0x06001A16 RID: 6678 RVA: 0x00095404 File Offset: 0x00093604
		private void PlayAdminSound(AdminSoundEvent soundEvent)
		{
			if (!this._adminAudioEnabled)
			{
				return;
			}
			IPlayingAudioStream item = this._audio.PlayGlobal(soundEvent.Filename, Filter.Local(), false, soundEvent.AudioParams);
			this._adminAudio.Add(item);
		}

		// Token: 0x06001A17 RID: 6679 RVA: 0x00095444 File Offset: 0x00093644
		private void PlayStationEventMusic(StationEventMusicEvent soundEvent)
		{
			if (!this._eventAudioEnabled || this._eventAudio.ContainsKey(soundEvent.Type))
			{
				return;
			}
			IPlayingAudioStream value = this._audio.PlayGlobal(soundEvent.Filename, Filter.Local(), false, soundEvent.AudioParams);
			this._eventAudio.Add(soundEvent.Type, value);
		}

		// Token: 0x06001A18 RID: 6680 RVA: 0x0009549D File Offset: 0x0009369D
		private void PlayGameSound(GameGlobalSoundEvent soundEvent)
		{
			this._audio.PlayGlobal(soundEvent.Filename, Filter.Local(), false, soundEvent.AudioParams);
		}

		// Token: 0x06001A19 RID: 6681 RVA: 0x000954C0 File Offset: 0x000936C0
		private void StopStationEventMusic(StopStationEventMusic soundEvent)
		{
			IPlayingAudioStream playingAudioStream;
			if (!this._eventAudio.TryGetValue(soundEvent.Type, out playingAudioStream))
			{
				return;
			}
			if (playingAudioStream != null)
			{
				playingAudioStream.Stop();
			}
			this._eventAudio.Remove(soundEvent.Type);
		}

		// Token: 0x06001A1A RID: 6682 RVA: 0x00095500 File Offset: 0x00093700
		private void ToggleAdminSound(bool enabled)
		{
			this._adminAudioEnabled = enabled;
			if (this._adminAudioEnabled)
			{
				return;
			}
			foreach (IPlayingAudioStream playingAudioStream in this._adminAudio)
			{
				if (playingAudioStream != null)
				{
					playingAudioStream.Stop();
				}
			}
			this._adminAudio.Clear();
		}

		// Token: 0x06001A1B RID: 6683 RVA: 0x00095574 File Offset: 0x00093774
		private void ToggleStationEventMusic(bool enabled)
		{
			this._eventAudioEnabled = enabled;
			if (this._eventAudioEnabled)
			{
				return;
			}
			foreach (KeyValuePair<StationEventMusicType, IPlayingAudioStream> keyValuePair in this._eventAudio)
			{
				IPlayingAudioStream value = keyValuePair.Value;
				if (value != null)
				{
					value.Stop();
				}
			}
			this._eventAudio.Clear();
		}

		// Token: 0x04000D3B RID: 3387
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000D3C RID: 3388
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000D3D RID: 3389
		private bool _adminAudioEnabled = true;

		// Token: 0x04000D3E RID: 3390
		[Nullable(new byte[]
		{
			1,
			2
		})]
		private List<IPlayingAudioStream> _adminAudio = new List<IPlayingAudioStream>(1);

		// Token: 0x04000D3F RID: 3391
		private bool _eventAudioEnabled = true;

		// Token: 0x04000D40 RID: 3392
		[Nullable(new byte[]
		{
			1,
			2
		})]
		private Dictionary<StationEventMusicType, IPlayingAudioStream> _eventAudio = new Dictionary<StationEventMusicType, IPlayingAudioStream>(1);
	}
}
