using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Client.Gameplay;
using Content.Client.GameTicking.Managers;
using Content.Client.Lobby;
using Content.Shared.CCVar;
using Robust.Client;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Audio
{
	// Token: 0x02000430 RID: 1072
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MusicAudioSystem : EntitySystem
	{
		// Token: 0x06001A1E RID: 6686 RVA: 0x00095628 File Offset: 0x00093828
		public override void Initialize()
		{
			base.Initialize();
			this._stationAmbience = this._prototypeManager.Index<SoundCollectionPrototype>("StationAmbienceBase");
			this._spaceAmbience = this._prototypeManager.Index<SoundCollectionPrototype>("SpaceAmbienceBase");
			this._currentCollection = this._stationAmbience;
			this._ambienceMusicEnabled = this._configManager.GetCVar<bool>(CCVars.AmbienceMusicEnabled);
			this._ambienceMusicVolume = this._configManager.GetCVar<float>(CCVars.AmbienceMusicVolume);
			this._lobbyMusicEnabled = this._configManager.GetCVar<bool>(CCVars.LobbyMusicEnabled);
			IResourceCache resourceCache = IoCManager.Resolve<IResourceCache>();
			foreach (ResourcePath resourcePath in this._spaceAmbience.PickFiles)
			{
				resourceCache.GetResource<AudioResource>(resourcePath.ToString(), true);
			}
			foreach (ResourcePath resourcePath2 in this._stationAmbience.PickFiles)
			{
				resourceCache.GetResource<AudioResource>(resourcePath2.ToString(), true);
			}
			this._configManager.OnValueChanged<float>(CCVars.AmbienceMusicVolume, new Action<float>(this.AmbienceMusicVolumeCVarChanged), false);
			this._configManager.OnValueChanged<bool>(CCVars.LobbyMusicEnabled, new Action<bool>(this.LobbyMusicCVarChanged), false);
			this._configManager.OnValueChanged<bool>(CCVars.AmbienceMusicEnabled, new Action<bool>(this.AmbienceMusicCVarChanged), false);
			base.SubscribeLocalEvent<PlayerAttachedEvent>(new EntityEventHandler<PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
			base.SubscribeLocalEvent<EntParentChangedMessage>(new EntityEventRefHandler<EntParentChangedMessage>(this.EntParentChanged), null, null);
			base.SubscribeLocalEvent<PlayerDetachedEvent>(new EntityEventHandler<PlayerDetachedEvent>(this.OnPlayerDetached), null, null);
			this._stateManager.OnStateChanged += this.StateManagerOnStateChanged;
			this._client.PlayerJoinedServer += this.OnJoin;
			this._client.PlayerLeaveServer += this.OnLeave;
			this._gameTicker.LobbyStatusUpdated += this.LobbySongReceived;
		}

		// Token: 0x06001A1F RID: 6687 RVA: 0x0009584C File Offset: 0x00093A4C
		public override void Update(float frameTime)
		{
			State currentState = this._stateManager.CurrentState;
			if (!(currentState is GameplayState))
			{
				if (!(currentState is LobbyState))
				{
					return;
				}
				if (this._lobbyStream == null || this._lobbyStream.Done)
				{
					this.StartLobbyMusic();
					return;
				}
			}
			else if (this._ambientStream == null || this._ambientStream.Done)
			{
				this.StartAmbience();
				return;
			}
		}

		// Token: 0x06001A20 RID: 6688 RVA: 0x000958B0 File Offset: 0x00093AB0
		private void OnPlayerAttached(PlayerAttachedEvent ev)
		{
			TransformComponent xform;
			if (!base.TryComp<TransformComponent>(ev.Entity, ref xform))
			{
				return;
			}
			this.CheckAmbience(xform);
		}

		// Token: 0x06001A21 RID: 6689 RVA: 0x000958D5 File Offset: 0x00093AD5
		private void OnPlayerDetached(PlayerDetachedEvent ev)
		{
			this.EndAmbience();
		}

		// Token: 0x06001A22 RID: 6690 RVA: 0x000958E0 File Offset: 0x00093AE0
		public override void Shutdown()
		{
			base.Shutdown();
			this._configManager.UnsubValueChanged<float>(CCVars.AmbienceVolume, new Action<float>(this.AmbienceMusicVolumeCVarChanged));
			this._configManager.UnsubValueChanged<bool>(CCVars.LobbyMusicEnabled, new Action<bool>(this.LobbyMusicCVarChanged));
			this._configManager.UnsubValueChanged<bool>(CCVars.AmbienceMusicEnabled, new Action<bool>(this.AmbienceMusicCVarChanged));
			this._stateManager.OnStateChanged -= this.StateManagerOnStateChanged;
			this._client.PlayerJoinedServer -= this.OnJoin;
			this._client.PlayerLeaveServer -= this.OnLeave;
			this._gameTicker.LobbyStatusUpdated -= this.LobbySongReceived;
			this.EndAmbience();
			this.EndLobbyMusic();
		}

		// Token: 0x06001A23 RID: 6691 RVA: 0x000959B0 File Offset: 0x00093BB0
		private void CheckAmbience(TransformComponent xform)
		{
			if (xform.GridUid == null)
			{
				this.ChangeAmbience(this._spaceAmbience);
				return;
			}
			if (this._currentCollection == this._stationAmbience)
			{
				return;
			}
			this.ChangeAmbience(this._stationAmbience);
		}

		// Token: 0x06001A24 RID: 6692 RVA: 0x000959F8 File Offset: 0x00093BF8
		private void EntParentChanged(ref EntParentChangedMessage message)
		{
			if (this._playMan.LocalPlayer != null)
			{
				EntityUid? controlledEntity = this._playMan.LocalPlayer.ControlledEntity;
				EntityUid entity = message.Entity;
				if (controlledEntity != null && (controlledEntity == null || !(controlledEntity.GetValueOrDefault() != entity)) && this._timing.IsFirstTimePredicted)
				{
					this.CheckAmbience(message.Transform);
					return;
				}
			}
		}

		// Token: 0x06001A25 RID: 6693 RVA: 0x00095A6C File Offset: 0x00093C6C
		private void ChangeAmbience(SoundCollectionPrototype newAmbience)
		{
			if (this._currentCollection == newAmbience)
			{
				return;
			}
			this._timerCancelTokenSource.Cancel();
			this._currentCollection = newAmbience;
			this._timerCancelTokenSource = new CancellationTokenSource();
			Timer.Spawn(1500, delegate()
			{
				if (this._playingCollection == this._currentCollection || !(this._stateManager.CurrentState is GameplayState))
				{
					return;
				}
				this.EndAmbience();
				this.StartAmbience();
			}, this._timerCancelTokenSource.Token);
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x00095AC4 File Offset: 0x00093CC4
		private void StateManagerOnStateChanged(StateChangedEventArgs args)
		{
			this.EndAmbience();
			State newState = args.NewState;
			if (!(newState is LobbyState))
			{
				if (newState is GameplayState)
				{
					this.StartAmbience();
				}
				this.EndLobbyMusic();
				return;
			}
			this.StartLobbyMusic();
		}

		// Token: 0x06001A27 RID: 6695 RVA: 0x00095B03 File Offset: 0x00093D03
		private void OnJoin([Nullable(2)] object sender, PlayerEventArgs args)
		{
			if (this._stateManager.CurrentState is LobbyState)
			{
				this.EndAmbience();
				this.StartLobbyMusic();
				return;
			}
			this.EndLobbyMusic();
			this.StartAmbience();
		}

		// Token: 0x06001A28 RID: 6696 RVA: 0x00095B30 File Offset: 0x00093D30
		private void OnLeave([Nullable(2)] object sender, PlayerEventArgs args)
		{
			this.EndAmbience();
			this.EndLobbyMusic();
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x00095B3E File Offset: 0x00093D3E
		private void AmbienceMusicVolumeCVarChanged(float volume)
		{
			this._ambienceMusicVolume = volume;
			if (this._stateManager.CurrentState is GameplayState)
			{
				this.StartAmbience();
				return;
			}
			this.EndAmbience();
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x00095B68 File Offset: 0x00093D68
		private void StartAmbience()
		{
			this.EndAmbience();
			if (this._currentCollection == null || !this._ambienceMusicEnabled)
			{
				return;
			}
			this._playingCollection = this._currentCollection;
			string text = RandomExtensions.Pick<ResourcePath>(this._robustRandom, this._currentCollection.PickFiles).ToString();
			this._ambientStream = (this._audio.PlayGlobal(text, Filter.Local(), false, new AudioParams?(this._ambientParams.WithVolume(this._ambientParams.Volume + this._ambienceMusicVolume))) as AudioSystem.PlayingStream);
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x00095BF3 File Offset: 0x00093DF3
		private void EndAmbience()
		{
			this._playingCollection = null;
			AudioSystem.PlayingStream ambientStream = this._ambientStream;
			if (ambientStream != null)
			{
				ambientStream.Stop();
			}
			this._ambientStream = null;
		}

		// Token: 0x06001A2C RID: 6700 RVA: 0x00095C14 File Offset: 0x00093E14
		private void AmbienceMusicCVarChanged(bool enabled)
		{
			this._ambienceMusicEnabled = enabled;
			if (this._currentCollection == null)
			{
				return;
			}
			if (enabled && this._stateManager.CurrentState is GameplayState)
			{
				this.StartAmbience();
				return;
			}
			this.EndAmbience();
		}

		// Token: 0x06001A2D RID: 6701 RVA: 0x00095C48 File Offset: 0x00093E48
		private void LobbyMusicCVarChanged(bool musicEnabled)
		{
			this._lobbyMusicEnabled = musicEnabled;
			if (!musicEnabled)
			{
				this.EndLobbyMusic();
				return;
			}
			if (this._stateManager.CurrentState is LobbyState)
			{
				this.StartLobbyMusic();
				return;
			}
			this.EndLobbyMusic();
		}

		// Token: 0x06001A2E RID: 6702 RVA: 0x00095C7A File Offset: 0x00093E7A
		private void LobbySongReceived()
		{
			if (this._lobbyStream != null)
			{
				return;
			}
			if (this._stateManager.CurrentState is LobbyState)
			{
				this.StartLobbyMusic();
			}
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x00095CA0 File Offset: 0x00093EA0
		public void StartLobbyMusic()
		{
			if (this._lobbyStream != null || !this._lobbyMusicEnabled)
			{
				return;
			}
			string lobbySong = this._gameTicker.LobbySong;
			if (lobbySong == null)
			{
				return;
			}
			this._lobbyStream = (this._audio.PlayGlobal(lobbySong, Filter.Local(), false, new AudioParams?(this._lobbyParams)) as AudioSystem.PlayingStream);
		}

		// Token: 0x06001A30 RID: 6704 RVA: 0x00095CF6 File Offset: 0x00093EF6
		private void EndLobbyMusic()
		{
			AudioSystem.PlayingStream lobbyStream = this._lobbyStream;
			if (lobbyStream != null)
			{
				lobbyStream.Stop();
			}
			this._lobbyStream = null;
		}

		// Token: 0x04000D41 RID: 3393
		private readonly AudioParams _ambientParams = new AudioParams(-10f, 1f, "Master", 0f, 0f, 0f, false, 0f, null);

		// Token: 0x04000D42 RID: 3394
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000D43 RID: 3395
		[Dependency]
		private readonly IBaseClient _client;

		// Token: 0x04000D44 RID: 3396
		[Dependency]
		private readonly IConfigurationManager _configManager;

		// Token: 0x04000D45 RID: 3397
		[Dependency]
		private readonly ClientGameTicker _gameTicker;

		// Token: 0x04000D46 RID: 3398
		private readonly AudioParams _lobbyParams = new AudioParams(-5f, 1f, "Master", 0f, 0f, 0f, false, 0f, null);

		// Token: 0x04000D47 RID: 3399
		[Dependency]
		private readonly IPlayerManager _playMan;

		// Token: 0x04000D48 RID: 3400
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000D49 RID: 3401
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x04000D4A RID: 3402
		[Dependency]
		private readonly IStateManager _stateManager;

		// Token: 0x04000D4B RID: 3403
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000D4C RID: 3404
		[Nullable(2)]
		private AudioSystem.PlayingStream _ambientStream;

		// Token: 0x04000D4D RID: 3405
		[Nullable(2)]
		private AudioSystem.PlayingStream _lobbyStream;

		// Token: 0x04000D4E RID: 3406
		[Nullable(2)]
		private SoundCollectionPrototype _currentCollection;

		// Token: 0x04000D4F RID: 3407
		[Nullable(2)]
		private SoundCollectionPrototype _playingCollection;

		// Token: 0x04000D50 RID: 3408
		private SoundCollectionPrototype _spaceAmbience;

		// Token: 0x04000D51 RID: 3409
		private SoundCollectionPrototype _stationAmbience;

		// Token: 0x04000D52 RID: 3410
		private CancellationTokenSource _timerCancelTokenSource = new CancellationTokenSource();

		// Token: 0x04000D53 RID: 3411
		private float _ambienceMusicVolume;

		// Token: 0x04000D54 RID: 3412
		private bool _ambienceMusicEnabled;

		// Token: 0x04000D55 RID: 3413
		private bool _lobbyMusicEnabled;
	}
}
